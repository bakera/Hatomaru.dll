using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Xml;

namespace Bakera.Hatomaru{

	/// <summary>
	/// XML �� 200 OK ��Ԃ����X�|���X�ł��B
	/// </summary>
	public class XmlResponse : HatomaruResponse{

		private XmlDocument myDocument;

		public const string XmlMediaType = "application/xml";


		/// <summary>
		/// �f�[�^�\�[�X���w�肵�āAHatomaruResponse �̃C���X�^���X���J�n���܂��B
		/// �ʏ�͂��̃R���X�g���N�^���g�p���܂��B
		/// </summary>
		public XmlResponse(HatomaruData source) : base(source){
			ContentType = XmlMediaType;
			myDocument = new XmlDocument();
			myDocument.XmlResolver = null;
		}


// �v���p�e�B

		/// <summary>
		/// �`�F�b�N���Ԃ��擾���܂��B
		/// </summary>
		public XmlDocument Document{
			get{return myDocument;}
		}


		/// <summary>
		/// ���̃��X�|���X���L���b�V�����ėǂ��������l��ݒ�E�擾���܂��B
		/// xmlresponse �̓L���b�V���\�ł��B
		/// </summary>
		public override bool IsCacheable {
			get{return true;}
		}


// public ���\�b�h


		/// <summary>
		/// �n���ꂽ HttpResponse �Ƀ��X�|���X���������݂܂��B
		/// </summary>
		public override void WriteResponse(HttpResponse response){
			WriteResponseHeader(response);
			SetLastModified(response);
			response.Write(Document.OuterXml);
		}


// �v���C�x�[�g���\�b�h
	}

}
