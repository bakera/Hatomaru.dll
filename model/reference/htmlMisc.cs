using System;
using System.Collections.Generic;
using System.Xml;

namespace Bakera.Hatomaru{

	// HTML �̗v�f�E�����E�v�f�O���[�v�E�����O���[�v�E�f�[�^�`���ɊY�����Ȃ��f�[�^��\������N���X
	public class HtmlMisc : HtmlItem{
		public HtmlMisc(string name){
			myName = name;
		}

		public override string LinkId{
			get{return null;}
		}

		// �e��ǉ�����Ă��������Ȃ�
		public override void AddParent(HtmlItem item){}

	}
}