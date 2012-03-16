using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace Bakera.Hatomaru{

/// <summary>
/// ���یf������������N���X�ł��B
/// </summary>
	public partial class BbsViewRootList : BbsAction{

		public new const string Id = "rootlist";
		public new const string Label = "���[�g�L���ꗗ";
		protected new const int ItemPerPage = 50;

// �R���X�g���N�^

		/// <summary>
		/// ���[�g�L���ꗗ�\���̂��߂̃A�N�V�����̃C���X�^���X���J�n���܂��B
		/// </summary>
		public BbsViewRootList(HatomaruBbs model, AbsPath path, int page) : base(model, path){
			if(page <= 1){
				myPageNum = 1;
				myPath = myModel.BasePath.Combine(Id);
			} else {
				myPageNum = page;
				myPath = myModel.BasePath.Combine(Id, myPageNum.ToString());
			}
		}


		/// <summary>
		/// �f�[�^�� GET ���AHatomaruResponse ���擾���܂��B
		/// </summary>
		protected override HatomaruResponse GetHtmlResponse(){
			HatomaruBbs.SetAllReplaceUrl(Html);
			BbsThread[] threads = Bbs.GetAllThread();
			Pager pg = new Pager();
			pg.TotalItem = threads.Length;
			pg.ItemPerPage = ItemPerPage;
			pg.CurrentPage = myPageNum;
			pg.DescOrder = true;

			if(!pg.ExistsPage) return NotFound();

			string title = string.Format("{0} ({1}/{2})", Label, pg.CurrentPage, pg.LastPage);
			Response.SelfTitle = title;
			Response.AddTopicPath(Path, title);

			InsertHeading(2, title);
			InsertPageNav(pg, myModel.BasePath.Combine(Id));
			XmlElement treeDiv = Html.Create("div", "rootlist");
			XmlElement treeUl = Html.Create("ul");
			foreach(int i in pg.ItemIndexes){
				int threadNum = threads[i].Id;
				Article currentArticle = Bbs.GetArticle(threadNum);
				XmlElement treeLi = Html.Create("li");
				treeLi.AppendChild(MakeArticleAnchor(currentArticle));
				treeUl.AppendChild(treeLi);
			}
			treeDiv.AppendChild(treeUl);
			Html.Entry.AppendChild(treeDiv);
			InsertPageNav(pg, myModel.BasePath.Combine(Id));
			return Response;
		}




	} // End class
} // End Namespace Bakera



