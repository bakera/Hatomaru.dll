using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Xml;

namespace Bakera.Hatomaru{

/// <summary>
/// HatomaruDoc �𐧌䂷��N���X�ł��B
/// </summary>
	public abstract class DocAction : HatomaruGetAction{

		private HatomaruDoc myDoc;
// �R���X�g���N�^

		public DocAction(HatomaruDoc model, AbsPath path) : base(model, path){
			myDoc = model;
		}


		protected HatomaruDoc Doc{
			get{return myDoc;}
		}


		/// <summary>
		/// Prev / Next �ւ̃����N���擾���܂��B
		/// </summary>
		protected override XmlNode GetPrevNextNav(){
			if(Prev == null && Next == null) return Html.CreateDocumentFragment();
			XmlElement ul = Html.Create("ul");
			if(Prev != null) ul.AppendChild(Html.Create("li", null, "�O: ", Html.GetA(Prev)));
			if(Next != null) ul.AppendChild(Html.Create("li", null, "��: ", Html.GetA(Next)));
			return Html.Create("div", "prevnext", ul);
		}


	} // End class
} // End Namespace



