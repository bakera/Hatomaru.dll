using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace Bakera.Hatomaru{

/// <summary>
/// データ形式の一覧を表示するアクションです。
/// </summary>
	public partial class AmazonViewSearchForm : AmazonAction{

		public new const string Label = "アマ検";
		public const string FormElementName = "form";

// コンストラクタ

		/// <summary>
		/// アマ検のフォーム表示のためのアクションのインスタンスを開始します。
		/// </summary>
		public AmazonViewSearchForm(AmazonSearch model, AbsPath path) : base(model, path){
			myPath = myModel.BasePath;
		}

// プロパティ

		/// <summary>
		/// データを GET し、HatomaruResponse を取得します。
		/// </summary>
		protected override HatomaruResponse GetHtmlResponse(){

			InsertHeading(2, Label);
			XmlElement desc = Model.Document.DocumentElement[AmazonSearch.TopicName];
			if(desc != null) Html.Append(ParseNode(desc.ChildNodes, 3));
			Html.Append(GetSearchForm(null, AmazonIndexType.None));

			return Response;
		}



	} // End class
} // End Namespace Bakera



