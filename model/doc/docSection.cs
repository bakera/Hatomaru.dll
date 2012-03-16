using System;
using System.Collections.Generic;
using System.Xml;

namespace Bakera.Hatomaru{

/// <summary>
/// ドキュメントのsectionを処理するクラスです。
/// </summary>
	public class DocSection : IComparable<DocSection>{

		private XmlElement mySectionElement;
		private int myIndex;
		private DocSection myParent;
		private DocSection[] myChildren;

// コンストラクタ

		public DocSection(XmlElement e, DocSection parentSection, int index){
			mySectionElement = e;
			myParent = parentSection;
			myIndex = index;
			List<DocSection> children = new List<DocSection>();
			XmlNodeList xnl = e.GetElementsByTagName(HatomaruActionBase.SectionElement);
			for(int i=0; i < xnl.Count; i++){
				children.Add(new DocSection(xnl[i] as XmlElement, this, i+1));
			}
			myChildren = children.ToArray();
		}


// プロパティ

		public int Index{
			get{return myIndex;}
		}

		public DocSection Parent{
			get{return myParent;}
		}

		public string Title{
			get{
				return mySectionElement.GetAttributeValue(HatomaruActionBase.TitleAttribute);
			}
		}

		public string FullName{
			get{
				string prefix = "";
				DocSection[] sections = GetAnsestorSections();
				foreach(DocSection s in sections){
					prefix += s.Index.ToString() + ".";
				}
				return string.Format("{0}{1}. {2}", prefix, Index, Title);
			}
		}

		public XmlElement SectionElement{
			get{return mySectionElement;}
		}

// メソッド

		public DocSection[] GetAnsestorSections(){
			if(myParent == null) return new DocSection[0];
			List<DocSection> result = new List<DocSection>();
			DocSection s = this.Parent;
			for(;;){
				if(s == null) break;
				result.Add(s);
				s = s.Parent;
			}
			result.Reverse();
			return result.ToArray();
		}

		public virtual int CompareTo(DocSection s){
			return this.Index.CompareTo(s.Index);
		}

	} // End class
} // End Namespace


