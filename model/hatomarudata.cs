using System;
using System.IO;
using System.Web;

namespace Bakera.Hatomaru{

/* 動作メモ
ICacheData …… キャッシュできるデータが持つインターフェイス
HatomaruData …… データソースを示す抽象クラス
HatomaruFile …… 単にファイルの場所と拡張子を示す。キャッシュ不要
HatomaruXml : HatomaruData …… HatomaruData に加えて XmlDocument を保持
abstract HatomaruTable : HatomaruXml …… HatomaruXml に加えて、DataTable を保持
*/




/// <summary>
/// 鳩丸データ
/// データソースのファイルを示すクラスです。
/// </summary>
	public abstract class HatomaruData{
		private FileInfo myFile;
		private readonly HatomaruManager myManager;
		private readonly ExtInfo myExt;
		private readonly AbsPath myBasePath;
		private DateTime myLastModified;



// コンストラクタ

		/// <summary>
		/// 鳩丸データのデータソースの FileInfo を指定して、HatomaruData のインスタンスを開始します。
		/// </summary>
		protected HatomaruData(HatomaruManager manager, FileInfo f){
			myManager = manager;
			myFile = f;
			myLastModified = myFile.LastWriteTime;
			if(Manager.IniData.ExtInfo.ContainsKey(f.Extension.ToLower())){
				myExt = Manager.IniData.ExtInfo[f.Extension.ToLower()];
			}
			myBasePath = GetTrueFilePath();
		}

// プロパティ

		/// <summary>
		/// ファイルを読み込んだ時刻を取得します。
		/// </summary>
		public virtual DateTime LastModified{
			get{return myLastModified;}
		}


		/// <summary>
		/// データソースが最新かどうかをチェックし、最新なら true を、古くなっていれば false を返します。
		/// </summary>
		public abstract bool IsNewest{ get; }


		/// <summary>
		/// 鳩丸データのデータソースファイルを示す FileInfo を取得します。
		/// </summary>
		public FileInfo File{
			get{return myFile;}
		}

		/// <summary>
		/// データソースファイルの置かれているディレクトリを調べ、対応する画像ファイルディレクトリを取得します。
		/// </summary>
		public DirectoryInfo ImageDirectoryPath{
			get{
				string fileDir = myFile.Directory.FullName;
				string imgPath = fileDir + '\\' + Manager.IniData.ImageDir;
				DirectoryInfo result = new DirectoryInfo(imgPath);
				return result;
			}
		}

		/// <summary>
		/// 生成元の HatomaruManager を取得します。
		/// </summary>
		public HatomaruManager Manager{
			get{return myManager;}
		}

		/// <summary>
		/// 鳩丸データの ExtInfo を取得します。
		/// </summary>
		public ExtInfo Ext{
			get{return myExt;}
		}

		/// <summary>
		/// データの基準パスを取得します。
		/// これはパラメータが何もない場合の絶対パスです。
		/// </summary>
		public AbsPath BasePath{
			get{return myBasePath;}
		}





// 抽象パブリックメソッド

		/// <summary>
		/// データを GET し、HatomaruResponse を取得します。
		/// </summary>
		public abstract HatomaruResponse Get(AbsPath path);

		/// <summary>
		/// データを Post し、HatomaruResponse を取得します。
		/// </summary>
		public abstract HatomaruResponse Post(AbsPath path, HttpRequest req);



// オーバーライドメソッド

		/// <summary>
		/// 鳩丸データのデータソースファイルのフルパス名を取得します。
		/// </summary>
		public override string ToString(){
			return File.FullName;
		}


		/// <summary>
		/// ファイルにリンクするための Uri を返します。
		/// </summary>
		public virtual Uri GetLinkUri(){
			return BasePath.GetAbsUri(Manager.IniData.Domain);
		}



// プロテクトメソッド

// プライベートメソッド

		// ファイルパスから URL のパスを取得します。
		private AbsPath GetTrueFilePath(){
			string ext = File.Extension;
			string lastName = "\\" + HatomaruManager.DirectoryIndexName;
			string basePath = Manager.IniData.DataPath.FullName.TrimEnd('\\');

			string result = File.FullName;
			if(result.IndexOf(basePath) < 0) return null;
			result = result.CutRight(ext);
			result = result.CutRight(lastName);
			result = result.CutLeft(basePath);
			result = result.Replace('\\', '/');

			if(result == "") result = "/";

			return new AbsPath(result.ToLower());
		}




	} // End class HatomaruData
} // End Namespace Bakera







