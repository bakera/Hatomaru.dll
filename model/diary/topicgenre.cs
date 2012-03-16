using System;
using System.Collections.Generic;
using System.Xml;

namespace Bakera.Hatomaru{
	
	/// <summary>
	/// トピックのジャンルを表すクラスです。
	/// </summary>
	public class TopicGenre{
		private readonly string myId;
		private readonly List<Topic> myTopics = new List<Topic>();

		// 件数降順ソート用デリゲート
		public static Comparison<TopicGenre> GenreCountSort = delegate(TopicGenre x, TopicGenre y){return y.Count - x.Count;};


// コンストラクタ
		public TopicGenre(string id){
			myId = id;
		}

// プロパティ
		public string Id{
			get{return myId;}
		}

		public Topic[] Topics{
			get{return myTopics.ToArray();}
		}

		public int Count{
			get{return myTopics.Count;}
		}

// メソッド
		public void Add(Topic topic){
			myTopics.Add(topic);
		}


	} // public class TopicGenre

} // namespace Bakera




