using System;
using System.Collections.Generic;
using System.Xml;

namespace Bakera.Hatomaru{
	
	/// <summary>
	/// ���������̃W��������\���N���X�ł��B
	/// </summary>
	public class GlossaryGenre : IComparable<GlossaryGenre>{
		private readonly string myName;
		private readonly List<GlossaryWord> myGlossarys = new List<GlossaryWord>();


// �R���X�g���N�^
		public GlossaryGenre(string name){
			myName = name;
		}

// �v���p�e�B
		public string Name{
			get{return myName;}
		}

		public GlossaryWord[] Glossarys{
			get{return myGlossarys.ToArray();}
		}

		public int Count{
			get{return myGlossarys.Count;}
		}


// ���\�b�h
		public void Add(GlossaryWord gw){
			myGlossarys.Add(gw);
		}

// IComparable
		/// <summary>
		/// �����Ŕ�r���܂��B
		/// </summary>
		public int CompareTo(GlossaryGenre gg){
			return -this.Count.CompareTo(gg.Count);
		}

	} // public class TopicGenre

} // namespace Bakera




