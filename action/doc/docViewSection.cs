using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Xml;

namespace Bakera.Hatomaru{

/// <summary>
/// HatomaruDocの指定されたセクションを表示するクラスです。
/// </summary>
	public partial class DocViewSection : DocAction{

		protected string mySectionId;

// コンストラクタ

		public DocViewSection(HatomaruDoc model, AbsPath path, string sectionId) : base(model, path){
			mySectionId = sectionId;
			myPath = myModel.BasePath.Combine(sectionId);
		}


// プロパティ

		/// <summary>
		/// データを GET し、HatomaruResponse を取得します。
		/// </summary>
		protected override HatomaruResponse GetHtmlResponse(){
			DocTopic dt = Doc.GetTopicById(mySectionId);
			if(dt == null) return Redirect(myModel.BasePath);

			InsertHeading(2, dt.FullName);
			Html.Append(ParseNode(dt.SectionElement, 3));
			// FootNoteList があれば吐き出す

			Response.SelfTitle = dt.FullName;
			Response.AddTopicPath(myPath, dt.FullName);

			if(myFootNoteCount > 0){
				XmlElement fnDiv = Html.Div("footnotes");
				XmlElement fnH = Html.H(4, null, "注釈");
				fnDiv.AppendChild(fnH);
				fnDiv.AppendChild(ParseFootNoteList());
				Html.Append(fnDiv);
			}

			Html.Append(CommentLink(Path, dt.FullName));

			DocTopic prevTopic = Doc.GetPrevTopic(dt);
			if(prevTopic != null){
				Prev = new LinkItem(myModel.BasePath.Combine(prevTopic.Id), prevTopic.FullName);
			} else {
				Prev = new LinkItem(myModel.BasePath, "目次");
			}
			DocTopic nextTopic = Doc.GetNextTopic(dt);
			if(nextTopic != null){
				Next = new LinkItem(myModel.BasePath.Combine(nextTopic.Id), nextTopic.FullName);
			} else {
				Next = new LinkItem(myModel.BasePath, "目次");
			}
			return myResponse;
		}

	} // End class
} // End Namespace



