using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Xml;

namespace Bakera.Hatomaru{

/// <summary>
/// �A�}���𐧌䂷��N���X�ł��B
/// </summary>
	public abstract class AmazonAction : HatomaruGetAction{

		public const string Label = "�A�}��";

// �R���X�g���N�^

		protected AmazonAction(AmazonSearch model, AbsPath path) : base(model, path){}



// �ÓI���\�b�h
		public static void SetReplaceUrl(Xhtml html){}


// �v���e�N�g���\�b�h


		protected XmlElement GetSearchForm(string query, AmazonIndexType selectedType){
			XmlElement result = Html.Form();
			XmlElement fs = Html.Fieldset("Amazon����");

			XmlElement p1 = Html.P();
			p1.AppendChild(GetSelect(selectedType));
			fs.AppendChild(p1);

			XmlElement p2 = Html.P();
			p2.AppendChild(Html.Input(AmazonSearch.QueryName, query, "�L�[���[�h"));
			fs.AppendChild(p2);

			XmlElement submitP = Html.P("submit", Html.Submit("����"));
			fs.AppendChild(submitP);
			result.AppendChild(fs);
			return result;
		}


		protected XmlNode GetSelect(AmazonIndexType selectedType){
			XmlNodeList xnl = Model.Document.DocumentElement.GetElementsByTagName(AmazonSearch.AmazonFormOptionsName);
			if(xnl == null || xnl.Count == 0) throw new Exception(AmazonSearch.AmazonFormOptionsName + "�v�f���݂���܂���B");
			XmlElement options = xnl[0] as XmlElement;

			XmlNode result = Html.CreateDocumentFragment();
			XmlElement label = Html.Create("label", null, "�����Ώ�:");
			label.SetAttribute("for", AmazonSearch.IndexTypeName);
			result.AppendChild(label);

			XmlElement select = Html.Create("select");
			select.SetAttribute("name", AmazonSearch.IndexTypeName);
			select.SetAttribute("id", AmazonSearch.IndexTypeName);

			foreach(XmlElement e in options.GetElementsByTagName(AmazonSearch.OptionName)){
				XmlElement option = Html.Create("option");
				option.InnerText = e.InnerText;

				string val = e.GetAttributeValue(AmazonSearch.ValueAttributeName);
				AmazonIndexType currentType = (AmazonIndexType)Enum.Parse(typeof(AmazonIndexType), val, true);
				if((selectedType == AmazonIndexType.None && e.Attributes["selected"] != null) || currentType == selectedType){
					 option.SetAttribute("selected", "selected");
				}
				option.SetAttribute("value", currentType.ToString());
				select.AppendChild(option);
			}
			result.AppendChild(select);
			return result;
		}



	} // End class DiaryAction
} // End Namespace Bakera



