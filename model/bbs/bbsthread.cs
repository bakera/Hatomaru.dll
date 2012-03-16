using System;
using System.Collections.Generic;

namespace Bakera.Hatomaru{

	/// <summary>
	/// ���یf���̈�̃X���b�h�������N���X�ł��B
	/// </summary>
	public class BbsThread : IComparable<BbsThread>{

		private int myId; // �X���b�h�� ID�B�����̋L���̒��ł����Ƃ��ԍ����Ⴂ���̂� ID
		private DateTime myDate;
		private List<Article> myTempArticles = new List<Article>();// �X���b�h�����̋L�����ꎞ�I�ɒǉ�
		private Article[] myArticles;// �X���b�h�����̋L��
		private Article[] myAllArticles;// �X���b�h�ɑ�����S�Ă̋L��
		private AbsPath myCommentTo;// �R�����g��ABbs �����L���� null

// �R���X�g���N�^

		/// <summary>
		/// Article���w�肵�āABbsThread �̐V�����C���X�^���X���J�n���܂��B
		/// </summary>
		public BbsThread(Article a){
			myTempArticles.Add(a);
			myCommentTo = a.CommentTo;
		}


// �v���p�e�B

		/// <summary>
		/// ���̃X���b�h�����̋L���̔z����擾���܂��B
		/// </summary>
		public Article FirstArticle{
			get {return myArticles[0];}
		}

		/// <summary>
		/// ���̃X���b�h�����̋L���̔z����擾���܂��B
		/// </summary>
		public Article[] Articles{
			get {return myArticles;}
		}

		/// <summary>
		/// ���̃X���b�h�ɑ����邷�ׂĂ̋L���̔z����擾���܂��B
		/// </summary>
		public Article[] AllArticles{
			get {return myAllArticles;}
		}

		/// <summary>
		/// �R�����g��� Path ��ݒ�E�擾���܂��B
		/// </summary>
		public AbsPath CommentTo{
			get {return myCommentTo;}
		}

		/// <summary>
		/// ���̃X���b�h�� ID ���擾���܂��B
		/// </summary>
		public int Id{
			get {return myId;}
		}

		/// <summary>
		/// ���̃X���b�h�̓��t���擾���܂��B
		/// </summary>
		public DateTime Date{
			get {return myDate;}
		}

		/// <summary>
		/// ���̃X���b�h�̋L�������擾���܂��B
		/// </summary>
		public int Count{
			get {return myAllArticles.Length;}
		}

// ���\�b�h

		/// <summary>
		/// �X���b�h�Ƀ��[�g�L����ǉ����܂��B
		/// </summary>
		public void AddArticle(Article a){
			if(myTempArticles == null) throw new Exception("�t�@�C�i���C�Y���ꂽ�X���b�h�Ƀf�[�^��ǉ����悤�Ƃ��܂����B");
			myTempArticles.Add(a);
		}


		/// <summary>
		/// �X���b�h�ւ̃f�[�^�ǉ�����ߐ؂�A�f�[�^�̃Z�b�g�Ȃǂ��s���܂��B�B
		/// </summary>
		public void Set(){
			myArticles = new Article[myTempArticles.Count];
			myId = int.MaxValue;
			myDate = DateTime.MinValue;
			myTempArticles.Sort();

			List<Article> childList = new List<Article>();
			for(int i=0; i < myTempArticles.Count; i++){
				Article a = myTempArticles[i];
				myArticles[i] = a;
				if(a.Id < myId) myId = a.Id;
				CheckChildren(a, childList);
			}
			childList.Sort();
			myAllArticles = childList.ToArray();
		}

		// ���g�Ǝq�ǂ�������Aricle���`�F�b�N���ăJ�E���g�Ɠ��t�̃`�F�b�N���s���܂��B
		private void CheckChildren(Article a, List<Article> childList){
			childList.Add(a);
			if(!a.IsSpam && a.Date > myDate) myDate = a.Date;
			foreach(Article childA in a.Children){
				CheckChildren(childA, childList);
			}
		}


// IComparable �C���^�[�t�F�C�X�̎���

		/// <summary>
		/// BbsThread ����t�Ŕ�r���܂��B
		/// </summary>
		public int CompareTo(BbsThread other){
			return myDate.CompareTo(other.Date);
		}


	}// End Class Article
	
}// End Namespace hatomruBBS





