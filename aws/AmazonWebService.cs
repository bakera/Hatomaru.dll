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

	// �R���X�g���N�^
		public AmazonWebService(HatomaruManager manager){
			myManager = manager;
			myHelper = new AmazonProductAdvtApi.SignedRequestHelper(manager.IniData.AmazonWsAccessKeyId, manager.IniData.AmazonWsSecretKey, manager.IniData.AmazonServiceHostName);
		}


	// ID���w�肵�āAItemLookup�̌��ʂ��܂�XmlDocument���擾���܂��B
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


	// �L�[���[�h���w�肵�āAItemSearch�̌��ʂ��܂�XmlDocument���擾���܂��B
	// �����Ώۂ�AmazonIndexType.Blended�ƂȂ�܂��B
		public XmlDocument GetItemSearchXml(string keywords){
			return GetItemSearchXml(keywords, AmazonIndexType.Blended, 1);
		}

	// �L�[���[�h�ƌ����Ώۂ��w�肵�āAItemSearch�̌��ʂ��܂�XmlDocument���擾���܂��B
		public XmlDocument GetItemSearchXml(string keywords, AmazonIndexType type){
			return GetItemSearchXml(keywords, type, 1);
		}


	// �L�[���[�h�ƃy�[�W���A�����Ώۂ�AmazonIndexType���w�肵�āAItemSearch�̌��ʂ��܂�XmlDocument���擾���܂��B
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

	// ���N�G�X�g���s���AXmlDocument��Ԃ��܂��B
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



