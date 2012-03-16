using System;
using System.Data;
using System.Xml;
using System.IO;
using System.Collections.Generic;

namespace Bakera.Hatomaru{

	/// <summary>
	/// HTMLリファレンスのデータフォーマットのデータを格納する DataTable の派生クラスです。
	/// </summary>
	public class HtmlElementTable : HatomaruTable{

		// 列名称の定数
		public const string NumColName = "num";
		public const string IdColName = "id";
		public const string NameColName = "name";
		public const string ElementColName = "element";


		// 列
		private DataColumn myNumCol;
		private DataColumn myIdCol;
		private DataColumn myNameCol;
		private DataColumn myElementCol;

// プロパティ

		public DataColumn NumCol{get{return myNumCol;}}
		public DataColumn IdCol{get{return myIdCol;}}
		public DataColumn NameCol{get{return myNameCol;}}
		public DataColumn ElementCol{get{return myElementCol;}}


// コンストラクタ
		public HtmlElementTable() : base(){
			InitColumns();
		}


// データのロード

		public void AddData(HtmlElement e){
			Object[] data = new Object[]{null, e.Id, e.Name, e};
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


			// Element
			myElementCol = new DataColumn(ElementColName, typeof(HtmlElement));
			myElementCol.Unique = false;
			myElementCol.AutoIncrement = false;
			myElementCol.AllowDBNull = false;
			this.Columns.Add(myElementCol);

		}



	} // class GlossaryTable
} // namespace


