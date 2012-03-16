using System;
using System.Collections.Generic;
using System.Xml;

namespace Bakera.Hatomaru{

/// <summary>
/// Action のベースクラスです。
/// </summary>
	public abstract class HatomaruGetAction : HatomaruActionBase{

		protected HatomaruGetAction(HatomaruXml model, AbsPath path) : base(model, path){}

		// Response を取得します。
		// XML を返す場合、このメソッド自体をオーバーライドします。
		// HTML を返す場合、このメソッド自体ではなく、ここから呼ばれる個々のプロテクトメソッドをオーバーライドしてください。
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



