using System;
using System.Xml;

namespace Bakera.Hatomaru{
	
	/// <summary>
	/// HTML の要素を表すクラスです。
	/// </summary>
	public class HtmlElement : HtmlVersionItem{

		protected string myNote;
		protected string myNoteJa;
		protected bool myOmitStartTag;
		protected bool myOmitEndTag;


// コンストラクタ
		/// <summary>
		/// XmlNode を指定して、HtmlElement クラスのインスタンスを開始します。
		/// </summary>
		public HtmlElement(XmlElement e) : base(e){
			myNote = e.GetInnerText(HatomaruHtmlRef.NoteElementName);
			myNoteJa = e.GetInnerText(HatomaruHtmlRef.NoteJaElementName);
			string omitStr = e.GetInnerText(HatomaruHtmlRef.OmitElementName);
			if(omitStr != null && omitStr.Length == 2){
				myOmitStartTag = omitStr[0] == 'o';
				myOmitEndTag = omitStr[1] == 'o';
			}
		}


// プロパティ

		public HtmlAttribute[] Attributes{get; set;}
		public HtmlAttributeGroup[] AttributeGroups{get; set;}

		/// <summary>
		/// 開始タグが省略できるならtrueを返します。
		/// </summary>
		public bool OmitStartTag{
			get {return myOmitStartTag;}
		}

		/// <summary>
		/// 終了タグが省略できるならtrueを返します。
		/// </summary>
		public bool OmitEndTag{
			get {return myOmitEndTag;}
		}

		/// <summary>
		/// 空要素ならtrueを返します。
		/// </summary>
		public bool IsEmptyElement{get; set;}

		/// <summary>
		/// 子要素を設定・取得します。
		/// </summary>
		public HtmlItem[] Content{get; set;}

		/// <summary>
		/// 完全な名前を取得します。
		/// </summary>
		public override string FullName{
			get {return myName + "要素";}
		}

		public override string LinkId{
			get{return HtmlRefViewElementList.Id;}
		}


// メソッド
		public string GetOmitStartTag(){
			return myOmitStartTag ? "省略可" : "必須";
		}
		public string GetOmitEndTag(){
			if(IsEmptyElement) return "禁止";
			return myOmitEndTag ? "省略可" : "必須";
		}


	}

} // namespace Bakera




