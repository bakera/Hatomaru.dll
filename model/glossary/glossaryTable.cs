using System;
using System.Data;
using System.Xml;
using System.IO;
using System.Collections.Generic;

namespace Bakera.Hatomaru{

	/// <summary>
	/// ���ۂ��������̃f�[�^���i�[���� DataTable �̔h���N���X�ł��B
	/// </summary>
	public class GlossaryTable : HatomaruTable{

		// �񖼏̂̒萔
		public const string NameColName = "name";
		public const string GlossaryColName = "glossary";

		// ��
		private DataColumn myNameCol;
		private DataColumn myGlossaryCol;

// �R���X�g���N�^
		public GlossaryTable() : base(){
			InitColumns();
		}


// �v���p�e�B

		public DataColumn NameCol{get{return myNameCol;}}
		public DataColumn GlossaryCol{get{return myGlossaryCol;}}

// �f�[�^�̎擾



// �f�[�^�̃��[�h

		public void AddGlossary(GlossaryWord gw){
			Object[] data = new Object[]{gw.Name, gw,};
			DataRow row = this.NewRow();
			row.ItemArray = data;
			this.Rows.Add(row);
		}


// ������

		private void InitColumns(){
			// Column ��ݒ肵�܂��B
			myNameCol = new DataColumn(NameColName, typeof(string));
			myNameCol.Unique = true;
			myNameCol.AllowDBNull = false;
			this.Columns.Add(myNameCol);
			this.PrimaryKey = new DataColumn[]{myNameCol};

			myGlossaryCol = new DataColumn(GlossaryColName, typeof(GlossaryWord));
			myGlossaryCol.Unique = false;
			myGlossaryCol.AutoIncrement = false;
			myGlossaryCol.AllowDBNull = false;
			this.Columns.Add(myGlossaryCol);

		}



	} // class GlossaryTable
} // namespace


