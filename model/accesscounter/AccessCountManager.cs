using System;
using System.IO;
using System.Collections.Generic;


namespace Bakera.Hatomaru{


	public class AccessCountManager{

		public DirectoryInfo LogDir{ get; set; }
		public const string LogFileExt = ".txt";

		public Counter GetDateCount(DateTime dt){
			string filename = dt.ToString("yyyyMMdd") + LogFileExt;
			FileInfo[] files = LogDir.GetFiles(filename);
			if(files.Length == 0) return null;
			FileInfo file = files[0];
			Counter c = new Counter();
			c.Load(file);
			return c;
		}


		public Counter GetMonthCount(DateTime dt){
			string filename = dt.ToString("yyyyMM??") + ".txt";
			FileInfo[] files = LogDir.GetFiles(filename);
			if(files.Length == 0) return null;
			Counter c = new Counter();
			foreach(FileInfo file in files){
				c.Load(file);
			}
			return c;
		}



	}

}
