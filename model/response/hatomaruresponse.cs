using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Web;

namespace Bakera.Hatomaru{

	/// <summary>
	/// レスポンスデータを表す抽象クラスです。
	/// レスポンスのキャッシュにも使用されます。
	/// </summary>
	public abstract class HatomaruResponse : ICacheData{
	
		private string myContentType = XhtmlMediaType;

		protected int myStatusCode = 200;
		protected AbsPath myPath;
		private string myCharset = "UTF-8";
		private bool myDisposition;
		protected DateTime myLastModified;
		protected Xhtml myHtml;

		private string mySelfTitle; // SelfTitle | BaseTitle
		private string myBaseTitle;

		protected readonly HatomaruData myBaseSource;
		private List<HatomaruData> myDataSource = new List<HatomaruData>();
		private List<LinkItem> myTopicPath = new List<LinkItem>();

		public const string XhtmlMediaType = "application/xhtml+xml";
		public const string AtomMediaType = "application/atom+xml";
		public const string RssMediaType = "application/rss+xml";
		public const string HtmlMediaType = "text/html";


// コンストラクタ
		/// <summary>
		/// 初期設定を使用して HatomaruResponse のインスタンスを開始します。
		/// </summary>
		protected HatomaruResponse(){}

		/// <summary>
		/// データソースを指定して、HatomaruResponse のインスタンスを開始します。
		/// </summary>
		protected HatomaruResponse(HatomaruData source){
			myBaseSource = source;
			myDataSource.Add(source);
		}


// プロパティ
		/// <summary>
		/// ステータスコードを取得します。
		/// </summary>
		public int StatusCode {
			get{return myStatusCode;}
		}

		/// <summary>
		/// Htmlを設定・取得します。
		/// </summary>
		public virtual Xhtml Html{
			get{return myHtml;}
			set{myHtml = value;}
		}

		/// <summary>
		/// このレスポンスをキャッシュして良いか示す値を設定・取得します。
		/// キャッシュしても良ければ true となり、POST に対するレスポンス等では false とします。
		/// 抽象クラスの実装では false なので、true にする場合はこのプロパティをオーバーライドします。
		/// </summary>
		public virtual bool IsCacheable {
			get{return false;}
		}

		/// <summary>
		/// レスポンスのサイズを取得します。
		/// </summary>
		public virtual long Length{
			get{
				if(myHtml != null) return myHtml.OuterXml.Length;
				return 0;
			}
		}

		/// <summary>
		/// レスポンスのサイズを KB/MB 単位で取得します。
		/// </summary>
		public virtual string LengthFormat{
			get{
				long size = Length;
				if(size == 1) return "1byte";
				if(size < 1000) return string.Format("{0}bytes", size);
				if(size < 10 * 1000 * 1000) return string.Format("{0:N0}KB", size/1000);
				return string.Format("{0}MB", size/1000000);
			}
		}


		/// <summary>
		/// charset を設定・取得します。
		/// </summary>
		public string Charset {
			get{return myCharset;}
			set{myCharset = value;}
		}


		/// <summary>
		/// Content-Disposition の有無を設定・取得します。
		/// true なら Content-Disposition: attachement であることを示します。
		/// </summary>
		public bool Disposition {
			get{return myDisposition;}
			set{myDisposition = value;}
		}


		/// <summary>
		/// データソースの配列を取得します。
		/// </summary>
		public HatomaruData[] DataSource {
			get{
				HatomaruData[] result = new HatomaruData[myDataSource.Count];
				myDataSource.CopyTo(result);
				return result;
			}
		}

		/// <summary>
		/// レスポンスのメインタイトルを取得します。
		/// </summary>
		public string SelfTitle{
			get{return mySelfTitle;}
			set{mySelfTitle = value;}
		}

		/// <summary>
		/// レスポンスの親のタイトルを取得します。
		/// </summary>
		public string BaseTitle{
			get{return myBaseTitle;}
			set{myBaseTitle = value;}
		}

		/// <summary>
		/// title要素で使用するタイトルを取得します。
		/// </summary>
		public string Title{
			get{
				if(string.IsNullOrEmpty(myBaseTitle)){
					return mySelfTitle;
				}
				return mySelfTitle + " | " + myBaseTitle;
			}
		}

		/// <summary>
		/// コメントツリー等で使用するフルタイトルを取得します。
		/// </summary>
		public string FullTitle{
			get{
				if(string.IsNullOrEmpty(myBaseTitle)){
					return mySelfTitle;
				}
				return myBaseTitle + " : " + mySelfTitle;
			}
		}

		/// <summary>
		/// レスポンスの親のタイトルを取得します。
		/// </summary>
		public string Keywords{
			get; set;
		}

		/// <summary>
		/// パンくずリストを取得します。
		/// </summary>
		public LinkItem[] TopicPath{
			get{return myTopicPath.ToArray();}
		}

		/// <summary>
		/// Content-Typeの値を設定・取得します。
		/// </summary>
		public virtual string ContentType{
			get{return myContentType;}
			set{myContentType = value;}
		}


// ICacheData インターフェイスの実装

		/// <summary>
		/// このインスタンスの Last-Modified を取得します。
		/// これは、このインスタンスが生成された際にもっとも新しかったデータソースの時刻です。
		/// SetLastModified() メソッドによってセットされます。
		/// この値はミリ秒のデータを含みます (If-Modified-Since と比較の際にはミリ秒を切り落とす必要があります)。
		/// </summary>
		public DateTime LastModified{
			get{return myLastModified;}
		}

		/// <summary>
		/// このインスタンスが最新ならば true, 古ければ false を返します。
		/// このレスポンスのデータソースすべてが IsNewest = true であれば true が、そうでなければ false が返ります。
		/// </summary>
		public bool IsNewest{
			get{
				foreach(HatomaruData dat in myDataSource){
					if(!dat.IsNewest) return false;
					dat.File.Refresh();
					if(!dat.File.Exists || dat.File.LastWriteTime > LastModified){
						return false;
					}
				}
				return true;
			}
		}

		/// <summary>
		/// このインスタンスが破棄されていれば true, 使用できるなら false を返します。
		/// 既定ではfalseを返しますが、派生クラスではキャッシュデータが失われた場合に true を返すように実装します。
		/// </summary>
		public virtual bool IsExpired{
			get{
				return false;
			}
		}


		/// <summary>
		/// このインスタンスのリフレッシュを試みます。
		/// 成功すれば新しいインスタンスを、失敗すれば null を返します。
		/// </summary>
		public HatomaruResponse Refresh(){
			if(IsNewest && !IsExpired) return this;
			if(myBaseSource.IsNewest) return myBaseSource.Get(Path);
			return null;
		}



// 内部データ用プロパティ

		/// <summary>
		/// 「正しい」パスを設定・取得します。
		/// レスポンスをリフレッシュする際に使用します。
		/// 301 応答の場合、このパスをもとにリダイレクト先の URL を作成します。
		/// </summary>
		public virtual AbsPath Path {
			get{return myPath;}
			set{myPath = value;}
		}



// パブリックメソッド


		/// <summary>
		/// TopicPath に LinkItem を追加します。
		/// </summary>
		public void AddTopicPath(LinkItem item){
			myTopicPath.Add(item);
		}
		/// <summary>
		/// パスとテキストを指定して、TopicPath に LinkItem を追加します。
		/// </summary>
		public void AddTopicPath(AbsPath path, string innerText){
			AddTopicPath(new LinkItem(path, innerText));
		}

		/// <summary>
		/// このレスポンスを生成するために使用したデータソースのリストを追加します。
		/// </summary>
		public void AddDataSource(HatomaruData hd){
			if(hd == null) return;
			if(myDataSource.Contains(hd)) return;
			myDataSource.Add(hd);
		}

		/// <summary>
		/// このレスポンスを生成するために使用したデータソースのリストを追加します。
		/// </summary>
		public void AddDataSource(HatomaruData[] hd){
			if(hd == null) return;
			for(int i=0; i < hd.Length; i++){
				if(hd[i] == null) continue;
				if(myDataSource.Contains(hd[i])) continue;
				myDataSource.Add(hd[i]);
			}
		}

		/// <summary>
		/// データソースの中でもっとも新しいものの最終更新時刻を取得します。
		/// データソースがない場合は DateTime.MinValue を返します。
		/// </summary>
		public virtual DateTime GetNewestSourceTime(){
			DateTime result = DateTime.MinValue;
			foreach(HatomaruData hd in myDataSource){
				if(hd.LastModified > result) result = hd.LastModified;
			}
			return result;
		}


		/// <summary>
		/// LastModified を自動設定します。
		/// データソースの中でもっとも新しいものの最終更新時刻をセットします。
		/// </summary>
		public virtual void SetLastModified(){
			myLastModified = GetNewestSourceTime();
		}


		/// <summary>
		/// 渡された HttpResponse にレスポンスを書き込みます。
		/// </summary>
		public virtual void WriteResponse(HttpResponse response){
			WriteResponseHeader(response);
			SetLastModified(response);
		}


// プロテクトメソッド

		/// <summary>
		/// 渡された HttpResponse に基本のレスポンスヘッダを書き込みます。
		/// </summary>
		protected virtual void WriteResponseHeader(HttpResponse response){
			response.StatusCode = StatusCode;
			response.ContentType = myContentType;
			response.Charset = Charset;
		}

		/// <summary>
		/// 渡された HttpResponse に Last-Modified をセットします。
		/// </summary>
		protected virtual void SetLastModified(HttpResponse response){
			if(LastModified == default(DateTime)) return;
			try {
				response.Cache.SetLastModified(LastModified);
				response.Cache.SetCacheability(HttpCacheability.Public);
			} catch{
				response.AppendHeader("X-Last-Modified", LastModified.ToString());
			}
		}


		/// <summary>
		/// レスポンスボディ用の空の Xhtml を得ます。
		/// </summary>
		protected Xhtml GetXhtml(){
			Xhtml result = new Xhtml();
			result.Html.SetAttribute("xml:lang", "ja");
			return result;
		}



	}

}
