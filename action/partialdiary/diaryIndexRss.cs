using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace Bakera.Hatomaru{

/// <summary>
/// RSSを表示するアクションです。
/// </summary>
	public partial class DiaryIndexRss : PartialDiaryAction{

		public const string Id = "rss";

		public const string Rss1NameSpace = "http://purl.org/rss/1.0/";
		public const string RdfNameSpace = "http://www.w3.org/1999/02/22-rdf-syntax-ns#";
		public const string DublinCoreNameSpace = "http://purl.org/dc/elements/1.1/";
		public const string DublinCoreDateFormat = "yyyy-MM-dd";

// コンストラクタ

		/// <summary>
		/// RSS表示のためのアクションのインスタンスを開始します。
		/// </summary>
		public DiaryIndexRss(DiaryIndex model, AbsPath path) : base(model, path){
			myPath = myModel.BasePath.Combine(Id);
		}


		/// <summary>
		/// データを GET し、HatomaruResponse を取得します。
		/// </summary>
		protected override HatomaruResponse GetHtmlResponse(){
			Topic[] topics = GetRecentTopics();
			if(topics.Length == 0) return NotFound();

			string urlBase = "http://" + Model.Manager.IniData.Domain;
			string rssUrl = urlBase + Path.ToString();
			string diaryUrl = urlBase + myModel.BasePath.ToString();

			// 準備と last-modified の移植
			XmlResponse xr = new XmlResponse(Model);
			foreach(HatomaruData hd in myResponse.DataSource){
				xr.AddDataSource(hd);
			}
			xr.SetLastModified();

			XmlDocument rss = xr.Document;
			XmlElement rssRoot = rss.CreateElement("rdf", "RDF", RdfNameSpace);
			rssRoot.SetAttribute("xmlns", Rss1NameSpace);
			rssRoot.SetAttribute("xmlns:dc", DublinCoreNameSpace);
			rss.AppendChild(rssRoot);

			XmlElement channel = rss.CreateElement("channel", Rss1NameSpace);
			channel.SetAttribute("about", RdfNameSpace, rssUrl);
			rssRoot.AppendChild(channel);

			XmlElement title = rss.CreateElement("title", Rss1NameSpace);
			title.InnerText = DiaryIndexViewRecently.Label;
			channel.AppendChild(title);

			XmlElement link = rss.CreateElement("link", Rss1NameSpace);
			link.InnerText = diaryUrl;
			channel.AppendChild(link);

			XmlElement description = rss.CreateElement("description", Rss1NameSpace);
			description.InnerText = Diary.Description;
			channel.AppendChild(description);

			XmlElement language = rss.CreateElement("dc", "language", DublinCoreNameSpace);
			language.InnerText = "ja";
			channel.AppendChild(language);

			XmlElement dcDate = rss.CreateElement("dc", "date", DublinCoreNameSpace);
			dcDate.InnerText = Model.LastModified.ToString(DublinCoreDateFormat);
			channel.AppendChild(dcDate);

			XmlElement items = rss.CreateElement("items", Rss1NameSpace);
			channel.AppendChild(items);

			XmlElement seq = rss.CreateElement("rdf", "Seq", RdfNameSpace);
			items.AppendChild(seq);


			// 最新のトピックを取得
			foreach(Topic t in topics){
				string topicUrl = urlBase + Diary.BasePath.Combine(DiaryIndexViewTopic.Id, t.Id).ToString();

				// Seq に追加
				XmlElement li = rss.CreateElement("rdf", "li", RdfNameSpace);
				li.SetAttribute("resource", RdfNameSpace, topicUrl);
				seq.AppendChild(li);

				// item を追加
				XmlElement item = rss.CreateElement("item", Rss1NameSpace);
				item.SetAttribute("about", RdfNameSpace, topicUrl);
				rssRoot.AppendChild(item);

				XmlElement itemTitle = rss.CreateElement("title", Rss1NameSpace);
				itemTitle.InnerText = t.Title;
				item.AppendChild(itemTitle);

				XmlElement itemLink = rss.CreateElement("link", Rss1NameSpace);
				itemLink.InnerText = topicUrl;
				item.AppendChild(itemLink);

				XmlElement itemDescription = rss.CreateElement("description", Rss1NameSpace);
				itemDescription.InnerText = t.Message.InnerText.Truncate(150);
				item.AppendChild(itemDescription);

				XmlElement itemDate = rss.CreateElement("dc", "date", DublinCoreNameSpace);
				itemDate.InnerText = t.Date.ToString(DublinCoreDateFormat);
				item.AppendChild(itemDate);
			}

			return xr;
		}


	} // End class
} // End Namespace Bakera



