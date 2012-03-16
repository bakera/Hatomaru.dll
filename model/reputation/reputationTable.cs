using System;
using System.Collections.Generic;
using System.Data;
using System.Xml;

namespace Bakera.Hatomaru{

	/// <summary>
	/// 人気コンテンツのデータを格納する DataTable の派生クラスです。
	/// </summary>
	public class ReputationTable : HatomaruTable{

		// 列名称の定数
		public const string NumColName = "num";
		public const string CountColName = "count";
		public const string ContentColName = "content";

		// 列
		private DataColumn myNumCol;
		private DataColumn myCountCol;
		private DataColumn myContentCol;



// コンストラクタ

		public ReputationTable(){
			InitColumns();
		}
		public ReputationTable(HatomaruReputation x) : this(){
			Load(x);
		}



// データのロード

		/// <summary>
		/// XmlDocument からデータをロードします。
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

// 初期化

		private void InitColumns(){
			// Column を設定します。
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


