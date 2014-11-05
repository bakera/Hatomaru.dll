using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Collections.Generic;

namespace Bakera.Hatomaru{

	/// <summary>
	/// データをパースして Xhtml と NormalResponse を生成するクラスです。
	/// パースしながら、使用したデータソースを NormalResponse に追加して行きます。
	/// </summary>
	public abstract partial class HatomaruActionBase{

		public const string SpamBody = "(この記事は承認されていないため、管理者が許可するまで公開されません。)";
		public const string SpamClass = "unauthorized";


		/// <summary>
		/// Article をパースします。
		/// </summary>
		public XmlNode ParseArticle(Article a){
			XmlElement subjectElement = Html.H(3, "subject");

			// 記事番号 0 (番号未定) の場合はリンクしない
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

		// Bbs の記事にリンクするアンカーを作成します。
		protected XmlNode MakeArticleAnchor(Article a){
			HatomaruBbs bbs = GetBbs();
			if(bbs == null) throw new Exception("BBS 以外のインスタンスから Article 系メソッドが呼び出されました。");
			Uri linkUri = bbs.BasePath.Combine(BbsViewArticle.Id, a.Id);
			XmlElement result = Html.A(linkUri);
			result.InnerText = a.Title;
//			result.InnerText += "(" + SpamRule.GetSpamScore(a).ToString() + ")";
			return result;
		}


		// 記事のの名前と日付を取得します。
		protected XmlNode NameAndDate(Article a){
			XmlNode result = Html.CreateDocumentFragment();
			XmlElement nameElement = Html.Create("cite", "from", a.Name);
			XmlElement dateElement = Html.Create("span", "date", "(" + a.DateToString(Model.Manager.IniData.TimeZone) + ")");
			result.AppendChild(nameElement);
			result.AppendChild(Html.Space);
			result.AppendChild(dateElement);
			return result;
		}


		// 記事本文を取得します。
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
		/// コメント先へのリンクを取得します。
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
		/// コメントであることを説明する文言を含むp要素を取得します。
		/// </summary>
		protected XmlElement GetCommentToDesc(BbsThread bt){
			XmlElement a = GetCommentToLink(bt.CommentTo);
			XmlElement p = Html.P(BbsAction.NavDescClass, "これは「", a, "」に関連するコメントです。");
			return p;
		}


		private HatomaruBbs GetBbs(){
			if(myModel is HatomaruBbs) return myModel as HatomaruBbs;
			return myModel.Manager.Bbs;
		}





	}

} // end namespace Bakea

