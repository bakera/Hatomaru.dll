using System;
using System.Xml;



namespace Bakera.Hatomaru{

	/// <summary>
	/// URI と名前を持つアンカーの要素を扱うクラスです。
	/// </summary>
	public class LinkItem{

		private Uri myPath;
		private string myInnerText;
		private string myPrefix;
		private string mySuffix;


// コンストラクタ
		/// <summary>
		/// LinkItem のインスタンスを開始します。
		/// </summary>
		public LinkItem(){}

		/// <summary>
		/// パス文字列とテキストを指定して、LinkItem のインスタンスを開始します。
		/// </summary>
		public LinkItem(string path, string innerText){
			if(path == null) path = "";
			myPath = path.StartsWith("/") ? new Uri(path, UriKind.Relative) : new Uri(path);
			myInnerText = innerText;
		}
		/// <summary>
		/// パスのUriとテキストを指定して、LinkItem のインスタンスを開始します。
		/// </summary>
		public LinkItem(Uri path, string innerText){
			myPath = path;
			myInnerText = innerText;
		}


// プロパティ
		/// <summary>
		/// パスを設定・取得します。
		/// </summary>
		public Uri Path{
			get{return myPath;}
			set{myPath = value;}
		}

		/// <summary>
		/// InnerText を設定・取得します。
		/// </summary>
		public string InnerText{
			get{return myInnerText;}
			set{myInnerText = value;}
		}

		/// <summary>
		/// Prefix を設定・取得します。
		/// </summary>
		public string Prefix{
			get{return myPrefix;}
			set{myPrefix = value;}
		}

		/// <summary>
		/// Suffix を設定・取得します。
		/// </summary>
		public string Suffix{
			get{return mySuffix;}
			set{mySuffix = value;}
		}



// パブリックメソッド

		/// <summary>
		/// 文字列に変換します。
		/// </summary>
		public override string ToString(){
			return string.Format("{0}<{1}>", InnerText, Path);
		}

// 静的メソッド

		/// <summary>
		/// XmlNode から LinkItem を生成します。
		/// </summary>
		public static LinkItem GetItem(XmlNode node){
			XmlElement elem = node as XmlElement;
			if(elem == null) return null;

			string path = elem.GetAttribute("href");
			string text = elem.InnerText;
			return new LinkItem(path, text);
		}




	} // End class LinkItem

} // End namespace Bakera






