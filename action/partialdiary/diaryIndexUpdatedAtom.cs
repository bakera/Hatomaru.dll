using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace Bakera.Hatomaru{

/// <summary>
/// �ŋ߂̓��L��\������A�N�V�����ł��B
/// </summary>
	public partial class DiaryIndexUpdatedAtom : DiaryIndexAtom{

		public new const string Id = "atom";
		public const string Label = "�X�V�E�ǋL���ꂽ���ѓ��L";

// �R���X�g���N�^

		/// <summary>
		/// ���L�̌��o���ꗗ�\���̂��߂̃A�N�V�����̃C���X�^���X���J�n���܂��B
		/// </summary>
		public DiaryIndexUpdatedAtom(DiaryIndex model, AbsPath path) : base(model, path){
			myPath = myModel.BasePath.Combine(DiaryIndexViewUpdated.Id, Id);
		}


		/// <summary>
		/// �f�[�^�� GET ���AHatomaruResponse ���擾���܂��B
		/// </summary>
		protected override HatomaruResponse GetHtmlResponse(){
			Topic[] topics = GetUpdatedTopics();
			if(topics.Length == 0) return NotFound();
			return GetAtom(topics, Label);
		}


	} // End class
} // End Namespace Bakera



