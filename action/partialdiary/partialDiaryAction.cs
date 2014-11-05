using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Xml;

namespace Bakera.Hatomaru{

/// <summary>
/// 日記を制御するクラスです。
/// </summary>
	public abstract class PartialDiaryAction : HatomaruGetAction{

		public const string YearFormat = "yyyy年";
		public const string MonthFormat = "yyyy年M月";
		public const string DateFormat = "yyyy年M月d日(dddd)";
		public const string UpdatedFormat = "yyyy年M月d日H時m分";
		public const string UpdatedFormatShort = "yyyy年M月d日";
		public const string DateNoteFormat = "(M月d日)";


// コンストラクタ

		protected PartialDiaryAction(HatomaruXml model, AbsPath path) : base(model, path){}

// プロパティ
		/// <summary>
		/// モデルを YearDiary 形式で取得します。
		/// </summary>
		public YearDiary YearDiary{
			get{
				return myModel as YearDiary;
			}
		}

		/// <summary>
		/// DiaryIndexを取得します。
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


// 静的メソッド
		public static void SetReplaceUrl(Xhtml html){}


// プロテクトメソッド

		// 日記のサブナビの LinkItem を取得します。
		protected override LinkItem[] GetSubNav(){
			return new LinkItem[]{
				new LinkItem(Diary.BasePath.Combine(DiaryIndexViewUpdated.Id), DiaryIndexViewUpdated.Label),
				new LinkItem(Diary.BasePath.Combine(DiaryIndexViewBackNumber.Id), DiaryIndexViewBackNumber.Label),
				new LinkItem(Diary.BasePath.Combine(DiaryIndexViewGenre.Id), DiaryIndexViewGenre.Label),
			};
		}




// Topicの取得
// Modelに実装されていないのはAddDataSourceがあるため

		// すべてのTopicを取得し、DataSource を追加します。
		// 複数の YearDiaryにまたがって取得できます。
		protected Topic[] GetAllTopics(){
			List<Topic> topics = new List<Topic>();
			foreach(YearDiary yd in Diary.DiaryList){
				topics.AddRange(yd.GetAllTopics());
				myResponse.AddDataSource(yd);
			}
			return topics.ToArray();
		}

		// 最近のTopicを取得し、DataSource を追加します。
		// 複数の YearDiaryにまたがって取得できます。
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


		// 更新されたTopicを取得し、DataSource を追加します。
		// 複数の YearDiaryにまたがって取得できます。
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
		/// 次の日付を取得します。
		/// 渡された日付よりも一つ新しい日付を取得します。
		/// </summary>
		protected DateTime GetNextDate(DateTime date){
			// 古い順に見ていく
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
		/// 前の日付を取得します。
		/// 渡された日付よりも一つ古い日付を取得します。
		/// </summary>
		protected DateTime GetPrevDate(DateTime date){
			// 新しい順に見ていく
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
		/// 次のトピックを取得します。
		/// 渡されたIDよりもIDの大きいトピックを取得します。
		/// </summary>
		protected Topic GetNextTopic(int id){
			// 古い順に見ていく
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
		/// 前のトピックを取得します。
		/// 渡されたIDよりもIDの大きいトピックを取得します。
		/// </summary>
		protected Topic GetPrevTopic(int id){
			// 新しい順に見ていく
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
		/// 指定されたジャンルに属する topic を取得します。
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
		/// ジャンルをすべて取得します。
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

		// 特定のトピックにリンクするアンカーを作成します。
		protected XmlNode MakeTopicAnchor(Topic t){
			AbsPath linkPath = Diary.BasePath.Combine(DiaryIndexViewTopic.Id, t.Id);
			XmlElement result = Html.A(linkPath);
			result.InnerText = t.Title;
			return result;
		}

		// 日付にリンクするアンカーを作成します。
		protected XmlNode MakeDateAnchor(DateTime d){
			AbsPath linkPath = Diary.BasePath.Combine(d.Year, d.Month, d.Day);
			XmlElement result = Html.A(linkPath);
			result.InnerText = d.ToString(DateFormat);
			return result;
		}

		// 月にリンクするアンカーを作成します。
		protected XmlNode MakeMonthAnchor(DateTime d){
			AbsPath linkPath = Diary.BasePath.Combine(d.Year, d.Month);
			XmlElement result = Html.A(linkPath);
			result.InnerText = d.ToString(MonthFormat);
			return result;
		}

		/// <summary>
		/// 日付をつけて見出し一覧を出力します。
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
		/// 日記本文に日付の見出しをつけた形式で一覧を出力します。
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
		/// 日記本文を出力します。
		/// </summary>
		protected XmlElement GetTopicBody(Topic t, int baseLevel){
			if(baseLevel < 1 || baseLevel > 6){
				throw new ArgumentException(String.Format("見出しレベル {0} が指定されました。指定できる見出しのレベルは 1～6 です。", baseLevel));
			}
			XmlElement result = Html.Create("div", "topic");
			XmlElement topicHeading = Html.H(baseLevel, null, MakeTopicAnchor(t));
			result.AppendChild(topicHeading);
			result.AppendChild(GetSubInfo(t));
			result.AppendChild(ParseNode(t.Message.ChildNodes, baseLevel + 1));
			result.AppendChild(GetAmazonImageList());
			result.AppendChild(CommentLink(t));
			result.AppendChild(Html.P("genre", Html.Span("genre", "関連する話題: ", GetGenreLinks(t))));
			return result;
		}

		/// <summary>
		/// 更新日を表示します。
		/// </summary>
		protected XmlNode GetUpdated(Topic t){
			if(t.Updated != default(DateTime)){
				XmlElement result = Html.Span("updated");
				result.InnerText = "更新: " + GetDateStr(t.Updated);
				return result;
			}
			if(t.Created != default(DateTime)){
				XmlElement result = Html.Span("updated");
				result.InnerText = "公開: " + GetDateStr(t.Created);
				return result;
			}
			return Html.Null;
		}

		// 更新日の文字列を得ます
		// 00:00の場合は日付のみ返します。
		private string GetDateStr(DateTime t){
			if(t.Hour == 0 && t.Minute == 0) return t.ToString(UpdatedFormatShort);
			return t.ToString(UpdatedFormat) + "頃";
		}

// 見出しとリンク

		/// <summary>
		/// DateTime とレベルを指定して月の見出しを出力します。
		/// </summary>
		protected XmlElement GetMonthHeading(DateTime d, int baseLevel){
			return Html.H(baseLevel, null, MakeMonthAnchor(d));
		}
		/// <summary>
		/// DateTime とレベルを指定して日付の見出しを出力します。
		/// </summary>
		protected XmlElement GetDateHeading(DateTime d, int baseLevel){
			return Html.H(baseLevel, null, MakeDateAnchor(d));
		}

		/// <summary>
		/// 日記コメントへのリンクを取得します。
		/// </summary>
		protected XmlNode CommentLink(Topic t){
			return CommentLink(Diary.BasePath.Combine(DiaryIndexViewTopic.Id, t.Id), t.Title);
		}

		/// <summary>
		/// 更新日へのリンクを取得します。
		/// </summary>
		protected XmlNode GetSubInfo(Topic t){
			XmlNode result = Html.P("subinfo");
			result.AppendChild(GetUpdated(t));
			return result;
		}

		/// <summary>
		/// prefix, セパレータ,suffix を指定してジャンルへのリンクを取得します。
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



