using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace Bakera.Hatomaru{

/// <summary>
/// �v�f�O���[�v��\������A�N�V�����ł��B
/// </summary>
	public partial class HtmlRefViewAttributeGroup : HtmlRefAction{

		public new const string Label = "�v�f�O���[�v";
		private string myId;

// �R���X�g���N�^

		/// <summary>
		/// �v�f�O���[�v�\���̂��߂̃A�N�V�����̃C���X�^���X���J�n���܂��B
		/// </summary>
		public HtmlRefViewAttributeGroup(HatomaruHtmlRef model, AbsPath path, string id) : base(model, path){
			Data = model;
			myPath = myModel.BasePath.Combine(HtmlRefViewAttributeGroupList.Id, id);
			myId = id;
		}

// �v���p�e�B
		protected HatomaruHtmlRef Data{get; set;}

		/// <summary>
		/// �f�[�^�� GET ���AHatomaruResponse ���擾���܂��B
		/// </summary>
		protected override HatomaruResponse GetHtmlResponse(){
			HtmlAttributeGroup ag = Data.GetAttributeGroup(myId);
			if(ag == null) return NotFound();

			InsertHeading(2, ag.FullName);
			Response.SelfTitle = ag.FullName;

			XmlNode result = Html.Div("element-group");
			result.AppendChild(GetAttributeInfoTable(ag.Attributes));
			result.AppendChild(GetAttributeOwnerInfo(ag.Parents));
			result.AppendChild(GetDescription(ag));
			Html.Append(result);

			Response.SelfTitle = ag.FullName;
			Response.AddTopicPath(BasePath.Combine(HtmlRefViewAttributeGroupList.Id), HtmlRefViewAttributeGroupList.Label);
			Response.AddTopicPath(myPath, ag.FullName);

			Html.Append(CommentLink(Path, Response.SelfTitle));
			return Response;
		}



	} // End class
} // End Namespace Bakera



