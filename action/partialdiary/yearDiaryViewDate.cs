using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace Bakera.Hatomaru{

/// <summary>
/// 最近の日記を表示するアクションです。
/// </summary>
	public partial class YearDiaryViewDate : PartialDiaryAction{

		private DateTime myDate;

// コンストラクタ

		/// <summary>
		/// 最近の日記表示のためのアクションのインスタンスを開始します。
		/// </summary>
		public YearDiaryViewDate(YearDiary model, AbsPath path, int month, int day) : base(model, path){
			try{
				myDate = new DateTime(Year, month, day);
			} catch (ArgumentOutOfRangeException) {}
			myPath = myModel.BasePath.Combine(myDate.Month, myDate.Day);
		}


		/// <summary>
		/// データを GET し、HatomaruResponse を取得します。
		/// </summary>
		protected override HatomaruResponse GetHtmlResponse(){
			if(myDate == default(DateTime)) return NotFound();
			Topic[] topics = YearDiary.GetDayTopics(myDate);
			if(topics.Length == 0) return NotFound();

			string monthTitle = myDate.ToString(MonthFormat);
			Response.AddTopicPath(myModel.BasePath.Combine(myDate.Month), monthTitle);

			string dateTitle = myDate.ToString(DateFormat);
			Response.AddTopicPath(myModel.BasePath.Combine(myDate.Month, myDate.Day), dateTitle);
			Response.SelfTitle = dateTitle;
			Response.BaseTitle = Diary.BaseTitle;

			InsertHeading(2, dateTitle);
			for(int i=0; i < topics.Length; i++){
				Html.Append(GetTopicBody(topics[i], 3));
			}
			// 次ページ (一つ前の日記)
			DateTime prevDate = GetPrevDate(myDate);
			if(prevDate != default(DateTime)){
				AbsPath prevPath = myModel.ParentXml.BasePath.Combine(prevDate.Year, prevDate.Month, prevDate.Day);
				string prevTitle = string.Format("{0}の{1}", prevDate.ToString(DateFormat), myModel.TopicSuffix);
				Prev = new LinkItem(prevPath, prevTitle);
			}
			DateTime nextDate = GetNextDate(myDate);
			if(nextDate != default(DateTime)){
				AbsPath nextPath = myModel.ParentXml.BasePath.Combine(nextDate.Year, nextDate.Month, nextDate.Day);
				string nextTitle = string.Format("{0}の{1}", nextDate.ToString(DateFormat), myModel.TopicSuffix);
				Next = new LinkItem(nextPath, nextTitle);
			}
			return Response;
		}




	} // End class
} // End Namespace Bakera



