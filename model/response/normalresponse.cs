using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Web;

namespace Bakera.Hatomaru{

	/// <summary>
	/// text/html �������� application/xhtml+xml �� 200 OK ��Ԃ����X�|���X�ł��B
	/// </summary>
	public class NormalResponse: HatomaruResponse{

		private FileInfo myCacheFile;

		/// <summary>
		/// �f�[�^�\�[�X��AbsPath���w�肵�āAHatomaruResponse �̃C���X�^���X���J�n���܂��B
		/// �ʏ�͂��̃R���X�g���N�^���g�p���܂��B
		/// </summary>
		public NormalResponse(HatomaruXml source, AbsPath path) : base(source){
			myPath = path;
		}



// �v���p�e�B


		/// <summary>
		/// ���̃��X�|���X���L���b�V�����ėǂ��������l��ݒ�E�擾���܂��B
		/// normalresponse �̓L���b�V���\�ł��B
		/// </summary>
		public override bool IsCacheable {
			get{return true;}
		}

		/// <summary>
		/// ���̃C���X�^���X���j������Ă���� true, �g�p�ł���Ȃ� false ��Ԃ��܂��B
		/// ����ł�false��Ԃ��܂����A�h���N���X�ł̓L���b�V���f�[�^������ꂽ�ꍇ�� true ��Ԃ��悤�Ɏ������܂��B
		/// </summary>
		public override bool IsExpired{
			get{
				// HTML �����邩�L���b�V���t�@�C��������� false
				if(Html != null) return false;
				if(myCacheFile != null){
					myCacheFile.Refresh();
					if(myCacheFile.Exists) return false;
				}
				return true;
			}
		}

// public ���\�b�h


		/// <summary>
		/// �n���ꂽ HttpResponse �Ƀ��X�|���X���������݂܂��B
		/// ��x��������t�@�C���ɕۑ����܂��B
		/// </summary>
		public override void SetLastModified(){
			base.SetLastModified();
			string resultStr = Html.OuterXml;
			if(IsCacheable && Html != null){
				string cacheName = GetCacheName() + "_cache";
				string fullName = string.Format("{0}{1:X}", cacheName, resultStr.GetHashCode());
				myCacheFile = new FileInfo(fullName);
				if(myCacheFile.Exists){
					myLastModified = myCacheFile.LastWriteTime;
				} else {
					if(myCacheFile.Directory.Exists){
						string filename = System.IO.Path.GetFileName(cacheName);
						FileInfo[] delFiles = myCacheFile.Directory.GetFiles(filename +"*");
						Array.ForEach(delFiles, item=>{item.Delete();});
					}
					Html.SaveFile(myCacheFile);
					myCacheFile.LastWriteTime = myLastModified;
				}
			}
		}

		/// <summary>
		/// �n���ꂽ HttpResponse �Ƀ��X�|���X���������݂܂��B
		/// ��x��������t�@�C���ɕۑ����܂��B
		/// </summary>
		public override void WriteResponse(HttpResponse response){
			if(Html != null){
				string resultStr = Html.OuterXml;
				response.Write(resultStr);
				// ������x����
				if(myCacheFile.Exists) Html = null;
			} else if(myCacheFile != null){
				myCacheFile.Refresh();
				if(myCacheFile.Exists){
					response.WriteFile(myCacheFile.FullName);
				} else {
					Util.Throw("???");
				}
			}
			WriteResponseHeader(response);
			SetLastModified(response);
		}


// �v���C�x�[�g���\�b�h

		protected string GetCacheName(){
			string cachePath = myBaseSource.Manager.IniData.ResponseCachePath.FullName.TrimEnd('\\') + myPath.ToString();
			return cachePath;
		}


	}

}
