using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Web;

namespace Bakera.Hatomaru{

	/// <summary>
	/// キャッシュされないレスポンスを生成します。
	/// text/html もしくは application/xhtml+xml の 200 OK を返すレスポンスです。
	/// </summary>
	public class NoCacheResponse : NormalResponse{

		/// <summary>
		/// データソースを指定して、NoCacheResponse のインスタンスを開始します。
		/// </summary>
		public NoCacheResponse(HatomaruXml source, AbsPath path) : base(source, path){
		}


// プロパティ


		/// <summary>
		/// このレスポンスをキャッシュして良いか示す値を設定・取得します。
		/// nocacheresponse はキャッシュ不可能です。
		/// </summary>
		public override bool IsCacheable {
			get{return false;}
		}


		/// <summary>
		/// 渡された HttpResponse にレスポンスを書き込みます。
		/// </summary>
		public override void WriteResponse(HttpResponse response){
			WriteResponseHeader(response);
			response.Write(Html.OuterXml);
			response.Cache.SetCacheability(HttpCacheability.NoCache);
		}


	}

}
