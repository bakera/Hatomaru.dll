using System;
using System.Xml;

namespace Bakera.Hatomaru{

	public class AmazonImage{

		public const string UrlElementName = "URL";
		public const string HeightElementName = "Height";
		public const string WidthElementName = "Width";

		public const string NoImageUrl = "/shared/img/noimage.png";
		public const int NoImageWidth = 75;
		public const int NoImageHeight = 75;


	// コンストラクタ
		public AmazonImage(){}
		public AmazonImage(XmlElement imageElement){
			Load(imageElement);
		}


	// プロパティ
		public string Url{get; set;}
		public int Height{get; set;}
		public int Width{get; set;}


	// publicメソッド

		// XmlElementからAmazonItemのプロパティをロードします。
		public void Load(XmlElement imageElement){
			this.Url = imageElement.GetInnerText(UrlElementName);
			this.Height = imageElement.GetInnerText(HeightElementName).ToInt32();
			this.Width = imageElement.GetInnerText(WidthElementName).ToInt32();
		}

		// No Image 画像を取得します。
		public static AmazonImage GetNoImage(){
			AmazonImage result = new AmazonImage();
			result.Url = NoImageUrl;
			result.Width = NoImageWidth;
			result.Height = NoImageHeight;
			return result;
		}

	} // End class


} // End NameSpace

