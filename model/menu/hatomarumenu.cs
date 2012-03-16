using System;
using System.IO;
using System.Xml;

namespace Bakera.Hatomaru{

/// <summary>
/// ���j���[�� XML ����������N���X�ł��B
/// </summary>
	public class HatomaruMenu : HatomaruXml{

		new public const string Name = "menu";
		public const string MenuItem = "menuitem";

// �R���X�g���N�^

		/// <summary>
		/// ���ۃf�[�^�̃f�[�^�\�[�X�� FileInfo �� XmlDocument ���w�肵�āAMenu �̃C���X�^���X���J�n���܂��B
		/// </summary>
		public HatomaruMenu(HatomaruManager manager, FileInfo f, XmlDocument x) : base(manager, f, x){}


// �I�[�o�[���C�h���\�b�h

		/// <summary>
		/// �f�[�^�� GET ���AHatomaruResponse ���擾���܂��B
		/// </summary>
		public override HatomaruResponse Get(AbsPath path){
			HatomaruGetAction ga = new MenuAction(this, path);
			HatomaruResponse result = ga.Get();
			result.SetLastModified();
			return result;
		}



	} // End class Doc
} // End Namespace Bakera







