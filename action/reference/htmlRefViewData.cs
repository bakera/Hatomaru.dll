using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace Bakera.Hatomaru{

/// <summary>
/// データ形式の一覧を表示するアクションです。
/// </summary>
	public partial class HtmlRefViewData : HtmlRefAction{

		public new const string Label = "データ形式";
		private string myId;

// コンストラクタ

		/// <summary>
		/// データ形式表示のためのアクションのインスタンスを開始します。
		/// </summary>
		public HtmlRefViewData(HatomaruHtmlRef model, AbsPath path, string id) : base(model, path){
			Data = model;
			myPath = myModel.BasePath.Combine(HtmlRefViewDataList.Id, id);
			myId = id;
		}

// プロパティ
		protected HatomaruHtmlRef Data{get; set;}

		/// <summary>
		/// データを GET し、HatomaruResponse を取得します。
		/// </summary>
		protected override HatomaruResponse GetHtmlResponse(){
			HtmlData hd = Data.GetData(myId);
			if(hd == null) return NotFound();

			InsertHeading(2, hd.FullName);
			Response.SelfTitle = hd.FullName;

			XmlElement result = Html.Div("data");
			result.AppendChild(GetDataOwnerInfo(hd.Parents));
			result.AppendChild(GetDescription(hd));
			Html.Append(result);

			Response.SelfTitle = hd.FullName;
			Response.AddTopicPath(BasePath.Combine(HtmlRefViewDataList.Id), Label);
			Response.AddTopicPath(myPath, hd.FullName);

			Html.Append(CommentLink(Path, Response.SelfTitle));
			return Response;
		}

		protected XmlNode GetDataOwnerInfo(HtmlItem[] items){
			XmlNode result = Html.CreateDocumentFragment();
			if(items == null || items.Length == 0) return result;
			var elemList = new List<HtmlItem>();
			var elemGroupList = new List<HtmlItem>();
			var attrList = new List<HtmlItem>();
			foreach(HtmlItem i in items){
				if(i is HtmlElement){
					elemList.Add(i);
				} else if(i is HtmlElementGroup){
					elemGroupList.Add(i);
				} else if(i is HtmlAttribute){
					attrList.Add(i);
				}
			}
			if(elemList.Count > 0){
				XmlElement p = Html.P();
				p.InnerText = "このデータを持つ要素 …… ";
				p.AppendChild(GetHtmlItemList(elemList.ToArray(), ", "));
				result.AppendChild(p);
			}
			if(elemGroupList.Count > 0){
				XmlElement p = Html.P();
				p.InnerText = "このデータを持つグループ …… ";
				p.AppendChild(GetHtmlItemList(elemGroupList.ToArray(), ", "));
				result.AppendChild(p);
			}
			if(attrList.Count > 0){
				XmlElement p = Html.P();
				p.InnerText = "このデータを持つ属性 …… ";
				p.AppendChild(GetHtmlItemList(attrList.ToArray(), ", "));
				result.AppendChild(p);
			}
			return result;
		}



	} // End class
} // End Namespace Bakera



