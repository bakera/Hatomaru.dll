using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;


public class Counter : Dictionary<string, int>{
	
	public void CountUp(string s){
		CountUp(s, 1);
	}

	public void CountUp(string s, int count){
		if(string.IsNullOrEmpty(s)) return;
		if(this.ContainsKey(s)){
			this[s] += count;
		} else {
			this.Add(s, count);
		}
	}

	public KeyValuePair<string, int>[] GetSortedList(){
		// �~���Ƀ\�[�g
		List<KeyValuePair<string, int>> list = new List<KeyValuePair<string, int>>(this);
		list.Sort((x, y) => y.Value - x.Value);
		return list.ToArray();
	}

	public KeyValuePair<string, int>[] GetSortedList(int maxlength){
		// �~���Ƀ\�[�g
		List<KeyValuePair<string, int>> list = new List<KeyValuePair<string, int>>(this);
		list.Sort((x, y) => y.Value - x.Value);

		int resultSize = list.Count < maxlength ? list.Count : maxlength;
		KeyValuePair<string, int>[] result = new KeyValuePair<string, int>[resultSize];
		for(int i=0; i < resultSize; i++){
			result[i] = list[i];
		}
		return result;
	}

	public void Save(string filename){
		KeyValuePair<string, int>[] uriList = GetSortedList();
		if(uriList.Length == 0){
			Console.WriteLine("No Data: {0}", filename);
			return;
		}
		
		using(FileStream fs = File.Open(filename, FileMode.Create, FileAccess.Write, FileShare.None))
		using(StreamWriter sw = new StreamWriter(fs)){
			foreach(KeyValuePair<string, int> pair in uriList){
				if(pair.Value <= 1) continue;
				sw.Write(pair.Key);
				sw.Write("\t");
				sw.WriteLine(pair.Value);
			}
		}
		Console.WriteLine("Saved: {0}", filename);
	}


	// �t�@�C�������[�h���ăJ�E���g��ǂݍ��݂܂��B
	// �����̃J�E���g�f�[�^��ێ����Ă���ꍇ�́A�J�E���g�����Z���܂��B
	public void Load(FileInfo file){
		using(FileStream fs = file.Open(FileMode.Open, FileAccess.Read, FileShare.Read))
		using(StreamReader sr = new StreamReader(fs)){
			while (sr.Peek() >= 0){
				string line = sr.ReadLine();
				string[] data = line.Split('\t');
				if(data.Length < 2) continue;
				string name = data[0];
				int count = 0;
				int.TryParse(data[1], out count);
				if(count <= 0) continue;
				this.CountUp(name, count);
			}
		}
	}

	// �ʂ�Counter�̃J�E���g�����Z���܂��B
	public void Merge(Counter c){
		foreach(string s in c.Keys){
			this.CountUp(s, c[s]);
		}
	}

}


