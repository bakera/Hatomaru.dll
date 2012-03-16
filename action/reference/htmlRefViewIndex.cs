using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace Bakera.Hatomaru{

/// <summary>
/// �p��̈ꗗ��\������A�N�V�����ł��B
/// </summary>
	public partial class HtmlRefViewIndex : HtmlRefAction{

		public new const string Label = "�ڎ�";

// �R���X�g���N�^

		/// <summary>
		/// ���t�@�����X�̖ڎ��\���̂��߂̃A�N�V�����̃C���X�^���X���J�n���܂��B
		/// </summary>
		public HtmlRefViewIndex(HatomaruHtmlRef model, AbsPath path) : base(model, path){
			myPath = myModel.BasePath;
		}


		/// <summary>
		/// �f�[�^�� GET ���AHatomaruResponse ���擾���܂��B
		/// </summary>
		protected override HatomaruResponse GetHtmlResponse(){
			InsertHeading(2, Label);
			Response.SelfTitle = Label;

			XmlNode result = Html.Create("div", "index");
			XmlElement ul = Html.Create("ul");
			foreach(LinkItem li in GetSubNav()){
				ul.AppendChild(Html.Create("li", null, Html.GetA(li)));
			}
			result.AppendChild(ul);
			Html.Append(result);

			return Response;
		}

		private XmlElement GetLi(string id, string name){
			XmlElement li = Html.Create("li");
			AbsPath dataPath = BasePath.Combine(id);
			XmlElement a = Html.A(dataPath);
			a.InnerText = name;
			li.AppendChild(a);
			return li;
		}



	} // End class
} // End Namespace Bakera



