using System;
using System.Xml;

namespace Bakera.Hatomaru{
	
	/// <summary>
	/// HTML �̗v�f��\���N���X�ł��B
	/// </summary>
	public class HtmlElement : HtmlVersionItem{

		protected string myNote;
		protected string myNoteJa;
		protected bool myOmitStartTag;
		protected bool myOmitEndTag;


// �R���X�g���N�^
		/// <summary>
		/// XmlNode ���w�肵�āAHtmlElement �N���X�̃C���X�^���X���J�n���܂��B
		/// </summary>
		public HtmlElement(XmlElement e) : base(e){
			myNote = e.GetInnerText(HatomaruHtmlRef.NoteElementName);
			myNoteJa = e.GetInnerText(HatomaruHtmlRef.NoteJaElementName);
			string omitStr = e.GetInnerText(HatomaruHtmlRef.OmitElementName);
			if(omitStr != null && omitStr.Length == 2){
				myOmitStartTag = omitStr[0] == 'o';
				myOmitEndTag = omitStr[1] == 'o';
			}
		}


// �v���p�e�B

		public HtmlAttribute[] Attributes{get; set;}
		public HtmlAttributeGroup[] AttributeGroups{get; set;}

		/// <summary>
		/// �J�n�^�O���ȗ��ł���Ȃ�true��Ԃ��܂��B
		/// </summary>
		public bool OmitStartTag{
			get {return myOmitStartTag;}
		}

		/// <summary>
		/// �I���^�O���ȗ��ł���Ȃ�true��Ԃ��܂��B
		/// </summary>
		public bool OmitEndTag{
			get {return myOmitEndTag;}
		}

		/// <summary>
		/// ��v�f�Ȃ�true��Ԃ��܂��B
		/// </summary>
		public bool IsEmptyElement{get; set;}

		/// <summary>
		/// �q�v�f��ݒ�E�擾���܂��B
		/// </summary>
		public HtmlItem[] Content{get; set;}

		/// <summary>
		/// ���S�Ȗ��O���擾���܂��B
		/// </summary>
		public override string FullName{
			get {return myName + "�v�f";}
		}

		public override string LinkId{
			get{return HtmlRefViewElementList.Id;}
		}


// ���\�b�h
		public string GetOmitStartTag(){
			return myOmitStartTag ? "�ȗ���" : "�K�{";
		}
		public string GetOmitEndTag(){
			if(IsEmptyElement) return "�֎~";
			return myOmitEndTag ? "�ȗ���" : "�K�{";
		}


	}

} // namespace Bakera




