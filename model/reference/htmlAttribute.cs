using System;
using System.Xml;

namespace Bakera.Hatomaru{
	
	/// <summary>
	/// HTML の属性を表すクラスです。
	/// </summary>
	public class HtmlAttribute : HtmlVersionItem{

		protected string myNote;
		protected string myDefault;
		protected string myFor;

		public const string IdSeparator = "@";

// コンストラクタ
		/// <summary>
		/// XmlNode を指定して、HtmlElement クラスのインスタンスを開始します。
		/// </summary>
		public HtmlAttribute(XmlElement e) : base(e){
			myNote = e.GetInnerText(HatomaruHtmlRef.NoteElementName);
			myDefault = e.GetInnerText(HatomaruHtmlRef.DefaultElementName);
			myFor = e.GetAttributeValue(HatomaruHtmlRef.ForAttributeName);
			if(string.IsNullOrEmpty(myId)){
				myId = myName;
				if(!string.IsNullOrEmpty(myFor)) myId += IdSeparator + myFor;
			}
		}


// プロパティ

		/// <summary>
		/// 属性値を取得します。
		/// </summary>
		public HtmlItem Value{get; set;}

		/// <summary>
		/// 既定値を取得します。
		/// </summary>
		public string Default{
			get {return myDefault;}
		}

		/// <summary>
		/// 備考を取得します。
		/// </summary>
		public string Note{
			get {return myNote;}
		}

		/// <summary>
		/// 詳しい名前を取得します。
		/// 特定要素に結びつく属性の場合 name (elem) が返るようにオーバーライドされます。
		/// </summary>
		public override string SpecName{
			get {
				string result = myName;
				if(!string.IsNullOrEmpty(myFor)){
					result += string.Format("({0})", myFor);
				}
				return result;
			}
		}

		/// <summary>
		/// 完全な名前を取得します。
		/// </summary>
		public override string FullName{
			get {
				string result = myName + "属性";
				if(!string.IsNullOrEmpty(myFor)){
					result += string.Format("({0})", myFor);
				}
				return result;
			}
		}

		public override string LinkId{
			get{return HtmlRefViewAttributeList.Id;}
		}

// メソッド


	}

} // namespace Bakera




