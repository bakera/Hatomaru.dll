using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Xml;

namespace Bakera.Hatomaru{

/// <summary>
/// HatomaruDoc を制御するクラスです。
/// </summary>
	public partial class MenuAction : HatomaruGetAction{

// コンストラクタ
		public MenuAction(HatomaruMenu model, AbsPath path) : base(model, path){
			myPath = myModel.BasePath;
		}


// プロパティ

		/// <summary>
		/// データを GET し、HatomaruResponse を取得します。
		/// </summary>
		protected override HatomaruResponse GetHtmlResponse(){
			XmlNodeList menus = myModel.Document.GetElementsByTagName(HatomaruMenu.MenuItem);
			XmlElement ul = Html.Create("ul", "menu");
			foreach(XmlElement e in menus){
				XmlElement li = Html.Create("li");
				li.AppendChild(ParseNode(e, 3));
				ul.AppendChild(li);
			}
			Html.Append(ul);
			return myResponse;
		}


// オーバーライド

		/// <summary>
		/// タイトルをHtml に反映します。
		/// </summary>
		protected override void SetTitle(HatomaruResponse hr){
			if(hr.SelfTitle == null){
				hr.SelfTitle = Model.BaseTitle;
				if(Model.ParentXml != null) hr.BaseTitle = Model.ParentXml.BaseTitle;
			} else if(hr.BaseTitle == null){
				hr.BaseTitle = Model.BaseTitle;
			}
			hr.Html.Title.InnerText = hr.Title;
			hr.Html.H1.InnerText = Model.BaseTitle;
		}

		/// <summary>
		/// menuでは子ナビは使用しません。
		/// </summary>
		protected override LinkItem[] GetSubNav(){
			return new LinkItem[0];
		}



	} // End class BbsController
} // End Namespace Bakera



