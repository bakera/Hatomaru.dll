using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace Bakera.Hatomaru{

/// <summary>
/// �p��̈ꗗ��\������A�N�V�����ł��B
/// </summary>
	public partial class GlossaryViewGenre : GlossaryAction{

		public const string Id = "genre";
		public new const string Label = "�W�������ꗗ";
		private string myGenreName;

// �R���X�g���N�^

		/// <summary>
		/// �W�������\���̂��߂̃A�N�V�����̃C���X�^���X���J�n���܂��B
		/// </summary>
		public GlossaryViewGenre(HatomaruGlossary model, AbsPath path, string name) : base(model, path){
			myPath = BasePath.Combine(Id, name);
			myGenreName = name;
		}


		/// <summary>
		/// �f�[�^�� GET ���AHatomaruResponse ���擾���܂��B
		/// </summary>
		protected override HatomaruResponse GetHtmlResponse(){
			GlossaryGenre gg = Glossary.GetGenre(myGenreName.PathDecode());
			if(gg == null){
				// ����݊������_�C���N�g
				string newName = myGenreName.Base16ToString();
				gg = Glossary.GetGenre(newName);
				if(gg != null){
					AbsPath newPath = BasePath.Combine(Id, newName.PathEncode());
					return Redirect(newPath);
				}
				return NotFound();
			}
			GlossaryWord[] gws = gg.Glossarys;
			if(gws.Length == 0) return NotFound();
			string title = string.Format("�u{0}�v�Ɋւ���p��", gg.Name);
			Response.SelfTitle = title;
			Response.AddTopicPath(BasePath.Combine(Id), Label);
			Response.AddTopicPath(Path, title);
			InsertHeading(2, title);
			XmlElement p = WordList(gws);
			Html.Append(p);
			return Response;
		}

	} // End class
} // End Namespace Bakera



