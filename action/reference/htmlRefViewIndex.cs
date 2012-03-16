using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace Bakera.Hatomaru{

/// <summary>
/// 用語の一覧を表示するアクションです。
/// </summary>
	public partial class HtmlRefViewIndex : HtmlRefAction{

		public new const string Label = "目次";

// コンストラクタ

		/// <summary>
		/// リファレンスの目次表示のためのアクションのインスタンスを開始します。
		/// </summary>
		public HtmlRefViewIndex(HatomaruHtmlRef model, AbsPath path) : base(model, path){
			myPath = myModel.BasePath;
		}


		/// <summary>
		/// データを GET し、HatomaruResponse を取得します。
		/// </summary>
		protected override HatomaruResponse GetHtmlResponse(){
			InsertHeading(2, Label);
			Response.SelfTitle = Label;

			XmlNode result = Html.Create("div", "index");
			XmlElement ul = Html.Create("ul");
			foreach(LinkItem li in GetSubNav()){
				ul.AppendChild(Html.Create("li", null, Html.GetA(li)));
			}
			result.AppendChild(ul);
			Html.Append(result);

			return Response;
		}

		private XmlElement GetLi(string id, string name){
			XmlElement li = Html.Create("li");
			AbsPath dataPath = BasePath.Combine(id);
			XmlElement a = Html.A(dataPath);
			a.InnerText = name;
			li.AppendChild(a);
			return li;
		}



	} // End class
} // End Namespace Bakera



