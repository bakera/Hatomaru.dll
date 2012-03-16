using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Web;

namespace Bakera.Hatomaru{

	/// <summary>
	/// 200 OK でファイルを返すレスポンスです。
	/// </summary>
	public class FileResponse : HatomaruResponse{

		private FileInfo myFileSource;
		private ExtInfo myExtInfo;

		/// <summary>
		/// データソースと ExtInfo を元に、FileResponse のインスタンスを開始します。
		/// ファイルデータを返すレスポンスに使用します。
		/// </summary>
		public FileResponse(HatomaruData source, ExtInfo ex) : base(source){
			myExtInfo = ex;
			ContentType = ex.ContentType;
			Charset = ex.Charset;
			myFileSource = source.File;
		}

// プロパティ

		/// <summary>
		/// このレスポンスのファイルソースを設定・取得します。
		/// </summary>
		public FileInfo FileSource{
			get{return myFileSource;}
		}

		/// <summary>
		/// このレスポンスの拡張子情報を取得します。
		/// </summary>
		public ExtInfo ExtInfo{
			get{return myExtInfo;}
		}

		/// <summary>
		/// このレスポンスをキャッシュして良いか示す値を設定・取得します。
		/// FileResponse はキャッシュ可能ですが、キャッシュしません。
		/// </summary>
		public override bool IsCacheable {
			get{return false;}
		}

		/// <summary>
		/// レスポンスのサイズを取得します。
		/// </summary>
		public override long Length{
			get{
				return FileSource.Length;
			}
		}


// オーバーライド

		/// <summary>
		/// データソースの中でもっとも新しいものの最終更新時刻を取得します。
		/// データソースがない場合は DateTime.MinValue を返します。
		/// </summary>
		public override DateTime GetNewestSourceTime(){
			if(FileSource == null) return default(DateTime);
			return FileSource.LastWriteTime;
		}


		/// <summary>
		/// 渡された HttpResponse にレスポンスを書き込みます。
		/// </summary>
		public override void WriteResponse(HttpResponse response){
			WriteResponseHeader(response);
			WriteAdditionalHeader(response);
			SetLastModified(response);
			response.WriteFile(FileSource.FullName);
		}


		/// <summary>
		/// 渡された HttpResponse にレスポンスヘッダを書き込みます。
		/// </summary>
		protected void WriteAdditionalHeader(HttpResponse response){
			// Disposition = true ならダウンロードに
			if(myExtInfo.Disposition){
				string condispValue = String.Format(System.Globalization.CultureInfo.InvariantCulture, "attachment; filename={0}", FileSource.Name);
				response.AppendHeader("Content-Disposition", condispValue);
			}

			if(myExtInfo.MaxAge > TimeSpan.Zero){
				response.Cache.SetCacheability(HttpCacheability.Public);
				response.Cache.SetMaxAge(myExtInfo.MaxAge);
				response.Cache.SetExpires(DateTime.Now + myExtInfo.MaxAge);
			}

		}



	}

}
