using System;

namespace Bakera.Hatomaru{

	// 静的メソッドのみを持つユーテリティクラス
	public static class CharUtil{

		private const string Hiragana = "あぁいぃうぅえぇおぉかきくけこがぎぐげごさしすせそざじずぜぞたちつってとだぢづでどなにぬねのはひふへほばびぶべぼぱぴぷぺぽまみむめもやゐゆゑよらりるれろわゎをん";
		private const string Katakana = "アァイィウゥエェオォカキクケコガギグゲゴサシスセソザジズゼゾタチツッテトダヂヅデドナニヌネノハヒフヘホバビブベボパピプペポマミムメモヤヰユヱヨラリルレロワヮヲン";
		private const string Smark = "ガギグゲゴザジズゼゾダヂヅデドバビブベボパピプペポがぎぐげござじずぜぞだぢづでどばびぶべぼぱぴぷぺぽ";
		private const string NoSmark = "カキクケコサシスセソタチツテトハヒフヘホハヒフヘホかきくけこさしすせそたちつてとはひふへほはひふへほ";

		/// <summary>
		/// ひらがなをカタカナに変換します。
		/// </summary>
		public static string HiraganaToKatakana(this string s){
			char[] result = new char[s.Length];
			for(int i = s.Length -1; i >= 0; i--){
				result[i] = s[i].HiraganaToKatakana();
			}
			return new String(result);
		}
		public static char HiraganaToKatakana(this char c){
			if(c < 'ぁ') return c;
			if(c > 'ん') return c;
			for(int i = 0; i < Hiragana.Length; i++){
				if(c == Hiragana[i]) return Katakana[i];
			}
			return c;
		}
		/// <summary>
		/// カタカナをひらがなに変換します。
		/// </summary>
		public static string KatakanaToHiragana(this string s){
			char[] result = new char[s.Length];
			for(int i = s.Length -1; i >= 0; i--){
				result[i] = s[i].KatakanaToHiragana();
			}
			return new String(result);
		}
		public static char KatakanaToHiragana(this char c){
			if(c < 'ァ') return c;
			if(c > '\x30fa') return c;
			if(c > 'ン'){
				return SpecialKatakanaToHiragana(c);
			}
			for(int i = 0; i < Katakana.Length; i++){
				if(c == Katakana[i]) return Hiragana[i];
			}
			return c;
		}
		private static char SpecialKatakanaToHiragana(this char c){
			switch(c){
			case '\x30f4': return 'う';
			case '\x30f5': return 'か';
			case '\x30f6': return 'け';
			case '\x30f7': return 'わ';
			case '\x30f8': return 'ゐ';
			case '\x30f9': return 'ゑ';
			case '\x30fa': return 'を';
			}
			return c;
		}

		/// <summary>
		/// ひらがなやカタカナから濁点や半濁点を取り除きます。
		/// </summary>
		public static string DeleteSonantMark(this string s){
			char[] result = new char[s.Length];
			for(int i = s.Length -1; i >= 0; i--){
				result[i] = s[i].DeleteSonantMark();
			}
			return new String(result);
		}
		public static char DeleteSonantMark(this char c){
			if(c < 'が') return c;
			if(c > 'ポ') return c;
			for(int i = 0; i < Smark.Length; i++){
				if(c == Smark[i]) return NoSmark[i];
			}
			return c;
		}

		/// <summary>
		/// 文字を「読み」見出し文字に変換します。
		/// アルファベットは大文字、仮名はひらがな濁点無し
		/// </summary>
		public static char ToReadChar(this char c){
			return Char.ToUpper(c).DeleteSonantMark().KatakanaToHiragana();
		}


	} // end class Util

} // end namespace Bakea

