using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Web;
using System.Xml;

namespace Bakera.Hatomaru{

/* 動作メモ
ICacheData …… キャッシュできるデータが持つインターフェイス
HatomaruData …… データソースを示す抽象クラス
HatomaruFile …… 単にファイルの場所と拡張子を示す。キャッシュ不要
HatomaruXml : HatomaruData …… HatomaruData に加えて XmlDocument を保持
abstract HatomaruTable : HatomaruXml …… HatomaruXml に加えて、DataTable を保持
*/



/// <summary>
/// 鳩丸データの XML を示すクラスです。
/// </summary>
	public abstract class HatomaruXml : HatomaruData, ICacheData{

		private readonly XmlDocument myDocument = null;
		private string myBaseTitle;
		private string myDescription;
		private string myKeywords;
		private string myTopicSuffix;
		private List<LinkItem> myStyles = new List<LinkItem>();
		private readonly HatomaruXml myParentXml;

		private XmlElement myMetaData;

		public const int SaveRetryTime = 5;
		public const int SaveRetryInterval = 1000; // ミリ秒

		public const string Name = null;
		public const string MetaName = "metadata";
		public const string TitleName = "title";
		public const string DescName = "description";
		public const string KeywordName = "keyword";
		public const string StyleName = "style";
		public const string TopicPathName = "topicpath";
		public const string TopicSuffixName = "topicname";
		public const string IndexXmlName = "index";
		public const string IndexXmlRefAttrName = "ref";

		private static readonly Type[] mySubTypes;

// 静的コンストラクタ
// Hatomaru.Xmlのサブタイプを収集して mySubTypes に格納します。
		static HatomaruXml(){
			Type thisType = typeof(HatomaruXml);
			Assembly asm = Assembly.GetAssembly(thisType);
			List<Type> types = new List<Type>();

			foreach(Type t in asm.GetTypes()){
				if(t.IsSubclassOf(thisType)) types.Add(t);
			}
			mySubTypes = types.ToArray();
		}


// コンストラクタ

		/// <summary>
		/// 鳩丸データのデータソースの FileInfo と XmlDocument を指定して、HatomaruData のインスタンスを開始します。
		/// </summary>
		protected HatomaruXml(HatomaruManager manager, FileInfo f, XmlDocument doc) : base(manager, f){
			myDocument = doc; // ロード済み
			
			myMetaData = Document.DocumentElement[MetaName];
			if(myMetaData == null) return;
			
			XmlElement title = myMetaData[TitleName];
			if(title != null) myBaseTitle = title.InnerText;
			XmlElement desc = myMetaData[DescName];
			if(desc != null) myDescription = desc.InnerText;
			XmlElement keywords = myMetaData[KeywordName];
			if(keywords != null) myKeywords = keywords.InnerText;

			XmlElement topicSuffix = myMetaData[TopicSuffixName];
			if(topicSuffix != null) myTopicSuffix = topicSuffix.InnerText;

			XmlNodeList styles = myMetaData.GetElementsByTagName(StyleName);
			foreach(XmlNode x in styles){
				XmlElement e = x as XmlElement;
				if(x == null) continue;
				string linkHref = e.GetAttribute("href");
				string linkTitle = e.GetAttribute("title");
				linkHref = '/' + linkHref.TrimStart('/');
				LinkItem s = new LinkItem(linkHref, linkTitle);
				myStyles.Add(s);
			}

			// 親 XML を取得
			XmlNodeList indexes = myMetaData.GetElementsByTagName(IndexXmlName);
			if(indexes.Count > 0){
				string indexXmlName = Util.GetAttributeValue(indexes[0], IndexXmlRefAttrName);
				if(!string.IsNullOrEmpty(indexXmlName)) myParentXml = GetDataByPathString(indexXmlName);
			} else if(!manager.RootXmlFile.FullName.Equals(f.FullName)){
				myParentXml = manager.RootXml;
			}

		}

// プロパティ

		/// <summary>
		/// XmlDocument を取得します。
		/// </summary>
		public XmlDocument Document{
			get{return myDocument;}
		}

		/// <summary>
		/// metadata 要素を取得します。
		/// </summary>
		public XmlElement MetaData{
			get{return myMetaData;}
		}

		/// <summary>
		/// ベースタイトルを取得します。
		/// </summary>
		public string BaseTitle{
			get{return myBaseTitle;}
		}

		/// <summary>
		/// この XML のインデクスとなる XML データを取得します。
		/// このプロパティを参照すると、インデクス XML がデータソースとして
		/// </summary>
		public HatomaruXml ParentXml{
			get{
				return myParentXml;
			}
		}

		/// <summary>
		/// description を取得します。
		/// </summary>
		public string Description{
			get{return myDescription;}
		}

		/// <summary>
		/// keywords を取得します。
		/// </summary>
		public string Keywords{
			get{return myKeywords;}
		}

		/// <summary>
		/// トピックの Suffix (「記事」「えび日記」など) を取得します。
		/// </summary>
		public string TopicSuffix{
			get{return myTopicSuffix;}
		}

		/// <summary>
		/// スタイルシートへリンクする LinkItem を取得します。
		/// </summary>
		public LinkItem[] Styles{
			get{return myStyles.ToArray();}
		}


// パブリックメソッド

		/// <summary>
		/// BasePath を元に、相対指定の string で対象ファイルを求め、ファイルデータを取得します。
		/// </summary>
		public HatomaruXml GetDataByPathString(string pathStr){
			string path = Path.Combine(File.Directory.FullName, pathStr);
			FileInfo f = new FileInfo(path);
			if(!f.Exists){
				f = new FileInfo(path + ".xml");
				if(!f.Exists){
					f = new FileInfo(path + "/index.xml");
					if(!f.Exists) throw new Exception("ファイルが見つかりません : " + path);
				}
			}
			Manager.Log.Add("{0} : GetDataByPathString({1})  -> {2}", this, pathStr, f.FullName);
			// 自身だったりすると無限ループになる
			if(File.FullName == f.FullName) throw new Exception("GetDataByPathString : 自身を取得しようとしました。 : " + f.FullName);
			return Manager.GetData(f);
		}

		/// <summary>
		/// この HatomaruXml にリンクする LinkItem を取得します。
		/// </summary>
		public LinkItem GetLinkItem(){
			return new LinkItem(BasePath, BaseTitle);
		}

		/// <summary>
		/// この HatomaruXml からリンクする HatomaruXml のリストを取得します。
		/// </summary>
		public HatomaruXml[] GetChildren(){
			XmlNodeList nodes = Document.DocumentElement.GetElementsByTagName(HatomaruActionBase.MenuitemElement);
			HatomaruXml[] result = new HatomaruXml[nodes.Count];
			for(int i=0; i < nodes.Count; i++){
				XmlElement elem = nodes[i] as XmlElement;
				string linkSource = elem.GetAttribute("src");
				result[i] = GetDataByPathString(linkSource);
			}
			return result;
		}


// オーバーライドメソッド

		/// <summary>
		/// データを Post し、HatomaruResponse を取得します。
		/// HatomaruXml に共通の、コメント投稿を処理するメソッドです。
		/// </summary>
		public override HatomaruResponse Post(AbsPath path, HttpRequest req){
			HatomaruPostAction pa = GetPostAction(path, req);
			if(pa == null) new NotAllowedResponse("get", this.BaseTitle + " は POST を受け付けません。");

			HatomaruResponse result = pa.Post();
			return result;
		}


		/// <summary>
		/// Post を処理するための HatomaruPostAction を取得します。
		/// このモデルが POST を処理しない場合は null を返します。
		/// </summary>
		protected virtual HatomaruPostAction GetPostAction(AbsPath path, HttpRequest req){
			return new BbsPostAction(this, path, req);
		}





// ICacheData インターフェイスの実装

		/// <summary>
		/// このデータをキャッシュしても良いかどうかを返します。
		/// hatomaruXml は常にキャッシュ可能です。
		/// </summary>
		public bool IsCacheable{
			get{return true;}
		}

		/// <summary>
		/// このインスタンスの Last-Modified とファイルの更新日を比較します。
		/// インスタンスがが最新ならば true を、古ければ false を返します。
		/// </summary>
		public override bool IsNewest{
			get{
				File.Refresh();
				if(File.LastWriteTime > LastModified) return false;
				return true;
			}
		}

		/// <summary>
		/// このインスタンスが破棄されていれば true, 使用できるなら false を返します。
		/// ファィルが削除されている場合には true が返ります。
		/// </summary>
		public virtual bool IsExpired{
			get{
				return !File.Exists;
			}
		}

		/// <summary>
		/// このインスタンスのリフレッシュを試みます。
		/// 成功すれば新しいインスタンスを、失敗すれば null を返します。
		/// </summary>
		public HatomaruXml Refresh(){
			if(IsNewest) return this;
			return null;
		}


// 静的メソッド

		// XMLデータを読み取り、HatomaruXml を生成します。
		public static HatomaruXml GetHatomaruXml(HatomaruManager manager, FileInfo f){
			try{
				XmlDocument x = new XmlDocument();
				x.XmlResolver = null;
				for(int i=0;;i++){
					try{
						x.Load(f.FullName);
						break;
					} catch(IOException e){
						System.Threading.Thread.Sleep(SaveRetryInterval);
						if(i > SaveRetryTime){
							throw new HatomaruXmlException("データのロードに失敗しました。", e);
						}
					}
				}
				if(x.DocumentElement == null) throw new HatomaruXmlException("XMLファイルが空か、ルート要素がありません。");
				string rootName = x.DocumentElement.Name;
				foreach(Type t in mySubTypes){
					string name = GetFieldValue(t, "Name");
					if(name.Eq(rootName)){
						ConstructorInfo ci = t.GetConstructor(new Type[]{typeof(HatomaruManager), typeof(FileInfo), typeof(XmlDocument)});
						if(ci == null) throw new Exception(name + "には適切なコンストラクタがありません。");
						Object o = ci.Invoke(new Object[]{manager, f, x});
						HatomaruXml result = o as HatomaruXml;
						return result;
					}
				}
				return new HatomaruDoc(manager, f, x);
			} catch(System.Data.ConstraintException e){
				throw new HatomaruXmlException("データのロードに失敗しました。", e);
			} catch(System.Xml.XmlException e){
				throw new HatomaruXmlException("XMLデータのパースに失敗しました。", e);
			}
		}

		// フィールドの値を取得します。
		private static string GetFieldValue(Type t, string name){
			Object o = t.GetField(name).GetValue(null);
			if(o == null) return null;
			return o.ToString();
		}


	} // End class HatomaruXml
} // End Namespace Bakera



