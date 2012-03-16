using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Data;

namespace Bakera.Hatomaru{

	/// <summary>
	/// スパム処理ルールを扱うクラスです。
	/// </summary>
	public partial class SpamRule : HatomaruXml{
		public const string WordElementName = "word";
		public const string AsciionlyScoreName = "asciionly";
		public const string NewpostScoreName = "newpost";
		public const string UrlScoreName = "url";
		public const string EmailScoreName = "email";
		public const string WeightAttributeName = "weight";

		new public const string Name = "spamrule";
		private int myAsciionlyScore;
		private int myNewpostScore;
		private int myUrlScore;
		private int myEmailScore;

		private List<NGWord> myNGWords = new List<NGWord>();

		/// <summary>
		/// 鳩丸データのデータソースの FileInfo と XmlDocument を指定して、HatomaruBbs のインスタンスを開始します。
		/// </summary>
		public SpamRule(HatomaruManager manager, FileInfo f, XmlDocument x) : base(manager, f, x){
			myAsciionlyScore = GetWeight(AsciionlyScoreName);
			myNewpostScore = GetWeight(NewpostScoreName);
			myUrlScore = GetWeight(UrlScoreName);
			myEmailScore = GetWeight(EmailScoreName);
			
			foreach(XmlElement e in Document.GetElementsByTagName(WordElementName)){
				myNGWords.Add(new NGWord(e));
			}
			
		}


// オーバーライドメソッド
		/// <summary>
		/// データを GET し、HatomaruResponse を取得します。
		/// </summary>
		public override HatomaruResponse Get(AbsPath path){
			return new NotFoundResponse(this, path);
		}

// パブリックメソッド
		public int GetSpamScore(Article a){
			string text = a.Subject + "\n" + a.Name + "\n" + a.Message;
			int result = 0;
			if(a.Parent == 0) result += myNewpostScore;
			if(HatomaruActionBase.NotMbsReg.IsMatch(a.Message)) result += myAsciionlyScore;
			if(HatomaruActionBase.UrlReg.IsMatch(text)) result += myUrlScore;
			if(HatomaruActionBase.EmailReg.IsMatch(text)) result += myEmailScore;

			foreach(NGWord n in myNGWords){
				foreach(string s in n.Words){
					if(text.IndexOf(s) >= 0){
						result += n.Score;
					}
				}
			}
			// 中国はスコア+100
			if(a.SrcCountry != null && a.SrcCountry.Equals("CN", StringComparison.InvariantCultureIgnoreCase)){
				result += 100;
			}
			return result;
		}



// プライベートメソッド
		private int GetWeight(string elemName){
			XmlNodeList target = Document.GetElementsByTagName(elemName);
			if(target.Count == 0) return 0;
			XmlElement targetElement = target[0] as XmlElement;
			return targetElement.GetAttribute(WeightAttributeName).ToInt32();
		}



	}
	
}// End Namespace Hatomru





