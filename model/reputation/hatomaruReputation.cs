using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Reflection;
using System.Xml;

namespace Bakera.Hatomaru{

/// <summary>
/// 人気コンテンツを処理するクラスです。
/// </summary>
	public partial class HatomaruReputation : HatomaruXml{

		public new const string Name = "reputation";
		public const string ContentElementName = "content";
		public const string UriAttributeName = "uri";
		public const string CountAttributeName = "count";
		public const string DescriptionElementName = "desc";

		/// <summary>
		/// データソースの FileInfo と XmlDocument を指定して、Reputation のインスタンスを開始します。
		/// </summary>
		public HatomaruReputation(HatomaruManager manager, FileInfo f, XmlDocument x) : base(manager, f, x){
			Table = new ReputationTable(this);
		}

// プロパティ

		public ReputationTable Table{
			get; private set;
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

// メソッド

		/// <summary>
		/// path を元に、適切なコントローラを作成します。
		/// </summary>
		private HatomaruGetAction GetAction(AbsPath path){
			string[] fragments = path.GetFragments(BasePath);
			return new ReputationView(this, path);
		}

// データ取得メソッド

		// Select 文字列を指定して Topic を取得します。
		public ReputationContent[] GetAllReputation(){
			return null;
		}



	} // End class
} // End Namespace Bakera



