using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace Bakera.Hatomaru{

/// <summary>
/// ���یf������������N���X�ł��B
/// </summary>
	public partial class BbsViewOrder : BbsAction{

		public new const string Id = "order";
		public new const string Label = "���e���\��";

// �R���X�g���N�^

		/// <summary>
		/// �L���𓊍e���\������A�N�V�����̃C���X�^���X���J�n���܂��B
		/// </summary>
		public BbsViewOrder(HatomaruBbs model, AbsPath path, int page) : base(model, path){
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
			Article[] articles = Bbs.GetAllArticle();
			Pager pg = new Pager();
			pg.TotalItem = articles.Length;
			pg.ItemPerPage = ItemPerPage;
			pg.CurrentPage = myPageNum;
			pg.DescOrder = true;

			if(!pg.ExistsPage) return NotFound();

			string title = string.Format("{0} ({1}/{2})", Label, pg.CurrentPage, pg.LastPage);
			Response.SelfTitle = title;
			Response.AddTopicPath(Path, title);

			InsertHeading(2, title);
			InsertPageNav(pg, myModel.BasePath.Combine(Id));
			foreach(int i in pg.ItemIndexes){
				Html.Entry.AppendChild(ParseArticle(articles[i]));
			}
			InsertPageNav(pg, myModel.BasePath.Combine(Id));
			return Response;
		}




	} // End class
} // End Namespace Bakera



