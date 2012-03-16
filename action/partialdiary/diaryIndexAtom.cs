using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Xml;

namespace Bakera.Hatomaru{

/// <summary>
/// RSSを表示するアクションです。
/// </summary>
	public partial class DiaryIndexAtom : PartialDiaryAction{

		public const string Id = "atom";
		
		public const string AtomContentType = "application/atom+xml";
		public const string AtomNameSpace = "http://www.w3.org/2005/Atom";
		public const string AtomDateFormat = "yyyy'-'MM'-'dd'T'HH':'mm':'sszzz";
		public const string IdTagFormat = "tag:{0},2008:{1}";

// コンストラクタ


		/// <summary>
		/// Atom表示のためのアクションのインスタンスを開始します。
		/// </summary>
		public DiaryIndexAtom(DiaryIndex model, AbsPath path) : base(model, path){
			myPath = myModel.BasePath.Combine(Id);
		}



		/// <summary>
		/// データを GET し、HatomaruResponse を取得します。
		/// </summary>
		protected override HatomaruResponse GetHtmlResponse(){
			Topic[] topics = GetRecentTopics();
			if(topics.Length == 0) return NotFound();
			return GetAtom(topics, DiaryIndexViewRecently.Label);
		}


		protected XmlResponse GetAtom(Topic[] topics, string feedTitle){
			string urlBase = "http://" + Model.Manager.IniData.Domain;
			string diaryUrl = urlBase + Model.BasePath.ToString();
			string selfUrl = urlBase + Model.BasePath.Combine(Id).ToString();
			string idTagBase = string.Format(IdTagFormat, Model.Manager.IniData.Domain, Diary.BasePath);

			// 準備と last-modified の移植
			XmlResponse xr = new XmlResponse(Model);
			foreach(HatomaruData hd in myResponse.DataSource){
				xr.AddDataSource(hd);
			}
			xr.SetLastModified();

			XmlDocument atom = xr.Document;
			XmlElement feed = atom.CreateElement("feed", AtomNameSpace);
			atom.AppendChild(feed);

			XmlElement title = atom.CreateElement("title", AtomNameSpace);
			title.InnerText = feedTitle;
			feed.AppendChild(title);

			XmlElement link = atom.CreateElement("link", AtomNameSpace);
			link.SetAttribute("href", diaryUrl);
			feed.AppendChild(link);

			XmlElement selfLink = atom.CreateElement("link", AtomNameSpace);
			selfLink.SetAttribute("href", selfUrl);
			selfLink.SetAttribute("rel", "self");
			selfLink.SetAttribute("type", HatomaruResponse.AtomMediaType);
			feed.AppendChild(selfLink);

			XmlElement updated = atom.CreateElement("updated", AtomNameSpace);
			updated.InnerText = xr.LastModified.ToString(AtomDateFormat);
			feed.AppendChild(updated);

			XmlElement author = atom.CreateElement("author", AtomNameSpace);
			feed.AppendChild(author);

			XmlElement name = atom.CreateElement("name", AtomNameSpace);
			name.InnerText = "水無月ばけら";
			author.AppendChild(name);

			XmlElement id = atom.CreateElement("id", AtomNameSpace);
			id.InnerText = idTagBase;
			feed.AppendChild(id);

			// 最新のトピック
			foreach(Topic t in topics){
				AbsPath topicPath = Diary.BasePath.Combine(DiaryIndexViewTopic.Id, t.Id);
				string topicUrl = urlBase + topicPath.ToString();
				string topicIdTag = string.Format(IdTagFormat, Model.Manager.IniData.Domain, topicPath);

				XmlElement entry = atom.CreateElement("entry", AtomNameSpace);
				feed.AppendChild(entry);

				XmlElement entryTitle = atom.CreateElement("title", AtomNameSpace);
				entryTitle.InnerText = t.Title;
				entry.AppendChild(entryTitle);

				XmlElement entryLink = atom.CreateElement("link", AtomNameSpace);
				entryLink.SetAttribute("href", topicUrl);
				entry.AppendChild(entryLink);

				XmlElement entryId = atom.CreateElement("id", AtomNameSpace);
				entryId.InnerText = topicIdTag;
				entry.AppendChild(entryId);

				XmlElement entryUpdated = atom.CreateElement("updated", AtomNameSpace);
				DateTime topicUpdated = t.Date;
				if(t.Updated != default(DateTime)){
					topicUpdated = t.Updated;
				} else if(t.Created  != default(DateTime)){
					topicUpdated = t.Created;
				}
				entryUpdated.InnerText = topicUpdated.ToString(AtomDateFormat);
				entry.AppendChild(entryUpdated);

				XmlElement content = atom.CreateElement("content", AtomNameSpace);
				content.SetAttribute("type", "xhtml");
				content.SetAttribute("xml:lang", "ja");
				content.SetAttribute("xml:base", selfUrl);

				XmlElement contentDiv = atom.CreateElement("div", Xhtml.NameSpace);

				XmlNode topicHeading = GetDateHeading(t.Date, 1);
				XmlNode topicBody = GetTopicBody(t, 2);

				XmlNode atomTopicHeading = atom.ImportNode(topicHeading, true);
				XmlNode atomTopicBody = atom.ImportNode(topicBody, true);

				contentDiv.AppendChild(atomTopicHeading);
				contentDiv.AppendChild(atomTopicBody);

				content.AppendChild(contentDiv);

				entry.AppendChild(content);
			}
			return xr;
		}

	} // End class
} // End Namespace Bakera



