using System;
using System.Globalization;
using System.Collections.Generic;
using System.IO;


namespace Bakera.Hatomaru{
	public class Log{

		private List<LogItem> myMessages = new List<LogItem>();



// public ���\�b�h

		// ���b�Z�[�W��ǉ����܂��B
		public void Add(string message){
			LogItem item = new LogItem(message);
			myMessages.Add(item);
		}

		// �t�H�[�}�b�g��������w�肵�āA���b�Z�[�W��ǉ����܂��B
		public void Add(string format, params object[] messages){
			Add(string.Format(CultureInfo.CurrentCulture, format, messages));
		}

		// ���ׂẴ��b�Z�[�W���o�͂��܂��B
		public override string ToString(){
			string result = "";
			foreach(LogItem log in myMessages){
				result += string.Format(CultureInfo.CurrentCulture, "{0} : {1}", log.Time, log.Data);
				result += "\n";
			}
			return result;
		}

		// ���ׂẴ��b�Z�[�W���N���A���܂��B
		public void Clear(){
			myMessages.Clear();
		}

	}


}

