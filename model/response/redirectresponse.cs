using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Xml;

namespace Bakera.Hatomaru{

	/// <summary>
	/// 301 Permanently Moved を返すレスポンスです。
	/// </summary>
	public class RedirectResponse : HatomaruResponse{

		private string myDomain;
		private AbsPath myDestPath;

		/// <summary>
		/// リダイレクト先となるパスとドメインを指定して、RedirectResponseを作成します。
		/// </summary>
		public RedirectResponse(AbsPath destPath, string domain) : base(){
			myStatusCode = 301;
			myDestPath = destPath;
			myDomain = domain;
		}

		/// <summary>
		/// リダイレクト先となるパスとドメイン、リダイレクト元のパスを指定して、RedirectResponseを作成します。
		/// </summary>
		public RedirectResponse(AbsPath destPath, string domain, AbsPath srcPath) : this(destPath, domain){
			Path = srcPath;
		}


// プロパティ

		/// <summary>
		/// ドメインを取得します。
		/// </summary>
		public string Domain{
			get{return myDomain;}
		}

		/// <summary>
		/// このレスポンスをキャッシュして良いか示す値を設定・取得します。
		/// RedirectResponse はキャッシュ可能です。
		/// </summary>
		public override bool IsCacheable {
			get{return true;}
		}

		/// <summary>
		/// リダイレクト先のパスを返します。
		/// </summary>
		public AbsPath DestPath{
			get{return myDestPath;}
			set{myDestPath = value;}
		}



// メソッド

		/// <summary>
		/// 渡された HttpResponse にレスポンスを書き込みます。
		/// </summary>
		public override void WriteResponse(HttpResponse response){
			Uri redUri = DestPath.GetAbsUri(myDomain);
			WriteResponseHeader(response);

			response.RedirectLocation = redUri.AbsoluteUri;

			Xhtml result = GetXhtml();
			result.Title.InnerText = response.StatusDescription;

			XmlElement a = result.A(redUri, null, redUri);
			XmlElement p = result.P(null, "URL が正しくないか、コンテンツが移動しているようです。次の URL を参照してください : ", a);
			result.Body.AppendChild(p);
			response.Write(result.OuterXml);

		}

	}

}
