using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace Bakera.Hatomaru{

/// <summary>
/// �W��������\������A�N�V�����ł��B
/// </summary>
	public partial class DiaryIndexViewGenre : PartialDiaryAction{

		public const string Id = "genre";
		public const string Label = "�b��ꗗ";
		private string myWord;

// �R���X�g���N�^

		/// <summary>
		/// �W�������\���̂��߂̃A�N�V�����̃C���X�^���X���J�n���܂��B
		/// </summary>
		public DiaryIndexViewGenre(DiaryIndex model, AbsPath path, string word) : base(model, path){
			myWord = word;
			if(string.IsNullOrEmpty(word)){
				myPath = BasePath.Combine(Id);
			} else {
				myPath = BasePath.Combine(Id, word);
			}
		}


		/// <summary>
		/// �f�[�^�� GET ���AHatomaruResponse ���擾���܂��B
		/// </summary>
		protected override HatomaruResponse GetHtmlResponse(){
			Topic[] topics = GetTopicsByGenre(myWord.PathDecode());
			if(topics == null){
				// ����݊������_�C���N�g
				string newWord = myWord.Base16ToString();
				topics = GetTopicsByGenre(newWord);
				if(topics != null){
					AbsPath newPath = BasePath.Combine(Id, newWord.PathEncode());
					return Redirect(newPath);
				}
			}
			
			if(topics == null || topics.Length == 0) return NotFound();
			string title = string.Format("�b��u{0}�v���܂�{1}", myWord.PathDecode(), myModel.TopicSuffix);
			Response.SelfTitle = title;
			Response.AddTopicPath(BasePath.Combine(Id), Label);
			Response.AddTopicPath(Path, title);
			InsertHeading(2, title);
			Html.Append(DiaryHeadingList(topics, 3));
			return Response;
		}

	} // End class
} // End Namespace Bakera



