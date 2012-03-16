using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Reflection;
using System.Xml;

namespace Bakera.Hatomaru{

/// <summary>
/// 用語集を処理するクラスです。
/// </summary>
	public partial class HatomaruGlossary : HatomaruXml{

		public new const string Name = "glossary"; // ルート要素の名前

		public const string WordElementName = "word";
		public const string DescElementName = "desc";
		public const string NameAttributeName = "name";
		public const string ReadAttributeName = "read";
		public const string AltreadAttributeName = "altread";
		public const string PronounceAttributeName = "pronounce";
		public const string GenreAttributeName = "genre";
		public const string RelateElementName = "relate";
		public const string SourceElementName = "source";

		public const string ReadOrder = "ABCDEFGHIJKLMNOPQRSTUVWXYZあいうえおかきくけこさしすせそたちつてとなにぬねののはひふへほまみむめもらりるれろやゆよわをん";
		public const char ReadNull = ' ';


		private GlossaryTable myTable;
		private GlossaryReadTable myReadTable;
		private Dictionary<string, GlossaryGenre> myGenreDic = new Dictionary<string, GlossaryGenre>(); // ジャンルの Dictionary
		private GlossaryGenre[] myGenreList; // ジャンルのリスト
		private Dictionary<char, List<GlossaryWord>> myReadDic = new Dictionary<char, List<GlossaryWord>>(); // 読みの Dictionary


// コンストラクタ

		/// <summary>
		/// 鳩丸データのデータソースの FileInfo と XmlDocument を指定して、HatomaruBbs のインスタンスを開始します。
		/// </summary>
		public HatomaruGlossary(HatomaruManager manager, FileInfo f, XmlDocument x) : base(manager, f, x){
			myTable = new GlossaryTable();
			myReadTable = new GlossaryReadTable();
			Load();
		}

// プロパティ


// データの取得
		// ID を指定して GlossaryWord を取得します。
		public GlossaryWord GetWordById(string id){
			if(string.IsNullOrEmpty(id)) return null;
			return GetWordByName(id.PathDecode());
		}

		// 名前を指定して GlossaryWord を取得します。
		public GlossaryWord GetWordByName(string name){
			if(string.IsNullOrEmpty(name)) return null;
			return myTable.GetData(GlossaryTable.NameColName, name, myTable.GlossaryCol) as GlossaryWord;
		}

		// 特定の読みで始まる GlossaryWord を取得します。
		public GlossaryWord[] GetWordByRead(char read){
			return myReadTable.GetMultiData<GlossaryWord>(GlossaryReadTable.CharColName, read.ToString(), myReadTable.GlossaryCol, GlossaryReadTable.ReadColName);
		}

		// 読みがカナで始まらない GlossaryWord を取得します。
		public GlossaryWord[] GetWordByRead(){
			return GetWordByRead(ReadNull);
		}

		// GlossaryGenre の一覧を取得します。
		public GlossaryGenre[] GetGenreList(){
			return myGenreList;
		}

		// GlossaryGenre を取得します。
		public GlossaryGenre GetGenre(string name){
			if(string.IsNullOrEmpty(name)) return null;
			if(myGenreDic.ContainsKey(name)) return myGenreDic[name];
			return null;
		}

// データのロード
		/// <summary>
		/// XML ファイルからデータをロードします。
		/// </summary>
		public void Load(){
			XmlNodeList xnl = Document.GetElementsByTagName(HatomaruGlossary.WordElementName);
			myTable.MinimumCapacity = xnl.Count;
			foreach(XmlElement e in xnl){
				GlossaryWord gw = new GlossaryWord(e);
				// ジャンルを追加
				foreach(string s in gw.Genre){
					if(!myGenreDic.ContainsKey(s)) myGenreDic.Add(s, new GlossaryGenre(s));
					myGenreDic[s].Add(gw);
				}
				myTable.AddGlossary(gw);
				myReadTable.AddReads(gw);
			}
			myGenreList = new GlossaryGenre[myGenreDic.Count];
			myGenreDic.Values.CopyTo(myGenreList, 0);
			Array.Sort(myGenreList);
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
				string first = fragments[0];
				if(first == GlossaryViewGenreList.Id){
					if(fragments.Length > 1) return new GlossaryViewGenre(this, path, fragments[1]);
					return new GlossaryViewGenreList(this, path);
				}
				if(fragments.Length > 1 && fragments[1].Equals(HatomaruActionBase.CommentPath, StringComparison.InvariantCultureIgnoreCase)){
					return new ViewComment(this, path, BasePath.Combine(first));
				}
				return new GlossaryViewWord(this, path, first);
			}
			// どれでもないときはトップ
			return new GlossaryViewList(this, path);
		}



	} // End class
} // End Namespace Bakera



