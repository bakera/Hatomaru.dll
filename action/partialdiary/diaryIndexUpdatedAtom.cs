using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace Bakera.Hatomaru{

/// <summary>
/// 最近の日記を表示するアクションです。
/// </summary>
	public partial class DiaryIndexUpdatedAtom : DiaryIndexAtom{

		public new const string Id = "atom";
		public const string Label = "更新・追記されたえび日記";

// コンストラクタ

		/// <summary>
		/// 日記の見出し一覧表示のためのアクションのインスタンスを開始します。
		/// </summary>
		public DiaryIndexUpdatedAtom(DiaryIndex model, AbsPath path) : base(model, path){
			myPath = myModel.BasePath.Combine(DiaryIndexViewUpdated.Id, Id);
		}


		/// <summary>
		/// データを GET し、HatomaruResponse を取得します。
		/// </summary>
		protected override HatomaruResponse GetHtmlResponse(){
			Topic[] topics = GetUpdatedTopics();
			if(topics.Length == 0) return NotFound();
			return GetAtom(topics, Label);
		}


	} // End class
} // End Namespace Bakera



