using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Xml;

namespace Bakera.Hatomaru{

/// <summary>
/// ���L�𐧌䂷��N���X�ł��B
/// </summary>
	public abstract class HtmlRefAction : HatomaruGetAction{

		public const string Label = "�΂���� HTML ���t�@�����X";

// �R���X�g���N�^

		protected HtmlRefAction(HatomaruXml model, AbsPath path) : base(model, path){}



// �ÓI���\�b�h
		public static void SetReplaceUrl(Xhtml html){}


// �v���e�N�g���\�b�h


		// HtmlItem �̉�����擾���܂��B
		protected XmlNode GetDescription(HtmlItem item){
			XmlNode result = Html.CreateDocumentFragment();
			if(item == null) return result;
			if(item.Description == null) return result;
			if(item.Description.InnerText.Length == 0) return result;
			result.AppendChild(Html.H(3, null, item.Name + "�̉��"));
			result.AppendChild(ParseNode(item.Description.ChildNodes, 4));
			return result;
		}

		// HtmlItem �ւ̃����N���擾���܂��B
		// true ������ƁAName �ł͂Ȃ� SpecName ���g���܂��B
		protected XmlNode GetHtmlItemLink(HtmlItem item){
			return GetHtmlItemLink(item, false);
		}
		protected XmlNode GetHtmlItemLink(HtmlItem item, bool useSpecName){
			AbsPath path = BasePath.Combine(item.LinkId, item.Id.PathEncode());
			XmlElement a = Html.A(path);
			a.InnerText = useSpecName ? item.SpecName : item.Name;
			return a;
		}

		// HtmlItem �̔z��������N�ɂ��܂��B
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


		// �����̃��X�g��table�Ƃ��Ď擾���܂��B
		protected XmlNode GetAttributeInfoTable(HtmlAttribute attr){
			return GetAttributeInfoTable(new HtmlAttribute[]{attr});
		}

		protected XmlNode GetAttributeInfoTable(HtmlAttribute[] attrs){
			if(attrs == null || attrs.Length == 0) return Html.Null;

			XmlElement result = Html.Create("table");
			result.SetAttribute("summary", HtmlRefViewAttribute.AttributeInfoTableSummary);

			XmlElement thead = Html.Create("thead");
			result.AppendChild(thead);

			XmlElement theadTr = Html.HeadTr(null, "������", "�o�[�W����", "�����l", "����l", "���l");
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
				p.InnerText = "���̑�����������O���[�v �c�c ";
				p.AppendChild(GetHtmlItemList(groupList.ToArray(), ", "));
				result.AppendChild(p);
			}
			if(elemList.Count > 0){
				XmlElement p = Html.P();
				p.InnerText = "���̑��������v�f �c�c ";
				if(groupList.Count > 0) p.PrependChild(Html.Text("����"));
				p.AppendChild(GetHtmlItemList(elemList.ToArray(), ", "));
				result.AppendChild(p);
			}
			return result;
		}


// �I�[�o�[���C�h���\�b�h

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



