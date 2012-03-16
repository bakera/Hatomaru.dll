using System;
using System.Data;
using System.Xml;
using System.IO;
using System.Collections.Generic;

namespace Bakera.Hatomaru{

	/// <summary>
	/// ���ۂ��������̓ǂ݂̃f�[�^���i�[���� DataTable �̔h���N���X�ł��B
	/// </summary>
	public class GlossaryReadTable : HatomaruTable{

		// �񖼏̂̒萔
		public const string CharColName = "char";
		public const string ReadColName = "read";
		public const string GlossaryColName = "glossary";

		// ��
		private DataColumn myCharCol;
		private DataColumn myReadCol;
		private DataColumn myGlossaryCol;


// �R���X�g���N�^
		public GlossaryReadTable() : base(){
			InitColumns();
		}


// �v���p�e�B

		public DataColumn CharCol{get{return myCharCol;}}
		public DataColumn ReadCol{get{return myReadCol;}}
		public DataColumn GlossaryCol{get{return myGlossaryCol;}}

// �f�[�^�̎擾



// �f�[�^�̃��[�h


		// �ǂ݂�ǉ�
		// �擪�����������Œ������قȂ�ǂ݂�����ꍇ�A�����������̗p����
		// �擪�����������ǂ݂����o�^����邱�Ƃ͂Ȃ��B
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



// ������

		private void InitColumns(){
			// Column ��ݒ肵�܂��B
			myCharCol = new DataColumn(CharColName, typeof(char));
			this.Columns.Add(myCharCol);
			myReadCol = new DataColumn(ReadColName, typeof(string));
			this.Columns.Add(myReadCol);
			myGlossaryCol = new DataColumn(GlossaryColName, typeof(GlossaryWord));
			this.Columns.Add(myGlossaryCol);
		}



	} // class GlossaryTable
} // namespace


