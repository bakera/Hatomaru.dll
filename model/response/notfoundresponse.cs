using System;
using System.IO;
using System.Collections.Generic;
using System.Text;

namespace Bakera.Hatomaru{

	/// <summary>
	/// 404 Not Found を返すレスポンスです。
	/// </summary>
	public class NotFoundResponse : ErrorResponse{
		public NotFoundResponse(HatomaruXml source, AbsPath path) : this(source, path, "Not Found"){}
		public NotFoundResponse(HatomaruXml source, AbsPath path, string message) : base(source, path, 404, message){}
	}

}
