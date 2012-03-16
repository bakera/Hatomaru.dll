using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace Bakera.Hatomaru{

/// <summary>
/// ���_�C���N�g�̐錾�݂̂�XML���������邽�߂̃N���X�ł��B
/// </summary>
	public class HatomaruRedirect : HatomaruXml{

		new public const string Name = "redirect";
		public const string PathAttrName = "path";

// �R���X�g���N�^

		/// <summary>
		/// ���ۃf�[�^�̃f�[�^�\�[�X�� FileInfo �� XmlDocument ���w�肵�āARedirect �̃C���X�^���X���J�n���܂��B
		/// </summary>
		public HatomaruRedirect(HatomaruManager manager, FileInfo f, XmlDocument x) : base(manager, f, x){
		}

// �I�[�o�[���C�h���\�b�h

		/// <summary>
		/// �f�[�^�� GET ���AHatomaruResponse ���擾���܂��B
		/// </summary>
		public override HatomaruResponse Get(AbsPath path){
			XmlElement e = this.Document.DocumentElement;
			string targetPath = e.GetAttributeValue(PathAttrName);
			if(string.IsNullOrEmpty(targetPath)){
				throw new Exception(string.Format("{0}�v�f��{1}�������Ȃ����A�l����ł��B", Name, PathAttrName));
			}
			AbsPath redPath = new AbsPath(targetPath);
			
			return new RedirectResponse(redPath, Manager.IniData.Domain);

		}



	} // End class Doc
} // End Namespace Bakera







