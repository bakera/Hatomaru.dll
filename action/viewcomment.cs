using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace Bakera.Hatomaru{

/// <summary>
/// �R�����g�����邽�߂̃A�N�V�����ł��B
/// </summary>
	public class ViewComment : HatomaruGetAction{

		AbsPath myCommentToPath;

// �R���X�g���N�^

		/// <summary>
		/// �R�����g������A�N�V�����̃C���X�^���X���J�n���܂��B
		/// </summary>
		public ViewComment(HatomaruXml model, AbsPath path, AbsPath commentToPath) : base(model, path){
			myCommentToPath = commentToPath;
			myPath = myCommentToPath.Combine(CommentPath);
		}

		/// <summary>
		/// �f�[�^�� GET ���AHatomaruResponse ���擾���܂��B
		/// </summary>
		protected override HatomaruResponse GetHtmlResponse(){
			HatomaruResponse parentResponse = myModel.Manager.GetResponse(myCommentToPath);
			if(parentResponse == null || parentResponse is NotFoundResponse || parentResponse is RedirectResponse) return NotFound();

			string title = string.Format("�u{0}�v�ւ�{1}", parentResponse.SelfTitle, CommentName);
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
			string commentTitle = Article.CommentPrefix + string.Format("�u{0}�v", parentResponse.SelfTitle);
			if(bt == null || bt.Articles.Length == 0){
				commentMessage = "�ɂ��āA�R�����g�͂܂�������Ă��܂���B�R�����g�������ꍇ�́A�ȉ��̃t�H�[���ɋL�����Ă��������B";
				Html.Append(Html.P(BbsAction.NavDescClass, "�u", linkA, "�v", commentMessage));
			} else {
				commentMessage = string.Format("�ɂ��āA{0}���̃R�����g��������Ă��܂��B", bt.Count);
				Html.Append(Html.P(BbsAction.NavDescClass, "�u", linkA, "�v", commentMessage));
				foreach(Article a in bt.AllArticles){
					Html.Append(ParseArticle(a));
				}
				Html.Append(Html.P(BbsAction.NavDescClass, "�u", linkA.Clone(), "�v�ɂ��ăR�����g�������ꍇ�́A�ȉ��̃t�H�[���ɋL�����Ă��������B"));
			}
			Html.Append(GetForm(commentTitle));
			return Response;
		}




	} // End class
} // End Namespace Bakera



