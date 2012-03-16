using System;
using System.Xml;

namespace Bakera.Hatomaru{
	
	/// <summary>
	/// HTML �̑�����\���N���X�ł��B
	/// </summary>
	public class HtmlAttribute : HtmlVersionItem{

		protected string myNote;
		protected string myDefault;
		protected string myFor;

		public const string IdSeparator = "@";

// �R���X�g���N�^
		/// <summary>
		/// XmlNode ���w�肵�āAHtmlElement �N���X�̃C���X�^���X���J�n���܂��B
		/// </summary>
		public HtmlAttribute(XmlElement e) : base(e){
			myNote = e.GetInnerText(HatomaruHtmlRef.NoteElementName);
			myDefault = e.GetInnerText(HatomaruHtmlRef.DefaultElementName);
			myFor = e.GetAttributeValue(HatomaruHtmlRef.ForAttributeName);
			if(string.IsNullOrEmpty(myId)){
				myId = myName;
				if(!string.IsNullOrEmpty(myFor)) myId += IdSeparator + myFor;
			}
		}


// �v���p�e�B

		/// <summary>
		/// �����l���擾���܂��B
		/// </summary>
		public HtmlItem Value{get; set;}

		/// <summary>
		/// ����l���擾���܂��B
		/// </summary>
		public string Default{
			get {return myDefault;}
		}

		/// <summary>
		/// ���l���擾���܂��B
		/// </summary>
		public string Note{
			get {return myNote;}
		}

		/// <summary>
		/// �ڂ������O���擾���܂��B
		/// ����v�f�Ɍ��т������̏ꍇ name (elem) ���Ԃ�悤�ɃI�[�o�[���C�h����܂��B
		/// </summary>
		public override string SpecName{
			get {
				string result = myName;
				if(!string.IsNullOrEmpty(myFor)){
					result += string.Format("({0})", myFor);
				}
				return result;
			}
		}

		/// <summary>
		/// ���S�Ȗ��O���擾���܂��B
		/// </summary>
		public override string FullName{
			get {
				string result = myName + "����";
				if(!string.IsNullOrEmpty(myFor)){
					result += string.Format("({0})", myFor);
				}
				return result;
			}
		}

		public override string LinkId{
			get{return HtmlRefViewAttributeList.Id;}
		}

// ���\�b�h


	}

} // namespace Bakera




