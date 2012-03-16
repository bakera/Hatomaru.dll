using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml;

namespace Bakera.Hatomaru{

/// <summary>
/// Amazon検索を処理するクラスです。
/// </summary>
	public partial class AmazonSearch : HatomaruXml{

		public new const string Name = "amazonsearch"; // ルート要素の名前
		public const string TopicName = "topic";
		public const string QueryName = "q";
		public const string IndexTypeName = "i";
		public const string PageName = "p";

		public const string AmazonFormOptionsName = "amazonformoptions";
		public const string OptionName = "option";
		public const string ValueAttributeName = "value";

// コンストラクタ

		/// <summary>
		/// 鳩丸データのデータソースの FileInfo と XmlDocument を指定して、インスタンスを開始します。
		/// </summary>
		public AmazonSearch(HatomaruManager manager, FileInfo f, XmlDocument x) : base(manager, f, x){
		}

// プロパティ


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
			string pathStr = path.ToString();
			int qpos = pathStr.IndexOf('?');
			if(qpos > 0){
				path = new AbsPath(pathStr.Substring(0,qpos));
				string query = pathStr.Substring(qpos+1);
				string q = null;
				string i = null;
				string p = null;
				foreach(string pair in query.Split('&', ';')){
					int eqpos = pair.IndexOf('=');
					if(eqpos < 0) continue;
					string name = pair.Substring(0,eqpos);
					string val = pair.Substring(eqpos+1);
					switch(name){
						case QueryName:
						q = val;
						break;
						case IndexTypeName:
						i = val;
						break;
						case PageName:
						p = val;
						break;
					}
				}
				if(!string.IsNullOrEmpty(q)){
					AmazonIndexType index = AmazonIndexType.None;
					if(!string.IsNullOrEmpty(i)) index = (AmazonIndexType)Enum.Parse(typeof(AmazonIndexType), i, true);
					return new AmazonDoSearch(this, path, q.UrlDecode(), index, p.ToInt32());
				}
			}
			return new AmazonViewSearchForm(this, path);
		}


	} // End class
} // End Namespace Bakera



