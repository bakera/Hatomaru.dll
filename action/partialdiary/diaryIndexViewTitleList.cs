using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace Bakera.Hatomaru{

/// <summary>
/// �ŋ߂̓��L��\������A�N�V�����ł��B
/// </summary>
	public partial class DiaryIndexViewTitleList : PartialDiaryAction{

		public const string Id = "titlelist";
		public const string Label = "���o���ꗗ";


// �R���X�g���N�^

		/// <summary>
		/// ���L�̌��o���ꗗ�\���̂��߂̃A�N�V�����̃C���X�^���X���J�n���܂��B
		/// </summary>
		public DiaryIndexViewTitleList(DiaryIndex model, AbsPath path) : base(model, path){
			myPath = myModel.BasePath.Combine(Id);
		}


		/// <summary>
		/// �f�[�^�� GET ���AHatomaruResponse ���擾���܂��B
		/// </summary>
		protected override HatomaruResponse GetHtmlResponse(){
			Topic[] topics = GetAllTopics();
			if(topics.Length == 0) return NotFound();
			Response.SelfTitle = Label;
			Response.AddTopicPath(Path, Label);
			InsertHeading(2, Label);
			Html.Append(DiaryHeadingList(topics, 3));
			return Response;
		}


	} // End class
} // End Namespace Bakera



