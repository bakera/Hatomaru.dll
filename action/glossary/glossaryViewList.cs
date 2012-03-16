using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace Bakera.Hatomaru{

/// <summary>
/// 用語の一覧を表示するアクションです。
/// </summary>
	public partial class GlossaryViewList : GlossaryAction{

		public new const string Label = "用語一覧";

// コンストラクタ

		/// <summary>
		/// 最近の日記表示のためのアクションのインスタンスを開始します。
		/// </summary>
		public GlossaryViewList(HatomaruGlossary model, AbsPath path) : base(model, path){
			myPath = myModel.BasePath;
		}


		/// <summary>
		/// データを GET し、HatomaruResponse を取得します。
		/// </summary>
		protected override HatomaruResponse GetHtmlResponse(){
			Response.SelfTitle = Label;
			InsertHeading(2, Label);
			{
				GlossaryWord[] words = Glossary.GetWordByRead();
				Html.Append(GetReadList("数字・記号・その他", words));
			}
			foreach(char c in HatomaruGlossary.ReadOrder){
				GlossaryWord[] words = Glossary.GetWordByRead(c);
				Html.Append(GetReadList(c.ToString(), words));
			}
			return Response;
		}

		private XmlNode GetReadList(string title, GlossaryWord[] words){
			if(words == null || words.Length == 0) return Html.Null;

			XmlNode result = Html.Div("word-list");
			result.AppendChild(Html.H(3, null, title));
			XmlElement p = WordList(words);
			result.AppendChild(p);
			return result;
		}




	} // End class
} // End Namespace Bakera



