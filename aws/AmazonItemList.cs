using System;
using System.Collections.Generic;
using System.Xml;

namespace Bakera.Hatomaru{

	// 検索結果の AmazonItem のグループを格納します。
	public class AmazonItemList : List<AmazonItem>{

		public const string ItemsElementName = "Items";
		public const string TotalResultsElementName = "TotalResults";
		public const string TotalPagesElementName = "TotalPages";


	// コンストラクタ
		public AmazonItemList(){}
		public AmazonItemList(XmlElement itemsElement){
			Load(itemsElement);
		}


		public int TotalResults{get; set;}
		public int TotalPages{get; set;}

	// メソッド
		// Items要素からAmazonItemをロードして格納します。
		public void Load(XmlElement itemsElement){
			this.TotalResults = itemsElement.GetInnerText(TotalResultsElementName).ToInt32();
			this.TotalPages = itemsElement.GetInnerText(TotalPagesElementName).ToInt32();
			XmlNodeList items = itemsElement.GetElementsByTagName(AmazonItem.ItemElementName);
			foreach(XmlElement e in items){
				AmazonItem i = new AmazonItem(e);
				this.Add(i);
			}

		}

		// XmlDocumentから Items要素を探して AmazonItemListを生成します。
		// Item要素が無い場合はnullを返します。
		public static AmazonItemList Parse(XmlDocument doc){
			if(doc == null) return null;
			XmlNodeList xnl = doc.GetElementsByTagName(ItemsElementName);
			if(xnl.Count == 0) return null;
			return new AmazonItemList(xnl[0] as XmlElement);
		}


	} // End class


} // End NameSpace

