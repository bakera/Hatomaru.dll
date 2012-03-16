using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace Bakera.Hatomaru{

/// <summary>
/// �v�f�O���[�v��\������A�N�V�����ł��B
/// </summary>
	public partial class HtmlRefViewElementGroup : HtmlRefAction{

		public new const string Label = "�v�f�O���[�v";
		private string myId;
		public const string ElementGroupInfoTableSummary = "�v�f�O���[�v�̃f�[�^�ł��B�f�[�^�͏��ɁA���ށA���g�ƂȂ��Ă��܂��B";

// �R���X�g���N�^

		/// <summary>
		/// �v�f�O���[�v�\���̂��߂̃A�N�V�����̃C���X�^���X���J�n���܂��B
		/// </summary>
		public HtmlRefViewElementGroup(HatomaruHtmlRef model, AbsPath path, string id) : base(model, path){
			Data = model;
			myPath = myModel.BasePath.Combine(HtmlRefViewElementGroupList.Id, id);
			myId = id;
		}

// �v���p�e�B
		protected HatomaruHtmlRef Data{get; set;}

		/// <summary>
		/// �f�[�^�� GET ���AHatomaruResponse ���擾���܂��B
		/// </summary>
		protected override HatomaruResponse GetHtmlResponse(){
			HtmlElementGroup eg = Data.GetElementGroup(myId);
			if(eg == null) return NotFound();

			InsertHeading(2, eg.FullName);
			Response.SelfTitle = eg.FullName;
			XmlNode result = Html.Div("element-group");

			Html.Append(GetElementGroupInfoTable(eg));
			Html.Append(GetDescription(eg));

			Response.SelfTitle = eg.FullName;
			Response.AddTopicPath(BasePath.Combine(HtmlRefViewElementGroupList.Id), HtmlRefViewElementGroupList.Label);
			Response.AddTopicPath(myPath, eg.FullName);

			Html.Append(CommentLink(Path, Response.SelfTitle));

			return Response;
		}


		private XmlElement GetElementGroupInfoTable(HtmlElementGroup e){
			XmlElement result = Html.Create("table");
			result.SetAttribute("summary", ElementGroupInfoTableSummary);

			XmlElement thead = Html.Create("thead");
			result.AppendChild(thead);

			XmlElement theadTr = Html.HeadTr(null, "���̎Q��", "����/�e", "���g");
			thead.AppendChild(theadTr);

			XmlElement tbody = Html.Create("tbody");
			result.AppendChild(tbody);

			XmlNode parents = GetHtmlItemList(e.Parents, ", ");
			XmlNode contents = GetHtmlItemList(e.Content, ", ");

			XmlElement tbodyTr = Html.Tr(null, 0, e.Name, parents, contents);
			tbody.AppendChild(tbodyTr);

			return result;
		}


	} // End class
} // End Namespace Bakera



