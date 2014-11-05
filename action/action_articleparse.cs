using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Collections.Generic;

namespace Bakera.Hatomaru{

	/// <summary>
	/// �f�[�^���p�[�X���� Xhtml �� NormalResponse �𐶐�����N���X�ł��B
	/// �p�[�X���Ȃ���A�g�p�����f�[�^�\�[�X�� NormalResponse �ɒǉ����čs���܂��B
	/// </summary>
	public abstract partial class HatomaruActionBase{

		public const string SpamBody = "(���̋L���͏��F����Ă��Ȃ����߁A�Ǘ��҂�������܂Ō��J����܂���B)";
		public const string SpamClass = "unauthorized";


		/// <summary>
		/// Article ���p�[�X���܂��B
		/// </summary>
		public XmlNode ParseArticle(Article a){
			XmlElement subjectElement = Html.H(3, "subject");

			// �L���ԍ� 0 (�ԍ�����) �̏ꍇ�̓����N���Ȃ�
			if(a.Id == 0){
				subjectElement.InnerText = a.Subject;
			} else {
				subjectElement.AppendChild(MakeArticleAnchor(a));
			}

			XmlElement messageHeaderChildElement = Html.P(null, NameAndDate(a));
			XmlElement messageHeaderElement = Html.Create("div", "message-header", messageHeaderChildElement);
			XmlElement messageBodyElement = GetArticleBody(a);


			XmlElement result = Html.Create("div", "message", subjectElement, messageHeaderElement, messageBodyElement);
			return result;
		}

		// Bbs �̋L���Ƀ����N����A���J�[���쐬���܂��B
		protected XmlNode MakeArticleAnchor(Article a){
			HatomaruBbs bbs = GetBbs();
			if(bbs == null) throw new Exception("BBS �ȊO�̃C���X�^���X���� Article �n���\�b�h���Ăяo����܂����B");
			Uri linkUri = bbs.BasePath.Combine(BbsViewArticle.Id, a.Id);
			XmlElement result = Html.A(linkUri);
			result.InnerText = a.Title;
//			result.InnerText += "(" + SpamRule.GetSpamScore(a).ToString() + ")";
			return result;
		}


		// �L���̖̂��O�Ɠ��t���擾���܂��B
		protected XmlNode NameAndDate(Article a){
			XmlNode result = Html.CreateDocumentFragment();
			XmlElement nameElement = Html.Create("cite", "from", a.Name);
			XmlElement dateElement = Html.Create("span", "date", "(" + a.DateToString(Model.Manager.IniData.TimeZone) + ")");
			result.AppendChild(nameElement);
			result.AppendChild(Html.Space);
			result.AppendChild(dateElement);
			return result;
		}


		// �L���{�����擾���܂��B
		private XmlElement GetArticleBody(Article a){
			XmlElement messageBodyElement = Html.Create("div", "message-body");

			if(a.IsSpam){
				XmlNode spamMessage = Html.P(SpamClass, SpamBody);
				messageBodyElement.AppendChild(spamMessage);
				return messageBodyElement;
			}

			string[] messageFragments = a.Message.Split(new string[]{"\n\n"}, StringSplitOptions.RemoveEmptyEntries);
			for(int i=0; i < messageFragments.Length; i++){
				XmlElement sectionElement = Html.Create("div", "section");
				string[] subFragments = messageFragments[i].Split(new Char[]{'\n'}, StringSplitOptions.RemoveEmptyEntries);
				for(int j=0; j < subFragments.Length; j++){
					string s = subFragments[j];
					if(s.StartsWith(">")){
						XmlElement q = Html.Create("q", null, UrlRef(s));
						XmlElement p = Html.P(null, q);
						sectionElement.AppendChild(p);
					} else {
						XmlElement p = Html.P(null, UrlRef(s));
						sectionElement.AppendChild(p);
					}
				}
				messageBodyElement.AppendChild(sectionElement);
			}
			return messageBodyElement;
		}


		/// <summary>
		/// �R�����g��ւ̃����N���擾���܂��B
		/// </summary>
		protected XmlElement GetCommentToLink(HatomaruResponse hr){
			XmlElement commentToA = Html.A(hr.Path, "comment-to", hr.FullTitle);
			return commentToA;
		}
		protected XmlElement GetCommentToLink(AbsPath uri){
			string title = Model.Manager.GetResponseTitle(uri);
			XmlElement commentToA = Html.A(uri, "comment-to", title);
			return commentToA;
		}



		/// <summary>
		/// �R�����g�ł��邱�Ƃ�������镶�����܂�p�v�f���擾���܂��B
		/// </summary>
		protected XmlElement GetCommentToDesc(BbsThread bt){
			XmlElement a = GetCommentToLink(bt.CommentTo);
			XmlElement p = Html.P(BbsAction.NavDescClass, "����́u", a, "�v�Ɋ֘A����R�����g�ł��B");
			return p;
		}


		private HatomaruBbs GetBbs(){
			if(myModel is HatomaruBbs) return myModel as HatomaruBbs;
			return myModel.Manager.Bbs;
		}





	}

} // end namespace Bakea

