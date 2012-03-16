using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace Bakera.Hatomaru{

/// <summary>
/// ジャンルを表示するアクションです。
/// </summary>
	public partial class DiaryIndexViewGenre : PartialDiaryAction{

		public const string Id = "genre";
		public const string Label = "話題一覧";
		private string myWord;

// コンストラクタ

		/// <summary>
		/// ジャンル表示のためのアクションのインスタンスを開始します。
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
		/// データを GET し、HatomaruResponse を取得します。
		/// </summary>
		protected override HatomaruResponse GetHtmlResponse(){
			Topic[] topics = GetTopicsByGenre(myWord.PathDecode());
			if(topics == null){
				// 後方互換性リダイレクト
				string newWord = myWord.Base16ToString();
				topics = GetTopicsByGenre(newWord);
				if(topics != null){
					AbsPath newPath = BasePath.Combine(Id, newWord.PathEncode());
					return Redirect(newPath);
				}
			}
			
			if(topics == null || topics.Length == 0) return NotFound();
			string title = string.Format("話題「{0}」を含む{1}", myWord.PathDecode(), myModel.TopicSuffix);
			Response.SelfTitle = title;
			Response.AddTopicPath(BasePath.Combine(Id), Label);
			Response.AddTopicPath(Path, title);
			InsertHeading(2, title);
			Html.Append(DiaryHeadingList(topics, 3));
			return Response;
		}

	} // End class
} // End Namespace Bakera



