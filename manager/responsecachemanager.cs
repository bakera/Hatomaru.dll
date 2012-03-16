using System;
using System.Collections.Generic;
using System.Data;
using System.IO;

namespace Bakera.Hatomaru{

/*
 レスポンスのキャッシュファイルは パス + 「_cache」
 キャッシュファイルは Last-Modified の管理に必要

●新規作成・キャッシュファイルがない場合
 レスポンスを生成、最新の DataSource が Last-Modified
 レスポンスをキャッシュに格納
 キャッシュファイル作成、キャッシュファイルの LastWriteTime をレスポンスの Last-Modified の値にする

●新規作成・キャッシュファイルがある場合
 レスポンスを生成、最新の DataSource が Last-Modified
 レスポンスをキャッシュに格納
 キャッシュファイルと内容を比較
 内容が違っていればキャッシュファイルを上書き、キャッシュファイルの LastWriteTime を設定し直す

 内容が一致していればキャッシュファイルはそのまま、レスポンスの Last-Modified の値をキャッシュファイルのものにする
 このままだと IsNewest が false になってしまうので、CheckedTime に現在時刻を記憶する。

 以降、IsNewest は CheckedTime と DataSource の時刻を比較し、CheckedTime の方が新しければ Newest とみなす。

●キャッシュの照会
 IsNewest = true ならそのキャッシュを返す

 IsNewest の動作は?
 DataSource すべてが最新であれば Newest

 そうでない場合は?
 CheckedTime と DataSource の時刻を比較する
 CheckedTime の方が新しければ Newest

 そうでない場合は?
 Refresh を試みる
 成功したら「新規作成・キャッシュファイルがある場合」の処理を行う

*/


	/// <summary>
	/// hatomaru.dll のレスポンスキャッシュを管理するクラスです。
	/// </summary>
	public class ResponseCacheManager : CacheManager<HatomaruResponse>{

		/// <summary>
		/// キャッシュマネージャのインスタンスを作成します。
		/// </summary>
		public ResponseCacheManager(){}


		/// <summary>
		/// キーと値を指定してキャッシュデータを追加します。
		/// 書き込み時にはロックが行われます。このメソッドはスレッドセーフのつもりです。
		/// </summary>
		public void Add(AbsPath keyPath, HatomaruResponse hr){
			if(hr.Path == null) hr.Path = keyPath;
			string key = keyPath.ToString();
			base.Add(key, hr);
		}


		public bool Contains(AbsPath keyPath){
			return Contains(keyPath.ToString());
		}


		/// <summary>
		/// 指定されたキーに対応するキャッシュデータを探し、データが有効であれば返します。
		/// データがない場合、古い場合は null を返します。
		/// </summary>
		public HatomaruResponse GetEneabledCache(AbsPath keyPath){
			return GetEneabledCache(keyPath.ToString());
		}


	} // End Class ResponseCacheManager


} // End Namespace 
