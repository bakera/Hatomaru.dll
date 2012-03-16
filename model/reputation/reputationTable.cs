using System;
using System.Collections.Generic;
using System.Data;
using System.Xml;

namespace Bakera.Hatomaru{

	/// <summary>
	/// �l�C�R���e���c�̃f�[�^���i�[���� DataTable �̔h���N���X�ł��B
	/// </summary>
	public class ReputationTable : HatomaruTable{

		// �񖼏̂̒萔
		public const string NumColName = "num";
		public const string CountColName = "count";
		public const string ContentColName = "content";

		// ��
		private DataColumn myNumCol;
		private DataColumn myCountCol;
		private DataColumn myContentCol;



// �R���X�g���N�^

		public ReputationTable(){
			InitColumns();
		}
		public ReputationTable(HatomaruReputation x) : this(){
			Load(x);
		}



// �f�[�^�̃��[�h

		/// <summary>
		/// XmlDocument ����f�[�^�����[�h���܂��B
		/// </summary>
		public void Load(HatomaruReputation x){
			XmlNodeList contents = x.Document.GetElementsByTagName(HatomaruReputation.ContentElementName);
			foreach(XmlElement e in contents){
				var rc = new ReputationContent(x.Manager, e);
				Rows.Add(new Object[]{
					null,
					rc.Count,
					rc
				});
			}
		}

// ������

		private void InitColumns(){
			// Column ��ݒ肵�܂��B
			// Num
			myNumCol = new DataColumn(NumColName, typeof(int));
			myNumCol.Unique = true;
			myNumCol.AutoIncrement = true;
			myNumCol.AllowDBNull = false;
			this.Columns.Add(myNumCol);
			this.PrimaryKey = new DataColumn[]{myNumCol};

			// Count
			myCountCol = new DataColumn(CountColName, typeof(int));
			myCountCol.Unique = false;
			myCountCol.AutoIncrement = false;
			myCountCol.AllowDBNull = false;
			this.Columns.Add(myCountCol);

			// Content
			myContentCol = new DataColumn(ContentColName, typeof(ReputationContent));
			myContentCol.Unique = false;
			myContentCol.AutoIncrement = false;
			myContentCol.AllowDBNull = true;
			this.Columns.Add(myContentCol);

		}




	} // class BbsTable
} // namespace


