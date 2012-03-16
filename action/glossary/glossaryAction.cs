using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Xml;

namespace Bakera.Hatomaru{

/// <summary>
/// ���L�𐧌䂷��N���X�ł��B
/// </summary>
	public abstract class GlossaryAction : HatomaruGetAction{

		public const string Label = "���ۂ�������� (�p��W)";

// �R���X�g���N�^

		protected GlossaryAction(HatomaruGlossary model, AbsPath path) : base(model, path){}



// �ÓI���\�b�h
		public static void SetReplaceUrl(Xhtml html){}


// �v���e�N�g���\�b�h

		// ���L�̃T�u�i�r�� LinkItem ���擾���܂��B
		protected override LinkItem[] GetSubNav(){
			return new LinkItem[]{
				GetActionLink(typeof(GlossaryViewGenre)),
			};
		}

		protected XmlElement WordList(GlossaryWord[] words){
			XmlElement p = Html.P();
			for(int i=0; i < words.Length; i++){
				if(i > 0) p.AppendChild(Html.Text(" / "));
				AbsPath wordPath = BasePath.Combine(words[i].Name.PathEncode());
				XmlElement a = Html.A(wordPath);
				a.InnerText = words[i].Name;
				p.AppendChild(a);
			}
			return p;
		}




	} // End class DiaryAction
} // End Namespace Bakera



