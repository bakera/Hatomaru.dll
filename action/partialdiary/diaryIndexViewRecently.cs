using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace Bakera.Hatomaru{

/// <summary>
/// �ŋ߂̓��L��\������A�N�V�����ł��B
/// </summary>
	public class DiaryIndexViewRecently : PartialDiaryAction{

		public const string Label = "�ŋ߈�T�Ԃقǂ̂��ѓ��L";

// �R���X�g���N�^

		/// <summary>
		/// �ŋ߂̓��L�\���̂��߂̃A�N�V�����̃C���X�^���X���J�n���܂��B
		/// </summary>
		public DiaryIndexViewRecently(DiaryIndex model, AbsPath path) : base(model, path){
			myPath = myModel.BasePath;
		}


		/// <summary>
		/// �f�[�^�� GET ���AHatomaruResponse ���擾���܂��B
		/// </summary>
		protected override HatomaruResponse GetHtmlResponse(){
			Topic[] topics = GetRecentTopics();
			if(topics.Length == 0) return NotFound();
			Topic oldestTopic = topics[topics.Length-1];
			
			int year = oldestTopic.Date.Year;
			YearDiary oldestDiary = Diary.GetYearDiary(year);

			Response.SelfTitle = Label;
			InsertHeading(2, Label);
			Html.Append(DiaryListWithDate(topics, 3));

			// ���y�[�W (��O�̓�)
			DateTime prevDate = GetPrevDate(oldestTopic.Date);
			if(prevDate != default(DateTime)){
				AbsPath prevPath = myModel.BasePath.Combine(prevDate.Year, prevDate.Month, prevDate.Day);
				string prevTitle = string.Format("{0}��{1}", prevDate.ToString(DateFormat), myModel.TopicSuffix);
				Prev = new LinkItem(prevPath, prevTitle);
			}


			//RSS�I�[�g�f�B�X�J�o��
			LinkItem rssLink = new LinkItem(BasePath.Combine(DiaryIndexRss.Id), Label + "(RSS1.0)");
			Html.AddLinkRel("alternate", HatomaruResponse.RssMediaType, rssLink);

			LinkItem atomLink = new LinkItem(BasePath.Combine(DiaryIndexAtom.Id), Label + "(Atom1.0)");
			Html.AddLinkRel("alternate", HatomaruResponse.AtomMediaType, atomLink);

			return Response;
		}




	} // End class
} // End Namespace Bakera



