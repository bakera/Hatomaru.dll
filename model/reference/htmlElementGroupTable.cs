using System;
using System.Data;
using System.Xml;
using System.IO;
using System.Collections.Generic;

namespace Bakera.Hatomaru{

	/// <summary>
	/// HTML���t�@�����X�̗v�f�O���[�v�̃f�[�^���i�[���� DataTable �̔h���N���X�ł��B
	/// </summary>
	public class HtmlElementGroupTable : HatomaruTable{

		// �񖼏̂̒萔
		public const string NumColName = "num";
		public const string IdColName = "id";
		public const string NameColName = "name";
		public const string DataColName = "data";

		// ��
		private DataColumn myNumCol;
		private DataColumn myIdCol;
		private DataColumn myNameCol;
		private DataColumn myDataCol;

// �v���p�e�B

		public DataColumn NumCol{get{return myNumCol;}}
		public DataColumn IdCol{get{return myIdCol;}}
		public DataColumn NameCol{get{return myNameCol;}}
		public DataColumn DataCol{get{return myDataCol;}}


// �R���X�g���N�^
		public HtmlElementGroupTable() : base(){
			InitColumns();
		}


// �f�[�^�̎擾

// �f�[�^�̃��[�h

		public void AddData(HtmlElementGroup heg){
			Object[] data = new Object[]{null, heg.Id, heg.Name, heg};
			DataRow row = this.NewRow();
			row.ItemArray = data;
			this.Rows.Add(row);
		}



// ������

		private void InitColumns(){
			// Column ��ݒ肵�܂��B
			// Num
			myNumCol = new DataColumn(NumColName, typeof(string));
			myNumCol.Unique = true;
			myNumCol.AutoIncrement = true;
			myNumCol.AllowDBNull = false;
			this.Columns.Add(myNumCol);
			this.PrimaryKey = new DataColumn[]{myNumCol};

			// Id
			myIdCol = new DataColumn(IdColName, typeof(string));
			myIdCol.Unique = true;
			myIdCol.AutoIncrement = false;
			myIdCol.AllowDBNull = false;
			this.Columns.Add(myIdCol);

			// Name
			myNameCol = new DataColumn(NameColName, typeof(string));
			myNameCol.Unique = false;
			myNameCol.AutoIncrement = false;
			myNameCol.AllowDBNull = false;
			this.Columns.Add(myNameCol);

			// Data
			myDataCol = new DataColumn(DataColName, typeof(HtmlElementGroup));
			myDataCol.Unique = false;
			myDataCol.AutoIncrement = false;
			myDataCol.AllowDBNull = false;
			this.Columns.Add(myDataCol);
		}



	} // class
} // namespace


