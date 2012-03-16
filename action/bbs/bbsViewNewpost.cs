using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace Bakera.Hatomaru{

/// <summary>
/// 鳩丸掲示板を処理するクラスです。
/// </summary>
	public partial class BbsViewNewPost : BbsAction{

		public new const string Id = "newpost";
		public new const string Label = "新規投稿";

// コンストラクタ

		/// <summary>
		/// 新規投稿フォーム表示のためのアクションのインスタンスを開始します。
		/// </summary>
		public BbsViewNewPost(HatomaruBbs model, AbsPath path) : base(model, path){
			myPath = myModel.BasePath.Combine(Id);
		}


		/// <summary>
		/// データを GET し、HatomaruResponse を取得します。
		/// </summary>
		protected override HatomaruResponse GetHtmlResponse(){
			HatomaruBbs.SetAllReplaceUrl(Html);
			string title = "新規投稿フォーム";
			Response.SelfTitle = title;
			Response.AddTopicPath(Path, title);
			InsertHeading(2, title);
			XmlNode result = GetForm();
			Html.Entry.AppendChild(result);
			return Response;
		}




	} // End class
} // End Namespace Bakera



