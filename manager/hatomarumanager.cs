using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Web;
using System.Xml;

namespace Bakera.Hatomaru{

/*
	���̃N���X�̓��상��


	Request ���󂯎��
	���N�G�X�g����p�X�𒊏o

	�����L���b�V����T��
	�L���b�V�����Ȃ���΃t�@�C����T��

	�t�@�C���̎�ނɉ����ď�������
	XML �Ȃ� HatomaruGenerator �̃C���X�^���X���擾���� Get �������� Post ����B
	�C���X�^���X���L���b�V���ɂ���΃C���X�^���X����擾�A�����łȂ���΍쐬�B

	Get ���ꂽ�Ƃ��c�c�L���ȃL���b�V�������邩�ǂ������ׂ�
	����ΕԂ�
	������ΐ������ĕԂ�
	�L���b�V���ł���f�[�^�Ȃ�L���b�V������

	Post ���ꂽ�Ƃ��c�c�������Č��ʂ�Ԃ� (Post �̓L���b�V�����Ȃ�)

	�Ԃ��^�͂������ ResponseData�B

�����̃N���X�� Hatomaru �̃C���X�^���X�̃v���C�x�[�g�����o�ƂȂ�
�@Hatomaru �̃C���X�^���X�� Reusable �ŁA���̃N���X�̃C���X�^���X���ė��p�����B

*/


	/// <summary>
	/// hatomaru.dll �̃R���e���c���Ǘ�����N���X�ł��B
	/// URL ��n�����΂����K�؂ɏ������܂��B
	/// </summary>
	public class HatomaruManager{

		// �f�[�^�L���b�V�� : XML�f�[�^�̃L���b�V��
		public CacheManager<HatomaruXml> myDataCache = new CacheManager<HatomaruXml>();

		// �t�@�C���L���b�V�� : ��XML�f�[�^�̃L���b�V��(���ɉ摜�T�C�Y�擾�p)
		public CacheManager<HatomaruFile> myFileCache = new CacheManager<HatomaruFile>();

		// �����L���b�V�� : ���X�|���X�f�[�^�̃L���b�V��
		public ResponseCacheManager myResponseCache;

		// Amazon Manager
		private AmazonManager myAmazonManager = null;

		// Response Title
		private AbsPathKvs myResponseTitle = null;
		private AbsPathKvs myResponseKeywords = null;

		// �����ݒ�
		private readonly IniData myIniData = null;
		private DateTime myIniDataTime;
		private Log myLog = new Log();
		private static readonly char[] myInvalidPathChars;
		private readonly XmlDocument myTemplate;
		private DateTime myTemplateTime;

		private readonly FileInfo myRootXmlFile;

		public const string DirectoryIndexName = "index";
		public const string DirectoryIndexExt = ".xml";
		public const string HatomaruAspxPath = "/hatomaru.aspx";


// �R���X�g���N�^

		/// <summary>
		/// �ÓI�R���X�g���N�^
		/// myInvalidPathChars �̒l��ݒ肵�܂��B
		/// </summary>
		static HatomaruManager(){
			List<char> invalidChars = new List<char>();
			invalidChars.AddRange(Path.GetInvalidPathChars());
			invalidChars.Add('*');
			invalidChars.Add('?');
			myInvalidPathChars = new char[invalidChars.Count];
			invalidChars.CopyTo(myInvalidPathChars);
		}


		/// <summary>
		/// �����ݒ� XML �t�@�C���̃p�X���w�肵�āAHatomaruManager�̃C���X�^���X���J�n���܂��B
		/// </summary>
		public HatomaruManager(string iniFilePath){
			myIniData = new IniData(iniFilePath);
			myIniDataTime = myIniData.File.LastWriteTime;

			myTemplate = new XmlDocument();
			myTemplate.XmlResolver = null;
			myTemplate.Load(myIniData.Template.FullName);
			myTemplateTime = myIniData.Template.LastWriteTime;
			myResponseCache = new ResponseCacheManager();

			// ���[�gXML���擾
			string rootXmlFileName = IniData.DataPath.FullName + '\\' + DirectoryIndexName + DirectoryIndexExt;
			myRootXmlFile = new FileInfo(rootXmlFileName);


			myAmazonManager = new AmazonManager(this);
			myAmazonManager.CacheDir = myIniData.AmazonCachePath;
		}


// �v���p�e�B

		/// <summary>
		/// �����ݒ�f�[�^���擾���܂��B
		/// </summary>
		public IniData IniData{
			get{return myIniData;}
		}

		/// <summary>
		/// ���O�I�u�W�F�N�g���擾���܂��B
		/// </summary>
		public Log Log{
			get{return myLog;}
		}

		/// <summary>
		/// ���[�g XML ���擾���܂��B
		/// </summary>
		public HatomaruXml RootXml{
			get{return GetData(myRootXmlFile);}
		}

		/// <summary>
		/// ���[�g XML �̃t�@�C�����擾���܂��B
		/// </summary>
		public FileInfo RootXmlFile{
			get{return myRootXmlFile;}
		}

		/// <summary>
		/// �e���v���[�g��XmlDocument���擾���܂��B
		/// </summary>
		public XmlDocument Template{
			get{return myTemplate;}
		}

		/// <summary>
		/// �R�����g�Ώۂ� HatomaruBbs ���擾���܂��B
		/// </summary>
		public HatomaruBbs Bbs{
			get{return GetData(IniData.CommentTo) as HatomaruBbs;}
		}

		/// <summary>
		/// HatomaruGlossary ���擾���܂��B
		/// </summary>
		public HatomaruGlossary Glossary{
			get{return GetData(IniData.Glossary) as HatomaruGlossary;}
		}

		/// <summary>
		/// HatomaruHtmlRef ���擾���܂��B
		/// </summary>
		public HatomaruHtmlRef HtmlRef{
			get{return GetData(IniData.HtmlRef) as HatomaruHtmlRef;}
		}

		/// <summary>
		/// SpamRule ���擾���܂��B
		/// </summary>
		public SpamRule SpamRule{
			get{return GetData(IniData.SpamRule) as SpamRule;}
		}

		/// <summary>
		/// AmazonManager ���擾���܂��B
		/// </summary>
		public AmazonManager AmazonManager{
			get{return myAmazonManager;}
		}

		/// <summary>
		/// Diary ���擾���܂��B
		/// </summary>
		public DiaryIndex Diary{
			get{
				return GetData(IniData.Diary) as DiaryIndex;
			}
		}

		/// <summary>
		/// ���̃C���X�^���X���ŐV���ǂ������������A�ŐV�ł���� false ���A�Â���� true ��Ԃ��܂��B
		/// IniData �� Template ���X�V����Ă���� true ��Ԃ��܂��B
		/// </summary>
		public bool IsOld{
			get{
				myIniData.File.Refresh();
				myIniData.Template.Refresh();
				return myIniDataTime < myIniData.File.LastWriteTime || myTemplateTime < myIniData.Template.LastWriteTime;
			}
		}

		/// <summary>
		/// ���X�|���X�^�C�g�����L���b�V������AbsPathKvs���擾���܂��B
		/// </summary>
		public AbsPathKvs ResponseTitle{
			get{
				if(myResponseTitle == null) myResponseTitle = new AbsPathKvs(myIniData.TitleCacheFile);
				return myResponseTitle;
			}
		}

		/// <summary>
		/// ���X�|���X�L�[���[�h���L���b�V������AbsPathKvs���擾���܂��B
		/// </summary>
		public AbsPathKvs ResponseKeywords{
			get{
				if(myResponseKeywords == null) myResponseKeywords = new AbsPathKvs(myIniData.KeywordCacheFile);
				return myResponseKeywords;
			}
		}


// �p�u���b�N���\�b�h - ���X�|���X�n

		/// <summary>
		/// ���N�G�X�g���� ResponseData �𐶐����ĕԂ��܂��B
		/// </summary>
		public HatomaruResponse GetResponse(HttpRequest req){
			if(req == null) throw new Exception("���N�G�X�g���n����Ă��܂���B");
			string method = req.HttpMethod.ToLower(System.Globalization.CultureInfo.InvariantCulture);
			switch(method){
			case "get":
			case "head":
				return Get(req);

			case "post":
				return Post(req);
			default:
				return new NotAllowedResponse("GET,POST");
			}
		}

		/// <summary>
		/// Get ���N�G�X�g���� ResponseData �𐶐����ĕԂ��܂��B
		/// </summary>
		public HatomaruResponse Get(HttpRequest r){
			string path = r.RawUrl;
			AbsPath absPath = new AbsPath(path);
			return GetResponse(absPath);
		}


		public HatomaruResponse GetResponse(AbsPath absPath){

			// �L���ȃ��X�|���X���L���b�V������Ă���΂����Ԃ�
			HatomaruResponse result = myResponseCache.GetEneabledCache(absPath);
			if(result != null) return result;

			// ���ꃌ�X�|���X�ɊY������Ȃ烊�_�C���N�g����
			if(absPath.Contains(HatomaruAspxPath)){
				result = GetHatomaruAspxRedirect(absPath);
				result.SetLastModified();
				myResponseCache.Add(absPath, result);
			}
			if(result != null) return result;

			// �L���b�V�����Ȃ� or �p�X���ςȂ̂ŁA�ЂƂ܂��t�@�C���𓯒肷��
			FileInfo f = SingleFileSearch(absPath);
			if(f == null) throw new Exception("�t�@�C���������ł��܂���ł����B");
			// ���肵���t�@�C������ HatomaruData ���擾
			HatomaruData hd = null;
			ExtInfo ex = myIniData.ExtInfo[f.Extension];
			if(ex.ContentType == null){
				hd = GetData(f);
			} else {
				hd = GetFile(f);
			}
			if(hd == null) throw new Exception("�f�[�^�I�u�W�F�N�g�̎擾�Ɏ��s���܂����B�����ł��Ȃ��f�[�^�`����������܂���B");
			HatomaruResponse res = hd.Get(absPath);
			res.SetLastModified();
			myResponseCache.Add(absPath, res);
			// ���_�C���N�g�̖������[�v�`�F�b�N
			if(res is RedirectResponse){
				RedirectResponse rr = res as RedirectResponse;
				if(absPath == rr.DestPath) throw new Exception("�������_�C���N�g�x�� : ���N�G�X�g���ꂽ�p�X���g�Ƀ��_�C���N�g���鉞����Ԃ����Ƃ��܂����B");
			}

			// �^�C�g���ƃL�[���[�h���L���b�V��
			if(!string.IsNullOrEmpty(res.Title)) ResponseTitle.Add(absPath, res.FullTitle);
			if(!string.IsNullOrEmpty(res.Keywords)) ResponseKeywords.Add(absPath, res.Keywords);
			return res;
		}


		/// <summary>
		/// Post ���N�G�X�g���� ResponseData �𐶐����ĕԂ��܂��B
		/// </summary>
		public HatomaruResponse Post(HttpRequest r){
			AbsPath absPath = new AbsPath(r.RawUrl);

			// �ЂƂ܂��t�@�C���𓯒肷��
			AbsPath filePath = new AbsPath(r.Path);
			FileInfo f = SingleFileSearch(filePath);
			if(f == null) throw new Exception("�t�@�C���������ł��܂���ł����B");
			Log.Add("{0} : �t�@�C���𓯒肵�܂��� : {1}", absPath, f.FullName);

			// ���肵���t�@�C������ HatomaruData ���擾
			HatomaruData hd = null;
			ExtInfo ex = myIniData.ExtInfo[f.Extension];
			if(ex.ContentType == null){
				hd = GetData(f);
			} else {
				hd = GetFile(f);
			}
			if(hd == null) throw new Exception("�f�[�^�I�u�W�F�N�g�̎擾�Ɏ��s���܂����B�����ł��Ȃ��f�[�^�`����������܂���B");
			return hd.Post(absPath, r);
		}


		/// <summary>
		/// �n���ꂽ AbsPath �ɑ�������R���e���c�̃^�C�g�����擾���܂��B
		/// </summary>
		public string GetResponseTitle(AbsPath absPath){
			string result = ResponseTitle[absPath];
			if(result != null) return result;
			HatomaruResponse hx = GetResponse(absPath);
			return hx.Title;
		}


		/// <summary>
		/// �n���ꂽ AbsPath �ɑ�������R���e���c�̃L�[���[�h���擾���܂��B
		/// </summary>
		public string GetResponseKeywords(AbsPath absPath){
			string result = ResponseKeywords[absPath];
			if(result != null) return result;
			HatomaruResponse hx = GetResponse(absPath);
			return hx.Keywords;
		}



// �p�u���b�N���\�b�h - �⏕�n

		/// <summary>
		/// �n���ꂽ FileInfo �ɑ������� HatomaruXml �f�[�^���擾���܂��B
		/// </summary>
		public HatomaruXml GetData(FileInfo f){
			if(f == null) return null;
			// �L���b�V���ɂ���̂�?
			HatomaruXml result = myDataCache.GetEneabledCache(f.FullName);
			if(result != null){
				Log.Add("Cached : {0}", f.FullName);
				return result;
			}
			result = HatomaruXml.GetHatomaruXml(this, f);
			if(result != null){
				myDataCache.Add(f.FullName, result);
				Log.Add("Loaded : {0} {1}", f.FullName, result.GetType());
				return result;
			}
			Log.Add("Not Found : {0}", f.FullName);
			return null;
		}

		/// <summary>
		/// �n���ꂽ FileInfo �ɑ������� HatomaruFile �f�[�^���擾���܂��B
		/// </summary>
		public HatomaruFile GetFile(FileInfo f){
			// �L���b�V���ɂ���̂�?
			HatomaruFile result = myFileCache.GetEneabledCache(f.FullName);
			if(result != null){
				Log.Add("Cached : {0}", f.FullName);
				return result;
			}

			result = new HatomaruFile(this, f);
			if(result == null){
				Log.Add("Not Found : {0}", f.FullName);
				return null;
			}

			if(result.Ext == null){
				Log.Add("Unknown Extension : {0}", f.FullName);
				return null;
			}

			myFileCache.Add(f.FullName, result);
			Log.Add("Loaded : {0}", f.FullName);
			return result;
		}


		/// <summary>
		/// �L���b�V������Ă��� HatomaruResponse �����ׂĎ擾���܂��B
		/// </summary>
		public HatomaruResponse[] GetCachedResponse(){
			return myResponseCache.GetAllData();;
		}


		/// <summary>
		/// �p�X����P��̃t�@�C����{�����܂��B
		/// </summary>
		public FileInfo SingleFileSearch(AbsPath path){
			List<FileInfo> files = FileSearch(path);
			if(files.Count == 0) return null;
			if(files.Count == 1) return files[0];
			files.Sort((x, y) => x.Name.Length - y.Name.Length);
			return files[0];
		}




// �v���C�x�[�g���\�b�h


		// �t�@�C����{�����܂��B
		private List<FileInfo> FileSearch(AbsPath absPath){
			Log.Add("FileSearch : {0}", absPath);
			string basePath = myIniData.DataPath.FullName;
			List<FileInfo> result = new List<FileInfo>();
			string path = absPath.RemoveQuery().ToString();

			// �A������ .. �� // �͂܂Ƃ߂�
			while(path.IndexOf("..") >= 0){
				path = path.Replace("..", ".");
			}
			while(path.IndexOf("//") >= 0){
				path = path.Replace("//", "/");
			}
			while(path.IndexOf(":") >= 0){
				path = path.Replace(":", "");
			}
			while(path.IndexOf(")") >= 0){
				path = path.Replace(")", "");
			}
			while(path.IndexOf("(") >= 0){
				path = path.Replace("(", "");
			}

			for(;;){
				string searchPath = basePath + path;
				if(searchPath.IndexOfAny(myInvalidPathChars) < 0){
					string lastName = null;
					DirectoryInfo currentDir = null;

					// �f�B���N�g���ł���?
					if(Directory.Exists(searchPath)){
						currentDir = new DirectoryInfo(searchPath);
						lastName = DirectoryIndexName;
					} else {
						lastName = Util.GetLastName(path);
						FileInfo f = new FileInfo(searchPath);
						currentDir = f.Directory;
					}

					if(currentDir.Exists){
						FileInfo[] files = currentDir.GetFiles(lastName + "*");
						foreach(FileInfo f in files){
							string ext = f.Extension;
							if(myIniData.ExtInfo.ContainsKey(ext)){
								result.Add(f);
							}
						}
						if(result.Count > 0) return result;
					}
				}
				if(path == "/") break;
				path = Util.ShortenPath(path);
			}
			return result;
		} // End FileSearch()




// ���ꃌ�X�|���X

	// hatomaru.aspx ���܂� URL �����_�C���N�g����
		private RedirectResponse GetHatomaruAspxRedirect(AbsPath path){
			AbsPath destPath = path.Remove(HatomaruAspxPath);
			RedirectResponse red = new RedirectResponse(destPath, IniData.Domain, path);
			return red;
		}


	} // End Class HatomaruManager

} // End Namespace 
