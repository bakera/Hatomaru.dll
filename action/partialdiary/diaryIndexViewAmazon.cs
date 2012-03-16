using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace Bakera.Hatomaru{

/// <summary>
/// �ŋ߂̓��L��\������A�N�V�����ł��B
/// </summary>
	public partial class DiaryIndexViewAmazon : PartialDiaryAction{

		public const string Label = "Amazon�����N�ꗗ";
		public const string Id = "amazon";

// �R���X�g���N�^

		/// <summary>
		/// �ŋ߂̓��L�\���̂��߂̃A�N�V�����̃C���X�^���X���J�n���܂��B
		/// </summary>
		public DiaryIndexViewAmazon(DiaryIndex model, AbsPath path) : base(model, path){
			myPath = myModel.BasePath.Combine(Id);
		}


		/// <summary>
		/// �f�[�^�� GET ���AHatomaruResponse ���擾���܂��B
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
			amazonDesc.AppendChild(Html.Text(" �ւ̃����N�̈ꗗ�ł��B��{�I�ɂ͊Ǘ��p�̂��̂Ȃ̂ŁA��ʂ̓ǎ҂̕������Ă��Ӗ��͂Ȃ���������܂���B"));
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



