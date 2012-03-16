using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Web;

namespace Bakera.Hatomaru{

	/// <summary>
	/// 200 OK �Ńt�@�C����Ԃ����X�|���X�ł��B
	/// </summary>
	public class FileResponse : HatomaruResponse{

		private FileInfo myFileSource;
		private ExtInfo myExtInfo;

		/// <summary>
		/// �f�[�^�\�[�X�� ExtInfo �����ɁAFileResponse �̃C���X�^���X���J�n���܂��B
		/// �t�@�C���f�[�^��Ԃ����X�|���X�Ɏg�p���܂��B
		/// </summary>
		public FileResponse(HatomaruData source, ExtInfo ex) : base(source){
			myExtInfo = ex;
			ContentType = ex.ContentType;
			Charset = ex.Charset;
			myFileSource = source.File;
		}

// �v���p�e�B

		/// <summary>
		/// ���̃��X�|���X�̃t�@�C���\�[�X��ݒ�E�擾���܂��B
		/// </summary>
		public FileInfo FileSource{
			get{return myFileSource;}
		}

		/// <summary>
		/// ���̃��X�|���X�̊g���q�����擾���܂��B
		/// </summary>
		public ExtInfo ExtInfo{
			get{return myExtInfo;}
		}

		/// <summary>
		/// ���̃��X�|���X���L���b�V�����ėǂ��������l��ݒ�E�擾���܂��B
		/// FileResponse �̓L���b�V���\�ł����A�L���b�V�����܂���B
		/// </summary>
		public override bool IsCacheable {
			get{return false;}
		}

		/// <summary>
		/// ���X�|���X�̃T�C�Y���擾���܂��B
		/// </summary>
		public override long Length{
			get{
				return FileSource.Length;
			}
		}


// �I�[�o�[���C�h

		/// <summary>
		/// �f�[�^�\�[�X�̒��ł����Ƃ��V�������̂̍ŏI�X�V�������擾���܂��B
		/// �f�[�^�\�[�X���Ȃ��ꍇ�� DateTime.MinValue ��Ԃ��܂��B
		/// </summary>
		public override DateTime GetNewestSourceTime(){
			if(FileSource == null) return default(DateTime);
			return FileSource.LastWriteTime;
		}


		/// <summary>
		/// �n���ꂽ HttpResponse �Ƀ��X�|���X���������݂܂��B
		/// </summary>
		public override void WriteResponse(HttpResponse response){
			WriteResponseHeader(response);
			WriteAdditionalHeader(response);
			SetLastModified(response);
			response.WriteFile(FileSource.FullName);
		}


		/// <summary>
		/// �n���ꂽ HttpResponse �Ƀ��X�|���X�w�b�_���������݂܂��B
		/// </summary>
		protected void WriteAdditionalHeader(HttpResponse response){
			// Disposition = true �Ȃ�_�E�����[�h��
			if(myExtInfo.Disposition){
				string condispValue = String.Format(System.Globalization.CultureInfo.InvariantCulture, "attachment; filename={0}", FileSource.Name);
				response.AppendHeader("Content-Disposition", condispValue);
			}

			if(myExtInfo.MaxAge > TimeSpan.Zero){
				response.Cache.SetCacheability(HttpCacheability.Public);
				response.Cache.SetMaxAge(myExtInfo.MaxAge);
				response.Cache.SetExpires(DateTime.Now + myExtInfo.MaxAge);
			}

		}



	}

}
