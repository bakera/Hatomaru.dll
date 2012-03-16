using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace Bakera.Hatomaru{

/// <summary>
/// �v�f�O���[�v�̈ꗗ��\������A�N�V�����ł��B
/// </summary>
	public partial class HtmlRefViewElementGroupList : HtmlRefAction{

		public new const string Label = "�v�f�O���[�v�ꗗ";
		public const string Id = "element-group";

// �R���X�g���N�^

		/// <summary>
		/// �����ꗗ�\���̂��߂̃A�N�V�����̃C���X�^���X���J�n���܂��B
		/// </summary>
		public HtmlRefViewElementGroupList(HatomaruHtmlRef model, AbsPath path) : base(model, path){
			Data = model;
			myPath = myModel.BasePath.Combine(Id);
		}

// �v���p�e�B
		protected HatomaruHtmlRef Data{get; set;}

		/// <summary>
		/// �f�[�^�� GET ���AHatomaruResponse ���擾���܂��B
		/// </summary>
		protected override HatomaruResponse GetHtmlResponse(){
			InsertHeading(2, Label);
			Response.SelfTitle = Label;

			HtmlElementGroup[] groups = Data.GetAllElementGroups();
			XmlNode result = Html.Create("div", "element-group-list");
			XmlElement ul = Html.Create("ul");
			foreach(HtmlElementGroup heg in groups){
				XmlElement li = Html.Create("li");
				AbsPath dataPath = BasePath.Combine(Id, heg.Id.PathEncode());
				XmlElement a = Html.A(dataPath);
				a.InnerText = heg.FullName;
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



