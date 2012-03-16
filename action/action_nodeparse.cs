using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Collections.Generic;

namespace Bakera.Hatomaru{

	/// <summary>
	/// データをパースして Xhtml と NormalResponse を生成するクラスです。
	/// パースしながら、使用したデータソースを NormalResponse に追加して行きます。
	/// </summary>
	public abstract partial class HatomaruActionBase{

		// ToC 関係
		private XmlNode myToc = null;
		private XmlElement[] myCurrentUl;
		private int myCurrentLevel = 0;
		private int[] mySectionLevelCounter = new int[6];
		private int mySectionBaseLevel = -1;
		private const string HeadingIdSeparator = "-";
		private const string HeadingIdPrefix = "section";
		private const string LargeImageSuffix = "_large";

		// footnote関係
		private XmlElement myFootNote = null;
		protected int myFootNoteCount = 0;
		private const string FootNoteLinkTextFormat = "*{0}";
		private const string FootNoteLinkIdPrefix = "footnotelink";
		private const string FootNoteHeadingIdPrefix = "footnote";

		//snip
		public const string SnipText = "(～中略～)";

		public const string TopicElement = "topic";
		public const string MenuitemElement = "menuitem";
		public const string LinkitemElement = "linkitem";//menuitemの別名、ただし子ドキュメント扱いされない
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



// パブリックメソッド


		// ノードリストをパースします。
		public XmlNode ParseNode(XmlNodeList myNodeList){
			return ParseNode(myNodeList, 1);
		}
		// ノードリストをパースします。
		public XmlNode ParseNode(XmlNodeList myNodeList, int headingLevel){
			XmlNode result = Html.CreateDocumentFragment();
			foreach(XmlNode x in myNodeList){
				XmlNode append = ParseNode(x, headingLevel);
				result.AppendChild(append);
			}
			return result;
		}

		// ノードをパースします。
		public XmlNode ParseNode(XmlNode myNode){
			return ParseNode(myNode, 1);
		}
		// ノードをパースします。
		public XmlNode ParseNode(XmlNode myNode, int headingLevel){
			XmlNode result = null;
			switch(myNode.NodeType){
			case XmlNodeType.Document:
				result = ParseNode(myNode.ChildNodes, headingLevel);
				break;

			case XmlNodeType.Element:
				// 要素の場合
				result = ParseElement(myNode as XmlElement, headingLevel);
				// langかxml:langがあったら継承する
				string lang = myNode.GetAttributeValue("xml:lang");
				if(lang == null) lang = myNode.GetAttributeValue("lang");
				if(result is XmlElement && !string.IsNullOrEmpty(lang)){
					XmlElement e = result as XmlElement;
					e.SetAttribute("xml:lang", lang);
				}
				break;

			case XmlNodeType.Text:
			case XmlNodeType.CDATA:
				// テキストの場合
				string text = myNode.Value;
				text = text.Replace("\t", "    ");
				result = Html.CreateTextNode(text);
				break;

			case XmlNodeType.Comment:
			case XmlNodeType.XmlDeclaration:
			case XmlNodeType.DocumentType:
				// コメントやXML宣言は無視
				result = Html.Null;
				break;

			default:
				//その他のノード
				throw new Exception(String.Format("ノードタイプ {0} は処理できません", myNode.NodeType));
			}

			if(myRoopCounter++ > RoopMax){
				throw new Exception("パースループエラー: パーサーの処理回数が一定量を超えました。無限ループの可能性があるため処理を中止しました。");
			}

			return result;
		}


		// 要素をパースします。
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



// 個別要素

		// default 
		// 特に操作が必要ない要素などをそのまま出力します。
		private XmlNode ParseDefaultElement(XmlElement myNode, int headingLevel){
			XmlElement result = Html.Create(myNode.Name);

			result.AppendChild(ParseNode(myNode.ChildNodes, headingLevel));

			// 属性を継承
			foreach(XmlAttribute attr in myNode.Attributes){
				result.SetAttributeNode(Html.ImportNode(attr, true) as XmlAttribute);
			}

			return result;
		}


		// list要素をパースします。
		private XmlNode ParseList(XmlElement myNode, int headingLevel, string className){
			XmlElement result = Html.Create("ul", className);
			result.AppendChild(ParseNode(myNode.ChildNodes, headingLevel));
			return result;
		}
		private XmlNode ParseList(XmlElement myNode, int headingLevel){
			return ParseList(myNode, headingLevel, null);
		}


		// section要素をパースします。
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


		// note要素をパースします。
		private XmlNode ParseNote(XmlElement myNode, int headingLevel){
			string gi = "";
			
			// 先祖に p 要素があるか？
			if(myNode.ParentNode.Name == "p"){
				gi = "em";
			} else {
				gi = "p";
			}

			XmlElement result = Html.Create(gi, "note", "※");
			result.AppendChild(ParseNode(myNode.ChildNodes, headingLevel));

			// 空の要素は返さない
			if(result.ChildNodes.Count == 0) return null;

			return result;
		}


		// tabledata要素をパースします。
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


		// menuitem要素をパースします。
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


		// navitem要素をパースします。
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


		// ruby要素をパースします。
		private XmlNode ParseRuby(XmlElement myNode, int headingLevel){

			string rtStr = myNode.GetAttribute("rt");

			// rt 属性がなければ W3C の ruby 要素とみなす
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


		// sample要素をパースします。
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

						// 最初と最後のテキストノードだけ Trim する
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


		// bq要素をパースします。
		private XmlNode ParseBq(XmlElement myNode, int headingLevel){
			XmlElement result = null;

			// とりあえず属性値を取得
			string cite = myNode.GetAttribute("cite");
			string title = myNode.GetAttribute("title");
			string author = myNode.GetAttribute("author");
			string note = myNode.GetAttribute("note");

			// 結果の title属性
			string resultTitle = "";
			if(string.IsNullOrEmpty(title)) resultTitle = string.Format("出典 : {0}", title);
			if(string.IsNullOrEmpty(author)) resultTitle += string.Format(" ({0})", author);
			if(string.IsNullOrEmpty(cite)) resultTitle += "\n" + cite;

			result = Html.Create("div", "quote-and-cite");
			XmlElement bqElement = Html.Create("blockquote");

			// 中身の処理
			XmlNodeList children = myNode.ChildNodes;
			if(children.Count == 1 && children[0].NodeType == XmlNodeType.Text){
				//bq 直下 #PCDATA 直書きの場合は p 要素とする
				string[] sampleText = children[0].Value.Trim().Split('\n');
				for(int i = 0; i < sampleText.Length; i++){
					string s = sampleText[i].Trim();
					if(string.IsNullOrEmpty(s)) continue;
					bqElement.AppendChild(Html.P(null, s));
				}
			} else {
				bqElement.AppendChild(ParseNode(myNode.ChildNodes, headingLevel));
			}

			// 結果の cite属性
			if(!string.IsNullOrEmpty(cite)) bqElement.SetAttribute("cite", cite);
			result.AppendChild(bqElement);

			XmlElement resultCiteElement = GetBqCite(title, author, cite);
			// 出典を追加
			if(resultCiteElement != null){
				resultCiteElement.PrependChild(Html.Text("以上、"));
				resultCiteElement.AppendChild(Html.Text(" より"));
				result.AppendChild(resultCiteElement);
			}

			// メモ
			if(note != null && note != ""){
				XmlElement resultNoteElement = Html.P("citenote");
				resultNoteElement.InnerText = note;
				resultNoteElement.PrependChild(Html.CreateTextNode("※"));
				result.AppendChild(resultNoteElement);
			}
			return result;
		} // End private Method ParseQuoteElement


		// title, author, cite から出典の要素を取得します。
		private XmlElement GetBqCite(string title, string author, string cite){
			// タイトルも URL もない場合は何もなし
			if(string.IsNullOrEmpty(title) && string.IsNullOrEmpty(cite)) return null;

			XmlElement resultCiteElement = Html.P("cite");
			if(!string.IsNullOrEmpty(author)){
				resultCiteElement.AppendChild((Html.Text(author + "の")));
			}

			// タイトルがあるが URL がない場合
			if(string.IsNullOrEmpty(cite)){
				resultCiteElement.AppendChild(Html.Text(title));
				return resultCiteElement;
			}

			// URL がある場合、リンクするかも
			// タイトルがなければ URL をタイトルとする
			if(string.IsNullOrEmpty(title)) title = cite;

			XmlElement resultCiteAnchor = Html.A(new Uri(cite));
			resultCiteAnchor.InnerText = title;
			resultCiteElement.AppendChild(resultCiteAnchor);
			return resultCiteElement;
		}

		// q要素をパースします。
		private XmlNode ParseQ(XmlElement myNode, int headingLevel){
			XmlElement result = null;
			XmlElement q = Html.Create("q");

			// とりあえず属性値を取得
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


		// fig要素を処理する
		private XmlNode ParseFig(XmlElement myNode, int headingLevel){

			string imgId = myNode.GetAttribute("name");
			if(string.IsNullOrEmpty(imgId)){
				return Html.Comment("fig : no imgId");
			}

			// 画像を探す
			// ターゲットファイルディレクトリ/image/{name}.*
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


		// toc要素を処理する
		private XmlNode ParseToc(XmlElement myNode, int headingLevel){
			CheckMyToc();
			return Html.Create("div", "toc", myToc);
		}


		// a要素を処理します。
		public XmlNode ParseA(XmlNode myNode, int headingLevel){
			string href = myNode.GetAttributeValue("href");
			if(string.IsNullOrEmpty(href)){
				href = myNode.InnerText;
			}
			return GetLink(myNode, href);
		}


		// download要素を処理します。
		public XmlNode ParseDownload(XmlNode myNode, int headingLevel){
			string href = myNode.GetAttributeValue("href");
			if(string.IsNullOrEmpty(href)){
				href = myNode.InnerText;
			}
			AbsPath path = new AbsPath(href);
			if(path.Equals(myPath)) throw new Exception("ダウンロードリンクに自身が指定されています。");
			FileResponse dlResponse = myModel.Manager.GetResponse(path) as FileResponse;
			if(dlResponse == null){
				return Html.Text("(" + href + "のダウンロードは利用できません)");
			}
			XmlElement a = Html.A(path);
			string inner = string.Format("{0} ({1} {2})", dlResponse.FileSource.Name, dlResponse.ExtInfo.Description, dlResponse.LengthFormat);
			a.InnerText = inner;
			return a;
		}


		// dfn要素を処理します。
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

		// data要素を処理します。
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

		// elem要素を処理します。
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

		// attr要素を処理します。
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


		// footnote要素を処理します。
		public XmlNode ParseFootNote(XmlNode myNode, int headingLevel){

			// 脚注のぶら下がり先を作る
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

		// footnote要素を処理します。
		public XmlNode ParseFootNoteList(){
			XmlNode result = Html.CreateDocumentFragment();
			if(myFootNote == null) return result;
			result.AppendChild(myFootNote);
			// リセット
			myFootNote = null;
			myFootNoteCount = 0;
			return result;
		}

		// snip要素を処理します。
		private XmlNode ParseSnip(XmlElement myNode, int headingLevel){
			XmlElement result = Html.P("snip", Html.Create("em", null, SnipText));
			return result;
		}


		private HatomaruXml GetHatomaruXml(string linkSource){
			if(linkSource == null) return null;
			HatomaruXml hx = myModel.GetDataByPathString(linkSource);
			// データソース取得・記録
			if(hx == null) throw new Exception(linkSource + "の XML は取得できませんでした。");
			myResponse.AddDataSource(hx);
			return hx;
		}



// Amazon関連

		// Amazon へのリンクを生成する
		public XmlNode ParseAmazon(XmlNode myNode, int headingLevel){
			XmlElement result = Html.Span("amazon");
			string code = myNode.GetAttributeValue(CodeAttribute);
			if(string.IsNullOrEmpty(code)){
				result.AppendChild(GetLink(myNode, AmazonManager.AmazonTopUrl));
				return result;
			}
			string amazonHref = String.Format(AmazonManager.AmazonHrefFormat, code);
			string amazonSrc = String.Format(AmazonManager.AmazonSrcFormat, code);

			// URL を作成
			XmlNode amazonA = GetLink(myNode, amazonHref);

			XmlElement amazonImg = Html.Img(amazonSrc, "", 1, 1);

			result.AppendChild(amazonA);
			result.AppendChild(amazonImg);

			myASINList.AddNew(code);
			return result;
		}


		// Amazon の画像を生成する
		public XmlNode ParseAmazonImage(XmlNode myNode, int headingLevel){
			string code = myNode.GetAttributeValue(CodeAttribute);
			if(string.IsNullOrEmpty(code)) return Html.Null;
			return GetAmazonImage(code);
		}


		// Amazonへのリンクを追加
		private XmlNode ParseAmazonInfo(XmlNode myNode, int headingLevel){
			string code = myNode.GetAttributeValue(CodeAttribute);
			if(!string.IsNullOrEmpty(code)) myASINList.AddNew(code);
			return Html.CreateDocumentFragment();
		}


		/// <summary>
		/// 現在のパースで参照した Amazon の画像リストを得ます。
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
		/// ASIN に対応する Amazon の商品画像リンクを得ます。
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

// 補助系プライベートメソッド

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


		// リンクの後ろにつくドメイン情報を取得します。
		private XmlNode DomainNotice(Uri u){
			if(!u.IsAbsoluteUri || u.Host == Html.BaseUri.Host || string.IsNullOrEmpty(u.Host)) return Html.CreateDocumentFragment();
			XmlNode result = Html.CreateDocumentFragment();
			result.AppendChild(Html.Space);
			result.AppendChild(Html.Create("em", "domain-info", "(" + u.Host + ")"));
			return result;
		}


		// 見出しに通し番号の Fragment ID を付加する
		// myAutoHeadingNumbering = true なら見出し文字列自身にも通し番号を付加する
		private void PutFragmentID(XmlElement headingElement, int headingLevel, XmlNode sectionNode){
			string headingName = headingElement.Name;
			if(headingName.Length < 2) return;

			XmlAttribute idAttr = headingElement.Attributes["id"];
			if(idAttr != null) return;

			// 見出しベースレベルが設定されていなければ取得
			// 見出しベースレベル = 連番を振る最低レベルの見出しレベル
			if(mySectionBaseLevel < 0){
				mySectionBaseLevel = headingLevel;
			}

			// 使用するカウンタのインデクスを取得
			int countIndex = headingLevel - mySectionBaseLevel;

			// 下位の見出しカウンタをリセット
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

			// 見出しにセット
			string headingNumber = result.ToString();
			string fragmentId = HeadingIdPrefix + headingNumber;
			headingElement.SetAttribute("id", fragmentId);

			// Toc に追加
			CheckMyToc();

			// 子が来たら UL を足しておく
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

			// 長さを足す
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

