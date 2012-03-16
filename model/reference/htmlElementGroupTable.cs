using System;
using System.Data;
using System.Xml;
using System.IO;
using System.Collections.Generic;

namespace Bakera.Hatomaru{

	/// <summary>
	/// HTMLリファレンスの要素グループのデータを格納する DataTable の派生クラスです。
	/// </summary>
	public class HtmlElementGroupTable : HatomaruTable{

		// 列名称の定数
		public const string NumColName = "num";
		public const string IdColName = "id";
		public const string NameColName = "name";
		public const string DataColName = "data";

		// 列
		private DataColumn myNumCol;
		private DataColumn myIdCol;
		private DataColumn myNameCol;
		private DataColumn myDataCol;

// プロパティ

		public DataColumn NumCol{get{return myNumCol;}}
		public DataColumn IdCol{get{return myIdCol;}}
		public DataColumn NameCol{get{return myNameCol;}}
		public DataColumn DataCol{get{return myDataCol;}}


// コンストラクタ
		public HtmlElementGroupTable() : base(){
			InitColumns();
		}


// データの取得

// データのロード

		public void AddData(HtmlElementGroup heg){
			Object[] data = new Object[]{null, heg.Id, heg.Name, heg};
			DataRow row = this.NewRow();
			row.ItemArray = data;
			this.Rows.Add(row);
		}



// 初期化

		private void InitColumns(){
			// Column を設定します。
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


