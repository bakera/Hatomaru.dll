using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace Bakera.Hatomaru{

/// <summary>
/// 最近の日記を表示するアクションです。
/// </summary>
	public partial class DiaryIndexViewAmazon : PartialDiaryAction{

		public const string Label = "Amazonリンク一覧";
		public const string Id = "amazon";

// コンストラクタ

		/// <summary>
		/// 最近の日記表示のためのアクションのインスタンスを開始します。
		/// </summary>
		public DiaryIndexViewAmazon(DiaryIndex model, AbsPath path) : base(model, path){
			myPath = myModel.BasePath.Combine(Id);
		}


		/// <summary>
		/// データを GET し、HatomaruResponse を取得します。
		/// </summary>
		protected override HatomaruResponse GetHtmlResponse(){
			Topic[] topics = GetAllTopics();
			if(topics.Length == 0) return NotFound();

			InsertHeading(2, Label);
			Response.SelfTitle = Label;
			Response.AddTopicPath(Path, Label);

			XmlElement amazonDesc = Html.P();
			XmlElement amazonTopLink = Html.A(new Uri(AmazonManager.AmazonTopUrl));
			amazonTopLink.InnerText = "amazon.co.jp";
			amazonDesc.AppendChild(amazonTopLink);
			amazonDesc.AppendChild(Html.Text(" へのリンクの一覧です。基本的には管理用のものなので、一般の読者の方が見ても意味はないかもしれません。"));
			Html.Append(amazonDesc);

			XmlElement topicUl = null;
			DateTime currentDate = default(DateTime);
			for(int i=0; i < topics.Length; i++){
				Topic t = topics[i];
				XmlNodeList amazonList = t.Message.GetElementsByTagName(AmazonElement);
				XmlNodeList amazonInfoList = t.Message.GetElementsByTagName(AmazonInfoElement);
				List<string> asinList = new List<string>();
				foreach(XmlElement e in amazonList){
					string code = e.GetAttributeValue(CodeAttribute);
					if(!string.IsNullOrEmpty(code)) asinList.Add(code);
				}
				foreach(XmlElement e in amazonInfoList){
					string code = e.GetAttributeValue(CodeAttribute);
					if(!string.IsNullOrEmpty(code)) asinList.Add(code);
				}
				if(asinList.Count == 0) continue;
				if(t.Date != currentDate){
					Html.Append(GetDateHeading(t.Date, 3));
					currentDate = t.Date;
					topicUl = Html.Create("ul");
					Html.Append(topicUl);
 				}

				XmlElement topicLi = Html.Create("li");
				topicLi.AppendChild(MakeTopicAnchor(t));
				XmlElement ul = Html.Create("ul");
				foreach(string asin in asinList){
					XmlElement li = Html.Create("li");
					AmazonItem item = Model.Manager.AmazonManager.GetItem(asin);
					if(item != null){
						XmlElement a = Html.Create("a");
						a.SetAttribute("href", item.DetailPageUrl);
						a.InnerText = item.Title;
						li.AppendChild(a);
						li.AppendChild(Html.Space);
					}
					li.AppendChild(Html.Text(asin));
					ul.AppendChild(li);
				}
				topicLi.AppendChild(ul);
				topicUl.AppendChild(topicLi);
			}
			return Response;
		}




	} // End class
} // End Namespace Bakera



