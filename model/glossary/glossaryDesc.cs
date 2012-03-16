using System;
using System.Xml;
using System.Collections.Generic;

namespace Bakera.Hatomaru{
	
	/// <summary>
	/// 一つの用語説明を表すクラスです。
	/// </summary>
	public class GlossaryDesc{

		private XmlElement myDescription;
		private string[] myGenre;

/* ======== コンストラクタ ======== */

		/// <summary>
		/// XmlElement を指定して、GlossaryDesc クラスのインスタンスを開始します。
		/// </summary>
		public GlossaryDesc(XmlElement e){
			myDescription = e;
			string genres = e.GetAttributeValue(HatomaruGlossary.GenreAttributeName);
			if(string.IsNullOrEmpty(genres)) return;
			myGenre = genres.Split(new[]{','}, StringSplitOptions.RemoveEmptyEntries);
		}


/* ======== プロパティ ======== */

		/// <summary>
		/// ジャンルの配列を取得します。
		/// </summary>
		public string[] Genre{
			get{return myGenre;}
		}
		/// <summary>
		/// desc要素を取得します。
		/// </summary>
		public XmlElement Description{
			get{return myDescription;}
		}


	} // public class Topic

} // namespace Bakera




