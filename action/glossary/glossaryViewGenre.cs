using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace Bakera.Hatomaru{

/// <summary>
/// 用語の一覧を表示するアクションです。
/// </summary>
	public partial class GlossaryViewGenre : GlossaryAction{

		public const string Id = "genre";
		public new const string Label = "ジャンル一覧";
		private string myGenreName;

// コンストラクタ

		/// <summary>
		/// ジャンル表示のためのアクションのインスタンスを開始します。
		/// </summary>
		public GlossaryViewGenre(HatomaruGlossary model, AbsPath path, string name) : base(model, path){
			myPath = BasePath.Combine(Id, name);
			myGenreName = name;
		}


		/// <summary>
		/// データを GET し、HatomaruResponse を取得します。
		/// </summary>
		protected override HatomaruResponse GetHtmlResponse(){
			GlossaryGenre gg = Glossary.GetGenre(myGenreName.PathDecode());
			if(gg == null){
				// 後方互換性リダイレクト
				string newName = myGenreName.Base16ToString();
				gg = Glossary.GetGenre(newName);
				if(gg != null){
					AbsPath newPath = BasePath.Combine(Id, newName.PathEncode());
					return Redirect(newPath);
				}
				return NotFound();
			}
			GlossaryWord[] gws = gg.Glossarys;
			if(gws.Length == 0) return NotFound();
			string title = string.Format("「{0}」に関する用語", gg.Name);
			Response.SelfTitle = title;
			Response.AddTopicPath(BasePath.Combine(Id), Label);
			Response.AddTopicPath(Path, title);
			InsertHeading(2, title);
			XmlElement p = WordList(gws);
			Html.Append(p);
			return Response;
		}

	} // End class
} // End Namespace Bakera



