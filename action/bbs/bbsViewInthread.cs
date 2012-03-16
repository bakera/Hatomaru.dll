using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace Bakera.Hatomaru{

/// <summary>
/// ���یf������������N���X�ł��B
/// </summary>
	public partial class BbsViewInThread : BbsAction{

		public new const string Id = "inthread";

// �R���X�g���N�^

		/// <summary>
		/// �X���b�h���L���\���̂��߂̃A�N�V�����̃C���X�^���X���J�n���܂��B
		/// </summary>
		public BbsViewInThread(HatomaruBbs model, AbsPath path, int page) : base(model, path){
			myPageNum = page;
			myPath = myModel.BasePath.Combine(Id, myPageNum.ToString());
		}


		/// <summary>
		/// �f�[�^�� GET ���AHatomaruResponse ���擾���܂��B
		/// </summary>
		protected override HatomaruResponse GetHtmlResponse(){
			BbsThread bt = Bbs.GetThread(myPageNum);
			if(bt == null) return NotFound();
			if(bt.Id != myPageNum) return Redirect(myModel.BasePath.Combine(Id, bt.Id));
			HatomaruBbs.SetAllReplaceUrl(Html);
			string title = string.Format("�X���b�h���S�L���\�� (�L�� {0} ����̃X���b�h)", bt.Id);
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
			// �X���b�h�c���[�̕\��
			Html.Append(GetThread(bt));
			return Response;
		}



	} // End class
} // End Namespace Bakera



