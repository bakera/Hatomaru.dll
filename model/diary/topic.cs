using System;
using System.Xml;

namespace Bakera.Hatomaru{
	
	/// <summary>
	/// ��̃g�s�b�N��\���N���X�ł��B
	/// </summary>
	public class Topic{
		private const string GenreSeparator = ",";

		public const string DiaryElementName = "diary";
		public const string TopicElementName = "topic";
		public const string IdAttr = "num";
		public const string NameAttr = "name";
		public const string DateAttr = "date";
		public const string UpdatedAttr = "updated";
		public const string CreatedAttr = "created";
		public const string GenreAttr = "genre";


		protected int myId;
		protected DateTime myDate;
		protected DateTime myCreated;
		protected DateTime myUpdated;
		protected string myTitle;
		protected XmlElement myMessage;
		protected string[] myGenre;



/* ======== �v���p�e�B ======== */

		/// <summary>
		/// �L���� ID �ԍ����擾���܂��B
		/// </summary>
		public int Id{
			get {return myId;}
		}
		/// <summary>
		/// �g�s�b�N�̓��t���擾���܂��B
		/// </summary>
		public DateTime Date{
			get{return myDate;}
		}
		/// <summary>
		/// �g�s�b�N���쐬���ꂽ���t���擾���܂��B
		/// </summary>
		public DateTime Created{
			get{return myCreated;}
		}
		/// <summary>
		/// �g�s�b�N���X�V���ꂽ���t���擾���܂��B
		/// </summary>
		public DateTime Updated{
			get{return myUpdated;}
		}
		/// <summary>
		/// �g�s�b�N�̌��o�����擾���܂��B
		/// </summary>
		public string Title{
			get{return myTitle;}
		}
		/// <summary>
		/// �g�s�b�N�̃��b�Z�[�W���擾���܂��B
		/// </summary>
		public XmlElement Message{
			get{return myMessage;}
		}
		/// <summary>
		/// �g�s�b�N�̃W���������擾���܂��B
		/// </summary>
		public string[] Genre{
			get{return myGenre;}
		}


/* ======== �R���X�g���N�^ ======== */

		public Topic(){}
		/// <summary>
		/// XmlNode ���w�肵�āATopic �N���X�̃C���X�^���X���J�n���܂��B
		/// </summary>
		public Topic(XmlNode topicElement){
			myMessage = (XmlElement)topicElement;
			Load(myMessage);
		}


/* ======== ���\�b�h ======== */


		/// <summary>
		/// XML Element ����f�[�^�����[�h���܂��B
		/// </summary>
		public void Load(XmlElement message){
			myDate = message.ParentNode.GetAttributeDateTime(DateAttr);
			myId = message.GetAttributeInt(IdAttr);
			myTitle = message.GetAttributeValue(NameAttr);
			myGenre = message.GetAttributeValues(GenreAttr);
			if(myGenre.Length == 0) myGenre = new string[]{"�m���W������"};
			myUpdated = message.GetAttributeDateTime(UpdatedAttr, myDate);
			myCreated = message.GetAttributeDateTime(CreatedAttr, myDate);
		}

		/// <summary>
		/// �I�[�o�[���C�h�BTopic �𕶎���ɕϊ����܂��B
		/// </summary>
		public override string ToString(){
			return "No." + this.Id.ToString() + ": " + this.Title.ToString() + this.Date.ToString(" (yyyy-MM-dd)");
		}


		public static int CompareByUpdated(Topic x, Topic y){
			return y.Updated.CompareTo(x.Updated);
		}


	} // public class Topic

} // namespace Bakera




