using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace Bakera.Hatomaru{

/// <summary>
/// �h�L�������g�� XML ����������N���X�ł��B
/// </summary>
	public class HatomaruDoc : HatomaruXml{

		new public const string Name = "doc";
		public const string TopicElement = "topic";
		public const string PageIdAttribute = "pageid";

		private Dictionary<string, DocTopic> myTopicsDic = new Dictionary<string, DocTopic>();
		private List<DocTopic> myAnonymousTopics = new List<DocTopic>();
		private DocTopic[] myNamedTopics;

// �R���X�g���N�^

		/// <summary>
		/// ���ۃf�[�^�̃f�[�^�\�[�X�� FileInfo �� XmlDocument ���w�肵�āADoc �̃C���X�^���X���J�n���܂��B
		/// </summary>
		public HatomaruDoc(HatomaruManager manager, FileInfo f, XmlDocument x) : base(manager, f, x){
			int index = 0;
			foreach(XmlElement e in x.GetElementsByTagName(TopicElement)){
				if(string.IsNullOrEmpty(e.GetAttributeValue(HatomaruDoc.PageIdAttribute))){
					DocTopic dt = new DocTopic(e, 0);
					myAnonymousTopics.Add(dt);
				} else {
					DocTopic dt = new DocTopic(e, ++index);
					myTopicsDic.Add(dt.Id, dt);
				}
			}
			myNamedTopics = new DocTopic[myTopicsDic.Values.Count];
			myTopicsDic.Values.CopyTo(myNamedTopics, 0);
		}

// �v���p�e�B

		public DocTopic[] AnonymousTopics{
			get{return myAnonymousTopics.ToArray();}
		}

// ���\�b�h

		public DocTopic GetTopicById(string id){
			if(myTopicsDic.ContainsKey(id)) return myTopicsDic[id];
			return null;
		}

		public DocTopic[] GetAllNamedTopics(){
			return myNamedTopics;
		}

		public DocTopic GetPrevTopic(DocTopic dt){
			if(dt.Index < 2) return null;
			return myNamedTopics[dt.Index-2];
		}

		public DocTopic GetNextTopic(DocTopic dt){
			if(dt.Index >= myNamedTopics.Length) return null;
			return myNamedTopics[dt.Index];
		}
		public DocTopic GetFirstTopic(){
			if(myNamedTopics.Length == 0) return null;
			return myNamedTopics[0];
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
				string first = fragments[0];
				if(first.Equals(HatomaruActionBase.CommentPath, StringComparison.InvariantCultureIgnoreCase)){
					return new ViewComment(this, path, BasePath);
				}
				if(fragments.Length > 1 && fragments[1].Equals(HatomaruActionBase.CommentPath, StringComparison.InvariantCultureIgnoreCase)){
					return new ViewComment(this, path, BasePath.Combine(first));
				}
				return new DocViewSection(this, path, first);
			}
			return new DocViewTop(this, path);
		}



	} // End class Doc
} // End Namespace Bakera







