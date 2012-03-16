using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Xml;

namespace Bakera.Hatomaru{

/// <summary>
/// HatomaruDoc の先頭ページを表示するクラスです。
/// </summary>
	public partial class DocViewTop : DocAction{

// コンストラクタ

		public DocViewTop(HatomaruDoc model, AbsPath path) : base(model, path){
			myPath = myModel.BasePath;
		}


// プロパティ

		/// <summary>
		/// データを GET し、HatomaruResponse を取得します。
		/// </summary>
		protected override HatomaruResponse GetHtmlResponse(){
			InsertHeading(2, myModel.BaseTitle);
			foreach(DocTopic dt in Doc.AnonymousTopics){
				Html.Append(ParseNode(dt.SectionElement, 3));
			}

			DocTopic[] topics = Doc.GetAllNamedTopics();
			if(topics != null && topics.Length > 0){
				XmlElement ul = Html.Create("ul");
				foreach(DocTopic dt in topics){
					XmlElement li = Html.Create("li");
					AbsPath path = myModel.BasePath.Combine(dt.Id);
					XmlElement a = Html.A(path);
					a.InnerText = dt.FullName;
					li.AppendChild(a);
					ul.AppendChild(li);
				}
				Html.Append(ul);
			}
			Html.Append(CommentLink(myModel.BasePath, myModel.BaseTitle));
			DocTopic firstTopic = Doc.GetFirstTopic();
			if(firstTopic != null){
				Next = new LinkItem(myModel.BasePath.Combine(firstTopic.Id), firstTopic.FullName);
			}
			return myResponse;
		}

	} // End class
} // End Namespace



