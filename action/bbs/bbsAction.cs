using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Xml;

namespace Bakera.Hatomaru{

/// <summary>
/// BBS を制御するクラスです。
/// </summary>
	public abstract class BbsAction : HatomaruGetAction{

		public const string Id = null;
		public const string Label = null;
		protected int myPageNum;
		protected const int ItemPerPage = 20;

		public const string NavDescClass = "navdesc";

// コンストラクタ

		protected BbsAction(HatomaruBbs model, AbsPath path) : base(model, path){}


// プロテクトメソッド

		// Bbs のサブナビの LinkItem を取得します。
		protected override LinkItem[] GetSubNav(){
			return new LinkItem[]{
				GetActionLink(typeof(BbsViewOrder)),
				GetActionLink(typeof(BbsViewRootList)),
				GetActionLink(typeof(BbsViewNewPost)),
			};
		}

		/// <summary>
		/// 渡された BbsThread の子たちをスレッドを表示する XmlElement を返します。
		/// </summary>
		protected XmlNode GetThread(BbsThread bt){
			return GetThread(bt, false);
		}
		/// <summary>
		/// 渡された BbsThread の子たちをスレッドを表示する XmlElement を返します。
		/// toolong が true の場合、子のスレッドは省略されます。
		/// </summary>
		protected XmlNode GetThread(BbsThread bt, bool toolong){
			XmlElement result = Html.Create("ul", "tree");

			XmlElement rootLi = Html.Create("li", "root");
			result.AppendChild(rootLi);
			XmlElement ia = MakeInthreadAnchor(bt);
			rootLi.AppendChild(ia);
			rootLi.AppendChild(Html.Space);

			if(bt.CommentTo != null){
				XmlElement commentToP = GetCommentToDesc(bt);
				rootLi.PrependChild(commentToP);
			}

			if(toolong){
				rootLi.AppendChild(Html.P("threadabort", "(このスレッドは長すぎるため、省略されました)"));
			} else {
				XmlElement secondUl = Html.Create("ul");
				rootLi.AppendChild(secondUl);
				for(int i=0; i < bt.Articles.Length; i++){
					Article a = bt.Articles[i];
					XmlElement secondLi = Html.Create("li");
					secondLi.AppendChild(MakeTree(a));
					secondUl.AppendChild(secondLi);
				}
			}
			return result;
		}



		// 記事へのリンクと、その子要素たちへのリンクを作成します。
		protected XmlNode MakeTree(Article a){
			XmlNode result = Html.CreateDocumentFragment();
			result.AppendChild(MakeArticleAnchor(a));
			result.AppendChild(Html.Text(" : "));
			result.AppendChild(NameAndDate(a));
			
			if(a.Children.Length > 0){
				XmlElement ul = Html.Create("ul");
				foreach(Article child in a.Children){
					XmlElement li = Html.Create("li", null, MakeTree(child));
					ul.AppendChild(li);
				}
				result.AppendChild(ul);
			}
			
			return result;
		}

		// スレッド全記事表示のリンクを生成します。
		protected XmlElement MakeInthreadAnchor(BbsThread bt){
			string inthreadLinkText = string.Format("全読: [{0}]{1}からのスレッド({2}件)]", bt.Id, bt.FirstArticle.Subject, bt.Count);
			Uri linkUri = Bbs.BasePath.Combine(BbsViewInThread.Id, bt.Id);
			return Html.A(linkUri, null, inthreadLinkText);
		}



		


	} // End class BbsController
} // End Namespace Bakera



