using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Xml;

namespace Bakera.Hatomaru{

	/// <summary>
	/// XML の 200 OK を返すレスポンスです。
	/// </summary>
	public class XmlResponse : HatomaruResponse{

		private XmlDocument myDocument;

		public const string XmlMediaType = "application/xml";


		/// <summary>
		/// データソースを指定して、HatomaruResponse のインスタンスを開始します。
		/// 通常はこのコンストラクタを使用します。
		/// </summary>
		public XmlResponse(HatomaruData source) : base(source){
			ContentType = XmlMediaType;
			myDocument = new XmlDocument();
			myDocument.XmlResolver = null;
		}


// プロパティ

		/// <summary>
		/// チェック時間を取得します。
		/// </summary>
		public XmlDocument Document{
			get{return myDocument;}
		}


		/// <summary>
		/// このレスポンスをキャッシュして良いか示す値を設定・取得します。
		/// xmlresponse はキャッシュ可能です。
		/// </summary>
		public override bool IsCacheable {
			get{return true;}
		}


// public メソッド


		/// <summary>
		/// 渡された HttpResponse にレスポンスを書き込みます。
		/// </summary>
		public override void WriteResponse(HttpResponse response){
			WriteResponseHeader(response);
			SetLastModified(response);
			response.Write(Document.OuterXml);
		}


// プライベートメソッド
	}

}
