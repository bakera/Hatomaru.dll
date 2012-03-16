using System;
using System.Xml;



namespace Bakera.Hatomaru{

	/// <summary>
	/// / ����n�܂��΃p�X�������N���X�ł��B
	/// </summary>
	public class AbsPath : Uri{

		public const string StartString = "/";

		/// <summary>
		/// ��΃p�X�̕�������w�肵�āAAbsPath �̃C���X�^���X���J�n���܂��B
		/// </summary>
		public AbsPath(string pathStr) : base(pathStr, UriKind.Relative){
			if(!pathStr.StartsWith(StartString)) throw new Exception("AbsPath �� / �Ŏn�܂�Ȃ��Ă͂Ȃ�܂��� : " + pathStr);
		}


// �p�u���b�N���\�b�h

	// ����
		/// <summary>
		/// ���݂� AbsPath �ɓ���̕����񂪊܂܂�邩���������܂��B�܂܂�Ă����true��Ԃ��܂��B
		/// </summary>
		public bool Contains(string str){
			string currentPath = this.OriginalString;
			return currentPath.IndexOf(str) >= 0;
		}

		/// <summary>
		/// ���݂� AbsPath ������̕�����ł͂��܂邩���������܂��B���̕�����ł͂��܂��true��Ԃ��܂��B
		/// </summary>
		public bool StartsWith(string str){
			string currentPath = this.OriginalString;
			return currentPath.StartsWith(str);
		}
		/// <summary>
		/// ���݂� AbsPath ������̕�����ł���邩���������܂��B���̕�����ł�����true��Ԃ��܂��B
		/// </summary>
		public bool EndsWith(string str){
			string currentPath = this.OriginalString;
			return currentPath.EndsWith(str);
		}


	// �擾
		/// <summary>
		/// ���݂� AbsPath �Ƀp�X�������A�����A�V���� AbsPath �𓾂܂�
		/// </summary>
		public AbsPath Combine(string relStr){
			string currentPath = this.OriginalString;
			return new AbsPath(currentPath.TrimEnd('/') + '/' + relStr);
		}

		/// <summary>
		/// ���݂� AbsPath �Ƀp�X�������A�����A�V���� AbsPath �𓾂܂�
		/// </summary>
		public AbsPath Combine(string relStr, string relStr2){
			return Combine(relStr + '/' + relStr2);
		}

		/// <summary>
		/// ���݂� AbsPath �Ƀp�X�������A�����A�V���� AbsPath �𓾂܂�
		/// </summary>
		public AbsPath Combine(object relStr, params object[] relStrs){
			string currentPath = relStr.ToString();
			for(int i=0; i < relStrs.Length; i++){
				currentPath += '/' + relStrs[i].ToString();
			}
			return Combine(currentPath);
		}

		/// <summary>
		/// ���݂� AbsPath �ɃN�G���������A�����A�V���� AbsPath �𓾂܂�
		/// </summary>
		public AbsPath CombineQuery(string queryStr){
			string currentPath = this.OriginalString;
			return new AbsPath(currentPath.TrimEnd('?') + '?' + queryStr);
		}

		/// <summary>
		/// ���݂� AbsPath �Ɋg���q��A�����A�V���� AbsPath �𓾂܂�
		/// </summary>
		public AbsPath CombineExtension(string ext){
			string currentPath = this.OriginalString;
			return new AbsPath(currentPath.TrimEnd('.') + '.' + ext.TrimStart('.'));
		}

		/// <summary>
		/// ���݂� AbsPath �̖������當�������菜���A�V���� AbsPath �𓾂܂�
		/// </summary>
		public AbsPath RemoveLast(string removeStr){
			string currentPath = this.OriginalString.CutRight(removeStr).TrimEnd('/');
			if(string.IsNullOrEmpty(currentPath)) currentPath = "/";
			return new AbsPath(currentPath);
		}

		/// <summary>
		/// ���݂� AbsPath �̖�������N�G�����������菜���A�V���� AbsPath �𓾂܂�
		/// </summary>
		public AbsPath RemoveQuery(){
			string current = this.OriginalString;
			int index = current.IndexOf("?");
			if(index >= 0){
				current = current.Substring(0, index);
			}
			return new AbsPath(current);
		}

		/// <summary>
		/// ���݂� AbsPath ���當�������菜���A�V���� AbsPath �𓾂܂��B
		/// hatomaru.aspx�̃��_�C���N�g�Ɏg�p���܂��B
		/// </summary>
		public AbsPath Remove(string removeStr){
			string currentPath = this.OriginalString;
			if(currentPath.IndexOf(removeStr) >= 0){
				currentPath = currentPath.Replace(removeStr, "").Replace("//", "/").TrimEnd('/');
				if(string.IsNullOrEmpty(currentPath)) currentPath = "/";
			}
			return new AbsPath(currentPath);
		}


		/// <summary>
		/// �h���C���̕�������w�肵�āA���݂� AbsPath ������ Uri �𓾂܂��B
		/// </summary>
		public Uri GetAbsUri(string domainStr){
			Uri baseUri = new Uri(Uri.UriSchemeHttp + Uri.SchemeDelimiter + domainStr);
			return new Uri(baseUri, this);
		}

		/// <summary>
		/// BasePath ��n���āA�c��̃p�X�f�Ђ̔z����擾���܂��B
		/// </summary>
		public string[] GetFragments(AbsPath basePath){
			string baseStr = basePath.ToString();
			string result = this.RemoveQuery().ToString();
			if(result.StartsWith(baseStr)){
				result = result.Remove(0, baseStr.Length);
			}
			return result.Split(new Char[]{'/', '\\'}, StringSplitOptions.RemoveEmptyEntries);
		}




	} // End class Path

} // End namespace Bakera






