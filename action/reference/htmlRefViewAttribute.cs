using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace Bakera.Hatomaru{

/// <summary>
/// データ形式の一覧を表示するアクションです。
/// </summary>
	public partial class HtmlRefViewAttribute : HtmlRefAction{

		public new const string Label = "属性";
		public const string AttributeInfoTableSummary = "属性のデータです。データは順に、属性の名称、適合バージョン、属性値、既定値、備考となっています。";
		private string myId;

// コンストラクタ

		/// <summary>
		/// データ形式表示のためのアクションのインスタンスを開始します。
		/// </summary>
		public HtmlRefViewAttribute(HatomaruHtmlRef model, AbsPath path, string id) : base(model, path){
			Data = model;
			myPath = myModel.BasePath.Combine(HtmlRefViewAttributeList.Id, id);
			myId = id;
		}

// プロパティ
		protected HatomaruHtmlRef Data{get; set;}

		/// <summary>
		/// データを GET し、HatomaruResponse を取得します。
		/// </summary>
		protected override HatomaruResponse GetHtmlResponse(){
			HtmlAttribute a = Data.GetAttribute(myId);
			if(a == null) return NotFound();

			InsertHeading(2, a.FullName);
			Response.SelfTitle = a.FullName;

			XmlNode result;
			if(!a.IsSpecified){
				result = Html.Div("not-defined-element");
				XmlElement obsNote = Html.P("alert");
				obsNote.InnerText = "※この属性は HTML 仕様で定義されていません。";
				result.AppendChild(obsNote);
			} else if(a.IsObsolete){
				result = Html.Div("obsolete-element");
				XmlElement obsNote = Html.P("alert");
				obsNote.InnerText = "※この属性は廃止されました。";
				result.AppendChild(obsNote);
			} else if(a.IsDeprecated){
				result = Html.Div("deprecated-element");
				XmlElement deprNote = Html.P("alert");
				deprNote.InnerText = "※この属性は HTML4 や XHTML1.0 では非推奨とされています。";
				result.AppendChild(deprNote);
			} else {
				result = Html.Div("element");
			}
			result.AppendChild(GetAttributeInfoTable(a));
			result.AppendChild(GetAttributeOwnerInfo(a.Parents));
			result.AppendChild(GetDescription(a));
			Html.Append(result);

			Response.SelfTitle = a.FullName;
			Response.AddTopicPath(BasePath.Combine(HtmlRefViewAttributeList.Id), HtmlRefViewAttributeList.Label);
			Response.AddTopicPath(myPath, a.FullName);

			Html.Append(CommentLink(Path, Response.SelfTitle));
			return Response;
		}




	} // End class
} // End Namespace Bakera



