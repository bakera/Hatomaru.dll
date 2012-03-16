using System;
using System.Xml;
using System.Collections.Generic;

namespace Bakera.Hatomaru{
	
	/// <summary>
	/// ��̗p���\���N���X�ł��B
	/// </summary>
	public class GlossaryWord : IComparable<GlossaryWord>{
		private XmlElement myElement;
		private string myName;
		private string myAltRead;
		private string myRead;
		private string myPronounce;
		private GlossaryDesc[] myDescs;
		private string[] myGenres;

/* ======== �R���X�g���N�^ ======== */

		/// <summary>
		/// XmlElement ���w�肵�āAWord �N���X�̃C���X�^���X���J�n���܂��B
		/// </summary>
		public GlossaryWord(XmlElement x){
			myElement = x;
			Load(x);
		}

		/// <summary>
		/// �p���ǂݏ��Ƀ\�[�g���邽�߂̃��\�b�h�ł��B
		/// </summary>
		public int CompareTo(GlossaryWord gw){
			return this.Read.CompareTo(gw.Read);
		}


/* ======== �v���p�e�B ======== */

		/// <summary>
		/// �p��̖��̂��擾���܂��B
		/// </summary>
		public string Name{
			get{return myName;}
		}
		/// <summary>
		/// �p��̓ǂ݂��擾���܂��B
		/// </summary>
		public string Read{
			get{return myRead;}
		}
		/// <summary>
		/// �p��̔������擾���܂��B
		/// </summary>
		public string Pronounce{
			get{return myPronounce;}
		}
		/// <summary>
		/// �p��̕ʖ����擾���܂��B
		/// </summary>
		public string AltRead{
			get{return myAltRead;}
		}
		/// <summary>
		/// �W�������̔z����擾���܂��B
		/// </summary>
		public string[] Genre{
			get{return myGenres;}
		}
		/// <summary>
		/// �p�������擾���܂��B
		/// </summary>
		public GlossaryDesc[] Descs{
			get{return myDescs;}
		}
		/// <summary>
		/// XmlElement���擾���܂��B
		/// </summary>
		public XmlElement Element{
			get{return myElement;}
		}

/* ======== ���\�b�h ======== */

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




