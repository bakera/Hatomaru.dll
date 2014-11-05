using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;

namespace Bakera.Hatomaru{

/// <summary>
/// Action �̃x�[�X�N���X�ł��B
/// </summary>
	public abstract partial class HatomaruActionBase{


// �i�r�Q�[�V�����n


// virtual ���\�b�h
		// ���_�C���N�g����ׂ����ǂ������`�F�b�N���܂��B
		// ���_�C���N�g����ׂ��ł���� true ��Ԃ��APath �̒l���g�p���ă��_�C���N�g���܂��B
		// ���_�C���N�g���s��Ȃ��ꍇ�́A���̃��\�b�h�� override ���ď�� false ��Ԃ��悤�ɂ��܂��B
		protected virtual bool CheckRedirect(){
			return Path != UserPath;
		}


// ���^���

		/// <summary>
		/// �^�C�g����Html �ɔ��f���܂��B
		/// �^�C�g���������ݒ肳��Ă��Ȃ��ꍇ�AModel.BaseTitle ��ݒ肵�܂��B
		/// BaseTitle �������ݒ肳��Ă��Ȃ��ꍇ�AModel.BaseTitle ��ݒ肵�܂��B
		/// </summary>
		protected virtual void SetTitle(HatomaruResponse hr){
			if(hr.SelfTitle == null){
				hr.SelfTitle = Model.BaseTitle;
				if(Model.ParentXml != null) hr.BaseTitle = Model.ParentXml.BaseTitle;
			} else if(hr.BaseTitle == null){
				hr.BaseTitle = Model.BaseTitle;
			}
			hr.Html.Title.InnerText = hr.Title;
			if(hr.BaseTitle != null){
				hr.Html.H1.InnerText = hr.BaseTitle;
			} else {
				hr.Html.H1.InnerText = Model.BaseTitle;
			}
		}


		/// <summary>
		/// �L�[���[�h���擾���܂��B
		/// </summary>
		protected virtual string GetKeywords(){
			return Model.Keywords;
		}

		/// <summary>
		/// �L�[���[�h��Html �ɔ��f���܂��B
		/// </summary>
		protected virtual void SetKeywords(HatomaruResponse hr, string keywords){
			if(!string.IsNullOrEmpty(keywords)){
				hr.Keywords = keywords;
				XmlElement metaKeywords = hr.Html.Create("meta");
				metaKeywords.SetAttribute("name", "keywords");
				metaKeywords.SetAttribute("content", keywords);
				hr.Html.Head.AppendChild(metaKeywords);
			}
		}



// �i�r�Q�[�V����

		/// <summary>
		/// TopicPath ��ݒ肵�܂��B
		/// Model �܂ł� Path �ƁA�ǉ����ꂽ LinkItem �ƂŃ��X�g���쐬���܂��B
		/// </summary>
		protected void SetTopicPath(HatomaruResponse hr){
			List<LinkItem> tempList = new List<LinkItem>();
			HatomaruXml currentXml = Model;
			for(;;){
				if(currentXml == null) break;
				hr.AddDataSource(currentXml);
				LinkItem item = currentXml.GetLinkItem();
				tempList.Add(item);
				currentXml = currentXml.ParentXml;
			}
			tempList.Reverse();
			if(hr.TopicPath != null) tempList.AddRange(hr.TopicPath);
			XmlNode result = null;
			if(tempList.Count > 1){
				result = Html.P(TopicPathClassName);
				for(int i = 0; i < tempList.Count; i++){
					LinkItem item = tempList[i];
					if(i > 0){
						result.AppendChild(Html.Text(" "));
						XmlElement sep = Html.Create("span", "separate", ">");
						result.AppendChild(sep);
						result.AppendChild(Html.Text(" "));
					}
					XmlElement a = Html.A(item.Path, null, item.InnerText);
					result.AppendChild(a);
				}
			}
			hr.Html.Replace(TopicPathPlaceHolderName, result);
		}


		/// <summary>
		/// �i�r�Q�[�V������ݒ肵�܂��B
		/// </summary>
		protected virtual void SetNavigation(HatomaruResponse hr){
			SetChildrenNav(GetSubNav());
			SetRecentlyArticle();
			// Prev/Next��ݒ肵�܂��B
			hr.Html.Append(GetPrevNextNav());
		}


		/// <summary>
		/// ���[�J���i�r�Q�[�V�������Z�b�g���܂��B
		/// </summary>
		protected void SetChildrenNav(LinkItem[] links){
			XmlElement result = Html.Div("local-nav");
			XmlElement resultUl = Html.Create("ul");
			XmlElement resultLi = Html.Create("li");
			result.AppendChild(resultUl);
			resultUl.AppendChild(resultLi);

			LinkItem item = Model.GetLinkItem();
			XmlElement a = Html.A(item.Path, null, item.InnerText);
			resultLi.AppendChild(a);

			if(links != null && links.Length > 0){
				XmlElement childUl = Html.Create("ul");
				for(int i=0; i < links.Length; i++){
					XmlElement childLi = Html.Create("li", null, Html.GetA(links[i]));
					childUl.AppendChild(childLi);
				}
				resultLi.AppendChild(childUl);
			}
			Html.Replace(NavName, result);
		}


		/// <summary>
		/// �ŋ߂̋L���ꗗ���Z�b�g���܂��B
		/// ����ł̓R���e���c�ɂ�����炸�A�ŋ߂̓��L���Z�b�g���܂��B
		/// </summary>
		protected void SetRecentlyArticle(){
			DiaryIndex d = Model.Manager.Diary;
			if(d == null) return;

			Topic firstTopic = d.DiaryList[0].GetLatestTopic();
			List<Topic> topicsList = new List<Topic>();
			for(int i = 0; i < d.DiaryList.Length; i++){
				YearDiary yd = d.DiaryList[i];
				Topic[] topics = yd.GetRecentTopics(firstTopic);
				if(topics.Length == 0) break;
				topicsList.AddRange(topics);
				myResponse.AddDataSource(yd);
				if(topics.Length < yd.Length) break;
			}
			if(topicsList.Count == 0) return;

			XmlElement result = Html.Div("recently-topics");
			result.AppendChild(Html.P(null, "�ŋ߂̓��L"));
			XmlElement childUl = Html.Create("ul");
			foreach(Topic t in topicsList){
				AbsPath linkPath = d.BasePath.Combine(DiaryIndexViewTopic.Id, t.Id);
				XmlElement a = Html.A(linkPath);
				a.InnerText = t.Title;
				XmlElement childLi = Html.Create("li", null, a);
				childUl.AppendChild(childLi);
			}
			result.AppendChild(childUl);
			Html.Replace(RecentlyTopicsName, result);
		}
 

		/// <summary>
		/// �q�̃i�r�Q�[�V������\�� LinkItem �̔z����擾���܂��B
		/// ����ł́A��̔z���Ԃ��܂��B
		/// �h���N���X�ŃI�[�o�[���C�h���܂��B
		/// </summary>
		protected virtual LinkItem[] GetSubNav(){
			return null;
		}



		/// <summary>
		/// ���݂̃R���e���c�Ƀy�[�W�i�r�Q�[�V�������Z�b�g���܂��B
		/// </summary>
		protected void InsertPageNav(Pager pg, AbsPath path){
			if(myPageNav == null){
				myPageNav = pg.GetPageNav(Html, path);
			}
			Html.Entry.AppendChild(myPageNav.Clone());
		}


		/// <summary>
		/// ���݂̃R���e���c�Ɍ��o����}�����܂��B
		/// </summary>
		protected void InsertHeading(int level, string text){
			XmlElement titleHeading = Html.H(level, null, text);
			Html.Entry.AppendChild(titleHeading);
		}

		/// <summary>
		/// Prev / Next �ւ̃����N��ݒ肵�܂��B
		/// </summary>
		protected virtual XmlNode GetPrevNextNav(){
			if(Prev == null && Next == null) return Html.CreateDocumentFragment();

			XmlElement ul = Html.Create("ul");
			if(Prev != null){
				XmlElement prevA = Html.GetA(Prev);
				prevA.SetAttribute("rel", "prev");
				ul.AppendChild(Html.Create("li", "prev", "�O(�Â�): ", prevA));
 				Html.AddLinkRel("prev", "text/html", myPrev);
 			}
			if(Next != null){
				XmlElement nextA = Html.GetA(Next);
				nextA.SetAttribute("rel", "next");
				ul.AppendChild(Html.Create("li", "next", "��(�V����): ", nextA));
				Html.AddLinkRel("next", "text/html", myNext);
			}
			return Html.Create("div", "prevnext", ul);
		}




	} // End class
} // End Namespace



