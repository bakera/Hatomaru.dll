using System;
using System.IO;
using System.Xml;

namespace Bakera.Hatomaru{

/// <summary>
/// �h�L�������g��topic����������N���X�ł��B
/// </summary>
	public class DocTopic : DocSection{


// �R���X�g���N�^

		public DocTopic(XmlElement e, int index) : base(e, null, index){}

		public string Id{
			get{return SectionElement.GetAttributeValue(HatomaruDoc.PageIdAttribute);}
		}

	} // End class
} // End Namespace







