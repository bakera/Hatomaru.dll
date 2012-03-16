using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace Bakera.Hatomaru{

/// <summary>
/// 鳩丸掲示板を処理するクラスです。
/// </summary>
	public partial class BbsViewInThread : BbsAction{

		public new const string Id = "inthread";

// コンストラクタ

		/// <summary>
		/// スレッド内記事表示のためのアクションのインスタンスを開始します。
		/// </summary>
		public BbsViewInThread(HatomaruBbs model, AbsPath path, int page) : base(model, path){
			myPageNum = page;
			myPath = myModel.BasePath.Combine(Id, myPageNum.ToString());
		}


		/// <summary>
		/// データを GET し、HatomaruResponse を取得します。
		/// </summary>
		protected override HatomaruResponse GetHtmlResponse(){
			BbsThread bt = Bbs.GetThread(myPageNum);
			if(bt == null) return NotFound();
			if(bt.Id != myPageNum) return Redirect(myModel.BasePath.Combine(Id, bt.Id));
			HatomaruBbs.SetAllReplaceUrl(Html);
			string title = string.Format("スレッド内全記事表示 (記事 {0} からのスレッド)", bt.Id);
			Response.SelfTitle = title;
			Response.AddTopicPath(Path, title);
			InsertHeading(2, title);
			Article[] articles = bt.AllArticles;

			if(bt.CommentTo != null){
				Html.Append(GetCommentToDesc(bt));
			}

			for(int i=0; i< articles.Length; i++){
				Html.Append(ParseArticle(articles[i]));
			}
			// スレッドツリーの表示
			Html.Append(GetThread(bt));
			return Response;
		}



	} // End class
} // End Namespace Bakera



