using System;
using System.IO;
using System.Drawing;

namespace Bakera.Hatomaru{


/// <summary>
/// ���ۃf�[�^
/// �f�[�^�\�[�X�̃t�@�C���������N���X�ł��B
/// </summary>
	public class HatomaruFile : HatomaruData, ICacheData{
		
		private static string[] imageExtension = new string[]{".png", ".jpg", ".gif"};
		private Size mySize;

// �R���X�g���N�^

		/// <summary>
		/// HatomaruFile �̃C���X�^���X���J�n���܂��B
		/// </summary>
		public HatomaruFile(HatomaruManager manager, FileInfo f) : base(manager, f){
		}


// �v���p�e�B

		/// <summary>
		/// �摜�T�C�Y���擾���܂��B
		/// </summary>
		public Size Size{
			get{
				if(!mySize.IsEmpty) return mySize;
				mySize = CalcImageSize();
				return mySize;
			}
		}


// �p�u���b�N���\�b�h

		/// <summary>
		/// �f�[�^�� GET ���AHatomaruResponse ���擾���܂��B
		/// �t�@�C���̃p�X���������Ȃ���΃��_�C���N�g���܂� (�g���q�̗L���̂݋��e���܂�)
		/// </summary>
		public override HatomaruResponse Get(AbsPath path){
			FileResponse result = new FileResponse(this, Ext);

			if(path != BasePath && path != BasePath.CombineExtension(File.Extension)){
				return new RedirectResponse(BasePath, Manager.IniData.Domain);
			}

			result.Path = BasePath;
			result.SetLastModified();
			return result;
		}

		/// <summary>
		/// �f�[�^�� Post ���AHatomaruResponse ���擾���܂��B
		/// </summary>
		public override HatomaruResponse Post(AbsPath path, System.Web.HttpRequest req){
			HatomaruResponse result = new NotAllowedResponse("GET");
			return result;
		}




// ICacheData �C���^�[�t�F�C�X�̎���

		/// <summary>
		/// ���̃f�[�^���L���b�V�����Ă��ǂ����ǂ�����Ԃ��܂��B
		/// hatomaruFile �͏�ɃL���b�V���\�ł��B
		/// </summary>
		public bool IsCacheable{
			get{return true;}
		}

		/// <summary>
		/// ���̃C���X�^���X�� Last-Modified �ƃt�@�C���̍X�V�����r���܂��B
		/// �C���X�^���X���ŐV�Ȃ�� true ���A�Â���� false ��Ԃ��܂��B
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




// �v���C�x�[�g���\�b�h

		// �摜�̃T�C�Y���擾���܂��B
		private Size CalcImageSize(){
			Size result = default(Size);
			using(FileStream imgStream = File.Open(FileMode.Open, FileAccess.Read, FileShare.Read)){
				using(System.Drawing.Bitmap bmp = new System.Drawing.Bitmap(imgStream)){
					result = bmp.Size;
				}
			}
			return result;
		}



	} // End class HatomaruFile
} // End Namespace Bakera

