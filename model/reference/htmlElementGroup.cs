using System;
using System.Xml;

namespace Bakera.Hatomaru{
	
	/// <summary>
	/// HTML�̗v�f�Q��\���N���X�ł��B
	/// </summary>
	public class HtmlElementGroup : HtmlVersionItem{


// �R���X�g���N�^
		/// <summary>
		/// XmlNode ���w�肵�āAHtmlElementGroup �N���X�̃C���X�^���X���J�n���܂��B
		/// </summary>
		public HtmlElementGroup(XmlElement e) : base(e){
		}


// �v���p�e�B

		public HtmlItem[] Content{get; set;}

		public override string LinkId{
			get{return HtmlRefViewElementGroupList.Id;}
		}

// ���\�b�h


	}

} // namespace Bakera




