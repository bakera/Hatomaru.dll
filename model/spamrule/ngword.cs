using System;
using System.Xml;

namespace Bakera.Hatomaru{


	/// <summary>
	/// NGワードを扱うクラスです。
	/// </summary>
	public class NGWord{
		private int myScore;
		private string[] myWords;

		public NGWord(XmlElement e){
			if(e == null) return;
			myWords = e.InnerText.Split(' ');
			myScore = e.GetAttribute(SpamRule.WeightAttributeName).ToInt32();
		}

		public string[] Words{
			get{return myWords;}
		}

		public int Score{
			get{return myScore;}
		}

	}

}

