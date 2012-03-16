using System;
using System.IO;
using System.Collections.Generic;
using System.Text;

namespace Bakera.Hatomaru{

	/// <summary>
	/// 403 Forbidden を返すレスポンスです。
	/// </summary>
	public class ForbiddenResponse : ErrorResponse{
		public ForbiddenResponse(HatomaruXml source, AbsPath path) : base(source, path, 403, "Forbidden"){}
		public ForbiddenResponse(HatomaruXml source, AbsPath path, string message) : base(source, path, 403, message){}
	}

}
