using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Xml;

namespace Bakera.Hatomaru{

	/// <summary>
	/// 301 Permanently Moved ��Ԃ����X�|���X�ł��B
	/// </summary>
	public class RedirectResponse : HatomaruResponse{

		private string myDomain;
		private AbsPath myDestPath;

		/// <summary>
		/// ���_�C���N�g��ƂȂ�p�X�ƃh���C�����w�肵�āARedirectResponse���쐬���܂��B
		/// </summary>
		public RedirectResponse(AbsPath destPath, string domain) : base(){
			myStatusCode = 301;
			myDestPath = destPath;
			myDomain = domain;
		}

		/// <summary>
		/// ���_�C���N�g��ƂȂ�p�X�ƃh���C���A���_�C���N�g���̃p�X���w�肵�āARedirectResponse���쐬���܂��B
		/// </summary>
		public RedirectResponse(AbsPath destPath, string domain, AbsPath srcPath) : this(destPath, domain){
			Path = srcPath;
		}


// �v���p�e�B

		/// <summary>
		/// �h���C�����擾���܂��B
		/// </summary>
		public string Domain{
			get{return myDomain;}
		}

		/// <summary>
		/// ���̃��X�|���X���L���b�V�����ėǂ��������l��ݒ�E�擾���܂��B
		/// RedirectResponse �̓L���b�V���\�ł��B
		/// </summary>
		public override bool IsCacheable {
			get{return true;}
		}

		/// <summary>
		/// ���_�C���N�g��̃p�X��Ԃ��܂��B
		/// </summary>
		public AbsPath DestPath{
			get{return myDestPath;}
			set{myDestPath = value;}
		}



// ���\�b�h

		/// <summary>
		/// �n���ꂽ HttpResponse �Ƀ��X�|���X���������݂܂��B
		/// </summary>
		public override void WriteResponse(HttpResponse response){
			Uri redUri = DestPath.GetAbsUri(myDomain);
			WriteResponseHeader(response);

			response.RedirectLocation = redUri.AbsoluteUri;

			Xhtml result = GetXhtml();
			result.Title.InnerText = response.StatusDescription;

			XmlElement a = result.A(redUri, null, redUri);
			XmlElement p = result.P(null, "URL ���������Ȃ����A�R���e���c���ړ����Ă���悤�ł��B���� URL ���Q�Ƃ��Ă������� : ", a);
			result.Body.AppendChild(p);
			response.Write(result.OuterXml);

		}

	}

}
