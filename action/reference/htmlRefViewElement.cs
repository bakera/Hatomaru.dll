using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace Bakera.Hatomaru{

/// <summary>
/// データ形式の一覧を表示するアクションです。
/// </summary>
	public partial class HtmlRefViewElement : HtmlRefAction{

		public new const string Label = "要素";
		public const string ElementInfoTableSummary = "要素のデータです。データは順に、要素の名称、適合バージョン、タグ省略の可否、分類、中身となっています。";
		private string myId;

// コンストラクタ

		/// <summary>
		/// データ形式表示のためのアクションのインスタンスを開始します。
		/// </summary>
		public HtmlRefViewElement(HatomaruHtmlRef model, AbsPath path, string id) : base(model, path){
			Data = model;
			myPath = myModel.BasePath.Combine(HtmlRefViewElementList.Id, id);
			myId = id;
		}

// プロパティ
		protected HatomaruHtmlRef Data{get; set;}

		/// <summary>
		/// データを GET し、HatomaruResponse を取得します。
		/// </summary>
		protected override HatomaruResponse GetHtmlResponse(){
			HtmlElement e = Data.GetElement(myId);
			if(e == null) return NotFound();

			InsertHeading(2, e.FullName);
			Response.SelfTitle = e.FullName;
			XmlNode result;
			if(!e.IsSpecified){
				result = Html.Div("not-defined-element");
				XmlElement obsNote = Html.P("alert");
				obsNote.InnerText = "※この要素は HTML 仕様で定義されていません。";
				result.AppendChild(obsNote);
			} else if(e.IsObsolete){
				result = Html.Div("obsolete-element");
				XmlElement obsNote = Html.P("alert");
				obsNote.InnerText = "※この要素は廃止されました。";
				result.AppendChild(obsNote);
			} else if(e.IsDeprecated){
				result = Html.Div("deprecated-element");
				XmlElement deprNote = Html.P("alert");
				deprNote.InnerText = "※この要素は HTML4 や XHTML1.0 では非推奨とされています。";
				result.AppendChild(deprNote);
			} else {
				result = Html.Div("element");
			}

			result.AppendChild(GetElementInfoTable(e));
			result.AppendChild(GetAttributeInfoTable(e.Attributes));
			result.AppendChild(GetCommonAttributeInfo(e.AttributeGroups));
			result.AppendChild(GetDescription(e));
			Html.Append(result);

			Response.SelfTitle = e.FullName;
			Response.AddTopicPath(BasePath.Combine(HtmlRefViewElementList.Id), HtmlRefViewElementList.Label);
			Response.AddTopicPath(myPath, e.FullName);

			Html.Append(CommentLink(Path, Response.SelfTitle));

			return Response;
		}


		private XmlElement GetElementInfoTable(HtmlElement e){
			XmlElement result = Html.Create("table");
			result.SetAttribute("summary", ElementInfoTableSummary);

			XmlElement thead = Html.Create("thead");
			result.AppendChild(thead);

			XmlElement theadTr = Html.HeadTr(null, "要素名", "バージョン", "開始タグ", "終了タグ", "分類/親", "中身");
			thead.AppendChild(theadTr);

			XmlElement tbody = Html.Create("tbody");
			result.AppendChild(tbody);

			XmlNode parents = GetHtmlItemList(e.Parents, ", ");
			XmlNode contents = GetHtmlItemList(e.Content);

			XmlElement tbodyTr = Html.Tr(null, 0, e.Name, e.GetVersion(), e.GetOmitStartTag(), e.GetOmitEndTag(), parents, contents);
			tbody.AppendChild(tbodyTr);

			return result;
		}

		protected XmlNode GetCommonAttributeInfo(HtmlAttributeGroup[] attrGroups){
			if(attrGroups == null || attrGroups.Length == 0) return Html.Null;
			XmlNode result = Html.P();
			result.InnerText = "共通属性 …… ";
			result.AppendChild(GetHtmlItemList(attrGroups, ", "));
			return result;
		}


	} // End class
} // End Namespace Bakera



