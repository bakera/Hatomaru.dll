using System;
using System.Collections.Generic;
using System.Xml;

namespace Bakera.Hatomaru{

	// �������ʂ� AmazonItem �̃O���[�v���i�[���܂��B
	public class AmazonItemList : List<AmazonItem>{

		public const string ItemsElementName = "Items";
		public const string TotalResultsElementName = "TotalResults";
		public const string TotalPagesElementName = "TotalPages";


	// �R���X�g���N�^
		public AmazonItemList(){}
		public AmazonItemList(XmlElement itemsElement){
			Load(itemsElement);
		}


		public int TotalResults{get; set;}
		public int TotalPages{get; set;}

	// ���\�b�h
		// Items�v�f����AmazonItem�����[�h���Ċi�[���܂��B
		public void Load(XmlElement itemsElement){
			this.TotalResults = itemsElement.GetInnerText(TotalResultsElementName).ToInt32();
			this.TotalPages = itemsElement.GetInnerText(TotalPagesElementName).ToInt32();
			XmlNodeList items = itemsElement.GetElementsByTagName(AmazonItem.ItemElementName);
			foreach(XmlElement e in items){
				AmazonItem i = new AmazonItem(e);
				this.Add(i);
			}

		}

		// XmlDocument���� Items�v�f��T���� AmazonItemList�𐶐����܂��B
		// Item�v�f�������ꍇ��null��Ԃ��܂��B
		public static AmazonItemList Parse(XmlDocument doc){
			if(doc == null) return null;
			XmlNodeList xnl = doc.GetElementsByTagName(ItemsElementName);
			if(xnl.Count == 0) return null;
			return new AmazonItemList(xnl[0] as XmlElement);
		}


	} // End class


} // End NameSpace

