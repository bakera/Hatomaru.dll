using System;
using System.Collections.Specialized;
using System.IO;
using System.Xml;
using System.Xml.Resolvers;

namespace Bakera.Hatomaru{

	/// <summary>
	/// XML DOM �𗘗p���ďo�͗p�� XHTML ���ȒP�ɍ�邽�߂̃N���X�ł��B
	/// �O�����͓̂ǂ݂ɍs���܂���B
	/// </summary>
	public partial class Xhtml : XmlDocument{
		public const string NameSpace = "http://www.w3.org/1999/xhtml";
		public const string RootElement = "html";
		public const string PublicIdentifier = "-//W3C//DTD XHTML 1.1//EN";
		public const string SystemIdentifier = "http://www.w3.org/TR/xhtml11/DTD/xhtml11.dtd";

		public const string UrnIetfRfcPrefix = "urn:ietf:rfc:";
		public const string UrnIetfRfcFormat = "http://tools.ietf.org/html/rfc{0}";

		private Uri myBaseUri;
		private XmlElement myEntry;
		private NameValueCollection myReplaceUrl; // URL ��u������ݒ��ۑ�

// �R���X�g���N�^

		/// <summary>
		/// XHTML �h�L�������g�̃C���X�^���X���쐬���܂��B
		/// �O�����͓̂ǂ݂ɍs���܂���B
		/// </summary>
		public Xhtml() : base(){
			PreserveWhitespace = true;
			XmlResolver = null;
			AppendChild(CreateXmlDeclaration("1.0", "UTF-8", null));
			AppendChild(CreateDocumentType(RootElement, PublicIdentifier, SystemIdentifier, null));
		}

		/// <summary>
		/// �t�@�C�����w�肵�āAXHTML �h�L�������g�̃C���X�^���X���쐬���܂��B
		/// </summary>
		public Xhtml(string filename) : this(){
			LoadFile(filename);
		}


// �v���p�e�B

		/// <summary>
		/// XHTML �h�L�������g�̊�ƂȂ� URL ��ݒ�E�擾���܂��B
		/// </summary>
		public Uri BaseUri{
			get {return myBaseUri;}
			set {myBaseUri = value;}
		}

		/// <summary>
		/// �G���g���[�|�C���g��ݒ�E�擾���܂��B
		/// </summary>
		public XmlElement Entry{
			get {return myEntry;}
			set {myEntry = value;}
		}
// �e�v�f�ɃA�N�Z�X����v���p�e�B

		/// <summary>
		/// XHTML �h�L�������g�� html �v�f�ɃA�N�Z�X���܂��B
		/// </summary>
		public XmlElement Html{
			get {
				XmlElement result = DocumentElement;
				if(result == null){
					result = Create("html");
					this.AppendChild(result);
				}
				return result;
			}
		}

		/// <summary>
		/// XHTML �h�L�������g�� head �v�f��\�� XmlElement �ɃA�N�Z�X���܂��B
		/// </summary>
		public XmlElement Head{
			get {
				XmlElement result = Html["head"];
				if(result == null){
					result = Create("head");
					Html.PrependChild(result);
				}
				return result;
			}
		}

		/// <summary>
		/// XHTML �h�L�������g�� body �v�f��\�� XmlElement �ɃA�N�Z�X���܂��B
		/// </summary>
		public XmlElement Body{
			get {
				XmlElement result = Html["body"];
				if(result == null){
					result = Create("body");
					Html.AppendChild(result);
				}
				return result;
			}
		}

		/// <summary>
		/// XHTML �h�L�������g�� title �v�f��\�� XmlElement �ɃA�N�Z�X���܂��B
		/// </summary>
		public XmlElement Title{
			get {
				XmlElement result = this.Head["title"];
				if(result == null){
					result = Create("title");
					Head.AppendChild(result);
				}
				return result;
			}
		}

		/// <summary>
		/// XHTML �h�L�������g�̍ŏ��� h1 �v�f��\�� XmlElement �ɃA�N�Z�X���܂��B
		/// h1 �v�f�������ꍇ�� null ��Ԃ��܂��B
		/// </summary>
		public XmlElement H1{
			get {
				XmlNodeList nodes = this.Body.GetElementsByTagName("h1");
				if(nodes.Count == 0) return null;
				return nodes[0] as XmlElement;
			}
		}

// �p�u���b�N���\�b�h


		/// <summary>
		/// ���`�� Xhtml ���w�肵�āAXhtml �̐V�����C���X�^���X���쐬���܂��B
		/// </summary>
		public static Xhtml Copy(Xhtml html){
			if(html == null) throw new ArgumentException("���ƂȂ� XHTML �� null �ł��B");
			Xhtml result = new Xhtml();
			// �����^�錾�Ȃǂ��܂߂ăR�s�[
			foreach(XmlNode x in html.ChildNodes){
				result.AppendChild(result.ImportNode(x, true));
			}
			return result;
		}


		/// <summary>
		/// �t�@�C�������w�肵�� XML �f�[�^��ǂݎ��܂��B
		/// Load �ƈقȂ�A�\�[�X�t�@�C���͓ǂݎ��֎~�ɂȂ�܂��� (�㏑���֎~�ɂȂ邾���ł�)�B
		/// </summary>
		public void LoadFile(string filename){
			using(FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.Read)){
				this.Load(fs);
			}
		}

		/// <summary>
		/// FileInfo���w�肵�� XML �f�[�^��ۑ����܂��B
		/// </summary>
		public void SaveFile(FileInfo f){
			f.Directory.Create();
			using(FileStream fs = f.Open(FileMode.Create, FileAccess.Write, FileShare.None)){
				this.Save(fs);
			}
		}


		/// <summary>
		/// Uri �� href �̒l�ɕϊ����܂��B
		/// ���g�ւ̃����N�ƂȂ�ꍇ�͋󕶎����Ԃ��܂��B
		/// </summary>
		public string GetHref(Uri uri){
			if(myReplaceUrl != null){
				foreach(string key in myReplaceUrl.AllKeys){
					if(uri.OriginalString.EndsWith(key)){
						string newUrlString = uri.OriginalString.Replace(key, myReplaceUrl[key]);
						uri = new Uri(newUrlString, UriKind.RelativeOrAbsolute);
						break;
					}
				}
			}
			// URN 
			if(uri.ToString().StartsWith(UrnIetfRfcPrefix)){
				string rfcNumberString = uri.ToString().Substring(UrnIetfRfcPrefix.Length);
				if(rfcNumberString.Length == 0){
					return String.Format(UrnIetfRfcFormat, "-index");
				}
				return String.Format(UrnIetfRfcFormat, rfcNumberString);
			}
			Uri absUri = null;
			if(myBaseUri != null){
				absUri = new Uri(myBaseUri, uri);
				if(absUri == myBaseUri) return "";
			} else {
				absUri = uri;
			}
			if(absUri == null) return null;

			Uri hrefUri = MakeRelative(absUri);
			return hrefUri.OriginalString;
		}


		/// <summary>
		/// �ݒ肳�ꂽ�x�[�X Uri �����ɑ��� Uri �𐶐����܂��B
		/// </summary>
		public Uri MakeRelative(Uri uri){
			if(myBaseUri == null) return uri;
			if(myBaseUri == uri) return new Uri("", UriKind.Relative);

			if(uri == null) return new Uri("", UriKind.Relative);
			if(!uri.IsAbsoluteUri) return uri;

			Uri result = myBaseUri.MakeRelativeUri(uri);
			if(string.IsNullOrEmpty(result.ToString())) return new Uri("./", UriKind.Relative);
			return result;
		}


		/// <summary>
		/// ���閼�O�̗v�f�̒��ōŏ��̂��̂�Ԃ��܂��B
		/// </summary>
		public XmlElement GetElementByTagNameFirst(string name){
			XmlNodeList list = GetElementsByTagName(name);
			if(list.Count == 0) return null;
			return list[0] as XmlElement;
		}


		/// <summary>
		/// ���閼�O�̗v�f��n���ꂽ�v�f�Œu�����܂��B
		/// </summary>
		public void Replace(string name, XmlNode rep){
			if(rep == null) rep = CreateDocumentFragment();
			XmlElement target = GetElementByTagNameFirst(name);
			if(target == null) return;
			target.ParentNode.ReplaceChild(rep, target);
		}

	} // End class OutXhtml
}
