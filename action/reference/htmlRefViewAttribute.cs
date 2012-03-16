using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace Bakera.Hatomaru{

/// <summary>
/// �f�[�^�`���̈ꗗ��\������A�N�V�����ł��B
/// </summary>
	public partial class HtmlRefViewAttribute : HtmlRefAction{

		public new const string Label = "����";
		public const string AttributeInfoTableSummary = "�����̃f�[�^�ł��B�f�[�^�͏��ɁA�����̖��́A�K���o�[�W�����A�����l�A����l�A���l�ƂȂ��Ă��܂��B";
		private string myId;

// �R���X�g���N�^

		/// <summary>
		/// �f�[�^�`���\���̂��߂̃A�N�V�����̃C���X�^���X���J�n���܂��B
		/// </summary>
		public HtmlRefViewAttribute(HatomaruHtmlRef model, AbsPath path, string id) : base(model, path){
			Data = model;
			myPath = myModel.BasePath.Combine(HtmlRefViewAttributeList.Id, id);
			myId = id;
		}

// �v���p�e�B
		protected HatomaruHtmlRef Data{get; set;}

		/// <summary>
		/// �f�[�^�� GET ���AHatomaruResponse ���擾���܂��B
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
				obsNote.InnerText = "�����̑����� HTML �d�l�Œ�`����Ă��܂���B";
				result.AppendChild(obsNote);
			} else if(a.IsObsolete){
				result = Html.Div("obsolete-element");
				XmlElement obsNote = Html.P("alert");
				obsNote.InnerText = "�����̑����͔p�~����܂����B";
				result.AppendChild(obsNote);
			} else if(a.IsDeprecated){
				result = Html.Div("deprecated-element");
				XmlElement deprNote = Html.P("alert");
				deprNote.InnerText = "�����̑����� HTML4 �� XHTML1.0 �ł͔񐄏��Ƃ���Ă��܂��B";
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



