using System;
using System.IO;

namespace Bakera.Hatomaru{

/*
 キャッシュデータに必要な機能 : そのキャッシュが有効かどうかを示す bool値

※更新日を見る場合
A: そのキャッシュの生成日時
B: そのキャッシュの構成データの最終更新日
 を比較して、A < B ならこのキャッシュは無効となる。
 ローカルデータのキャッシュの場合はこの比較を行う。

※キャッシュの TTL を見る場合
A: そのキャッシュの生成日時
B: そのキャッシュの TTL
C: 現在時刻
 を取得して、A+B > C であればキャッシュは無効となる。
 Web サービスのキャッシュはこちら。

*/

/*
　レスポンスが最新かどうか?
　→データソースすべてが最新かどうか?
　最新でないオブジェクトは IsNewest に false を返す。

　レスポンスの LastModified は?
　→ *生成時点の* データソースすべての LastModified の中でもっとも新しいもの
　キャッシュする前にこれを覚えておく。

*/

	/// <summary>
	/// CacehManager で処理することができるキャッシュデータが持つインターフェイスを定義します。
	/// </summary>
	public interface ICacheData{

		/// <summary>
		/// このデータをキャッシュして良いか示す値を設定・取得します。
		/// キャッシュしても良ければ true となります。
		/// </summary>
		bool IsCacheable { get; }

		/// <summary>
		/// このキャッシュの生成日時を取得します。
		/// </summary>
		DateTime LastModified{ get; }

		/// <summary>
		/// このキャッシュが最新のデータと一致しているかどうか判断し、最新であれば ture を返します。
		/// </summary>
		bool IsNewest{ get; }

		/// <summary>
		/// このキャッシュが破棄されているかどうか判断し、破棄されていれば ture を返します。
		/// </summary>
		bool IsExpired{ get; }


	} // End class HatomaruData
} // End Namespace Bakera







