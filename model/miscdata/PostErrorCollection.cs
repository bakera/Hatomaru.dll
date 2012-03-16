using System;
using System.Collections.Generic;

namespace Bakera.Hatomaru{

	public class PostErrorCollection : List<PostError>{

		private Dictionary<string, bool> myDic = new Dictionary<string, bool>();

		public void Add(string name, string message){
			base.Add(new PostError(name, message));
			if(!string.IsNullOrEmpty(name)) myDic[name] = true;
		}

		public bool IsError(string name){
			if(myDic.ContainsKey(name)) return true;
			return false;
		}
	
	
	}

}

