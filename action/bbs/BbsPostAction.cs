using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Web;
using System.Xml;
using System.Text.RegularExpressions;

namespace Bakera.Hatomaru{

/// <summary>
/// BBS の Post データを処理する HatomaruPostAction の派生クラスです。
/// </summary>
	public class BbsPostAction : HatomaruPostAction{

		public const int NameMaxLength = 20;
		public const int SubjectMaxLength = 100;
		public const int MessageMaxLength = 2000;

		// コメント先の Article
		// 日記等へのコメントの場合、CommentTo が設定された Id 0 の Article
		// 掲示板新規投稿の場合は null
		private Article myTargetArticle;

// コンストラクタ
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


// Postメソッド

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
				title = "コメント投稿の修正";
			} else if(errors.Count > 0){
				result.AppendChild(GetErrors(errors));
				result.AppendChild(GetForm(a, errors));
				title = "コメント投稿エラー";
			} else {
				int score = SpamRule.GetSpamScore(a);
				if(isSubmitMode){
					// 投稿する
					// キーを確認
					string key = Bbs.GetPostKey();
					string hash = GetPostedValue(InputHashName);
					if(key.Equals(hash, StringComparison.InvariantCulture)){
						try{
							a.Id = latest.Id + 1;
							if(score > 100) a.IsSpam = true;
							Bbs.SaveArticle(a);
							result.AppendChild(Html.P(null, "以下のコメントが投稿されました。"));
							result.AppendChild(ParseArticle(a));
							title = "コメント投稿";
						} catch(UnauthorizedAccessException) {
							errors.Add(null, "投稿処理ができませんでした。現在、投稿が禁止されています。");
							result.AppendChild(GetErrors(errors));
							result.AppendChild(GetForm(a, errors));
							title = "コメント投稿エラー";
						} catch (IOException) {
							errors.Add(null, "投稿処理ができませんでした。メンテナンスのため、一時的に投稿できなくなっている可能性があります。しばらく待って再投稿してみてください。");
							result.AppendChild(GetErrors(errors));
							result.AppendChild(GetForm(a, errors));
							title = "コメント投稿エラー";
						}
					} else {
						errors.Add(null, "投稿処理ができませんでした。誰かが同時に投稿しようとしたため、先の投稿の処理中だった可能性があります。少し待ってこのまま再投稿してみてください。");
						result.AppendChild(GetErrors(errors));
						result.AppendChild(GetForm(a, errors));
						title = "コメント投稿の競合エラー";
					}
				} else {
					result.AppendChild(ConfirmMessage(a, score));
					result.AppendChild(GetConfirmForm(a));
					result.AppendChild(ParseArticle(a));
					result.AppendChild(GetConfirmForm(a));
					title = "コメント投稿の確認";
				}
			}

			Response.SelfTitle = title;
			Response.AddTopicPath(Path, title);
			InsertHeading(2, title);
			Html.Append(result);
			return Response;
		}


// Private メソッド

		private XmlNode ConfirmMessage(Article a, int spamScore){
			XmlNode result = Html.CreateDocumentFragment();
			XmlElement postInfoNotice = Html.Div("post-info-notice");
			XmlElement em = Html.Create("em", null, a.IpAddress);
			if(a.SrcCountry != null){
				em.AppendChild(Html.Text(string.Format("({0})", a.SrcCountry)));
			}
			postInfoNotice.AppendChild(Html.P("ipinfo", "注意: 投稿の送信元IPアドレス ", em, " が記録されます。"));
			postInfoNotice.AppendChild(Html.P("note", "※広告・宣伝の書き込みや問題のある投稿が行われた場合、この送信元情報を利用して調査を行うことがあります。"));
			if(spamScore > 100){
				postInfoNotice.AppendChild(Html.P("note", "※この記事は管理者が許可するまで公開されません。"));
			}
			result.AppendChild(postInfoNotice);
			result.AppendChild(Html.P(BbsAction.NavDescClass, "コメントの内容を確認してください。問題なければ「投稿する」を選択してください。"));
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

		// エラーメッセージを ul 要素として返します。
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


		// 投稿された Article にエラーがないかどうかチェックします。
		private PostErrorCollection ArticleCheck(Article a, Article latestArticle){
			PostErrorCollection errors = new PostErrorCollection();
			if(string.IsNullOrEmpty(a.Subject)){
				errors.Add(InputSubjectName, "題名が入力されていません。");
			} else if(a.Subject.Length > SubjectMaxLength){
				errors.Add(InputSubjectName, String.Format("題名は{0}字ありますが、最大{1}字までしか入力できません。あと{2}字減らしてください。", a.Subject.Length, SubjectMaxLength, SubjectMaxLength - a.Subject.Length));
			}

			if(string.IsNullOrEmpty(a.Name)){
				errors.Add(InputSenderName, "投稿者の名前が入力されていません。");
			} else if(a.Name.Length > NameMaxLength){
				errors.Add(InputSenderName, String.Format("投稿者の名前は{0}字ありますが、最大{1}字までしか入力できません。あと{2}字減らしてください。", a.Name.Length, NameMaxLength, NameMaxLength - a.Name.Length));
			}

			if(String.IsNullOrEmpty(a.Message)){
				errors.Add(InputBodyName, "本文が入力されていません。空白のみの投稿は出来ません。");
			} else {
				// 本文を調査 1. 長すぎないか？ 2. 引用のみでないか？
				int messageLine = 0;
				int QorELine = 0; // 引用or空行の数
				Regex crlf = new BakeraReg.CrLf();
				foreach(string s in crlf.Split(a.Message)){
					if(s.StartsWith(">") || s == "") QorELine++;
					messageLine++;
				}
				if(a.Message.Length > MessageMaxLength){
					errors.Add(InputBodyName, String.Format("本文は{0}字ありますが、最大{1}字までしか入力できません。あと{2}字減らしてください。", a.Message.Length, MessageMaxLength, a.Message.Length - MessageMaxLength));
				}
				if(QorELine == messageLine){
					errors.Add(InputBodyName, "本文に引用部と空行しか含まれていません。引用でない本文を記入してください。");
				}
			}

			// コメント先の実在チェック
			if(a.CommentTo != null){
				HatomaruResponse parentResponse = myModel.Manager.GetResponse(a.CommentTo);
				if(parentResponse == null) errors.Add(null, "コメント先のコンテンツが取得できませんでした。コメント先が削除されているか、URLが変更になっている可能性があります。");
			}

			// 二重投稿チェック
			if(latestArticle != null && a.Message == latestArticle.Message){
				errors.Add(null, string.Format("二重投稿です。本文が同一の記事が #{0} として既に投稿されています (同じ文面での連続投稿はできません)。", latestArticle.Id));
			}

			return errors;
		}


		// 投稿元のIPv4アドレスからCCTLDを取得します。CCTLD不明の場合はnullを返します。
		// IPAddrToCCTLD が設定されていない場合もnullを返します。
		// IPv6アドレスが渡された場合もnullを返します。
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
						throw new Exception("IPアドレスが不正です: " + data[0]);
					}
					if(CompareIPAddress(srcAddr, startAddress) < 0){
						continue;
					}
					IPAddress endAddress = null;
					IPAddress.TryParse(data[1], out endAddress);
					if(endAddress == null){
						throw new Exception("IPアドレスが不正です: " + data[1]);
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



