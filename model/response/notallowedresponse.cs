using System;
using System.IO;
using System.Collections.Generic;
using System.Text;

namespace Bakera.Hatomaru{

	/// <summary>
	/// 405 Not Allowed を返すレスポンスです。
	/// </summary>
	public class NotAllowedResponse : ErrorResponse{

		private string myAllow;

		/// <summary>
		/// Allow の値を指定して、NotAllowedResponse のインスタンスを開始します。
		/// </summary>
		public NotAllowedResponse(string allow) : this(allow, "Method Not Allowed"){}
		/// <summary>
		/// Allow の値とメッセージを指定して、NotAllowedResponse のインスタンスを開始します。
		/// </summary>
		public NotAllowedResponse(string allow, string message) : base(null, null, 405, message){
			myAllow = allow;
		}

		/// <summary>
		/// Allow を設定・取得します。
		/// </summary>
		public string Allow{
			get{return myAllow;}
		}

		/// <summary>
		/// このレスポンスをキャッシュして良いか示す値を設定・取得します。
		/// NotAllowedResponse はキャッシュ不可です。
		/// </summary>
		public override bool IsCacheable {
			get{return false;}
		}



	}

}
