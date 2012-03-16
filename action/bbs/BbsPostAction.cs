using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Web;
using System.Xml;
using System.Text.RegularExpressions;

namespace Bakera.Hatomaru{

/// <summary>
/// BBS �� Post �f�[�^���������� HatomaruPostAction �̔h���N���X�ł��B
/// </summary>
	public class BbsPostAction : HatomaruPostAction{

		public const int NameMaxLength = 20;
		public const int SubjectMaxLength = 100;
		public const int MessageMaxLength = 2000;

		// �R�����g��� Article
		// ���L���ւ̃R�����g�̏ꍇ�ACommentTo ���ݒ肳�ꂽ Id 0 �� Article
		// �f���V�K���e�̏ꍇ�� null
		private Article myTargetArticle;

// �R���X�g���N�^
		public BbsPostAction(HatomaruXml model, AbsPath path, HttpRequest req) : base(model, path, req){
			string[] fragments = path.GetFragments(BasePath);
			if(model is HatomaruBbs){
				if(fragments.Length > 1 && fragments[0].Equals(BbsViewArticle.Id)){
					int articleId = fragments[1].ToInt32();
					myTargetArticle = Bbs.GetArticle(articleId);
				}
			} else {
				myTargetArticle = new Article();
				myTargetArticle.CommentTo = path.RemoveLast(CommentPath);
			}
			myResponse = new NoCacheResponse(Model, path);
			GetHtml();
		}


// Post���\�b�h

		protected override HatomaruResponse PostAndGetHtmlResponse(){
			Article latest = Bbs.GetLatestArticle();
			Article a = MakeArticle();
			bool isEditMode = !string.IsNullOrEmpty(GetPostedValue(InputEditName));
			bool isSubmitMode = !string.IsNullOrEmpty(GetPostedValue(InputSubmitName));
			PostErrorCollection errors = ArticleCheck(a, latest);
			string title = "";
			XmlNode result = Html.CreateDocumentFragment();
			if(isEditMode){
				result.AppendChild(GetForm(a));
				title = "�R�����g���e�̏C��";
			} else if(errors.Count > 0){
				result.AppendChild(GetErrors(errors));
				result.AppendChild(GetForm(a, errors));
				title = "�R�����g���e�G���[";
			} else {
				int score = SpamRule.GetSpamScore(a);
				if(isSubmitMode){
					// ���e����
					// �L�[���m�F
					string key = Bbs.GetPostKey();
					string hash = GetPostedValue(InputHashName);
					if(key.Equals(hash, StringComparison.InvariantCulture)){
						try{
							a.Id = latest.Id + 1;
							if(score > 100) a.IsSpam = true;
							Bbs.SaveArticle(a);
							result.AppendChild(Html.P(null, "�ȉ��̃R�����g�����e����܂����B"));
							result.AppendChild(ParseArticle(a));
							title = "�R�����g���e";
						} catch(UnauthorizedAccessException) {
							errors.Add(null, "���e�������ł��܂���ł����B���݁A���e���֎~����Ă��܂��B");
							result.AppendChild(GetErrors(errors));
							result.AppendChild(GetForm(a, errors));
							title = "�R�����g���e�G���[";
						} catch (IOException) {
							errors.Add(null, "���e�������ł��܂���ł����B�����e�i���X�̂��߁A�ꎞ�I�ɓ��e�ł��Ȃ��Ȃ��Ă���\��������܂��B���΂炭�҂��čē��e���Ă݂Ă��������B");
							result.AppendChild(GetErrors(errors));
							result.AppendChild(GetForm(a, errors));
							title = "�R�����g���e�G���[";
						}
					} else {
						errors.Add(null, "���e�������ł��܂���ł����B�N���������ɓ��e���悤�Ƃ������߁A��̓��e�̏������������\��������܂��B�����҂��Ă��̂܂܍ē��e���Ă݂Ă��������B");
						result.AppendChild(GetErrors(errors));
						result.AppendChild(GetForm(a, errors));
						title = "�R�����g���e�̋����G���[";
					}
				} else {
					result.AppendChild(ConfirmMessage(a, score));
					result.AppendChild(GetConfirmForm(a));
					result.AppendChild(ParseArticle(a));
					result.AppendChild(GetConfirmForm(a));
					title = "�R�����g���e�̊m�F";
				}
			}

			Response.SelfTitle = title;
			Response.AddTopicPath(Path, title);
			InsertHeading(2, title);
			Html.Append(result);
			return Response;
		}


// Private ���\�b�h

		private XmlNode ConfirmMessage(Article a, int spamScore){
			XmlNode result = Html.CreateDocumentFragment();
			XmlElement postInfoNotice = Html.Div("post-info-notice");
			XmlElement em = Html.Create("em", null, a.IpAddress);
			if(a.SrcCountry != null){
				em.AppendChild(Html.Text(string.Format("({0})", a.SrcCountry)));
			}
			postInfoNotice.AppendChild(Html.P("ipinfo", "����: ���e�̑��M��IP�A�h���X ", em, " ���L�^����܂��B"));
			postInfoNotice.AppendChild(Html.P("note", "���L���E��`�̏������݂���̂��铊�e���s��ꂽ�ꍇ�A���̑��M�����𗘗p���Ē������s�����Ƃ�����܂��B"));
			if(spamScore > 100){
				postInfoNotice.AppendChild(Html.P("note", "�����̋L���͊Ǘ��҂�������܂Ō��J����܂���B"));
			}
			result.AppendChild(postInfoNotice);
			result.AppendChild(Html.P(BbsAction.NavDescClass, "�R�����g�̓��e���m�F���Ă��������B���Ȃ���΁u���e����v��I�����Ă��������B"));
			return result;
		}


		private Article MakeArticle(){
			Article a = new Article();
			a.Date = DateTime.Now;
			a.Subject = GetPostedValue(InputSubjectName);
			a.Name = GetPostedValue(InputSenderName);
			a.Message = GetPostedValue(InputBodyName);
			a.IpAddress = Request.UserHostAddress;
			a.SrcCountry = IPAddrToCCTLD(a.IpAddress);
			if(myTargetArticle != null){
				a.Parent = myTargetArticle.Id;
				a.CommentTo = myTargetArticle.CommentTo;
			}
			return a;
		}

		// �G���[���b�Z�[�W�� ul �v�f�Ƃ��ĕԂ��܂��B
		private XmlNode GetErrors(PostErrorCollection errors){
			if(errors.Count == 0) return Html.CreateDocumentFragment();
			XmlNode result = Html.Create("ul");
			foreach(PostError e in errors){
				if(string.IsNullOrEmpty(e.Name)){
					result.AppendChild(Html.Create("li", null, e.Message));
				} else {
					XmlElement label = Html.Create("label");
					label.InnerText = e.Message;
					label.SetAttribute("for", e.Name);
					result.AppendChild(Html.Create("li", null, label));
				}
			}
			return result;
		}


		// ���e���ꂽ Article �ɃG���[���Ȃ����ǂ����`�F�b�N���܂��B
		private PostErrorCollection ArticleCheck(Article a, Article latestArticle){
			PostErrorCollection errors = new PostErrorCollection();
			if(string.IsNullOrEmpty(a.Subject)){
				errors.Add(InputSubjectName, "�薼�����͂���Ă��܂���B");
			} else if(a.Subject.Length > SubjectMaxLength){
				errors.Add(InputSubjectName, String.Format("�薼��{0}������܂����A�ő�{1}���܂ł������͂ł��܂���B����{2}�����炵�Ă��������B", a.Subject.Length, SubjectMaxLength, SubjectMaxLength - a.Subject.Length));
			}

			if(string.IsNullOrEmpty(a.Name)){
				errors.Add(InputSenderName, "���e�҂̖��O�����͂���Ă��܂���B");
			} else if(a.Name.Length > NameMaxLength){
				errors.Add(InputSenderName, String.Format("���e�҂̖��O��{0}������܂����A�ő�{1}���܂ł������͂ł��܂���B����{2}�����炵�Ă��������B", a.Name.Length, NameMaxLength, NameMaxLength - a.Name.Length));
			}

			if(String.IsNullOrEmpty(a.Message)){
				errors.Add(InputBodyName, "�{�������͂���Ă��܂���B�󔒂݂̂̓��e�͏o���܂���B");
			} else {
				// �{���𒲍� 1. �������Ȃ����H 2. ���p�݂̂łȂ����H
				int messageLine = 0;
				int QorELine = 0; // ���por��s�̐�
				Regex crlf = new BakeraReg.CrLf();
				foreach(string s in crlf.Split(a.Message)){
					if(s.StartsWith(">") || s == "") QorELine++;
					messageLine++;
				}
				if(a.Message.Length > MessageMaxLength){
					errors.Add(InputBodyName, String.Format("�{����{0}������܂����A�ő�{1}���܂ł������͂ł��܂���B����{2}�����炵�Ă��������B", a.Message.Length, MessageMaxLength, a.Message.Length - MessageMaxLength));
				}
				if(QorELine == messageLine){
					errors.Add(InputBodyName, "�{���Ɉ��p���Ƌ�s�����܂܂�Ă��܂���B���p�łȂ��{�����L�����Ă��������B");
				}
			}

			// �R�����g��̎��݃`�F�b�N
			if(a.CommentTo != null){
				HatomaruResponse parentResponse = myModel.Manager.GetResponse(a.CommentTo);
				if(parentResponse == null) errors.Add(null, "�R�����g��̃R���e���c���擾�ł��܂���ł����B�R�����g�悪�폜����Ă��邩�AURL���ύX�ɂȂ��Ă���\��������܂��B");
			}

			// ��d���e�`�F�b�N
			if(latestArticle != null && a.Message == latestArticle.Message){
				errors.Add(null, string.Format("��d���e�ł��B�{��������̋L���� #{0} �Ƃ��Ċ��ɓ��e����Ă��܂� (�������ʂł̘A�����e�͂ł��܂���)�B", latestArticle.Id));
			}

			return errors;
		}


		// ���e����IPv4�A�h���X����CCTLD���擾���܂��BCCTLD�s���̏ꍇ��null��Ԃ��܂��B
		// IPAddrToCCTLD ���ݒ肳��Ă��Ȃ��ꍇ��null��Ԃ��܂��B
		// IPv6�A�h���X���n���ꂽ�ꍇ��null��Ԃ��܂��B
		private string IPAddrToCCTLD(string srcAddr){
			IPAddress addr = null;
			IPAddress.TryParse(srcAddr, out addr);
			if(addr == null) return null;
			return IPAddrToCCTLD(addr);
		}
		private string IPAddrToCCTLD(IPAddress srcAddr){
			if(srcAddr == null) return null;
			if(srcAddr.AddressFamily != AddressFamily.InterNetwork) return null;
			FileInfo f = Model.Manager.IniData.IPAddrToCCTLDFile;
			if(f == null) return  null;

			using(FileStream fs = f.Open(FileMode.Open, FileAccess.Read, FileShare.Read))
			using(StreamReader sr = new StreamReader(fs)){
				while (sr.Peek() >= 0){
					string line = sr.ReadLine();
					string[] data = line.Split('\t');
					if(data.Length < 2) continue;
					IPAddress startAddress = null;
					IPAddress.TryParse(data[0], out startAddress);
					if(startAddress == null){
						throw new Exception("IP�A�h���X���s���ł�: " + data[0]);
					}
					if(CompareIPAddress(srcAddr, startAddress) < 0){
						continue;
					}
					IPAddress endAddress = null;
					IPAddress.TryParse(data[1], out endAddress);
					if(endAddress == null){
						throw new Exception("IP�A�h���X���s���ł�: " + data[1]);
					}
					if(CompareIPAddress(srcAddr, endAddress) > 0){
						continue;
					}
					return data[2];
				}
			}
			return null;
		}

		private static int CompareIPAddress(IPAddress src, IPAddress dest){
			byte[] srcBytes = src.GetAddressBytes();
			byte[] destBytes = dest.GetAddressBytes();
			for(int i = 0; i < srcBytes.Length; i++){
				if(srcBytes[i] != destBytes[i]){
					return srcBytes[i].CompareTo(destBytes[i]);
				}
			}
			return 0;
		}
 

	} // End class
} // End Namespace



