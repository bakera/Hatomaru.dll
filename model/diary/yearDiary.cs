using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Reflection;
using System.Xml;

namespace Bakera.Hatomaru{

/// <summary>
/// 1年分の日記を処理するクラスです。
/// </summary>
	public partial class YearDiary : HatomaruXml{

		new public const string Name = "year-diary";

		public static readonly TimeSpan RecentSpan = new TimeSpan(7, 0, 0, 0);
		private Dictionary<string, TopicGenre> myGenreDic; // ジャンルの Dictionary
		private int myMaxId = Int32.MinValue; // 最大の ID = 最新記事のID
		private int myMinId = Int32.MaxValue; // 最小の ID
		private DiaryTable myTable;
		private DateTime[] myDates; // すべての日付を古い順に格納
		private const string DateSortString = DiaryTable.DateColName + " DESC";
		private const string DateReverseSortString = DiaryTable.DateColName;

		/// <summary>
		/// 鳩丸データのデータソースの FileInfo と XmlDocument を指定して、HatomaruBbs のインスタンスを開始します。
		/// </summary>
		public YearDiary(HatomaruManager manager, FileInfo f, XmlDocument x) : base(manager, f, x){
			Load();
		}


// プロパティ
		public int Year{
			get{return Dates[0].Year;}
		}
		public DateTime[] Dates{
			get{return myDates;}
		}
		public int Length{
			get{return myTable.Rows.Count;}
		}

// データ取得メソッド

		/// <summary>
		/// すべての Topic を新しい順に取得します。
		/// </summary>
		public Topic[] GetAllTopics(){
			DataRow[] dr = myTable.Select();
			return DataRowsToTopicsReverse(dr);
		}

		/// <summary>
		/// 最近の Topic を取得します。
		/// 引数として「最近」の基準となるTopicを受け取ります。
		/// </summary>
		public Topic[] GetRecentTopics(Topic latestTopic){
			DateTime recentDate = latestTopic.Date - YearDiary.RecentSpan;
			DateTime recentCreatedDate = latestTopic.Created - YearDiary.RecentSpan;
			string selectString = String.Format("[{0}]>{1} OR [{2}]>{3}", DiaryTable.DateColName, recentDate.Ticks, DiaryTable.CreatedColName, recentCreatedDate.Ticks);
			return GetTopicsBySelectString(selectString, DateSortString);
		}

		/// <summary>
		/// 最新の Topic 1件を取得します。
		/// </summary>
		public Topic GetLatestTopic(){
			return GetTopic(myMaxId);
		}

		/// <summary>
		/// 最古の Topic 1件を取得します。
		/// </summary>
		public Topic GetOldestTopic(){
			return GetTopic(myMinId);
		}


		/// <summary>
		/// 更新された Topic を取得します。
		/// </summary>
		public Topic[] GetUpdatedTopics(){
			List<Topic> updated = new List<Topic>();
			foreach(Topic t in GetAllTopics()){
				if(t.Updated > t.Date) updated.Add(t);
			}
			updated.Sort(Topic.CompareByUpdated);
			return updated.ToArray();
		}

		// Select 文字列を指定して Topic を取得します。
		private Topic[] GetTopicsBySelectString(string selectString, string sortString){
			DataRow[] dr = myTable.Select(selectString, sortString);
			return DataRowsToTopics(dr);
		}

		// Row[] を Topic[] に変換します。
		private Topic[] DataRowsToTopics(DataRow[] rows){
			Topic[] result = new Topic[rows.Length];
			for(int i = 0; i < rows.Length; i++){
				result[i] = GetTopic(rows[i]);
			}
			return result;
		}

		// Row[] を Topic[] に変換し、ついでに逆順にします。
		private Topic[] DataRowsToTopicsReverse(DataRow[] rows){
			Topic[] result = new Topic[rows.Length];
			for(int i = 0; i < rows.Length; i++){
				int idx = rows.Length - 1 - i;
				result[i] = GetTopic(rows[idx]);
			}
			return result;
		}

		/// <summary>
		/// DataRow から Topic を取得します。
		/// </summary>
		private Topic GetTopic(DataRow row){
			if(row == null) return null;
			return row[myTable.MessageCol] as Topic;
		}

// 最近の日記

		/// <summary>
		/// 最近の Topic の次の Topic を取得します。
		/// </summary>
		public Topic GetRecentNextTopic(){
			string selectString = String.Format("[{0}]<=Max([{0}])-{1}", DiaryTable.DateColName, RecentSpan.Ticks);
			Topic[] topics = GetTopicsBySelectString(selectString, DateSortString);
			if(topics.Length == 0) return null;
			return topics[0];
		}


// 特定番号の日記

		/// <summary>
		/// 特定番号の Topic を取得します。
		/// </summary>
		public Topic GetTopic(int num){
			if(num > myMaxId || num < myMinId) return null;
			return GetTopic(myTable.Rows.Find(num));
		}

		/// <summary>
		/// 指定された番号よりひとつ新しい Topic を取得します。
		/// </summary>
		public Topic GetNextTopic(int num){
			if(num >= myMaxId) return null;
			Topic t = GetTopic(num + 1);
			if(t != null) return t;
			//再帰
			return GetNextTopic(num + 1);
		}

		/// <summary>
		/// 指定された番号よりひとつ古い Topic を取得します。
		/// </summary>
		public Topic GetPrevTopic(int num){
			if(num <= myMinId) return null;
			Topic t = GetTopic(num - 1);
			if(t != null) return t;
			//再帰
			return GetPrevTopic(num - 1);
		}


// 特定日の日記

		/// <summary>
		/// 特定日の Topic を取得します。
		/// </summary>
		public Topic[] GetDayTopics(DateTime date){
			string selectString = String.Format("[{0}]={1}", DiaryTable.DateColName, date.Ticks);
			return GetTopicsBySelectString(selectString, DateSortString);
		}

		/// <summary>
		/// 特定期間の Topic を取得します。
		/// </summary>
		public Topic[] GetSpanTopics(DateTime start, DateTime end){
			string selectString = String.Format("[{0}]>={1} AND [{0}] < {2}", DiaryTable.DateColName, start.Ticks, end.Ticks);
			return GetTopicsBySelectString(selectString, DateSortString);
		}

		/// <summary>
		/// 前の日付を取得します。
		/// </summary>
		public DateTime GetPrevDate(DateTime date){
			int pos = Array.IndexOf<DateTime>(myDates, date);
			if(pos < 1) return default(DateTime);
			return myDates[pos-1];
		}

		/// <summary>
		/// 次の日付を取得します。
		/// </summary>
		public DateTime GetNextDate(DateTime date){
			int pos = Array.IndexOf<DateTime>(myDates, date);
			if(pos < 0) return default(DateTime);
			if(pos >= myDates.Length - 1) return default(DateTime);
			return myDates[pos + 1];
		}


		/// <summary>
		/// 指定されたジャンルに属する topic を取得します。
		/// </summary>
		public Topic[] GetTopicsByGenre(string genre){
			if(string.IsNullOrEmpty(genre))  return new Topic[0];
			TopicGenre topics = null;
			myGenreDic.TryGetValue(genre, out topics);
			if(topics == null)  return new Topic[0];
			return topics.Topics;
		}

		/// <summary>
		/// ジャンルをすべて取得します。
		/// </summary>
		public TopicGenre[] GetGenreList(){
			TopicGenre[] result = new TopicGenre[myGenreDic.Count];
			myGenreDic.Values.CopyTo(result, 0);
			Array.Sort(result, TopicGenre.GenreCountSort);
			return result;
		}




// データのロード
		/// <summary>
		/// XML ファイルからデータをロードし、table に格納します。
		/// </summary>
		public void Load(){
			XmlNodeList xnl = Document.GetElementsByTagName(Topic.TopicElementName);
			myTable = new DiaryTable();
			myTable.MinimumCapacity = xnl.Count;
			myGenreDic = new Dictionary<string, TopicGenre>();
			List<DateTime> dateList = new List<DateTime>();
			foreach(XmlElement e in xnl){
				Topic t = new Topic(e);
				// テーブルに追加
				myTable.AddTopic(t);
				// キーワードを追加
				foreach(string s in t.Genre){
					if(!myGenreDic.ContainsKey(s)) myGenreDic.Add(s, new TopicGenre(s));
					myGenreDic[s].Add(t);
				}
				// 日付を追加
				dateList.AddNew(t.Date);
				// IDを更新
				if(t.Id > myMaxId) myMaxId = t.Id;
				if(t.Id < myMinId) myMinId = t.Id;
			}
			dateList.Sort();
			myDates = dateList.ToArray();
		}






// オーバーライドメソッド

		/// <summary>
		/// データを GET し、HatomaruResponse を取得します。
		/// </summary>
		public override HatomaruResponse Get(AbsPath path){
			HatomaruGetAction act = GetAction(path);
			HatomaruResponse result = act.Get();
			result.SetLastModified();
			return result;
		}

		/// <summary>
		/// path を元に、適切なコントローラを作成します。
		/// </summary>
		private HatomaruGetAction GetAction(AbsPath path){
			string[] fragments = path.GetFragments(BasePath);
			if(fragments.Length > 0){
				string firstStr = fragments[0];
				int month = firstStr.ToInt32();
				if(month > 0){
					if(fragments.Length > 1){
						int day = fragments[1].ToInt32();
						if(day > 0) return new YearDiaryViewDate(this, path, month, day);
					}
					return new YearDiaryViewMonth(this, path, month);
				}
			}
			return new YearDiaryViewYear(this, path);
		}



	} // End class
} // End Namespace Bakera



