using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace Bakera.Hatomaru{

/// <summary>
/// �f�[�^�`���̈ꗗ��\������A�N�V�����ł��B
/// </summary>
	public partial class HtmlRefViewData : HtmlRefAction{

		public new const string Label = "�f�[�^�`��";
		private string myId;

// �R���X�g���N�^

		/// <summary>
		/// �f�[�^�`���\���̂��߂̃A�N�V�����̃C���X�^���X���J�n���܂��B
		/// </summary>
		public HtmlRefViewData(HatomaruHtmlRef model, AbsPath path, string id) : base(model, path){
			Data = model;
			myPath = myModel.BasePath.Combine(HtmlRefViewDataList.Id, id);
			myId = id;
		}

// �v���p�e�B
		protected HatomaruHtmlRef Data{get; set;}

		/// <summary>
		/// �f�[�^�� GET ���AHatomaruResponse ���擾���܂��B
		/// </summary>
		protected override HatomaruResponse GetHtmlResponse(){
			HtmlData hd = Data.GetData(myId);
			if(hd == null) return NotFound();

			InsertHeading(2, hd.FullName);
			Response.SelfTitle = hd.FullName;

			XmlElement result = Html.Div("data");
			result.AppendChild(GetDataOwnerInfo(hd.Parents));
			result.AppendChild(GetDescription(hd));
			Html.Append(result);

			Response.SelfTitle = hd.FullName;
			Response.AddTopicPath(BasePath.Combine(HtmlRefViewDataList.Id), Label);
			Response.AddTopicPath(myPath, hd.FullName);

			Html.Append(CommentLink(Path, Response.SelfTitle));
			return Response;
		}

		protected XmlNode GetDataOwnerInfo(HtmlItem[] items){
			XmlNode result = Html.CreateDocumentFragment();
			if(items == null || items.Length == 0) return result;
			var elemList = new List<HtmlItem>();
			var elemGroupList = new List<HtmlItem>();
			var attrList = new List<HtmlItem>();
			foreach(HtmlItem i in items){
				if(i is HtmlElement){
					elemList.Add(i);
				} else if(i is HtmlElementGroup){
					elemGroupList.Add(i);
				} else if(i is HtmlAttribute){
					attrList.Add(i);
				}
			}
			if(elemList.Count > 0){
				XmlElement p = Html.P();
				p.InnerText = "���̃f�[�^�����v�f �c�c ";
				p.AppendChild(GetHtmlItemList(elemList.ToArray(), ", "));
				result.AppendChild(p);
			}
			if(elemGroupList.Count > 0){
				XmlElement p = Html.P();
				p.InnerText = "���̃f�[�^�����O���[�v �c�c ";
				p.AppendChild(GetHtmlItemList(elemGroupList.ToArray(), ", "));
				result.AppendChild(p);
			}
			if(attrList.Count > 0){
				XmlElement p = Html.P();
				p.InnerText = "���̃f�[�^�������� �c�c ";
				p.AppendChild(GetHtmlItemList(attrList.ToArray(), ", "));
				result.AppendChild(p);
			}
			return result;
		}



	} // End class
} // End Namespace Bakera



