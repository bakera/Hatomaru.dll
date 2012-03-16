using System;
using System.Collections.Generic;
using System.Xml;

namespace Bakera.Hatomaru{

	// HTML のバージョン情報を有するクラスのベースとなる抽象クラス
	public abstract class HtmlVersionItem : HtmlItem{

		protected HtmlVersions myVersion;
		public HtmlVersions Version{
			get{return myVersion;}
			set{myVersion = value;}
		}

		public HtmlVersionItem(XmlElement e) : base(e){
			string verStr = e.GetInnerText(HatomaruHtmlRef.VersionElementName);
			if(!string.IsNullOrEmpty(verStr)) myVersion = (HtmlVersions)Enum.Parse(typeof(HtmlVersions), verStr, true);
		}

		/// <summary>
		/// この要素・属性が HTML4 で deprecated とされていれば true を返します。
		/// obsolete の場合も true を返します。
		/// </summary>
		public bool IsDeprecated{
			// HTML 4.01 strict にない == deprecated
			get{
				if((myVersion & HtmlVersions.x11) != 0) return false;
				if((myVersion & HtmlVersions.h40) != 0) return false;
				return true;
			}
		}
		/// <summary>
		/// この要素・属性が HTML4 で obsolete とされていれば true を返します。
		/// </summary>
		public bool IsObsolete{
			get{
				if((myVersion & HtmlVersions.x11) != 0) return false;
				if((myVersion & HtmlVersions.h40) != 0) return false;
				if((myVersion & HtmlVersions.h40t) != 0) return false;
				if((myVersion & HtmlVersions.h40f) != 0) return false;
				return true;
			}
		}
		/// <summary>
		/// この要素・属性が HTML 仕様内であれば true を、そうでなければ false を返します。
		/// </summary>
		public bool IsSpecified{
			get{
				if(myVersion == 0) return false;
				return true;
			}
		}


// メソッド
		/// <summary>
		/// バージョンを表す文字列を得ます。
		/// </summary>
		public string GetVersion(){

			List<string> sc = new List<string>();
			if((myVersion & HtmlVersions.h10) != 0) sc.Add("1.0");
			if((myVersion & HtmlVersions.h20) != 0) sc.Add("2.0");
			if((myVersion & HtmlVersions.h2x) != 0) sc.Add("2.x");
			if((myVersion & HtmlVersions.hp) != 0) sc.Add("+");
			if((myVersion & HtmlVersions.h30) != 0) sc.Add("3.0");
			if((myVersion & HtmlVersions.h32) != 0) sc.Add("3.2");
			if((myVersion & HtmlVersions.h40) != 0){
				sc.Add("4.0");
			} else if((myVersion & (HtmlVersions.h40t | HtmlVersions.h40f)) != 0){
				sc.Add("(4.0)");
			} else if((myVersion & HtmlVersions.h401) != 0){
				sc.Add("4.01");
			} else if((myVersion & (HtmlVersions.h401t | HtmlVersions.h401f)) != 0){
				sc.Add("(4.01)");
			}
			if((myVersion & HtmlVersions.x10) != 0){
				sc.Add("X1.0");
			} else if((myVersion & (HtmlVersions.x10t | HtmlVersions.x10f)) != 0){
				sc.Add("(X1.0)");
			}
			if((myVersion & HtmlVersions.x11) != 0) sc.Add("X1.1");
			if((myVersion & HtmlVersions.x20) != 0) sc.Add("X2.0");
			return String.Join(", ", sc.ToArray());
		}


	}
}