using System;
using System.IO;
using System.Xml;


namespace Bakera.Hatomaru{

/// <summary>
/// 鳩丸掲示板を処理するクラスです。
/// </summary>
	public partial class HatomaruAccessCounter : HatomaruXml{

		new public const string Name = "accesscounter";


		/// <summary>
		/// 鳩丸データのデータソースの FileInfo と XmlDocument を指定して、HatomaruAccessCounter のインスタンスを開始します。
		/// </summary>
		public HatomaruAccessCounter(HatomaruManager manager, FileInfo f, XmlDocument x) : base(manager, f, x){}

		/// <summary>
		/// データを GET し、HatomaruResponse を取得します。
		/// </summary>
		public override HatomaruResponse Get(AbsPath path){
/*
			HatomaruGetAction act = GetAction(path);
			HatomaruResponse result = act.Get();
			result.SetLastModified();
			return result;
*/
			return null;
		}

	}

}

