using System;
using System.Collections.Generic;
using System.Data;

namespace Bakera.Hatomaru{

	/// <summary>
	/// ÉfÅ[É^Çäiî[Ç∑ÇÈ DataTable ÇÃîhê∂ÉNÉâÉXÇ≈Ç∑ÅB
	/// </summary>
	public abstract class HatomaruTable : DataTable{

		public string SelectString(string colname, string data){
			string select = String.Format("[{0}]='{1}'", EscapeBracket(colname), EscapeSingleQuote(data));
			return select;
		}

		public string EscapeBracket(object o){
			return o.ToString().Replace("[", "\\[").Replace("]", "\\]");
		}

		public string EscapeSingleQuote(object o){
			return o.ToString().Replace("'", "''");
		}

		public virtual DataRow[] GetDataRows(string searchCol, string searchValue){
			DataRow[] r = this.Select(SelectString(searchCol, searchValue));
			if(r == null) return null;
			return r;
		}
		public virtual DataRow[] GetDataRows(string searchCol, string searchValue, string sort){
			DataRow[] r = this.Select(SelectString(searchCol, searchValue), sort);
			if(r == null) return null;
			return r;
		}

		public virtual DataRow GetDataRow(string searchCol, string searchValue){
			DataRow[] r = GetDataRows(searchCol, searchValue);
			if(r == null) return null;
			if(r.Length == 0) return null;
			return r[0];
		}

		public virtual Object GetData(string searchCol, string searchValue, DataColumn dataCol){
			DataRow r = GetDataRow(searchCol, searchValue);
			if(r == null) return null;
			return r[dataCol];
		}

		public virtual T GetData<T>(string searchCol, string searchValue, DataColumn dataCol) where T : class{
			return GetData(searchCol, searchValue, dataCol) as T;
		}

		public virtual T[] GetMultiData<T>(string searchCol, string searchValue, DataColumn dataCol) where T : class{
			return GetMultiData<T>(searchCol, searchValue, dataCol, null);
		}
		public virtual T[] GetMultiData<T>(string searchCol, string searchValue, DataColumn dataCol, string sort) where T : class{
			DataRow[] r = GetDataRows(searchCol, searchValue, sort);
			if(r == null) return null;
			T[] result = new T[r.Length];
			for(int i = 0; i < result.Length; i++){
				result[i] = r[i][dataCol] as T;
			}
			return result;
		}


	} // class HatomaruTable
} // namespace


