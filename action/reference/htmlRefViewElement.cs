using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace Bakera.Hatomaru{

/// <summary>
/// �f�[�^�`���̈ꗗ��\������A�N�V�����ł��B
/// </summary>
	public partial class HtmlRefViewElement : HtmlRefAction{

		public new const string Label = "�v�f";
		public const string ElementInfoTableSummary = "�v�f�̃f�[�^�ł��B�f�[�^�͏��ɁA�v�f�̖��́A�K���o�[�W�����A�^�O�ȗ��̉ہA���ށA���g�ƂȂ��Ă��܂��B";
		private string myId;

// �R���X�g���N�^

		/// <summary>
		/// �f�[�^�`���\���̂��߂̃A�N�V�����̃C���X�^���X���J�n���܂��B
		/// </summary>
		public HtmlRefViewElement(HatomaruHtmlRef model, AbsPath path, string id) : base(model, path){
			Data = model;
			myPath = myModel.BasePath.Combine(HtmlRefViewElementList.Id, id);
			myId = id;
		}

// �v���p�e�B
		protected HatomaruHtmlRef Data{get; set;}

		/// <summary>
		/// �f�[�^�� GET ���AHatomaruResponse ���擾���܂��B
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
				obsNote.InnerText = "�����̗v�f�� HTML �d�l�Œ�`����Ă��܂���B";
				result.AppendChild(obsNote);
			} else if(e.IsObsolete){
				result = Html.Div("obsolete-element");
				XmlElement obsNote = Html.P("alert");
				obsNote.InnerText = "�����̗v�f�͔p�~����܂����B";
				result.AppendChild(obsNote);
			} else if(e.IsDeprecated){
				result = Html.Div("deprecated-element");
				XmlElement deprNote = Html.P("alert");
				deprNote.InnerText = "�����̗v�f�� HTML4 �� XHTML1.0 �ł͔񐄏��Ƃ���Ă��܂��B";
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

			XmlElement theadTr = Html.HeadTr(null, "�v�f��", "�o�[�W����", "�J�n�^�O", "�I���^�O", "����/�e", "���g");
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
			result.InnerText = "���ʑ��� �c�c ";
			result.AppendChild(GetHtmlItemList(attrGroups, ", "));
			return result;
		}


	} // End class
} // End Namespace Bakera



