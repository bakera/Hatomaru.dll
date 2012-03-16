using System;
using System.Xml;
using System.Collections.Generic;

namespace Bakera.Hatomaru{
	
	/// <summary>
	/// ��̗p�������\���N���X�ł��B
	/// </summary>
	public class GlossaryDesc{

		private XmlElement myDescription;
		private string[] myGenre;

/* ======== �R���X�g���N�^ ======== */

		/// <summary>
		/// XmlElement ���w�肵�āAGlossaryDesc �N���X�̃C���X�^���X���J�n���܂��B
		/// </summary>
		public GlossaryDesc(XmlElement e){
			myDescription = e;
			string genres = e.GetAttributeValue(HatomaruGlossary.GenreAttributeName);
			if(string.IsNullOrEmpty(genres)) return;
			myGenre = genres.Split(new[]{','}, StringSplitOptions.RemoveEmptyEntries);
		}


/* ======== �v���p�e�B ======== */

		/// <summary>
		/// �W�������̔z����擾���܂��B
		/// </summary>
		public string[] Genre{
			get{return myGenre;}
		}
		/// <summary>
		/// desc�v�f���擾���܂��B
		/// </summary>
		public XmlElement Description{
			get{return myDescription;}
		}


	} // public class Topic

} // namespace Bakera




