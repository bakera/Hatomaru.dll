using System;
using System.IO;
using System.Net;
using System.Xml;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Bakera.Hatomaru{

	public class AmazonManager{

		TimeSpan NoImageItemCacheSpan = new TimeSpan(1,0,0,0); // 1��
		TimeSpan GeneralCacheSpan = new TimeSpan(30,0,0,0); // 30��

		// �L���b�V���p�̐ÓI Dictionary
		private Dictionary<string, AmazonItemList> myItemsDic = new Dictionary<string, AmazonItemList>();

		public const string AmazonHrefFormat = "http://www.amazon.co.jp/exec/obidos/redirect?link_code=as2&path=ASIN/{0}&tag=bakerajp-22&camp=247&creative=1211";
		public const string AmazonSrcFormat = "http://www.assoc-amazon.jp/e/ir?t=bakerajp-22&l=as2&o=9&a={0}";
		public const string AmazonImageFormat = "http://images-jp.amazon.com/images/P/{0}.01._OU09_PE0_SCMZZZZZZZ_.jpg";
		public const string AmazonTopUrl = "http://www.amazon.co.jp/gp/redirect.html?link_code=ur2&tag=bakerajp-22&camp=247&creative=1211&location=%2Fgp%2Fhomepage.html";

		private static AmazonWebService Aws = null;
		private HatomaruManager myManager = null;

// �R���X�g���N�^

		public AmazonManager(HatomaruManager manager){
			myManager = manager;
			Aws = new AmazonWebService(manager);
		}


// �v���p�e�B
		public DirectoryInfo CacheDir{get; set;}


// �p�u���b�N���\�b�h
		// ASIN ��n���� Item ���擾���܂��B
		// �L���b�V���ɂ���΂�������A������΃��N�G�X�g���s���܂��B
		public AmazonItem GetItem(string asin){
			if(string.IsNullOrEmpty(asin)) return null;
			AmazonItem i = LoadItem(asin);
			if(i == null){
				i = RequestItem(asin);
			}
			return i;
		}


		// �������ʂ��擾���܂��B
		// �L���b�V���ɂ���΂�������A������΃��N�G�X�g���s���܂��B
		public AmazonItemList GetSearchItem(AmazonIndexType index, string query, int pageNum){
			string key = index.ToString() + "/" + query + "/" + pageNum.ToString();
			if(myItemsDic.ContainsKey(key)) return myItemsDic[key];
			AmazonItemList i = Search(index, query, pageNum);
			myItemsDic[key] = i;
			return i;
		}


// �v���C�x�[�g���\�b�h

		// �L���b�V���t�@�C�����w�� FileInfo ���擾���܂��B
		private FileInfo GetCacheFile(string asin){
			string fileName = CacheDir.FullName + '/' + asin.PathEncode() + ".xml";
			return new FileInfo(fileName);
		}

/*
		// Item ���V���A���C�Y���ĕۑ����܂��B
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

		// �L���b�V���Ƃ��ĕۑ����ꂽXML����AmazonItem �����[�h���܂��B
		// �Y������XML�������ꍇ�A�L���b�V�������؂�̏ꍇ��null��Ԃ��܂��B
		private AmazonItem LoadItem(string asin){
			if(string.IsNullOrEmpty(asin)) return null;
			FileInfo cacheFile = GetCacheFile(asin);
			if(!cacheFile.Exists) return null;

			// 0�o�C�g�̃t�@�C�����ł��Ă���ꍇ null ��Ԃ�
			if(cacheFile.Length == 0) return null;

			// �L���b�V�������؂�Ȃ�null��Ԃ�
			TimeSpan cacheTimeSpan = DateTime.Now - cacheFile.LastWriteTime;
			if(cacheTimeSpan > GeneralCacheSpan){
				return null;
			}

			// �f�[�^��ǂ�
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
			// �摜���Ȃ��ꍇ�̓L���b�V���������Z���Ȃ�܂�
			if(result.Image == null && cacheTimeSpan > NoImageItemCacheSpan){
				return null;
			}
			return result;
		}

		// Web �T�[�r�X�Ƀ��N�G�X�g�𔭍s���� AmazonItem ���擾���܂��B
		// �擾����XML�͊Y���ӏ��ɕۑ����܂��B
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


		// Web �T�[�r�X�Ƀ��N�G�X�g�𔭍s���Č������ʂ��擾���܂��B
		private AmazonItemList Search(AmazonIndexType index, string q, int pageNum){
			XmlDocument result = Aws.GetItemSearchXml(q, index, pageNum);
			if(result == null){
				throw new Exception("XML���擾�ł��܂���ł����B");
			}
			return AmazonItemList.Parse(result);
		}


	} // End class


} // End NameSpace

