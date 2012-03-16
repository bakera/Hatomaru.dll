using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Xml;

namespace Bakera.Hatomaru{

/// <summary>
/// HatomaruDoc �𐧌䂷��N���X�ł��B
/// </summary>
	public partial class MenuAction : HatomaruGetAction{

// �R���X�g���N�^
		public MenuAction(HatomaruMenu model, AbsPath path) : base(model, path){
			myPath = myModel.BasePath;
		}


// �v���p�e�B

		/// <summary>
		/// �f�[�^�� GET ���AHatomaruResponse ���擾���܂��B
		/// </summary>
		protected override HatomaruResponse GetHtmlResponse(){
			XmlNodeList menus = myModel.Document.GetElementsByTagName(HatomaruMenu.MenuItem);
			XmlElement ul = Html.Create("ul", "menu");
			foreach(XmlElement e in menus){
				XmlElement li = Html.Create("li");
				li.AppendChild(ParseNode(e, 3));
				ul.AppendChild(li);
			}
			Html.Append(ul);
			return myResponse;
		}


// �I�[�o�[���C�h

		/// <summary>
		/// �^�C�g����Html �ɔ��f���܂��B
		/// </summary>
		protected override void SetTitle(HatomaruResponse hr){
			if(hr.SelfTitle == null){
				hr.SelfTitle = Model.BaseTitle;
				if(Model.ParentXml != null) hr.BaseTitle = Model.ParentXml.BaseTitle;
			} else if(hr.BaseTitle == null){
				hr.BaseTitle = Model.BaseTitle;
			}
			hr.Html.Title.InnerText = hr.Title;
			hr.Html.H1.InnerText = Model.BaseTitle;
		}

		/// <summary>
		/// menu�ł͎q�i�r�͎g�p���܂���B
		/// </summary>
		protected override LinkItem[] GetSubNav(){
			return new LinkItem[0];
		}



	} // End class BbsController
} // End Namespace Bakera



