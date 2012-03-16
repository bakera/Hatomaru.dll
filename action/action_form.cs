using System;
using System.IO;
using System.Text;
using System.Web;
using System.Xml;
using System.Collections.Generic;

namespace Bakera.Hatomaru{

	/// <summary>
	/// �f�[�^���p�[�X���� Xhtml �� NormalResponse �𐶐�����N���X�ł��B
	/// �p�[�X���Ȃ���A�g�p�����f�[�^�\�[�X�� NormalResponse �ɒǉ����čs���܂��B
	/// </summary>
	public abstract partial class HatomaruActionBase{

		public const string FormCharset = "UTF-8";
		public const string MultipartContentType = "multipart/form-data";

		public const string InputCharsetName = "charset";
		public const string InputCharsetValue = "�t�s�e�|�W";

		public const string InputSubjectName = "subject";
		public const string InputSubjectLabel = "�薼";

		public const string InputSenderName = "sender";
		public const string InputSenderLabel = "���O";

		public const string InputBodyName = "body";
		public const string InputBodyLabel = "�{��";

		public const string InputSubmitName = "submit";
		public const string InputEditName = "edit";
		public const string InputHashName = "hash";


		public const string FormNotice = "���L�����`�̏������݂͂��������������B";

		protected XmlNode GetForm(){
			return GetForm(new Article());
		}

		protected XmlNode GetForm(string title){
			Article commentArticle = new Article();
			commentArticle.Subject = title;
			return GetForm(commentArticle);
		}

		protected XmlNode GetForm(Article a){
			return GetForm(a, null);
		}

		// ���e�t�H�[�����o�͂��܂��B
		// �����̓R�����g��ƂȂ�e�� Article �ł��B
		protected XmlNode GetForm(Article a, PostErrorCollection errors){
			string formTitle = "";
			if(a.Parent == 0 && a.CommentTo == null){
				formTitle = "�V�K���e�t�H�[��";
			} else {
				formTitle =  "�R�����g�t�H�[��";
			}
			XmlElement result = Html.Form(null, "post");
			result.SetAttribute("enctype", MultipartContentType);
			result.SetAttribute("accept-charset", FormCharset);

			XmlElement fs = Html.Fieldset(formTitle);
			fs.AppendChild(Html.P("note", FormNotice));
			fs.AppendChild(GetTextField(InputSubjectName, a.Subject, InputSubjectLabel, errors));
			fs.AppendChild(GetTextField(InputSenderName, a.Name, InputSenderLabel, errors));
			fs.AppendChild(GetTextArea(InputBodyName, a.Message, InputBodyLabel, errors));
			fs.AppendChild(Html.P("submit", Html.Submit("���e���e�m�F"), Html.Hidden(InputCharsetName, InputCharsetValue)));

			result.AppendChild(fs);
			return result;
		}

		// �m�F��ʂ̃t�H�[�����o�͂��܂��B
		protected XmlNode GetConfirmForm(Article a){
			XmlElement result = Html.Form(null, "post");
			result.SetAttribute("enctype", MultipartContentType);
			result.SetAttribute("accept-charset", FormCharset);

			XmlElement p = Html.P("submit");
			p.AppendChild(Html.Hidden(InputSubjectName, a.Subject));
			p.AppendChild(Html.Hidden(InputSenderName, a.Name));
			p.AppendChild(Html.Hidden(InputBodyName, a.Message));
			p.AppendChild(Html.Hidden(InputCharsetName, InputCharsetValue));
			string key = Bbs.GetPostKey();
			p.AppendChild(Html.Hidden(InputHashName, key));

			XmlElement submit = Html.Submit("���e����");
			submit.SetAttribute("name", InputSubmitName);
			p.AppendChild(submit);
			XmlElement reedit = Html.Submit("���e��ҏW����");
			reedit.SetAttribute("name", InputEditName);
			p.AppendChild(reedit);

			result.AppendChild(p);

			return result;
		}


		private XmlNode GetTextField(string name, string val, string label, PostErrorCollection errors){
			XmlElement result = Html.P();
			if(errors != null && errors.IsError(name)) result.SetAttribute("class", "error");
			result.AppendChild(Html.Input(name, val, label));
			return result;
		}

		private XmlNode GetTextArea(string name, string val, string label, PostErrorCollection errors){
			XmlElement result = Html.P();
			if(errors != null && errors.IsError(name)) result.SetAttribute("class", "error");
			result.AppendChild(Html.TextArea(name, val, label));
			return result;
		}



		// ���N�G�X�g�� multipart/form-data �Ȃ� true ��Ԃ��܂��B
		public static bool IsMultipartFormData(HttpRequest req){
			string ct = req.ContentType.ToLower();
			return ct.StartsWith(MultipartContentType);
		}


	}

} // end namespace Bakea

