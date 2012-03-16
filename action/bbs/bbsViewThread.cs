using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace Bakera.Hatomaru{

/// <summary>
/// 鳩丸掲示板を処理するクラスです。
/// </summary>
	public partial class BbsViewThread : BbsAction{

		public new const string Id = "thread";
		public new const string Label = "スレッド一覧表示";
		protected const int ThreadMaxLength = 100; //スレッドの最大表示件数

// コンストラクタ

		/// <summary>
		/// スレッド一覧表示のためのアクションのインスタンスを開始します。
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
		/// データを GET し、HatomaruResponse を取得します。
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
				string mes = string.Format("{0}ページはありません。(記事{1}件/全{2}ページ)", myPageNum, pg.TotalItem, pg.LastPage);
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



