using System;
using System.Xml;

namespace Bakera.Hatomaru{
	
	/// <summary>
	/// 一つのデータ形式を表すクラスです。
	/// </summary>
	public class HtmlData : HtmlItem{

		protected string myNameJa;


// コンストラクタ
		/// <summary>
		/// XmlNode を指定して、Topic クラスのインスタンスを開始します。
		/// </summary>
		public HtmlData(XmlElement e) : base(e){
			myNameJa = e.GetInnerText(HatomaruHtmlRef.NameJaElementName);
		}


// プロパティ


		/// <summary>
		/// 日本語名前を設定・取得します。
		/// </summary>
		public string NameJa{
			get {return myNameJa;}
			set {myNameJa = value;}
		}

		/// <summary>
		/// 完全名を取得します。
		/// </summary>
		public override string FullName{
			get {return string.Format("{0} ({1})", this.Name, this.NameJa);}
		}

		public override string LinkId{
			get{return HtmlRefViewDataList.Id;}
		}

	}

} // namespace Bakera




