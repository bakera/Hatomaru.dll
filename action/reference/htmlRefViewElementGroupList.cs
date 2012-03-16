using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace Bakera.Hatomaru{

/// <summary>
/// 要素グループの一覧を表示するアクションです。
/// </summary>
	public partial class HtmlRefViewElementGroupList : HtmlRefAction{

		public new const string Label = "要素グループ一覧";
		public const string Id = "element-group";

// コンストラクタ

		/// <summary>
		/// 属性一覧表示のためのアクションのインスタンスを開始します。
		/// </summary>
		public HtmlRefViewElementGroupList(HatomaruHtmlRef model, AbsPath path) : base(model, path){
			Data = model;
			myPath = myModel.BasePath.Combine(Id);
		}

// プロパティ
		protected HatomaruHtmlRef Data{get; set;}

		/// <summary>
		/// データを GET し、HatomaruResponse を取得します。
		/// </summary>
		protected override HatomaruResponse GetHtmlResponse(){
			InsertHeading(2, Label);
			Response.SelfTitle = Label;

			HtmlElementGroup[] groups = Data.GetAllElementGroups();
			XmlNode result = Html.Create("div", "element-group-list");
			XmlElement ul = Html.Create("ul");
			foreach(HtmlElementGroup heg in groups){
				XmlElement li = Html.Create("li");
				AbsPath dataPath = BasePath.Combine(Id, heg.Id.PathEncode());
				XmlElement a = Html.A(dataPath);
				a.InnerText = heg.FullName;
				li.AppendChild(a);
				ul.AppendChild(li);
			}
			result.AppendChild(ul);
			Html.Append(result);

			Response.AddTopicPath(myPath, Label);
			return Response;
		}




	} // End class
} // End Namespace Bakera



