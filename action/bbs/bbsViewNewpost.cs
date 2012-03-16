using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace Bakera.Hatomaru{

/// <summary>
/// ���یf������������N���X�ł��B
/// </summary>
	public partial class BbsViewNewPost : BbsAction{

		public new const string Id = "newpost";
		public new const string Label = "�V�K���e";

// �R���X�g���N�^

		/// <summary>
		/// �V�K���e�t�H�[���\���̂��߂̃A�N�V�����̃C���X�^���X���J�n���܂��B
		/// </summary>
		public BbsViewNewPost(HatomaruBbs model, AbsPath path) : base(model, path){
			myPath = myModel.BasePath.Combine(Id);
		}


		/// <summary>
		/// �f�[�^�� GET ���AHatomaruResponse ���擾���܂��B
		/// </summary>
		protected override HatomaruResponse GetHtmlResponse(){
			HatomaruBbs.SetAllReplaceUrl(Html);
			string title = "�V�K���e�t�H�[��";
			Response.SelfTitle = title;
			Response.AddTopicPath(Path, title);
			InsertHeading(2, title);
			XmlNode result = GetForm();
			Html.Entry.AppendChild(result);
			return Response;
		}




	} // End class
} // End Namespace Bakera



