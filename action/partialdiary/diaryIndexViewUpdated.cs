using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace Bakera.Hatomaru{

/// <summary>
/// 最近の日記を表示するアクションです。
/// </summary>
	public partial class DiaryIndexViewUpdated : PartialDiaryAction{

		public const string Id = "updated";
		public const string Label = "更新・追記された日記";

// コンストラクタ

		/// <summary>
		/// 日記の見出し一覧表示のためのアクションのインスタンスを開始します。
		/// </summary>
		public DiaryIndexViewUpdated(DiaryIndex model, AbsPath path) : base(model, path){
			myPath = myModel.BasePath.Combine(Id);
		}


		/// <summary>
		/// データを GET し、HatomaruResponse を取得します。
		/// </summary>
		protected override HatomaruResponse GetHtmlResponse(){

			Topic[] topics = GetUpdatedTopics();
			if(topics.Length == 0) return NotFound();
			Response.SelfTitle = Label;
			Response.AddTopicPath(Path, Label);
			InsertHeading(2, Label);
			XmlElement ul = Html.Create("ul");
			foreach(Topic t in topics){
				XmlElement li = Html.Create("li", null, MakeTopicAnchor(t), Html.Space, GetUpdated(t));
				ul.AppendChild(li);
			}
			Html.Append(ul);

			LinkItem atomLink = new LinkItem(BasePath.Combine(Id, DiaryIndexUpdatedAtom.Id), DiaryIndexUpdatedAtom.Label + "(Atom1.0)");
			Html.AddLinkRel("alternate", HatomaruResponse.AtomMediaType, atomLink);

			return Response;
		}


	} // End class
} // End Namespace Bakera



