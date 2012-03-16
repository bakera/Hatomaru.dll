using System;
using System.Collections.Generic;
using System.Xml;

namespace Bakera.Hatomaru{
	
	/// <summary>
	/// ぐろっさりのジャンルを表すクラスです。
	/// </summary>
	public class GlossaryGenre : IComparable<GlossaryGenre>{
		private readonly string myName;
		private readonly List<GlossaryWord> myGlossarys = new List<GlossaryWord>();


// コンストラクタ
		public GlossaryGenre(string name){
			myName = name;
		}

// プロパティ
		public string Name{
			get{return myName;}
		}

		public GlossaryWord[] Glossarys{
			get{return myGlossarys.ToArray();}
		}

		public int Count{
			get{return myGlossarys.Count;}
		}


// メソッド
		public void Add(GlossaryWord gw){
			myGlossarys.Add(gw);
		}

// IComparable
		/// <summary>
		/// 件数で比較します。
		/// </summary>
		public int CompareTo(GlossaryGenre gg){
			return -this.Count.CompareTo(gg.Count);
		}

	} // public class TopicGenre

} // namespace Bakera




