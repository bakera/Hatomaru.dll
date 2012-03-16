using System;
using System.Xml;
using System.Collections.Generic;


namespace Bakera.Hatomaru{

	/// <summary>
	/// BBS のページ送りを管理するクラスです。
	/// 項目全数/1ページの件数/現在のページを指定すると、何番目から何番目の項目を表示するのか返します。
	/// 降順の場合は
	/// </summary>
	public class Pager{

		private int myItemPerPage;
		private int myTotalItem;
		private int myCurrentPage;
		private bool myDescOrder;

		private int myNavPrevItems = 5;
		private int myNavNextItems = 5;


// コンストラクタ



// プロパティ

		/// <summary>
		/// 総項目数を設定・取得します。
		/// </summary>
		public int TotalItem{
			get{return myTotalItem;}
			set{myTotalItem = value;}
		}

		/// <summary>
		/// 1ページの項目数を設定・取得します。
		/// </summary>
		public int ItemPerPage{
			get{return myItemPerPage;}
			set{myItemPerPage = value;}
		}

		/// <summary>
		/// 現在のページを設定・取得します。
		/// </summary>
		public int CurrentPage{
			get{return myCurrentPage;}
			set{myCurrentPage = value;}
		}

		/// <summary>
		/// 項目が降順であれば true を設定します。
		/// </summary>
		public bool DescOrder{
			get{return myDescOrder;}
			set{myDescOrder = value;}
		}

		/// <summary>
		/// ナビゲーションの現在ページ以降の表示件数を設定・取得します。
		/// </summary>
		public int NavNextItems{
			get{return myNavNextItems;}
			set{myNavNextItems = value;}
		}

		/// <summary>
		/// ナビゲーションの現在ページ以前の表示件数を設定・取得します。
		/// </summary>
		public int NavPrevItems{
			get{return myNavPrevItems;}
			set{myNavPrevItems = value;}
		}



// 取得専用プロパティ

		/// <summary>
		/// 開始番号を取得します。
		/// </summary>
		public int StartNum{
			get{
				int startNum = myItemPerPage * (myCurrentPage - 1) + 1;
				if(startNum < 0) startNum = 0;
				if(myDescOrder){
					startNum = myTotalItem - startNum + 1;
				}
				return startNum;
			}
		}

		/// <summary>
		/// 終了番号を取得します。
		/// </summary>
		public int EndNum{
			get{
				int endNum = myItemPerPage * myCurrentPage;
				if(endNum > myTotalItem) endNum = myTotalItem;
				if(myDescOrder){
					endNum = myTotalItem - endNum + 1;
				}
				if(endNum < 0) endNum = 0;
				return endNum;
			}
		}

		/// <summary>
		/// 開始から終了までのインデクス番号を得ます。
		/// 得られるのは 0～ のインデクス番号です。
		/// </summary>
		public IEnumerable<int> ItemIndexes{
			get{
				if(StartNum > EndNum){
					for(int i = StartNum - 1; i >= EndNum - 1; i--){
						yield return i;
					}
				}
				for(int i = StartNum - 1; i < EndNum; i++){
					yield return i;
				}
			}
		}

		/// <summary>
		/// 現在ページが存在していれば true を返します。
		/// </summary>
		public bool ExistsPage{
			get{
				if(myItemPerPage <= 0) return false;
				if(myCurrentPage <= 0) return false;
				if(myCurrentPage > LastPage) return false;
				return true;
			}
		}

		/// <summary>
		/// 最終ページを取得します。
		/// </summary>
		public int LastPage{
			get{
				// 切り上げ
				int lastPageNum = (myTotalItem + myItemPerPage - 1) / myItemPerPage;
				return lastPageNum;
			}
		}

		/// <summary>
		/// ページナビゲーションを取得します。
		/// </summary>
		public XmlNode GetPageNav(Xhtml html, AbsPath uriPrefix){

			int startPos = CurrentPage - myNavPrevItems;
			if(startPos > LastPage - myNavPrevItems - myNavNextItems) startPos = LastPage - myNavPrevItems - myNavNextItems;
			if(startPos < 3) startPos = 1;
			int endPos = CurrentPage + myNavNextItems;
			if(endPos < myNavPrevItems + myNavNextItems) endPos = myNavPrevItems + myNavNextItems;
			if(endPos > LastPage - 2) endPos = LastPage;

			XmlDocumentFragment result = html.CreateDocumentFragment();
			XmlElement pageNav = html.P("pageNav");

			if(CurrentPage > 1){
				XmlElement prevLink = html.A(uriPrefix.Combine((CurrentPage-1).ToString()));
				prevLink.InnerText = "前のページ";
				prevLink.SetAttribute("rel", "prev");
				pageNav.AppendChild(prevLink);
				pageNav.AppendChild(html.Text(" "));
			}

			if(startPos > 1){
				pageNav.AppendChild(html.GetPageLink(uriPrefix, 1));
				pageNav.AppendChild(html.Span("omitted", "..."));
			}
			for(int i = startPos; i <= endPos; i++){
				if(i > startPos){
					pageNav.AppendChild(html.Span("separate", "/"));
				}
				pageNav.AppendChild(html.GetPageLink(uriPrefix, i));
			}
			if(endPos < LastPage){
				pageNav.AppendChild(html.Span("omitted", "..."));
				pageNav.AppendChild(html.GetPageLink(uriPrefix, LastPage));
			}

			if(CurrentPage < LastPage){
				XmlElement nextLink = html.A(uriPrefix.Combine((CurrentPage+1).ToString()));
				nextLink.InnerText = "次のページ";
				nextLink.SetAttribute("rel", "next");
				pageNav.AppendChild(html.Text(" "));
				pageNav.AppendChild(nextLink);
			}

			result.AppendChild(pageNav);
			return result;
		}

	}

}

