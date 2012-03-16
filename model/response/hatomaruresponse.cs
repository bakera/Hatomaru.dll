using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Web;

namespace Bakera.Hatomaru{

	/// <summary>
	/// ���X�|���X�f�[�^��\�����ۃN���X�ł��B
	/// ���X�|���X�̃L���b�V���ɂ��g�p����܂��B
	/// </summary>
	public abstract class HatomaruResponse : ICacheData{
	
		private string myContentType = XhtmlMediaType;

		protected int myStatusCode = 200;
		protected AbsPath myPath;
		private string myCharset = "UTF-8";
		private bool myDisposition;
		protected DateTime myLastModified;
		protected Xhtml myHtml;

		private string mySelfTitle; // SelfTitle | BaseTitle
		private string myBaseTitle;

		protected readonly HatomaruData myBaseSource;
		private List<HatomaruData> myDataSource = new List<HatomaruData>();
		private List<LinkItem> myTopicPath = new List<LinkItem>();

		public const string XhtmlMediaType = "application/xhtml+xml";
		public const string AtomMediaType = "application/atom+xml";
		public const string RssMediaType = "application/rss+xml";
		public const string HtmlMediaType = "text/html";


// �R���X�g���N�^
		/// <summary>
		/// �����ݒ���g�p���� HatomaruResponse �̃C���X�^���X���J�n���܂��B
		/// </summary>
		protected HatomaruResponse(){}

		/// <summary>
		/// �f�[�^�\�[�X���w�肵�āAHatomaruResponse �̃C���X�^���X���J�n���܂��B
		/// </summary>
		protected HatomaruResponse(HatomaruData source){
			myBaseSource = source;
			myDataSource.Add(source);
		}


// �v���p�e�B
		/// <summary>
		/// �X�e�[�^�X�R�[�h���擾���܂��B
		/// </summary>
		public int StatusCode {
			get{return myStatusCode;}
		}

		/// <summary>
		/// Html��ݒ�E�擾���܂��B
		/// </summary>
		public virtual Xhtml Html{
			get{return myHtml;}
			set{myHtml = value;}
		}

		/// <summary>
		/// ���̃��X�|���X���L���b�V�����ėǂ��������l��ݒ�E�擾���܂��B
		/// �L���b�V�����Ă��ǂ���� true �ƂȂ�APOST �ɑ΂��郌�X�|���X���ł� false �Ƃ��܂��B
		/// ���ۃN���X�̎����ł� false �Ȃ̂ŁAtrue �ɂ���ꍇ�͂��̃v���p�e�B���I�[�o�[���C�h���܂��B
		/// </summary>
		public virtual bool IsCacheable {
			get{return false;}
		}

		/// <summary>
		/// ���X�|���X�̃T�C�Y���擾���܂��B
		/// </summary>
		public virtual long Length{
			get{
				if(myHtml != null) return myHtml.OuterXml.Length;
				return 0;
			}
		}

		/// <summary>
		/// ���X�|���X�̃T�C�Y�� KB/MB �P�ʂŎ擾���܂��B
		/// </summary>
		public virtual string LengthFormat{
			get{
				long size = Length;
				if(size == 1) return "1byte";
				if(size < 1000) return string.Format("{0}bytes", size);
				if(size < 10 * 1000 * 1000) return string.Format("{0:N0}KB", size/1000);
				return string.Format("{0}MB", size/1000000);
			}
		}


		/// <summary>
		/// charset ��ݒ�E�擾���܂��B
		/// </summary>
		public string Charset {
			get{return myCharset;}
			set{myCharset = value;}
		}


		/// <summary>
		/// Content-Disposition �̗L����ݒ�E�擾���܂��B
		/// true �Ȃ� Content-Disposition: attachement �ł��邱�Ƃ������܂��B
		/// </summary>
		public bool Disposition {
			get{return myDisposition;}
			set{myDisposition = value;}
		}


		/// <summary>
		/// �f�[�^�\�[�X�̔z����擾���܂��B
		/// </summary>
		public HatomaruData[] DataSource {
			get{
				HatomaruData[] result = new HatomaruData[myDataSource.Count];
				myDataSource.CopyTo(result);
				return result;
			}
		}

		/// <summary>
		/// ���X�|���X�̃��C���^�C�g�����擾���܂��B
		/// </summary>
		public string SelfTitle{
			get{return mySelfTitle;}
			set{mySelfTitle = value;}
		}

		/// <summary>
		/// ���X�|���X�̐e�̃^�C�g�����擾���܂��B
		/// </summary>
		public string BaseTitle{
			get{return myBaseTitle;}
			set{myBaseTitle = value;}
		}

		/// <summary>
		/// title�v�f�Ŏg�p����^�C�g�����擾���܂��B
		/// </summary>
		public string Title{
			get{
				if(string.IsNullOrEmpty(myBaseTitle)){
					return mySelfTitle;
				}
				return mySelfTitle + " | " + myBaseTitle;
			}
		}

		/// <summary>
		/// �R�����g�c���[���Ŏg�p����t���^�C�g�����擾���܂��B
		/// </summary>
		public string FullTitle{
			get{
				if(string.IsNullOrEmpty(myBaseTitle)){
					return mySelfTitle;
				}
				return myBaseTitle + " : " + mySelfTitle;
			}
		}

		/// <summary>
		/// ���X�|���X�̐e�̃^�C�g�����擾���܂��B
		/// </summary>
		public string Keywords{
			get; set;
		}

		/// <summary>
		/// �p���������X�g���擾���܂��B
		/// </summary>
		public LinkItem[] TopicPath{
			get{return myTopicPath.ToArray();}
		}

		/// <summary>
		/// Content-Type�̒l��ݒ�E�擾���܂��B
		/// </summary>
		public virtual string ContentType{
			get{return myContentType;}
			set{myContentType = value;}
		}


// ICacheData �C���^�[�t�F�C�X�̎���

		/// <summary>
		/// ���̃C���X�^���X�� Last-Modified ���擾���܂��B
		/// ����́A���̃C���X�^���X���������ꂽ�ۂɂ����Ƃ��V���������f�[�^�\�[�X�̎����ł��B
		/// SetLastModified() ���\�b�h�ɂ���ăZ�b�g����܂��B
		/// ���̒l�̓~���b�̃f�[�^���܂݂܂� (If-Modified-Since �Ɣ�r�̍ۂɂ̓~���b��؂藎�Ƃ��K�v������܂�)�B
		/// </summary>
		public DateTime LastModified{
			get{return myLastModified;}
		}

		/// <summary>
		/// ���̃C���X�^���X���ŐV�Ȃ�� true, �Â���� false ��Ԃ��܂��B
		/// ���̃��X�|���X�̃f�[�^�\�[�X���ׂĂ� IsNewest = true �ł���� true ���A�����łȂ���� false ���Ԃ�܂��B
		/// </summary>
		public bool IsNewest{
			get{
				foreach(HatomaruData dat in myDataSource){
					if(!dat.IsNewest) return false;
					dat.File.Refresh();
					if(!dat.File.Exists || dat.File.LastWriteTime > LastModified){
						return false;
					}
				}
				return true;
			}
		}

		/// <summary>
		/// ���̃C���X�^���X���j������Ă���� true, �g�p�ł���Ȃ� false ��Ԃ��܂��B
		/// ����ł�false��Ԃ��܂����A�h���N���X�ł̓L���b�V���f�[�^������ꂽ�ꍇ�� true ��Ԃ��悤�Ɏ������܂��B
		/// </summary>
		public virtual bool IsExpired{
			get{
				return false;
			}
		}


		/// <summary>
		/// ���̃C���X�^���X�̃��t���b�V�������݂܂��B
		/// ��������ΐV�����C���X�^���X���A���s����� null ��Ԃ��܂��B
		/// </summary>
		public HatomaruResponse Refresh(){
			if(IsNewest && !IsExpired) return this;
			if(myBaseSource.IsNewest) return myBaseSource.Get(Path);
			return null;
		}



// �����f�[�^�p�v���p�e�B

		/// <summary>
		/// �u�������v�p�X��ݒ�E�擾���܂��B
		/// ���X�|���X�����t���b�V������ۂɎg�p���܂��B
		/// 301 �����̏ꍇ�A���̃p�X�����ƂɃ��_�C���N�g��� URL ���쐬���܂��B
		/// </summary>
		public virtual AbsPath Path {
			get{return myPath;}
			set{myPath = value;}
		}



// �p�u���b�N���\�b�h


		/// <summary>
		/// TopicPath �� LinkItem ��ǉ����܂��B
		/// </summary>
		public void AddTopicPath(LinkItem item){
			myTopicPath.Add(item);
		}
		/// <summary>
		/// �p�X�ƃe�L�X�g���w�肵�āATopicPath �� LinkItem ��ǉ����܂��B
		/// </summary>
		public void AddTopicPath(AbsPath path, string innerText){
			AddTopicPath(new LinkItem(path, innerText));
		}

		/// <summary>
		/// ���̃��X�|���X�𐶐����邽�߂Ɏg�p�����f�[�^�\�[�X�̃��X�g��ǉ����܂��B
		/// </summary>
		public void AddDataSource(HatomaruData hd){
			if(hd == null) return;
			if(myDataSource.Contains(hd)) return;
			myDataSource.Add(hd);
		}

		/// <summary>
		/// ���̃��X�|���X�𐶐����邽�߂Ɏg�p�����f�[�^�\�[�X�̃��X�g��ǉ����܂��B
		/// </summary>
		public void AddDataSource(HatomaruData[] hd){
			if(hd == null) return;
			for(int i=0; i < hd.Length; i++){
				if(hd[i] == null) continue;
				if(myDataSource.Contains(hd[i])) continue;
				myDataSource.Add(hd[i]);
			}
		}

		/// <summary>
		/// �f�[�^�\�[�X�̒��ł����Ƃ��V�������̂̍ŏI�X�V�������擾���܂��B
		/// �f�[�^�\�[�X���Ȃ��ꍇ�� DateTime.MinValue ��Ԃ��܂��B
		/// </summary>
		public virtual DateTime GetNewestSourceTime(){
			DateTime result = DateTime.MinValue;
			foreach(HatomaruData hd in myDataSource){
				if(hd.LastModified > result) result = hd.LastModified;
			}
			return result;
		}


		/// <summary>
		/// LastModified �������ݒ肵�܂��B
		/// �f�[�^�\�[�X�̒��ł����Ƃ��V�������̂̍ŏI�X�V�������Z�b�g���܂��B
		/// </summary>
		public virtual void SetLastModified(){
			myLastModified = GetNewestSourceTime();
		}


		/// <summary>
		/// �n���ꂽ HttpResponse �Ƀ��X�|���X���������݂܂��B
		/// </summary>
		public virtual void WriteResponse(HttpResponse response){
			WriteResponseHeader(response);
			SetLastModified(response);
		}


// �v���e�N�g���\�b�h

		/// <summary>
		/// �n���ꂽ HttpResponse �Ɋ�{�̃��X�|���X�w�b�_���������݂܂��B
		/// </summary>
		protected virtual void WriteResponseHeader(HttpResponse response){
			response.StatusCode = StatusCode;
			response.ContentType = myContentType;
			response.Charset = Charset;
		}

		/// <summary>
		/// �n���ꂽ HttpResponse �� Last-Modified ���Z�b�g���܂��B
		/// </summary>
		protected virtual void SetLastModified(HttpResponse response){
			if(LastModified == default(DateTime)) return;
			try {
				response.Cache.SetLastModified(LastModified);
				response.Cache.SetCacheability(HttpCacheability.Public);
			} catch{
				response.AppendHeader("X-Last-Modified", LastModified.ToString());
			}
		}


		/// <summary>
		/// ���X�|���X�{�f�B�p�̋�� Xhtml �𓾂܂��B
		/// </summary>
		protected Xhtml GetXhtml(){
			Xhtml result = new Xhtml();
			result.Html.SetAttribute("xml:lang", "ja");
			return result;
		}



	}

}
