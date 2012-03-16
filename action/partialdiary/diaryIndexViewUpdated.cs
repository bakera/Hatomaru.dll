using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace Bakera.Hatomaru{

/// <summary>
/// �ŋ߂̓��L��\������A�N�V�����ł��B
/// </summary>
	public partial class DiaryIndexViewUpdated : PartialDiaryAction{

		public const string Id = "updated";
		public const string Label = "�X�V�E�ǋL���ꂽ���L";

// �R���X�g���N�^

		/// <summary>
		/// ���L�̌��o���ꗗ�\���̂��߂̃A�N�V�����̃C���X�^���X���J�n���܂��B
		/// </summary>
		public DiaryIndexViewUpdated(DiaryIndex model, AbsPath path) : base(model, path){
			myPath = myModel.BasePath.Combine(Id);
		}


		/// <summary>
		/// �f�[�^�� GET ���AHatomaruResponse ���擾���܂��B
		/// </summary>
		protected override HatomaruResponse GetHtmlResponse(){

			Topic[] topics = GetUpdatedTopics();
			if(topics.Length == 0) return NotFound();
			Response.SelfTitle = Label;
			Response.AddTopicPath(Path, Label);
			InsertHeading(2, Label);
			XmlElement ul = Html.Create("ul");
			foreach(Topic t in topics){
				XmlElement li = Html.Create("li", null, MakeTopicAnchor(t), Html.Space, GetUpdated(t));
				ul.AppendChild(li);
			}
			Html.Append(ul);

			LinkItem atomLink = new LinkItem(BasePath.Combine(Id, DiaryIndexUpdatedAtom.Id), DiaryIndexUpdatedAtom.Label + "(Atom1.0)");
			Html.AddLinkRel("alternate", HatomaruResponse.AtomMediaType, atomLink);

			return Response;
		}


	} // End class
} // End Namespace Bakera



