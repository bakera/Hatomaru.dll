using System;

namespace Bakera.Hatomaru{

	// �ÓI���\�b�h�݂̂������[�e���e�B�N���X
	public static class CharUtil{

		private const string Hiragana = "�������������������������������������������������������������������ĂƂ����ÂłǂȂɂʂ˂̂͂Ђӂւق΂тԂׂڂς҂Ղ؂ۂ܂݂ނ߂�����������������";
		private const string Katakana = "�A�@�C�B�E�D�G�F�I�H�J�L�N�P�R�K�M�O�Q�S�T�V�X�Z�\�U�W�Y�[�]�^�`�c�b�e�g�_�a�d�f�h�i�j�k�l�m�n�q�t�w�z�o�r�u�x�{�p�s�v�y�|�}�~����������������������������������";
		private const string Smark = "�K�M�O�Q�S�U�W�Y�[�]�_�a�d�f�h�o�r�u�x�{�p�s�v�y�|�������������������������Âłǂ΂тԂׂڂς҂Ղ؂�";
		private const string NoSmark = "�J�L�N�P�R�T�V�X�Z�\�^�`�c�e�g�n�q�t�w�z�n�q�t�w�z�������������������������ĂƂ͂Ђӂւق͂Ђӂւ�";

		/// <summary>
		/// �Ђ炪�Ȃ��J�^�J�i�ɕϊ����܂��B
		/// </summary>
		public static string HiraganaToKatakana(this string s){
			char[] result = new char[s.Length];
			for(int i = s.Length -1; i >= 0; i--){
				result[i] = s[i].HiraganaToKatakana();
			}
			return new String(result);
		}
		public static char HiraganaToKatakana(this char c){
			if(c < '��') return c;
			if(c > '��') return c;
			for(int i = 0; i < Hiragana.Length; i++){
				if(c == Hiragana[i]) return Katakana[i];
			}
			return c;
		}
		/// <summary>
		/// �J�^�J�i���Ђ炪�Ȃɕϊ����܂��B
		/// </summary>
		public static string KatakanaToHiragana(this string s){
			char[] result = new char[s.Length];
			for(int i = s.Length -1; i >= 0; i--){
				result[i] = s[i].KatakanaToHiragana();
			}
			return new String(result);
		}
		public static char KatakanaToHiragana(this char c){
			if(c < '�@') return c;
			if(c > '\x30fa') return c;
			if(c > '��'){
				return SpecialKatakanaToHiragana(c);
			}
			for(int i = 0; i < Katakana.Length; i++){
				if(c == Katakana[i]) return Hiragana[i];
			}
			return c;
		}
		private static char SpecialKatakanaToHiragana(this char c){
			switch(c){
			case '\x30f4': return '��';
			case '\x30f5': return '��';
			case '\x30f6': return '��';
			case '\x30f7': return '��';
			case '\x30f8': return '��';
			case '\x30f9': return '��';
			case '\x30fa': return '��';
			}
			return c;
		}

		/// <summary>
		/// �Ђ炪�Ȃ�J�^�J�i������_�┼���_����菜���܂��B
		/// </summary>
		public static string DeleteSonantMark(this string s){
			char[] result = new char[s.Length];
			for(int i = s.Length -1; i >= 0; i--){
				result[i] = s[i].DeleteSonantMark();
			}
			return new String(result);
		}
		public static char DeleteSonantMark(this char c){
			if(c < '��') return c;
			if(c > '�|') return c;
			for(int i = 0; i < Smark.Length; i++){
				if(c == Smark[i]) return NoSmark[i];
			}
			return c;
		}

		/// <summary>
		/// �������u�ǂ݁v���o�������ɕϊ����܂��B
		/// �A���t�@�x�b�g�͑啶���A�����͂Ђ炪�ȑ��_����
		/// </summary>
		public static char ToReadChar(this char c){
			return Char.ToUpper(c).DeleteSonantMark().KatakanaToHiragana();
		}


	} // end class Util

} // end namespace Bakea

