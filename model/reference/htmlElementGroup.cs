using System;
using System.Xml;

namespace Bakera.Hatomaru{
	
	/// <summary>
	/// HTMLの要素群を表すクラスです。
	/// </summary>
	public class HtmlElementGroup : HtmlVersionItem{


// コンストラクタ
		/// <summary>
		/// XmlNode を指定して、HtmlElementGroup クラスのインスタンスを開始します。
		/// </summary>
		public HtmlElementGroup(XmlElement e) : base(e){
		}


// プロパティ

		public HtmlItem[] Content{get; set;}

		public override string LinkId{
			get{return HtmlRefViewElementGroupList.Id;}
		}

// メソッド


	}

} // namespace Bakera




