using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace Bakera.Hatomaru{

/// <summary>
/// リダイレクトの宣言のみのXMLを処理するためのクラスです。
/// </summary>
	public class HatomaruRedirect : HatomaruXml{

		new public const string Name = "redirect";
		public const string PathAttrName = "path";

// コンストラクタ

		/// <summary>
		/// 鳩丸データのデータソースの FileInfo と XmlDocument を指定して、Redirect のインスタンスを開始します。
		/// </summary>
		public HatomaruRedirect(HatomaruManager manager, FileInfo f, XmlDocument x) : base(manager, f, x){
		}

// オーバーライドメソッド

		/// <summary>
		/// データを GET し、HatomaruResponse を取得します。
		/// </summary>
		public override HatomaruResponse Get(AbsPath path){
			XmlElement e = this.Document.DocumentElement;
			string targetPath = e.GetAttributeValue(PathAttrName);
			if(string.IsNullOrEmpty(targetPath)){
				throw new Exception(string.Format("{0}要素に{1}属性がないか、値が空です。", Name, PathAttrName));
			}
			AbsPath redPath = new AbsPath(targetPath);
			
			return new RedirectResponse(redPath, Manager.IniData.Domain);

		}



	} // End class Doc
} // End Namespace Bakera







