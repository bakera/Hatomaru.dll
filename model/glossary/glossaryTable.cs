using System;
using System.Data;
using System.Xml;
using System.IO;
using System.Collections.Generic;

namespace Bakera.Hatomaru{

	/// <summary>
	/// 鳩丸ぐろっさりのデータを格納する DataTable の派生クラスです。
	/// </summary>
	public class GlossaryTable : HatomaruTable{

		// 列名称の定数
		public const string NameColName = "name";
		public const string GlossaryColName = "glossary";

		// 列
		private DataColumn myNameCol;
		private DataColumn myGlossaryCol;

// コンストラクタ
		public GlossaryTable() : base(){
			InitColumns();
		}


// プロパティ

		public DataColumn NameCol{get{return myNameCol;}}
		public DataColumn GlossaryCol{get{return myGlossaryCol;}}

// データの取得



// データのロード

		public void AddGlossary(GlossaryWord gw){
			Object[] data = new Object[]{gw.Name, gw,};
			DataRow row = this.NewRow();
			row.ItemArray = data;
			this.Rows.Add(row);
		}


// 初期化

		private void InitColumns(){
			// Column を設定します。
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


