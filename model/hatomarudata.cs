using System;
using System.IO;
using System.Web;

namespace Bakera.Hatomaru{

/* ���상��
ICacheData �c�c �L���b�V���ł���f�[�^�����C���^�[�t�F�C�X
HatomaruData �c�c �f�[�^�\�[�X���������ۃN���X
HatomaruFile �c�c �P�Ƀt�@�C���̏ꏊ�Ɗg���q�������B�L���b�V���s�v
HatomaruXml : HatomaruData �c�c HatomaruData �ɉ����� XmlDocument ��ێ�
abstract HatomaruTable : HatomaruXml �c�c HatomaruXml �ɉ����āADataTable ��ێ�
*/




/// <summary>
/// ���ۃf�[�^
/// �f�[�^�\�[�X�̃t�@�C���������N���X�ł��B
/// </summary>
	public abstract class HatomaruData{
		private FileInfo myFile;
		private readonly HatomaruManager myManager;
		private readonly ExtInfo myExt;
		private readonly AbsPath myBasePath;
		private DateTime myLastModified;



// �R���X�g���N�^

		/// <summary>
		/// ���ۃf�[�^�̃f�[�^�\�[�X�� FileInfo ���w�肵�āAHatomaruData �̃C���X�^���X���J�n���܂��B
		/// </summary>
		protected HatomaruData(HatomaruManager manager, FileInfo f){
			myManager = manager;
			myFile = f;
			myLastModified = myFile.LastWriteTime;
			if(Manager.IniData.ExtInfo.ContainsKey(f.Extension.ToLower())){
				myExt = Manager.IniData.ExtInfo[f.Extension.ToLower()];
			}
			myBasePath = GetTrueFilePath();
		}

// �v���p�e�B

		/// <summary>
		/// �t�@�C����ǂݍ��񂾎������擾���܂��B
		/// </summary>
		public virtual DateTime LastModified{
			get{return myLastModified;}
		}


		/// <summary>
		/// �f�[�^�\�[�X���ŐV���ǂ������`�F�b�N���A�ŐV�Ȃ� true ���A�Â��Ȃ��Ă���� false ��Ԃ��܂��B
		/// </summary>
		public abstract bool IsNewest{ get; }


		/// <summary>
		/// ���ۃf�[�^�̃f�[�^�\�[�X�t�@�C�������� FileInfo ���擾���܂��B
		/// </summary>
		public FileInfo File{
			get{return myFile;}
		}

		/// <summary>
		/// �f�[�^�\�[�X�t�@�C���̒u����Ă���f�B���N�g���𒲂ׁA�Ή�����摜�t�@�C���f�B���N�g�����擾���܂��B
		/// </summary>
		public DirectoryInfo ImageDirectoryPath{
			get{
				string fileDir = myFile.Directory.FullName;
				string imgPath = fileDir + '\\' + Manager.IniData.ImageDir;
				DirectoryInfo result = new DirectoryInfo(imgPath);
				return result;
			}
		}

		/// <summary>
		/// �������� HatomaruManager ���擾���܂��B
		/// </summary>
		public HatomaruManager Manager{
			get{return myManager;}
		}

		/// <summary>
		/// ���ۃf�[�^�� ExtInfo ���擾���܂��B
		/// </summary>
		public ExtInfo Ext{
			get{return myExt;}
		}

		/// <summary>
		/// �f�[�^�̊�p�X���擾���܂��B
		/// ����̓p�����[�^�������Ȃ��ꍇ�̐�΃p�X�ł��B
		/// </summary>
		public AbsPath BasePath{
			get{return myBasePath;}
		}





// ���ۃp�u���b�N���\�b�h

		/// <summary>
		/// �f�[�^�� GET ���AHatomaruResponse ���擾���܂��B
		/// </summary>
		public abstract HatomaruResponse Get(AbsPath path);

		/// <summary>
		/// �f�[�^�� Post ���AHatomaruResponse ���擾���܂��B
		/// </summary>
		public abstract HatomaruResponse Post(AbsPath path, HttpRequest req);



// �I�[�o�[���C�h���\�b�h

		/// <summary>
		/// ���ۃf�[�^�̃f�[�^�\�[�X�t�@�C���̃t���p�X�����擾���܂��B
		/// </summary>
		public override string ToString(){
			return File.FullName;
		}


		/// <summary>
		/// �t�@�C���Ƀ����N���邽�߂� Uri ��Ԃ��܂��B
		/// </summary>
		public virtual Uri GetLinkUri(){
			return BasePath.GetAbsUri(Manager.IniData.Domain);
		}



// �v���e�N�g���\�b�h

// �v���C�x�[�g���\�b�h

		// �t�@�C���p�X���� URL �̃p�X���擾���܂��B
		private AbsPath GetTrueFilePath(){
			string ext = File.Extension;
			string lastName = "\\" + HatomaruManager.DirectoryIndexName;
			string basePath = Manager.IniData.DataPath.FullName.TrimEnd('\\');

			string result = File.FullName;
			if(result.IndexOf(basePath) < 0) return null;
			result = result.CutRight(ext);
			result = result.CutRight(lastName);
			result = result.CutLeft(basePath);
			result = result.Replace('\\', '/');

			if(result == "") result = "/";

			return new AbsPath(result.ToLower());
		}




	} // End class HatomaruData
} // End Namespace Bakera







