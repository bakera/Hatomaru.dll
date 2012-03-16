using System;
using System.Xml;

namespace Bakera.Hatomaru{
	
	/// <summary>
	/// 一つのトピックを表すクラスです。
	/// </summary>
	public class Topic{
		private const string GenreSeparator = ",";

		public const string DiaryElementName = "diary";
		public const string TopicElementName = "topic";
		public const string IdAttr = "num";
		public const string NameAttr = "name";
		public const string DateAttr = "date";
		public const string UpdatedAttr = "updated";
		public const string CreatedAttr = "created";
		public const string GenreAttr = "genre";


		protected int myId;
		protected DateTime myDate;
		protected DateTime myCreated;
		protected DateTime myUpdated;
		protected string myTitle;
		protected XmlElement myMessage;
		protected string[] myGenre;



/* ======== プロパティ ======== */

		/// <summary>
		/// 記事の ID 番号を取得します。
		/// </summary>
		public int Id{
			get {return myId;}
		}
		/// <summary>
		/// トピックの日付を取得します。
		/// </summary>
		public DateTime Date{
			get{return myDate;}
		}
		/// <summary>
		/// トピックが作成された日付を取得します。
		/// </summary>
		public DateTime Created{
			get{return myCreated;}
		}
		/// <summary>
		/// トピックが更新された日付を取得します。
		/// </summary>
		public DateTime Updated{
			get{return myUpdated;}
		}
		/// <summary>
		/// トピックの見出しを取得します。
		/// </summary>
		public string Title{
			get{return myTitle;}
		}
		/// <summary>
		/// トピックのメッセージを取得します。
		/// </summary>
		public XmlElement Message{
			get{return myMessage;}
		}
		/// <summary>
		/// トピックのジャンルを取得します。
		/// </summary>
		public string[] Genre{
			get{return myGenre;}
		}


/* ======== コンストラクタ ======== */

		public Topic(){}
		/// <summary>
		/// XmlNode を指定して、Topic クラスのインスタンスを開始します。
		/// </summary>
		public Topic(XmlNode topicElement){
			myMessage = (XmlElement)topicElement;
			Load(myMessage);
		}


/* ======== メソッド ======== */


		/// <summary>
		/// XML Element からデータをロードします。
		/// </summary>
		public void Load(XmlElement message){
			myDate = message.ParentNode.GetAttributeDateTime(DateAttr);
			myId = message.GetAttributeInt(IdAttr);
			myTitle = message.GetAttributeValue(NameAttr);
			myGenre = message.GetAttributeValues(GenreAttr);
			if(myGenre.Length == 0) myGenre = new string[]{"ノンジャンル"};
			myUpdated = message.GetAttributeDateTime(UpdatedAttr, myDate);
			myCreated = message.GetAttributeDateTime(CreatedAttr, myDate);
		}

		/// <summary>
		/// オーバーライド。Topic を文字列に変換します。
		/// </summary>
		public override string ToString(){
			return "No." + this.Id.ToString() + ": " + this.Title.ToString() + this.Date.ToString(" (yyyy-MM-dd)");
		}


		public static int CompareByUpdated(Topic x, Topic y){
			return y.Updated.CompareTo(x.Updated);
		}


	} // public class Topic

} // namespace Bakera




