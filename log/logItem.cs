using System;
using System.Globalization;
using System.Collections.Generic;
using System.IO;


namespace Bakera.Hatomaru{

	public class LogItem{
		
		private DateTime myTime;
		private string myData = null;

		public LogItem(string s){
			myTime = DateTime.Now;
			myData = s;
		}

		public string Data{
			get{return myData;}
			set{myData = value;}
		}

		public DateTime Time{
			get{return myTime;}
			set{myTime = value;}
		}
	}


}

