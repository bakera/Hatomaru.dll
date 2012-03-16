using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace Bakera.Hatomaru{

/// <summary>
/// 最近の日記を表示するアクションです。
/// </summary>
	public partial class DiaryIndexViewBackNumber : PartialDiaryAction{

		public const string Label = "えび日記バックナンバー";
		public const string Id = "backnumber";

// コンストラクタ

		/// <summary>
		/// 最近の日記表示のためのアクションのインスタンスを開始します。
		/// </summary>
		public DiaryIndexViewBackNumber(DiaryIndex model, AbsPath path) : base(model, path){
			myPath = myModel.BasePath.Combine(Id);
		}

		/// <summary>
		/// データを GET し、HatomaruResponse を取得します。
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
					monthA.InnerText = d.ToString("M月");
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



