using System;
using System.IO;
using System.Xml;

namespace Bakera.Hatomaru{

/// <summary>
/// ドキュメントのtopicを処理するクラスです。
/// </summary>
	public class DocTopic : DocSection{


// コンストラクタ

		public DocTopic(XmlElement e, int index) : base(e, null, index){}

		public string Id{
			get{return SectionElement.GetAttributeValue(HatomaruDoc.PageIdAttribute);}
		}

	} // End class
} // End Namespace







