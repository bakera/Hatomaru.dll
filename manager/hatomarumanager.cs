using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Web;
using System.Xml;

namespace Bakera.Hatomaru{

/*
	このクラスの動作メモ


	Request を受け取る
	リクエストからパスを抽出

	応答キャッシュを探す
	キャッシュがなければファイルを探す

	ファイルの種類に応じて処理する
	XML なら HatomaruGenerator のインスタンスを取得して Get もしくは Post する。
	インスタンスがキャッシュにあればインスタンスから取得、そうでなければ作成。

	Get されたとき……有効なキャッシュがあるかどうか調べる
	あれば返す
	無ければ生成して返す
	キャッシュできるデータならキャッシュする

	Post されたとき……処理して結果を返す (Post はキャッシュしない)

	返す型はいずれも ResponseData。

※このクラスは Hatomaru のインスタンスのプライベートメンバとなる
　Hatomaru のインスタンスは Reusable で、このクラスのインスタンスも再利用される。

*/


	/// <summary>
	/// hatomaru.dll のコンテンツを管理するクラスです。
	/// URL を渡されればそれを適切に処理します。
	/// </summary>
	public class HatomaruManager{

		// データキャッシュ : XMLデータのキャッシュ
		public CacheManager<HatomaruXml> myDataCache = new CacheManager<HatomaruXml>();

		// ファイルキャッシュ : 非XMLデータのキャッシュ(特に画像サイズ取得用)
		public CacheManager<HatomaruFile> myFileCache = new CacheManager<HatomaruFile>();

		// 応答キャッシュ : レスポンスデータのキャッシュ
		public ResponseCacheManager myResponseCache;

		// Amazon Manager
		private AmazonManager myAmazonManager = null;

		// Response Title
		private AbsPathKvs myResponseTitle = null;
		private AbsPathKvs myResponseKeywords = null;

		// 初期設定
		private readonly IniData myIniData = null;
		private DateTime myIniDataTime;
		private Log myLog = new Log();
		private static readonly char[] myInvalidPathChars;
		private readonly XmlDocument myTemplate;
		private DateTime myTemplateTime;

		private readonly FileInfo myRootXmlFile;

		public const string DirectoryIndexName = "index";
		public const string DirectoryIndexExt = ".xml";
		public const string HatomaruAspxPath = "/hatomaru.aspx";


// コンストラクタ

		/// <summary>
		/// 静的コンストラクタ
		/// myInvalidPathChars の値を設定します。
		/// </summary>
		static HatomaruManager(){
			List<char> invalidChars = new List<char>();
			invalidChars.AddRange(Path.GetInvalidPathChars());
			invalidChars.Add('*');
			invalidChars.Add('?');
			myInvalidPathChars = new char[invalidChars.Count];
			invalidChars.CopyTo(myInvalidPathChars);
		}


		/// <summary>
		/// 初期設定 XML ファイルのパスを指定して、HatomaruManagerのインスタンスを開始します。
		/// </summary>
		public HatomaruManager(string iniFilePath){
			myIniData = new IniData(iniFilePath);
			myIniDataTime = myIniData.File.LastWriteTime;

			myTemplate = new XmlDocument();
			myTemplate.XmlResolver = null;
			myTemplate.Load(myIniData.Template.FullName);
			myTemplateTime = myIniData.Template.LastWriteTime;
			myResponseCache = new ResponseCacheManager();

			// ルートXMLを取得
			string rootXmlFileName = IniData.DataPath.FullName + '\\' + DirectoryIndexName + DirectoryIndexExt;
			myRootXmlFile = new FileInfo(rootXmlFileName);


			myAmazonManager = new AmazonManager(this);
			myAmazonManager.CacheDir = myIniData.AmazonCachePath;
		}


// プロパティ

		/// <summary>
		/// 初期設定データを取得します。
		/// </summary>
		public IniData IniData{
			get{return myIniData;}
		}

		/// <summary>
		/// ログオブジェクトを取得します。
		/// </summary>
		public Log Log{
			get{return myLog;}
		}

		/// <summary>
		/// ルート XML を取得します。
		/// </summary>
		public HatomaruXml RootXml{
			get{return GetData(myRootXmlFile);}
		}

		/// <summary>
		/// ルート XML のファイルを取得します。
		/// </summary>
		public FileInfo RootXmlFile{
			get{return myRootXmlFile;}
		}

		/// <summary>
		/// テンプレートのXmlDocumentを取得します。
		/// </summary>
		public XmlDocument Template{
			get{return myTemplate;}
		}

		/// <summary>
		/// コメント対象の HatomaruBbs を取得します。
		/// </summary>
		public HatomaruBbs Bbs{
			get{return GetData(IniData.CommentTo) as HatomaruBbs;}
		}

		/// <summary>
		/// HatomaruGlossary を取得します。
		/// </summary>
		public HatomaruGlossary Glossary{
			get{return GetData(IniData.Glossary) as HatomaruGlossary;}
		}

		/// <summary>
		/// HatomaruHtmlRef を取得します。
		/// </summary>
		public HatomaruHtmlRef HtmlRef{
			get{return GetData(IniData.HtmlRef) as HatomaruHtmlRef;}
		}

		/// <summary>
		/// SpamRule を取得します。
		/// </summary>
		public SpamRule SpamRule{
			get{return GetData(IniData.SpamRule) as SpamRule;}
		}

		/// <summary>
		/// AmazonManager を取得します。
		/// </summary>
		public AmazonManager AmazonManager{
			get{return myAmazonManager;}
		}

		/// <summary>
		/// Diary を取得します。
		/// </summary>
		public DiaryIndex Diary{
			get{
				return GetData(IniData.Diary) as DiaryIndex;
			}
		}

		/// <summary>
		/// このインスタンスが最新かどうかを検査し、最新であれば false を、古ければ true を返します。
		/// IniData か Template が更新されていれば true を返します。
		/// </summary>
		public bool IsOld{
			get{
				myIniData.File.Refresh();
				myIniData.Template.Refresh();
				return myIniDataTime < myIniData.File.LastWriteTime || myTemplateTime < myIniData.Template.LastWriteTime;
			}
		}

		/// <summary>
		/// レスポンスタイトルをキャッシュするAbsPathKvsを取得します。
		/// </summary>
		public AbsPathKvs ResponseTitle{
			get{
				if(myResponseTitle == null) myResponseTitle = new AbsPathKvs(myIniData.TitleCacheFile);
				return myResponseTitle;
			}
		}

		/// <summary>
		/// レスポンスキーワードをキャッシュするAbsPathKvsを取得します。
		/// </summary>
		public AbsPathKvs ResponseKeywords{
			get{
				if(myResponseKeywords == null) myResponseKeywords = new AbsPathKvs(myIniData.KeywordCacheFile);
				return myResponseKeywords;
			}
		}


// パブリックメソッド - レスポンス系

		/// <summary>
		/// リクエストから ResponseData を生成して返します。
		/// </summary>
		public HatomaruResponse GetResponse(HttpRequest req){
			if(req == null) throw new Exception("リクエストが渡されていません。");
			string method = req.HttpMethod.ToLower(System.Globalization.CultureInfo.InvariantCulture);
			switch(method){
			case "get":
			case "head":
				return Get(req);

			case "post":
				return Post(req);
			default:
				return new NotAllowedResponse("GET,POST");
			}
		}

		/// <summary>
		/// Get リクエストから ResponseData を生成して返します。
		/// </summary>
		public HatomaruResponse Get(HttpRequest r){
			string path = r.RawUrl;
			AbsPath absPath = new AbsPath(path);
			return GetResponse(absPath);
		}


		public HatomaruResponse GetResponse(AbsPath absPath){

			// 有効なレスポンスがキャッシュされていればそれを返す
			HatomaruResponse result = myResponseCache.GetEneabledCache(absPath);
			if(result != null) return result;

			// 特殊レスポンスに該当するならリダイレクトする
			if(absPath.Contains(HatomaruAspxPath)){
				result = GetHatomaruAspxRedirect(absPath);
				result.SetLastModified();
				myResponseCache.Add(absPath, result);
			}
			if(result != null) return result;

			// キャッシュがない or パスが変なので、ひとまずファイルを同定する
			FileInfo f = SingleFileSearch(absPath);
			if(f == null) throw new Exception("ファイルが発見できませんでした。");
			// 同定したファイルから HatomaruData を取得
			HatomaruData hd = null;
			ExtInfo ex = myIniData.ExtInfo[f.Extension];
			if(ex.ContentType == null){
				hd = GetData(f);
			} else {
				hd = GetFile(f);
			}
			if(hd == null) throw new Exception("データオブジェクトの取得に失敗しました。処理できないデータ形式かもしれません。");
			HatomaruResponse res = hd.Get(absPath);
			res.SetLastModified();
			myResponseCache.Add(absPath, res);
			// リダイレクトの無限ループチェック
			if(res is RedirectResponse){
				RedirectResponse rr = res as RedirectResponse;
				if(absPath == rr.DestPath) throw new Exception("無限リダイレクト警告 : リクエストされたパス自身にリダイレクトする応答を返そうとしました。");
			}

			// タイトルとキーワードをキャッシュ
			if(!string.IsNullOrEmpty(res.Title)) ResponseTitle.Add(absPath, res.FullTitle);
			if(!string.IsNullOrEmpty(res.Keywords)) ResponseKeywords.Add(absPath, res.Keywords);
			return res;
		}


		/// <summary>
		/// Post リクエストから ResponseData を生成して返します。
		/// </summary>
		public HatomaruResponse Post(HttpRequest r){
			AbsPath absPath = new AbsPath(r.RawUrl);

			// ひとまずファイルを同定する
			AbsPath filePath = new AbsPath(r.Path);
			FileInfo f = SingleFileSearch(filePath);
			if(f == null) throw new Exception("ファイルが発見できませんでした。");
			Log.Add("{0} : ファイルを同定しました : {1}", absPath, f.FullName);

			// 同定したファイルから HatomaruData を取得
			HatomaruData hd = null;
			ExtInfo ex = myIniData.ExtInfo[f.Extension];
			if(ex.ContentType == null){
				hd = GetData(f);
			} else {
				hd = GetFile(f);
			}
			if(hd == null) throw new Exception("データオブジェクトの取得に失敗しました。処理できないデータ形式かもしれません。");
			return hd.Post(absPath, r);
		}


		/// <summary>
		/// 渡された AbsPath に相当するコンテンツのタイトルを取得します。
		/// </summary>
		public string GetResponseTitle(AbsPath absPath){
			string result = ResponseTitle[absPath];
			if(result != null) return result;
			HatomaruResponse hx = GetResponse(absPath);
			return hx.Title;
		}


		/// <summary>
		/// 渡された AbsPath に相当するコンテンツのキーワードを取得します。
		/// </summary>
		public string GetResponseKeywords(AbsPath absPath){
			string result = ResponseKeywords[absPath];
			if(result != null) return result;
			HatomaruResponse hx = GetResponse(absPath);
			return hx.Keywords;
		}



// パブリックメソッド - 補助系

		/// <summary>
		/// 渡された FileInfo に相当する HatomaruXml データを取得します。
		/// </summary>
		public HatomaruXml GetData(FileInfo f){
			if(f == null) return null;
			// キャッシュにあるのか?
			HatomaruXml result = myDataCache.GetEneabledCache(f.FullName);
			if(result != null){
				Log.Add("Cached : {0}", f.FullName);
				return result;
			}
			result = HatomaruXml.GetHatomaruXml(this, f);
			if(result != null){
				myDataCache.Add(f.FullName, result);
				Log.Add("Loaded : {0} {1}", f.FullName, result.GetType());
				return result;
			}
			Log.Add("Not Found : {0}", f.FullName);
			return null;
		}

		/// <summary>
		/// 渡された FileInfo に相当する HatomaruFile データを取得します。
		/// </summary>
		public HatomaruFile GetFile(FileInfo f){
			// キャッシュにあるのか?
			HatomaruFile result = myFileCache.GetEneabledCache(f.FullName);
			if(result != null){
				Log.Add("Cached : {0}", f.FullName);
				return result;
			}

			result = new HatomaruFile(this, f);
			if(result == null){
				Log.Add("Not Found : {0}", f.FullName);
				return null;
			}

			if(result.Ext == null){
				Log.Add("Unknown Extension : {0}", f.FullName);
				return null;
			}

			myFileCache.Add(f.FullName, result);
			Log.Add("Loaded : {0}", f.FullName);
			return result;
		}


		/// <summary>
		/// キャッシュされている HatomaruResponse をすべて取得します。
		/// </summary>
		public HatomaruResponse[] GetCachedResponse(){
			return myResponseCache.GetAllData();;
		}


		/// <summary>
		/// パスから単一のファイルを捜索します。
		/// </summary>
		public FileInfo SingleFileSearch(AbsPath path){
			List<FileInfo> files = FileSearch(path);
			if(files.Count == 0) return null;
			if(files.Count == 1) return files[0];
			files.Sort((x, y) => x.Name.Length - y.Name.Length);
			return files[0];
		}




// プライベートメソッド


		// ファイルを捜索します。
		private List<FileInfo> FileSearch(AbsPath absPath){
			Log.Add("FileSearch : {0}", absPath);
			string basePath = myIniData.DataPath.FullName;
			List<FileInfo> result = new List<FileInfo>();
			string path = absPath.RemoveQuery().ToString();

			// 連続する .. や // はまとめる
			while(path.IndexOf("..") >= 0){
				path = path.Replace("..", ".");
			}
			while(path.IndexOf("//") >= 0){
				path = path.Replace("//", "/");
			}
			while(path.IndexOf(":") >= 0){
				path = path.Replace(":", "");
			}
			while(path.IndexOf(")") >= 0){
				path = path.Replace(")", "");
			}
			while(path.IndexOf("(") >= 0){
				path = path.Replace("(", "");
			}

			for(;;){
				string searchPath = basePath + path;
				if(searchPath.IndexOfAny(myInvalidPathChars) < 0){
					string lastName = null;
					DirectoryInfo currentDir = null;

					// ディレクトリですか?
					if(Directory.Exists(searchPath)){
						currentDir = new DirectoryInfo(searchPath);
						lastName = DirectoryIndexName;
					} else {
						lastName = Util.GetLastName(path);
						FileInfo f = new FileInfo(searchPath);
						currentDir = f.Directory;
					}

					if(currentDir.Exists){
						FileInfo[] files = currentDir.GetFiles(lastName + "*");
						foreach(FileInfo f in files){
							string ext = f.Extension;
							if(myIniData.ExtInfo.ContainsKey(ext)){
								result.Add(f);
							}
						}
						if(result.Count > 0) return result;
					}
				}
				if(path == "/") break;
				path = Util.ShortenPath(path);
			}
			return result;
		} // End FileSearch()




// 特殊レスポンス

	// hatomaru.aspx を含む URL をリダイレクトする
		private RedirectResponse GetHatomaruAspxRedirect(AbsPath path){
			AbsPath destPath = path.Remove(HatomaruAspxPath);
			RedirectResponse red = new RedirectResponse(destPath, IniData.Domain, path);
			return red;
		}


	} // End Class HatomaruManager

} // End Namespace 
