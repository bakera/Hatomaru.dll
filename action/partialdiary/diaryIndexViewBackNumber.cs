using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace Bakera.Hatomaru{

/// <summary>
/// �ŋ߂̓��L��\������A�N�V�����ł��B
/// </summary>
	public partial class DiaryIndexViewBackNumber : PartialDiaryAction{

		public const string Label = "���ѓ��L�o�b�N�i���o�[";
		public const string Id = "backnumber";

// �R���X�g���N�^

		/// <summary>
		/// �ŋ߂̓��L�\���̂��߂̃A�N�V�����̃C���X�^���X���J�n���܂��B
		/// </summary>
		public DiaryIndexViewBackNumber(DiaryIndex model, AbsPath path) : base(model, path){
			myPath = myModel.BasePath.Combine(Id);
		}

		/// <summary>
		/// �f�[�^�� GET ���AHatomaruResponse ���擾���܂��B
		/// </summary>
		protected override HatomaruResponse GetHtmlResponse(){

			Response.SelfTitle = Label;
			Response.AddTopicPath(Path, Label);
			InsertHeading(2, Label);

			XmlNode result = Html.Create("div", "backnumber");
			foreach(YearDiary yd in Diary.DiaryList){
				myResponse.AddDataSource(yd);
				AbsPath linkPath = Diary.BasePath.Combine(yd.Year);
				XmlElement a = Html.A(linkPath);
				a.InnerText = yd.Dates[0].ToString(YearFormat);
				result.AppendChild(Html.H(3, null, a));
				XmlElement p = Html.P();
				result.AppendChild(p);
				DateTime currentDate = default(DateTime);
				bool firstFlag = true;
				foreach(DateTime d in yd.Dates){
					if(d.Month == currentDate.Month && d.Year == currentDate.Year) continue;
					AbsPath monthLinkPath = Diary.BasePath.Combine(d.Year, d.Month);
					XmlElement monthA = Html.A(monthLinkPath);
					monthA.InnerText = d.ToString("M��");
					if(!firstFlag){
						p.AppendChild(Html.Space);
						p.AppendChild(Html.Span("separate", "|"));
						p.AppendChild(Html.Space);
					}
					p.AppendChild(monthA);
					currentDate = d;
					firstFlag = false;
				}
			}
			Html.Append(result);
			return Response;
		}




	} // End class
} // End Namespace Bakera



