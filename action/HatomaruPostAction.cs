using System;
using System.Collections.Generic;
using System.Web;

namespace Bakera.Hatomaru{

/// <summary>
/// Post �f�[�^���������� HatomaruPostAction �N���X�ł��B
/// </summary>
	public abstract class HatomaruPostAction : HatomaruActionBase{

		private readonly HttpRequest myRequest;

// �R���X�g���N�^
		public HatomaruPostAction(HatomaruXml model, AbsPath path, HttpRequest req) : base(model, path){
			myRequest = req;
		}

// �v���p�e�B

		protected HttpRequest Request{
			get{return myRequest;}
		}



// ���\�b�h
		public virtual HatomaruResponse Post(){
			if(!IsMultipartFormData(Request)) return new UnsupportedMediaTypeResponse(Model, Path);
			if(GetPostedValue(InputCharsetName) != InputCharsetValue) return new ForbiddenResponse(Model, Path, "UTF-8 �ȊO�̕��������������ɂ�铊�e�͎󂯕t�����܂���B");

			HatomaruResponse hr = PostAndGetHtmlResponse();
			if(hr is XmlResponse) return hr;
			SetTopicPath(hr);
			SetNavigation(hr);
			SetTitle(hr);
			string keywords = GetKeywords();
			SetKeywords(hr, keywords);
			return hr;
		}

		protected string GetPostedValue(string key){
			if(Request.Form[key] == null) return null;
			return Request.Form[key].Trim();
		}

		protected abstract HatomaruResponse PostAndGetHtmlResponse();


	} // End class BbsController
} // End Namespace Bakera



