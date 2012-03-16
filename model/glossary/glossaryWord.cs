using System;
using System.Xml;
using System.Collections.Generic;

namespace Bakera.Hatomaru{
	
	/// <summary>
	/// 一つの用語を表すクラスです。
	/// </summary>
	public class GlossaryWord : IComparable<GlossaryWord>{
		private XmlElement myElement;
		private string myName;
		private string myAltRead;
		private string myRead;
		private string myPronounce;
		private GlossaryDesc[] myDescs;
		private string[] myGenres;

/* ======== コンストラクタ ======== */

		/// <summary>
		/// XmlElement を指定して、Word クラスのインスタンスを開始します。
		/// </summary>
		public GlossaryWord(XmlElement x){
			myElement = x;
			Load(x);
		}

		/// <summary>
		/// 用語を読み順にソートするためのメソッドです。
		/// </summary>
		public int CompareTo(GlossaryWord gw){
			return this.Read.CompareTo(gw.Read);
		}


/* ======== プロパティ ======== */

		/// <summary>
		/// 用語の名称を取得します。
		/// </summary>
		public string Name{
			get{return myName;}
		}
		/// <summary>
		/// 用語の読みを取得します。
		/// </summary>
		public string Read{
			get{return myRead;}
		}
		/// <summary>
		/// 用語の発音を取得します。
		/// </summary>
		public string Pronounce{
			get{return myPronounce;}
		}
		/// <summary>
		/// 用語の別名を取得します。
		/// </summary>
		public string AltRead{
			get{return myAltRead;}
		}
		/// <summary>
		/// ジャンルの配列を取得します。
		/// </summary>
		public string[] Genre{
			get{return myGenres;}
		}
		/// <summary>
		/// 用語解説を取得します。
		/// </summary>
		public GlossaryDesc[] Descs{
			get{return myDescs;}
		}
		/// <summary>
		/// XmlElementを取得します。
		/// </summary>
		public XmlElement Element{
			get{return myElement;}
		}

/* ======== メソッド ======== */

		public void Load(XmlElement e){
			myName = e.GetAttributeValue(HatomaruGlossary.NameAttributeName);
			myRead = e.GetAttributeValue(HatomaruGlossary.ReadAttributeName);
			myAltRead = e.GetAttributeValue(HatomaruGlossary.AltreadAttributeName);

			List<GlossaryDesc> descs = new List<GlossaryDesc>();
			List<string> genres = new List<string>();
			foreach(XmlElement desc in e.GetElementsByTagName(HatomaruGlossary.DescElementName)){
				GlossaryDesc gd = new GlossaryDesc(desc);
				descs.Add(gd);
				if(gd.Genre != null && gd.Genre.Length > 0) genres.AddRange(gd.Genre);
			}
			myDescs = descs.ToArray();
			myGenres = genres.ToArray();

			myPronounce = e.GetAttributeValue(HatomaruGlossary.PronounceAttributeName);
		}



	} // public class Topic

} // namespace Bakera




