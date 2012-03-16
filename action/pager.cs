using System;
using System.Xml;
using System.Collections.Generic;


namespace Bakera.Hatomaru{

	/// <summary>
	/// BBS �̃y�[�W������Ǘ�����N���X�ł��B
	/// ���ڑS��/1�y�[�W�̌���/���݂̃y�[�W���w�肷��ƁA���Ԗڂ��牽�Ԗڂ̍��ڂ�\������̂��Ԃ��܂��B
	/// �~���̏ꍇ��
	/// </summary>
	public class Pager{

		private int myItemPerPage;
		private int myTotalItem;
		private int myCurrentPage;
		private bool myDescOrder;

		private int myNavPrevItems = 5;
		private int myNavNextItems = 5;


// �R���X�g���N�^



// �v���p�e�B

		/// <summary>
		/// �����ڐ���ݒ�E�擾���܂��B
		/// </summary>
		public int TotalItem{
			get{return myTotalItem;}
			set{myTotalItem = value;}
		}

		/// <summary>
		/// 1�y�[�W�̍��ڐ���ݒ�E�擾���܂��B
		/// </summary>
		public int ItemPerPage{
			get{return myItemPerPage;}
			set{myItemPerPage = value;}
		}

		/// <summary>
		/// ���݂̃y�[�W��ݒ�E�擾���܂��B
		/// </summary>
		public int CurrentPage{
			get{return myCurrentPage;}
			set{myCurrentPage = value;}
		}

		/// <summary>
		/// ���ڂ��~���ł���� true ��ݒ肵�܂��B
		/// </summary>
		public bool DescOrder{
			get{return myDescOrder;}
			set{myDescOrder = value;}
		}

		/// <summary>
		/// �i�r�Q�[�V�����̌��݃y�[�W�ȍ~�̕\��������ݒ�E�擾���܂��B
		/// </summary>
		public int NavNextItems{
			get{return myNavNextItems;}
			set{myNavNextItems = value;}
		}

		/// <summary>
		/// �i�r�Q�[�V�����̌��݃y�[�W�ȑO�̕\��������ݒ�E�擾���܂��B
		/// </summary>
		public int NavPrevItems{
			get{return myNavPrevItems;}
			set{myNavPrevItems = value;}
		}



// �擾��p�v���p�e�B

		/// <summary>
		/// �J�n�ԍ����擾���܂��B
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
		/// �I���ԍ����擾���܂��B
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
		/// �J�n����I���܂ł̃C���f�N�X�ԍ��𓾂܂��B
		/// ������̂� 0�` �̃C���f�N�X�ԍ��ł��B
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
		/// ���݃y�[�W�����݂��Ă���� true ��Ԃ��܂��B
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
		/// �ŏI�y�[�W���擾���܂��B
		/// </summary>
		public int LastPage{
			get{
				// �؂�グ
				int lastPageNum = (myTotalItem + myItemPerPage - 1) / myItemPerPage;
				return lastPageNum;
			}
		}

		/// <summary>
		/// �y�[�W�i�r�Q�[�V�������擾���܂��B
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
				prevLink.InnerText = "�O�̃y�[�W";
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
				nextLink.InnerText = "���̃y�[�W";
				nextLink.SetAttribute("rel", "next");
				pageNav.AppendChild(html.Text(" "));
				pageNav.AppendChild(nextLink);
			}

			result.AppendChild(pageNav);
			return result;
		}

	}

}

