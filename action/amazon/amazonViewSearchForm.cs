using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace Bakera.Hatomaru{

/// <summary>
/// �f�[�^�`���̈ꗗ��\������A�N�V�����ł��B
/// </summary>
	public partial class AmazonViewSearchForm : AmazonAction{

		public new const string Label = "�A�}��";
		public const string FormElementName = "form";

// �R���X�g���N�^

		/// <summary>
		/// �A�}���̃t�H�[���\���̂��߂̃A�N�V�����̃C���X�^���X���J�n���܂��B
		/// </summary>
		public AmazonViewSearchForm(AmazonSearch model, AbsPath path) : base(model, path){
			myPath = myModel.BasePath;
		}

// �v���p�e�B

		/// <summary>
		/// �f�[�^�� GET ���AHatomaruResponse ���擾���܂��B
		/// </summary>
		protected override HatomaruResponse GetHtmlResponse(){

			InsertHeading(2, Label);
			XmlElement desc = Model.Document.DocumentElement[AmazonSearch.TopicName];
			if(desc != null) Html.Append(ParseNode(desc.ChildNodes, 3));
			Html.Append(GetSearchForm(null, AmazonIndexType.None));

			return Response;
		}



	} // End class
} // End Namespace Bakera



