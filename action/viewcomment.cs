using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace Bakera.Hatomaru{

/// <summary>
/// コメントを見るためのアクションです。
/// </summary>
	public class ViewComment : HatomaruGetAction{

		AbsPath myCommentToPath;

// コンストラクタ

		/// <summary>
		/// コメントを見るアクションのインスタンスを開始します。
		/// </summary>
		public ViewComment(HatomaruXml model, AbsPath path, AbsPath commentToPath) : base(model, path){
			myCommentToPath = commentToPath;
			myPath = myCommentToPath.Combine(CommentPath);
		}

		/// <summary>
		/// データを GET し、HatomaruResponse を取得します。
		/// </summary>
		protected override HatomaruResponse GetHtmlResponse(){
			HatomaruResponse parentResponse = myModel.Manager.GetResponse(myCommentToPath);
			if(parentResponse == null || parentResponse is NotFoundResponse || parentResponse is RedirectResponse) return NotFound();

			string title = string.Format("「{0}」への{1}", parentResponse.SelfTitle, CommentName);
			foreach(LinkItem li in parentResponse.TopicPath){
				Response.AddTopicPath(li);
			}
			Response.AddTopicPath(myPath, title);
			Response.SelfTitle = title;
			Response.BaseTitle = parentResponse.BaseTitle;
			InsertHeading(2, title);

			BbsThread bt = Bbs.GetCommentToThread(myCommentToPath);
			Response.AddDataSource(Bbs);
			XmlElement linkA = GetCommentToLink(parentResponse);
			string commentMessage;
			string commentTitle = Article.CommentPrefix + string.Format("「{0}」", parentResponse.SelfTitle);
			if(bt == null || bt.Articles.Length == 0){
				commentMessage = "について、コメントはまだ書かれていません。コメントを書く場合は、以下のフォームに記入してください。";
				Html.Append(Html.P(BbsAction.NavDescClass, "「", linkA, "」", commentMessage));
			} else {
				commentMessage = string.Format("について、{0}件のコメントが書かれています。", bt.Count);
				Html.Append(Html.P(BbsAction.NavDescClass, "「", linkA, "」", commentMessage));
				foreach(Article a in bt.AllArticles){
					Html.Append(ParseArticle(a));
				}
				Html.Append(Html.P(BbsAction.NavDescClass, "「", linkA.Clone(), "」についてコメントを書く場合は、以下のフォームに記入してください。"));
			}
			Html.Append(GetForm(commentTitle));
			return Response;
		}




	} // End class
} // End Namespace Bakera



