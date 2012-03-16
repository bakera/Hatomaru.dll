using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace Bakera.Hatomaru{

/// <summary>
/// Amazon�������ʂ�\������A�N�V�����ł��B
/// </summary>
	public partial class AmazonDoSearch : AmazonAction{

		public new const string Label = "�A�}��";
		private string myQuery;
		private AmazonIndexType myIndexType;
		private int myPageNum;

// �R���X�g���N�^

		/// <summary>
		/// �A�}���̌����̂��߂̃A�N�V�����̃C���X�^���X���J�n���܂��B
		/// </summary>
		public AmazonDoSearch(AmazonSearch model, AbsPath path, string query, AmazonIndexType ait, int page) : base(model, path){
			myPath = myModel.BasePath;
			myQuery = query;
			myIndexType = ait;
			myPageNum = page;
			if(myPageNum < 1) myPageNum = 1;
		}

// �v���p�e�B

		/// <summary>
		/// �f�[�^�� GET ���AHatomaruResponse ���擾���܂��B
		/// </summary>
		protected override HatomaruResponse GetHtmlResponse(){
			InsertHeading(2, Label);
			Html.Append(GetSearchForm(myQuery, myIndexType));

			// ��������
			if(!string.IsNullOrEmpty(myQuery)){
				Html.Append(Search());
			}
			return Response;
		}


		// �������Č��ʂ𓾂܂��B
		private XmlNode Search(){
			XmlNode result = Html.CreateDocumentFragment();

			string title = string.Format("�u{0}�v�̌�������", myQuery);
			result.AppendChild(Html.H(3, null, title));
			Response.SelfTitle = title;

			AmazonItemList items = Model.Manager.AmazonManager.GetSearchItem(myIndexType, myQuery, myPageNum);
			if(items == null){
				XmlElement p3 = Html.P(null, "�����ɖ�肪�������Ă���悤�ł��B���΂炭���҂����������B");
				result.AppendChild(p3);
				return result;
			}
			if(items.Count == 0){
				XmlElement p3 = Html.P(null, "�c�O�Ȃ���A����������܂���ł����B");
				result.AppendChild(p3);
				return result;
			}
			XmlElement p = Html.P();
			p.InnerText = string.Format("{0} ��������܂����B({1}�y�[�W��/��{2}�y�[�W)", items.TotalResults, myPageNum, items.TotalPages);
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

			// �O�̃y�[�W
			if(myPageNum > 1){
				string prevQuery = baseQuery + string.Format(";p={0}", myPageNum-1);
				AbsPath prevPath = myModel.BasePath.CombineQuery(prevQuery);
				XmlElement a = Html.A(prevPath);
				a.InnerText = "�O�̃y�[�W";
				XmlElement li = Html.Create("li");
				li.AppendChild(a);
				result.AppendChild(li);
			}
			if(myPageNum < maxPageNum){
				string nextQuery = baseQuery + string.Format(";p={0}", myPageNum+1);
				AbsPath nextPath = myModel.BasePath.CombineQuery(nextQuery);
				XmlElement a = Html.A(nextPath);
				a.InnerText = "���̃y�[�W";
				XmlElement li = Html.Create("li");
				li.AppendChild(a);
				result.AppendChild(li);
			}
			return result;

		}
	} // End class
} // End Namespace Bakera



