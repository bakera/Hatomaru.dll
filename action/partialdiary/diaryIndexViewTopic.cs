using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace Bakera.Hatomaru{

/// <summary>
/// 特定の日記を表示するアクションです。
/// </summary>
	public partial class DiaryIndexViewTopic : PartialDiaryAction{

		public const string Id = "topic";
		private int myTopicId;
		private Topic myTopic = null;

// コンストラクタ

		/// <summary>
		/// 特定の日記表示のためのアクションのインスタンスを開始します。
		/// </summary>
		public DiaryIndexViewTopic(DiaryIndex model, AbsPath path, int topicId) : base(model, path){
			myPath = myModel.BasePath.Combine(Id, topicId);
			myTopicId = topicId;
		}


		/// <summary>
		/// データを GET し、HatomaruResponse を取得します。
		/// </summary>
		protected override HatomaruResponse GetHtmlResponse(){
			myTopic = Diary.GetTopic(myTopicId);
			if(myTopic == null) return NotFound();

			Response.AddTopicPath(myPath, myTopic.Title);
			Response.SelfTitle = myTopic.Title;
			InsertHeading(2, myTopic.Title);

			Html.Append(GetDateHeading(myTopic.Date, 3));
			Html.Append(GetTopicBody(myTopic, 4));

			// 次ページ (一つ前の日記)
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
		/// キーワードを取得します。
		/// </summary>
		protected override string GetKeywords(){
			return string.Join(",", myTopic.Genre);
		}





	} // End class
} // End Namespace Bakera



