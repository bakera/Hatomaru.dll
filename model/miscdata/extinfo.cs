using System;
using System.Xml;

namespace Bakera.Hatomaru{
	
	
/// <summary>
/// �g���q�������̃N���X
/// </summary>
	public class ExtInfo{
		private string myDescription;

		public const string ExtInfoName = "name";
		public const string ExtInfoType = "type";
		public const string ExtInfoCharset = "charset";
		public const string ExtInfoDisposition = "disposition";
		public const string ExtInfoMaxAge = "maxage";

// �R���X�g���N�^

		public ExtInfo(){}
		public ExtInfo(XmlElement e){
			LoadXml(e);
		}


// �v���p�e�B
		public string Name{
			get; private set;
		}

		public string ContentType{
			get; private set;
		}

		public string Charset{
			get; private set;
		}

		public bool Disposition{
			get; private set;
		}

		// Expires �̊��� maxage�����Ƃ��ē��P�ʂŎw��
		public TimeSpan MaxAge{
			get;
			private set;
		}

		public string Description{
			get{
				if(!string.IsNullOrEmpty(myDescription)) return myDescription;
				return Name.TrimStart('.').ToUpper() + "�t�@�C��";
			}
			private set{
				myDescription = value;
			}
		}

// �p�u���b�N���\�b�h

		public void LoadXml(XmlElement e){
			Name = e.GetAttributeValue(ExtInfoName);
			ContentType = e.GetAttributeValue(ExtInfoType);
			Charset = e.GetAttributeValue(ExtInfoCharset);
			Description = e.InnerText;

			string tempDisposition = e.GetAttributeValue(ExtInfoDisposition);
			if(!string.IsNullOrEmpty(tempDisposition)) Disposition = true;

			int maxAgeDays = e.GetAttributeInt(ExtInfoMaxAge);
			MaxAge = new TimeSpan(maxAgeDays, 0, 0, 0);
		}




	} // End class ExtInfo
} // End Namespace Bakera







