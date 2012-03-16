using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace Bakera.Hatomaru{

/// <summary>
/// �p��̈ꗗ��\������A�N�V�����ł��B
/// </summary>
	public partial class GlossaryViewList : GlossaryAction{

		public new const string Label = "�p��ꗗ";

// �R���X�g���N�^

		/// <summary>
		/// �ŋ߂̓��L�\���̂��߂̃A�N�V�����̃C���X�^���X���J�n���܂��B
		/// </summary>
		public GlossaryViewList(HatomaruGlossary model, AbsPath path) : base(model, path){
			myPath = myModel.BasePath;
		}


		/// <summary>
		/// �f�[�^�� GET ���AHatomaruResponse ���擾���܂��B
		/// </summary>
		protected override HatomaruResponse GetHtmlResponse(){
			Response.SelfTitle = Label;
			InsertHeading(2, Label);
			{
				GlossaryWord[] words = Glossary.GetWordByRead();
				Html.Append(GetReadList("�����E�L���E���̑�", words));
			}
			foreach(char c in HatomaruGlossary.ReadOrder){
				GlossaryWord[] words = Glossary.GetWordByRead(c);
				Html.Append(GetReadList(c.ToString(), words));
			}
			return Response;
		}

		private XmlNode GetReadList(string title, GlossaryWord[] words){
			if(words == null || words.Length == 0) return Html.Null;

			XmlNode result = Html.Div("word-list");
			result.AppendChild(Html.H(3, null, title));
			XmlElement p = WordList(words);
			result.AppendChild(p);
			return result;
		}




	} // End class
} // End Namespace Bakera



