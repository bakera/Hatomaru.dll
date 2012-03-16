using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Xml;

namespace Bakera.Hatomaru{

/// <summary>
/// �l�C�R���e���c�𐧌䂷��N���X�ł��B
/// </summary>
	public class ReputationView : HatomaruGetAction{

		public const int MaxItem = 50;


// �R���X�g���N�^

		public ReputationView(HatomaruXml model, AbsPath path) : base(model, path){}


// ���\�b�h

		/// <summary>
		/// �f�[�^�� GET ���AHatomaruResponse ���擾���܂��B
		/// </summary>
		protected override HatomaruResponse GetHtmlResponse(){
			InsertHeading(2, myModel.BaseTitle);
			Response.SelfTitle = myModel.BaseTitle;

			XmlElement ol = Html.Create("ol");

			XmlNodeList contents = Model.Document.GetElementsByTagName(HatomaruReputation.ContentElementName);
			
			int count = 0;
			foreach(XmlElement e in contents){
				string path = e.GetAttribute(HatomaruReputation.UriAttributeName);
				AbsPath absPath = new AbsPath(path);
				string title = myModel.Manager.GetResponseTitle(absPath);
				if(string.IsNullOrEmpty(title)) continue;
				
				XmlElement a = Html.A(absPath);
				a.InnerText = title;
				XmlElement li = Html.Create("li", null, a);

				string keywords = myModel.Manager.GetResponseKeywords(absPath);
				if(!string.IsNullOrEmpty(keywords)){
					li.AppendChild(Html.P(null, keywords));
				}
				ol.AppendChild(li);
				if(++count > MaxItem) break;
			}
			Html.Append(ol);
			return myResponse;
		}


	} // End class ReputationAction
} // End Namespace Bakera



