using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace Bakera.Hatomaru{

/// <summary>
/// �f�[�^�`���̈ꗗ��\������A�N�V�����ł��B
/// </summary>
	public partial class HtmlRefViewAttributeList : HtmlRefAction{

		public new const string Label = "�����ꗗ";
		public const string Id = "attribute";

// �R���X�g���N�^

		/// <summary>
		/// �����ꗗ�\���̂��߂̃A�N�V�����̃C���X�^���X���J�n���܂��B
		/// </summary>
		public HtmlRefViewAttributeList(HatomaruHtmlRef model, AbsPath path) : base(model, path){
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
			HtmlAttribute[] attrs = Data.GetSortedAttributes();
			XmlNode result = Html.Create("div", "attributes-list");

			XmlElement ul = null;
			char firstLetter = '_';
			foreach(HtmlAttribute attr in attrs){
				if(Char.ToUpper(attr.Name[0]) != firstLetter){
					firstLetter = Char.ToUpper(attr.Name[0]);
					if(ul != null) result.AppendChild(ul);
					ul = Html.Create("ul");
					XmlElement h = Html.H(3, null, firstLetter);
					result.AppendChild(h);
				}
				XmlElement li = Html.Create("li");
				AbsPath dataPath = BasePath.Combine(Id, attr.Id.PathEncode());
				XmlElement a = Html.A(dataPath);
				a.InnerText = attr.SpecName;
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



