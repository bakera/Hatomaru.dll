using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace Bakera.Hatomaru{

/// <summary>
/// �N�̓��L��\������A�N�V�����ł��B
/// </summary>
	public partial class YearDiaryViewYear : PartialDiaryAction{


// �R���X�g���N�^

		/// <summary>
		/// �N�̓��L�\���̂��߂̃A�N�V�����̃C���X�^���X���J�n���܂��B
		/// </summary>
		public YearDiaryViewYear(YearDiary model, AbsPath path) : base(model, path){
			myPath = myModel.BasePath;
		}


		/// <summary>
		/// �f�[�^�� GET ���AHatomaruResponse ���擾���܂��B
		/// </summary>
		protected override HatomaruResponse GetHtmlResponse(){

			Topic[] topics = YearDiary.GetAllTopics();
			if(topics.Length == 0) return NotFound();

			string yearTitle = YearDiary.Dates[0].ToString(YearFormat);
			Response.SelfTitle = myModel.BaseTitle;
			Response.BaseTitle = Diary.BaseTitle;
			InsertHeading(2, yearTitle);
			Html.Append(DiaryHeadingList(topics, 3));

			Topic newestTopic = topics[0];
			Topic oldestTopic = topics[topics.Length - 1];

			// ���y�[�W (��O�̓��L)
			DateTime prevDate = GetPrevDate(oldestTopic.Date);
			if(prevDate != default(DateTime)){
				AbsPath prevPath = myModel.ParentXml.BasePath.Combine(prevDate.Year);
				string prevTitle = string.Format("{0}��{1}", prevDate.ToString(YearFormat), myModel.TopicSuffix);
				Prev = new LinkItem(prevPath, prevTitle);
			}

			DateTime nextDate = GetNextDate(newestTopic.Date);
			if(nextDate != default(DateTime)){
				AbsPath nextPath = myModel.ParentXml.BasePath.Combine(nextDate.Year);
				string nextTitle = string.Format("{0}��{1}", nextDate.ToString(YearFormat), myModel.TopicSuffix);
				Next = new LinkItem(nextPath, nextTitle);
			}

			return Response;
		}


	} // End class
} // End Namespace Bakera



