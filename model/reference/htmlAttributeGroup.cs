using System;
using System.Xml;

namespace Bakera.Hatomaru{
	
	/// <summary>
	/// HTML�̗v�f�Q��\���N���X�ł��B
	/// </summary>
	public class HtmlAttributeGroup : HtmlVersionItem{


// �R���X�g���N�^
		/// <summary>
		/// XmlNode ���w�肵�āAHtmlElementGroup �N���X�̃C���X�^���X���J�n���܂��B
		/// </summary>
		public HtmlAttributeGroup(XmlElement e) : base(e){
		}


// �v���p�e�B

		public HtmlAttribute[] Attributes{get; set;}
		public HtmlAttributeGroup[] AttributeGroups{get; set;}

		public override string LinkId{
			get{return HtmlRefViewAttributeGroupList.Id;}
		}

// ���\�b�h


	}

} // namespace Bakera




