using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Configuration;
using System.Xml;

namespace Bakera.Hatomaru{
	
/*
	このクラスの動作メモ

基本機能
	Manager に Request を渡す
	Manager から ResponseData を受け取って Response を返す
	Request データを分析するのは Manager の仕事。

ログとり機能
	例外が発生していたらキャッチして記録する。
	Post されていたら記録する。

リクエストパスをHatomaruPageに渡す
HatomaruPageはレスポンスを生成しつつ真のURLを返す
真のURLでキャッシュする
リクエストURLと違ったらリダイレクトをキャッシュ

　注意: このインスタンスは再利用可能なので、context をキャッシュしたりしないこと
*/


	public class Handler : IHttpHandler {

		private const string IniXmlPath = "HatomaruIniXml";
		private HatomaruManager myManager = null;

		private const int RetryTime = 10;
		private const int RetryInitInterval = 5000;

		private const string ErrorPathPrefix = "/error?aspxerrorpath=";
		private const string ErrorPathChars = "\"";

// パブリックコンストラクタ

		// このインスタンスは再利用可能
		public bool IsReusable {
			get{return true;}
		}


// パブリックメソッド
		public void ProcessRequest(HttpContext context){

			if(myManager == null || myManager.IsOld){
				string iniFilePath = WebConfigurationManager.AppSettings[IniXmlPath];
				if(!File.Exists(iniFilePath)){
					throw new Exception("初期設定ファイルが見つかりません :" + iniFilePath);
				}
				myManager = new HatomaruManager(iniFilePath);
			}

			// robots.txt で 204 を返す
			if(context.Request.RawUrl.EndsWith("/robots.txt")){
				context.Response.StatusCode = 204;
				return;
			}

			// 処理開始
			myManager.Log.Clear();
			try{
				SendResponse(context);
			} catch(HatomaruXmlException e){
				string errorLog = GetError(e, context.Request);
				try{
					ThreadPool.QueueUserWorkItem(new WaitCallback(SaveLog), errorLog);
				} catch {
					// ログに保存できない例外はそのままスロー
					throw e;
				}
				SendServiceUnabailableResponse(context, errorLog);
			} catch(Exception e){
				string errorLog = GetError(e, context.Request);
				try{
					ThreadPool.QueueUserWorkItem(new WaitCallback(SaveLog), errorLog);
				} catch {
					// ログに保存できない例外はそのままスロー
					throw e;
				}
				SendInternalServerErrorResponse(context, errorLog);
			}
		}



// プライベートメソッド

		// エラーログを取得します。
		private string GetError(Exception e, HttpRequest req){
			StringBuilder error = new StringBuilder();
			error.AppendLine(DateTime.Now.ToString());
			error.AppendLine(req.UserHostAddress);
			error.AppendLine(req.HttpMethod + " " + req.RawUrl);
			foreach(string key in req.Headers.Keys){
				error.AppendLine(key + ": " + req.Headers[key]);
			}
			error.AppendLine(e.ToString());
			if(myManager != null && myManager.Log != null){
				error.AppendLine(myManager.Log.ToString());
			}
			return error.ToString();
		}


		// ログファイルに保存します。
		private void SaveLog(Object o){
			for(int i = 0; i < RetryTime; i++){
				try{
					if(myManager == null) return;
					string dateName = DateTime.Now.ToString("yyyy-MM-dd") + ".txt";
					FileInfo logfile = new FileInfo(myManager.IniData.LogPath.FullName + '\\' + dateName);
					using(FileStream fs = logfile.Open(FileMode.Append, FileAccess.Write, FileShare.None)){
						using(StreamWriter sw = new StreamWriter(fs)){
							sw.WriteLine(o.ToString());
						}
					}
					break;
				} catch(IOException){
					Thread.Sleep(RetryInitInterval + RetryInitInterval * i);
				}
			}
		}


		// レスポンスを取得し、書き出します。
		private void SendResponse(HttpContext context){
			HatomaruResponse hr = myManager.GetResponse(context.Request);
			if(hr == null) throw new Exception("レスポンスが取得できませんでした。");

			// Host:が間違っていて200系だったらリダイレクト
			// ただし localhost は除く
			if(200 <= hr.StatusCode && hr.StatusCode < 300){
				string hostValue = context.Request.Headers["Host"];
				if(!string.IsNullOrEmpty(hostValue)){
					if(!hostValue.Eq(myManager.IniData.Domain) && !hostValue.Eq("localhost")){
						hr = new RedirectResponse(hr.Path, myManager.IniData.Domain);
					}
				}
			}

			// IMS つきのリクエストに 304 を返すか?
			string ims = context.Request.Headers["If-Modified-Since"];
			if(ims != null && hr.LastModified != null){
				DateTime imsTime = GetImsTime(ims);
				DateTime responseTime = CutMilliSeconds(hr.LastModified);
				
				if(imsTime >= responseTime && responseTime != default(DateTime)){
					SendNotModifiedResponse(context);
					return;
				}
			}
			try{
				hr.WriteResponse(context.Response);
				if(hr.ContentType == HatomaruResponse.XhtmlMediaType){
					if(IsXhtmlAccept(context.Request)){
						context.Response.Cache.VaryByHeaders.AcceptTypes = true;
					} else {
						context.Response.ContentType = HatomaruResponse.HtmlMediaType;
					}
				}
				return;
			}catch(System.IO.FileNotFoundException){
				hr = null;
			}
		}

		// application/xhtml+xml を返して良いかどうかを判断します。
		private bool IsXhtmlAccept(HttpRequest r){
			if(r.AcceptTypes == null) return false;
			if(r.AcceptTypes.Length == 0) return false;
			foreach(string s in r.AcceptTypes){
				if(s.Eq(HatomaruResponse.XhtmlMediaType)){
					return true;
				}
			}
			return false;
		}


		// 304応答を返します。
		private void SendNotModifiedResponse(HttpContext context){
			context.Response.StatusCode = 304;
			return;
		}

		// 204応答を返します。
		private void SendNoContentResponse(HttpContext context){
			context.Response.StatusCode = 204;
			return;
		}

		// エラーログを指定して503応答を返します。
		private void SendServiceUnabailableResponse(HttpContext context, string errorLog){
			context.Response.StatusCode = 503;
			context.Response.Write("<!DOCTYPE HTML PUBLIC \"-//W3C//DTD HTML 4.01//EN\">");
			context.Response.Write("<html lang=\"ja\">");
			context.Response.Write("<title>Service Temporarily Unavailable | ");
			context.Response.Write(context.Server.HtmlEncode(myManager.IniData.Domain));
			context.Response.Write("</title>");
			context.Response.Write("<h1>サービス一時停止中</h1>");
			context.Response.Write("<p>現在、サービス停止中です。データを更新している最中か、あるいはメンテナンス中のため処理を一時的に停止している可能性があります。</p><p>そのうち復旧すると思いますので、しばらく待ってからまたアクセスしてみてください。</p>");
			context.Response.Write("<h2>内部状況の詳細</h2>");
			context.Response.Write("<pre>");
			context.Response.Write(context.Server.HtmlEncode(errorLog));
			context.Response.Write("</pre>");
			return;
		}

		// エラーログを指定して500応答を返します。
		private void SendInternalServerErrorResponse(HttpContext context, string errorLog){
			context.Response.StatusCode = 500;
			context.Response.ContentType = HatomaruResponse.HtmlMediaType;
			context.Response.Write("<!DOCTYPE HTML PUBLIC \"-//W3C//DTD HTML 4.01//EN\">");
			context.Response.Write("<title>エラー</title>");
			context.Response.Write("<h1>エラー</h1>");
			context.Response.Write("<p>予想外のエラーが発生したようです。サーバ内部で何か残念なことが起きています……。</p>");
			context.Response.Write("<pre>");
			context.Response.Write(context.Server.HtmlEncode(errorLog));
			context.Response.Write("</pre>");
			return;
		}


		private static DateTime GetImsTime(string imsStr){
			int index = imsStr.IndexOf(';');
			if(index > -1) imsStr = imsStr.Remove(index);
			DateTime result;
			if(DateTime.TryParse(imsStr, out result)) return result;
			return DateTime.MinValue;
		}

		private static DateTime CutMilliSeconds(DateTime sourceTime){
			return new DateTime(sourceTime.Year, sourceTime.Month, sourceTime.Day, sourceTime.Hour, sourceTime.Minute, sourceTime.Second);
		}




	} // End Class

} // End Namespace

