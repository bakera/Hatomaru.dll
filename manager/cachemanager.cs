using System;
using System.Collections.Generic;
using System.Data;
using System.IO;

namespace Bakera.Hatomaru{

/*
 レスポンスは "/" で始まるパス文字列のキーで管理する。
 データは FileInfo.FullName で管理する
*/


	/// <summary>
	/// hatomaru.dll のレスポンスキャッシュを管理するクラスです。
	/// </summary>
	public class CacheManager<T> where T : ICacheData{

		// 応答キャッシュ : レスポンスデータのキャッシュ
		private readonly Dictionary<string, T> myCache = new Dictionary<string, T>();

// コンストラクタ
		/// <summary>
		/// キャッシュマネージャのインスタンスを作成します。
		/// </summary>
		public CacheManager(){}


// パブリックメソッド

		/// <summary>
		/// 指定されたキーに対応するキャッシュデータがあれば true を返します。
		/// </summary>
		public bool Contains(string key){
			return myCache.ContainsKey(key);
		}
		/// <summary>
		/// 指定されたキーに対応するキャッシュデータがあれば true を返します。
		/// </summary>
		public bool Contains(FileInfo f){
			return Contains(f.FullName);
		}

		/// <summary>
		/// 指定されたキーに対応するキャッシュデータを探し、データを返します。
		/// データがなければ null を返します。
		/// このメソッドではデータが有効かどうかはチェックしません。
		/// データの有効性をチェックして取得するには GetEneabledCache メソッドを使用します。
		/// </summary>
		protected T Get(string key){
			if(!Contains(key)) return default(T);
			return myCache[key];
		}

		/// <summary>
		/// 指定されたキーに対応するキャッシュデータを探し、データが有効であれば返します。
		/// データがない場合は null を返します。
		/// </summary>
		public virtual T GetEneabledCache(string key){
			T result = Get(key);
			if(result == null) return default(T);
			if(result.IsExpired || !result.IsNewest){
				myCache.Remove(key);
				return default(T);
			}
			return result;
		}

		/// <summary>
		/// キーと値を指定してキャッシュデータを追加します。
		/// 書き込み時にはロックが行われます。このメソッドはスレッドセーフのつもりです。
		/// </summary>
		public virtual void Add(string key, T value){
			if(!value.IsCacheable) return;
			lock(myCache){
				myCache[key] = value;
			}
		}

		/// <summary>
		/// キーを指定してキャッシュデータを削除します。
		/// 書き込み時にはロックが行われます。このメソッドはスレッドセーフのつもりです。
		/// </summary>
		public void Remove(string key){
			lock(myCache){
				myCache.Remove(key);
			}
		}


		/// <summary>
		/// すべてのキャッシュデータを取得します。
		/// </summary>
		public T[] GetAllData(){
			T[] result = new T[myCache.Count];
			int i = 0;
			foreach(KeyValuePair<string, T> pair in myCache){
				result[i++] = (T)pair.Value;
			}
			return result;
		}


	} // End Class CacheManager

} // End Namespace 
