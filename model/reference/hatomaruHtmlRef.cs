using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml;

namespace Bakera.Hatomaru{

/// <summary>
/// HTMLリファレンスを処理するクラスです。
/// </summary>
	public partial class HatomaruHtmlRef : HatomaruXml{

		public new const string Name = "ref"; // ルート要素の名前

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


// コンストラクタ

		/// <summary>
		/// 鳩丸データのデータソースの FileInfo と XmlDocument を指定して、HatomaruHtmlReference のインスタンスを開始します。
		/// </summary>
		public HatomaruHtmlRef(HatomaruManager manager, FileInfo f, XmlDocument x) : base(manager, f, x){
			InitTables();
			Load();
		}

// プロパティ
		/// <summary>
		/// HtmlDataformatTable を設定・取得します。
		/// </summary>
		protected HtmlDataformatTable DataTable{get; set;}
		protected HtmlElementTable ElementTable{get; set;}
		protected HtmlElementGroupTable ElementGroupTable{get; set;}
		protected HtmlAttributeTable AttributeTable{get; set;}
		protected HtmlAttributeGroupTable AttributeGroupTable{get; set;}



// データ取得メソッド
		/// <summary>
		/// 全てのデータ形式を取得します。
		/// </summary>
		public HtmlData[] GetAllData(){
			DataRow[] dr = DataTable.Select();
			return DataRowsToHtmlData(dr);
		}

		/// <summary>
		/// 指定されたIDのデータを取得します。
		/// </summary>
		public HtmlData GetData(string id){
			return DataTable.GetData(HtmlDataformatTable.IdColName, id, DataTable.DataCol) as HtmlData;
		}

		/// <summary>
		/// 指定された名前のデータを取得します。
		/// </summary>
		public HtmlData GetDataByName(string name){
			return DataTable.GetData(HtmlDataformatTable.NameColName, name, DataTable.DataCol) as HtmlData;
		}

		// Row[] を HtmlData[] に変換します。
		private HtmlData[] DataRowsToHtmlData(DataRow[] rows){
			HtmlData[] result = new HtmlData[rows.Length];
			for(int i = 0; i < rows.Length; i++){
				result[i] = GetHtmlData(rows[i]);
			}
			return result;
		}

		/// <summary>
		/// DataRow から HtmlData を取得します。
		/// </summary>
		private HtmlData GetHtmlData(DataRow row){
			if(row == null) return null;
			return row[DataTable.DataCol] as HtmlData;
		}


		/// <summary>
		/// 全ての要素を取得します。
		/// </summary>
		public HtmlElement[] GetAllElements(){
			DataRow[] dr = ElementTable.Select();
			return DataRowsToHtmlElement(dr);
		}

		/// <summary>
		/// 全ての要素をアルファベット順に取得します。
		/// </summary>
		public HtmlElement[] GetSortedElements(){
			DataRow[] dr = ElementTable.Select(null, HtmlElementTable.NameColName);
			return DataRowsToHtmlElement(dr);
		}

		// Row[] を HtmlElement[] に変換します。
		private HtmlElement[] DataRowsToHtmlElement(DataRow[] rows){
			HtmlElement[] result = new HtmlElement[rows.Length];
			for(int i = 0; i < rows.Length; i++){
				result[i] = GetHtmlElement(rows[i]);
			}
			return result;
		}

		/// <summary>
		/// DataRow から HtmlElement を取得します。
		/// </summary>
		private HtmlElement GetHtmlElement(DataRow row){
			if(row == null) return null;
			return row[ElementTable.ElementCol] as HtmlElement;
		}

		/// <summary>
		/// 指定されたIDの要素を取得します。
		/// </summary>
		public HtmlElement GetElement(string id){
			return ElementTable.GetData(HtmlElementTable.IdColName, id, ElementTable.ElementCol) as HtmlElement;
		}

		/// <summary>
		/// 指定された名前の要素を取得します。
		/// </summary>
		public HtmlElement GetElementByName(string name){
			return ElementTable.GetData(HtmlElementTable.NameColName, name, ElementTable.ElementCol) as HtmlElement;
		}



		/// <summary>
		/// 全ての属性を取得します。
		/// </summary>
		public HtmlAttribute[] GetAllAttributes(){
			DataRow[] dr = AttributeTable.Select();
			return DataRowsToHtmlAttribute(dr);
		}

		/// <summary>
		/// 全ての属性をアルファベット順に取得します。
		/// </summary>
		public HtmlAttribute[] GetSortedAttributes(){
			DataRow[] dr = AttributeTable.Select(null, HtmlAttributeTable.NameColName);
			return DataRowsToHtmlAttribute(dr);
		}

		// Row[] を HtmlAttribute[] に変換します。
		private HtmlAttribute[] DataRowsToHtmlAttribute(DataRow[] rows){
			HtmlAttribute[] result = new HtmlAttribute[rows.Length];
			for(int i = 0; i < rows.Length; i++){
				result[i] = GetHtmlAttribute(rows[i]);
			}
			return result;
		}

		/// <summary>
		/// DataRow から HtmlAttribute を取得します。
		/// </summary>
		private HtmlAttribute GetHtmlAttribute(DataRow row){
			if(row == null) return null;
			return row[AttributeTable.AttributeCol] as HtmlAttribute;
		}

		/// <summary>
		/// 指定されたIDの属性を取得します。
		/// 特定の要素に結びついた属性は attr@element が ID となります。
		/// </summary>
		public HtmlAttribute GetAttribute(string id){
			return AttributeTable.GetData(HtmlAttributeTable.IdColName, id, AttributeTable.AttributeCol) as HtmlAttribute;
		}

		/// <summary>
		/// 指定された要素の指定された名前の属性を取得します。
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
		/// 指定された要素の指定された名前の属性を取得します。
		/// </summary>
		public HtmlAttribute GetAttribute(string id, HtmlElement he){
			return GetAttribute(id, he.Id);
		}



		/// <summary>
		/// 全ての要素グループを取得します。
		/// </summary>
		public HtmlElementGroup[] GetAllElementGroups(){
			DataRow[] dr = ElementGroupTable.Select();
			return DataRowsToHtmlElementGroup(dr);
		}

		// Row[] を HtmlElementGroup[] に変換します。
		private HtmlElementGroup[] DataRowsToHtmlElementGroup(DataRow[] rows){
			HtmlElementGroup[] result = new HtmlElementGroup[rows.Length];
			for(int i = 0; i < rows.Length; i++){
				result[i] = GetHtmlElementGroup(rows[i]);
			}
			return result;
		}

		/// <summary>
		/// DataRow から HtmlElementGroup を取得します。
		/// </summary>
		private HtmlElementGroup GetHtmlElementGroup(DataRow row){
			if(row == null) return null;
			return row[ElementGroupTable.DataCol] as HtmlElementGroup;
		}

		/// <summary>
		/// 指定されたIDの要素グループを取得します。
		/// </summary>
		public HtmlElementGroup GetElementGroup(string id){
			return ElementGroupTable.GetData(HtmlElementGroupTable.IdColName, id, ElementGroupTable.DataCol) as HtmlElementGroup;
		}

		/// <summary>
		/// 指定された名前の要素グループを取得します。
		/// </summary>
		public HtmlElementGroup GetElementGroupByName(string name){
			return ElementGroupTable.GetData(HtmlElementGroupTable.NameColName, name, ElementGroupTable.DataCol) as HtmlElementGroup;
		}

		/// <summary>
		/// 全ての属性グループを取得します。
		/// </summary>
		public HtmlAttributeGroup[] GetAllAttributeGroups(){
			DataRow[] dr = AttributeGroupTable.Select();
			return DataRowsToHtmlAttributeGroup(dr);
		}

		// Row[] を HtmlAttributeGroup[] に変換します。
		private HtmlAttributeGroup[] DataRowsToHtmlAttributeGroup(DataRow[] rows){
			HtmlAttributeGroup[] result = new HtmlAttributeGroup[rows.Length];
			for(int i = 0; i < rows.Length; i++){
				result[i] = GetHtmlAttributeGroup(rows[i]);
			}
			return result;
		}

		/// <summary>
		/// DataRow から HtmlAttributeGroup を取得します。
		/// </summary>
		private HtmlAttributeGroup GetHtmlAttributeGroup(DataRow row){
			if(row == null) return null;
			return row[AttributeGroupTable.DataCol] as HtmlAttributeGroup;
		}

		/// <summary>
		/// 指定されたIDの要素を取得します。
		/// </summary>
		public HtmlAttributeGroup GetAttributeGroup(string id){
			return AttributeGroupTable.GetData(HtmlAttributeGroupTable.IdColName, id, AttributeGroupTable.DataCol) as HtmlAttributeGroup;
		}

		/// <summary>
		/// 指定された名前の属性グループを取得します。
		/// </summary>
		public HtmlAttributeGroup GetAttributeGroupByName(string name){
			return AttributeGroupTable.GetData(HtmlAttributeGroupTable.NameColName, name, AttributeGroupTable.DataCol) as HtmlAttributeGroup;
		}




// オーバーライドメソッド

		/// <summary>
		/// データを GET し、HatomaruResponse を取得します。
		/// </summary>
		public override HatomaruResponse Get(AbsPath path){
			HatomaruGetAction act = GetAction(path);
			HatomaruResponse result = act.Get();
			result.SetLastModified();
			return result;
		}

		/// <summary>
		/// path を元に、適切なコントローラを作成します。
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




// Table の初期化
		private void InitTables(){
			DataTable = new HtmlDataformatTable();
			ElementTable = new HtmlElementTable();
			ElementGroupTable = new HtmlElementGroupTable();
			AttributeTable = new HtmlAttributeTable();
			AttributeGroupTable = new HtmlAttributeGroupTable();
		}

		/// <summary>
		/// 指定された XML ファイルからデータをロードします。
		/// この処理はスレッドセーフではありません。
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

			// 属性や親子関係を取得
			Array.ForEach(he, item=>{SetChildren(item);SetAttribute(item);});
			Array.ForEach(heg, item=>{SetChildren(item);});
			Array.ForEach(ha, item=>{SetChildren(item);});
			Array.ForEach(hag, item=>{SetChildren(item);});

		}

		// 要素の子要素を解析して配列に格納します。
		// 排除要素以外の要素に対しては、親として関連づけます。
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
			// include な要素たちに対しては親として関連づける
			includeItems.ForEach(item=>{item.AddParent(he);});
			includeItems.AddRange(SearchChildren(exclude));
			he.Content = includeItems.ToArray();
		}

		// 要素グループの子要素を解析して配列に格納します。
		// 排除要素以外の要素に対しては、親として関連づけます。
		private void SetChildren(HtmlElementGroup heg){
			List<HtmlItem> contents = new List<HtmlItem>();
			//要素を探す
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


		// 要素の子要素を解析して配列に格納します。
		// 排除要素以外の要素に対しては、親として関連づけます。
		private void SetAttribute(HtmlElement he){
			// 属性を探す
			List<HtmlAttribute> attrs = new List<HtmlAttribute>();
			foreach(XmlElement e in he.XmlElement.GetElementsByTagName(AttributeElementName)){
				if(e == null) continue;
				string s = e.GetInnerText();
				HtmlAttribute ha = GetAttribute(s, he);
				attrs.Add(ha);
				ha.AddParent(he);
			}
			he.Attributes = attrs.ToArray();

			// 属性を探す
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


		// 属性値を解析して格納します。
		// 親として関連づけます。
		private void SetChildren(HtmlAttribute ha){
			string valueStr = ha.XmlElement.GetInnerText(ValueElementName);
			HtmlItem item = GetDataByName(valueStr);
			if(item == null) item = new HtmlMisc(valueStr);
			item.AddParent(ha);
			ha.Value = item;
		}


		// 属性値を解析して格納します。
		// 親として関連づけます。
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

		// 子要素を検索します。
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
				// マッチした
				string before = test.Substring(0, m.Index);
				string inner = m.Value;
				string after = test.Substring(m.Index + m.Length);
				if(!string.IsNullOrEmpty(before)) items.Add(new HtmlMisc(before));

				HtmlItem item = null;
				if(inner.StartsWith("%")){
					// 要素グループを検索→見つからなかったら Misc
					item = GetElementGroupByName(inner);
				} else {
					// 要素を検索→データを検索→見つからなかったら Misc
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



