using System;
using System.IO;
using System.Collections.Generic;
using System.Text;

namespace Bakera.Hatomaru{

	/// <summary>
	/// 405 Not Allowed ��Ԃ����X�|���X�ł��B
	/// </summary>
	public class NotAllowedResponse : ErrorResponse{

		private string myAllow;

		/// <summary>
		/// Allow �̒l���w�肵�āANotAllowedResponse �̃C���X�^���X���J�n���܂��B
		/// </summary>
		public NotAllowedResponse(string allow) : this(allow, "Method Not Allowed"){}
		/// <summary>
		/// Allow �̒l�ƃ��b�Z�[�W���w�肵�āANotAllowedResponse �̃C���X�^���X���J�n���܂��B
		/// </summary>
		public NotAllowedResponse(string allow, string message) : base(null, null, 405, message){
			myAllow = allow;
		}

		/// <summary>
		/// Allow ��ݒ�E�擾���܂��B
		/// </summary>
		public string Allow{
			get{return myAllow;}
		}

		/// <summary>
		/// ���̃��X�|���X���L���b�V�����ėǂ��������l��ݒ�E�擾���܂��B
		/// NotAllowedResponse �̓L���b�V���s�ł��B
		/// </summary>
		public override bool IsCacheable {
			get{return false;}
		}



	}

}
