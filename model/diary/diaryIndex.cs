using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Reflection;
using System.Xml;

namespace Bakera.Hatomaru{

/// <summary>
/// パーシャルな日記のインデクスを処理するクラスです。
/// </summary>
	public partial class DiaryIndex : HatomaruXml{

		public new const string Name = "diary-index";
		public const string DiaryElement = "diary";
		public const string SrcAttribiute = "src";

		public static readonly TimeSpan RecentSpan = new TimeSpan(7, 0, 0, 0);

		private string[] myDiaryList;

		/// <summary>
		/// 鳩丸データのデータソースの FileInfo と XmlDocument を指定して、HatomaruBbs のインスタンスを開始します。
		/// </summary>
		public DiaryIndex(HatomaruManager manager, FileInfo f, XmlDocument x) : base(manager, f, x){
		}

// プロパティ

		public YearDiary[] DiaryList{
			get{
				if(myDiaryList == null) myDiaryList = Load();
				List<YearDiary> diaryList = new List<YearDiary>();
				foreach(string src in myDiaryList){
					HatomaruXml hx = GetDataByPathString(src);
					if(hx is YearDiary) diaryList.Add(hx as YearDiary);
				}
				return diaryList.ToArray();
			}
		}



// メソッド

		private string[] Load(){
			List<string> diaryList = new List<string>();
			foreach(XmlElement e in Document.GetElementsByTagName(DiaryElement)){
				string src = e.GetAttributeValue(SrcAttribiute);
				if(string.IsNullOrEmpty(src)) continue;
				diaryList.Add(src);
			}
			return diaryList.ToArray();
		}


// 日記の取得

		// 最新の日記を取得します。
		public YearDiary GetLatestYearDiary(){
			if(DiaryList.Length == 0) return null;
			return DiaryList[0];
		}


// トピックの取得


		// 特定年の日記を取得します。
		public YearDiary GetYearDiary(int year){
			foreach(YearDiary yd in DiaryList){
				if(yd.Year == year) return yd;
			}
			return null;
		}

		// 特定IDの日記を取得します。
		public Topic GetTopic(int id){
			foreach(YearDiary yd in DiaryList){
				Topic t = yd.GetTopic(id);
				if(t != null) return t;
			}
			return null;
		}

		// 渡された次の日記を取得します。
		public YearDiary GetNextDiary(YearDiary d){
			int index = Array.IndexOf(DiaryList, d);
			if(index == DiaryList.Length - 1) return null;
			if(index < 0) index = -1;
			return DiaryList[index+1];
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
				switch(firstStr){
				case DiaryIndexViewTopic.Id:
					if(fragments.Length > 1){
						int num = fragments[1].ToInt32();
						if(num == 0) break;
						if(fragments.Length > 2 && fragments[2].Equals(HatomaruActionBase.CommentPath, StringComparison.InvariantCultureIgnoreCase)){
							return new ViewComment(this, path, BasePath.Combine(DiaryIndexViewTopic.Id, fragments[1]));
						}
						return new DiaryIndexViewTopic(this, path, num);
					}
					break;
				case DiaryIndexViewGenre.Id:
					if(fragments.Length > 1) return new DiaryIndexViewGenre(this, path, fragments[1]);
					return new DiaryIndexViewGenreList(this, path);
				case DiaryIndexViewBackNumber.Id:
					return new DiaryIndexViewBackNumber(this, path);
				case DiaryIndexViewUpdated.Id:
					if(fragments.Length > 1 && fragments[1].Equals(DiaryIndexUpdatedAtom.Id, StringComparison.InvariantCultureIgnoreCase)){
						return new DiaryIndexUpdatedAtom(this, path);
					}
					return new DiaryIndexViewUpdated(this, path);
				case DiaryIndexViewTitleList.Id:
					return new DiaryIndexViewTitleList(this, path);
				case DiaryIndexViewAmazon.Id:
					return new DiaryIndexViewAmazon(this, path);
				case DiaryIndexRss.Id:
					return new DiaryIndexRss(this, path);
				case DiaryIndexAtom.Id:
					return new DiaryIndexAtom(this, path);


				}
			}
			// どれでもないときはトップ
			return new DiaryIndexViewRecently(this, path);
		}



	} // End class
} // End Namespace Bakera



