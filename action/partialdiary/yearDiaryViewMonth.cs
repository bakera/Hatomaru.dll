using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace Bakera.Hatomaru{

/// <summary>
/// 最近の日記を表示するアクションです。
/// </summary>
	public partial class YearDiaryViewMonth : PartialDiaryAction{

		private DateTime myDate;

// コンストラクタ

		/// <summary>
		/// 最近の日記表示のためのアクションのインスタンスを開始します。
		/// </summary>
		public YearDiaryViewMonth(YearDiary model, AbsPath path, int month) : base(model, path){
			try{
				myDate = new DateTime(Year, month, 1);
			} catch (ArgumentOutOfRangeException) {}
			myPath = myModel.BasePath.Combine(month);
		}



		/// <summary>
		/// データを GET し、HatomaruResponse を取得します。
		/// </summary>
		protected override HatomaruResponse GetHtmlResponse(){
			if(myDate == default(DateTime)) return NotFound();
			// 翌月の最初の日の 00:00:00.0000
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

			// 次ページ (一つ前の日記)
			DateTime prevDate = GetPrevDate(oldestTopic.Date);
			if(prevDate != default(DateTime)){
				AbsPath prevPath = myModel.ParentXml.BasePath.Combine(prevDate.Year, prevDate.Month);
				string prevTitle = string.Format("{0}の{1}", prevDate.ToString(MonthFormat), myModel.TopicSuffix);
				Prev = new LinkItem(prevPath, prevTitle);
			}

			DateTime nextDate = GetNextDate(newestTopic.Date);
			if(nextDate != default(DateTime)){
				AbsPath nextPath = myModel.ParentXml.BasePath.Combine(nextDate.Year, nextDate.Month);
				string nextTitle = string.Format("{0}の{1}", nextDate.ToString(MonthFormat), myModel.TopicSuffix);
				Next = new LinkItem(nextPath, nextTitle);
			}

			return Response;
		}




	} // End class
} // End Namespace Bakera



