using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Xml;

namespace Bakera.Hatomaru{

/// <summary>
/// ���L�𐧌䂷��N���X�ł��B
/// </summary>
	public abstract class PartialDiaryAction : HatomaruGetAction{

		public const string YearFormat = "yyyy�N";
		public const string MonthFormat = "yyyy�NM��";
		public const string DateFormat = "yyyy�NM��d��(dddd)";
		public const string UpdatedFormat = "yyyy�NM��d��H��m��";
		public const string UpdatedFormatShort = "yyyy�NM��d��";
		public const string DateNoteFormat = "(M��d��)";


// �R���X�g���N�^

		protected PartialDiaryAction(HatomaruXml model, AbsPath path) : base(model, path){}

// �v���p�e�B
		/// <summary>
		/// ���f���� YearDiary �`���Ŏ擾���܂��B
		/// </summary>
		public YearDiary YearDiary{
			get{
				return myModel as YearDiary;
			}
		}

		/// <summary>
		/// DiaryIndex���擾���܂��B
		/// </summary>
		public DiaryIndex Diary{
			get{
				if(myModel is DiaryIndex) return myModel as DiaryIndex;
				return myModel.ParentXml as DiaryIndex;
			}
		}

		public int Year{
			get{return YearDiary.Dates[0].Year;}
		}


// �ÓI���\�b�h
		public static void SetReplaceUrl(Xhtml html){}


// �v���e�N�g���\�b�h

		// ���L�̃T�u�i�r�� LinkItem ���擾���܂��B
		protected override LinkItem[] GetSubNav(){
			return new LinkItem[]{
				new LinkItem(Diary.BasePath.Combine(DiaryIndexViewUpdated.Id), DiaryIndexViewUpdated.Label),
				new LinkItem(Diary.BasePath.Combine(DiaryIndexViewBackNumber.Id), DiaryIndexViewBackNumber.Label),
				new LinkItem(Diary.BasePath.Combine(DiaryIndexViewGenre.Id), DiaryIndexViewGenre.Label),
			};
		}




// Topic�̎擾
// Model�Ɏ�������Ă��Ȃ��̂�AddDataSource�����邽��

		// ���ׂĂ�Topic���擾���ADataSource ��ǉ����܂��B
		// ������ YearDiary�ɂ܂������Ď擾�ł��܂��B
		protected Topic[] GetAllTopics(){
			List<Topic> topics = new List<Topic>();
			foreach(YearDiary yd in Diary.DiaryList){
				topics.AddRange(yd.GetAllTopics());
				myResponse.AddDataSource(yd);
			}
			return topics.ToArray();
		}

		// �ŋ߂�Topic���擾���ADataSource ��ǉ����܂��B
		// ������ YearDiary�ɂ܂������Ď擾�ł��܂��B
		protected Topic[] GetRecentTopics(){
			Topic firstTopic = Diary.DiaryList[0].GetLatestTopic();
			List<Topic> result = new List<Topic>();
			for(int i = 0; i < Diary.DiaryList.Length; i++){
				YearDiary yd = Diary.DiaryList[i];
				Topic[] topics = yd.GetRecentTopics(firstTopic);
				if(topics.Length == 0) break;
				result.AddRange(topics);
				myResponse.AddDataSource(yd);
				if(topics.Length < yd.Length) break;
			}
			return result.ToArray();
		}


		// �X�V���ꂽTopic���擾���ADataSource ��ǉ����܂��B
		// ������ YearDiary�ɂ܂������Ď擾�ł��܂��B
		protected Topic[] GetUpdatedTopics(){
			List<Topic> topics = new List<Topic>();
			foreach(YearDiary yd in Diary.DiaryList){
				topics.AddRange(yd.GetUpdatedTopics());
				myResponse.AddDataSource(yd);
			}
			topics.Sort(Topic.CompareByUpdated);
			return topics.ToArray();
		}


		/// <summary>
		/// ���̓��t���擾���܂��B
		/// �n���ꂽ���t������V�������t���擾���܂��B
		/// </summary>
		protected DateTime GetNextDate(DateTime date){
			// �Â����Ɍ��Ă���
			for(int i = Diary.DiaryList.Length - 1; i >=0; i--){
				YearDiary yd = Diary.DiaryList[i];
				if(yd.Year < date.Year) continue;
				for(int j=0; j < yd.Dates.Length; j++){
					if(yd.Dates[j] > date){
						myResponse.AddDataSource(yd);
						return yd.Dates[j];
					}
				}
			}
			return default(DateTime);
		}

		/// <summary>
		/// �O�̓��t���擾���܂��B
		/// �n���ꂽ���t������Â����t���擾���܂��B
		/// </summary>
		protected DateTime GetPrevDate(DateTime date){
			// �V�������Ɍ��Ă���
			for(int i = 0; i < Diary.DiaryList.Length; i++){
				YearDiary yd = Diary.DiaryList[i];
				if(yd.Year > date.Year) continue;
				for(int j = yd.Dates.Length-1; j >=0; j--){
					if(yd.Dates[j] < date){
						myResponse.AddDataSource(yd);
						return yd.Dates[j];
					}
				}
			}
			return default(DateTime);
		}

		/// <summary>
		/// ���̃g�s�b�N���擾���܂��B
		/// �n���ꂽID����ID�̑傫���g�s�b�N���擾���܂��B
		/// </summary>
		protected Topic GetNextTopic(int id){
			// �Â����Ɍ��Ă���
			for(int i = Diary.DiaryList.Length - 1; i >=0; i--){
				YearDiary yd = Diary.DiaryList[i];
				Topic t = yd.GetNextTopic(id);
				if(t != null){
					myResponse.AddDataSource(yd);
					return t;
				}
			}
			return null;
		}

		/// <summary>
		/// �O�̃g�s�b�N���擾���܂��B
		/// �n���ꂽID����ID�̑傫���g�s�b�N���擾���܂��B
		/// </summary>
		protected Topic GetPrevTopic(int id){
			// �V�������Ɍ��Ă���
			for(int i = 0; i < Diary.DiaryList.Length; i++){
				YearDiary yd = Diary.DiaryList[i];
				Topic t = yd.GetPrevTopic(id);
				if(t != null){
					myResponse.AddDataSource(yd);
					return t;
				}
			}
			return null;
		}


		/// <summary>
		/// �w�肳�ꂽ�W�������ɑ����� topic ���擾���܂��B
		/// </summary>
		protected Topic[] GetTopicsByGenre(string genre){
			if(string.IsNullOrEmpty(genre)) return new Topic[0];
			List<Topic> result = new List<Topic>();
			foreach(YearDiary yd in Diary.DiaryList){
				result.AddRange(yd.GetTopicsByGenre(genre));
				myResponse.AddDataSource(yd);
			}
			return result.ToArray();
		}


		/// <summary>
		/// �W�����������ׂĎ擾���܂��B
		/// </summary>
		protected TopicGenre[] GetGenreList(){
			Dictionary<string, TopicGenre> myGenreDic = new Dictionary<string, TopicGenre>();
			foreach(Topic t in GetAllTopics()){
				foreach(string s in t.Genre){
					if(!myGenreDic.ContainsKey(s)) myGenreDic.Add(s, new TopicGenre(s));
					myGenreDic[s].Add(t);
				}
			}
			TopicGenre[] result = new TopicGenre[myGenreDic.Count];
			myGenreDic.Values.CopyTo(result, 0);
			Array.Sort(result, TopicGenre.GenreCountSort);
			return result;
		}






//

		// ����̃g�s�b�N�Ƀ����N����A���J�[���쐬���܂��B
		protected XmlNode MakeTopicAnchor(Topic t){
			AbsPath linkPath = Diary.BasePath.Combine(DiaryIndexViewTopic.Id, t.Id);
			XmlElement result = Html.A(linkPath);
			result.InnerText = t.Title;
			return result;
		}

		// ���t�Ƀ����N����A���J�[���쐬���܂��B
		protected XmlNode MakeDateAnchor(DateTime d){
			AbsPath linkPath = Diary.BasePath.Combine(d.Year, d.Month, d.Day);
			XmlElement result = Html.A(linkPath);
			result.InnerText = d.ToString(DateFormat);
			return result;
		}

		// ���Ƀ����N����A���J�[���쐬���܂��B
		protected XmlNode MakeMonthAnchor(DateTime d){
			AbsPath linkPath = Diary.BasePath.Combine(d.Year, d.Month);
			XmlElement result = Html.A(linkPath);
			result.InnerText = d.ToString(MonthFormat);
			return result;
		}

		/// <summary>
		/// ���t�����Č��o���ꗗ���o�͂��܂��B
		/// </summary>
		protected XmlNode DiaryHeadingList(Topic[] topics, int baseLevel){
			XmlDocumentFragment result = Html.CreateDocumentFragment();
	
			DateTime currentDate = default(DateTime);
			XmlElement ul = null;

			for(int i=0; i < topics.Length; i++){
				Topic t = topics[i];
				if(currentDate == default(DateTime) || (t.Date.Month != currentDate.Month || t.Date.Year != currentDate.Year)){
					if(ul != null) result.AppendChild(ul);
					result.AppendChild(GetMonthHeading(t.Date, baseLevel));
					ul = Html.Create("ul");
					currentDate = t.Date;
				}
				XmlElement li = Html.Create("li", null, MakeTopicAnchor(t), Html.Space, Html.Span("date", t.Date.ToString(DateNoteFormat)));
				ul.AppendChild(li);
			}
			if(ul != null) result.AppendChild(ul);
			return result;
		}

		/// <summary>
		/// ���L�{���ɓ��t�̌��o���������`���ňꗗ���o�͂��܂��B
		/// </summary>
		protected XmlNode DiaryListWithDate(Topic[] topics, int baseLevel){
			XmlDocumentFragment result = Html.CreateDocumentFragment();
			DateTime currentDate = default(DateTime);
			for(int i=0; i < topics.Length; i++){
				Topic t = topics[i];
				if(t.Date != currentDate){
					result.AppendChild(GetDateHeading(t.Date, baseLevel));
					currentDate = t.Date;
				}
				result.AppendChild(GetTopicBody(t, baseLevel + 1));
			}
			return result;
		}


		/// <summary>
		/// ���L�{�����o�͂��܂��B
		/// </summary>
		protected XmlElement GetTopicBody(Topic t, int baseLevel){
			if(baseLevel < 1 || baseLevel > 6){
				throw new ArgumentException(String.Format("���o�����x�� {0} ���w�肳��܂����B�w��ł��錩�o���̃��x���� 1�`6 �ł��B", baseLevel));
			}
			XmlElement result = Html.Create("div", "topic");
			XmlElement topicHeading = Html.H(baseLevel, null, MakeTopicAnchor(t));
			result.AppendChild(topicHeading);
			result.AppendChild(GetSubInfo(t));
			result.AppendChild(ParseNode(t.Message.ChildNodes, baseLevel + 1));
			result.AppendChild(GetAmazonImageList());
			result.AppendChild(CommentLink(t));
			result.AppendChild(Html.P("genre", Html.Span("genre", "�֘A����b��: ", GetGenreLinks(t))));
			return result;
		}

		/// <summary>
		/// �X�V����\�����܂��B
		/// </summary>
		protected XmlNode GetUpdated(Topic t){
			if(t.Updated != default(DateTime)){
				XmlElement result = Html.Span("updated");
				result.InnerText = "�X�V: " + GetDateStr(t.Updated);
				return result;
			}
			if(t.Created != default(DateTime)){
				XmlElement result = Html.Span("updated");
				result.InnerText = "���J: " + GetDateStr(t.Created);
				return result;
			}
			return Html.Null;
		}

		// �X�V���̕�����𓾂܂�
		// 00:00�̏ꍇ�͓��t�̂ݕԂ��܂��B
		private string GetDateStr(DateTime t){
			if(t.Hour == 0 && t.Minute == 0) return t.ToString(UpdatedFormatShort);
			return t.ToString(UpdatedFormat) + "��";
		}

// ���o���ƃ����N

		/// <summary>
		/// DateTime �ƃ��x�����w�肵�Č��̌��o�����o�͂��܂��B
		/// </summary>
		protected XmlElement GetMonthHeading(DateTime d, int baseLevel){
			return Html.H(baseLevel, null, MakeMonthAnchor(d));
		}
		/// <summary>
		/// DateTime �ƃ��x�����w�肵�ē��t�̌��o�����o�͂��܂��B
		/// </summary>
		protected XmlElement GetDateHeading(DateTime d, int baseLevel){
			return Html.H(baseLevel, null, MakeDateAnchor(d));
		}

		/// <summary>
		/// ���L�R�����g�ւ̃����N���擾���܂��B
		/// </summary>
		protected XmlNode CommentLink(Topic t){
			return CommentLink(Diary.BasePath.Combine(DiaryIndexViewTopic.Id, t.Id), t.Title);
		}

		/// <summary>
		/// �X�V���ւ̃����N���擾���܂��B
		/// </summary>
		protected XmlNode GetSubInfo(Topic t){
			XmlNode result = Html.P("subinfo");
			result.AppendChild(GetUpdated(t));
			return result;
		}

		/// <summary>
		/// prefix, �Z�p���[�^,suffix ���w�肵�ăW�������ւ̃����N���擾���܂��B
		/// </summary>
		protected XmlNode GetGenreLinks(Topic t){
			if(t.Genre == null || t.Genre.Length == 0) return Html.Null;
			XmlDocumentFragment result = Html.CreateDocumentFragment();
			for(int i = 0; i < t.Genre.Length; i++){
				if(i > 0) result.AppendChild(Html.Text(" / "));

				AbsPath linkPath = Diary.BasePath.Combine(DiaryIndexViewGenre.Id, t.Genre[i].PathEncode());
				XmlElement gLink = Html.A(linkPath);
				gLink.InnerText = t.Genre[i];
				result.AppendChild(gLink);
			}
			return result;
		}



	} // End class DiaryAction
} // End Namespace Bakera



