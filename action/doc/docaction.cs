using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Xml;

namespace Bakera.Hatomaru{

/// <summary>
/// HatomaruDoc を制御するクラスです。
/// </summary>
	public abstract class DocAction : HatomaruGetAction{

		private HatomaruDoc myDoc;
// コンストラクタ

		public DocAction(HatomaruDoc model, AbsPath path) : base(model, path){
			myDoc = model;
		}


		protected HatomaruDoc Doc{
			get{return myDoc;}
		}


		/// <summary>
		/// Prev / Next へのリンクを取得します。
		/// </summary>
		protected override XmlNode GetPrevNextNav(){
			if(Prev == null && Next == null) return Html.CreateDocumentFragment();
			XmlElement ul = Html.Create("ul");
			if(Prev != null) ul.AppendChild(Html.Create("li", null, "前: ", Html.GetA(Prev)));
			if(Next != null) ul.AppendChild(Html.Create("li", null, "次: ", Html.GetA(Next)));
			return Html.Create("div", "prevnext", ul);
		}


	} // End class
} // End Namespace



