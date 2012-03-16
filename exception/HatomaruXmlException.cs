using System;

namespace Bakera.Hatomaru{

	public class HatomaruXmlException : Exception{
		public HatomaruXmlException() : base(){}
		public HatomaruXmlException(string mes) : base(mes){}
		public HatomaruXmlException(string mes, Exception e) : base(mes, e){}

	} // End class
} // End Namespace


