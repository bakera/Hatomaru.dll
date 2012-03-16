using System;
using System.IO;
using System.Xml;

namespace Bakera.Hatomaru{

/// <summary>
/// メニューの XML を処理するクラスです。
/// </summary>
	public class CacheViewer : HatomaruXml{

		new public const string Name = "cacheviewer";

// コンストラクタ

		/// <summary>
		/// 鳩丸データのデータソースの FileInfo と XmlDocument を指定して、インスタンスを開始します。
		/// </summary>
		public CacheViewer(HatomaruManager manager, FileInfo f, XmlDocument x) : base(manager, f, x){}


// オーバーライドメソッド

		/// <summary>
		/// データを GET し、HatomaruResponse を取得します。
		/// </summary>
		public override HatomaruResponse Get(AbsPath path){
			CacheViewerAction da = new CacheViewerAction(this, path);
			HatomaruResponse result = da.Get();
			result.SetLastModified();
			return result;
		}


	} // End class
} // End Namespace Bakera







