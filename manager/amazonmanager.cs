using System;
using System.IO;
using System.Net;
using System.Xml;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Bakera.Hatomaru{

	public class AmazonManager{

		TimeSpan NoImageItemCacheSpan = new TimeSpan(1,0,0,0); // 1日
		TimeSpan GeneralCacheSpan = new TimeSpan(30,0,0,0); // 30日

		// キャッシュ用の静的 Dictionary
		private Dictionary<string, AmazonItemList> myItemsDic = new Dictionary<string, AmazonItemList>();

		public const string AmazonHrefFormat = "http://www.amazon.co.jp/exec/obidos/redirect?link_code=as2&path=ASIN/{0}&tag=bakerajp-22&camp=247&creative=1211";
		public const string AmazonSrcFormat = "http://www.assoc-amazon.jp/e/ir?t=bakerajp-22&l=as2&o=9&a={0}";
		public const string AmazonImageFormat = "http://images-jp.amazon.com/images/P/{0}.01._OU09_PE0_SCMZZZZZZZ_.jpg";
		public const string AmazonTopUrl = "http://www.amazon.co.jp/gp/redirect.html?link_code=ur2&tag=bakerajp-22&camp=247&creative=1211&location=%2Fgp%2Fhomepage.html";

		private static AmazonWebService Aws = null;
		private HatomaruManager myManager = null;

// コンストラクタ

		public AmazonManager(HatomaruManager manager){
			myManager = manager;
			Aws = new AmazonWebService(manager);
		}


// プロパティ
		public DirectoryInfo CacheDir{get; set;}


// パブリックメソッド
		// ASIN を渡して Item を取得します。
		// キャッシュにあればそこから、無ければリクエストを行います。
		public AmazonItem GetItem(string asin){
			if(string.IsNullOrEmpty(asin)) return null;
			AmazonItem i = LoadItem(asin);
			if(i == null){
				i = RequestItem(asin);
			}
			return i;
		}


		// 検索結果を取得します。
		// キャッシュにあればそこから、無ければリクエストを行います。
		public AmazonItemList GetSearchItem(AmazonIndexType index, string query, int pageNum){
			string key = index.ToString() + "/" + query + "/" + pageNum.ToString();
			if(myItemsDic.ContainsKey(key)) return myItemsDic[key];
			AmazonItemList i = Search(index, query, pageNum);
			myItemsDic[key] = i;
			return i;
		}


// プライベートメソッド

		// キャッシュファイルを指す FileInfo を取得します。
		private FileInfo GetCacheFile(string asin){
			string fileName = CacheDir.FullName + '/' + asin.PathEncode() + ".xml";
			return new FileInfo(fileName);
		}

/*
		// Item をシリアライズして保存します。
		private void SaveSerializedItem(Item item){
			if(item == null) return;
			if(CacheDir == null || !CacheDir.Exists) return;
			FileInfo cacheFile = GetCacheFile(item.ASIN);
			try{
				using(FileStream fs = cacheFile.Open(FileMode.Create, FileAccess.Write, FileShare.None)){
					mySerializer.Serialize(fs, item);
					fs.Close();
				}
			} catch(IOException){}
		}
*/

		// キャッシュとして保存されたXMLからAmazonItem をロードします。
		// 該当するXMLが無い場合、キャッシュ期限切れの場合はnullを返します。
		private AmazonItem LoadItem(string asin){
			if(string.IsNullOrEmpty(asin)) return null;
			FileInfo cacheFile = GetCacheFile(asin);
			if(!cacheFile.Exists) return null;

			// 0バイトのファイルができている場合 null を返す
			if(cacheFile.Length == 0) return null;

			// キャッシュ期限切れならnullを返す
			TimeSpan cacheTimeSpan = DateTime.Now - cacheFile.LastWriteTime;
			if(cacheTimeSpan > GeneralCacheSpan){
				return null;
			}

			// データを読む
			XmlDocument doc = new XmlDocument();
			doc.XmlResolver = null;
			try{
				using(FileStream fs = cacheFile.Open(FileMode.Open, FileAccess.Read, FileShare.Read)){
					doc.Load(fs);
					fs.Close();
				}
			} catch(IOException){
				return null;
			}
			AmazonItem result = AmazonItem.Parse(doc);
			if(result == null){
				return null;
			}
			// 画像がない場合はキャッシュ期限が短くなります
			if(result.Image == null && cacheTimeSpan > NoImageItemCacheSpan){
				return null;
			}
			return result;
		}

		// Web サービスにリクエストを発行して AmazonItem を取得します。
		// 取得したXMLは該当箇所に保存します。
		private AmazonItem RequestItem(string asin){
			XmlDocument result = null;
			try{
				result = Aws.GetItemLookupXml(asin);
			} catch (WebException){}
			if(result == null) return null;

			FileInfo cacheFile = GetCacheFile(asin);
			try{
				using(FileStream fs = cacheFile.Open(FileMode.Create, FileAccess.Write, FileShare.None)){
					result.Save(fs);
					fs.Close();
				}
			} catch(IOException){}
			return AmazonItem.Parse(result);
		}


		// Web サービスにリクエストを発行して検索結果を取得します。
		private AmazonItemList Search(AmazonIndexType index, string q, int pageNum){
			XmlDocument result = Aws.GetItemSearchXml(q, index, pageNum);
			if(result == null){
				throw new Exception("XMLが取得できませんでした。");
			}
			return AmazonItemList.Parse(result);
		}


	} // End class


} // End NameSpace

