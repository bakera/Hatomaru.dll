using System;
using System.Collections.Generic;

namespace Bakera.Hatomaru{

	/// <summary>
	/// 鳩丸掲示板の一つのスレッドを扱うクラスです。
	/// </summary>
	public class BbsThread : IComparable<BbsThread>{

		private int myId; // スレッドの ID。直下の記事の中でもっとも番号が若いものの ID
		private DateTime myDate;
		private List<Article> myTempArticles = new List<Article>();// スレッド直下の記事を一時的に追加
		private Article[] myArticles;// スレッド直下の記事
		private Article[] myAllArticles;// スレッドに属する全ての記事
		private AbsPath myCommentTo;// コメント先、Bbs 直下記事は null

// コンストラクタ

		/// <summary>
		/// Articleを指定して、BbsThread の新しいインスタンスを開始します。
		/// </summary>
		public BbsThread(Article a){
			myTempArticles.Add(a);
			myCommentTo = a.CommentTo;
		}


// プロパティ

		/// <summary>
		/// このスレッド直下の記事の配列を取得します。
		/// </summary>
		public Article FirstArticle{
			get {return myArticles[0];}
		}

		/// <summary>
		/// このスレッド直下の記事の配列を取得します。
		/// </summary>
		public Article[] Articles{
			get {return myArticles;}
		}

		/// <summary>
		/// このスレッドに属するすべての記事の配列を取得します。
		/// </summary>
		public Article[] AllArticles{
			get {return myAllArticles;}
		}

		/// <summary>
		/// コメント先の Path を設定・取得します。
		/// </summary>
		public AbsPath CommentTo{
			get {return myCommentTo;}
		}

		/// <summary>
		/// このスレッドの ID を取得します。
		/// </summary>
		public int Id{
			get {return myId;}
		}

		/// <summary>
		/// このスレッドの日付を取得します。
		/// </summary>
		public DateTime Date{
			get {return myDate;}
		}

		/// <summary>
		/// このスレッドの記事数を取得します。
		/// </summary>
		public int Count{
			get {return myAllArticles.Length;}
		}

// メソッド

		/// <summary>
		/// スレッドにルート記事を追加します。
		/// </summary>
		public void AddArticle(Article a){
			if(myTempArticles == null) throw new Exception("ファイナライズされたスレッドにデータを追加しようとしました。");
			myTempArticles.Add(a);
		}


		/// <summary>
		/// スレッドへのデータ追加を締め切り、データのセットなどを行います。。
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

		// 自身と子どもたちのAricleをチェックしてカウントと日付のチェックを行います。
		private void CheckChildren(Article a, List<Article> childList){
			childList.Add(a);
			if(!a.IsSpam && a.Date > myDate) myDate = a.Date;
			foreach(Article childA in a.Children){
				CheckChildren(childA, childList);
			}
		}


// IComparable インターフェイスの実装

		/// <summary>
		/// BbsThread を日付で比較します。
		/// </summary>
		public int CompareTo(BbsThread other){
			return myDate.CompareTo(other.Date);
		}


	}// End Class Article
	
}// End Namespace hatomruBBS





