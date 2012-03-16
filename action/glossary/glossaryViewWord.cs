using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace Bakera.Hatomaru{

/// <summary>
/// �p��̈ꗗ��\������A�N�V�����ł��B
/// </summary>
	public partial class GlossaryViewWord : GlossaryAction{

		private string myWordId;

// �R���X�g���N�^

		/// <summary>
		/// �ŋ߂̓��L�\���̂��߂̃A�N�V�����̃C���X�^���X���J�n���܂��B
		/// </summary>
		public GlossaryViewWord(HatomaruGlossary model, AbsPath path, string id) : base(model, path){
			myWordId = id;
			myPath = myModel.BasePath.Combine(id);
		}


		/// <summary>
		/// �f�[�^�� GET ���AHatomaruResponse ���擾���܂��B
		/// </summary>
		protected override HatomaruResponse GetHtmlResponse(){
			GlossaryWord gw = Glossary.GetWordById(myWordId);
			// �݊����Ή�
			if(gw == null){
				string base16decoded = myWordId.Base16ToString();
				GlossaryWord newWord = Glossary.GetWordById(base16decoded);
				if(newWord != null){
					// �݊��A�N�Z�X
					AbsPath newPath = BasePath.Combine(newWord.Name.PathEncode());
					return Redirect(newPath);
				}
				return NotFound();
			}

			Response.SelfTitle = gw.Name;
			AbsPath currentPath = BasePath.Combine(gw.Name.PathEncode());
			Response.AddTopicPath(currentPath, gw.Name);

			XmlElement titleHeading = Html.H(2, null, string.Format("�p��u{0}�v�ɂ���", gw.Name));
			Html.Append(titleHeading);

			for(int i=0; i < gw.Descs.Length; i++){
				GlossaryDesc gd = gw.Descs[i];
				XmlElement descHeading = Html.H(3, null, gw.Name);
				if(gw.Descs.Length > 1){
					string descNum = string.Format("({0})", i+1);
					descHeading.AppendChild(Html.Space);
					descHeading.AppendChild(Html.Span("num", descNum));
				}
				if(!string.IsNullOrEmpty(gw.Read)){
					descHeading.AppendChild(Html.Space);
					descHeading.AppendChild(Html.Span("read", "(" + gw.Read + ")"));
				} else  if(!string.IsNullOrEmpty(gw.Pronounce)){
					descHeading.AppendChild(Html.Space);
					descHeading.AppendChild(Html.Span("pronounce", "(" + gw.Pronounce + ")"));
				}
				Html.Append(descHeading);
				Html.Append(GenreLink(gd));
				Html.Append(ParseNode(gd.Description, 4));
			}

			XmlNodeList rels = gw.Element.GetElementsByTagName(HatomaruGlossary.RelateElementName);
			if(rels.Count > 0){
				string title = string.Format("�u{0}�v�Ɋ֘A����p��", gw.Name);
				Html.Append(Html.H(4, null, title));
				XmlElement ul = Html.Create("ul");
				foreach(XmlElement x in rels){
					GlossaryWord relGw = Glossary.GetWordByName(x.InnerText);
					if(relGw == null){
						ul.AppendChild(Html.Create("li", null, x.InnerText));
					} else {
						AbsPath relA = myModel.BasePath.Combine(relGw.Name.PathEncode());
						XmlElement a = Html.A(relA);
						a.InnerText = relGw.Name;
						ul.AppendChild(Html.Create("li", null, a));
					}
				}
				Html.Append(ul);
			}

			XmlNodeList refs = gw.Element.GetElementsByTagName(HatomaruGlossary.SourceElementName);
			if(refs.Count > 0){
				string title = string.Format("�u{0}�v�Ɋ֘A���� Web �T�C�g", gw.Name);
				Html.Append(Html.H(4, null, title));
				XmlElement ul = Html.Create("ul");
				foreach(XmlElement x in refs){
					ul.AppendChild(Html.Create("li", null, Html.GetA(x)));
				}
				Html.Append(ul);
			}
			Html.Append(CommentLink(currentPath, gw.Name));
			return Response;
		}

		/// <summary>
		/// �W�������ւ̃����N���擾���܂��B
		/// </summary>
		private XmlNode GenreLink(GlossaryDesc gd){
			XmlNode result = Html.P("subinfo");
			if(gd.Genre == null || gd.Genre.Length == 0) return Html.Null;
			XmlElement p = Html.Span("genre");
			p.AppendChild(Html.Text("�b�� : "));
			for(int i = 0; i < gd.Genre.Length; i++){
				if(i > 0) p.AppendChild(Html.Text(" / "));

				AbsPath linkPath = BasePath.Combine(GlossaryViewGenre.Id, gd.Genre[i].PathEncode());
				XmlElement gLink = Html.A(linkPath);
				gLink.InnerText = gd.Genre[i];
				p.AppendChild(gLink);
			}
			result.AppendChild(p);
			return result;
		}


	} // End class
} // End Namespace Bakera



