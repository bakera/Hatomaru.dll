using System;

namespace Bakera.Hatomaru{

	public class PostError{

		private string myName; // �G���[�ƂȂ������͍��ڂ� name
		private string myMessage; // �G���[���b�Z�[�W

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

