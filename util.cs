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

	// �ÓI���\�b�h�݂̂������[�e���e�B�N���X
	public static class Util{

		private const string NGPathChars = "/#\\\"<>:?";
		private const string ReplcaePathChars = "�^�����h�����F�H";
		public static Regex HtmlRefIdsRegex = new BakeraReg.HtmlRefIdsRegex();

		/// <summary>
		/// ��O���X���[���܂��B
		/// </summary>
		public static void Throw(){
			Throw("throw");
		}

		/// <summary>
		/// �n���ꂽ�I�u�W�F�N�g�����b�Z�[�W�Ƃ����O���X���[���܂��B
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
		/// �n���ꂽ�I�u�W�F�N�g��W�J���ĕ����񉻂��܂��B
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
		/// �t�B�[���h�̒l���擾���܂��B
		/// </summary>
		public static string GetFieldValue(Type t, string fieldName){
			FieldInfo f = t.GetField(fieldName);
			if(f ==null) return null;
			return f.GetValue(null) as string;
		}


// XML �֘A

		/// <summary>
		/// �����l���擾���܂��B
		/// �������Ȃ��Ƃ��� null ���Ԃ�܂��B
		/// </summary>
		public static string GetAttributeValue(this XmlNode node, string attrName){
			XmlElement target = node as XmlElement;
			if(target == null) return null;
			XmlAttribute attr = target.Attributes[attrName];
			if(attr == null) return null;
			return attr.Value.Trim();
		}

		/// <summary>
		/// �w�肳�ꂽ���O�̎q�v�f�̓��e�̃e�L�X�g���擾���܂��B
		/// </summary>
		public static string GetInnerText(this XmlNode node, string elemName){
			XmlElement target = node as XmlElement;
			if(target == null) return null;
			XmlElement child = target[elemName];
			return child.GetInnerText();
		}

		/// <summary>
		/// �v�f�̓��e�̃e�L�X�g���擾���܂��B
		/// </summary>
		public static string GetInnerText(this XmlNode node){
			XmlElement target = node as XmlElement;
			if(target == null) return null;
			return target.InnerText;
		}


		// �����l���� int ���擾���܂��B
		public static int GetAttributeInt(this XmlNode node, string attrName){
			string s = GetAttributeValue(node, attrName);
			if(string.IsNullOrEmpty(s)) return default(int);
			int result = 0;
			Int32.TryParse(s, out result);
			return result;
		}

		// �����l���� DateTime ���擾���܂��B
		// ���t���Ȃ������݂̂̏ꍇ�� 1/1/1 ���擾���܂��B
		public static DateTime GetAttributeDateTime(this XmlNode node, string attrName){
			string s = GetAttributeValue(node, attrName);
			if(string.IsNullOrEmpty(s)) return default(DateTime);
			DateTime result = default(DateTime);
			DateTime.TryParse(s, null, DateTimeStyles.NoCurrentDateDefault & DateTimeStyles.AdjustToUniversal, out result);
			return result;
		}

		// �����l���� DateTime ���擾���܂��B
		// ���t���Ȃ��ꍇ�A�n���ꂽDateTime�̓��t���g�p���܂��B
		// �������󕶎���̏ꍇ�A�n���ꂽDateTime�����̂܂܎g�p���܂��B
		// ���������݂��Ȃ��ꍇ�� default ��Ԃ��܂��B
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


		// �����l���� bool ���擾���܂��B
		public static bool GetAttributeBool(this XmlNode node, string attrName){
			string s = GetAttributeValue(node, attrName);
			if(string.IsNullOrEmpty(s)) return false;
			if(s.Equals(attrName)) return true;
			return false;
		}

		// �����l���� AbsPath ���擾���܂��B
		public static AbsPath GetAttributePath(this XmlNode node, string attrName){
			string s = GetAttributeValue(node, attrName);
			if(string.IsNullOrEmpty(s)) return null;
			return new AbsPath(s);
		}

		// �����l����J���}��؂�̕�����z����擾���܂��B
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



// ������֘A

		/// <summary>
		/// URL/�p�X�Ɏg���Ɩ��̂��镶����S�p�ɒu�������܂��B
		/// </summary>
		public static string PathEncode(this string s){
			if(s == null) return null;
			for(int i=0; i < NGPathChars.Length; i++){
				s = s.Replace(NGPathChars[i], ReplcaePathChars[i]);
			}
			return s;
		}
		/// <summary>
		/// PathEncode ��߂��܂��B
		/// </summary>
		public static string PathDecode(this string s){
			if(s == null) return null;
			for(int i=0; i < ReplcaePathChars.Length; i++){
				s = s.Replace(ReplcaePathChars[i], NGPathChars[i]);
			}
			return s;
		}

		/// <summary>
		/// �t�@�C���p�X��Z�����܂��B
		/// </summary>
		public static string ShortenPath(this string s){
			int pos = s.LastIndexOf('/');
			if(pos < 0) return "/";
			return s.Remove(pos);
		}

		/// <summary>
		/// �t�@�C���p�X�̖����̒f�Ђ��擾���܂��B
		/// </summary>
		public static string GetLastName(this string s){
			int pos = s.LastIndexOf('/');
			if(pos < 0) return "";
			return s.Substring(pos+1);
		}

		// ������̖������當������폜���܂��B
		public static string CutRight(this string s, string cut){
			if(!s.EndsWith(cut)) return s;
			int pos = s.LastIndexOf(cut);
			if(pos < 0) return s;
			return s.Remove(pos);
		}

		// ������̐擪���當������폜���܂��B
		public static string CutLeft(this string s, string cut){
			if(!s.StartsWith(cut)) return s;
			return s.Remove(0, cut.Length);
		}

		/// <summary>
		/// Base16 ������� string �ɕϊ����܂��B
		/// </summary>
		public static string Base16ToString(this string s){
			// ���������������s���Ȃ̂ŏI��
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

		// ������𐔒l�ɕϊ����܂��B
		// �󕶎���̏ꍇ�A�ϊ��ł��Ȃ��ꍇ�A���̐��̏ꍇ�� 0 ��Ԃ��܂��B
		public static int ToInt32(this object o){
			if(o == null) return 0;
			string s = o.ToString();
			if(string.IsNullOrEmpty(s)) return 0;
			int result = 1;
			if(!int.TryParse(s, out result)) return 0;
			if(result < 0) return 0;
			return result;
		}

		// ������� URL �G���R�[�h���܂��B
		// �X�y�[�X��+�ł͂Ȃ�%20�ɃG���R�[�h���܂��B
		public static string UrlEncode(this string s){
			return HttpUtility.UrlEncode(s).Replace("+", "%20");
		}
		// ������� URL �f�R�[�h���܂��B
		public static string UrlDecode(this string s){
			return HttpUtility.UrlDecode(s);
		}

		// ������� Uri �ɕϊ����܂��B
		public static Uri ToUri(this string s){
			try{
				if(s.StartsWith(AbsPath.StartString)) return new AbsPath(s);
				return new Uri(s);
			} catch (UriFormatException e){
				throw new Exception("������URI: " + s, e);
			}
		}

		// ��������w�蕶�����Ɋۂ߂܂��B
		public static string Truncate(this string s, int length){
			if(s.Length <= length) return s;
			if(length < 3) length = 3;
			return s.Substring(0, length - 2) + "�c�c";
		}

		// �J���}�ŕ������܂��B
		public static string[] CommaSplit(this string s){
			if(string.IsNullOrEmpty(s)) return new string[0];
			return s.Split(new char[]{','}, StringSplitOptions.RemoveEmptyEntries);
		}

		// ���������ID������؂藎�Ƃ��Đ��K��������������擾���܂��B
		public static string GetIdString(this string s){
			Match m = HtmlRefIdsRegex.Match(s);
			if(m.Success) return m.Value;
			return null;
		}

		// StringComparison.InvariantCultureIgnoreCase ���g���ĕ������r���܂��B
		public static bool Eq(this string s1, string s2){
			return s1.Equals(s2, StringComparison.InvariantCultureIgnoreCase);
		}



// ���X�g�֘A
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


// ���t�֘A
		// ���̏T�̓��j�����擾���܂��B
		public static DateTime GetSunday(this DateTime dt){
			return dt.AddDays(-Convert.ToDouble(dt.DayOfWeek));
		}


// �f�o�b�O

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

