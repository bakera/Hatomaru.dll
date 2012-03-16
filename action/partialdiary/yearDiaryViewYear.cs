using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace Bakera.Hatomaru{

/// <summary>
/// 年の日記を表示するアクションです。
/// </summary>
	public partial class YearDiaryViewYear : PartialDiaryAction{


// コンストラクタ

		/// <summary>
		/// 年の日記表示のためのアクションのインスタンスを開始します。
		/// </summary>
		public YearDiaryViewYear(YearDiary model, AbsPath path) : base(model, path){
			myPath = myModel.BasePath;
		}


		/// <summary>
		/// データを GET し、HatomaruResponse を取得します。
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

			// 次ページ (一つ前の日記)
			DateTime prevDate = GetPrevDate(oldestTopic.Date);
			if(prevDate != default(DateTime)){
				AbsPath prevPath = myModel.ParentXml.BasePath.Combine(prevDate.Year);
				string prevTitle = string.Format("{0}の{1}", prevDate.ToString(YearFormat), myModel.TopicSuffix);
				Prev = new LinkItem(prevPath, prevTitle);
			}

			DateTime nextDate = GetNextDate(newestTopic.Date);
			if(nextDate != default(DateTime)){
				AbsPath nextPath = myModel.ParentXml.BasePath.Combine(nextDate.Year);
				string nextTitle = string.Format("{0}の{1}", nextDate.ToString(YearFormat), myModel.TopicSuffix);
				Next = new LinkItem(nextPath, nextTitle);
			}

			return Response;
		}


	} // End class
} // End Namespace Bakera



