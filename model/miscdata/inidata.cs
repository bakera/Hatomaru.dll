using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace Bakera.Hatomaru{
	
	
/// <summary>
/// ���ۃV�X�e�����
/// </summary>
	public class IniData{
		// ini.xml �̃f�B���N�g��
		private readonly FileInfo myFile;

		private DirectoryInfo myDataPath;
		private string myImageDir;
		private DirectoryInfo myLogPath;
		private DirectoryInfo myAccessLogPath;
		private DirectoryInfo myAmazonCachePath;
		private DirectoryInfo myResponseCachePath;
		private FileInfo myTitleCacheFile;
		private FileInfo myKeywordCacheFile;
		private FileInfo myTemplate;
		private FileInfo myCommentTo;
		private FileInfo myGlossary;
		private FileInfo myHtmlRef;
		private FileInfo mySpamRule;
		private FileInfo myDiary;
		private string myDomain;

		public const string DataPathName = "docs";
		public const string ImageDirName = "imgdir";
		public const string LogPathName = "log";
		public const string AmazonCachePathName = "amazonCache";
		public const string ResponseCachePathName = "responseCache";
		public const string AccessLogPathName = "accessLog";
		public const string TitleCacheFileName = "titleCache";
		public const string KeywordCacheFileName = "keywordCache";
		public const string IPAddrToCCTLDFileName = "IPAddrToCCTLD";
		public const string TemplateFileName = "template";
		public const string DomainName = "domain";
		public const string CommentToName = "commentto";
		public const string GlossaryName = "glossary";
		public const string SpamRuleName = "spamrule";

		public const string AmazonWsAccessKeyIdName = "amazonWsAccessKeyId";
		public const string AmazonWsSecretKeyName = "amazonWsSecretKey";
		public const string AmazonServiceHostNameName = "amazonServiceHostName";
		public const string AmazonAssociateTagName = "amazonAssociateTag";

		public const string DiaryName = "diary";
		public const string HtmlRefName = "htmlref";
		public const string ExtensionInfo = "ext";
		public const string ExtensionInfoBase = "extinfo";

		private Dictionary<string, ExtInfo> myExtInfo = new Dictionary<string, ExtInfo>();
		private Dictionary<string, string> myData = new Dictionary<string, string>();


// �R���X�g���N�^
		public IniData(string filename){
			myFile = new FileInfo(filename);
			Load(filename);
		}


// �v���p�e�B
		/// <summary>
		/// ���� IniData �̃t�@�B�����擾���܂��B
		/// </summary>
		public FileInfo File{
			get{return myFile;}
		}


		/// <summary>
		/// �����ݒ�t�@�C���Őݒ肳�ꂽ�A�g���q���̃��X�g���擾���܂��B
		/// </summary>
		public Dictionary<string, ExtInfo> ExtInfo{
			get{return myExtInfo;}
		}

		/// <summary>
		/// �����ݒ�t�@�C���Őݒ肳�ꂽ�A�h�L�������g�z�u�f�B���N�g���� DirectoryInfo ���擾���܂��B
		/// </summary>
		public DirectoryInfo DataPath{
			get{return myDataPath;}
		}

		/// <summary>
		/// �����ݒ�t�@�C���Őݒ肳�ꂽ�A�摜�z�u�f�B���N�g���̃p�X��������擾���܂��B
		/// </summary>
		public string ImageDir{
			get{return myImageDir;}
		}

		/// <summary>
		/// �����ݒ�t�@�C���Őݒ肳�ꂽ�A���O�ۑ��f�B���N�g���� DirectoryInfo ���擾���܂��B
		/// </summary>
		public DirectoryInfo LogPath{
			get{return myLogPath;}
		}

		/// <summary>
		/// �����ݒ�t�@�C���Őݒ肳�ꂽ�A�A�N�Z�X���O�ۑ��f�B���N�g���� DirectoryInfo ���擾���܂��B
		/// </summary>
		public DirectoryInfo AccessLogPath{
			get{return myAccessLogPath;}
		}

		/// <summary>
		/// �����ݒ�t�@�C���Őݒ肳�ꂽ�AAmazon�L���b�V���ۑ��f�B���N�g���� DirectoryInfo ���擾���܂��B
		/// </summary>
		public DirectoryInfo AmazonCachePath{
			get{return myAmazonCachePath;}
		}

		/// <summary>
		/// �����ݒ�t�@�C���Őݒ肳�ꂽ�A���X�|���X�L���b�V���ۑ��f�B���N�g���� DirectoryInfo ���擾���܂��B
		/// </summary>
		public DirectoryInfo ResponseCachePath{
			get{return myResponseCachePath;}
		}

		/// <summary>
		/// �����ݒ�t�@�C���Őݒ肳�ꂽ�A�y�[�W�^�C�g���L���b�V���ۑ��t�@�C���� FileInfo ���擾���܂��B
		/// </summary>
		public FileInfo TitleCacheFile{
			get{return myTitleCacheFile;}
		}

		/// <summary>
		/// �����ݒ�t�@�C���Őݒ肳�ꂽ�A�L�[���[�h�L���b�V���ۑ��t�@�C���� FileInfo ���擾���܂��B
		/// </summary>
		public FileInfo KeywordCacheFile{
			get{return myKeywordCacheFile;}
		}

		/// <summary>
		/// �����ݒ�t�@�C���Őݒ肳�ꂽ�AIPAddrToCCTLD�t�@�C���� FileInfo ���擾���܂��B
		/// </summary>
		public FileInfo IPAddrToCCTLDFile{
			private set; get;
		}

		/// <summary>
		/// �����ݒ�t�@�C���Őݒ肳�ꂽ�A�e���v���[�g�� FileInfo ���擾���܂��B
		/// </summary>
		public FileInfo Template{
			get{return myTemplate;}
		}

		/// <summary>
		/// �����ݒ�t�@�C���Őݒ肳�ꂽ�A�R�����g���e�Ώۂ� HatomaruBbs ������ FileInfo ���擾���܂��B
		/// </summary>
		public FileInfo CommentTo{
			get{return myCommentTo;}
		}

		/// <summary>
		/// �����ݒ�t�@�C���Őݒ肳�ꂽ�AHatomaruGlossary ������ FileInfo ���擾���܂��B
		/// </summary>
		public FileInfo Glossary{
			get{return myGlossary;}
		}

		/// <summary>
		/// �����ݒ�t�@�C���Őݒ肳�ꂽ�AHatomaruHtmlRef ������ FileInfo ���擾���܂��B
		/// </summary>
		public FileInfo HtmlRef{
			get{return myHtmlRef;}
		}

		/// <summary>
		/// �����ݒ�t�@�C���Őݒ肳�ꂽ�A�X�p�����e�������[�������� FileInfo ���擾���܂��B
		/// </summary>
		public FileInfo SpamRule{
			get{return mySpamRule;}
		}

		/// <summary>
		/// �����ݒ�t�@�C���Őݒ肳�ꂽ�A���L������ FileInfo ���擾���܂��B
		/// </summary>
		public FileInfo Diary{
			get{return myDiary;}
		}

		/// <summary>
		/// �����ݒ�t�@�C���Őݒ肳�ꂽ�h���C���̏����擾���܂��B
		/// </summary>
		public string Domain{
			get{return myDomain;}
		}

		/// <summary>
		/// �����ݒ�t�@�C���Őݒ肳�ꂽamazonWsAccessKeyId�̏����擾���܂��B
		/// </summary>
		public string AmazonWsAccessKeyId{
			get;
			private set;
		}

		/// <summary>
		/// �����ݒ�t�@�C���Őݒ肳�ꂽamazonWsSecretKey�̏����擾���܂��B
		/// </summary>
		public string AmazonWsSecretKey{
			get;
			private set;
		}

		/// <summary>
		/// �����ݒ�t�@�C���Őݒ肳�ꂽamazonServiceHostName�̏����擾���܂��B
		/// </summary>
		public string AmazonServiceHostName{
			get;
			private set;
		}

		/// <summary>
		/// �����ݒ�t�@�C���Őݒ肳�ꂽamazonAssociateTag�̏����擾���܂��B
		/// </summary>
		public string AmazonAssociateTag{
			get;
			private set;
		}



// �p�u���b�N���\�b�h


		/// <summary>
		/// �w�肳�ꂽ XML �t�@�C������f�[�^�����[�h���܂��B
		/// ���̏����̓X���b�h�Z�[�t�ł͂���܂���B
		/// </summary>
		public void Load(string filename){
			XmlDocument doc = new XmlDocument();
			doc.Load(filename);
			LoadProperties(doc);
			LoadExtInfo(doc);
		}


		// XML �f�[�^����v���p�e�B���擾���܂��B
		private void LoadProperties(XmlDocument doc){
			myDataPath = LoadDirectory(doc, DataPathName);
			myLogPath = LoadDirectory(doc, LogPathName);
			myImageDir = LoadString(doc, ImageDirName);
			myResponseCachePath = LoadDirectory(doc, ResponseCachePathName);
			myAccessLogPath = LoadDirectory(doc, AccessLogPathName);
			myTitleCacheFile = LoadFile(doc, TitleCacheFileName, false);
			myKeywordCacheFile = LoadFile(doc, KeywordCacheFileName, false);
			IPAddrToCCTLDFile = LoadFile(doc, IPAddrToCCTLDFileName, false);
			myTemplate = LoadFile(doc, TemplateFileName);
			myDomain = LoadString(doc, DomainName);
			myCommentTo = LoadFile(doc, CommentToName);
			myGlossary = LoadFile(doc, GlossaryName);
			myHtmlRef = LoadFile(doc, HtmlRefName);
			mySpamRule = LoadFile(doc, SpamRuleName);
			myDiary = LoadFile(doc, DiaryName);

			myAmazonCachePath = LoadDirectory(doc, AmazonCachePathName);

			AmazonWsAccessKeyId = LoadString(doc, AmazonWsAccessKeyIdName);
			AmazonWsSecretKey = LoadString(doc, AmazonWsSecretKeyName);
			AmazonServiceHostName = LoadString(doc, AmazonServiceHostNameName);
			AmazonAssociateTag = LoadString(doc, AmazonAssociateTagName);

		}

		// XML �f�[�^����f�B���N�g�������擾���܂��B
		// �p�X�� ini.xml ����̑��΃p�X�Ƃ��ĉ��߂��܂��B
		private DirectoryInfo LoadDirectory(XmlDocument doc, string elementName){
			string dir = LoadString(doc, elementName);
			if(dir == null) return null;
			string fullpath = Path.Combine(myFile.DirectoryName, dir);
			DirectoryInfo result = new DirectoryInfo(fullpath);
			if(!result.Exists) throw new Exception("�ݒ�t�@�C���� " +  elementName + " �̒l�Ƃ��Ďw�肳�ꂽ�f�B���N�g�� " + fullpath + "���݂���܂���ł����B");
			return result;
		}

		// XML �f�[�^���� FileInfo �����擾���܂��B
		// �p�X�� ini.xml ����̑��΃p�X�Ƃ��ĉ��߂��܂��B
		private FileInfo LoadFile(XmlDocument doc, string elementName, bool fileRequired){
			string file = LoadString(doc, elementName);
			if(file == null) return null;
			string fullpath = Path.Combine(myFile.DirectoryName, file);
			FileInfo result = new FileInfo(fullpath);
			if(fileRequired && !result.Exists) throw new Exception("�ݒ�t�@�C���� " +  elementName + " �̒l�Ƃ��Ďw�肳�ꂽ�t�@�C��" + fullpath + "���݂���܂���ł����B");
			return result;
		}
		// XML �f�[�^���� FileInfo �����擾���܂��B
		// �p�X�� ini.xml ����̑��΃p�X�Ƃ��ĉ��߂��܂��B
		private FileInfo LoadFile(XmlDocument doc, string elementName){
			return LoadFile(doc, elementName, true);
		}

		// XML �f�[�^���當������擾���܂��B
		private string LoadString(XmlDocument doc, string elementName){
			XmlNodeList xnl = doc.GetElementsByTagName(elementName);
			if(xnl == null) return null;
			if(xnl.Count == 0) return null;

			XmlElement e = xnl[0] as XmlElement;
			if(!e.HasAttributes) return null;
			string str = e.Attributes[0].Value;
			return str;
		}


		// XML �f�[�^���� ExtInfo ���擾���܂��B
		private void LoadExtInfo(XmlDocument doc){
			XmlNodeList extInfosBases =  doc.DocumentElement.GetElementsByTagName(ExtensionInfoBase);
			if(extInfosBases.Count == 0) throw new Exception("ini �f�[�^�� " + ExtensionInfoBase + " ���܂܂�Ă��܂���B");
			XmlElement extInfosBase = extInfosBases[0] as XmlElement;
			XmlNodeList elems = extInfosBase.GetElementsByTagName(ExtensionInfo);
			for(int i=0; i < elems.Count; i++){
				XmlElement e = elems[i] as XmlElement;
				ExtInfo ex = new ExtInfo(e);
				if(ex == null) continue;
				if(ex.Name == null) continue;
				myExtInfo[ex.Name] = ex;
			}
		}


	} // End class IniData
} // End Namespace Bakera







