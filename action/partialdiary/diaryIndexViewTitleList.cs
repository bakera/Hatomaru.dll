using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace Bakera.Hatomaru{

/// <summary>
/// 最近の日記を表示するアクションです。
/// </summary>
	public partial class DiaryIndexViewTitleList : PartialDiaryAction{

		public const string Id = "titlelist";
		public const string Label = "見出し一覧";


// コンストラクタ

		/// <summary>
		/// 日記の見出し一覧表示のためのアクションのインスタンスを開始します。
		/// </summary>
		public DiaryIndexViewTitleList(DiaryIndex model, AbsPath path) : base(model, path){
			myPath = myModel.BasePath.Combine(Id);
		}


		/// <summary>
		/// データを GET し、HatomaruResponse を取得します。
		/// </summary>
		protected override HatomaruResponse GetHtmlResponse(){
			Topic[] topics = GetAllTopics();
			if(topics.Length == 0) return NotFound();
			Response.SelfTitle = Label;
			Response.AddTopicPath(Path, Label);
			InsertHeading(2, Label);
			Html.Append(DiaryHeadingList(topics, 3));
			return Response;
		}


	} // End class
} // End Namespace Bakera



