using System;
using System.Collections.Specialized;
using System.IO;
using System.Xml;

namespace Bakera.Hatomaru{

	/// <summary>
	/// XML DOM �𗘗p���ďo�͗p�� XHTML ���ȒP�ɍ�邽�߂̃N���X�ł��B
	/// �O�����͓̂ǂ݂ɍs���܂���B
	/// </summary>
	public partial class Xhtml : XmlDocument{

		public XmlDocumentFragment Null{
			get{return CreateDocumentFragment();}
		}

// �p�u���b�N���\�b�h

		/// <summary>
		/// �G���g���|�C���g�� XmlNode ��ǉ����܂��B
		/// </summary>
		public void Append(XmlNode node){
			Entry.AppendChild(node);
		}


		/// <summary>
		/// �v�f�����w�肵�� XmlElement ���쐬���܂��B
		/// </summary>
		public XmlElement Create(string name){
			return Create(name, null);
		}
		/// <summary>
		/// �v�f���ƃN���X�����w�肵�� XmlElement ���쐬���܂��B
		/// </summary>
		public XmlElement Create(string name, string className){
			XmlElement result = base.CreateElement(name, NameSpace);
			if(!string.IsNullOrEmpty(className)) result.SetAttribute("class", className);
			return result;
		}
		/// <summary>
		/// �v�f���A�N���X���A���e�� Object ���w�肵�� XmlElement ���쐬���܂��B
		/// </summary>
		public XmlElement Create(string name, string className, params Object[] innerObj){
			if(string.IsNullOrEmpty(name)){
				throw new ArgumentException("XmlElement ���쐬���悤�Ƃ��܂������A�v�f������ł��B");
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
		/// �v�f�����w�肵�� XmlElement ���쐬���܂��B
		/// </summary>
		public void SetReplaceUrl(string source, string dest){
			if(myReplaceUrl == null) myReplaceUrl = new NameValueCollection();
			myReplaceUrl.Add(source, dest);
		}


// �ʗv�f�쐬���\�b�h : Class ���w��\

		/// <summary>
		/// Hn�v�f���쐬���܂��B
		/// </summary>
		public XmlElement H(int level){
			return H(level, null, null);
		}
		public XmlElement H(int level, string className){
			return H(level, className, null);
		}
		public XmlElement H(int level, string className, params Object[] innerObj){
			string lStr = level.ToString(System.Globalization.CultureInfo.InvariantCulture);
			if(level < 1) throw new Exception("Hn �̌��o�����x���Ƃ��� " + lStr +" ���w�肳��܂����B");
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
		/// P�v�f���쐬���܂��B
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
		/// Div�v�f���쐬���܂��B
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
		/// Span�v�f���쐬���܂��B
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



// �J�X�^���v�f�쐬���\�b�h

		/// <summary>
		/// href �̕�������w�肵�āAa�v�f���쐬���܂��B
		/// ��� Uri �͑��� Uri �ɕϊ�����܂��Burn: �Ȃǂ͂��̂܂܏o�͂���܂��B
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
		/// src �� Uri �� alt �̕�������w�肵�āAimg�v�f���쐬���܂��B
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
		/// src �� string �� alt �̕�������w�肵�āAimg�v�f���쐬���܂��B
		/// </summary>
		public XmlElement Img(string uri, string alt){
			XmlElement result = Create("img");
			result.SetAttribute("src", uri);
			result.SetAttribute("alt", alt);
			return result;
		}

		/// <summary>
		/// src �� Uri �� alt �̕�����ƃT�C�Y���w�肵�āAimg�v�f���쐬���܂��B
		/// </summary>
		public XmlElement Img(Uri uri, string alt, System.Drawing.Size size){
			return Img(uri, alt, size.Width, size.Height);
		}

		/// <summary>
		/// src �� Uri �� alt �̕�����ƕ��A�������w�肵�āAimg�v�f���쐬���܂��B
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
		/// src �� string �� alt �̕�����ƕ��A�������w�肵�āAimg�v�f���쐬���܂��B
		/// </summary>
		public XmlElement Img(string uri, string alt, int width, int height){
			XmlElement result = Img(uri, alt);
			result.SetAttribute("width", width.ToString());
			result.SetAttribute("height", height.ToString());
			return result;
		}


		/// <summary>
		/// AbsPath �Ɣԍ����w�肵�āA�y�[�W�i�r�Q�[�V�����p�� a�v�f���쐬���܂��B
		/// </summary>
		public XmlElement GetPageLink(AbsPath uriPrefix, int pageNum){
			XmlElement pageNavA = A(uriPrefix.Combine(pageNum.ToString()));
			pageNavA.InnerText = pageNum.ToString();
			return pageNavA;
		}


		/// <summary>
		/// ���x�����w�肵�āAsubmit�{�^�����쐬���܂��B
		/// </summary>
		public XmlElement Submit(string label){
			XmlElement result = this.Create("input");
			result.SetAttribute("type", "submit");
			if(label != null) result.SetAttribute("value", label);
			return result;
		}

		/// <summary>
		/// ���x�����w�肵�āACheckbox���쐬���܂��B
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
		/// ���O�ƒl���w�肵�� input �v�f���쐬���܂��B
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
		/// ���O�ƒl���w�肵�� textarea �v�f���쐬���܂��B
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
		/// ���O�ƒl���w�肵�� input type="hidden" ���쐬���܂��B
		/// </summary>
		public XmlNode Hidden(string name, string value){

			XmlElement input = Create("input");
			input.SetAttribute("type", "hidden");
			input.SetAttribute("name", name);
			input.SetAttribute("value", value);
			return input;
		}

		/// <summary>
		/// ���g�� submit ���� form�v�f���쐬���܂��B
		/// </summary>
		public XmlElement Form(){return Form(null);}
		/// <summary>
		/// action ���w�肵�āAform�v�f���쐬���܂��B
		/// </summary>
		public XmlElement Form(Uri action){
			return Form(action, null);
		}
		/// <summary>
		/// action �� method ���w�肵�āAform�v�f���쐬���܂��B
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
		/// legend���w�肵�āAfieldset�v�f���쐬���܂��B
		/// </summary>
		public XmlElement Fieldset(string legendLabel){
			XmlElement result = this.Create("fieldset");
			XmlElement legend = this.Create("legend", null, legendLabel);
			result.AppendChild(legend);
			return result;
		}


// table
		/// <summary>
		/// �N���X���A���o���Z���̐��A�e�Z���̃f�[�^���w�肵�āA�f�[�^���܂�tr�v�f���쐬���܂��B
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
		/// �N���X���A�e�Z���̃f�[�^���w�肵�āA���o���s��tr�v�f���쐬���܂��B
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


// �J�X�^���m�[�h�쐬���\�b�h

		/// <summary>
		/// ���p�X�y�[�X���܂� TextNode ���쐬���܂��B
		/// </summary>
		public XmlText Space{
			get{return this.CreateTextNode(" ");}
		}

		/// <summary>
		/// �w�肳�ꂽ�I�u�W�F�N�g���܂� TextNode ���쐬���܂��B
		/// </summary>
		public XmlText Text(Object o){
			return this.CreateTextNode(o.ToString());
		}

		/// <summary>
		/// �w�肳�ꂽ�e�L�X�g���܂ރR�����g�m�[�h���쐬���܂��B
		/// </summary>
		public XmlComment Comment(string s){
			return this.CreateComment(s);
		}
		/// <summary>
		/// �w�肳�ꂽ�e�L�X�g���܂ރR�����g�m�[�h���쐬���܂��B
		/// </summary>
		public XmlComment Comment(string format, params Object[] datas){
			string s = string.Format(format, datas);
			return this.CreateComment(s);
		}


// ���^���ǉ����\�b�h

		/// <summary>
		/// rel�����̒l�� type�����̒l�ALinkItem ���w�肵�āA<link rel="" href=""> ��ǉ����܂��B
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
		/// rel�����̒l�� type�����̒l�AAbsPath ���w�肵�āA<link rel="" href=""> ��ǉ����܂��B
		/// </summary>
		public void AddLinkRel(string relValue, string typeValue, AbsPath path){
			XmlElement link = this.Create("link");
			link.SetAttribute("rel", relValue);
			link.SetAttribute("type", typeValue);
			link.SetAttribute("href", GetHref(path));
			this.Head.AppendChild(link);
		}


		/// <summary>
		/// LinkItem ���w�肵�āA<link rel=stylesheet> ��ǉ����܂��B
		/// </summary>
		public void AddStyleLink(LinkItem link){
			AddLinkRel("stylesheet", "text/css", link);
		}

// �v�f�ϊ�

		/// <summary>
		/// href ���������C�ӂ� XmlElement �� a�v�f�ɕϊ����܂��B
		/// </summary>
		public XmlElement GetA(XmlNode node){
			return GetA(LinkItem.GetItem(node));
		}
		/// <summary>
		/// LinkItem �� a�v�f�ɕϊ����܂��B
		/// </summary>
		public XmlElement GetA(LinkItem item){
			if(item == null) return null;
			XmlElement result = A(item.Path, null, item.InnerText);
			return result;
		}



	} // End class OutXhtml
}
