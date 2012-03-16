using System;
using System.Collections.Specialized;
using System.IO;
using System.Xml;

namespace Bakera.Hatomaru{

	/// <summary>
	/// XML DOM を利用して出力用の XHTML を簡単に作るためのクラスです。
	/// 外部実体は読みに行きません。
	/// </summary>
	public partial class Xhtml : XmlDocument{

		public XmlDocumentFragment Null{
			get{return CreateDocumentFragment();}
		}

// パブリックメソッド

		/// <summary>
		/// エントリポイントに XmlNode を追加します。
		/// </summary>
		public void Append(XmlNode node){
			Entry.AppendChild(node);
		}


		/// <summary>
		/// 要素名を指定して XmlElement を作成します。
		/// </summary>
		public XmlElement Create(string name){
			return Create(name, null);
		}
		/// <summary>
		/// 要素名とクラス名を指定して XmlElement を作成します。
		/// </summary>
		public XmlElement Create(string name, string className){
			XmlElement result = base.CreateElement(name, NameSpace);
			if(!string.IsNullOrEmpty(className)) result.SetAttribute("class", className);
			return result;
		}
		/// <summary>
		/// 要素名、クラス名、内容の Object を指定して XmlElement を作成します。
		/// </summary>
		public XmlElement Create(string name, string className, params Object[] innerObj){
			if(string.IsNullOrEmpty(name)){
				throw new ArgumentException("XmlElement を作成しようとしましたが、要素名が空です。");
			}
			XmlElement result = base.CreateElement(name, NameSpace);
			if(!string.IsNullOrEmpty(className)) result.SetAttribute("class", className);
			if(innerObj == null) return result;
			foreach(Object o in innerObj){
				if(o == null) continue;
				if(o is XmlNode){
					 result.AppendChild(o as XmlNode);
				} else {
					result.AppendChild(Text(o));
				}
			}
			return result;
		}

		/// <summary>
		/// 要素名を指定して XmlElement を作成します。
		/// </summary>
		public void SetReplaceUrl(string source, string dest){
			if(myReplaceUrl == null) myReplaceUrl = new NameValueCollection();
			myReplaceUrl.Add(source, dest);
		}


// 個別要素作成メソッド : Class 名指定可能

		/// <summary>
		/// Hn要素を作成します。
		/// </summary>
		public XmlElement H(int level){
			return H(level, null, null);
		}
		public XmlElement H(int level, string className){
			return H(level, className, null);
		}
		public XmlElement H(int level, string className, params Object[] innerObj){
			string lStr = level.ToString(System.Globalization.CultureInfo.InvariantCulture);
			if(level < 1) throw new Exception("Hn の見出しレベルとして " + lStr +" が指定されました。");
			if(level > 6){
				if(string.IsNullOrEmpty(className)){
					className = "level" + lStr;
				} else {
					className += " level" + lStr;
				}
				lStr = "6";
			}
			return Create("h" + lStr, className, innerObj);
		}

		/// <summary>
		/// P要素を作成します。
		/// </summary>
		public XmlElement P(){
			return Create("p", null);
		}
		public XmlElement P(string className){
			return Create("p", className);
		}
		public XmlElement P(string className, params Object[] innerObj){
			return Create("p", className, innerObj);
		}

		/// <summary>
		/// Div要素を作成します。
		/// </summary>
		public XmlElement Div(){
			return Create("div", null);
		}
		public XmlElement Div(string className){
			return Create("div", className);
		}
		public XmlElement Div(string className, params Object[] innerObj){
			return Create("div", className, innerObj);
		}

		/// <summary>
		/// Span要素を作成します。
		/// </summary>
		public XmlElement Span(){
			return Create("span", null);
		}
		public XmlElement Span(string className){
			return Create("span", className);
		}
		public XmlElement Span(string className, params Object[] innerObj){
			return Create("span", className, innerObj);
		}



// カスタム要素作成メソッド

		/// <summary>
		/// href の文字列を指定して、a要素を作成します。
		/// 絶対 Uri は相対 Uri に変換されます。urn: などはそのまま出力されます。
		/// </summary>
		public XmlElement A(Uri uri){
			return A(uri, null, null);
		}
		public XmlElement A(Uri uri, string className){
			return A(uri, className, null);
		}
		public XmlElement A(Uri uri, string className, params Object[] innerObj){
			string href = GetHref(uri);
			if(href == "") return Create("em", "myself", innerObj);
			XmlElement result = Create("a", className, innerObj);
			if(href != null) result.SetAttribute("href", href);
			return result;
		}

		/// <summary>
		/// src の Uri と alt の文字列を指定して、img要素を作成します。
		/// </summary>
		public XmlElement Img(Uri uri, string alt){
			Uri absUri = null;
			if(myBaseUri != null){
				absUri = new Uri(myBaseUri, uri);
			} else {
				absUri = uri;
			}
			Uri srcUri = MakeRelative(absUri);
			return Img(srcUri.ToString(), alt);
		}
		/// <summary>
		/// src の string と alt の文字列を指定して、img要素を作成します。
		/// </summary>
		public XmlElement Img(string uri, string alt){
			XmlElement result = Create("img");
			result.SetAttribute("src", uri);
			result.SetAttribute("alt", alt);
			return result;
		}

		/// <summary>
		/// src の Uri と alt の文字列とサイズを指定して、img要素を作成します。
		/// </summary>
		public XmlElement Img(Uri uri, string alt, System.Drawing.Size size){
			return Img(uri, alt, size.Width, size.Height);
		}

		/// <summary>
		/// src の Uri と alt の文字列と幅、高さを指定して、img要素を作成します。
		/// </summary>
		public XmlElement Img(Uri uri, string alt, int width, int height){
			Uri absUri = null;
			if(myBaseUri != null){
				absUri = new Uri(myBaseUri, uri);
			} else {
				absUri = uri;
			}
			Uri srcUri = MakeRelative(absUri);
			return Img(srcUri.ToString(), alt, width, height);
		}

		/// <summary>
		/// src の string と alt の文字列と幅、高さを指定して、img要素を作成します。
		/// </summary>
		public XmlElement Img(string uri, string alt, int width, int height){
			XmlElement result = Img(uri, alt);
			result.SetAttribute("width", width.ToString());
			result.SetAttribute("height", height.ToString());
			return result;
		}


		/// <summary>
		/// AbsPath と番号を指定して、ページナビゲーション用の a要素を作成します。
		/// </summary>
		public XmlElement GetPageLink(AbsPath uriPrefix, int pageNum){
			XmlElement pageNavA = A(uriPrefix.Combine(pageNum.ToString()));
			pageNavA.InnerText = pageNum.ToString();
			return pageNavA;
		}


		/// <summary>
		/// ラベルを指定して、submitボタンを作成します。
		/// </summary>
		public XmlElement Submit(string label){
			XmlElement result = this.Create("input");
			result.SetAttribute("type", "submit");
			if(label != null) result.SetAttribute("value", label);
			return result;
		}

		/// <summary>
		/// ラベルを指定して、Checkboxを作成します。
		/// </summary>
		public XmlNode Checkbox(string name, string value, string labelStr){
			XmlNode result = CreateDocumentFragment();
			
			XmlElement input = Create("input", "checkbox");
			input.SetAttribute("type", "checkbox");
			input.SetAttribute("name", name);
			input.SetAttribute("id", name);
			input.SetAttribute("value", value);

			result.AppendChild(input);

			XmlElement label = Create("label", null, labelStr);
			label.SetAttribute("for", name);
			result.AppendChild(label);

			return result;
		}


		/// <summary>
		/// 名前と値を指定して input 要素を作成します。
		/// </summary>
		public XmlNode Input(string name, string value){
			return Input(name, value, null);
		}
		public XmlNode Input(string name, string value, string labelStr){
			if(value == null) value = "";

			XmlElement input = Create("input", "text");
			input.SetAttribute("name", name);
			input.SetAttribute("id", name);
			input.SetAttribute("value", value);
			input.SetAttribute("size", "50");
			if(string.IsNullOrEmpty(labelStr)) return input;

			XmlNode result = CreateDocumentFragment();

			XmlElement label = Create("label", null, labelStr);
			label.SetAttribute("for", name);
			result.AppendChild(label);
			result.AppendChild(CreateTextNode(" : "));
			result.AppendChild(input);

			return result;
		}


		/// <summary>
		/// 名前と値を指定して textarea 要素を作成します。
		/// </summary>
		public XmlNode TextArea(string name, string value, string labelStr){

			XmlElement input = Create("textarea");
			input.SetAttribute("name", name);
			input.SetAttribute("id", name);
			input.InnerText = value;
			input.SetAttribute("rows", "10");
			input.SetAttribute("cols", "60");
			if(string.IsNullOrEmpty(labelStr)) return input;

			XmlNode result = CreateDocumentFragment();
			XmlElement label = Create("label", null, labelStr);
			label.SetAttribute("for", name);
			result.AppendChild(label);
			result.AppendChild(CreateTextNode(" : "));
			result.AppendChild(input);

			return result;
		}


		/// <summary>
		/// 名前と値を指定して input type="hidden" を作成します。
		/// </summary>
		public XmlNode Hidden(string name, string value){

			XmlElement input = Create("input");
			input.SetAttribute("type", "hidden");
			input.SetAttribute("name", name);
			input.SetAttribute("value", value);
			return input;
		}

		/// <summary>
		/// 自身に submit する form要素を作成します。
		/// </summary>
		public XmlElement Form(){return Form(null);}
		/// <summary>
		/// action を指定して、form要素を作成します。
		/// </summary>
		public XmlElement Form(Uri action){
			return Form(action, null);
		}
		/// <summary>
		/// action と method を指定して、form要素を作成します。
		/// </summary>
		public XmlElement Form(Uri action, string method){
			XmlElement result = this.Create("form");
			action = MakeRelative(action);
			string actionStr;
			if(action == null){
				actionStr = "";
			} else {
				actionStr = action.ToString();
			}
			result.SetAttribute("action", actionStr);
			if(method != null) result.SetAttribute("method", method);
			return result;
		}

		/// <summary>
		/// legendを指定して、fieldset要素を作成します。
		/// </summary>
		public XmlElement Fieldset(string legendLabel){
			XmlElement result = this.Create("fieldset");
			XmlElement legend = this.Create("legend", null, legendLabel);
			result.AppendChild(legend);
			return result;
		}


// table
		/// <summary>
		/// クラス名、見出しセルの数、各セルのデータを指定して、データを含んだtr要素を作成します。
		/// </summary>
		public XmlElement Tr(string className, int headCount, params object[] dataObj){
			XmlElement result = this.Create("tr", className);
			for(int i=0; i < dataObj.Length; i++){
				XmlElement td = null;
				if(i < headCount){
					td = Create("th", null, dataObj[i]);
					td.SetAttribute("scope", "row");
				} else {
					td = Create("td", null, dataObj[i]);
				}
				result.AppendChild(td);
			}
			return result;
		}

		/// <summary>
		/// クラス名、各セルのデータを指定して、見出し行のtr要素を作成します。
		/// </summary>
		public XmlElement HeadTr(string className, params object[] dataObj){
			XmlElement result = this.Create("tr", className);
			for(int i=0; i < dataObj.Length; i++){
				XmlElement th = Create("th", null, dataObj[i]);
				th.SetAttribute("scope", "col");
				result.AppendChild(th);
			}
			return result;
		}


// カスタムノード作成メソッド

		/// <summary>
		/// 半角スペースを含む TextNode を作成します。
		/// </summary>
		public XmlText Space{
			get{return this.CreateTextNode(" ");}
		}

		/// <summary>
		/// 指定されたオブジェクトを含む TextNode を作成します。
		/// </summary>
		public XmlText Text(Object o){
			return this.CreateTextNode(o.ToString());
		}

		/// <summary>
		/// 指定されたテキストを含むコメントノードを作成します。
		/// </summary>
		public XmlComment Comment(string s){
			return this.CreateComment(s);
		}
		/// <summary>
		/// 指定されたテキストを含むコメントノードを作成します。
		/// </summary>
		public XmlComment Comment(string format, params Object[] datas){
			string s = string.Format(format, datas);
			return this.CreateComment(s);
		}


// メタ情報追加メソッド

		/// <summary>
		/// rel属性の値と type属性の値、LinkItem を指定して、<link rel="" href=""> を追加します。
		/// </summary>
		public void AddLinkRel(string relValue, string typeValue, LinkItem item){
			XmlElement link = this.Create("link");
			link.SetAttribute("rel", relValue);
			link.SetAttribute("type", typeValue);
			link.SetAttribute("href", GetHref(item.Path));
			if(!string.IsNullOrEmpty(item.InnerText)) link.SetAttribute("title", item.InnerText);
			this.Head.AppendChild(link);
		}

		/// <summary>
		/// rel属性の値と type属性の値、AbsPath を指定して、<link rel="" href=""> を追加します。
		/// </summary>
		public void AddLinkRel(string relValue, string typeValue, AbsPath path){
			XmlElement link = this.Create("link");
			link.SetAttribute("rel", relValue);
			link.SetAttribute("type", typeValue);
			link.SetAttribute("href", GetHref(path));
			this.Head.AppendChild(link);
		}


		/// <summary>
		/// LinkItem を指定して、<link rel=stylesheet> を追加します。
		/// </summary>
		public void AddStyleLink(LinkItem link){
			AddLinkRel("stylesheet", "text/css", link);
		}

// 要素変換

		/// <summary>
		/// href 属性を持つ任意の XmlElement を a要素に変換します。
		/// </summary>
		public XmlElement GetA(XmlNode node){
			return GetA(LinkItem.GetItem(node));
		}
		/// <summary>
		/// LinkItem を a要素に変換します。
		/// </summary>
		public XmlElement GetA(LinkItem item){
			if(item == null) return null;
			XmlElement result = A(item.Path, null, item.InnerText);
			return result;
		}



	} // End class OutXhtml
}
