using System;
using System.Collections.Generic;
using System.Xml;

namespace Bakera.Hatomaru{

	// HTML �̗v�f�E�����E�v�f�O���[�v�E�����O���[�v�E�f�[�^�`���̃x�[�X�ƂȂ钊�ۃN���X
	public abstract class HtmlItem{

		protected string myId;
		protected string myName;
		protected List<HtmlItem> myParents = new List<HtmlItem>();
		protected XmlElement myDescription;
		protected XmlElement myXmlElement;

		public HtmlItem(){}
		public HtmlItem(XmlElement e){
			myXmlElement = e;
			myId = e.GetAttributeValue(HatomaruHtmlRef.IdAttributeName);
			myName = e.GetAttributeValue(HatomaruHtmlRef.NameAttributeName);
			myDescription = e[HatomaruHtmlRef.DescElementName];
		}


// �v���p�e�B
		/// <summary>
		/// �x�[�X�� XmlElement���擾���܂��B
		/// </summary>
		public XmlElement XmlElement{
			get {return myXmlElement;}
		}

		/// <summary>
		/// ID��ݒ�E�擾���܂��B
		/// </summary>
		public string Id{
			get {return myId;}
		}

		/// <summary>
		/// ���O��ݒ�E�擾���܂��B
		/// </summary>
		public string Name{
			get {return myName;}
		}

		/// <summary>
		/// �������i�[���� desc �v�f���擾���܂��B
		/// </summary>
		public XmlElement Description{
			get {return myDescription;}
		}

		/// <summary>
		/// �e�ƂȂ� HtmlItem ���擾���܂��B
		/// </summary>
		public HtmlItem[] Parents{
			get {return myParents.ToArray();}
		}

		/// <summary>
		/// �ڂ������O���擾���܂��B
		/// ����v�f�Ɍ��т������̏ꍇ name (elem) ���Ԃ�悤�ɃI�[�o�[���C�h����܂��B
		/// </summary>
		public virtual string SpecName{
			get {return myName;}
		}
		/// <summary>
		/// ���S�Ȗ��O���擾���܂��B
		/// </summary>
		public virtual string FullName{
			get {return myName;}
		}

		public abstract string LinkId{get;}


// ���\�b�h

		/// <summary>
		/// �e�v�f��ǉ����܂��B
		/// </summary>
		public virtual void AddParent(HtmlItem item){
			myParents.Add(item);
		}



	}
}