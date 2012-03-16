using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace Bakera.Hatomaru{

/// <summary>
/// Amazon検索結果を表示するアクションです。
/// </summary>
	public partial class AmazonDoSearch : AmazonAction{

		public new const string Label = "アマ検";
		private string myQuery;
		private AmazonIndexType myIndexType;
		private int myPageNum;

// コンストラクタ

		/// <summary>
		/// アマ検の検索のためのアクションのインスタンスを開始します。
		/// </summary>
		public AmazonDoSearch(AmazonSearch model, AbsPath path, string query, AmazonIndexType ait, int page) : base(model, path){
			myPath = myModel.BasePath;
			myQuery = query;
			myIndexType = ait;
			myPageNum = page;
			if(myPageNum < 1) myPageNum = 1;
		}

// プロパティ

		/// <summary>
		/// データを GET し、HatomaruResponse を取得します。
		/// </summary>
		protected override HatomaruResponse GetHtmlResponse(){
			InsertHeading(2, Label);
			Html.Append(GetSearchForm(myQuery, myIndexType));

			// 検索する
			if(!string.IsNullOrEmpty(myQuery)){
				Html.Append(Search());
			}
			return Response;
		}


		// 検索して結果を得ます。
		private XmlNode Search(){
			XmlNode result = Html.CreateDocumentFragment();

			string title = string.Format("「{0}」の検索結果", myQuery);
			result.AppendChild(Html.H(3, null, title));
			Response.SelfTitle = title;

			AmazonItemList items = Model.Manager.AmazonManager.GetSearchItem(myIndexType, myQuery, myPageNum);
			if(items == null){
				XmlElement p3 = Html.P(null, "検索に問題が発生しているようです。しばらくお待ちください。");
				result.AppendChild(p3);
				return result;
			}
			if(items.Count == 0){
				XmlElement p3 = Html.P(null, "残念ながら、何も見つかりませんでした。");
				result.AppendChild(p3);
				return result;
			}
			XmlElement p = Html.P();
			p.InnerText = string.Format("{0} 件見つかりました。({1}ページ目/約{2}ページ)", items.TotalResults, myPageNum, items.TotalPages);
			result.AppendChild(p);

			XmlElement ul = Html.Create("ul");
			foreach (AmazonItem item in items){
				ul.AppendChild(GetItem(item));
			}
			result.AppendChild(ul);

			int pages = Convert.ToInt32(items.TotalPages);
			if(pages > 1){
				result.AppendChild(GetPageNav(pages));
			}
			return result;
		}

		private XmlElement GetItem(AmazonItem item){
			XmlElement li = Html.Create("li");
			XmlElement a = Html.Create("a");
			a.InnerText = item.Title;
			li.InnerText += " " + item.Asin;
			a.SetAttribute("href", item.DetailPageUrl);
			a.PrependChild(GetImages(item));
			li.PrependChild(a);
			return li;
		}

		private XmlNode GetImages(AmazonItem item){
			if(item.Image != null) return GetImage(item.Image);
//			if(item.SmallImage != null) return GetImage(item.SmallImage);
//			if(item.MediumImage != null) return GetImage(item.MediumImage);
//			if(item.LargeImage != null) return GetImage(item.LargeImage);
			return Html.Null;
		}

		private XmlElement GetImage(AmazonImage image){
			if(image == null) return null;
			XmlElement result = Html.Create("img");
			result.SetAttribute("alt","");
			result.SetAttribute("src", image.Url);
			result.SetAttribute("width", image.Width.ToString());
			result.SetAttribute("height", image.Height.ToString());
			return result;
		}

		private XmlNode GetPageNav(int maxPageNum){
			XmlElement result = Html.Create("ul", "page-nav");

			string baseQuery = string.Format("i={0};q={1}", myIndexType.ToString(), myQuery.UrlEncode());

			// 前のページ
			if(myPageNum > 1){
				string prevQuery = baseQuery + string.Format(";p={0}", myPageNum-1);
				AbsPath prevPath = myModel.BasePath.CombineQuery(prevQuery);
				XmlElement a = Html.A(prevPath);
				a.InnerText = "前のページ";
				XmlElement li = Html.Create("li");
				li.AppendChild(a);
				result.AppendChild(li);
			}
			if(myPageNum < maxPageNum){
				string nextQuery = baseQuery + string.Format(";p={0}", myPageNum+1);
				AbsPath nextPath = myModel.BasePath.CombineQuery(nextQuery);
				XmlElement a = Html.A(nextPath);
				a.InnerText = "次のページ";
				XmlElement li = Html.Create("li");
				li.AppendChild(a);
				result.AppendChild(li);
			}
			return result;

		}
	} // End class
} // End Namespace Bakera



