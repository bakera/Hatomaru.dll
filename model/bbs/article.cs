using System;
using System.Xml;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Data;

namespace Bakera.Hatomaru{

	/// <summary>
	/// 鳩丸掲示板の一つの記事を扱うクラスです。
	/// </summary>
	public class Article : IComparable<Article>{

		public const string SpamTitle = "未承認メッセージ (投稿元:{0})";

		private const string MessageElement = "message";
		private const string IdAttr = "num";
		private const string ParentAttr = "parent";
		private const string SubjectAttr = "subject";
		private const string NameAttr = "name";
		private const string DateAttr = "date";
		private const string IpAddressAttr = "ip";
		private const string UrlRefAttr = "urlref";
		private const string SpamAttr = "spam";
		private const string CommentToAttr = "commentto";
		private const string TargetTitleAttr = "target";
		private const string SrcCountryAttr = "cctld";

		public const string CommentPrefix = "Re:";

		private int myId;
		private int myParent;           // 親記事番号
		private string mySubject;      // Subject
		private string myName;         // myName 投稿者名
		private string myMessage;  // myMessage 内容。行ごとに LF 区切りとする
		private DateTime myDate;

		private string myIpAddress;    // myIpAddress リモートアドレス
		private bool myIsSpam;     // URI を削除するなら true
		private AbsPath myCommentTo;     // コメント先パス
		private BbsThread myThread;         // この記事が属するスレッド

		private List<Article> myChildList = new List<Article>();         // この記事の子たち
		private Article[] myChildren;         // この記事の子たち


// コンストラクタ

		/// <summary>
		/// Article の新しいインスタンスを開始します。
		/// </summary>
		public Article(){}

		/// <summary>
		/// message 要素を指定して、Article の新しいインスタンスを開始します。
		/// </summary>
		public Article(XmlElement messageElement){
			this.Load(messageElement);
		}



/* ======== プロパティ ======== */

		/// <summary>
		/// この記事の ID を設定・取得します。
		/// </summary>
		public int Id{
			get {return myId;}
			set {myId = value;}
		}

		/// <summary>
		/// この記事の親記事の ID 番号を設定・取得します。
		/// </summary>
		public int Parent{
			get {return myParent;}
			set {myParent = value;}
		}

		/// <summary>
		/// 記事の Subject を設定・取得します。
		/// </summary>
		public string Subject{
			get {return mySubject;}
			set {mySubject = value;}
		}

		/// <summary>
		/// 記事の Title を取得します。ID と Subject からなります。
		/// Spam の場合は Spam 用のタイトルを返します。
		/// </summary>
		public string Title{
			get {
				if(Id == 0) return Subject;
				string subjectStr = Subject;
				if(this.IsSpam) subjectStr = string.Format(SpamTitle, this.IpAddress);
				return string.Format("[{0}] {1}", Id, subjectStr);
			}
		}

		/// <summary>
		/// 記事の投稿日時を設定・取得します。
		/// </summary>
		public DateTime Date{
			get {return myDate;}
			set {myDate = value;}
		}

		/// <summary>
		/// 記事の投稿者名を設定・取得します。
		/// </summary>
		public string Name{
			get {return myName;}
			set {myName = value;}
		}

		/// <summary>
		/// 記事の投稿に使用されたリモートホストの IP アドレスを設定・取得します。
		/// </summary>
		public string IpAddress{
			get {return myIpAddress;}
			set {myIpAddress = value;}
		}

		/// <summary>
		/// 記事の本文を設定・取得します。
		/// </summary>
		public string Message{
			get {return myMessage;}
			set {myMessage = value;}
		}


		/// <summary>
		/// spam判定されている場合 true を返します。
		/// </summary>
		public bool IsSpam{
			get {return myIsSpam;}
			set {myIsSpam = value;}
		}

		/// <summary>
		/// 投稿元の国を返します。
		/// </summary>
		public string SrcCountry{
			get; set;
		}

		/// <summary>
		/// コメント先記事のパスを設定・取得します。
		/// </summary>
		public AbsPath CommentTo{
			get {return myCommentTo;}
			set {myCommentTo = value;}
		}

		/// <summary>
		/// この記事の属するスレッド番号を設定・取得します。
		/// </summary>
		public BbsThread Thread{
			get {return myThread;}
			set {myThread = value;}
		}

		/// <summary>
		/// この記事の子たちを設定・取得します。
		/// </summary>
		public Article[] Children{
			get {
				if(myChildren != null) return myChildren;
				myChildren = new Article[myChildList.Count];
				myChildList.CopyTo(myChildren);
				return myChildren;
			}
		}


		/// <summary>
		/// 記事の ID と Subject からなる題名を取得します。
		/// </summary>
		public override string ToString(){
			return string.Format("[{0}] {1}", Id, Subject);
		}




// パブリックメソッド

		public string DateToString(){
			return Date.ToString("yyyy年M月d日 H時m分");
		}

		/// <summary>
		/// XML message 要素から記事のデータをロードします。
		/// </summary>
		public void Load(XmlElement messageElement){
			// 各属性値をプロパティに入れる
			Id = messageElement.GetAttributeInt(IdAttr);
			Parent = messageElement.GetAttributeInt(ParentAttr);
			Subject = messageElement.GetAttributeValue(SubjectAttr);
			Name = messageElement.GetAttributeValue(NameAttr);
			IpAddress = messageElement.GetAttributeValue(IpAddressAttr);
			IsSpam = messageElement.GetAttributeBool(SpamAttr);
			Date = messageElement.GetAttributeDateTime(DateAttr);
			CommentTo = messageElement.GetAttributePath(CommentToAttr);
			SrcCountry = messageElement.GetAttributeValue(SrcCountryAttr);

			// 要素の内容が Message プロパティの値
			XmlNodeList pList = messageElement.GetElementsByTagName("p");
			if(pList.Count > 0){
				StringBuilder mesString = new StringBuilder();
				foreach(XmlNode n in messageElement.ChildNodes){
					XmlElement e = n as XmlElement;
					if (e == null) continue;
					if(e.Name == "p") mesString.Append(n.InnerText);
					mesString.Append("\n");
				}
				myMessage = mesString.ToString().TrimEnd();
			} else {
				myMessage = messageElement.InnerText;
			}
		}

		/// <summary>
		/// 記事のデータを XML message 要素として出力します。
		/// </summary>
		public XmlElement ToXmlElement(XmlDocument owner){
			XmlElement result = owner.CreateElement(MessageElement);
			if(Id <= 0) throw new Exception("IDがありません。");
			result.SetAttribute(IdAttr, Id.ToString());
			if(Parent > 0) result.SetAttribute(ParentAttr, Parent.ToString());
			result.SetAttribute(SubjectAttr, Subject.ToString());
			result.SetAttribute(NameAttr, Name.ToString());
			if(IpAddress == null) throw new Exception("IPアドレスがありません。");
			result.SetAttribute(IpAddressAttr, IpAddress.ToString());
			if(IsSpam) result.SetAttribute(SpamAttr, SpamAttr);
			result.SetAttribute(DateAttr, Date.ToString());
			if(SrcCountry != null) result.SetAttribute(SrcCountryAttr, SrcCountry);
			if(CommentTo != null) result.SetAttribute(CommentToAttr, CommentTo.ToString());
			result.InnerText = Message;
			return result;
		}

		/// <summary>
		/// この Article にコメントする際のフォームの要素となる Article を取得します。
		/// </summary>
		public Article GetCommentArticle(){
			Article result = new Article();
			result.Parent = this.Id;
			if(this.Subject.StartsWith(CommentPrefix)){
				result.Subject = this.Subject;
			} else {
				result.Subject = CommentPrefix + this.Subject;
			}
			if(!String.IsNullOrEmpty(this.Message)){
				result.Message = ">" + String.Join("\n>", this.Message.Split('\n')) + '\n';
 			}
			return result;
		}


		/// <summary>
		/// 子の Article を追加します。
		/// </summary>
		public void AddChild(Article child){
			myChildList.Add(child);
		}

// IComparable インターフェイスの実装

		/// <summary>
		/// ArticleBase を日付で比較します。
		/// </summary>
		public int CompareTo(Article a){
			return myDate.CompareTo(a.Date);
		}


	}// End Class Article
	
}// End Namespace hatomruBBS





