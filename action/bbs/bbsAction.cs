using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Xml;

namespace Bakera.Hatomaru{

/// <summary>
/// BBS �𐧌䂷��N���X�ł��B
/// </summary>
	public abstract class BbsAction : HatomaruGetAction{

		public const string Id = null;
		public const string Label = null;
		protected int myPageNum;
		protected const int ItemPerPage = 20;

		public const string NavDescClass = "navdesc";

// �R���X�g���N�^

		protected BbsAction(HatomaruBbs model, AbsPath path) : base(model, path){}


// �v���e�N�g���\�b�h

		// Bbs �̃T�u�i�r�� LinkItem ���擾���܂��B
		protected override LinkItem[] GetSubNav(){
			return new LinkItem[]{
				GetActionLink(typeof(BbsViewOrder)),
				GetActionLink(typeof(BbsViewRootList)),
				GetActionLink(typeof(BbsViewNewPost)),
			};
		}

		/// <summary>
		/// �n���ꂽ BbsThread �̎q�������X���b�h��\������ XmlElement ��Ԃ��܂��B
		/// </summary>
		protected XmlNode GetThread(BbsThread bt){
			return GetThread(bt, false);
		}
		/// <summary>
		/// �n���ꂽ BbsThread �̎q�������X���b�h��\������ XmlElement ��Ԃ��܂��B
		/// toolong �� true �̏ꍇ�A�q�̃X���b�h�͏ȗ�����܂��B
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
				rootLi.AppendChild(Html.P("threadabort", "(���̃X���b�h�͒������邽�߁A�ȗ�����܂���)"));
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



		// �L���ւ̃����N�ƁA���̎q�v�f�����ւ̃����N���쐬���܂��B
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

		// �X���b�h�S�L���\���̃����N�𐶐����܂��B
		protected XmlElement MakeInthreadAnchor(BbsThread bt){
			string inthreadLinkText = string.Format("�S��: [{0}]{1}����̃X���b�h({2}��)]", bt.Id, bt.FirstArticle.Subject, bt.Count);
			Uri linkUri = Bbs.BasePath.Combine(BbsViewInThread.Id, bt.Id);
			return Html.A(linkUri, null, inthreadLinkText);
		}



		


	} // End class BbsController
} // End Namespace Bakera



