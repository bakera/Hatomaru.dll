using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;

namespace Bakera.Hatomaru{

/// <summary>
/// Action のベースクラスです。
/// </summary>
	public abstract partial class HatomaruActionBase{

		protected HatomaruResponse myResponse;
		protected HatomaruXml myModel;
		protected AbsPath myPath;
		protected AbsPath myUserPath;

		protected XmlNode myPageNav;

		private LinkItem myPrev;
		private LinkItem myNext;


		private XmlElement myChildrenNavPoint = null; // 横ナビのステイ位置を覚えておく

		public const string TopicPathPlaceHolderName = "TOPICPATH";
		public const string TopicPathClassName = "topic-path";
		public const string TopicPathSeparator = " > ";

		public const string RemovePath = "hatomaru.aspx";

		public const string NavName = "CONTENTS-NAVI";
		public const string RecentlyTopicsName = "RECENTLY-TOPICS";
		public const string NavHeadingName = "MENUHEADING";
		public const string NavItemName = "MENU";

		public const string CommentName = "コメント";
		public const string CommentPath = "comment";

		public static Regex UrlReg = new BakeraReg.UrlRefRegex();
		public static Regex EmailReg = new BakeraReg.MailBoxRegex();
		public static Regex ParenUrlReg = new BakeraReg.ParenUrlRefRegex();
		public static Regex NotMbsReg = new BakeraReg.NotMBS();


// コンストラクタ

		protected HatomaruActionBase(HatomaruXml model, AbsPath path){
			myModel = model;
			myUserPath = path;
			myPath = path;
			myResponse = new NormalResponse(model, myPath);
			GetHtml();
		}


// プロパティ

		/// <summary>
		/// ユーザがリクエストした Path を取得します。
		/// </summary>
		public AbsPath UserPath{
			get{return myUserPath;}
		}

		/// <summary>
		/// 内部で認識された「正しい」Path を取得します。
		/// </summary>
		public AbsPath Path{
			get{return myPath;}
		}

		/// <summary>
		/// Model を取得します。
		/// </summary>
		protected virtual HatomaruXml Model{
			get{return myModel;}
		}

		/// <summary>
		/// BasePath を取得します。
		/// </summary>
		protected AbsPath BasePath{
			get{return myModel.BasePath;}
		}

		/// <summary>
		/// Xhtml を取得します。
		/// </summary>
		protected Xhtml Html{
			get{return Response.Html;}
		}

		/// <summary>
		/// Response を設定・取得します。
		/// </summary>
		protected HatomaruResponse Response{
			get{return myResponse;}
		}

		/// <summary>
		/// Prev の LinkItem を設定・取得します。
		/// </summary>
		protected LinkItem Prev{
			get{return myPrev;}
			set{myPrev = value;}
		}

		/// <summary>
		/// Next の LinkItem を設定・取得します。
		/// </summary>
		protected LinkItem Next{
			get{return myNext;}
			set{myNext = value;}
		}


		/// <summary>
		/// BBS を取得します。
		/// </summary>
		protected HatomaruBbs Bbs{
			get{
				if(myModel is HatomaruBbs) return myModel as HatomaruBbs;
				return myModel.Manager.Bbs;
			}
		}

		/// <summary>
		/// Glossary を取得します。
		/// </summary>
		protected virtual HatomaruGlossary Glossary{
			get{
				if(myModel is HatomaruGlossary) return myModel as HatomaruGlossary;
				return myModel.Manager.Glossary;
			}
		}

		/// <summary>
		/// HtmlRef を取得します。
		/// </summary>
		protected virtual HatomaruHtmlRef HtmlRef{
			get{
				if(myModel is HatomaruHtmlRef) return myModel as HatomaruHtmlRef;
				return myModel.Manager.HtmlRef;
			}
		}

		/// <summary>
		/// SpamRule を取得します。
		/// </summary>
		protected SpamRule SpamRule{
			get{
				return myModel.Manager.SpamRule;
			}
		}




// プロテクトメソッド


		/// <summary>
		/// リダイレクト応答を返します。
		/// </summary>
		protected HatomaruResponse Redirect(AbsPath path){
			return new RedirectResponse(path, myModel.Manager.IniData.Domain);
		}

		/// <summary>
		/// 404応答を返します。
		/// </summary>
		protected HatomaruResponse NotFound(){
			return NotFound("Not Found");
		}
		protected HatomaruResponse NotFound(string mes){
			myResponse = new NotFoundResponse(Model, Path, mes);
			GetHtml();
			XmlElement p = Html.P(null, mes);
			Html.Append(p);
			return myResponse;
		}





// プロテクトメソッド

		/// <summary>
		/// テキスト内に含まれる HTTP URL を a要素によるハイパーリンクに変換した DocumentFragment を返します。
		/// </summary>
		protected XmlNode UrlRef(string text){
			XmlNode result = Html.CreateDocumentFragment();
			string test = text;
			for(;;){
				// 括弧付き
				Match mp = ParenUrlReg.Match(test);
				if(mp.Success){
					string beforeURL = test.Substring(0, mp.Index);
					string afterURL = test.Substring(mp.Index + mp.Length);
					test = afterURL;
					string innerParen = mp.Value.Substring(1, mp.Value.Length - 2);
					Uri link = new Uri(innerParen);
					XmlElement a = Html.A(link, null, innerParen);
					a.SetAttribute("rel", "nofollow");
					result.AppendChild(Html.Text(beforeURL));
					result.AppendChild(Html.Text("("));
					result.AppendChild(a);
					result.AppendChild(Html.Text(")"));
					continue;
				}

				Match m = UrlReg.Match(test);
				if(m.Success){
					string beforeURL = test.Substring(0, m.Index);
					string afterURL = test.Substring(m.Index + m.Length);
					test = afterURL;
					Uri link = new Uri(m.Value);
					XmlElement a = Html.A(link, null, m.Value);
					a.SetAttribute("rel", "nofollow");
					result.AppendChild(Html.Text(beforeURL));
					result.AppendChild(a);
					continue;
				}
				result.AppendChild(Html.Text(test));
				break;
			}
			return result;
		}


		/// <summary>
		/// Action へのリンクを取得します。
		/// </summary>
		protected LinkItem GetActionLink(Type t){
			string id = Util.GetFieldValue(t, "Id");
			string label = Util.GetFieldValue(t, "Label");
			if(label == null) return null;

			AbsPath linkpath = BasePath.Combine(id);
			return new LinkItem(linkpath, label);
		}


		/// <summary>
		/// コメントへのリンクを取得します。
		/// </summary>
		protected XmlNode CommentLink(AbsPath path, string title){

			XmlNode result = Html.Create("ul", "comment-link");
			XmlElement resultLi = Html.Create("li");

			Response.AddDataSource(Bbs);
			BbsThread bt = Bbs.GetCommentToThread(path);
			AbsPath commentPath = path.Combine(CommentPath);

			XmlElement commentA = Html.A(commentPath);
			if(bt == null || bt.Articles.Length == 0){
				commentA.InnerText = string.Format("「{0}」に{1}を書く", title, CommentName);
			} else {
				commentA.InnerText = string.Format("「{0}」への{1} ({2}件)", title, CommentName, bt.Count);
			}
			resultLi.AppendChild(commentA);
			result.AppendChild(resultLi);
			return result;
		}


		// myResponse.Html に XHTML の雛形をセットします。
		protected void GetHtml(){
			myResponse.Html = new Xhtml();
			Html.BaseUri = myPath.GetAbsUri(Model.Manager.IniData.Domain);
			Html.AppendChild(ParseNode(Model.Manager.Template));
			foreach(LinkItem li in Model.Styles){
				Html.AddStyleLink(li);
			}
			XmlNodeList lists = Html.Body.GetElementsByTagName("div");
			foreach(XmlNode x in lists){
				XmlElement e = x as XmlElement;
				string idVal = e.GetAttributeValue("id");
				if(idVal == HtmlMainContentsId){
					Html.Entry = e;
					break;
				}
			}
		}

// 後方互換機能

		/// <summary>
		/// 文字列を UTF-16 の16進表記に変換します。
		/// </summary>
		public static string StringToHex(string s){
			if(s == null) return null;
			string encoded = "";
			foreach(Byte b in Encoding.Unicode.GetBytes(s)){
				encoded += String.Format("{0:x2}", b);
			}
			return encoded;
		}

		/// <summary>
		/// UTF-16 の16進表記を文字列に変換します。
		/// </summary>
		public static string HexToString(string s){
			if(s == null) return null;
			byte[] bytes = new byte[s.Length];
			for(int i=0; i< s.Length; i += 2){
				string temp = new string(s[i],s[i+1]);
				bytes[i] = Convert.ToByte(temp, 16);
			}
			return Encoding.Unicode.GetString(bytes);
		}


	} // End class
} // End Namespace



