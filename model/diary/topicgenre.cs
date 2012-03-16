using System;
using System.Collections.Generic;
using System.Xml;

namespace Bakera.Hatomaru{
	
	/// <summary>
	/// �g�s�b�N�̃W��������\���N���X�ł��B
	/// </summary>
	public class TopicGenre{
		private readonly string myId;
		private readonly List<Topic> myTopics = new List<Topic>();

		// �����~���\�[�g�p�f���Q�[�g
		public static Comparison<TopicGenre> GenreCountSort = delegate(TopicGenre x, TopicGenre y){return y.Count - x.Count;};


// �R���X�g���N�^
		public TopicGenre(string id){
			myId = id;
		}

// �v���p�e�B
		public string Id{
			get{return myId;}
		}

		public Topic[] Topics{
			get{return myTopics.ToArray();}
		}

		public int Count{
			get{return myTopics.Count;}
		}

// ���\�b�h
		public void Add(Topic topic){
			myTopics.Add(topic);
		}


	} // public class TopicGenre

} // namespace Bakera




