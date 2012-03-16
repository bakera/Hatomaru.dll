using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace Bakera.Hatomaru{

/// <summary>
/// 鳩丸掲示板を処理するクラスです。
/// </summary>
	public partial class BbsViewArticle : BbsAction{

		public new const string Id = "article";

// コンストラクタ

		/// <summary>
		/// 個別記事を表示するためのアクションのインスタンスを開始します。
		/// </summary>
		public BbsViewArticle(HatomaruBbs model, AbsPath path, int page) : base(model, path){
			myPageNum = page;
			myPath = myModel.BasePath.Combine(Id, myPageNum.ToString());
		}

		/// <summary>
		/// データを GET し、HatomaruResponse を取得します。
		/// </summary>
		protected override HatomaruResponse GetHtmlResponse(){
			HatomaruBbs.SetAllReplaceUrl(Html);
			Article a = Bbs.GetArticle(myPageNum);
			if(a == null) return NotFound();
			string title = a.Title;
			Response.SelfTitle = title;
			Response.AddTopicPath(Path, title);
			InsertHeading(2, string.Format("記事個別表示 ({0})", myPageNum));

			if(a.Thread.CommentTo != null){
				Html.Append(GetCommentToDesc(a.Thread));
			}

			Html.Entry.AppendChild(ParseArticle(a));
			// スレッドの表示
			XmlElement commentNav = Html.Create("div", "comment-nav");
			commentNav.AppendChild(GetThread(a.Thread));
			Html.Entry.AppendChild(commentNav);
			// コメントフォームの表示
			if(!a.IsSpam){
				Article commentA = a.GetCommentArticle();
				Html.Entry.AppendChild(GetForm(commentA));
			}
			return Response;
		}




	} // End class
} // End Namespace Bakera



