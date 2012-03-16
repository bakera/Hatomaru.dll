using System;
using System.Xml;



namespace Bakera.Hatomaru{

	/// <summary>
	/// / から始まる絶対パスを扱うクラスです。
	/// </summary>
	public class AbsPath : Uri{

		public const string StartString = "/";

		/// <summary>
		/// 絶対パスの文字列を指定して、AbsPath のインスタンスを開始します。
		/// </summary>
		public AbsPath(string pathStr) : base(pathStr, UriKind.Relative){
			if(!pathStr.StartsWith(StartString)) throw new Exception("AbsPath は / で始まらなくてはなりません : " + pathStr);
		}


// パブリックメソッド

	// 検査
		/// <summary>
		/// 現在の AbsPath に特定の文字列が含まれるかを検査します。含まれていればtrueを返します。
		/// </summary>
		public bool Contains(string str){
			string currentPath = this.OriginalString;
			return currentPath.IndexOf(str) >= 0;
		}

		/// <summary>
		/// 現在の AbsPath が特定の文字列ではじまるかを検査します。その文字列ではじまればtrueを返します。
		/// </summary>
		public bool StartsWith(string str){
			string currentPath = this.OriginalString;
			return currentPath.StartsWith(str);
		}
		/// <summary>
		/// 現在の AbsPath が特定の文字列でおわるかを検査します。その文字列でおわればtrueを返します。
		/// </summary>
		public bool EndsWith(string str){
			string currentPath = this.OriginalString;
			return currentPath.EndsWith(str);
		}


	// 取得
		/// <summary>
		/// 現在の AbsPath にパス文字列を連結し、新しい AbsPath を得ます
		/// </summary>
		public AbsPath Combine(string relStr){
			string currentPath = this.OriginalString;
			return new AbsPath(currentPath.TrimEnd('/') + '/' + relStr);
		}

		/// <summary>
		/// 現在の AbsPath にパス文字列を連結し、新しい AbsPath を得ます
		/// </summary>
		public AbsPath Combine(string relStr, string relStr2){
			return Combine(relStr + '/' + relStr2);
		}

		/// <summary>
		/// 現在の AbsPath にパス文字列を連結し、新しい AbsPath を得ます
		/// </summary>
		public AbsPath Combine(object relStr, params object[] relStrs){
			string currentPath = relStr.ToString();
			for(int i=0; i < relStrs.Length; i++){
				currentPath += '/' + relStrs[i].ToString();
			}
			return Combine(currentPath);
		}

		/// <summary>
		/// 現在の AbsPath にクエリ文字列を連結し、新しい AbsPath を得ます
		/// </summary>
		public AbsPath CombineQuery(string queryStr){
			string currentPath = this.OriginalString;
			return new AbsPath(currentPath.TrimEnd('?') + '?' + queryStr);
		}

		/// <summary>
		/// 現在の AbsPath に拡張子を連結し、新しい AbsPath を得ます
		/// </summary>
		public AbsPath CombineExtension(string ext){
			string currentPath = this.OriginalString;
			return new AbsPath(currentPath.TrimEnd('.') + '.' + ext.TrimStart('.'));
		}

		/// <summary>
		/// 現在の AbsPath の末尾から文字列を取り除き、新しい AbsPath を得ます
		/// </summary>
		public AbsPath RemoveLast(string removeStr){
			string currentPath = this.OriginalString.CutRight(removeStr).TrimEnd('/');
			if(string.IsNullOrEmpty(currentPath)) currentPath = "/";
			return new AbsPath(currentPath);
		}

		/// <summary>
		/// 現在の AbsPath の末尾からクエリ文字列を取り除き、新しい AbsPath を得ます
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
		/// 現在の AbsPath から文字列を取り除き、新しい AbsPath を得ます。
		/// hatomaru.aspxのリダイレクトに使用します。
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
		/// ドメインの文字列を指定して、現在の AbsPath から絶対 Uri を得ます。
		/// </summary>
		public Uri GetAbsUri(string domainStr){
			Uri baseUri = new Uri(Uri.UriSchemeHttp + Uri.SchemeDelimiter + domainStr);
			return new Uri(baseUri, this);
		}

		/// <summary>
		/// BasePath を渡して、残りのパス断片の配列を取得します。
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






