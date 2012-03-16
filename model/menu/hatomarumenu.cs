using System;
using System.IO;
using System.Xml;

namespace Bakera.Hatomaru{

/// <summary>
/// メニューの XML を処理するクラスです。
/// </summary>
	public class HatomaruMenu : HatomaruXml{

		new public const string Name = "menu";
		public const string MenuItem = "menuitem";

// コンストラクタ

		/// <summary>
		/// 鳩丸データのデータソースの FileInfo と XmlDocument を指定して、Menu のインスタンスを開始します。
		/// </summary>
		public HatomaruMenu(HatomaruManager manager, FileInfo f, XmlDocument x) : base(manager, f, x){}


// オーバーライドメソッド

		/// <summary>
		/// データを GET し、HatomaruResponse を取得します。
		/// </summary>
		public override HatomaruResponse Get(AbsPath path){
			HatomaruGetAction ga = new MenuAction(this, path);
			HatomaruResponse result = ga.Get();
			result.SetLastModified();
			return result;
		}



	} // End class Doc
} // End Namespace Bakera







