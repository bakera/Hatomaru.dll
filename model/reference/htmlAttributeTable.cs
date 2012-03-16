using System;
using System.Data;
using System.Xml;
using System.IO;
using System.Collections.Generic;

namespace Bakera.Hatomaru{

	/// <summary>
	/// HTML���t�@�����X�̃f�[�^�t�H�[�}�b�g�̃f�[�^���i�[���� DataTable �̔h���N���X�ł��B
	/// </summary>
	public class HtmlAttributeTable : HatomaruTable{

		// �񖼏̂̒萔
		public const string NumColName = "num";
		public const string IdColName = "id";
		public const string NameColName = "name";
		public const string AttributeColName = "attribute";


		// ��
		private DataColumn myNumCol;
		private DataColumn myIdCol;
		private DataColumn myNameCol;
		private DataColumn myAttributeCol;

// �v���p�e�B

		public DataColumn NumCol{get{return myNumCol;}}
		public DataColumn IdCol{get{return myIdCol;}}
		public DataColumn NameCol{get{return myNameCol;}}
		public DataColumn AttributeCol{get{return myAttributeCol;}}


// �R���X�g���N�^
		public HtmlAttributeTable() : base(){
			InitColumns();
		}


// �f�[�^�̃��[�h

		public void AddData(HtmlAttribute e){
			Object[] data = new Object[]{null, e.Id, e.Name, e};
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


			// Attribute
			myAttributeCol = new DataColumn(AttributeColName, typeof(HtmlAttribute));
			myAttributeCol.Unique = false;
			myAttributeCol.AutoIncrement = false;
			myAttributeCol.AllowDBNull = false;
			this.Columns.Add(myAttributeCol);

		}



	} // class GlossaryTable
} // namespace


