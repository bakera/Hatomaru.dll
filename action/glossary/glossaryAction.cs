using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Xml;

namespace Bakera.Hatomaru{

/// <summary>
/// 日記を制御するクラスです。
/// </summary>
	public abstract class GlossaryAction : HatomaruGetAction{

		public const string Label = "鳩丸ぐろっさり (用語集)";

// コンストラクタ

		protected GlossaryAction(HatomaruGlossary model, AbsPath path) : base(model, path){}



// 静的メソッド
		public static void SetReplaceUrl(Xhtml html){}


// プロテクトメソッド

		// 日記のサブナビの LinkItem を取得します。
		protected override LinkItem[] GetSubNav(){
			return new LinkItem[]{
				GetActionLink(typeof(GlossaryViewGenre)),
			};
		}

		protected XmlElement WordList(GlossaryWord[] words){
			XmlElement p = Html.P();
			for(int i=0; i < words.Length; i++){
				if(i > 0) p.AppendChild(Html.Text(" / "));
				AbsPath wordPath = BasePath.Combine(words[i].Name.PathEncode());
				XmlElement a = Html.A(wordPath);
				a.InnerText = words[i].Name;
				p.AppendChild(a);
			}
			return p;
		}




	} // End class DiaryAction
} // End Namespace Bakera



