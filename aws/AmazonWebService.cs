using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Web;
using System.Xml;

namespace Bakera.Hatomaru{
	class AmazonWebService{
		private HatomaruManager myManager = null;
		private AmazonProductAdvtApi.SignedRequestHelper myHelper = null;

	// コンストラクタ
		public AmazonWebService(HatomaruManager manager){
			myManager = manager;
			myHelper = new AmazonProductAdvtApi.SignedRequestHelper(manager.IniData.AmazonWsAccessKeyId, manager.IniData.AmazonWsSecretKey, manager.IniData.AmazonServiceHostName);
		}


	// IDを指定して、ItemLookupの結果を含むXmlDocumentを取得します。
		public XmlDocument GetItemLookupXml(string itemID){
			Dictionary<string, string> requestParams = new Dictionary<string, String>();
			requestParams["Service"] = "AWSECommerceService";
			requestParams["Version"] = "2009-03-31";
			requestParams["AssociateTag"] = myManager.IniData.AmazonAssociateTag;
			requestParams["Operation"] = "ItemLookup";
			requestParams["ItemId"] = itemID;
			requestParams["ResponseGroup"] = "ItemAttributes,Images";
			string requestUrl = myHelper.Sign(requestParams);
			return Request(requestUrl);
		}


	// キーワードを指定して、ItemSearchの結果を含むXmlDocumentを取得します。
	// 検索対象はAmazonIndexType.Blendedとなります。
		public XmlDocument GetItemSearchXml(string keywords){
			return GetItemSearchXml(keywords, AmazonIndexType.Blended, 1);
		}

	// キーワードと検索対象を指定して、ItemSearchの結果を含むXmlDocumentを取得します。
		public XmlDocument GetItemSearchXml(string keywords, AmazonIndexType type){
			return GetItemSearchXml(keywords, type, 1);
		}


	// キーワードとページ数、検索対象のAmazonIndexTypeを指定して、ItemSearchの結果を含むXmlDocumentを取得します。
		public XmlDocument GetItemSearchXml(string keywords, AmazonIndexType type, int itemPage){
			Dictionary<string, string> requestParams = new Dictionary<string, String>();
			requestParams["Service"] = "AWSECommerceService";
			requestParams["Version"] = "2009-03-31";
			requestParams["AssociateTag"] = myManager.IniData.AmazonAssociateTag;
			requestParams["Operation"] = "ItemSearch";

			requestParams["Keywords"] = keywords;
			requestParams["ItemPage"] = itemPage.ToString();

			requestParams["SearchIndex"] = type.ToString();
			requestParams["ResponseGroup"] = "ItemAttributes,Images";
			string requestUrl = myHelper.Sign(requestParams);
			return Request(requestUrl);
		}

	// リクエストを行い、XmlDocumentを返します。
		private XmlDocument Request(string requestUrl){
			XmlDocument result = new XmlDocument();
			result.XmlResolver = null;
			WebRequest request = HttpWebRequest.Create(requestUrl);
			WebResponse response = request.GetResponse();
			result.Load(response.GetResponseStream());
			return result;
		}
	}
}



