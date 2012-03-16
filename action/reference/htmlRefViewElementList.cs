using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace Bakera.Hatomaru{

/// <summary>
/// データ形式の一覧を表示するアクションです。
/// </summary>
	public partial class HtmlRefViewElementList : HtmlRefAction{

		public new const string Label = "要素一覧";
		public const string Id = "element";

// コンストラクタ

		/// <summary>
		/// 最近の日記表示のためのアクションのインスタンスを開始します。
		/// </summary>
		public HtmlRefViewElementList(HatomaruHtmlRef model, AbsPath path) : base(model, path){
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

			HtmlElement[] elems = Data.GetSortedElements();
			XmlNode result = Html.Create("div", "elements-list");

			XmlElement ul = null;
			char firstLetter = '_';
			foreach(HtmlElement elem in elems){
				if(Char.ToUpper(elem.Name[0]) != firstLetter){
					firstLetter = Char.ToUpper(elem.Name[0]);
					if(ul != null) result.AppendChild(ul);
					ul = Html.Create("ul");
					XmlElement h = Html.H(3, null, firstLetter);
					result.AppendChild(h);
				}
				XmlElement li = Html.Create("li");
				AbsPath dataPath = BasePath.Combine(Id, elem.Id.PathEncode());
				XmlElement a = Html.A(dataPath);
				a.InnerText = elem.Name;
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



