using System;
using System.IO;
using System.Xml;


namespace Bakera.Hatomaru{

/// <summary>
/// ���یf������������N���X�ł��B
/// </summary>
	public partial class HatomaruAccessCounter : HatomaruXml{

		new public const string Name = "accesscounter";


		/// <summary>
		/// ���ۃf�[�^�̃f�[�^�\�[�X�� FileInfo �� XmlDocument ���w�肵�āAHatomaruAccessCounter �̃C���X�^���X���J�n���܂��B
		/// </summary>
		public HatomaruAccessCounter(HatomaruManager manager, FileInfo f, XmlDocument x) : base(manager, f, x){}

		/// <summary>
		/// �f�[�^�� GET ���AHatomaruResponse ���擾���܂��B
		/// </summary>
		public override HatomaruResponse Get(AbsPath path){
/*
			HatomaruGetAction act = GetAction(path);
			HatomaruResponse result = act.Get();
			result.SetLastModified();
			return result;
*/
			return null;
		}

	}

}

