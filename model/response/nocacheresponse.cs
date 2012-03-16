using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Web;

namespace Bakera.Hatomaru{

	/// <summary>
	/// �L���b�V������Ȃ����X�|���X�𐶐����܂��B
	/// text/html �������� application/xhtml+xml �� 200 OK ��Ԃ����X�|���X�ł��B
	/// </summary>
	public class NoCacheResponse : NormalResponse{

		/// <summary>
		/// �f�[�^�\�[�X���w�肵�āANoCacheResponse �̃C���X�^���X���J�n���܂��B
		/// </summary>
		public NoCacheResponse(HatomaruXml source, AbsPath path) : base(source, path){
		}


// �v���p�e�B


		/// <summary>
		/// ���̃��X�|���X���L���b�V�����ėǂ��������l��ݒ�E�擾���܂��B
		/// nocacheresponse �̓L���b�V���s�\�ł��B
		/// </summary>
		public override bool IsCacheable {
			get{return false;}
		}


		/// <summary>
		/// �n���ꂽ HttpResponse �Ƀ��X�|���X���������݂܂��B
		/// </summary>
		public override void WriteResponse(HttpResponse response){
			WriteResponseHeader(response);
			response.Write(Html.OuterXml);
			response.Cache.SetCacheability(HttpCacheability.NoCache);
		}


	}

}
