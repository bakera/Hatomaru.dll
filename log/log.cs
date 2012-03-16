using System;
using System.Globalization;
using System.Collections.Generic;
using System.IO;


namespace Bakera.Hatomaru{
	public class Log{

		private List<LogItem> myMessages = new List<LogItem>();



// public メソッド

		// メッセージを追加します。
		public void Add(string message){
			LogItem item = new LogItem(message);
			myMessages.Add(item);
		}

		// フォーマット文字列を指定して、メッセージを追加します。
		public void Add(string format, params object[] messages){
			Add(string.Format(CultureInfo.CurrentCulture, format, messages));
		}

		// すべてのメッセージを出力します。
		public override string ToString(){
			string result = "";
			foreach(LogItem log in myMessages){
				result += string.Format(CultureInfo.CurrentCulture, "{0} : {1}", log.Time, log.Data);
				result += "\n";
			}
			return result;
		}

		// すべてのメッセージをクリアします。
		public void Clear(){
			myMessages.Clear();
		}

	}


}

