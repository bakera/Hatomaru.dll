using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Reflection;
using System.Xml;

namespace Bakera.Hatomaru{

/// <summary>
/// 1�N���̓��L����������N���X�ł��B
/// </summary>
	public partial class YearDiary : HatomaruXml{

		new public const string Name = "year-diary";

		public static readonly TimeSpan RecentSpan = new TimeSpan(7, 0, 0, 0);
		private Dictionary<string, TopicGenre> myGenreDic; // �W�������� Dictionary
		private int myMaxId = Int32.MinValue; // �ő�� ID = �ŐV�L����ID
		private int myMinId = Int32.MaxValue; // �ŏ��� ID
		private DiaryTable myTable;
		private DateTime[] myDates; // ���ׂĂ̓��t���Â����Ɋi�[
		private const string DateSortString = DiaryTable.DateColName + " DESC";
		private const string DateReverseSortString = DiaryTable.DateColName;

		/// <summary>
		/// ���ۃf�[�^�̃f�[�^�\�[�X�� FileInfo �� XmlDocument ���w�肵�āAHatomaruBbs �̃C���X�^���X���J�n���܂��B
		/// </summary>
		public YearDiary(HatomaruManager manager, FileInfo f, XmlDocument x) : base(manager, f, x){
			Load();
		}


// �v���p�e�B
		public int Year{
			get{return Dates[0].Year;}
		}
		public DateTime[] Dates{
			get{return myDates;}
		}
		public int Length{
			get{return myTable.Rows.Count;}
		}

// �f�[�^�擾���\�b�h

		/// <summary>
		/// ���ׂĂ� Topic ��V�������Ɏ擾���܂��B
		/// </summary>
		public Topic[] GetAllTopics(){
			DataRow[] dr = myTable.Select();
			return DataRowsToTopicsReverse(dr);
		}

		/// <summary>
		/// �ŋ߂� Topic ���擾���܂��B
		/// �����Ƃ��āu�ŋ߁v�̊�ƂȂ�Topic���󂯎��܂��B
		/// </summary>
		public Topic[] GetRecentTopics(Topic latestTopic){
			DateTime recentDate = latestTopic.Date - YearDiary.RecentSpan;
			DateTime recentCreatedDate = latestTopic.Created - YearDiary.RecentSpan;
			string selectString = String.Format("[{0}]>{1} OR [{2}]>{3}", DiaryTable.DateColName, recentDate.Ticks, DiaryTable.CreatedColName, recentCreatedDate.Ticks);
			return GetTopicsBySelectString(selectString, DateSortString);
		}

		/// <summary>
		/// �ŐV�� Topic 1�����擾���܂��B
		/// </summary>
		public Topic GetLatestTopic(){
			return GetTopic(myMaxId);
		}

		/// <summary>
		/// �ŌÂ� Topic 1�����擾���܂��B
		/// </summary>
		public Topic GetOldestTopic(){
			return GetTopic(myMinId);
		}


		/// <summary>
		/// �X�V���ꂽ Topic ���擾���܂��B
		/// </summary>
		public Topic[] GetUpdatedTopics(){
			List<Topic> updated = new List<Topic>();
			foreach(Topic t in GetAllTopics()){
				if(t.Updated > t.Date) updated.Add(t);
			}
			updated.Sort(Topic.CompareByUpdated);
			return updated.ToArray();
		}

		// Select ��������w�肵�� Topic ���擾���܂��B
		private Topic[] GetTopicsBySelectString(string selectString, string sortString){
			DataRow[] dr = myTable.Select(selectString, sortString);
			return DataRowsToTopics(dr);
		}

		// Row[] �� Topic[] �ɕϊ����܂��B
		private Topic[] DataRowsToTopics(DataRow[] rows){
			Topic[] result = new Topic[rows.Length];
			for(int i = 0; i < rows.Length; i++){
				result[i] = GetTopic(rows[i]);
			}
			return result;
		}

		// Row[] �� Topic[] �ɕϊ����A���łɋt���ɂ��܂��B
		private Topic[] DataRowsToTopicsReverse(DataRow[] rows){
			Topic[] result = new Topic[rows.Length];
			for(int i = 0; i < rows.Length; i++){
				int idx = rows.Length - 1 - i;
				result[i] = GetTopic(rows[idx]);
			}
			return result;
		}

		/// <summary>
		/// DataRow ���� Topic ���擾���܂��B
		/// </summary>
		private Topic GetTopic(DataRow row){
			if(row == null) return null;
			return row[myTable.MessageCol] as Topic;
		}

// �ŋ߂̓��L

		/// <summary>
		/// �ŋ߂� Topic �̎��� Topic ���擾���܂��B
		/// </summary>
		public Topic GetRecentNextTopic(){
			string selectString = String.Format("[{0}]<=Max([{0}])-{1}", DiaryTable.DateColName, RecentSpan.Ticks);
			Topic[] topics = GetTopicsBySelectString(selectString, DateSortString);
			if(topics.Length == 0) return null;
			return topics[0];
		}


// ����ԍ��̓��L

		/// <summary>
		/// ����ԍ��� Topic ���擾���܂��B
		/// </summary>
		public Topic GetTopic(int num){
			if(num > myMaxId || num < myMinId) return null;
			return GetTopic(myTable.Rows.Find(num));
		}

		/// <summary>
		/// �w�肳�ꂽ�ԍ����ЂƂV���� Topic ���擾���܂��B
		/// </summary>
		public Topic GetNextTopic(int num){
			if(num >= myMaxId) return null;
			Topic t = GetTopic(num + 1);
			if(t != null) return t;
			//�ċA
			return GetNextTopic(num + 1);
		}

		/// <summary>
		/// �w�肳�ꂽ�ԍ����ЂƂÂ� Topic ���擾���܂��B
		/// </summary>
		public Topic GetPrevTopic(int num){
			if(num <= myMinId) return null;
			Topic t = GetTopic(num - 1);
			if(t != null) return t;
			//�ċA
			return GetPrevTopic(num - 1);
		}


// ������̓��L

		/// <summary>
		/// ������� Topic ���擾���܂��B
		/// </summary>
		public Topic[] GetDayTopics(DateTime date){
			string selectString = String.Format("[{0}]={1}", DiaryTable.DateColName, date.Ticks);
			return GetTopicsBySelectString(selectString, DateSortString);
		}

		/// <summary>
		/// ������Ԃ� Topic ���擾���܂��B
		/// </summary>
		public Topic[] GetSpanTopics(DateTime start, DateTime end){
			string selectString = String.Format("[{0}]>={1} AND [{0}] < {2}", DiaryTable.DateColName, start.Ticks, end.Ticks);
			return GetTopicsBySelectString(selectString, DateSortString);
		}

		/// <summary>
		/// �O�̓��t���擾���܂��B
		/// </summary>
		public DateTime GetPrevDate(DateTime date){
			int pos = Array.IndexOf<DateTime>(myDates, date);
			if(pos < 1) return default(DateTime);
			return myDates[pos-1];
		}

		/// <summary>
		/// ���̓��t���擾���܂��B
		/// </summary>
		public DateTime GetNextDate(DateTime date){
			int pos = Array.IndexOf<DateTime>(myDates, date);
			if(pos < 0) return default(DateTime);
			if(pos >= myDates.Length - 1) return default(DateTime);
			return myDates[pos + 1];
		}


		/// <summary>
		/// �w�肳�ꂽ�W�������ɑ����� topic ���擾���܂��B
		/// </summary>
		public Topic[] GetTopicsByGenre(string genre){
			if(string.IsNullOrEmpty(genre))  return new Topic[0];
			TopicGenre topics = null;
			myGenreDic.TryGetValue(genre, out topics);
			if(topics == null)  return new Topic[0];
			return topics.Topics;
		}

		/// <summary>
		/// �W�����������ׂĎ擾���܂��B
		/// </summary>
		public TopicGenre[] GetGenreList(){
			TopicGenre[] result = new TopicGenre[myGenreDic.Count];
			myGenreDic.Values.CopyTo(result, 0);
			Array.Sort(result, TopicGenre.GenreCountSort);
			return result;
		}




// �f�[�^�̃��[�h
		/// <summary>
		/// XML �t�@�C������f�[�^�����[�h���Atable �Ɋi�[���܂��B
		/// </summary>
		public void Load(){
			XmlNodeList xnl = Document.GetElementsByTagName(Topic.TopicElementName);
			myTable = new DiaryTable();
			myTable.MinimumCapacity = xnl.Count;
			myGenreDic = new Dictionary<string, TopicGenre>();
			List<DateTime> dateList = new List<DateTime>();
			foreach(XmlElement e in xnl){
				Topic t = new Topic(e);
				// �e�[�u���ɒǉ�
				myTable.AddTopic(t);
				// �L�[���[�h��ǉ�
				foreach(string s in t.Genre){
					if(!myGenreDic.ContainsKey(s)) myGenreDic.Add(s, new TopicGenre(s));
					myGenreDic[s].Add(t);
				}
				// ���t��ǉ�
				dateList.AddNew(t.Date);
				// ID���X�V
				if(t.Id > myMaxId) myMaxId = t.Id;
				if(t.Id < myMinId) myMinId = t.Id;
			}
			dateList.Sort();
			myDates = dateList.ToArray();
		}






// �I�[�o�[���C�h���\�b�h

		/// <summary>
		/// �f�[�^�� GET ���AHatomaruResponse ���擾���܂��B
		/// </summary>
		public override HatomaruResponse Get(AbsPath path){
			HatomaruGetAction act = GetAction(path);
			HatomaruResponse result = act.Get();
			result.SetLastModified();
			return result;
		}

		/// <summary>
		/// path �����ɁA�K�؂ȃR���g���[�����쐬���܂��B
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



