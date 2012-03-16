using System;
using System.Collections.Generic;
using System.Xml;

namespace Bakera.Hatomaru{

	// HTML の要素・属性・要素グループ・属性グループ・データ形式のベースとなる抽象クラス
	public abstract class HtmlItem{

		protected string myId;
		protected string myName;
		protected List<HtmlItem> myParents = new List<HtmlItem>();
		protected XmlElement myDescription;
		protected XmlElement myXmlElement;

		public HtmlItem(){}
		public HtmlItem(XmlElement e){
			myXmlElement = e;
			myId = e.GetAttributeValue(HatomaruHtmlRef.IdAttributeName);
			myName = e.GetAttributeValue(HatomaruHtmlRef.NameAttributeName);
			myDescription = e[HatomaruHtmlRef.DescElementName];
		}


// プロパティ
		/// <summary>
		/// ベースの XmlElementを取得します。
		/// </summary>
		public XmlElement XmlElement{
			get {return myXmlElement;}
		}

		/// <summary>
		/// IDを設定・取得します。
		/// </summary>
		public string Id{
			get {return myId;}
		}

		/// <summary>
		/// 名前を設定・取得します。
		/// </summary>
		public string Name{
			get {return myName;}
		}

		/// <summary>
		/// 説明を格納する desc 要素を取得します。
		/// </summary>
		public XmlElement Description{
			get {return myDescription;}
		}

		/// <summary>
		/// 親となる HtmlItem を取得します。
		/// </summary>
		public HtmlItem[] Parents{
			get {return myParents.ToArray();}
		}

		/// <summary>
		/// 詳しい名前を取得します。
		/// 特定要素に結びつく属性の場合 name (elem) が返るようにオーバーライドされます。
		/// </summary>
		public virtual string SpecName{
			get {return myName;}
		}
		/// <summary>
		/// 完全な名前を取得します。
		/// </summary>
		public virtual string FullName{
			get {return myName;}
		}

		public abstract string LinkId{get;}


// メソッド

		/// <summary>
		/// 親要素を追加します。
		/// </summary>
		public virtual void AddParent(HtmlItem item){
			myParents.Add(item);
		}



	}
}