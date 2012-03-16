using System;
using System.Xml;

namespace Bakera.Hatomaru{

	public class AmazonItem{

		public const string ItemElementName = "Item";
		public const string AsinElementName = "ASIN";
		public const string ItemAttributesElementName = "ItemAttributes";
		public const string TitleElementName = "Title";
		public const string DetailPageUrlElementName = "DetailPageURL";
		public const string ImageElementName = "SmallImage";


	// �R���X�g���N�^
		public AmazonItem(){}
		public AmazonItem(XmlElement itemElement){
			Load(itemElement);
		}


	// �v���p�e�B
		public string Asin{get; set;}
		public string Title{get; set;}
		public string DetailPageUrl{get; set;}
		public AmazonImage Image{get; set;}

	// public���\�b�h

		// XmlElement����AmazonItem�̃v���p�e�B�����[�h���܂��B
		public void Load(XmlElement itemElement){
			if(itemElement.Name != ItemElementName){
				throw new ArgumentException("AmazonItem�����[�h���悤�Ƃ��܂������AItem�v�f���n����܂���ł����B�v�f��:" + itemElement.Name);
			}
			this.Asin = itemElement.GetInnerText(AsinElementName);
			this.DetailPageUrl = itemElement.GetInnerText(DetailPageUrlElementName);

			XmlElement attrElement = itemElement[ItemAttributesElementName];
			if(attrElement != null){
				this.Title = attrElement.GetInnerText(TitleElementName);
			}
			XmlNodeList imageElements = itemElement.GetElementsByTagName(ImageElementName);
			if(imageElements.Count > 0) this.Image = new AmazonImage(imageElements[0] as XmlElement);
		}

		// XmlDocument���� Item�v�f��T���� AmazonItem�𐶐����܂��B
		// Item�v�f�������ꍇ��null��Ԃ��܂��B
		public static AmazonItem Parse(XmlDocument doc){
			if(doc == null) return null;
			XmlNodeList xnl = doc.GetElementsByTagName(ItemElementName);
			if(xnl.Count == 0) return null;
			return new AmazonItem(xnl[0] as XmlElement);
		}


		public override string ToString(){
			string result = this.Title + "\n";
			if(!string.IsNullOrEmpty(this.DetailPageUrl)) result += this.DetailPageUrl + "\n";
			if(this.Image != null) result += this.Image.Url + "\n";
			return result;
		}


	} // End class


} // End NameSpace

