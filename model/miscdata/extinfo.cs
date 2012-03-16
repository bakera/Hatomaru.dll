using System;
using System.Xml;

namespace Bakera.Hatomaru{
	
	
/// <summary>
/// 拡張子処理情報のクラス
/// </summary>
	public class ExtInfo{
		private string myDescription;

		public const string ExtInfoName = "name";
		public const string ExtInfoType = "type";
		public const string ExtInfoCharset = "charset";
		public const string ExtInfoDisposition = "disposition";
		public const string ExtInfoMaxAge = "maxage";

// コンストラクタ

		public ExtInfo(){}
		public ExtInfo(XmlElement e){
			LoadXml(e);
		}


// プロパティ
		public string Name{
			get; private set;
		}

		public string ContentType{
			get; private set;
		}

		public string Charset{
			get; private set;
		}

		public bool Disposition{
			get; private set;
		}

		// Expires の期間 maxage属性として日単位で指定
		public TimeSpan MaxAge{
			get;
			private set;
		}

		public string Description{
			get{
				if(!string.IsNullOrEmpty(myDescription)) return myDescription;
				return Name.TrimStart('.').ToUpper() + "ファイル";
			}
			private set{
				myDescription = value;
			}
		}

// パブリックメソッド

		public void LoadXml(XmlElement e){
			Name = e.GetAttributeValue(ExtInfoName);
			ContentType = e.GetAttributeValue(ExtInfoType);
			Charset = e.GetAttributeValue(ExtInfoCharset);
			Description = e.InnerText;

			string tempDisposition = e.GetAttributeValue(ExtInfoDisposition);
			if(!string.IsNullOrEmpty(tempDisposition)) Disposition = true;

			int maxAgeDays = e.GetAttributeInt(ExtInfoMaxAge);
			MaxAge = new TimeSpan(maxAgeDays, 0, 0, 0);
		}




	} // End class ExtInfo
} // End Namespace Bakera







