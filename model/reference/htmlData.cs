using System;
using System.Xml;

namespace Bakera.Hatomaru{
	
	/// <summary>
	/// ��̃f�[�^�`����\���N���X�ł��B
	/// </summary>
	public class HtmlData : HtmlItem{

		protected string myNameJa;


// �R���X�g���N�^
		/// <summary>
		/// XmlNode ���w�肵�āATopic �N���X�̃C���X�^���X���J�n���܂��B
		/// </summary>
		public HtmlData(XmlElement e) : base(e){
			myNameJa = e.GetInnerText(HatomaruHtmlRef.NameJaElementName);
		}


// �v���p�e�B


		/// <summary>
		/// ���{�ꖼ�O��ݒ�E�擾���܂��B
		/// </summary>
		public string NameJa{
			get {return myNameJa;}
			set {myNameJa = value;}
		}

		/// <summary>
		/// ���S�����擾���܂��B
		/// </summary>
		public override string FullName{
			get {return string.Format("{0} ({1})", this.Name, this.NameJa);}
		}

		public override string LinkId{
			get{return HtmlRefViewDataList.Id;}
		}

	}

} // namespace Bakera




