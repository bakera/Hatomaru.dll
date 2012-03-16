using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Web;

namespace Bakera.Hatomaru{

	/// <summary>
	/// text/html もしくは application/xhtml+xml の 200 OK を返すレスポンスです。
	/// </summary>
	public class NormalResponse: HatomaruResponse{

		private FileInfo myCacheFile;

		/// <summary>
		/// データソースとAbsPathを指定して、HatomaruResponse のインスタンスを開始します。
		/// 通常はこのコンストラクタを使用します。
		/// </summary>
		public NormalResponse(HatomaruXml source, AbsPath path) : base(source){
			myPath = path;
		}



// プロパティ


		/// <summary>
		/// このレスポンスをキャッシュして良いか示す値を設定・取得します。
		/// normalresponse はキャッシュ可能です。
		/// </summary>
		public override bool IsCacheable {
			get{return true;}
		}

		/// <summary>
		/// このインスタンスが破棄されていれば true, 使用できるなら false を返します。
		/// 既定ではfalseを返しますが、派生クラスではキャッシュデータが失われた場合に true を返すように実装します。
		/// </summary>
		public override bool IsExpired{
			get{
				// HTML があるかキャッシュファイルがあれば false
				if(Html != null) return false;
				if(myCacheFile != null){
					myCacheFile.Refresh();
					if(myCacheFile.Exists) return false;
				}
				return true;
			}
		}

// public メソッド


		/// <summary>
		/// 渡された HttpResponse にレスポンスを書き込みます。
		/// 一度書いたらファイルに保存します。
		/// </summary>
		public override void SetLastModified(){
			base.SetLastModified();
			string resultStr = Html.OuterXml;
			if(IsCacheable && Html != null){
				string cacheName = GetCacheName() + "_cache";
				string fullName = string.Format("{0}{1:X}", cacheName, resultStr.GetHashCode());
				myCacheFile = new FileInfo(fullName);
				if(myCacheFile.Exists){
					myLastModified = myCacheFile.LastWriteTime;
				} else {
					if(myCacheFile.Directory.Exists){
						string filename = System.IO.Path.GetFileName(cacheName);
						FileInfo[] delFiles = myCacheFile.Directory.GetFiles(filename +"*");
						Array.ForEach(delFiles, item=>{item.Delete();});
					}
					Html.SaveFile(myCacheFile);
					myCacheFile.LastWriteTime = myLastModified;
				}
			}
		}

		/// <summary>
		/// 渡された HttpResponse にレスポンスを書き込みます。
		/// 一度書いたらファイルに保存します。
		/// </summary>
		public override void WriteResponse(HttpResponse response){
			if(Html != null){
				string resultStr = Html.OuterXml;
				response.Write(resultStr);
				// もう一度見る
				if(myCacheFile.Exists) Html = null;
			} else if(myCacheFile != null){
				myCacheFile.Refresh();
				if(myCacheFile.Exists){
					response.WriteFile(myCacheFile.FullName);
				} else {
					Util.Throw("???");
				}
			}
			WriteResponseHeader(response);
			SetLastModified(response);
		}


// プライベートメソッド

		protected string GetCacheName(){
			string cachePath = myBaseSource.Manager.IniData.ResponseCachePath.FullName.TrimEnd('\\') + myPath.ToString();
			return cachePath;
		}


	}

}
