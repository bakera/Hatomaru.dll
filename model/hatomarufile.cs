using System;
using System.IO;
using System.Drawing;

namespace Bakera.Hatomaru{


/// <summary>
/// 鳩丸データ
/// データソースのファイルを示すクラスです。
/// </summary>
	public class HatomaruFile : HatomaruData, ICacheData{
		
		private static string[] imageExtension = new string[]{".png", ".jpg", ".gif"};
		private Size mySize;

// コンストラクタ

		/// <summary>
		/// HatomaruFile のインスタンスを開始します。
		/// </summary>
		public HatomaruFile(HatomaruManager manager, FileInfo f) : base(manager, f){
		}


// プロパティ

		/// <summary>
		/// 画像サイズを取得します。
		/// </summary>
		public Size Size{
			get{
				if(!mySize.IsEmpty) return mySize;
				mySize = CalcImageSize();
				return mySize;
			}
		}


// パブリックメソッド

		/// <summary>
		/// データを GET し、HatomaruResponse を取得します。
		/// ファイルのパスが正しくなければリダイレクトします (拡張子の有無のみ許容します)
		/// </summary>
		public override HatomaruResponse Get(AbsPath path){
			FileResponse result = new FileResponse(this, Ext);

			if(path != BasePath && path != BasePath.CombineExtension(File.Extension)){
				return new RedirectResponse(BasePath, Manager.IniData.Domain);
			}

			result.Path = BasePath;
			result.SetLastModified();
			return result;
		}

		/// <summary>
		/// データを Post し、HatomaruResponse を取得します。
		/// </summary>
		public override HatomaruResponse Post(AbsPath path, System.Web.HttpRequest req){
			HatomaruResponse result = new NotAllowedResponse("GET");
			return result;
		}




// ICacheData インターフェイスの実装

		/// <summary>
		/// このデータをキャッシュしても良いかどうかを返します。
		/// hatomaruFile は常にキャッシュ可能です。
		/// </summary>
		public bool IsCacheable{
			get{return true;}
		}

		/// <summary>
		/// このインスタンスの Last-Modified とファイルの更新日を比較します。
		/// インスタンスが最新ならば true を、古ければ false を返します。
		/// </summary>
		public override bool IsNewest{
			get{
				File.Refresh();
				if(File.LastWriteTime > LastModified) return false;
				return true;
			}
		}

		/// <summary>
		/// このインスタンスが破棄されていれば true, 使用できるなら false を返します。
		/// ファィルが削除されている場合には true が返ります。
		/// </summary>
		public virtual bool IsExpired{
			get{
				return !File.Exists;
			}
		}




// プライベートメソッド

		// 画像のサイズを取得します。
		private Size CalcImageSize(){
			Size result = default(Size);
			using(FileStream imgStream = File.Open(FileMode.Open, FileAccess.Read, FileShare.Read)){
				using(System.Drawing.Bitmap bmp = new System.Drawing.Bitmap(imgStream)){
					result = bmp.Size;
				}
			}
			return result;
		}



	} // End class HatomaruFile
} // End Namespace Bakera

