using System;
using System.Collections.Generic;
using System.Xml;

namespace Bakera.Hatomaru{

/// <summary>
/// Action �̃x�[�X�N���X�ł��B
/// </summary>
	public abstract class HatomaruGetAction : HatomaruActionBase{

		protected HatomaruGetAction(HatomaruXml model, AbsPath path) : base(model, path){}

		// Response ���擾���܂��B
		// XML ��Ԃ��ꍇ�A���̃��\�b�h���̂��I�[�o�[���C�h���܂��B
		// HTML ��Ԃ��ꍇ�A���̃��\�b�h���̂ł͂Ȃ��A��������Ă΂��X�̃v���e�N�g���\�b�h���I�[�o�[���C�h���Ă��������B
		public virtual HatomaruResponse Get(){
			if(CheckRedirect()){
				return new RedirectResponse(Path, Model.Manager.IniData.Domain);
			}
			HatomaruResponse hr = GetHtmlResponse();
			if(hr is XmlResponse) return hr;
			if(hr is RedirectResponse) return hr;
			SetTopicPath(hr);
			SetNavigation(hr);
			SetTitle(hr);
			string keywords = GetKeywords();
			SetKeywords(hr, keywords);
			return hr;
		}

		protected abstract HatomaruResponse GetHtmlResponse();



	}
} // End Namespace Bakera



