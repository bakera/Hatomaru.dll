using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Xml;

namespace Bakera.Hatomaru{

/// <summary>
/// HatomaruDoc�̎w�肳�ꂽ�Z�N�V������\������N���X�ł��B
/// </summary>
	public partial class DocViewSection : DocAction{

		protected string mySectionId;

// �R���X�g���N�^

		public DocViewSection(HatomaruDoc model, AbsPath path, string sectionId) : base(model, path){
			mySectionId = sectionId;
			myPath = myModel.BasePath.Combine(sectionId);
		}


// �v���p�e�B

		/// <summary>
		/// �f�[�^�� GET ���AHatomaruResponse ���擾���܂��B
		/// </summary>
		protected override HatomaruResponse GetHtmlResponse(){
			DocTopic dt = Doc.GetTopicById(mySectionId);
			if(dt == null) return Redirect(myModel.BasePath);

			InsertHeading(2, dt.FullName);
			Html.Append(ParseNode(dt.SectionElement, 3));
			// FootNoteList ������Γf���o��

			Response.SelfTitle = dt.FullName;
			Response.AddTopicPath(myPath, dt.FullName);

			if(myFootNoteCount > 0){
				XmlElement fnDiv = Html.Div("footnotes");
				XmlElement fnH = Html.H(4, null, "����");
				fnDiv.AppendChild(fnH);
				fnDiv.AppendChild(ParseFootNoteList());
				Html.Append(fnDiv);
			}

			Html.Append(CommentLink(Path, dt.FullName));

			DocTopic prevTopic = Doc.GetPrevTopic(dt);
			if(prevTopic != null){
				Prev = new LinkItem(myModel.BasePath.Combine(prevTopic.Id), prevTopic.FullName);
			} else {
				Prev = new LinkItem(myModel.BasePath, "�ڎ�");
			}
			DocTopic nextTopic = Doc.GetNextTopic(dt);
			if(nextTopic != null){
				Next = new LinkItem(myModel.BasePath.Combine(nextTopic.Id), nextTopic.FullName);
			} else {
				Next = new LinkItem(myModel.BasePath, "�ڎ�");
			}
			return myResponse;
		}

	} // End class
} // End Namespace



