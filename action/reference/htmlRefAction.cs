using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Xml;

namespace Bakera.Hatomaru{

/// <summary>
/// 日記を制御するクラスです。
/// </summary>
	public abstract class HtmlRefAction : HatomaruGetAction{

		public const string Label = "ばけらの HTML リファレンス";

// コンストラクタ

		protected HtmlRefAction(HatomaruXml model, AbsPath path) : base(model, path){}



// 静的メソッド
		public static void SetReplaceUrl(Xhtml html){}


// プロテクトメソッド


		// HtmlItem の解説を取得します。
		protected XmlNode GetDescription(HtmlItem item){
			XmlNode result = Html.CreateDocumentFragment();
			if(item == null) return result;
			if(item.Description == null) return result;
			if(item.Description.InnerText.Length == 0) return result;
			result.AppendChild(Html.H(3, null, item.Name + "の解説"));
			result.AppendChild(ParseNode(item.Description.ChildNodes, 4));
			return result;
		}

		// HtmlItem へのリンクを取得します。
		// true をつけると、Name ではなく SpecName を使います。
		protected XmlNode GetHtmlItemLink(HtmlItem item){
			return GetHtmlItemLink(item, false);
		}
		protected XmlNode GetHtmlItemLink(HtmlItem item, bool useSpecName){
			AbsPath path = BasePath.Combine(item.LinkId, item.Id.PathEncode());
			XmlElement a = Html.A(path);
			a.InnerText = useSpecName ? item.SpecName : item.Name;
			return a;
		}

		// HtmlItem の配列をリンクにします。
		protected XmlNode GetHtmlItemList(HtmlItem item){
			return GetHtmlItemList(new HtmlItem[]{item}, null);
		}
		protected XmlNode GetHtmlItemList(HtmlItem[] items){
			return GetHtmlItemList(items, null);
		}
		protected XmlNode GetHtmlItemList(HtmlItem[] items, string sepchar){
			XmlNode result = Html.CreateDocumentFragment();
			if(items == null) return result;
			for(int i = 0; i < items.Length; i++){
				if(i > 0 && sepchar != null) result.AppendChild(Html.Text(sepchar));
				HtmlItem hi = items[i];
				if(hi is HtmlMisc){
					result.AppendChild(Html.Text(hi.Name));
				} else {
					result.AppendChild(GetHtmlItemLink(hi, true));
				}
			}
			return result;
		}


		// 属性のリストをtableとして取得します。
		protected XmlNode GetAttributeInfoTable(HtmlAttribute attr){
			return GetAttributeInfoTable(new HtmlAttribute[]{attr});
		}

		protected XmlNode GetAttributeInfoTable(HtmlAttribute[] attrs){
			if(attrs == null || attrs.Length == 0) return Html.Null;

			XmlElement result = Html.Create("table");
			result.SetAttribute("summary", HtmlRefViewAttribute.AttributeInfoTableSummary);

			XmlElement thead = Html.Create("thead");
			result.AppendChild(thead);

			XmlElement theadTr = Html.HeadTr(null, "属性名", "バージョン", "属性値", "既定値", "備考");
			thead.AppendChild(theadTr);

			XmlElement tbody = Html.Create("tbody");
			result.AppendChild(tbody);
			foreach(HtmlAttribute a in attrs){
				XmlNode values = GetHtmlItemList(a.Value);
				XmlElement tbodyTr = Html.Tr(null, 0, GetHtmlItemLink(a, false), a.GetVersion(), values, a.Default, a.Note);
				tbody.AppendChild(tbodyTr);
			}
			return result;
		}


		protected XmlNode GetAttributeOwnerInfo(HtmlItem[] items){
			XmlNode result = Html.CreateDocumentFragment();
			if(items == null || items.Length == 0) return result;
			var elemList = new List<HtmlItem>();
			var groupList = new List<HtmlItem>();
			foreach(HtmlItem i in items){
				if(i is HtmlElement){
					elemList.Add(i);
				} else if(i is HtmlAttributeGroup){
					groupList.Add(i);
				}
			}
			if(groupList.Count > 0){
				XmlElement p = Html.P();
				p.InnerText = "この属性が属するグループ …… ";
				p.AppendChild(GetHtmlItemList(groupList.ToArray(), ", "));
				result.AppendChild(p);
			}
			if(elemList.Count > 0){
				XmlElement p = Html.P();
				p.InnerText = "この属性を持つ要素 …… ";
				if(groupList.Count > 0) p.PrependChild(Html.Text("他に"));
				p.AppendChild(GetHtmlItemList(elemList.ToArray(), ", "));
				result.AppendChild(p);
			}
			return result;
		}


// オーバーライドメソッド

		protected override LinkItem[] GetSubNav(){
			LinkItem[] result = new LinkItem[]{
				new LinkItem(BasePath.Combine(HtmlRefViewElementList.Id), HtmlRefViewElementList.Label),
				new LinkItem(BasePath.Combine(HtmlRefViewElementGroupList.Id), HtmlRefViewElementGroupList.Label),
				new LinkItem(BasePath.Combine(HtmlRefViewAttributeList.Id), HtmlRefViewAttributeList.Label),
				new LinkItem(BasePath.Combine(HtmlRefViewAttributeGroupList.Id), HtmlRefViewAttributeGroupList.Label),
				new LinkItem(BasePath.Combine(HtmlRefViewDataList.Id), HtmlRefViewDataList.Label)
			};
			return result;
		}

	} // End class DiaryAction
} // End Namespace Bakera



