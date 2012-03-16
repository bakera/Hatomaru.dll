using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Xml;


namespace Bakera.Hatomaru{

/// <summary>
/// 鳩丸掲示板を処理するクラスです。
/// </summary>
	public partial class HatomaruBbs : HatomaruXml{

		new public const string Name = "hatomarubbs";

		private BbsTable myTable;
		private static SHA512 mySha = new SHA512Managed();


// コンストラクタ

		/// <summary>
		/// 鳩丸データのデータソースの FileInfo と XmlDocument を指定して、HatomaruBbs のインスタンスを開始します。
		/// </summary>
		public HatomaruBbs(HatomaruManager manager, FileInfo f, XmlDocument x) : base(manager, f, x){
			myTable = new BbsTable(x);
		}

// プロパティ


// データ取得メソッド

		/// <summary>
		/// 最新の Article を取得します。
		/// </summary>
		public Article GetLatestArticle(){
			DataRow[] rows = myTable.Select();
			if(rows.Length == 0) return null;
			return GetArticle(rows[rows.Length-1]);
		}
		/// <summary>
		/// table からすべての Article を取得します。
		/// </summary>
		public Article[] GetAllArticle(){
			DataRow[] resultRows = myTable.Select();
			return GetArticle(resultRows);
		}
		/// <summary>
		/// table から特定 ID の Article を取得します。
		/// </summary>
		public Article GetArticle(int id){
			DataRow resultRow = myTable.Rows.Find(id);
			if(resultRow == null) return null;
			return resultRow[myTable.ArticleCol] as Article;
		}
		/// <summary>
		/// table から全てのスレッドを取得します。
		/// </summary>
		public BbsThread[] GetAllThread(){
			return myTable.ThreadList;
		}
		/// <summary>
		/// table から特定 ID の記事を含むスレッドを取得します。
		/// </summary>
		public BbsThread GetThread(int id){
			DataRow resultRow = myTable.Rows.Find(id);
			if(resultRow == null) return null;
			return GetArticle(resultRow).Thread;
		}
		/// <summary>
		/// 渡された Path に対するコメントを含むスレッドを取得します。
		/// </summary>
		public BbsThread GetCommentToThread(AbsPath path){
			string selectStr = string.Format("[{0}]='{1}'", BbsTable.CommenttoColName, myTable.EscapeSingleQuote(path));
			DataRow[] resultRows = myTable.Select(selectStr);
			if(resultRows.Length == 0) return null;
			return GetArticle(resultRows[0]).Thread;
		}


		/// <summary>
		/// DataRow から Article を取得します。
		/// </summary>
		private Article[] GetArticle(DataRow[] rows){
			Article[] result = new Article[rows.Length];
			for(int i=0; i < result.Length; i++){
				result[i] = rows[i][myTable.ArticleCol] as Article;
			}
			return result;
		}
		/// <summary>
		/// DataRow から Article を取得します。
		/// </summary>
		private Article GetArticle(DataRow row){
			return row[myTable.ArticleCol] as Article;
		}



// その他メソッド

		/// <summary>
		/// POST に必要なキーを取得します。
		/// </summary>
		public string GetPostKey(){
			byte[] shaByte = null;
			using(FileStream fs = File.Open(FileMode.Open, FileAccess.Read, FileShare.Read)){
				shaByte = mySha.ComputeHash(fs);
				fs.Close();
			}
			StringBuilder result = new StringBuilder(shaByte.Length);
			for(int i=0; i < shaByte.Length; i++){
				result.Append(shaByte[i].ToString("X2"));
			}
			return result.ToString();
		}


		/// <summary>
		/// 新しい Article を追加して保存します。
		/// </summary>
		public void SaveArticle(Article a){

			// 保存用 Document を作る
			XmlDocument newDoc = Document.Clone() as XmlDocument;
			XmlNode newNode = a.ToXmlElement(newDoc);

			XmlElement metaData = newDoc.DocumentElement[MetaName];
			if(metaData == null) throw new Exception("XMLにメタデータが含まれていません。");
			newDoc.DocumentElement.InsertAfter(newNode, metaData);

			for(int i = 1; i <= SaveRetryTime; i++){
				try{
					using(FileStream saveFile = File.Open(FileMode.Create, FileAccess.Write, FileShare.None)){
						newDoc.Save(saveFile);
					}
					return;
				} catch(UnauthorizedAccessException){
					throw;
				} catch(IOException e) {
					if(i == SaveRetryTime) throw new IOException("書き込みアクセスが拒否されました。" + e.ToString());
					System.Threading.Thread.Sleep(SaveRetryInterval);
				}
			}
		}

// オーバーライドメソッド

		/// <summary>
		/// データを GET し、HatomaruResponse を取得します。
		/// </summary>
		public override HatomaruResponse Get(AbsPath path){
			BbsAction ba = GetAction(path);
			HatomaruResponse result = ba.Get();
			result.SetLastModified();
			return result;
		}


		/// <summary>
		/// path を元に、適切なコントローラを作成します。
		/// </summary>
		private BbsAction GetAction(AbsPath path){
			string[] fragments = path.GetFragments(BasePath);
			if(fragments.Length > 0){
				string firstStr = fragments[0];
				int num = 0;
				if(fragments.Length > 1) num = fragments[1].ToInt32();
				switch(firstStr){
				case BbsViewThread.Id:
					return new BbsViewThread(this, path, num);
				case BbsViewOrder.Id:
					return new BbsViewOrder(this, path, num);
				case BbsViewRootList.Id:
					return new BbsViewRootList(this, path, num);
				case BbsViewNewPost.Id:
					return new BbsViewNewPost(this, path);
				case BbsViewArticle.Id:
					return new BbsViewArticle(this, path, num);
				case BbsViewInThread.Id:
					return new BbsViewInThread(this, path, num);
				}
			}
			// どれでもないときは BBS のトップ
			return new BbsViewThread(this, path, 1);
		}

		/// <summary>
		/// Post を処理するための HatomaruPostAction を取得します。
		/// </summary>
		protected override HatomaruPostAction GetPostAction(AbsPath path, HttpRequest req){
			return new BbsPostAction(this, path, req);
		}

		// Bbs に属する Action すべての SetReplaceUrl メソッドを実行します。
		public static void SetAllReplaceUrl(Xhtml html){
			html.SetReplaceUrl("/" + BbsViewThread.Id + "/1", "");
			html.SetReplaceUrl("/" + BbsViewOrder.Id + "/1", "/" + BbsViewOrder.Id);
			html.SetReplaceUrl("/" + BbsViewRootList.Id + "/1", "/" + BbsViewRootList.Id);
		}


	} // End class
} // End Namespace Bakera



