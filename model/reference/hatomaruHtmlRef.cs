using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml;

namespace Bakera.Hatomaru{

/// <summary>
/// HTML���t�@�����X����������N���X�ł��B
/// </summary>
	public partial class HatomaruHtmlRef : HatomaruXml{

		public new const string Name = "ref"; // ���[�g�v�f�̖��O

		public const string ElementsRefName = "elements-reference";
		public const string AttributesRefName = "attributes-reference";
		public const string DataRefName = "data-reference";

		public const string DataFormatElementName = "data-format";
		public const string ElementElementName = "element";
		public const string ElementGroupElementName = "elemgroup";
		public const string AttributeElementName = "attribute";
		public const string AttributeGroupElementName = "attrgroup";

		public const string NameJaElementName = "name-ja";
		public const string NoteElementName = "note";
		public const string NoteJaElementName = "note-ja";
		public const string VersionElementName = "version";
		public const string OmitElementName = "omit";
		public const string ContentElementName = "content";
		public const string AttributesElementName = "attributes";
		public const string ElementsElementName = "elements";
		public const string DatasElementName = "datas";
		public const string GroupsElementName = "groups";
		public const string DescElementName = "desc";
		public const string DefaultElementName = "default";
		public const string ValueElementName = "value";

		public const string IdAttributeName = "id";
		public const string NameAttributeName = "name";
		public const string ForAttributeName = "for";

		public const string EmptyContent = "EMPTY";
		public const string CDATAContent = "CDATA";
		public const string ContentExcludeSymbol = " -";
		
		private static Regex ContentRegex = new Regex("[%#]?[-\\.A-Za-z0-9]+;?");


// �R���X�g���N�^

		/// <summary>
		/// ���ۃf�[�^�̃f�[�^�\�[�X�� FileInfo �� XmlDocument ���w�肵�āAHatomaruHtmlReference �̃C���X�^���X���J�n���܂��B
		/// </summary>
		public HatomaruHtmlRef(HatomaruManager manager, FileInfo f, XmlDocument x) : base(manager, f, x){
			InitTables();
			Load();
		}

// �v���p�e�B
		/// <summary>
		/// HtmlDataformatTable ��ݒ�E�擾���܂��B
		/// </summary>
		protected HtmlDataformatTable DataTable{get; set;}
		protected HtmlElementTable ElementTable{get; set;}
		protected HtmlElementGroupTable ElementGroupTable{get; set;}
		protected HtmlAttributeTable AttributeTable{get; set;}
		protected HtmlAttributeGroupTable AttributeGroupTable{get; set;}



// �f�[�^�擾���\�b�h
		/// <summary>
		/// �S�Ẵf�[�^�`�����擾���܂��B
		/// </summary>
		public HtmlData[] GetAllData(){
			DataRow[] dr = DataTable.Select();
			return DataRowsToHtmlData(dr);
		}

		/// <summary>
		/// �w�肳�ꂽID�̃f�[�^���擾���܂��B
		/// </summary>
		public HtmlData GetData(string id){
			return DataTable.GetData(HtmlDataformatTable.IdColName, id, DataTable.DataCol) as HtmlData;
		}

		/// <summary>
		/// �w�肳�ꂽ���O�̃f�[�^���擾���܂��B
		/// </summary>
		public HtmlData GetDataByName(string name){
			return DataTable.GetData(HtmlDataformatTable.NameColName, name, DataTable.DataCol) as HtmlData;
		}

		// Row[] �� HtmlData[] �ɕϊ����܂��B
		private HtmlData[] DataRowsToHtmlData(DataRow[] rows){
			HtmlData[] result = new HtmlData[rows.Length];
			for(int i = 0; i < rows.Length; i++){
				result[i] = GetHtmlData(rows[i]);
			}
			return result;
		}

		/// <summary>
		/// DataRow ���� HtmlData ���擾���܂��B
		/// </summary>
		private HtmlData GetHtmlData(DataRow row){
			if(row == null) return null;
			return row[DataTable.DataCol] as HtmlData;
		}


		/// <summary>
		/// �S�Ă̗v�f���擾���܂��B
		/// </summary>
		public HtmlElement[] GetAllElements(){
			DataRow[] dr = ElementTable.Select();
			return DataRowsToHtmlElement(dr);
		}

		/// <summary>
		/// �S�Ă̗v�f���A���t�@�x�b�g���Ɏ擾���܂��B
		/// </summary>
		public HtmlElement[] GetSortedElements(){
			DataRow[] dr = ElementTable.Select(null, HtmlElementTable.NameColName);
			return DataRowsToHtmlElement(dr);
		}

		// Row[] �� HtmlElement[] �ɕϊ����܂��B
		private HtmlElement[] DataRowsToHtmlElement(DataRow[] rows){
			HtmlElement[] result = new HtmlElement[rows.Length];
			for(int i = 0; i < rows.Length; i++){
				result[i] = GetHtmlElement(rows[i]);
			}
			return result;
		}

		/// <summary>
		/// DataRow ���� HtmlElement ���擾���܂��B
		/// </summary>
		private HtmlElement GetHtmlElement(DataRow row){
			if(row == null) return null;
			return row[ElementTable.ElementCol] as HtmlElement;
		}

		/// <summary>
		/// �w�肳�ꂽID�̗v�f���擾���܂��B
		/// </summary>
		public HtmlElement GetElement(string id){
			return ElementTable.GetData(HtmlElementTable.IdColName, id, ElementTable.ElementCol) as HtmlElement;
		}

		/// <summary>
		/// �w�肳�ꂽ���O�̗v�f���擾���܂��B
		/// </summary>
		public HtmlElement GetElementByName(string name){
			return ElementTable.GetData(HtmlElementTable.NameColName, name, ElementTable.ElementCol) as HtmlElement;
		}



		/// <summary>
		/// �S�Ă̑������擾���܂��B
		/// </summary>
		public HtmlAttribute[] GetAllAttributes(){
			DataRow[] dr = AttributeTable.Select();
			return DataRowsToHtmlAttribute(dr);
		}

		/// <summary>
		/// �S�Ă̑������A���t�@�x�b�g���Ɏ擾���܂��B
		/// </summary>
		public HtmlAttribute[] GetSortedAttributes(){
			DataRow[] dr = AttributeTable.Select(null, HtmlAttributeTable.NameColName);
			return DataRowsToHtmlAttribute(dr);
		}

		// Row[] �� HtmlAttribute[] �ɕϊ����܂��B
		private HtmlAttribute[] DataRowsToHtmlAttribute(DataRow[] rows){
			HtmlAttribute[] result = new HtmlAttribute[rows.Length];
			for(int i = 0; i < rows.Length; i++){
				result[i] = GetHtmlAttribute(rows[i]);
			}
			return result;
		}

		/// <summary>
		/// DataRow ���� HtmlAttribute ���擾���܂��B
		/// </summary>
		private HtmlAttribute GetHtmlAttribute(DataRow row){
			if(row == null) return null;
			return row[AttributeTable.AttributeCol] as HtmlAttribute;
		}

		/// <summary>
		/// �w�肳�ꂽID�̑������擾���܂��B
		/// ����̗v�f�Ɍ��т��������� attr@element �� ID �ƂȂ�܂��B
		/// </summary>
		public HtmlAttribute GetAttribute(string id){
			return AttributeTable.GetData(HtmlAttributeTable.IdColName, id, AttributeTable.AttributeCol) as HtmlAttribute;
		}

		/// <summary>
		/// �w�肳�ꂽ�v�f�̎w�肳�ꂽ���O�̑������擾���܂��B
		/// </summary>
		public HtmlAttribute GetAttribute(string id, string elemName){
			HtmlAttribute item = null;
			if(!string.IsNullOrEmpty(elemName)){
				string fqid = id + HtmlAttribute.IdSeparator + elemName;
				item = GetAttribute(fqid);
			}
			if(item == null) item = GetAttribute(id);
			return item;
		}

		/// <summary>
		/// �w�肳�ꂽ�v�f�̎w�肳�ꂽ���O�̑������擾���܂��B
		/// </summary>
		public HtmlAttribute GetAttribute(string id, HtmlElement he){
			return GetAttribute(id, he.Id);
		}



		/// <summary>
		/// �S�Ă̗v�f�O���[�v���擾���܂��B
		/// </summary>
		public HtmlElementGroup[] GetAllElementGroups(){
			DataRow[] dr = ElementGroupTable.Select();
			return DataRowsToHtmlElementGroup(dr);
		}

		// Row[] �� HtmlElementGroup[] �ɕϊ����܂��B
		private HtmlElementGroup[] DataRowsToHtmlElementGroup(DataRow[] rows){
			HtmlElementGroup[] result = new HtmlElementGroup[rows.Length];
			for(int i = 0; i < rows.Length; i++){
				result[i] = GetHtmlElementGroup(rows[i]);
			}
			return result;
		}

		/// <summary>
		/// DataRow ���� HtmlElementGroup ���擾���܂��B
		/// </summary>
		private HtmlElementGroup GetHtmlElementGroup(DataRow row){
			if(row == null) return null;
			return row[ElementGroupTable.DataCol] as HtmlElementGroup;
		}

		/// <summary>
		/// �w�肳�ꂽID�̗v�f�O���[�v���擾���܂��B
		/// </summary>
		public HtmlElementGroup GetElementGroup(string id){
			return ElementGroupTable.GetData(HtmlElementGroupTable.IdColName, id, ElementGroupTable.DataCol) as HtmlElementGroup;
		}

		/// <summary>
		/// �w�肳�ꂽ���O�̗v�f�O���[�v���擾���܂��B
		/// </summary>
		public HtmlElementGroup GetElementGroupByName(string name){
			return ElementGroupTable.GetData(HtmlElementGroupTable.NameColName, name, ElementGroupTable.DataCol) as HtmlElementGroup;
		}

		/// <summary>
		/// �S�Ă̑����O���[�v���擾���܂��B
		/// </summary>
		public HtmlAttributeGroup[] GetAllAttributeGroups(){
			DataRow[] dr = AttributeGroupTable.Select();
			return DataRowsToHtmlAttributeGroup(dr);
		}

		// Row[] �� HtmlAttributeGroup[] �ɕϊ����܂��B
		private HtmlAttributeGroup[] DataRowsToHtmlAttributeGroup(DataRow[] rows){
			HtmlAttributeGroup[] result = new HtmlAttributeGroup[rows.Length];
			for(int i = 0; i < rows.Length; i++){
				result[i] = GetHtmlAttributeGroup(rows[i]);
			}
			return result;
		}

		/// <summary>
		/// DataRow ���� HtmlAttributeGroup ���擾���܂��B
		/// </summary>
		private HtmlAttributeGroup GetHtmlAttributeGroup(DataRow row){
			if(row == null) return null;
			return row[AttributeGroupTable.DataCol] as HtmlAttributeGroup;
		}

		/// <summary>
		/// �w�肳�ꂽID�̗v�f���擾���܂��B
		/// </summary>
		public HtmlAttributeGroup GetAttributeGroup(string id){
			return AttributeGroupTable.GetData(HtmlAttributeGroupTable.IdColName, id, AttributeGroupTable.DataCol) as HtmlAttributeGroup;
		}

		/// <summary>
		/// �w�肳�ꂽ���O�̑����O���[�v���擾���܂��B
		/// </summary>
		public HtmlAttributeGroup GetAttributeGroupByName(string name){
			return AttributeGroupTable.GetData(HtmlAttributeGroupTable.NameColName, name, AttributeGroupTable.DataCol) as HtmlAttributeGroup;
		}




// �I�[�o�[���C�h���\�b�h

		/// <summary>
		/// �f�[�^�� GET ���AHatomaruResponse ���擾���܂��B
		/// </summary>
		public override HatomaruResponse Get(AbsPath path){
			HatomaruGetAction act = GetAction(path);
			HatomaruResponse result = act.Get();
			result.SetLastModified();
			return result;
		}

		/// <summary>
		/// path �����ɁA�K�؂ȃR���g���[�����쐬���܂��B
		/// </summary>
		private HatomaruGetAction GetAction(AbsPath path){
			string[] fragments = path.GetFragments(BasePath);
			if(fragments.Length > 0){
				string first = fragments[0];
				switch(first){
				case HtmlRefViewDataList.Id:
					if(fragments.Length > 1){
						string second = fragments[1];
						if(fragments.Length > 2 && fragments[2].Equals(HatomaruActionBase.CommentPath, StringComparison.InvariantCultureIgnoreCase)){
							return new ViewComment(this, path, BasePath.Combine(first, second));
						}
						return new HtmlRefViewData(this, path, second);
					}
					return new HtmlRefViewDataList(this, path);
				case HtmlRefViewElementList.Id:
					if(fragments.Length > 1){
						string second = fragments[1];
						if(fragments.Length > 2 && fragments[2].Equals(HatomaruActionBase.CommentPath, StringComparison.InvariantCultureIgnoreCase)){
							return new ViewComment(this, path, BasePath.Combine(first, second));
						}
						return new HtmlRefViewElement(this, path, second);
					}
					return new HtmlRefViewElementList(this, path);
				case HtmlRefViewAttributeList.Id:
					if(fragments.Length > 1){
						string second = fragments[1];
						if(fragments.Length > 2 && fragments[2].Equals(HatomaruActionBase.CommentPath, StringComparison.InvariantCultureIgnoreCase)){
							return new ViewComment(this, path, BasePath.Combine(first, second));
						}
						return new HtmlRefViewAttribute(this, path, second);
					}
					return new HtmlRefViewAttributeList(this, path);
				case HtmlRefViewElementGroupList.Id:
					if(fragments.Length > 1){
						string second = fragments[1];
						if(fragments.Length > 2 && fragments[2].Equals(HatomaruActionBase.CommentPath, StringComparison.InvariantCultureIgnoreCase)){
							return new ViewComment(this, path, BasePath.Combine(first, second));
						}
						return new HtmlRefViewElementGroup(this, path, second);
					}
					return new HtmlRefViewElementGroupList(this, path);
				case HtmlRefViewAttributeGroupList.Id:
					if(fragments.Length > 1){
						string second = fragments[1];
						if(fragments.Length > 2 && fragments[2].Equals(HatomaruActionBase.CommentPath, StringComparison.InvariantCultureIgnoreCase)){
							return new ViewComment(this, path, BasePath.Combine(first, second));
						}
						return new HtmlRefViewAttributeGroup(this, path, second);
					}
					return new HtmlRefViewAttributeGroupList(this, path);
				}
			}
			return new HtmlRefViewIndex(this, path);
		}




// Table �̏�����
		private void InitTables(){
			DataTable = new HtmlDataformatTable();
			ElementTable = new HtmlElementTable();
			ElementGroupTable = new HtmlElementGroupTable();
			AttributeTable = new HtmlAttributeTable();
			AttributeGroupTable = new HtmlAttributeGroupTable();
		}

		/// <summary>
		/// �w�肳�ꂽ XML �t�@�C������f�[�^�����[�h���܂��B
		/// ���̏����̓X���b�h�Z�[�t�ł͂���܂���B
		/// </summary>
		public void Load(){
			XmlNodeList elems = Document.DocumentElement[ElementsRefName].GetElementsByTagName(ElementElementName);
			XmlNodeList elemGroups = Document.DocumentElement[ElementsRefName].GetElementsByTagName(ElementGroupElementName);
			XmlNodeList attrs = Document.DocumentElement[AttributesRefName].GetElementsByTagName(AttributeElementName);
			XmlNodeList attrGroups = Document.DocumentElement[AttributesRefName].GetElementsByTagName(AttributeGroupElementName);
			XmlNodeList datas = Document.DocumentElement[DataRefName].GetElementsByTagName(DataFormatElementName);

			HtmlElement[] he = new HtmlElement[elems.Count];
			for(int i=0; i < elems.Count; i++){he[i] = new HtmlElement(elems[i] as XmlElement);}
			HtmlElementGroup[] heg = new HtmlElementGroup[elemGroups.Count];
			for(int i=0; i < elemGroups.Count; i++){heg[i] = new HtmlElementGroup(elemGroups[i] as XmlElement);}
			HtmlAttribute[] ha = new HtmlAttribute[attrs.Count];
			for(int i=0; i < attrs.Count; i++){ha[i] = new HtmlAttribute(attrs[i] as XmlElement);}
			HtmlAttributeGroup[] hag = new HtmlAttributeGroup[attrGroups.Count];
			for(int i=0; i < attrGroups.Count; i++){hag[i] = new HtmlAttributeGroup(attrGroups[i] as XmlElement);}
			HtmlData[] hd = new HtmlData[datas.Count];
			for(int i=0; i < datas.Count; i++){hd[i] = new HtmlData(datas[i] as XmlElement);}

			ElementTable.MinimumCapacity = he.Length;
			Array.ForEach(he, item=>{ElementTable.AddData(item);});
			ElementGroupTable.MinimumCapacity = heg.Length;
			Array.ForEach(heg, item=>{ElementGroupTable.AddData(item);});
			AttributeTable.MinimumCapacity = ha.Length;
			Array.ForEach(ha, item=>{AttributeTable.AddData(item);});
			AttributeGroupTable.MinimumCapacity = hag.Length;
			Array.ForEach(hag, item=>{AttributeGroupTable.AddData(item);});
			DataTable.MinimumCapacity = hd.Length;
			Array.ForEach(hd, item=>{DataTable.AddData(item);});

			// ������e�q�֌W���擾
			Array.ForEach(he, item=>{SetChildren(item);SetAttribute(item);});
			Array.ForEach(heg, item=>{SetChildren(item);});
			Array.ForEach(ha, item=>{SetChildren(item);});
			Array.ForEach(hag, item=>{SetChildren(item);});

		}

		// �v�f�̎q�v�f����͂��Ĕz��Ɋi�[���܂��B
		// �r���v�f�ȊO�̗v�f�ɑ΂��ẮA�e�Ƃ��Ċ֘A�Â��܂��B
		private void SetChildren(HtmlElement he){
			string contentStr = he.XmlElement.GetInnerText(ContentElementName);
			if(contentStr == EmptyContent) he.IsEmptyElement = true;
			int exIndex = contentStr.IndexOf(ContentExcludeSymbol);
			string include;
			string exclude;
			if(exIndex < 0){
				include = contentStr;
				exclude = null;
			} else {
				include = contentStr.Substring(0, exIndex);
				exclude = contentStr.Substring(exIndex);
			}
			List<HtmlItem> includeItems = SearchChildren(include);
			// include �ȗv�f�����ɑ΂��Ă͐e�Ƃ��Ċ֘A�Â���
			includeItems.ForEach(item=>{item.AddParent(he);});
			includeItems.AddRange(SearchChildren(exclude));
			he.Content = includeItems.ToArray();
		}

		// �v�f�O���[�v�̎q�v�f����͂��Ĕz��Ɋi�[���܂��B
		// �r���v�f�ȊO�̗v�f�ɑ΂��ẮA�e�Ƃ��Ċ֘A�Â��܂��B
		private void SetChildren(HtmlElementGroup heg){
			List<HtmlItem> contents = new List<HtmlItem>();
			//�v�f��T��
			foreach(XmlElement e in heg.XmlElement.ChildNodes){
				if(e == null) continue;
				string s = e.GetInnerText();
				HtmlItem item = null;
				switch(e.LocalName){
					case ElementsElementName:
					item = GetElement(s);
					break;
					case GroupsElementName:
					item = GetElementGroup(s);
					if(item == null) item = GetElementGroupByName(s);
					break;
					case DatasElementName:
					item = GetDataByName(s);
					break;
				}
				if(item == null) continue;
				contents.Add(item);
				item.AddParent(heg);
			}
			heg.Content = contents.ToArray();
		}


		// �v�f�̎q�v�f����͂��Ĕz��Ɋi�[���܂��B
		// �r���v�f�ȊO�̗v�f�ɑ΂��ẮA�e�Ƃ��Ċ֘A�Â��܂��B
		private void SetAttribute(HtmlElement he){
			// ������T��
			List<HtmlAttribute> attrs = new List<HtmlAttribute>();
			foreach(XmlElement e in he.XmlElement.GetElementsByTagName(AttributeElementName)){
				if(e == null) continue;
				string s = e.GetInnerText();
				HtmlAttribute ha = GetAttribute(s, he);
				attrs.Add(ha);
				ha.AddParent(he);
			}
			he.Attributes = attrs.ToArray();

			// ������T��
			List<HtmlAttributeGroup> attrGroups = new List<HtmlAttributeGroup>();
			foreach(XmlElement e in he.XmlElement.GetElementsByTagName(AttributesElementName)){
				if(e == null) continue;
				string s = e.GetInnerText();
				HtmlAttributeGroup hag = GetAttributeGroupByName(s);
				attrGroups.Add(hag);
				hag.AddParent(he);
			}
			he.AttributeGroups = attrGroups.ToArray();
		}


		// �����l����͂��Ċi�[���܂��B
		// �e�Ƃ��Ċ֘A�Â��܂��B
		private void SetChildren(HtmlAttribute ha){
			string valueStr = ha.XmlElement.GetInnerText(ValueElementName);
			HtmlItem item = GetDataByName(valueStr);
			if(item == null) item = new HtmlMisc(valueStr);
			item.AddParent(ha);
			ha.Value = item;
		}


		// �����l����͂��Ċi�[���܂��B
		// �e�Ƃ��Ċ֘A�Â��܂��B
		private void SetChildren(HtmlAttributeGroup hag){
			List<HtmlAttribute> attrs = new List<HtmlAttribute>();
			foreach(XmlElement e in hag.XmlElement.GetElementsByTagName(AttributesElementName)){
				if(e == null) continue;
				string s = e.GetInnerText();
				HtmlAttribute ha = GetAttribute(s);
				attrs.Add(ha);
				ha.AddParent(hag);
			}
			hag.Attributes = attrs.ToArray();
		}

		// �q�v�f���������܂��B
		private List<HtmlItem> SearchChildren(string s){
			if(s == null) return new List<HtmlItem>();

			List<HtmlItem> items = new List<HtmlItem>();
			string test = s;
			for(;;){
				if(string.IsNullOrEmpty(test)) break;
				Match m = ContentRegex.Match(test);
				if(!m.Success){
					items.Add(new HtmlMisc(test));
					break;
				}
				// �}�b�`����
				string before = test.Substring(0, m.Index);
				string inner = m.Value;
				string after = test.Substring(m.Index + m.Length);
				if(!string.IsNullOrEmpty(before)) items.Add(new HtmlMisc(before));

				HtmlItem item = null;
				if(inner.StartsWith("%")){
					// �v�f�O���[�v��������������Ȃ������� Misc
					item = GetElementGroupByName(inner);
				} else {
					// �v�f���������f�[�^��������������Ȃ������� Misc
					item = GetElement(inner);
					if(item == null) item = GetDataByName(inner);
				}
				if(item == null) item = new HtmlMisc(inner);
				items.Add(item);
				test = after;
			}
			return items;
		}


	} // End class
} // End Namespace Bakera



