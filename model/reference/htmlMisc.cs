using System;
using System.Collections.Generic;
using System.Xml;

namespace Bakera.Hatomaru{

	// HTML の要素・属性・要素グループ・属性グループ・データ形式に該当しないデータを表現するクラス
	public class HtmlMisc : HtmlItem{
		public HtmlMisc(string name){
			myName = name;
		}

		public override string LinkId{
			get{return null;}
		}

		// 親を追加されても何もしない
		public override void AddParent(HtmlItem item){}

	}
}