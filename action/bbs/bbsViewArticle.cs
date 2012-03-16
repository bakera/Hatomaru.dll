using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace Bakera.Hatomaru{

/// <summary>
/// ���یf������������N���X�ł��B
/// </summary>
	public partial class BbsViewArticle : BbsAction{

		public new const string Id = "article";

// �R���X�g���N�^

		/// <summary>
		/// �ʋL����\�����邽�߂̃A�N�V�����̃C���X�^���X���J�n���܂��B
		/// </summary>
		public BbsViewArticle(HatomaruBbs model, AbsPath path, int page) : base(model, path){
			myPageNum = page;
			myPath = myModel.BasePath.Combine(Id, myPageNum.ToString());
		}

		/// <summary>
		/// �f�[�^�� GET ���AHatomaruResponse ���擾���܂��B
		/// </summary>
		protected override HatomaruResponse GetHtmlResponse(){
			HatomaruBbs.SetAllReplaceUrl(Html);
			Article a = Bbs.GetArticle(myPageNum);
			if(a == null) return NotFound();
			string title = a.Title;
			Response.SelfTitle = title;
			Response.AddTopicPath(Path, title);
			InsertHeading(2, string.Format("�L���ʕ\�� ({0})", myPageNum));

			if(a.Thread.CommentTo != null){
				Html.Append(GetCommentToDesc(a.Thread));
			}

			Html.Entry.AppendChild(ParseArticle(a));
			// �X���b�h�̕\��
			XmlElement commentNav = Html.Create("div", "comment-nav");
			commentNav.AppendChild(GetThread(a.Thread));
			Html.Entry.AppendChild(commentNav);
			// �R�����g�t�H�[���̕\��
			if(!a.IsSpam){
				Article commentA = a.GetCommentArticle();
				Html.Entry.AppendChild(GetForm(commentA));
			}
			return Response;
		}




	} // End class
} // End Namespace Bakera



