using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Collections.Generic;

namespace Bakera.Hatomaru{

	/// <summary>
	/// �f�[�^���p�[�X���� Xhtml �� NormalResponse �𐶐�����N���X�ł��B
	/// �p�[�X���Ȃ���A�g�p�����f�[�^�\�[�X�� NormalResponse �ɒǉ����čs���܂��B
	/// </summary>
	public abstract partial class HatomaruActionBase{

		// ToC �֌W
		private XmlNode myToc = null;
		private XmlElement[] myCurrentUl;
		private int myCurrentLevel = 0;
		private int[] mySectionLevelCounter = new int[6];
		private int mySectionBaseLevel = -1;
		private const string HeadingIdSeparator = "-";
		private const string HeadingIdPrefix = "section";
		private const string LargeImageSuffix = "_large";

		// footnote�֌W
		private XmlElement myFootNote = null;
		protected int myFootNoteCount = 0;
		private const string FootNoteLinkTextFormat = "*{0}";
		private const string FootNoteLinkIdPrefix = "footnotelink";
		private const string FootNoteHeadingIdPrefix = "footnote";

		//snip
		public const string SnipText = "(�`�����`)";

		public const string TopicElement = "topic";
		public const string MenuitemElement = "menuitem";
		public const string LinkitemElement = "linkitem";//menuitem�̕ʖ��A�������q�h�L�������g��������Ȃ�
		public const string SectionElement = "section";
		public const string SubSectionElement = "subsection";
		public const string SubSubSectionElement = "subsubsection";
		public const string ListElement = "list";
		public const string NoteElement = "note";
		public const string MemoElement = "memo";
		public const string TableDataElement = "tabledata";
		public const string MenuElement = "menu";
		public const string SampleElement = "sample";
		public const string BqElement = "bq";
		public const string QElement = "q";
		public const string RubyElement = "ruby";
		public const string FigElement = "fig";
		public const string TocElement = "toc";
		public const string AmazonElement = "amazon";
		public const string AmazonImageElement = "amazonimg";
		public const string AmazonInfoElement = "amazoninfo";
		public const string CodeAttribute = "code";
		public const string AElement = "a";
		public const string DfnElement = "dfn";
		public const string DataElement = "data";
		public const string ElemElement = "elem";
		public const string AttrElement = "attr";
		public const string FootNoteElement = "footnote";
		public const string FootNoteListElement = "footnotelist";
		public const string SnipElement = "snip";
		public const string NavItemElement = "navitem";
		public const string DownloadElement = "download";
		public const string RankingElement = "ranking";

		public const string ForAttribute = "for";
		public const string TargetAttribute = "target";
		public const string TitleAttribute = "title";

		public const string HtmlMainContentsId = "main-contents";

		List<string> myASINList = new List<string>();
		private int myRoopCounter = 0;
		private const int RoopMax = 10000;



// �p�u���b�N���\�b�h


		// �m�[�h���X�g���p�[�X���܂��B
		public XmlNode ParseNode(XmlNodeList myNodeList){
			return ParseNode(myNodeList, 1);
		}
		// �m�[�h���X�g���p�[�X���܂��B
		public XmlNode ParseNode(XmlNodeList myNodeList, int headingLevel){
			XmlNode result = Html.CreateDocumentFragment();
			foreach(XmlNode x in myNodeList){
				XmlNode append = ParseNode(x, headingLevel);
				result.AppendChild(append);
			}
			return result;
		}

		// �m�[�h���p�[�X���܂��B
		public XmlNode ParseNode(XmlNode myNode){
			return ParseNode(myNode, 1);
		}
		// �m�[�h���p�[�X���܂��B
		public XmlNode ParseNode(XmlNode myNode, int headingLevel){
			XmlNode result = null;
			switch(myNode.NodeType){
			case XmlNodeType.Document:
				result = ParseNode(myNode.ChildNodes, headingLevel);
				break;

			case XmlNodeType.Element:
				// �v�f�̏ꍇ
				result = ParseElement(myNode as XmlElement, headingLevel);
				// lang��xml:lang����������p������
				string lang = myNode.GetAttributeValue("xml:lang");
				if(lang == null) lang = myNode.GetAttributeValue("lang");
				if(result is XmlElement && !string.IsNullOrEmpty(lang)){
					XmlElement e = result as XmlElement;
					e.SetAttribute("xml:lang", lang);
				}
				break;

			case XmlNodeType.Text:
			case XmlNodeType.CDATA:
				// �e�L�X�g�̏ꍇ
				string text = myNode.Value;
				text = text.Replace("\t", "    ");
				result = Html.CreateTextNode(text);
				break;

			case XmlNodeType.Comment:
			case XmlNodeType.XmlDeclaration:
			case XmlNodeType.DocumentType:
				// �R�����g��XML�錾�͖���
				result = Html.Null;
				break;

			default:
				//���̑��̃m�[�h
				throw new Exception(String.Format("�m�[�h�^�C�v {0} �͏����ł��܂���", myNode.NodeType));
			}

			if(myRoopCounter++ > RoopMax){
				throw new Exception("�p�[�X���[�v�G���[: �p�[�T�[�̏����񐔂����ʂ𒴂��܂����B�������[�v�̉\�������邽�ߏ����𒆎~���܂����B");
			}

			return result;
		}


		// �v�f���p�[�X���܂��B
		private XmlNode ParseElement(XmlElement myNode, int headingLevel){
			switch(myNode.Name){
			case TopicElement:
				return ParseNode(myNode.ChildNodes, headingLevel);
			case MenuitemElement:
			case LinkitemElement:
				return ParseMenuItem(myNode, headingLevel);
			case ListElement:
				return ParseList(myNode, headingLevel);
			case MenuElement:
				return ParseList(myNode, headingLevel, "menu");
			case SectionElement:
			case SubSectionElement:
			case SubSubSectionElement:
				return ParseSection(myNode, headingLevel);
			case NoteElement:
			case MemoElement:
				return ParseNote(myNode, headingLevel);
			case TableDataElement:
				return ParseTableData(myNode, headingLevel);
			case SampleElement:
				return ParseSample(myNode, headingLevel);
			case BqElement:
				return ParseBq(myNode, headingLevel);
			case QElement:
				return ParseQ(myNode, headingLevel);
			case RubyElement:
				return ParseRuby(myNode, headingLevel);
			case FigElement:
				return ParseFig(myNode, headingLevel);
			case TocElement:
				return ParseToc(myNode, headingLevel);
			case AmazonElement:
				return ParseAmazon(myNode, headingLevel);
			case AmazonImageElement:
				return ParseAmazonImage(myNode, headingLevel);
			case AmazonInfoElement:
				return ParseAmazonInfo(myNode, headingLevel);
			case AElement:
				return ParseA(myNode, headingLevel);
			case DfnElement:
				return ParseDfn(myNode, headingLevel);
			case DataElement:
				return ParseData(myNode, headingLevel);
			case ElemElement:
				return ParseElem(myNode, headingLevel);
			case AttrElement:
				return ParseAttr(myNode, headingLevel);
			case FootNoteElement:
				return ParseFootNote(myNode, headingLevel);
			case FootNoteListElement:
				return ParseFootNoteList();
			case SnipElement:
				return ParseSnip(myNode, headingLevel);
			case NavItemElement:
				return ParseNavItem(myNode, headingLevel);
			case DownloadElement:
				return ParseDownload(myNode, headingLevel);
			default:
				return ParseDefaultElement(myNode, headingLevel);
			}
		}



// �ʗv�f

		// default 
		// ���ɑ��삪�K�v�Ȃ��v�f�Ȃǂ����̂܂܏o�͂��܂��B
		private XmlNode ParseDefaultElement(XmlElement myNode, int headingLevel){
			XmlElement result = Html.Create(myNode.Name);

			result.AppendChild(ParseNode(myNode.ChildNodes, headingLevel));

			// �������p��
			foreach(XmlAttribute attr in myNode.Attributes){
				result.SetAttributeNode(Html.ImportNode(attr, true) as XmlAttribute);
			}

			return result;
		}


		// list�v�f���p�[�X���܂��B
		private XmlNode ParseList(XmlElement myNode, int headingLevel, string className){
			XmlElement result = Html.Create("ul", className);
			result.AppendChild(ParseNode(myNode.ChildNodes, headingLevel));
			return result;
		}
		private XmlNode ParseList(XmlElement myNode, int headingLevel){
			return ParseList(myNode, headingLevel, null);
		}


		// section�v�f���p�[�X���܂��B
		private XmlNode ParseSection(XmlElement myNode, int headingLevel){
			XmlElement result = Html.Create("div", "section");

			string title = myNode.GetAttribute("title");
			if(!String.IsNullOrEmpty(title)){
				XmlElement sectionHeading = Html.H(headingLevel, null, title);
				PutFragmentID(sectionHeading, headingLevel, myNode);
				result.AppendChild(sectionHeading);
			}

			result.AppendChild(ParseNode(myNode.ChildNodes, headingLevel + 1));
			return result;
		}


		// note�v�f���p�[�X���܂��B
		private XmlNode ParseNote(XmlElement myNode, int headingLevel){
			string gi = "";
			
			// ��c�� p �v�f�����邩�H
			if(myNode.ParentNode.Name == "p"){
				gi = "em";
			} else {
				gi = "p";
			}

			XmlElement result = Html.Create(gi, "note", "��");
			result.AppendChild(ParseNode(myNode.ChildNodes, headingLevel));

			// ��̗v�f�͕Ԃ��Ȃ�
			if(result.ChildNodes.Count == 0) return null;

			return result;
		}


		// tabledata�v�f���p�[�X���܂��B
		private XmlNode ParseTableData(XmlElement myNode, int headingLevel){
			string hcolStr = myNode.GetAttribute("hcol");
			int hcol = 0;
			Int32.TryParse(hcolStr, out hcol);

			string hrowStr = myNode.GetAttribute("hrow");
			int hrow = 0;
			Int32.TryParse(hrowStr, out hrow);
			XmlDocumentFragment result = Html.CreateDocumentFragment();
			string data = myNode.InnerText.Trim();
			int rowCount = 0;
			foreach(string row in data.Split('\n')){
				XmlElement tr = Html.Create("tr");
				int colCount = 0;
				foreach(string dat in row.Split('\t')){
					XmlElement td;
					if(rowCount < hrow && colCount < hcol){
						td = Html.Create("th");
					} else if (rowCount < hrow){
						td = Html.Create("th");
						td.SetAttribute("scope", "col");
					} else if (colCount < hcol){
						td = Html.Create("th");
						td.SetAttribute("scope", "row");
					} else {
						td = Html.Create("td");
					}
					td.InnerText = dat.Trim();
					tr.AppendChild(td);
					colCount++;
				}
				result.AppendChild(tr);
				rowCount++;
			}

			return result;
		}


		// menuitem�v�f���p�[�X���܂��B
		private XmlNode ParseMenuItem(XmlElement myNode, int headingLevel){
			string linkSource = myNode.GetAttribute("src");
			if(linkSource == null) return Html.Null;
			HatomaruXml hx = GetHatomaruXml(linkSource);

			XmlNode result = Html.CreateDocumentFragment();
			XmlElement p1 = Html.Create("p");
			XmlElement a = Html.A(hx.BasePath);
			a.InnerText = hx.BaseTitle;
			p1.AppendChild(a);

			result.AppendChild(p1);
			XmlElement p2 = Html.Create("p", "description", hx.Description);
			result.AppendChild(p2);

			return result;
		}


		// navitem�v�f���p�[�X���܂��B
		private XmlNode ParseNavItem(XmlElement myNode, int headingLevel){
			XmlElement result = Html.Create("li");
			XmlElement a = Html.GetA(myNode);
			result.AppendChild(a);
			string itemPath = myNode.GetAttributeValue("href");
			if(myPath.ToString().StartsWith(itemPath)){
				result.SetAttribute("class", "stay");
				myChildrenNavPoint = result;
			}
			return result;
		}


		// ruby�v�f���p�[�X���܂��B
		private XmlNode ParseRuby(XmlElement myNode, int headingLevel){

			string rtStr = myNode.GetAttribute("rt");

			// rt �������Ȃ���� W3C �� ruby �v�f�Ƃ݂Ȃ�
			if(rtStr == null){
				XmlElement noRtResult = Html.Create("ruby");
				noRtResult.AppendChild(ParseNode(myNode.ChildNodes, headingLevel));
				return noRtResult;
			}

			XmlElement rb = Html.Create("rb");
			rb.AppendChild(ParseNode(myNode.ChildNodes, headingLevel));

			XmlElement srp = Html.Create("rp", null, "(");
			XmlElement rt = Html.Create("rt", null, rtStr);
			XmlElement erp = Html.Create("rp", null, ")");
			XmlElement result = Html.Create("ruby", null, rb, srp, rt, erp);
			return result;
		}


		// sample�v�f���p�[�X���܂��B
		private XmlNode ParseSample(XmlElement myNode, int headingLevel){
			string classString = "sample";

			XmlElement result = Html.Create("div", classString);
			string text = myNode.InnerText.Replace("\r\n", "\n");

			if(text.IndexOf('\t') >= 0 || text.IndexOf("\n\n") >= 0){
				XmlElement pre = Html.Create("pre");
				pre.AppendChild(ParseNode(myNode.ChildNodes, headingLevel));
				result.AppendChild(pre);
			} else {
				XmlElement p = Html.P();
				for(int i = 0; i < myNode.ChildNodes.Count; i++){
					XmlNode x = myNode.ChildNodes[i];
					XmlNode append = ParseNode(x, headingLevel);
					if(append == null) continue;
					if(append.NodeType == XmlNodeType.Text){
						string innerText = append.Value;

						// �ŏ��ƍŌ�̃e�L�X�g�m�[�h���� Trim ����
						if(i == 0) innerText = innerText.TrimStart('\n', '\r');
						if(i == myNode.ChildNodes.Count-1) innerText = innerText.TrimEnd('\n', '\r');

						string[] sampleText = innerText.Split('\n');
						for(int j = 0; j < sampleText.Length; j++){
							XmlNode t = Html.CreateTextNode(sampleText[j].Trim('\r'));
							p.AppendChild(t);
							if(j == sampleText.Length - 1) break;
							XmlNode br = Html.Create("br");
							p.AppendChild(br);
						}
					} else {
						p.AppendChild(append);
					}
				}
				result.AppendChild(p);
			}
			return result;
		}


		// bq�v�f���p�[�X���܂��B
		private XmlNode ParseBq(XmlElement myNode, int headingLevel){
			XmlElement result = null;

			// �Ƃ肠���������l���擾
			string cite = myNode.GetAttribute("cite");
			string title = myNode.GetAttribute("title");
			string author = myNode.GetAttribute("author");
			string note = myNode.GetAttribute("note");

			// ���ʂ� title����
			string resultTitle = "";
			if(string.IsNullOrEmpty(title)) resultTitle = string.Format("�o�T : {0}", title);
			if(string.IsNullOrEmpty(author)) resultTitle += string.Format(" ({0})", author);
			if(string.IsNullOrEmpty(cite)) resultTitle += "\n" + cite;

			result = Html.Create("div", "quote-and-cite");
			XmlElement bqElement = Html.Create("blockquote");

			// ���g�̏���
			XmlNodeList children = myNode.ChildNodes;
			if(children.Count == 1 && children[0].NodeType == XmlNodeType.Text){
				//bq ���� #PCDATA �������̏ꍇ�� p �v�f�Ƃ���
				string[] sampleText = children[0].Value.Trim().Split('\n');
				for(int i = 0; i < sampleText.Length; i++){
					string s = sampleText[i].Trim();
					if(string.IsNullOrEmpty(s)) continue;
					bqElement.AppendChild(Html.P(null, s));
				}
			} else {
				bqElement.AppendChild(ParseNode(myNode.ChildNodes, headingLevel));
			}

			// ���ʂ� cite����
			if(!string.IsNullOrEmpty(cite)) bqElement.SetAttribute("cite", cite);
			result.AppendChild(bqElement);

			XmlElement resultCiteElement = GetBqCite(title, author, cite);
			// �o�T��ǉ�
			if(resultCiteElement != null){
				resultCiteElement.PrependChild(Html.Text("�ȏ�A"));
				resultCiteElement.AppendChild(Html.Text(" ���"));
				result.AppendChild(resultCiteElement);
			}

			// ����
			if(note != null && note != ""){
				XmlElement resultNoteElement = Html.P("citenote");
				resultNoteElement.InnerText = note;
				resultNoteElement.PrependChild(Html.CreateTextNode("��"));
				result.AppendChild(resultNoteElement);
			}
			return result;
		} // End private Method ParseQuoteElement


		// title, author, cite ����o�T�̗v�f���擾���܂��B
		private XmlElement GetBqCite(string title, string author, string cite){
			// �^�C�g���� URL ���Ȃ��ꍇ�͉����Ȃ�
			if(string.IsNullOrEmpty(title) && string.IsNullOrEmpty(cite)) return null;

			XmlElement resultCiteElement = Html.P("cite");
			if(!string.IsNullOrEmpty(author)){
				resultCiteElement.AppendChild((Html.Text(author + "��")));
			}

			// �^�C�g�������邪 URL ���Ȃ��ꍇ
			if(string.IsNullOrEmpty(cite)){
				resultCiteElement.AppendChild(Html.Text(title));
				return resultCiteElement;
			}

			// URL ������ꍇ�A�����N���邩��
			// �^�C�g�����Ȃ���� URL ���^�C�g���Ƃ���
			if(string.IsNullOrEmpty(title)) title = cite;

			XmlElement resultCiteAnchor = Html.A(new Uri(cite));
			resultCiteAnchor.InnerText = title;
			resultCiteElement.AppendChild(resultCiteAnchor);
			return resultCiteElement;
		}

		// q�v�f���p�[�X���܂��B
		private XmlNode ParseQ(XmlElement myNode, int headingLevel){
			XmlElement result = null;
			XmlElement q = Html.Create("q");

			// �Ƃ肠���������l���擾
			string cite = myNode.GetAttribute("cite");
			if(string.IsNullOrEmpty(cite)){
				result = q;
				result.AppendChild(ParseNode(myNode.ChildNodes, headingLevel));
			} else {
				result = Html.A(new Uri(cite));
				result.AppendChild(q);
				q.SetAttribute("cite", cite);
				q.AppendChild(ParseNode(myNode.ChildNodes, headingLevel));
			}
			return result;
		} // End private Method ParseQuoteElement


		// fig�v�f����������
		private XmlNode ParseFig(XmlElement myNode, int headingLevel){

			string imgId = myNode.GetAttribute("name");
			if(string.IsNullOrEmpty(imgId)){
				return Html.Comment("fig : no imgId");
			}

			// �摜��T��
			// �^�[�Q�b�g�t�@�C���f�B���N�g��/image/{name}.*
			DirectoryInfo imgDir = myModel.ImageDirectoryPath;
			if(!imgDir.Exists){
				return Html.Comment("directory not found: {0} - {1}", imgDir.FullName, imgId);
			}
			HatomaruFile imgFile = SearchImage(imgDir, imgId);
			try{
				Uri srcUri = imgFile.GetLinkUri();
				System.Drawing.Size size = imgFile.Size;
				XmlElement img = Html.Img(srcUri, myNode.InnerText, size);
				HatomaruFile largeImg = SearchImage(imgDir, imgId + LargeImageSuffix);
				if(largeImg == null){
					return Html.Create("div", "figure", img);
				}
				XmlElement imgLinkAnchor = Html.A(largeImg.GetLinkUri());
				imgLinkAnchor.AppendChild(img);
				return Html.Create("div", "figure", imgLinkAnchor);

			} catch(Exception e){
				return Html.Comment("{0} - Exception: {1}", imgFile.File.FullName, e);
			}
		}

		private HatomaruFile SearchImage(DirectoryInfo dir, string imgId){
			FileInfo[] files = dir.GetFiles(imgId + "*");
			if(files.Length == 0) return null;
			if(files.Length > 1){
				Array.Sort(files, (x,y) => x.Name.Length - y.Name.Length);
			}
			HatomaruFile result = myModel.Manager.GetFile(files[0]);
			return result;
		}


		// toc�v�f����������
		private XmlNode ParseToc(XmlElement myNode, int headingLevel){
			CheckMyToc();
			return Html.Create("div", "toc", myToc);
		}


		// a�v�f���������܂��B
		public XmlNode ParseA(XmlNode myNode, int headingLevel){
			string href = myNode.GetAttributeValue("href");
			if(string.IsNullOrEmpty(href)){
				href = myNode.InnerText;
			}
			return GetLink(myNode, href);
		}


		// download�v�f���������܂��B
		public XmlNode ParseDownload(XmlNode myNode, int headingLevel){
			string href = myNode.GetAttributeValue("href");
			if(string.IsNullOrEmpty(href)){
				href = myNode.InnerText;
			}
			AbsPath path = new AbsPath(href);
			if(path.Equals(myPath)) throw new Exception("�_�E�����[�h�����N�Ɏ��g���w�肳��Ă��܂��B");
			FileResponse dlResponse = myModel.Manager.GetResponse(path) as FileResponse;
			if(dlResponse == null){
				return Html.Text("(" + href + "�̃_�E�����[�h�͗��p�ł��܂���)");
			}
			XmlElement a = Html.A(path);
			string inner = string.Format("{0} ({1} {2})", dlResponse.FileSource.Name, dlResponse.ExtInfo.Description, dlResponse.LengthFormat);
			a.InnerText = inner;
			return a;
		}


		// dfn�v�f���������܂��B
		public XmlNode ParseDfn(XmlNode myNode, int headingLevel){
			XmlElement result = Html.Create("dfn", "glossary");
			if(Glossary != null){
				string name = myNode.GetAttributeValue(TargetAttribute);
				if(string.IsNullOrEmpty(name)) name = myNode.InnerText;
				GlossaryWord gw = Glossary.GetWordByName(name);
				if(gw != null){
					AbsPath wordLink = Glossary.BasePath.Combine(gw.Name.PathEncode());
					XmlElement a = Html.A(wordLink);
					a.AppendChild(ParseNode(myNode.ChildNodes, headingLevel));
					result.AppendChild(a);
					return result;
				}
			}
			result.AppendChild(ParseNode(myNode.ChildNodes, headingLevel));
			return result;
		}

		// data�v�f���������܂��B
		public XmlNode ParseData(XmlNode myNode, int headingLevel){
			XmlElement result = Html.Create("dfn", "data");
			if(HtmlRef != null){
				string name = myNode.GetAttributeValue(TargetAttribute);
				if(string.IsNullOrEmpty(name)) name = myNode.InnerText;
				name = name.GetIdString();
				HtmlData hd = HtmlRef.GetDataByName(name);
				if(hd != null){
					AbsPath link = HtmlRef.BasePath.Combine(hd.LinkId, hd.Id.PathEncode());
					XmlElement a = Html.A(link);
					a.AppendChild(ParseNode(myNode.ChildNodes, headingLevel));
					result.AppendChild(a);
					return result;
				}
			}
			result.AppendChild(ParseNode(myNode.ChildNodes, headingLevel));
			return result;
		}

		// elem�v�f���������܂��B
		public XmlNode ParseElem(XmlNode myNode, int headingLevel){
			XmlElement result = Html.Create("dfn", "element");
			if(HtmlRef != null){
				string name = myNode.GetAttributeValue(TargetAttribute);
				if(string.IsNullOrEmpty(name)) name = myNode.InnerText;
				name = name.GetIdString();
				HtmlElement he = HtmlRef.GetElement(name);
				if(he != null){
					AbsPath link = HtmlRef.BasePath.Combine(he.LinkId, he.Id.PathEncode());
					XmlElement a = Html.A(link);
					a.AppendChild(ParseNode(myNode.ChildNodes, headingLevel));
					result.AppendChild(a);
					return result;
				}
			}
			result.AppendChild(ParseNode(myNode.ChildNodes, headingLevel));
			return result;
		}

		// attr�v�f���������܂��B
		public XmlNode ParseAttr(XmlNode myNode, int headingLevel){
			XmlElement result = Html.Create("dfn", "element");
			if(HtmlRef != null){
				string name = myNode.GetAttributeValue(TargetAttribute);
				if(string.IsNullOrEmpty(name)) name = myNode.InnerText;
				name = name.GetIdString();
				string forStr = myNode.GetAttributeValue(ForAttribute);
				HtmlAttribute ha = HtmlRef.GetAttribute(name, forStr);
				if(ha != null){
					AbsPath link = HtmlRef.BasePath.Combine(ha.LinkId, ha.Id.PathEncode());
					XmlElement a = Html.A(link);
					a.AppendChild(ParseNode(myNode.ChildNodes, headingLevel));
					result.AppendChild(a);
					return result;
				}
			}
			result.AppendChild(ParseNode(myNode.ChildNodes, headingLevel));
			return result;
		}


		// footnote�v�f���������܂��B
		public XmlNode ParseFootNote(XmlNode myNode, int headingLevel){

			// �r���̂Ԃ牺���������
			if(myFootNote == null){
				myFootNote = Html.Create("dl", "footnotes");
			}
			string fnTitle = myNode.GetAttributeValue(TitleAttribute);
			myFootNoteCount++;
			string fnHeadingId = FootNoteHeadingIdPrefix + myFootNoteCount.ToString();
			string fnLinkId = FootNoteLinkIdPrefix + myFootNoteCount.ToString();
			string fnLinkText = string.Format(FootNoteLinkTextFormat, myFootNoteCount);

			XmlElement fnHeadingAnchor = Html.Create("a");
			fnHeadingAnchor.SetAttribute("name", fnHeadingId);
			fnHeadingAnchor.SetAttribute("id", fnHeadingId);
			fnHeadingAnchor.SetAttribute("href", "#" + fnLinkId);
			fnHeadingAnchor.InnerText = fnLinkText;

			XmlElement fnDt = Html.Create("dt", null, fnHeadingAnchor);
			if(!string.IsNullOrEmpty(fnTitle)) fnDt.AppendChild(Html.Text(" " + fnTitle));
			XmlElement fnDd = Html.Create("dd");
			fnDd.AppendChild(ParseNode(myNode.ChildNodes, headingLevel));
			myFootNote.AppendChild(fnDt);
			myFootNote.AppendChild(fnDd);

			XmlElement fnLinkAnchor = Html.Create("a", FootNoteLinkIdPrefix);
			fnLinkAnchor.SetAttribute("name", fnLinkId);
			fnLinkAnchor.SetAttribute("id", fnLinkId);
			fnLinkAnchor.SetAttribute("href", "#" + fnHeadingId);
			fnLinkAnchor.InnerText = fnLinkText;
			if(!string.IsNullOrEmpty(fnTitle)) fnLinkAnchor.SetAttribute("title", fnTitle);

			return fnLinkAnchor;
		}

		// footnote�v�f���������܂��B
		public XmlNode ParseFootNoteList(){
			XmlNode result = Html.CreateDocumentFragment();
			if(myFootNote == null) return result;
			result.AppendChild(myFootNote);
			// ���Z�b�g
			myFootNote = null;
			myFootNoteCount = 0;
			return result;
		}

		// snip�v�f���������܂��B
		private XmlNode ParseSnip(XmlElement myNode, int headingLevel){
			XmlElement result = Html.P("snip", Html.Create("em", null, SnipText));
			return result;
		}


		private HatomaruXml GetHatomaruXml(string linkSource){
			if(linkSource == null) return null;
			HatomaruXml hx = myModel.GetDataByPathString(linkSource);
			// �f�[�^�\�[�X�擾�E�L�^
			if(hx == null) throw new Exception(linkSource + "�� XML �͎擾�ł��܂���ł����B");
			myResponse.AddDataSource(hx);
			return hx;
		}



// Amazon�֘A

		// Amazon �ւ̃����N�𐶐�����
		public XmlNode ParseAmazon(XmlNode myNode, int headingLevel){
			XmlElement result = Html.Span("amazon");
			string code = myNode.GetAttributeValue(CodeAttribute);
			if(string.IsNullOrEmpty(code)){
				result.AppendChild(GetLink(myNode, AmazonManager.AmazonTopUrl));
				return result;
			}
			string amazonHref = String.Format(AmazonManager.AmazonHrefFormat, code);
			string amazonSrc = String.Format(AmazonManager.AmazonSrcFormat, code);

			// URL ���쐬
			XmlNode amazonA = GetLink(myNode, amazonHref);

			XmlElement amazonImg = Html.Img(amazonSrc, "", 1, 1);

			result.AppendChild(amazonA);
			result.AppendChild(amazonImg);

			myASINList.AddNew(code);
			return result;
		}


		// Amazon �̉摜�𐶐�����
		public XmlNode ParseAmazonImage(XmlNode myNode, int headingLevel){
			string code = myNode.GetAttributeValue(CodeAttribute);
			if(string.IsNullOrEmpty(code)) return Html.Null;
			return GetAmazonImage(code);
		}


		// Amazon�ւ̃����N��ǉ�
		private XmlNode ParseAmazonInfo(XmlNode myNode, int headingLevel){
			string code = myNode.GetAttributeValue(CodeAttribute);
			if(!string.IsNullOrEmpty(code)) myASINList.AddNew(code);
			return Html.CreateDocumentFragment();
		}


		/// <summary>
		/// ���݂̃p�[�X�ŎQ�Ƃ��� Amazon �̉摜���X�g�𓾂܂��B
		/// </summary>
		public XmlNode GetAmazonImageList(){
			if(myASINList.Count == 0) return Html.CreateDocumentFragment();
			XmlElement result = Html.Div("amazon-list");
			XmlElement p = Html.P();
			foreach(string s in myASINList){
				XmlNode x = GetAmazonImage(s);
				if(x == null) continue;
				p.AppendChild(x);
			}
			myASINList.Clear();
			result.AppendChild(p);
			return result;
		}


		/// <summary>
		/// ASIN �ɑΉ����� Amazon �̏��i�摜�����N�𓾂܂��B
		/// </summary>
		public XmlNode GetAmazonImage(string asin){
			Model.Manager.Log.Add("GetAmazonImage start: " + asin);
			AmazonItem item = Model.Manager.AmazonManager.GetItem(asin);
			if(item == null) return Html.Null;

			AmazonImage image = item.Image;
			XmlElement a = Html.Create("a");
			a.SetAttribute("href", item.DetailPageUrl);
			if(image == null){
				image = AmazonImage.GetNoImage();
			}
			XmlElement img = Html.Img(image.Url, item.Title, image.Width, image.Height);

			a.AppendChild(img);
			Model.Manager.Log.Add("GetAmazonImage end: " + asin);
			return a;
		}

// �⏕�n�v���C�x�[�g���\�b�h

		private XmlNode GetLink(XmlNode myNode, string href){
			Uri u = href.ToUri();
			XmlNode result = Html.CreateDocumentFragment();
			XmlElement a = Html.A(u);
			a.AppendChild(ParseNode(myNode.ChildNodes, 0));
			result.AppendChild(a);
			if(myNode.ChildNodes.Count == 1 && (myNode.ChildNodes[0].Name == "img")){
				
			} else {
				result.AppendChild(DomainNotice(u));
			}
			return result;
		}


		// �����N�̌��ɂ��h���C�������擾���܂��B
		private XmlNode DomainNotice(Uri u){
			if(!u.IsAbsoluteUri || u.Host == Html.BaseUri.Host || string.IsNullOrEmpty(u.Host)) return Html.CreateDocumentFragment();
			XmlNode result = Html.CreateDocumentFragment();
			result.AppendChild(Html.Space);
			result.AppendChild(Html.Create("em", "domain-info", "(" + u.Host + ")"));
			return result;
		}


		// ���o���ɒʂ��ԍ��� Fragment ID ��t������
		// myAutoHeadingNumbering = true �Ȃ猩�o�������񎩐g�ɂ��ʂ��ԍ���t������
		private void PutFragmentID(XmlElement headingElement, int headingLevel, XmlNode sectionNode){
			string headingName = headingElement.Name;
			if(headingName.Length < 2) return;

			XmlAttribute idAttr = headingElement.Attributes["id"];
			if(idAttr != null) return;

			// ���o���x�[�X���x�����ݒ肳��Ă��Ȃ���Ύ擾
			// ���o���x�[�X���x�� = �A�Ԃ�U��Œ჌�x���̌��o�����x��
			if(mySectionBaseLevel < 0){
				mySectionBaseLevel = headingLevel;
			}

			// �g�p����J�E���^�̃C���f�N�X���擾
			int countIndex = headingLevel - mySectionBaseLevel;

			// ���ʂ̌��o���J�E���^�����Z�b�g
			for(int i = 5; i > countIndex; i--){
				mySectionLevelCounter[i] = 0;
			}
			mySectionLevelCounter[countIndex]++;

			StringBuilder result = new StringBuilder();
			result.Append(mySectionLevelCounter[0].ToString());
			for(int i = 1; i <= countIndex; i++){
				result.Append(HeadingIdSeparator);
				result.Append(mySectionLevelCounter[i].ToString());
			}

			// ���o���ɃZ�b�g
			string headingNumber = result.ToString();
			string fragmentId = HeadingIdPrefix + headingNumber;
			headingElement.SetAttribute("id", fragmentId);

			// Toc �ɒǉ�
			CheckMyToc();

			// �q�������� UL �𑫂��Ă���
			if(myCurrentLevel < countIndex){
				XmlElement lc = myCurrentUl[countIndex-1].LastChild as XmlElement;
				if(lc != null){
					myCurrentUl[countIndex] = Html.Create("ul");
					lc.AppendChild(myCurrentUl[countIndex]);
				}
			}
			XmlElement li = Html.Create("li");
			XmlElement a = Html.Create("a");
			a.SetAttribute("href", "#" + fragmentId);

			a.InnerText = headingElement.InnerText;

			// �����𑫂�
//			XmlElement section = sectionNode as XmlElement;
//			a.InnerText += "(" + section.InnerText.Length.ToString() + ")";


			li.AppendChild(a);
			myCurrentUl[countIndex].AppendChild(li);

			myCurrentLevel = countIndex;
		}

		private void CheckMyToc(){
			if(myToc == null){
				myToc = Html.CreateDocumentFragment();
				myCurrentUl = new XmlElement[5];
				myCurrentUl[0] = Html.Create("ul");
				myToc.AppendChild(myCurrentUl[0]);
			}
		}

	} // end class Parser

} // end namespace Bakea

