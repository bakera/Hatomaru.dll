using System;
using System.IO;
using System.Collections.Generic;
using System.Text;

namespace Bakera.Hatomaru{

	/// <summary>
	/// 415 Unsupported Media Type ��Ԃ����X�|���X�ł��B
	/// </summary>
	public class UnsupportedMediaTypeResponse : ErrorResponse{
		public UnsupportedMediaTypeResponse(HatomaruXml source, AbsPath path) : base(source, path, 415, "Unsupported Media Type"){}
	}

}
