using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace Bakera.Hatomaru{

/// <summary>
/// データ形式の一覧を表示するアクションです。
/// </summary>
	public partial class HtmlRefViewDataList : HtmlRefAction{

		public new const string Label = "データ形式一覧";
		public const string Id = "data";

// コンストラクタ

		/// <summary>
		/// 最近の日記表示のためのアクションのインスタンスを開始します。
		/// </summary>
		public HtmlRefViewDataList(HatomaruHtmlRef model, AbsPath path) : base(model, path){
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

			HtmlData[] datas = Data.GetAllData();
			XmlNode result = Html.Create("div", "dataformats");
			XmlElement ul = Html.Create("ul");
			foreach(HtmlData hd in datas){
				XmlElement li = Html.Create("li");
				AbsPath dataPath = myPath.Combine(hd.Id.PathEncode());
				XmlElement a = Html.A(dataPath);
				a.InnerText = hd.FullName;
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



