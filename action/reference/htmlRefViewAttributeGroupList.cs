using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace Bakera.Hatomaru{

/// <summary>
/// 属性グループの一覧を表示するアクションです。
/// </summary>
	public partial class HtmlRefViewAttributeGroupList : HtmlRefAction{

		public new const string Label = "属性グループ一覧";
		public const string Id = "attribute-group";

// コンストラクタ

		/// <summary>
		/// 属性一覧表示のためのアクションのインスタンスを開始します。
		/// </summary>
		public HtmlRefViewAttributeGroupList(HatomaruHtmlRef model, AbsPath path) : base(model, path){
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

			HtmlAttributeGroup[] groups = Data.GetAllAttributeGroups();
			XmlNode result = Html.Create("div", "element-group-list");
			XmlElement ul = Html.Create("ul");
			foreach(HtmlAttributeGroup hag in groups){
				XmlElement li = Html.Create("li");
				AbsPath dataPath = BasePath.Combine(Id, hag.Id.PathEncode());
				XmlElement a = Html.A(dataPath);
				a.InnerText = hag.FullName;
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



