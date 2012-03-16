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
/// ���یf������������N���X�ł��B
/// </summary>
	public partial class HatomaruBbs : HatomaruXml{

		new public const string Name = "hatomarubbs";

		private BbsTable myTable;
		private static SHA512 mySha = new SHA512Managed();


// �R���X�g���N�^

		/// <summary>
		/// ���ۃf�[�^�̃f�[�^�\�[�X�� FileInfo �� XmlDocument ���w�肵�āAHatomaruBbs �̃C���X�^���X���J�n���܂��B
		/// </summary>
		public HatomaruBbs(HatomaruManager manager, FileInfo f, XmlDocument x) : base(manager, f, x){
			myTable = new BbsTable(x);
		}

// �v���p�e�B


// �f�[�^�擾���\�b�h

		/// <summary>
		/// �ŐV�� Article ���擾���܂��B
		/// </summary>
		public Article GetLatestArticle(){
			DataRow[] rows = myTable.Select();
			if(rows.Length == 0) return null;
			return GetArticle(rows[rows.Length-1]);
		}
		/// <summary>
		/// table ���炷�ׂĂ� Article ���擾���܂��B
		/// </summary>
		public Article[] GetAllArticle(){
			DataRow[] resultRows = myTable.Select();
			return GetArticle(resultRows);
		}
		/// <summary>
		/// table ������� ID �� Article ���擾���܂��B
		/// </summary>
		public Article GetArticle(int id){
			DataRow resultRow = myTable.Rows.Find(id);
			if(resultRow == null) return null;
			return resultRow[myTable.ArticleCol] as Article;
		}
		/// <summary>
		/// table ����S�ẴX���b�h���擾���܂��B
		/// </summary>
		public BbsThread[] GetAllThread(){
			return myTable.ThreadList;
		}
		/// <summary>
		/// table ������� ID �̋L�����܂ރX���b�h���擾���܂��B
		/// </summary>
		public BbsThread GetThread(int id){
			DataRow resultRow = myTable.Rows.Find(id);
			if(resultRow == null) return null;
			return GetArticle(resultRow).Thread;
		}
		/// <summary>
		/// �n���ꂽ Path �ɑ΂���R�����g���܂ރX���b�h���擾���܂��B
		/// </summary>
		public BbsThread GetCommentToThread(AbsPath path){
			string selectStr = string.Format("[{0}]='{1}'", BbsTable.CommenttoColName, myTable.EscapeSingleQuote(path));
			DataRow[] resultRows = myTable.Select(selectStr);
			if(resultRows.Length == 0) return null;
			return GetArticle(resultRows[0]).Thread;
		}


		/// <summary>
		/// DataRow ���� Article ���擾���܂��B
		/// </summary>
		private Article[] GetArticle(DataRow[] rows){
			Article[] result = new Article[rows.Length];
			for(int i=0; i < result.Length; i++){
				result[i] = rows[i][myTable.ArticleCol] as Article;
			}
			return result;
		}
		/// <summary>
		/// DataRow ���� Article ���擾���܂��B
		/// </summary>
		private Article GetArticle(DataRow row){
			return row[myTable.ArticleCol] as Article;
		}



// ���̑����\�b�h

		/// <summary>
		/// POST �ɕK�v�ȃL�[���擾���܂��B
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
		/// �V���� Article ��ǉ����ĕۑ����܂��B
		/// </summary>
		public void SaveArticle(Article a){

			// �ۑ��p Document �����
			XmlDocument newDoc = Document.Clone() as XmlDocument;
			XmlNode newNode = a.ToXmlElement(newDoc);

			XmlElement metaData = newDoc.DocumentElement[MetaName];
			if(metaData == null) throw new Exception("XML�Ƀ��^�f�[�^���܂܂�Ă��܂���B");
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
					if(i == SaveRetryTime) throw new IOException("�������݃A�N�Z�X�����ۂ���܂����B" + e.ToString());
					System.Threading.Thread.Sleep(SaveRetryInterval);
				}
			}
		}

// �I�[�o�[���C�h���\�b�h

		/// <summary>
		/// �f�[�^�� GET ���AHatomaruResponse ���擾���܂��B
		/// </summary>
		public override HatomaruResponse Get(AbsPath path){
			BbsAction ba = GetAction(path);
			HatomaruResponse result = ba.Get();
			result.SetLastModified();
			return result;
		}


		/// <summary>
		/// path �����ɁA�K�؂ȃR���g���[�����쐬���܂��B
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
			// �ǂ�ł��Ȃ��Ƃ��� BBS �̃g�b�v
			return new BbsViewThread(this, path, 1);
		}

		/// <summary>
		/// Post ���������邽�߂� HatomaruPostAction ���擾���܂��B
		/// </summary>
		protected override HatomaruPostAction GetPostAction(AbsPath path, HttpRequest req){
			return new BbsPostAction(this, path, req);
		}

		// Bbs �ɑ����� Action ���ׂĂ� SetReplaceUrl ���\�b�h�����s���܂��B
		public static void SetAllReplaceUrl(Xhtml html){
			html.SetReplaceUrl("/" + BbsViewThread.Id + "/1", "");
			html.SetReplaceUrl("/" + BbsViewOrder.Id + "/1", "/" + BbsViewOrder.Id);
			html.SetReplaceUrl("/" + BbsViewRootList.Id + "/1", "/" + BbsViewRootList.Id);
		}


	} // End class
} // End Namespace Bakera



