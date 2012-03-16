using System;
using System.Xml;



namespace Bakera.Hatomaru{

	/// <summary>
	/// URI �Ɩ��O�����A���J�[�̗v�f�������N���X�ł��B
	/// </summary>
	public class LinkItem{

		private Uri myPath;
		private string myInnerText;
		private string myPrefix;
		private string mySuffix;


// �R���X�g���N�^
		/// <summary>
		/// LinkItem �̃C���X�^���X���J�n���܂��B
		/// </summary>
		public LinkItem(){}

		/// <summary>
		/// �p�X������ƃe�L�X�g���w�肵�āALinkItem �̃C���X�^���X���J�n���܂��B
		/// </summary>
		public LinkItem(string path, string innerText){
			if(path == null) path = "";
			myPath = path.StartsWith("/") ? new Uri(path, UriKind.Relative) : new Uri(path);
			myInnerText = innerText;
		}
		/// <summary>
		/// �p�X��Uri�ƃe�L�X�g���w�肵�āALinkItem �̃C���X�^���X���J�n���܂��B
		/// </summary>
		public LinkItem(Uri path, string innerText){
			myPath = path;
			myInnerText = innerText;
		}


// �v���p�e�B
		/// <summary>
		/// �p�X��ݒ�E�擾���܂��B
		/// </summary>
		public Uri Path{
			get{return myPath;}
			set{myPath = value;}
		}

		/// <summary>
		/// InnerText ��ݒ�E�擾���܂��B
		/// </summary>
		public string InnerText{
			get{return myInnerText;}
			set{myInnerText = value;}
		}

		/// <summary>
		/// Prefix ��ݒ�E�擾���܂��B
		/// </summary>
		public string Prefix{
			get{return myPrefix;}
			set{myPrefix = value;}
		}

		/// <summary>
		/// Suffix ��ݒ�E�擾���܂��B
		/// </summary>
		public string Suffix{
			get{return mySuffix;}
			set{mySuffix = value;}
		}



// �p�u���b�N���\�b�h

		/// <summary>
		/// ������ɕϊ����܂��B
		/// </summary>
		public override string ToString(){
			return string.Format("{0}<{1}>", InnerText, Path);
		}

// �ÓI���\�b�h

		/// <summary>
		/// XmlNode ���� LinkItem �𐶐����܂��B
		/// </summary>
		public static LinkItem GetItem(XmlNode node){
			XmlElement elem = node as XmlElement;
			if(elem == null) return null;

			string path = elem.GetAttribute("href");
			string text = elem.InnerText;
			return new LinkItem(path, text);
		}




	} // End class LinkItem

} // End namespace Bakera






