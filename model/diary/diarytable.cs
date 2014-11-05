using System;
using System.Data;
using System.Xml;
using System.IO;
using System.Collections.Generic;

namespace Bakera.Hatomaru{


/* ================================ */
/*                                  */
/*     DiaryTable �N���X            */
/*                                  */
/* ================================ */

	/// <summary>
	/// ���L�̃f�[�^���i�[���� DataTable �̔h���N���X�ł��B
	/// </summary>
	public class DiaryTable : HatomaruTable{

		// �񖼏̂̒萔
		public const string NumColName = "num";
		public const string DateColName = "date";
		public const string CreatedColName = "created";
		public const string MessageColName = "topic";

		// ��
		private DataColumn myNumCol;
		private DataColumn myDateCol;
		private DataColumn myCreatedCol;
		private DataColumn myMessageCol;


/* ======================== */
/*     �R���X�g���N�^       */
/* ======================== */

		public DiaryTable(){
			InitColumns();
		}



/* ================================ */
/*      �v���p�e�B        */
/* ================================ */

		public DataColumn NumCol{ get{return myNumCol;}}
		public DataColumn DateCol{ get{return myDateCol;}}
		public DataColumn CreatedCol{ get{return myCreatedCol;}}
		public DataColumn MessageCol{ get{return myMessageCol;}}


/* ================================ */
/*     Load ���\�b�h        */
/* ================================ */

		public void AddTopic(Topic t){
			if(t.Id == 0) throw new Exception("�g�s�b�N�̔ԍ�������܂��� : " + t.ToString());
			Object[] data = new Object[]{t.Id, t.Date.Ticks, t.Created.Ticks, t};
			DataRow row = this.NewRow();
			row.ItemArray = data;
			this.Rows.Add(row);
		}



/* ================================ */
/*     �v���C�x�[�g���\�b�h         */
/* ================================ */

		private void InitColumns(){
			// Column ��ݒ肵�܂��B
			// Num
			myNumCol = new DataColumn(NumColName, typeof(int));
			myNumCol.Unique = true;
			myNumCol.AutoIncrement = true;
			myNumCol.AllowDBNull = false;
			this.Columns.Add(myNumCol);
			this.PrimaryKey = new DataColumn[]{myNumCol};


			// Date
			myDateCol = new DataColumn(DateColName, typeof(long));
			myDateCol.Unique = false;
			myDateCol.AutoIncrement = false;
			myDateCol.AllowDBNull = false;
			this.Columns.Add(myDateCol);

			// Created
			myCreatedCol = new DataColumn(CreatedColName, typeof(long));
			myCreatedCol.Unique = false;
			myCreatedCol.AutoIncrement = false;
			myCreatedCol.AllowDBNull = false;
			this.Columns.Add(myCreatedCol);


			// �{��
			myMessageCol = new DataColumn(MessageColName, typeof(Topic));
			myMessageCol.Unique = false;
			myMessageCol.AutoIncrement = false;
			myMessageCol.AllowDBNull = false;
			this.Columns.Add(myMessageCol);

		}



	} // class DiaryTable
} // namespace


