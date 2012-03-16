using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace Bakera.Hatomaru{

/// <summary>
/// 最近の日記を表示するアクションです。
/// </summary>
	public class DiaryIndexViewRecently : PartialDiaryAction{

		public const string Label = "最近一週間ほどのえび日記";

// コンストラクタ

		/// <summary>
		/// 最近の日記表示のためのアクションのインスタンスを開始します。
		/// </summary>
		public DiaryIndexViewRecently(DiaryIndex model, AbsPath path) : base(model, path){
			myPath = myModel.BasePath;
		}


		/// <summary>
		/// データを GET し、HatomaruResponse を取得します。
		/// </summary>
		protected override HatomaruResponse GetHtmlResponse(){
			Topic[] topics = GetRecentTopics();
			if(topics.Length == 0) return NotFound();
			Topic oldestTopic = topics[topics.Length-1];
			
			int year = oldestTopic.Date.Year;
			YearDiary oldestDiary = Diary.GetYearDiary(year);

			Response.SelfTitle = Label;
			InsertHeading(2, Label);
			Html.Append(DiaryListWithDate(topics, 3));

			// 次ページ (一つ前の日)
			DateTime prevDate = GetPrevDate(oldestTopic.Date);
			if(prevDate != default(DateTime)){
				AbsPath prevPath = myModel.BasePath.Combine(prevDate.Year, prevDate.Month, prevDate.Day);
				string prevTitle = string.Format("{0}の{1}", prevDate.ToString(DateFormat), myModel.TopicSuffix);
				Prev = new LinkItem(prevPath, prevTitle);
			}


			//RSSオートディスカバリ
			LinkItem rssLink = new LinkItem(BasePath.Combine(DiaryIndexRss.Id), Label + "(RSS1.0)");
			Html.AddLinkRel("alternate", HatomaruResponse.RssMediaType, rssLink);

			LinkItem atomLink = new LinkItem(BasePath.Combine(DiaryIndexAtom.Id), Label + "(Atom1.0)");
			Html.AddLinkRel("alternate", HatomaruResponse.AtomMediaType, atomLink);

			return Response;
		}




	} // End class
} // End Namespace Bakera



