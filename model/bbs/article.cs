using System;
using System.Xml;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Data;

namespace Bakera.Hatomaru{

	/// <summary>
	/// ���یf���̈�̋L���������N���X�ł��B
	/// </summary>
	public class Article : IComparable<Article>{

		public const string SpamTitle = "�����F���b�Z�[�W (���e��:{0})";

		private const string MessageElement = "message";
		private const string IdAttr = "num";
		private const string ParentAttr = "parent";
		private const string SubjectAttr = "subject";
		private const string NameAttr = "name";
		private const string DateAttr = "date";
		private const string IpAddressAttr = "ip";
		private const string UrlRefAttr = "urlref";
		private const string SpamAttr = "spam";
		private const string CommentToAttr = "commentto";
		private const string TargetTitleAttr = "target";
		private const string SrcCountryAttr = "cctld";

		public const string CommentPrefix = "Re:";

		private int myId;
		private int myParent;           // �e�L���ԍ�
		private string mySubject;      // Subject
		private string myName;         // myName ���e�Җ�
		private string myMessage;  // myMessage ���e�B�s���Ƃ� LF ��؂�Ƃ���
		private DateTime myDate;

		private string myIpAddress;    // myIpAddress �����[�g�A�h���X
		private bool myIsSpam;     // URI ���폜����Ȃ� true
		private AbsPath myCommentTo;     // �R�����g��p�X
		private BbsThread myThread;         // ���̋L����������X���b�h

		private List<Article> myChildList = new List<Article>();         // ���̋L���̎q����
		private Article[] myChildren;         // ���̋L���̎q����


// �R���X�g���N�^

		/// <summary>
		/// Article �̐V�����C���X�^���X���J�n���܂��B
		/// </summary>
		public Article(){}

		/// <summary>
		/// message �v�f���w�肵�āAArticle �̐V�����C���X�^���X���J�n���܂��B
		/// </summary>
		public Article(XmlElement messageElement){
			this.Load(messageElement);
		}



/* ======== �v���p�e�B ======== */

		/// <summary>
		/// ���̋L���� ID ��ݒ�E�擾���܂��B
		/// </summary>
		public int Id{
			get {return myId;}
			set {myId = value;}
		}

		/// <summary>
		/// ���̋L���̐e�L���� ID �ԍ���ݒ�E�擾���܂��B
		/// </summary>
		public int Parent{
			get {return myParent;}
			set {myParent = value;}
		}

		/// <summary>
		/// �L���� Subject ��ݒ�E�擾���܂��B
		/// </summary>
		public string Subject{
			get {return mySubject;}
			set {mySubject = value;}
		}

		/// <summary>
		/// �L���� Title ���擾���܂��BID �� Subject ����Ȃ�܂��B
		/// Spam �̏ꍇ�� Spam �p�̃^�C�g����Ԃ��܂��B
		/// </summary>
		public string Title{
			get {
				if(Id == 0) return Subject;
				string subjectStr = Subject;
				if(this.IsSpam) subjectStr = string.Format(SpamTitle, this.IpAddress);
				return string.Format("[{0}] {1}", Id, subjectStr);
			}
		}

		/// <summary>
		/// �L���̓��e������ݒ�E�擾���܂��B
		/// </summary>
		public DateTime Date{
			get {return myDate;}
			set {myDate = value;}
		}

		/// <summary>
		/// �L���̓��e�Җ���ݒ�E�擾���܂��B
		/// </summary>
		public string Name{
			get {return myName;}
			set {myName = value;}
		}

		/// <summary>
		/// �L���̓��e�Ɏg�p���ꂽ�����[�g�z�X�g�� IP �A�h���X��ݒ�E�擾���܂��B
		/// </summary>
		public string IpAddress{
			get {return myIpAddress;}
			set {myIpAddress = value;}
		}

		/// <summary>
		/// �L���̖{����ݒ�E�擾���܂��B
		/// </summary>
		public string Message{
			get {return myMessage;}
			set {myMessage = value;}
		}


		/// <summary>
		/// spam���肳��Ă���ꍇ true ��Ԃ��܂��B
		/// </summary>
		public bool IsSpam{
			get {return myIsSpam;}
			set {myIsSpam = value;}
		}

		/// <summary>
		/// ���e���̍���Ԃ��܂��B
		/// </summary>
		public string SrcCountry{
			get; set;
		}

		/// <summary>
		/// �R�����g��L���̃p�X��ݒ�E�擾���܂��B
		/// </summary>
		public AbsPath CommentTo{
			get {return myCommentTo;}
			set {myCommentTo = value;}
		}

		/// <summary>
		/// ���̋L���̑�����X���b�h�ԍ���ݒ�E�擾���܂��B
		/// </summary>
		public BbsThread Thread{
			get {return myThread;}
			set {myThread = value;}
		}

		/// <summary>
		/// ���̋L���̎q������ݒ�E�擾���܂��B
		/// </summary>
		public Article[] Children{
			get {
				if(myChildren != null) return myChildren;
				myChildren = new Article[myChildList.Count];
				myChildList.CopyTo(myChildren);
				return myChildren;
			}
		}


		/// <summary>
		/// �L���� ID �� Subject ����Ȃ�薼���擾���܂��B
		/// </summary>
		public override string ToString(){
			return string.Format("[{0}] {1}", Id, Subject);
		}




// �p�u���b�N���\�b�h

		public string DateToString(){
			return Date.ToString("yyyy�NM��d�� H��m��");
		}

		/// <summary>
		/// XML message �v�f����L���̃f�[�^�����[�h���܂��B
		/// </summary>
		public void Load(XmlElement messageElement){
			// �e�����l���v���p�e�B�ɓ����
			Id = messageElement.GetAttributeInt(IdAttr);
			Parent = messageElement.GetAttributeInt(ParentAttr);
			Subject = messageElement.GetAttributeValue(SubjectAttr);
			Name = messageElement.GetAttributeValue(NameAttr);
			IpAddress = messageElement.GetAttributeValue(IpAddressAttr);
			IsSpam = messageElement.GetAttributeBool(SpamAttr);
			Date = messageElement.GetAttributeDateTime(DateAttr);
			CommentTo = messageElement.GetAttributePath(CommentToAttr);
			SrcCountry = messageElement.GetAttributeValue(SrcCountryAttr);

			// �v�f�̓��e�� Message �v���p�e�B�̒l
			XmlNodeList pList = messageElement.GetElementsByTagName("p");
			if(pList.Count > 0){
				StringBuilder mesString = new StringBuilder();
				foreach(XmlNode n in messageElement.ChildNodes){
					XmlElement e = n as XmlElement;
					if (e == null) continue;
					if(e.Name == "p") mesString.Append(n.InnerText);
					mesString.Append("\n");
				}
				myMessage = mesString.ToString().TrimEnd();
			} else {
				myMessage = messageElement.InnerText;
			}
		}

		/// <summary>
		/// �L���̃f�[�^�� XML message �v�f�Ƃ��ďo�͂��܂��B
		/// </summary>
		public XmlElement ToXmlElement(XmlDocument owner){
			XmlElement result = owner.CreateElement(MessageElement);
			if(Id <= 0) throw new Exception("ID������܂���B");
			result.SetAttribute(IdAttr, Id.ToString());
			if(Parent > 0) result.SetAttribute(ParentAttr, Parent.ToString());
			result.SetAttribute(SubjectAttr, Subject.ToString());
			result.SetAttribute(NameAttr, Name.ToString());
			if(IpAddress == null) throw new Exception("IP�A�h���X������܂���B");
			result.SetAttribute(IpAddressAttr, IpAddress.ToString());
			if(IsSpam) result.SetAttribute(SpamAttr, SpamAttr);
			result.SetAttribute(DateAttr, Date.ToString());
			if(SrcCountry != null) result.SetAttribute(SrcCountryAttr, SrcCountry);
			if(CommentTo != null) result.SetAttribute(CommentToAttr, CommentTo.ToString());
			result.InnerText = Message;
			return result;
		}

		/// <summary>
		/// ���� Article �ɃR�����g����ۂ̃t�H�[���̗v�f�ƂȂ� Article ���擾���܂��B
		/// </summary>
		public Article GetCommentArticle(){
			Article result = new Article();
			result.Parent = this.Id;
			if(this.Subject.StartsWith(CommentPrefix)){
				result.Subject = this.Subject;
			} else {
				result.Subject = CommentPrefix + this.Subject;
			}
			if(!String.IsNullOrEmpty(this.Message)){
				result.Message = ">" + String.Join("\n>", this.Message.Split('\n')) + '\n';
 			}
			return result;
		}


		/// <summary>
		/// �q�� Article ��ǉ����܂��B
		/// </summary>
		public void AddChild(Article child){
			myChildList.Add(child);
		}

// IComparable �C���^�[�t�F�C�X�̎���

		/// <summary>
		/// ArticleBase ����t�Ŕ�r���܂��B
		/// </summary>
		public int CompareTo(Article a){
			return myDate.CompareTo(a.Date);
		}


	}// End Class Article
	
}// End Namespace hatomruBBS





