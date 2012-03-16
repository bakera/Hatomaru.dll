using System;

namespace Bakera.Hatomaru{

	public class PostError{

		private string myName; // エラーとなった入力項目の name
		private string myMessage; // エラーメッセージ

		public PostError(string name, string message){
			myName = name;
			myMessage = message;
		}

		public string Name{
			get{return myName;}
		}

		public string Message{
			get{return myMessage;}
		}

	
	
	}

}

