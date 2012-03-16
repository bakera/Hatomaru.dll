using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Reflection;
using System.Xml;

namespace Bakera.Hatomaru{

/// <summary>
/// �X�̐l�C�R���e���c��\���N���X�ł��B
/// </summary>
	public partial class ReputationContent {

		public ReputationContent(HatomaruManager manager, XmlElement contentElement){
			Load(manager, contentElement);
		}

// �v���p�e�B

		public AbsPath Path{
			get; private set;
		}

		public int Count{
			get; private set;
		}

		public XmlElement Description{
			get; private set;
		}

		public string Title{
			get; private set;
		}

		public string[] Keywords{
			get; private set;
		}

// ���\�b�h

		public void Load(HatomaruManager manager, XmlElement contentElement){
			string path = contentElement.GetAttribute(HatomaruReputation.UriAttributeName);
			Path = new AbsPath(path);
			Count = contentElement.GetAttributeInt(HatomaruReputation.CountAttributeName);
			Description = contentElement[HatomaruReputation.DescriptionElementName];

			Title = manager.GetResponseTitle(Path);

		}


	} // End class
} // End Namespace Bakera



