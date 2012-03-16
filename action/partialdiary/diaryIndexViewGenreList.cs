using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace Bakera.Hatomaru{

/// <summary>
/// ジャンルを表示するアクションです。
/// </summary>
	public partial class DiaryIndexViewGenreList : PartialDiaryAction{

		public const string Id = "genre";
		public const string Label = "話題一覧";

// コンストラクタ

		/// <summary>
		/// ジャンル表示のためのアクションのインスタンスを開始します。
		/// </summary>
		public DiaryIndexViewGenreList(DiaryIndex model, AbsPath path) : base(model, path){
			myPath = BasePath.Combine(Id);
		}

		/// <summary>
		/// データを GET し、HatomaruResponse を取得します。
		/// </summary>
		protected override HatomaruResponse GetHtmlResponse(){
			TopicGenre[] genres = GetGenreList();
			XmlNode result = Html.Create("ul");
			for(int i=0; i < genres.Length; i++){
				XmlElement genreA = Html.A(BasePath.Combine(Id, genres[i].Id.PathEncode()));
				genreA.InnerText = genres[i].Id;
				XmlElement countSpan = Html.Span("count", "(" + genres[i].Count.ToString() + ")");
				genreA.AppendChild(countSpan);
				result.AppendChild(Html.Create("li", null, genreA));
			}

			Response.SelfTitle = Label;
			Response.AddTopicPath(Path, Label);
			InsertHeading(2, Label);
			Html.Append(result);
			return Response;
		}

	} // End class
} // End Namespace Bakera



