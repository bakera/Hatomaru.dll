using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace Bakera.Hatomaru{

/// <summary>
/// �f�[�^�`���̈ꗗ��\������A�N�V�����ł��B
/// </summary>
	public partial class HtmlRefViewElementList : HtmlRefAction{

		public new const string Label = "�v�f�ꗗ";
		public const string Id = "element";

// �R���X�g���N�^

		/// <summary>
		/// �ŋ߂̓��L�\���̂��߂̃A�N�V�����̃C���X�^���X���J�n���܂��B
		/// </summary>
		public HtmlRefViewElementList(HatomaruHtmlRef model, AbsPath path) : base(model, path){
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

			HtmlElement[] elems = Data.GetSortedElements();
			XmlNode result = Html.Create("div", "elements-list");

			XmlElement ul = null;
			char firstLetter = '_';
			foreach(HtmlElement elem in elems){
				if(Char.ToUpper(elem.Name[0]) != firstLetter){
					firstLetter = Char.ToUpper(elem.Name[0]);
					if(ul != null) result.AppendChild(ul);
					ul = Html.Create("ul");
					XmlElement h = Html.H(3, null, firstLetter);
					result.AppendChild(h);
				}
				XmlElement li = Html.Create("li");
				AbsPath dataPath = BasePath.Combine(Id, elem.Id.PathEncode());
				XmlElement a = Html.A(dataPath);
				a.InnerText = elem.Name;
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



