using System;
using System.Web;
using System.Xml;

namespace Bakera.Hatomaru{

	/// <summary>
	/// エラーを返すレスポンスの抽象クラスです。
	/// </summary>
	public abstract class ErrorResponse : NormalResponse{

		private string myMessage;

		protected ErrorResponse(HatomaruXml source, AbsPath path, int errorCode) : base(source, path){
			myStatusCode = errorCode;
		}
		/// <summary>
		/// エラーメッセージを指定して ErrorResponse を作成します。
		/// </summary>
		protected ErrorResponse(HatomaruXml source, AbsPath path, int errorCode, string message) : this(source, path, errorCode){
			myMessage = message;
		}


// プロパティ

		/// <summary>
		/// エラーメッセージを取得します。
		/// </summary>
		public string Message{
			get{return myMessage;}
		}

		/// <summary>
		/// このレスポンスをキャッシュして良いか示す値を設定・取得します。
		/// nocacheresponse はキャッシュ不可能です。
		/// </summary>
		public override bool IsCacheable {
			get{return false;}
		}


		/// <summary>
		/// 渡された HttpResponse にレスポンスを書き込みます。
		/// このメソッドは何度も使われるため、Html に AppendChild してはいけません。
		/// </summary>
		public override void WriteResponse(HttpResponse response){
			WriteResponseHeader(response);
			SetLastModified(response);
			// HTML にメッセージを投入
			if(Html == null){
				response.Write("<plaintext>");
				response.Write(myMessage);
			} else {
//				XmlElement mes = Html.P(null, myMessage);
//				Html.Append(mes);
				response.Write(Html.OuterXml);
			}


		}


	}

}
