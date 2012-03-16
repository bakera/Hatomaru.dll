using System;
using System.IO;
using System.Xml;

namespace Bakera.Hatomaru{

/// <summary>
/// ���j���[�� XML ����������N���X�ł��B
/// </summary>
	public class CacheViewer : HatomaruXml{

		new public const string Name = "cacheviewer";

// �R���X�g���N�^

		/// <summary>
		/// ���ۃf�[�^�̃f�[�^�\�[�X�� FileInfo �� XmlDocument ���w�肵�āA�C���X�^���X���J�n���܂��B
		/// </summary>
		public CacheViewer(HatomaruManager manager, FileInfo f, XmlDocument x) : base(manager, f, x){}


// �I�[�o�[���C�h���\�b�h

		/// <summary>
		/// �f�[�^�� GET ���AHatomaruResponse ���擾���܂��B
		/// </summary>
		public override HatomaruResponse Get(AbsPath path){
			CacheViewerAction da = new CacheViewerAction(this, path);
			HatomaruResponse result = da.Get();
			result.SetLastModified();
			return result;
		}


	} // End class
} // End Namespace Bakera







