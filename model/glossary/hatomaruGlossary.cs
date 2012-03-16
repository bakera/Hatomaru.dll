using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Reflection;
using System.Xml;

namespace Bakera.Hatomaru{

/// <summary>
/// �p��W����������N���X�ł��B
/// </summary>
	public partial class HatomaruGlossary : HatomaruXml{

		public new const string Name = "glossary"; // ���[�g�v�f�̖��O

		public const string WordElementName = "word";
		public const string DescElementName = "desc";
		public const string NameAttributeName = "name";
		public const string ReadAttributeName = "read";
		public const string AltreadAttributeName = "altread";
		public const string PronounceAttributeName = "pronounce";
		public const string GenreAttributeName = "genre";
		public const string RelateElementName = "relate";
		public const string SourceElementName = "source";

		public const string ReadOrder = "ABCDEFGHIJKLMNOPQRSTUVWXYZ�����������������������������������ĂƂȂɂʂ˂̂̂͂Ђӂւق܂݂ނ߂��������������";
		public const char ReadNull = ' ';


		private GlossaryTable myTable;
		private GlossaryReadTable myReadTable;
		private Dictionary<string, GlossaryGenre> myGenreDic = new Dictionary<string, GlossaryGenre>(); // �W�������� Dictionary
		private GlossaryGenre[] myGenreList; // �W�������̃��X�g
		private Dictionary<char, List<GlossaryWord>> myReadDic = new Dictionary<char, List<GlossaryWord>>(); // �ǂ݂� Dictionary


// �R���X�g���N�^

		/// <summary>
		/// ���ۃf�[�^�̃f�[�^�\�[�X�� FileInfo �� XmlDocument ���w�肵�āAHatomaruBbs �̃C���X�^���X���J�n���܂��B
		/// </summary>
		public HatomaruGlossary(HatomaruManager manager, FileInfo f, XmlDocument x) : base(manager, f, x){
			myTable = new GlossaryTable();
			myReadTable = new GlossaryReadTable();
			Load();
		}

// �v���p�e�B


// �f�[�^�̎擾
		// ID ���w�肵�� GlossaryWord ���擾���܂��B
		public GlossaryWord GetWordById(string id){
			if(string.IsNullOrEmpty(id)) return null;
			return GetWordByName(id.PathDecode());
		}

		// ���O���w�肵�� GlossaryWord ���擾���܂��B
		public GlossaryWord GetWordByName(string name){
			if(string.IsNullOrEmpty(name)) return null;
			return myTable.GetData(GlossaryTable.NameColName, name, myTable.GlossaryCol) as GlossaryWord;
		}

		// ����̓ǂ݂Ŏn�܂� GlossaryWord ���擾���܂��B
		public GlossaryWord[] GetWordByRead(char read){
			return myReadTable.GetMultiData<GlossaryWord>(GlossaryReadTable.CharColName, read.ToString(), myReadTable.GlossaryCol, GlossaryReadTable.ReadColName);
		}

		// �ǂ݂��J�i�Ŏn�܂�Ȃ� GlossaryWord ���擾���܂��B
		public GlossaryWord[] GetWordByRead(){
			return GetWordByRead(ReadNull);
		}

		// GlossaryGenre �̈ꗗ���擾���܂��B
		public GlossaryGenre[] GetGenreList(){
			return myGenreList;
		}

		// GlossaryGenre ���擾���܂��B
		public GlossaryGenre GetGenre(string name){
			if(string.IsNullOrEmpty(name)) return null;
			if(myGenreDic.ContainsKey(name)) return myGenreDic[name];
			return null;
		}

// �f�[�^�̃��[�h
		/// <summary>
		/// XML �t�@�C������f�[�^�����[�h���܂��B
		/// </summary>
		public void Load(){
			XmlNodeList xnl = Document.GetElementsByTagName(HatomaruGlossary.WordElementName);
			myTable.MinimumCapacity = xnl.Count;
			foreach(XmlElement e in xnl){
				GlossaryWord gw = new GlossaryWord(e);
				// �W��������ǉ�
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
				if(first == GlossaryViewGenreList.Id){
					if(fragments.Length > 1) return new GlossaryViewGenre(this, path, fragments[1]);
					return new GlossaryViewGenreList(this, path);
				}
				if(fragments.Length > 1 && fragments[1].Equals(HatomaruActionBase.CommentPath, StringComparison.InvariantCultureIgnoreCase)){
					return new ViewComment(this, path, BasePath.Combine(first));
				}
				return new GlossaryViewWord(this, path, first);
			}
			// �ǂ�ł��Ȃ��Ƃ��̓g�b�v
			return new GlossaryViewList(this, path);
		}



	} // End class
} // End Namespace Bakera



