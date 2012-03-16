using System;
using System.Collections.Specialized;
using System.IO;
using System.Xml;
using System.Xml.Resolvers;

namespace Bakera.Hatomaru{

	/// <summary>
	/// XML DOM を利用して出力用の XHTML を簡単に作るためのクラスです。
	/// 外部実体は読みに行きません。
	/// </summary>
	public partial class Xhtml : XmlDocument{
		public const string NameSpace = "http://www.w3.org/1999/xhtml";
		public const string RootElement = "html";
		public const string PublicIdentifier = "-//W3C//DTD XHTML 1.1//EN";
		public const string SystemIdentifier = "http://www.w3.org/TR/xhtml11/DTD/xhtml11.dtd";

		public const string UrnIetfRfcPrefix = "urn:ietf:rfc:";
		public const string UrnIetfRfcFormat = "http://tools.ietf.org/html/rfc{0}";

		private Uri myBaseUri;
		private XmlElement myEntry;
		private NameValueCollection myReplaceUrl; // URL を置換する設定を保存

// コンストラクタ

		/// <summary>
		/// XHTML ドキュメントのインスタンスを作成します。
		/// 外部実体は読みに行きません。
		/// </summary>
		public Xhtml() : base(){
			PreserveWhitespace = true;
			XmlResolver = null;
			AppendChild(CreateXmlDeclaration("1.0", "UTF-8", null));
			AppendChild(CreateDocumentType(RootElement, PublicIdentifier, SystemIdentifier, null));
		}

		/// <summary>
		/// ファイルを指定して、XHTML ドキュメントのインスタンスを作成します。
		/// </summary>
		public Xhtml(string filename) : this(){
			LoadFile(filename);
		}


// プロパティ

		/// <summary>
		/// XHTML ドキュメントの基準となる URL を設定・取得します。
		/// </summary>
		public Uri BaseUri{
			get {return myBaseUri;}
			set {myBaseUri = value;}
		}

		/// <summary>
		/// エントリーポイントを設定・取得します。
		/// </summary>
		public XmlElement Entry{
			get {return myEntry;}
			set {myEntry = value;}
		}
// 各要素にアクセスするプロパティ

		/// <summary>
		/// XHTML ドキュメントの html 要素にアクセスします。
		/// </summary>
		public XmlElement Html{
			get {
				XmlElement result = DocumentElement;
				if(result == null){
					result = Create("html");
					this.AppendChild(result);
				}
				return result;
			}
		}

		/// <summary>
		/// XHTML ドキュメントの head 要素を表す XmlElement にアクセスします。
		/// </summary>
		public XmlElement Head{
			get {
				XmlElement result = Html["head"];
				if(result == null){
					result = Create("head");
					Html.PrependChild(result);
				}
				return result;
			}
		}

		/// <summary>
		/// XHTML ドキュメントの body 要素を表す XmlElement にアクセスします。
		/// </summary>
		public XmlElement Body{
			get {
				XmlElement result = Html["body"];
				if(result == null){
					result = Create("body");
					Html.AppendChild(result);
				}
				return result;
			}
		}

		/// <summary>
		/// XHTML ドキュメントの title 要素を表す XmlElement にアクセスします。
		/// </summary>
		public XmlElement Title{
			get {
				XmlElement result = this.Head["title"];
				if(result == null){
					result = Create("title");
					Head.AppendChild(result);
				}
				return result;
			}
		}

		/// <summary>
		/// XHTML ドキュメントの最初の h1 要素を表す XmlElement にアクセスします。
		/// h1 要素が無い場合は null を返します。
		/// </summary>
		public XmlElement H1{
			get {
				XmlNodeList nodes = this.Body.GetElementsByTagName("h1");
				if(nodes.Count == 0) return null;
				return nodes[0] as XmlElement;
			}
		}

// パブリックメソッド


		/// <summary>
		/// 雛形の Xhtml を指定して、Xhtml の新しいインスタンスを作成します。
		/// </summary>
		public static Xhtml Copy(Xhtml html){
			if(html == null) throw new ArgumentException("元となる XHTML が null です。");
			Xhtml result = new Xhtml();
			// 文書型宣言なども含めてコピー
			foreach(XmlNode x in html.ChildNodes){
				result.AppendChild(result.ImportNode(x, true));
			}
			return result;
		}


		/// <summary>
		/// ファイル名を指定して XML データを読み取ります。
		/// Load と異なり、ソースファイルは読み取り禁止になりません (上書き禁止になるだけです)。
		/// </summary>
		public void LoadFile(string filename){
			using(FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.Read)){
				this.Load(fs);
			}
		}

		/// <summary>
		/// FileInfoを指定して XML データを保存します。
		/// </summary>
		public void SaveFile(FileInfo f){
			f.Directory.Create();
			using(FileStream fs = f.Open(FileMode.Create, FileAccess.Write, FileShare.None)){
				this.Save(fs);
			}
		}


		/// <summary>
		/// Uri を href の値に変換します。
		/// 自身へのリンクとなる場合は空文字列を返します。
		/// </summary>
		public string GetHref(Uri uri){
			if(myReplaceUrl != null){
				foreach(string key in myReplaceUrl.AllKeys){
					if(uri.OriginalString.EndsWith(key)){
						string newUrlString = uri.OriginalString.Replace(key, myReplaceUrl[key]);
						uri = new Uri(newUrlString, UriKind.RelativeOrAbsolute);
						break;
					}
				}
			}
			// URN 
			if(uri.ToString().StartsWith(UrnIetfRfcPrefix)){
				string rfcNumberString = uri.ToString().Substring(UrnIetfRfcPrefix.Length);
				if(rfcNumberString.Length == 0){
					return String.Format(UrnIetfRfcFormat, "-index");
				}
				return String.Format(UrnIetfRfcFormat, rfcNumberString);
			}
			Uri absUri = null;
			if(myBaseUri != null){
				absUri = new Uri(myBaseUri, uri);
				if(absUri == myBaseUri) return "";
			} else {
				absUri = uri;
			}
			if(absUri == null) return null;

			Uri hrefUri = MakeRelative(absUri);
			return hrefUri.OriginalString;
		}


		/// <summary>
		/// 設定されたベース Uri を元に相対 Uri を生成します。
		/// </summary>
		public Uri MakeRelative(Uri uri){
			if(myBaseUri == null) return uri;
			if(myBaseUri == uri) return new Uri("", UriKind.Relative);

			if(uri == null) return new Uri("", UriKind.Relative);
			if(!uri.IsAbsoluteUri) return uri;

			Uri result = myBaseUri.MakeRelativeUri(uri);
			if(string.IsNullOrEmpty(result.ToString())) return new Uri("./", UriKind.Relative);
			return result;
		}


		/// <summary>
		/// ある名前の要素の中で最初のものを返します。
		/// </summary>
		public XmlElement GetElementByTagNameFirst(string name){
			XmlNodeList list = GetElementsByTagName(name);
			if(list.Count == 0) return null;
			return list[0] as XmlElement;
		}


		/// <summary>
		/// ある名前の要素を渡された要素で置換します。
		/// </summary>
		public void Replace(string name, XmlNode rep){
			if(rep == null) rep = CreateDocumentFragment();
			XmlElement target = GetElementByTagNameFirst(name);
			if(target == null) return;
			target.ParentNode.ReplaceChild(rep, target);
		}

	} // End class OutXhtml
}
