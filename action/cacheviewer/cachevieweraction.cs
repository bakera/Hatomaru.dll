using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Xml;

namespace Bakera.Hatomaru{

/// <summary>
/// CacheViewer �𐧌䂷��N���X�ł��B
/// </summary>
	public partial class CacheViewerAction : HatomaruGetAction{

// �R���X�g���N�^

		public CacheViewerAction(CacheViewer model, AbsPath path) : base(model, path){
			myResponse = new NoCacheResponse(Model, path);
			GetHtml();
		}


// �v���p�e�B

		/// <summary>
		/// �f�[�^�� GET ���AHatomaruResponse ���擾���܂��B
		/// </summary>
		protected override HatomaruResponse GetHtmlResponse(){
			InsertHeading(2, "�L���b�V���f�[�^�ꗗ");

			HatomaruResponse[] cached = myModel.Manager.GetCachedResponse();
			if(cached.Length == 0){
				Html.Append(Html.P(null, "�L���b�V������Ă���f�[�^�͂���܂���B"));
			} else {
				Html.Append(Html.P(null, "�L���b�V��:", cached.Length));
				Html.Append(GetCacheDatas(cached));
			}
			return Response;
		}

		private XmlNode GetCacheDatas(HatomaruResponse[] cached){
			XmlElement result = Html.Create("table");
			foreach(HatomaruResponse hr in cached){
				// �f�[�^�\�[�X���܂Ƃ߂�
				string dataSourceNames = "";
				foreach(HatomaruData hd in hr.DataSource){
					dataSourceNames += hd.File.FullName.CutLeft(Model.Manager.IniData.DataPath.FullName);
					dataSourceNames += " ";
				}
				if(hr is RedirectResponse){
					RedirectResponse rr = hr as RedirectResponse;
					XmlElement tr = Html.Tr(null, 1, "", hr.Path, hr.StatusCode, hr.Length, hr.LastModified, rr.DestPath);
					result.AppendChild(tr);
				} else {
					XmlElement tr = Html.Tr(null, 1, hr.Title, hr.Path, hr.StatusCode, hr.Length, hr.LastModified, dataSourceNames);
					result.AppendChild(tr);
				}
			}
		return result;
		}



	} // End class BbsController
} // End Namespace Bakera



