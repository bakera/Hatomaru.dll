using System;
using System.Globalization;
using System.IO;
using System.Xml;
using System.Text;
using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web;
using System.Net;
using System.Security.Cryptography;

namespace Bakera.Hatomaru{

	// 静的メソッドのみを持つユーテリティクラス
	public static class Util{

		private const string NGPathChars = "/#\\\"<>:?";
		private const string ReplcaePathChars = "／＃￥”＜＞：？";
		public static Regex HtmlRefIdsRegex = new BakeraReg.HtmlRefIdsRegex();

		/// <summary>
		/// 例外をスローします。
		/// </summary>
		public static void Throw(){
			Throw("throw");
		}

		/// <summary>
		/// 渡されたオブジェクトをメッセージとする例外をスローします。
		/// </summary>
		public static void Throw(params object[] objList){
			string result = "";
			if(objList == null){
				result = ToString(objList);
			} else {
				foreach(Object o in objList){
					result += ToString(o);
				}
			}
			throw new Exception(result);
		}

		/// <summary>
		/// 渡されたオブジェクトを展開して文字列化します。
		/// </summary>
		public static string ToString(Object o){
			string result = "";
			if(o == null) return "[null]";
			if(o is string){
				result += o.ToString();
			} else if(o is System.Collections.IEnumerable){
				result += o.ToString();
				foreach(Object innerObj in (System.Collections.IEnumerable)o){
					result += "[" + ToString(innerObj) + "]";
				}
			} else {
				result = o.ToString();
			}
			return result;
		}

		/// <summary>
		/// フィールドの値を取得します。
		/// </summary>
		public static string GetFieldValue(Type t, string fieldName){
			FieldInfo f = t.GetField(fieldName);
			if(f ==null) return null;
			return f.GetValue(null) as string;
		}


// XML 関連

		/// <summary>
		/// 属性値を取得します。
		/// 属性がないときは null が返ります。
		/// </summary>
		public static string GetAttributeValue(this XmlNode node, string attrName){
			XmlElement target = node as XmlElement;
			if(target == null) return null;
			XmlAttribute attr = target.Attributes[attrName];
			if(attr == null) return null;
			return attr.Value.Trim();
		}

		/// <summary>
		/// 指定された名前の子要素の内容のテキストを取得します。
		/// </summary>
		public static string GetInnerText(this XmlNode node, string elemName){
			XmlElement target = node as XmlElement;
			if(target == null) return null;
			XmlElement child = target[elemName];
			return child.GetInnerText();
		}

		/// <summary>
		/// 要素の内容のテキストを取得します。
		/// </summary>
		public static string GetInnerText(this XmlNode node){
			XmlElement target = node as XmlElement;
			if(target == null) return null;
			return target.InnerText;
		}


		// 属性値から int を取得します。
		public static int GetAttributeInt(this XmlNode node, string attrName){
			string s = GetAttributeValue(node, attrName);
			if(string.IsNullOrEmpty(s)) return default(int);
			int result = 0;
			Int32.TryParse(s, out result);
			return result;
		}

		// 属性値から DateTime を取得します。
		// 日付がなく時刻のみの場合は 1/1/1 を取得します。
		public static DateTime GetAttributeDateTime(this XmlNode node, string attrName){
			string s = GetAttributeValue(node, attrName);
			if(string.IsNullOrEmpty(s)) return default(DateTime);
			DateTime result = default(DateTime);
			DateTime.TryParse(s, null, DateTimeStyles.NoCurrentDateDefault & DateTimeStyles.AdjustToUniversal, out result);
			return result;
		}

		// 属性値から DateTime を取得します。
		// 日付がない場合、渡されたDateTimeの日付を使用します。
		// 属性が空文字列の場合、渡されたDateTimeをそのまま使用します。
		// 属性が存在しない場合は default を返します。
		public static DateTime GetAttributeDateTime(this XmlNode node, string attrName, DateTime baseDate){
			string s = GetAttributeValue(node, attrName);
			if(s == null) return default(DateTime);
			if(s == "") return baseDate;
			DateTime result = default(DateTime);
			DateTime.TryParse(s, null, DateTimeStyles.NoCurrentDateDefault & DateTimeStyles.AdjustToUniversal, out result);
			if(result != default(DateTime) && result.Year == 1){
				result = new DateTime(baseDate.Year, baseDate.Month, baseDate.Day, result.Hour, result.Minute, result.Second);
			}
			return result;
		}


		// 属性値から bool を取得します。
		public static bool GetAttributeBool(this XmlNode node, string attrName){
			string s = GetAttributeValue(node, attrName);
			if(string.IsNullOrEmpty(s)) return false;
			if(s.Equals(attrName)) return true;
			return false;
		}

		// 属性値から AbsPath を取得します。
		public static AbsPath GetAttributePath(this XmlNode node, string attrName){
			string s = GetAttributeValue(node, attrName);
			if(string.IsNullOrEmpty(s)) return null;
			return new AbsPath(s);
		}

		// 属性値からカンマ区切りの文字列配列を取得します。
		public static string[] GetAttributeValues(this XmlNode node, string attrName){
			string s = GetAttributeValue(node, attrName);
			if(string.IsNullOrEmpty(s)) return new string[0];
			string[] strs = s.CommaSplit();
			string[] result = new string[strs.Length];
			for(int i=0; i< result.Length; i++){
				result[i] = strs[i].Trim();
			}
			return result;
		}



// 文字列関連

		/// <summary>
		/// URL/パスに使うと問題のある文字を全角に置き換えます。
		/// </summary>
		public static string PathEncode(this string s){
			if(s == null) return null;
			for(int i=0; i < NGPathChars.Length; i++){
				s = s.Replace(NGPathChars[i], ReplcaePathChars[i]);
			}
			return s;
		}
		/// <summary>
		/// PathEncode を戻します。
		/// </summary>
		public static string PathDecode(this string s){
			if(s == null) return null;
			for(int i=0; i < ReplcaePathChars.Length; i++){
				s = s.Replace(ReplcaePathChars[i], NGPathChars[i]);
			}
			return s;
		}

		/// <summary>
		/// ファイルパスを短くします。
		/// </summary>
		public static string ShortenPath(this string s){
			int pos = s.LastIndexOf('/');
			if(pos < 0) return "/";
			return s.Remove(pos);
		}

		/// <summary>
		/// ファイルパスの末尾の断片を取得します。
		/// </summary>
		public static string GetLastName(this string s){
			int pos = s.LastIndexOf('/');
			if(pos < 0) return "";
			return s.Substring(pos+1);
		}

		// 文字列の末尾から文字列を削除します。
		public static string CutRight(this string s, string cut){
			if(!s.EndsWith(cut)) return s;
			int pos = s.LastIndexOf(cut);
			if(pos < 0) return s;
			return s.Remove(pos);
		}

		// 文字列の先頭から文字列を削除します。
		public static string CutLeft(this string s, string cut){
			if(!s.StartsWith(cut)) return s;
			return s.Remove(0, cut.Length);
		}

		/// <summary>
		/// Base16 文字列を string に変換します。
		/// </summary>
		public static string Base16ToString(this string s){
			// 長さが奇数だったら不正なので終了
			if(s.Length % 2 !=0) return null;
			byte[] bytes = new byte[s.Length / 2];
			for(int i=0; i < bytes.Length; i++){
				string partial = s.Substring(i*2, 2);
				try{
					bytes[i] = Convert.ToByte(partial, 16);
				} catch {
					return null;
				}
			}
			return System.Text.Encoding.BigEndianUnicode.GetString(bytes);
		}

		// 文字列を数値に変換します。
		// 空文字列の場合、変換できない場合、負の数の場合は 0 を返します。
		public static int ToInt32(this object o){
			if(o == null) return 0;
			string s = o.ToString();
			if(string.IsNullOrEmpty(s)) return 0;
			int result = 1;
			if(!int.TryParse(s, out result)) return 0;
			if(result < 0) return 0;
			return result;
		}

		// 文字列を URL エンコードします。
		// スペースは+ではなく%20にエンコードします。
		public static string UrlEncode(this string s){
			return HttpUtility.UrlEncode(s).Replace("+", "%20");
		}
		// 文字列を URL デコードします。
		public static string UrlDecode(this string s){
			return HttpUtility.UrlDecode(s);
		}

		// 文字列を Uri に変換します。
		public static Uri ToUri(this string s){
			try{
				if(s.StartsWith(AbsPath.StartString)) return new AbsPath(s);
				return new Uri(s);
			} catch (UriFormatException e){
				throw new Exception("無効なURI: " + s, e);
			}
		}

		// 文字列を指定文字数に丸めます。
		public static string Truncate(this string s, int length){
			if(s.Length <= length) return s;
			if(length < 3) length = 3;
			return s.Substring(0, length - 2) + "……";
		}

		// カンマで分割します。
		public static string[] CommaSplit(this string s){
			if(string.IsNullOrEmpty(s)) return new string[0];
			return s.Split(new char[]{','}, StringSplitOptions.RemoveEmptyEntries);
		}

		// 末尾から非ID文字を切り落として正規化した文字列を取得します。
		public static string GetIdString(this string s){
			Match m = HtmlRefIdsRegex.Match(s);
			if(m.Success) return m.Value;
			return null;
		}

		// StringComparison.InvariantCultureIgnoreCase を使って文字列比較します。
		public static bool Eq(this string s1, string s2){
			return s1.Equals(s2, StringComparison.InvariantCultureIgnoreCase);
		}



// リスト関連
		public static bool AddNew<T>(this ICollection<T> collection, T item){
			if(collection.Contains(item)) return false;
			collection.Add(item);
			return true;
		}

		public static T[] Conv<T>(this Object[] o) where T : class{
			T[] result = new T[o.Length];
			for(int i = 0; i < o.Length; i++){
				result[i] = o[i] as T;
			}
			return result;
		}


// 日付関連
		// その週の日曜日を取得します。
		public static DateTime GetSunday(this DateTime dt){
			return dt.AddDays(-Convert.ToDouble(dt.DayOfWeek));
		}


// デバッグ

		public static void DebugPrint(this object o){
			string dateName = DateTime.Now.ToString("yyyy-MM-dd") + ".txt";
			FileInfo logfile = new FileInfo("C:\\Users\\bakera\\.NET\\hatomarudll\\debuglog.txt");
			using(FileStream fs = logfile.Open(FileMode.Append, FileAccess.Write, FileShare.None)){
				using(StreamWriter sw = new StreamWriter(fs)){
					sw.WriteLine(o.ToString());
				}
			}
		}

	} // end class Util

} // end namespace Bakea

