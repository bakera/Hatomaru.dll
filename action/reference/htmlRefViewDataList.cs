using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace Bakera.Hatomaru{

/// <summary>
/// �f�[�^�`���̈ꗗ��\������A�N�V�����ł��B
/// </summary>
	public partial class HtmlRefViewDataList : HtmlRefAction{

		public new const string Label = "�f�[�^�`���ꗗ";
		public const string Id = "data";

// �R���X�g���N�^

		/// <summary>
		/// �ŋ߂̓��L�\���̂��߂̃A�N�V�����̃C���X�^���X���J�n���܂��B
		/// </summary>
		public HtmlRefViewDataList(HatomaruHtmlRef model, AbsPath path) : base(model, path){
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

			HtmlData[] datas = Data.GetAllData();
			XmlNode result = Html.Create("div", "dataformats");
			XmlElement ul = Html.Create("ul");
			foreach(HtmlData hd in datas){
				XmlElement li = Html.Create("li");
				AbsPath dataPath = myPath.Combine(hd.Id.PathEncode());
				XmlElement a = Html.A(dataPath);
				a.InnerText = hd.FullName;
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



