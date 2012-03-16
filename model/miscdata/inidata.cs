using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace Bakera.Hatomaru{
	
	
/// <summary>
/// 鳩丸システム情報
/// </summary>
	public class IniData{
		// ini.xml のディレクトリ
		private readonly FileInfo myFile;

		private DirectoryInfo myDataPath;
		private string myImageDir;
		private DirectoryInfo myLogPath;
		private DirectoryInfo myAccessLogPath;
		private DirectoryInfo myAmazonCachePath;
		private DirectoryInfo myResponseCachePath;
		private FileInfo myTitleCacheFile;
		private FileInfo myKeywordCacheFile;
		private FileInfo myTemplate;
		private FileInfo myCommentTo;
		private FileInfo myGlossary;
		private FileInfo myHtmlRef;
		private FileInfo mySpamRule;
		private FileInfo myDiary;
		private string myDomain;

		public const string DataPathName = "docs";
		public const string ImageDirName = "imgdir";
		public const string LogPathName = "log";
		public const string AmazonCachePathName = "amazonCache";
		public const string ResponseCachePathName = "responseCache";
		public const string AccessLogPathName = "accessLog";
		public const string TitleCacheFileName = "titleCache";
		public const string KeywordCacheFileName = "keywordCache";
		public const string IPAddrToCCTLDFileName = "IPAddrToCCTLD";
		public const string TemplateFileName = "template";
		public const string DomainName = "domain";
		public const string CommentToName = "commentto";
		public const string GlossaryName = "glossary";
		public const string SpamRuleName = "spamrule";

		public const string AmazonWsAccessKeyIdName = "amazonWsAccessKeyId";
		public const string AmazonWsSecretKeyName = "amazonWsSecretKey";
		public const string AmazonServiceHostNameName = "amazonServiceHostName";
		public const string AmazonAssociateTagName = "amazonAssociateTag";

		public const string DiaryName = "diary";
		public const string HtmlRefName = "htmlref";
		public const string ExtensionInfo = "ext";
		public const string ExtensionInfoBase = "extinfo";

		private Dictionary<string, ExtInfo> myExtInfo = new Dictionary<string, ExtInfo>();
		private Dictionary<string, string> myData = new Dictionary<string, string>();


// コンストラクタ
		public IniData(string filename){
			myFile = new FileInfo(filename);
			Load(filename);
		}


// プロパティ
		/// <summary>
		/// この IniData のファィルを取得します。
		/// </summary>
		public FileInfo File{
			get{return myFile;}
		}


		/// <summary>
		/// 初期設定ファイルで設定された、拡張子情報のリストを取得します。
		/// </summary>
		public Dictionary<string, ExtInfo> ExtInfo{
			get{return myExtInfo;}
		}

		/// <summary>
		/// 初期設定ファイルで設定された、ドキュメント配置ディレクトリの DirectoryInfo を取得します。
		/// </summary>
		public DirectoryInfo DataPath{
			get{return myDataPath;}
		}

		/// <summary>
		/// 初期設定ファイルで設定された、画像配置ディレクトリのパス文字列を取得します。
		/// </summary>
		public string ImageDir{
			get{return myImageDir;}
		}

		/// <summary>
		/// 初期設定ファイルで設定された、ログ保存ディレクトリの DirectoryInfo を取得します。
		/// </summary>
		public DirectoryInfo LogPath{
			get{return myLogPath;}
		}

		/// <summary>
		/// 初期設定ファイルで設定された、アクセスログ保存ディレクトリの DirectoryInfo を取得します。
		/// </summary>
		public DirectoryInfo AccessLogPath{
			get{return myAccessLogPath;}
		}

		/// <summary>
		/// 初期設定ファイルで設定された、Amazonキャッシュ保存ディレクトリの DirectoryInfo を取得します。
		/// </summary>
		public DirectoryInfo AmazonCachePath{
			get{return myAmazonCachePath;}
		}

		/// <summary>
		/// 初期設定ファイルで設定された、レスポンスキャッシュ保存ディレクトリの DirectoryInfo を取得します。
		/// </summary>
		public DirectoryInfo ResponseCachePath{
			get{return myResponseCachePath;}
		}

		/// <summary>
		/// 初期設定ファイルで設定された、ページタイトルキャッシュ保存ファイルの FileInfo を取得します。
		/// </summary>
		public FileInfo TitleCacheFile{
			get{return myTitleCacheFile;}
		}

		/// <summary>
		/// 初期設定ファイルで設定された、キーワードキャッシュ保存ファイルの FileInfo を取得します。
		/// </summary>
		public FileInfo KeywordCacheFile{
			get{return myKeywordCacheFile;}
		}

		/// <summary>
		/// 初期設定ファイルで設定された、IPAddrToCCTLDファイルの FileInfo を取得します。
		/// </summary>
		public FileInfo IPAddrToCCTLDFile{
			private set; get;
		}

		/// <summary>
		/// 初期設定ファイルで設定された、テンプレートの FileInfo を取得します。
		/// </summary>
		public FileInfo Template{
			get{return myTemplate;}
		}

		/// <summary>
		/// 初期設定ファイルで設定された、コメント投稿対象の HatomaruBbs を示す FileInfo を取得します。
		/// </summary>
		public FileInfo CommentTo{
			get{return myCommentTo;}
		}

		/// <summary>
		/// 初期設定ファイルで設定された、HatomaruGlossary を示す FileInfo を取得します。
		/// </summary>
		public FileInfo Glossary{
			get{return myGlossary;}
		}

		/// <summary>
		/// 初期設定ファイルで設定された、HatomaruHtmlRef を示す FileInfo を取得します。
		/// </summary>
		public FileInfo HtmlRef{
			get{return myHtmlRef;}
		}

		/// <summary>
		/// 初期設定ファイルで設定された、スパム投稿処理ルールを示す FileInfo を取得します。
		/// </summary>
		public FileInfo SpamRule{
			get{return mySpamRule;}
		}

		/// <summary>
		/// 初期設定ファイルで設定された、日記を示す FileInfo を取得します。
		/// </summary>
		public FileInfo Diary{
			get{return myDiary;}
		}

		/// <summary>
		/// 初期設定ファイルで設定されたドメインの情報を取得します。
		/// </summary>
		public string Domain{
			get{return myDomain;}
		}

		/// <summary>
		/// 初期設定ファイルで設定されたamazonWsAccessKeyIdの情報を取得します。
		/// </summary>
		public string AmazonWsAccessKeyId{
			get;
			private set;
		}

		/// <summary>
		/// 初期設定ファイルで設定されたamazonWsSecretKeyの情報を取得します。
		/// </summary>
		public string AmazonWsSecretKey{
			get;
			private set;
		}

		/// <summary>
		/// 初期設定ファイルで設定されたamazonServiceHostNameの情報を取得します。
		/// </summary>
		public string AmazonServiceHostName{
			get;
			private set;
		}

		/// <summary>
		/// 初期設定ファイルで設定されたamazonAssociateTagの情報を取得します。
		/// </summary>
		public string AmazonAssociateTag{
			get;
			private set;
		}



// パブリックメソッド


		/// <summary>
		/// 指定された XML ファイルからデータをロードします。
		/// この処理はスレッドセーフではありません。
		/// </summary>
		public void Load(string filename){
			XmlDocument doc = new XmlDocument();
			doc.Load(filename);
			LoadProperties(doc);
			LoadExtInfo(doc);
		}


		// XML データからプロパティを取得します。
		private void LoadProperties(XmlDocument doc){
			myDataPath = LoadDirectory(doc, DataPathName);
			myLogPath = LoadDirectory(doc, LogPathName);
			myImageDir = LoadString(doc, ImageDirName);
			myResponseCachePath = LoadDirectory(doc, ResponseCachePathName);
			myAccessLogPath = LoadDirectory(doc, AccessLogPathName);
			myTitleCacheFile = LoadFile(doc, TitleCacheFileName, false);
			myKeywordCacheFile = LoadFile(doc, KeywordCacheFileName, false);
			IPAddrToCCTLDFile = LoadFile(doc, IPAddrToCCTLDFileName, false);
			myTemplate = LoadFile(doc, TemplateFileName);
			myDomain = LoadString(doc, DomainName);
			myCommentTo = LoadFile(doc, CommentToName);
			myGlossary = LoadFile(doc, GlossaryName);
			myHtmlRef = LoadFile(doc, HtmlRefName);
			mySpamRule = LoadFile(doc, SpamRuleName);
			myDiary = LoadFile(doc, DiaryName);

			myAmazonCachePath = LoadDirectory(doc, AmazonCachePathName);

			AmazonWsAccessKeyId = LoadString(doc, AmazonWsAccessKeyIdName);
			AmazonWsSecretKey = LoadString(doc, AmazonWsSecretKeyName);
			AmazonServiceHostName = LoadString(doc, AmazonServiceHostNameName);
			AmazonAssociateTag = LoadString(doc, AmazonAssociateTagName);

		}

		// XML データからディレクトリ情報を取得します。
		// パスは ini.xml からの相対パスとして解釈します。
		private DirectoryInfo LoadDirectory(XmlDocument doc, string elementName){
			string dir = LoadString(doc, elementName);
			if(dir == null) return null;
			string fullpath = Path.Combine(myFile.DirectoryName, dir);
			DirectoryInfo result = new DirectoryInfo(fullpath);
			if(!result.Exists) throw new Exception("設定ファイルで " +  elementName + " の値として指定されたディレクトリ " + fullpath + "がみつかりませんでした。");
			return result;
		}

		// XML データから FileInfo 情報を取得します。
		// パスは ini.xml からの相対パスとして解釈します。
		private FileInfo LoadFile(XmlDocument doc, string elementName, bool fileRequired){
			string file = LoadString(doc, elementName);
			if(file == null) return null;
			string fullpath = Path.Combine(myFile.DirectoryName, file);
			FileInfo result = new FileInfo(fullpath);
			if(fileRequired && !result.Exists) throw new Exception("設定ファイルで " +  elementName + " の値として指定されたファイル" + fullpath + "がみつかりませんでした。");
			return result;
		}
		// XML データから FileInfo 情報を取得します。
		// パスは ini.xml からの相対パスとして解釈します。
		private FileInfo LoadFile(XmlDocument doc, string elementName){
			return LoadFile(doc, elementName, true);
		}

		// XML データから文字列を取得します。
		private string LoadString(XmlDocument doc, string elementName){
			XmlNodeList xnl = doc.GetElementsByTagName(elementName);
			if(xnl == null) return null;
			if(xnl.Count == 0) return null;

			XmlElement e = xnl[0] as XmlElement;
			if(!e.HasAttributes) return null;
			string str = e.Attributes[0].Value;
			return str;
		}


		// XML データから ExtInfo を取得します。
		private void LoadExtInfo(XmlDocument doc){
			XmlNodeList extInfosBases =  doc.DocumentElement.GetElementsByTagName(ExtensionInfoBase);
			if(extInfosBases.Count == 0) throw new Exception("ini データに " + ExtensionInfoBase + " が含まれていません。");
			XmlElement extInfosBase = extInfosBases[0] as XmlElement;
			XmlNodeList elems = extInfosBase.GetElementsByTagName(ExtensionInfo);
			for(int i=0; i < elems.Count; i++){
				XmlElement e = elems[i] as XmlElement;
				ExtInfo ex = new ExtInfo(e);
				if(ex == null) continue;
				if(ex.Name == null) continue;
				myExtInfo[ex.Name] = ex;
			}
		}


	} // End class IniData
} // End Namespace Bakera







