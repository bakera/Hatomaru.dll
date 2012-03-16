using System;
using System.Data;
using System.Xml;
using System.IO;
using System.Collections.Generic;

namespace Bakera.Hatomaru{

	/// <summary>
	/// 鳩丸ぐろっさりの読みのデータを格納する DataTable の派生クラスです。
	/// </summary>
	public class GlossaryReadTable : HatomaruTable{

		// 列名称の定数
		public const string CharColName = "char";
		public const string ReadColName = "read";
		public const string GlossaryColName = "glossary";

		// 列
		private DataColumn myCharCol;
		private DataColumn myReadCol;
		private DataColumn myGlossaryCol;


// コンストラクタ
		public GlossaryReadTable() : base(){
			InitColumns();
		}


// プロパティ

		public DataColumn CharCol{get{return myCharCol;}}
		public DataColumn ReadCol{get{return myReadCol;}}
		public DataColumn GlossaryCol{get{return myGlossaryCol;}}

// データの取得



// データのロード


		// 読みを追加
		// 先頭文字が同じで長さが異なる読みがある場合、長い方だけ採用する
		// 先頭文字が同じ読みが二回登録されることはない。
		public void AddReads(GlossaryWord gw){
			List<string> reads = new List<string>();
			AddReadList(reads, gw.Name);
			AddReadList(reads, gw.Read);
			AddReadList(reads, gw.AltRead);
			AddReadList(reads, gw.Pronounce);
			foreach(string s in reads){
				AddRead(s, gw);
			}
		}

		private void AddReadList(List<string> read, string s){
			if(string.IsNullOrEmpty(s)) return;
			s = s.HiraganaToKatakana();
			for(int i=0; i < read.Count; i++){
				string tempStr = read[i];
				if(s[0] == tempStr[0]){
					if(s.Length > tempStr.Length) read[i] = s;
					return;
				}
			}
			read.Add(s);
		}

		private void AddRead(string read, GlossaryWord gw){
			if(string.IsNullOrEmpty(read)) return;
			char key = read[0].ToReadChar();
			if('\x4E00' <= key && key < '\xAC00') return;
			if(HatomaruGlossary.ReadOrder.IndexOf(key) < 0) key = HatomaruGlossary.ReadNull;
			Object[] data = new Object[]{key, read, gw};
			DataRow row = this.NewRow();
			row.ItemArray = data;
			this.Rows.Add(row);
		}



// 初期化

		private void InitColumns(){
			// Column を設定します。
			myCharCol = new DataColumn(CharColName, typeof(char));
			this.Columns.Add(myCharCol);
			myReadCol = new DataColumn(ReadColName, typeof(string));
			this.Columns.Add(myReadCol);
			myGlossaryCol = new DataColumn(GlossaryColName, typeof(GlossaryWord));
			this.Columns.Add(myGlossaryCol);
		}



	} // class GlossaryTable
} // namespace


