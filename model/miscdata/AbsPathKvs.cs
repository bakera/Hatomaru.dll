using System;
using System.Collections.Generic;
using System.IO;

namespace Bakera.Hatomaru{


	// AbsPath をキーとし string を値に持つ Key value store です。
	// テキストファイルにデータを保存・読み込みする機能を持ちます。
	public class AbsPathKvs{

		private Dictionary<AbsPath, string> myData = new Dictionary<AbsPath, string>();
		private FileInfo myFile;


		public AbsPathKvs(FileInfo file){
			myFile = file;
			Load();
		}


		public string this[AbsPath uri]{
			get{
				string result = null;
				myData.TryGetValue(uri, out result);
				return result;
			}
		}

		public void Add(AbsPath uri, string data){
			string prevdata = this[uri];
			if(prevdata == null){
				myData.Add(uri, data);
				Save();
			} else if(prevdata != data){
				myData[uri] = data;
				Save();
			}
		}

		public void Save(){
			using(FileStream fs = myFile.Open(FileMode.Create, FileAccess.Write, FileShare.None))
			using(StreamWriter sw = new StreamWriter(fs)){
				foreach(KeyValuePair<AbsPath, string> pair in myData){
					sw.Write(pair.Key);
					sw.Write("\t");
					sw.WriteLine(pair.Value);
				}
			}
		}

		public void Load(){
			if(!myFile.Exists) return;
			using(FileStream fs = myFile.Open(FileMode.Open, FileAccess.Read, FileShare.Read))
			using(StreamReader sr = new StreamReader(fs)){
				while (sr.Peek() >= 0){
					string line = sr.ReadLine();
					string[] data = line.Split('\t');
					if(data.Length < 2) continue;
					AbsPath ap = new AbsPath(data[0]);
					myData.Add(ap, data[1]);
				}
			}
		}

	} // End Class ResponseTitleManager

} // End Namespace 
