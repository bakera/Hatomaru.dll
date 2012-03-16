using System;
using System.Collections.Generic;
using System.Data;
using System.Xml;

namespace Bakera.Hatomaru{

	/// <summary>
	/// 鳩丸 BBS のデータを格納する DataTable の派生クラスです。
	/// </summary>
	public class BbsTable : HatomaruTable{

		private const string MessageElementName = "message";

		// 列名称の定数
		public const string NumColName = "num";
		public const string DateColName = "date";
		public const string CommenttoColName = "commentto";
		public const string ArticleColName = "article";

		// 列
		private DataColumn myNumCol;
		private DataColumn myDateCol;
		private DataColumn myCommenttoCol;
		private DataColumn myArticleCol;

		private BbsThread[] myThreadList;



// コンストラクタ

		public BbsTable(){
			InitColumns();
		}
		public BbsTable(XmlDocument x) : this(){
			Load(x);
		}


// プロパティ

		public DataColumn ArticleCol{
			get{return myArticleCol;}
		}

		public BbsThread[] ThreadList{
			get{return myThreadList;}
		}


// パブリックメソッド


// データのロード

		/// <summary>
		/// XmlDocument からデータをロードします。
		/// </summary>
		public void Load(XmlDocument x){

			XmlNodeList xnl = x.DocumentElement.GetElementsByTagName(MessageElementName);
			Dictionary<int, Article> articleDict = new Dictionary<int, Article>();
			Dictionary<string, BbsThread> threadDict = new Dictionary<string, BbsThread>();

			// 古い順に読む
			for(int i=xnl.Count-1; i >=0 ; i--){
				Article a = new Article(xnl[i] as XmlElement);
				int parent = a.Parent;
				if(parent > 0 && articleDict.ContainsKey(parent)){
					// 親がいる
					Article parentArticle = articleDict[parent];
					a.Thread = parentArticle.Thread;
					parentArticle.AddChild(a);
				} else {
					// 親がいない
					if(a.CommentTo != null){
						// 同じコメントのスレッドあるか?
						string key = a.CommentTo.OriginalString;
						if(threadDict.ContainsKey(key)){
							BbsThread bt = threadDict[key];
							bt.AddArticle(a);
							a.Thread = bt;
						} else {
							BbsThread bt = new BbsThread(a);
							a.Thread = bt;
							threadDict.Add(key, bt);
						}
					} else {
						BbsThread bt = new BbsThread(a);
						a.Thread = bt;
						threadDict.Add(a.Id.ToString(), bt);
					}
				}
				articleDict.Add(a.Id, a);
			}

			foreach(Article a in articleDict.Values){
				Rows.Add(new Object[]{
					a.Id,
					a.Date,
					a.CommentTo,
					a,
				});
			}

			myThreadList = new BbsThread[threadDict.Count];
			threadDict.Values.CopyTo(myThreadList, 0);
			Array.ForEach(myThreadList, item=>{item.Set();});
			Array.Sort(myThreadList);
		}

// 初期化

		private void InitColumns(){
			// Column を設定します。
			// Num
			myNumCol = new DataColumn(NumColName, typeof(int));
			myNumCol.Unique = true;
			myNumCol.AutoIncrement = true;
			myNumCol.AllowDBNull = false;
			this.Columns.Add(myNumCol);
			this.PrimaryKey = new DataColumn[]{myNumCol};

			// Date
			myDateCol = new DataColumn(DateColName, typeof(DateTime));
			myDateCol.Unique = false;
			myDateCol.AutoIncrement = false;
			myDateCol.AllowDBNull = false;
			this.Columns.Add(myDateCol);

			// CommentTo
			myCommenttoCol = new DataColumn(CommenttoColName, typeof(string));
			myCommenttoCol.Unique = false;
			myCommenttoCol.AutoIncrement = false;
			myCommenttoCol.AllowDBNull = true;
			this.Columns.Add(myCommenttoCol);

			// Article
			myArticleCol = new DataColumn(ArticleColName, typeof(Article));
			myArticleCol.Unique = false;
			myArticleCol.AutoIncrement = false;
			myArticleCol.AllowDBNull = true;
			this.Columns.Add(myArticleCol);
		}




	} // class BbsTable
} // namespace


