using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace Bakera.Hatomaru{

/// <summary>
/// ���یf������������N���X�ł��B
/// </summary>
	public partial class BbsViewThread : BbsAction{

		public new const string Id = "thread";
		public new const string Label = "�X���b�h�ꗗ�\��";
		protected const int ThreadMaxLength = 100; //�X���b�h�̍ő�\������

// �R���X�g���N�^

		/// <summary>
		/// �X���b�h�ꗗ�\���̂��߂̃A�N�V�����̃C���X�^���X���J�n���܂��B
		/// </summary>
		public BbsViewThread(HatomaruBbs model, AbsPath path, int page) : base(model, path){
			if(page <= 1){
				myPageNum = 1;
				myPath = myModel.BasePath;
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
			if(!pg.ExistsPage){
				string mes = string.Format("{0}�y�[�W�͂���܂���B(�L��{1}��/�S{2}�y�[�W)", myPageNum, pg.TotalItem, pg.LastPage);
				return NotFound(mes);
			}
			string title = string.Format("{0} ({1}/{2})", Label, pg.CurrentPage, pg.LastPage);
			Response.SelfTitle = title;
			if(myPageNum > 1){
				Response.AddTopicPath(Path, title);
			}
			InsertHeading(2, title);
			InsertPageNav(pg, myModel.BasePath.Combine(Id));
			XmlElement treeDiv = Html.Create("div", "tree");
			foreach(int i in pg.ItemIndexes){
				BbsThread target = threads[i];
				if(target.Count > ThreadMaxLength){
					treeDiv.AppendChild(GetThread(target, true));
				} else {
					treeDiv.AppendChild(GetThread(target));
				}
			}
			Html.Entry.AppendChild(treeDiv);
			InsertPageNav(pg, myModel.BasePath.Combine(Id));
			return Response;
		}




	} // End class
} // End Namespace Bakera



