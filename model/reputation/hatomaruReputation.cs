using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Reflection;
using System.Xml;

namespace Bakera.Hatomaru{

/// <summary>
/// �l�C�R���e���c����������N���X�ł��B
/// </summary>
	public partial class HatomaruReputation : HatomaruXml{

		public new const string Name = "reputation";
		public const string ContentElementName = "content";
		public const string UriAttributeName = "uri";
		public const string CountAttributeName = "count";
		public const string DescriptionElementName = "desc";

		/// <summary>
		/// �f�[�^�\�[�X�� FileInfo �� XmlDocument ���w�肵�āAReputation �̃C���X�^���X���J�n���܂��B
		/// </summary>
		public HatomaruReputation(HatomaruManager manager, FileInfo f, XmlDocument x) : base(manager, f, x){
			Table = new ReputationTable(this);
		}

// �v���p�e�B

		public ReputationTable Table{
			get; private set;
		}


// �I�[�o�[���C�h���\�b�h

		/// <summary>
		/// �f�[�^�� GET ���AHatomaruResponse ���擾���܂��B
		/// </summary>
		public override HatomaruResponse Get(AbsPath path){
			HatomaruGetAction act = GetAction(path);
			HatomaruResponse result = act.Get();
			result.SetLastModified();
			return result;
		}

// ���\�b�h

		/// <summary>
		/// path �����ɁA�K�؂ȃR���g���[�����쐬���܂��B
		/// </summary>
		private HatomaruGetAction GetAction(AbsPath path){
			string[] fragments = path.GetFragments(BasePath);
			return new ReputationView(this, path);
		}

// �f�[�^�擾���\�b�h

		// Select ��������w�肵�� Topic ���擾���܂��B
		public ReputationContent[] GetAllReputation(){
			return null;
		}



	} // End class
} // End Namespace Bakera



