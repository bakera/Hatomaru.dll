using System;
using System.Collections.Generic;
using System.Xml;

namespace Bakera.Hatomaru{

	// HTML �̃o�[�W��������L����N���X�̃x�[�X�ƂȂ钊�ۃN���X
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
		/// ���̗v�f�E������ HTML4 �� deprecated �Ƃ���Ă���� true ��Ԃ��܂��B
		/// obsolete �̏ꍇ�� true ��Ԃ��܂��B
		/// </summary>
		public bool IsDeprecated{
			// HTML 4.01 strict �ɂȂ� == deprecated
			get{
				if((myVersion & HtmlVersions.x11) != 0) return false;
				if((myVersion & HtmlVersions.h40) != 0) return false;
				return true;
			}
		}
		/// <summary>
		/// ���̗v�f�E������ HTML4 �� obsolete �Ƃ���Ă���� true ��Ԃ��܂��B
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
		/// ���̗v�f�E������ HTML �d�l���ł���� true ���A�����łȂ���� false ��Ԃ��܂��B
		/// </summary>
		public bool IsSpecified{
			get{
				if(myVersion == 0) return false;
				return true;
			}
		}


// ���\�b�h
		/// <summary>
		/// �o�[�W������\��������𓾂܂��B
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