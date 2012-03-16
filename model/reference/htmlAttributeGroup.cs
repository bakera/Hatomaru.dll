using System;
using System.Xml;

namespace Bakera.Hatomaru{
	
	/// <summary>
	/// HTMLの要素群を表すクラスです。
	/// </summary>
	public class HtmlAttributeGroup : HtmlVersionItem{


// コンストラクタ
		/// <summary>
		/// XmlNode を指定して、HtmlElementGroup クラスのインスタンスを開始します。
		/// </summary>
		public HtmlAttributeGroup(XmlElement e) : base(e){
		}


// プロパティ

		public HtmlAttribute[] Attributes{get; set;}
		public HtmlAttributeGroup[] AttributeGroups{get; set;}

		public override string LinkId{
			get{return HtmlRefViewAttributeGroupList.Id;}
		}

// メソッド


	}

} // namespace Bakera




