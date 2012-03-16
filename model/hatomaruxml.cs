using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Web;
using System.Xml;

namespace Bakera.Hatomaru{

/* ���상��
ICacheData �c�c �L���b�V���ł���f�[�^�����C���^�[�t�F�C�X
HatomaruData �c�c �f�[�^�\�[�X���������ۃN���X
HatomaruFile �c�c �P�Ƀt�@�C���̏ꏊ�Ɗg���q�������B�L���b�V���s�v
HatomaruXml : HatomaruData �c�c HatomaruData �ɉ����� XmlDocument ��ێ�
abstract HatomaruTable : HatomaruXml �c�c HatomaruXml �ɉ����āADataTable ��ێ�
*/



/// <summary>
/// ���ۃf�[�^�� XML �������N���X�ł��B
/// </summary>
	public abstract class HatomaruXml : HatomaruData, ICacheData{

		private readonly XmlDocument myDocument = null;
		private string myBaseTitle;
		private string myDescription;
		private string myKeywords;
		private string myTopicSuffix;
		private List<LinkItem> myStyles = new List<LinkItem>();
		private readonly HatomaruXml myParentXml;

		private XmlElement myMetaData;

		public const int SaveRetryTime = 5;
		public const int SaveRetryInterval = 1000; // �~���b

		public const string Name = null;
		public const string MetaName = "metadata";
		public const string TitleName = "title";
		public const string DescName = "description";
		public const string KeywordName = "keyword";
		public const string StyleName = "style";
		public const string TopicPathName = "topicpath";
		public const string TopicSuffixName = "topicname";
		public const string IndexXmlName = "index";
		public const string IndexXmlRefAttrName = "ref";

		private static readonly Type[] mySubTypes;

// �ÓI�R���X�g���N�^
// Hatomaru.Xml�̃T�u�^�C�v�����W���� mySubTypes �Ɋi�[���܂��B
		static HatomaruXml(){
			Type thisType = typeof(HatomaruXml);
			Assembly asm = Assembly.GetAssembly(thisType);
			List<Type> types = new List<Type>();

			foreach(Type t in asm.GetTypes()){
				if(t.IsSubclassOf(thisType)) types.Add(t);
			}
			mySubTypes = types.ToArray();
		}


// �R���X�g���N�^

		/// <summary>
		/// ���ۃf�[�^�̃f�[�^�\�[�X�� FileInfo �� XmlDocument ���w�肵�āAHatomaruData �̃C���X�^���X���J�n���܂��B
		/// </summary>
		protected HatomaruXml(HatomaruManager manager, FileInfo f, XmlDocument doc) : base(manager, f){
			myDocument = doc; // ���[�h�ς�
			
			myMetaData = Document.DocumentElement[MetaName];
			if(myMetaData == null) return;
			
			XmlElement title = myMetaData[TitleName];
			if(title != null) myBaseTitle = title.InnerText;
			XmlElement desc = myMetaData[DescName];
			if(desc != null) myDescription = desc.InnerText;
			XmlElement keywords = myMetaData[KeywordName];
			if(keywords != null) myKeywords = keywords.InnerText;

			XmlElement topicSuffix = myMetaData[TopicSuffixName];
			if(topicSuffix != null) myTopicSuffix = topicSuffix.InnerText;

			XmlNodeList styles = myMetaData.GetElementsByTagName(StyleName);
			foreach(XmlNode x in styles){
				XmlElement e = x as XmlElement;
				if(x == null) continue;
				string linkHref = e.GetAttribute("href");
				string linkTitle = e.GetAttribute("title");
				linkHref = '/' + linkHref.TrimStart('/');
				LinkItem s = new LinkItem(linkHref, linkTitle);
				myStyles.Add(s);
			}

			// �e XML ���擾
			XmlNodeList indexes = myMetaData.GetElementsByTagName(IndexXmlName);
			if(indexes.Count > 0){
				string indexXmlName = Util.GetAttributeValue(indexes[0], IndexXmlRefAttrName);
				if(!string.IsNullOrEmpty(indexXmlName)) myParentXml = GetDataByPathString(indexXmlName);
			} else if(!manager.RootXmlFile.FullName.Equals(f.FullName)){
				myParentXml = manager.RootXml;
			}

		}

// �v���p�e�B

		/// <summary>
		/// XmlDocument ���擾���܂��B
		/// </summary>
		public XmlDocument Document{
			get{return myDocument;}
		}

		/// <summary>
		/// metadata �v�f���擾���܂��B
		/// </summary>
		public XmlElement MetaData{
			get{return myMetaData;}
		}

		/// <summary>
		/// �x�[�X�^�C�g�����擾���܂��B
		/// </summary>
		public string BaseTitle{
			get{return myBaseTitle;}
		}

		/// <summary>
		/// ���� XML �̃C���f�N�X�ƂȂ� XML �f�[�^���擾���܂��B
		/// ���̃v���p�e�B���Q�Ƃ���ƁA�C���f�N�X XML ���f�[�^�\�[�X�Ƃ���
		/// </summary>
		public HatomaruXml ParentXml{
			get{
				return myParentXml;
			}
		}

		/// <summary>
		/// description ���擾���܂��B
		/// </summary>
		public string Description{
			get{return myDescription;}
		}

		/// <summary>
		/// keywords ���擾���܂��B
		/// </summary>
		public string Keywords{
			get{return myKeywords;}
		}

		/// <summary>
		/// �g�s�b�N�� Suffix (�u�L���v�u���ѓ��L�v�Ȃ�) ���擾���܂��B
		/// </summary>
		public string TopicSuffix{
			get{return myTopicSuffix;}
		}

		/// <summary>
		/// �X�^�C���V�[�g�փ����N���� LinkItem ���擾���܂��B
		/// </summary>
		public LinkItem[] Styles{
			get{return myStyles.ToArray();}
		}


// �p�u���b�N���\�b�h

		/// <summary>
		/// BasePath �����ɁA���Ύw��� string �őΏۃt�@�C�������߁A�t�@�C���f�[�^���擾���܂��B
		/// </summary>
		public HatomaruXml GetDataByPathString(string pathStr){
			string path = Path.Combine(File.Directory.FullName, pathStr);
			FileInfo f = new FileInfo(path);
			if(!f.Exists){
				f = new FileInfo(path + ".xml");
				if(!f.Exists){
					f = new FileInfo(path + "/index.xml");
					if(!f.Exists) throw new Exception("�t�@�C����������܂��� : " + path);
				}
			}
			Manager.Log.Add("{0} : GetDataByPathString({1})  -> {2}", this, pathStr, f.FullName);
			// ���g�������肷��Ɩ������[�v�ɂȂ�
			if(File.FullName == f.FullName) throw new Exception("GetDataByPathString : ���g���擾���悤�Ƃ��܂����B : " + f.FullName);
			return Manager.GetData(f);
		}

		/// <summary>
		/// ���� HatomaruXml �Ƀ����N���� LinkItem ���擾���܂��B
		/// </summary>
		public LinkItem GetLinkItem(){
			return new LinkItem(BasePath, BaseTitle);
		}

		/// <summary>
		/// ���� HatomaruXml ���烊���N���� HatomaruXml �̃��X�g���擾���܂��B
		/// </summary>
		public HatomaruXml[] GetChildren(){
			XmlNodeList nodes = Document.DocumentElement.GetElementsByTagName(HatomaruActionBase.MenuitemElement);
			HatomaruXml[] result = new HatomaruXml[nodes.Count];
			for(int i=0; i < nodes.Count; i++){
				XmlElement elem = nodes[i] as XmlElement;
				string linkSource = elem.GetAttribute("src");
				result[i] = GetDataByPathString(linkSource);
			}
			return result;
		}


// �I�[�o�[���C�h���\�b�h

		/// <summary>
		/// �f�[�^�� Post ���AHatomaruResponse ���擾���܂��B
		/// HatomaruXml �ɋ��ʂ́A�R�����g���e���������郁�\�b�h�ł��B
		/// </summary>
		public override HatomaruResponse Post(AbsPath path, HttpRequest req){
			HatomaruPostAction pa = GetPostAction(path, req);
			if(pa == null) new NotAllowedResponse("get", this.BaseTitle + " �� POST ���󂯕t���܂���B");

			HatomaruResponse result = pa.Post();
			return result;
		}


		/// <summary>
		/// Post ���������邽�߂� HatomaruPostAction ���擾���܂��B
		/// ���̃��f���� POST ���������Ȃ��ꍇ�� null ��Ԃ��܂��B
		/// </summary>
		protected virtual HatomaruPostAction GetPostAction(AbsPath path, HttpRequest req){
			return new BbsPostAction(this, path, req);
		}





// ICacheData �C���^�[�t�F�C�X�̎���

		/// <summary>
		/// ���̃f�[�^���L���b�V�����Ă��ǂ����ǂ�����Ԃ��܂��B
		/// hatomaruXml �͏�ɃL���b�V���\�ł��B
		/// </summary>
		public bool IsCacheable{
			get{return true;}
		}

		/// <summary>
		/// ���̃C���X�^���X�� Last-Modified �ƃt�@�C���̍X�V�����r���܂��B
		/// �C���X�^���X�����ŐV�Ȃ�� true ���A�Â���� false ��Ԃ��܂��B
		/// </summary>
		public override bool IsNewest{
			get{
				File.Refresh();
				if(File.LastWriteTime > LastModified) return false;
				return true;
			}
		}

		/// <summary>
		/// ���̃C���X�^���X���j������Ă���� true, �g�p�ł���Ȃ� false ��Ԃ��܂��B
		/// �t�@�B�����폜����Ă���ꍇ�ɂ� true ���Ԃ�܂��B
		/// </summary>
		public virtual bool IsExpired{
			get{
				return !File.Exists;
			}
		}

		/// <summary>
		/// ���̃C���X�^���X�̃��t���b�V�������݂܂��B
		/// ��������ΐV�����C���X�^���X���A���s����� null ��Ԃ��܂��B
		/// </summary>
		public HatomaruXml Refresh(){
			if(IsNewest) return this;
			return null;
		}


// �ÓI���\�b�h

		// XML�f�[�^��ǂݎ��AHatomaruXml �𐶐����܂��B
		public static HatomaruXml GetHatomaruXml(HatomaruManager manager, FileInfo f){
			try{
				XmlDocument x = new XmlDocument();
				x.XmlResolver = null;
				for(int i=0;;i++){
					try{
						x.Load(f.FullName);
						break;
					} catch(IOException e){
						System.Threading.Thread.Sleep(SaveRetryInterval);
						if(i > SaveRetryTime){
							throw new HatomaruXmlException("�f�[�^�̃��[�h�Ɏ��s���܂����B", e);
						}
					}
				}
				if(x.DocumentElement == null) throw new HatomaruXmlException("XML�t�@�C�����󂩁A���[�g�v�f������܂���B");
				string rootName = x.DocumentElement.Name;
				foreach(Type t in mySubTypes){
					string name = GetFieldValue(t, "Name");
					if(name.Eq(rootName)){
						ConstructorInfo ci = t.GetConstructor(new Type[]{typeof(HatomaruManager), typeof(FileInfo), typeof(XmlDocument)});
						if(ci == null) throw new Exception(name + "�ɂ͓K�؂ȃR���X�g���N�^������܂���B");
						Object o = ci.Invoke(new Object[]{manager, f, x});
						HatomaruXml result = o as HatomaruXml;
						return result;
					}
				}
				return new HatomaruDoc(manager, f, x);
			} catch(System.Data.ConstraintException e){
				throw new HatomaruXmlException("�f�[�^�̃��[�h�Ɏ��s���܂����B", e);
			} catch(System.Xml.XmlException e){
				throw new HatomaruXmlException("XML�f�[�^�̃p�[�X�Ɏ��s���܂����B", e);
			}
		}

		// �t�B�[���h�̒l���擾���܂��B
		private static string GetFieldValue(Type t, string name){
			Object o = t.GetField(name).GetValue(null);
			if(o == null) return null;
			return o.ToString();
		}


	} // End class HatomaruXml
} // End Namespace Bakera



