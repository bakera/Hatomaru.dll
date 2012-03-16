using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace Bakera.Hatomaru{

/// <summary>
/// ����̓��L��\������A�N�V�����ł��B
/// </summary>
	public partial class DiaryIndexViewTopic : PartialDiaryAction{

		public const string Id = "topic";
		private int myTopicId;
		private Topic myTopic = null;

// �R���X�g���N�^

		/// <summary>
		/// ����̓��L�\���̂��߂̃A�N�V�����̃C���X�^���X���J�n���܂��B
		/// </summary>
		public DiaryIndexViewTopic(DiaryIndex model, AbsPath path, int topicId) : base(model, path){
			myPath = myModel.BasePath.Combine(Id, topicId);
			myTopicId = topicId;
		}


		/// <summary>
		/// �f�[�^�� GET ���AHatomaruResponse ���擾���܂��B
		/// </summary>
		protected override HatomaruResponse GetHtmlResponse(){
			myTopic = Diary.GetTopic(myTopicId);
			if(myTopic == null) return NotFound();

			Response.AddTopicPath(myPath, myTopic.Title);
			Response.SelfTitle = myTopic.Title;
			InsertHeading(2, myTopic.Title);

			Html.Append(GetDateHeading(myTopic.Date, 3));
			Html.Append(GetTopicBody(myTopic, 4));

			// ���y�[�W (��O�̓��L)
			Topic prevTopic = GetPrevTopic(myTopicId);
			if(prevTopic != null){
				AbsPath prevPath = myModel.BasePath.Combine(Id, prevTopic.Id);
				string prevTitle = prevTopic.Title;
				Prev = new LinkItem(prevPath, prevTitle);
			}

			Topic nextTopic = GetNextTopic(myTopicId);
			if(nextTopic != null){
				AbsPath nextPath = myModel.BasePath.Combine(Id, nextTopic.Id);
				string nextTitle = nextTopic.Title;
				Next = new LinkItem(nextPath, nextTitle);
			}
			return Response;
		}

		/// <summary>
		/// �L�[���[�h���擾���܂��B
		/// </summary>
		protected override string GetKeywords(){
			return string.Join(",", myTopic.Genre);
		}





	} // End class
} // End Namespace Bakera



