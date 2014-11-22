using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace Bakera.Hatomaru{

/// <summary>
/// �ŋ߂̓��L��\������A�N�V�����ł��B
/// </summary>
	public partial class YearDiaryViewMonth : PartialDiaryAction{

		private DateTime myDate;

// �R���X�g���N�^

		/// <summary>
		/// �ŋ߂̓��L�\���̂��߂̃A�N�V�����̃C���X�^���X���J�n���܂��B
		/// </summary>
		public YearDiaryViewMonth(YearDiary model, AbsPath path, int month) : base(model, path){
			try{
				myDate = new DateTime(Year, month, 1);
			} catch (ArgumentOutOfRangeException) {}
			myPath = myModel.BasePath.Combine(month);
		}



		/// <summary>
		/// �f�[�^�� GET ���AHatomaruResponse ���擾���܂��B
		/// </summary>
		protected override HatomaruResponse GetHtmlResponse(){
			if(myDate == default(DateTime)) return NotFound();
			// �����̍ŏ��̓��� 00:00:00.0000
			DateTime endDate = myDate.Month == 12 ? new DateTime(myDate.Year + 1, 1, 1) : new DateTime(myDate.Year, myDate.Month + 1, 1);
			Topic[] topics = YearDiary.GetSpanTopics(myDate, endDate);
			if(topics.Length == 0) return NotFound();

			string monthTitle = myDate.ToString(MonthFormat, Model.Manager.IniData.CultureInfo);
			Response.AddTopicPath(myModel.BasePath.Combine(myDate.Month), monthTitle);
			Response.SelfTitle = monthTitle;
			Response.BaseTitle = Diary.BaseTitle;
			InsertHeading(2, monthTitle);
			Html.Append(DiaryListWithDate(topics, 3));
			Topic newestTopic = topics[0];
			Topic oldestTopic = topics[topics.Length - 1];

			// ���y�[�W (��O�̓��L)
			DateTime prevDate = GetPrevDate(oldestTopic.Date);
			if(prevDate != default(DateTime)){
				AbsPath prevPath = myModel.ParentXml.BasePath.Combine(prevDate.Year, prevDate.Month);
				string prevTitle = string.Format("{0}��{1}", prevDate.ToString(MonthFormat), myModel.TopicSuffix);
				Prev = new LinkItem(prevPath, prevTitle);
			}

			DateTime nextDate = GetNextDate(newestTopic.Date);
			if(nextDate != default(DateTime)){
				AbsPath nextPath = myModel.ParentXml.BasePath.Combine(nextDate.Year, nextDate.Month);
				string nextTitle = string.Format("{0}��{1}", nextDate.ToString(MonthFormat), myModel.TopicSuffix);
				Next = new LinkItem(nextPath, nextTitle);
			}

			return Response;
		}




	} // End class
} // End Namespace Bakera



