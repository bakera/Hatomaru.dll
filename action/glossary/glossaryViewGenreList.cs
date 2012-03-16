using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace Bakera.Hatomaru{

/// <summary>
/// 用語のジャンル一覧を表示するアクションです。
/// </summary>
	public partial class GlossaryViewGenreList : GlossaryAction{

		public const string Id = "genre";
		public new const string Label = "ジャンル一覧";

// コンストラクタ

		/// <summary>
		/// ジャンル表示のためのアクションのインスタンスを開始します。
		/// </summary>
		public GlossaryViewGenreList(HatomaruGlossary model, AbsPath path) : base(model, path){
			myPath = BasePath.Combine(Id);
		}


		/// <summary>
		/// データを GET し、HatomaruResponse を取得します。
		/// </summary>
		protected override HatomaruResponse GetHtmlResponse(){
			GlossaryGenre[] genres = Glossary.GetGenreList();
			XmlNode result = Html.Create("ul");
			for(int i=0; i < genres.Length; i++){
				XmlElement genreA = Html.A(BasePath.Combine(Id, genres[i].Name.PathEncode()));
				genreA.InnerText = genres[i].Name;
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



