using System;
using System.Web;
using System.Xml;

namespace Bakera.Hatomaru{

	/// <summary>
	/// �G���[��Ԃ����X�|���X�̒��ۃN���X�ł��B
	/// </summary>
	public abstract class ErrorResponse : NormalResponse{

		private string myMessage;

		protected ErrorResponse(HatomaruXml source, AbsPath path, int errorCode) : base(source, path){
			myStatusCode = errorCode;
		}
		/// <summary>
		/// �G���[���b�Z�[�W���w�肵�� ErrorResponse ���쐬���܂��B
		/// </summary>
		protected ErrorResponse(HatomaruXml source, AbsPath path, int errorCode, string message) : this(source, path, errorCode){
			myMessage = message;
		}


// �v���p�e�B

		/// <summary>
		/// �G���[���b�Z�[�W���擾���܂��B
		/// </summary>
		public string Message{
			get{return myMessage;}
		}

		/// <summary>
		/// ���̃��X�|���X���L���b�V�����ėǂ��������l��ݒ�E�擾���܂��B
		/// nocacheresponse �̓L���b�V���s�\�ł��B
		/// </summary>
		public override bool IsCacheable {
			get{return false;}
		}


		/// <summary>
		/// �n���ꂽ HttpResponse �Ƀ��X�|���X���������݂܂��B
		/// ���̃��\�b�h�͉��x���g���邽�߁AHtml �� AppendChild ���Ă͂����܂���B
		/// </summary>
		public override void WriteResponse(HttpResponse response){
			WriteResponseHeader(response);
			SetLastModified(response);
			// HTML �Ƀ��b�Z�[�W�𓊓�
			if(Html == null){
				response.Write("<plaintext>");
				response.Write(myMessage);
			} else {
//				XmlElement mes = Html.P(null, myMessage);
//				Html.Append(mes);
				response.Write(Html.OuterXml);
			}


		}


	}

}
