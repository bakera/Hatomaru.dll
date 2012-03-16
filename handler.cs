using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Configuration;
using System.Xml;

namespace Bakera.Hatomaru{
	
/*
	���̃N���X�̓��상��

��{�@�\
	Manager �� Request ��n��
	Manager ���� ResponseData ���󂯎���� Response ��Ԃ�
	Request �f�[�^�𕪐͂���̂� Manager �̎d���B

���O�Ƃ�@�\
	��O���������Ă�����L���b�`���ċL�^����B
	Post ����Ă�����L�^����B

���N�G�X�g�p�X��HatomaruPage�ɓn��
HatomaruPage�̓��X�|���X�𐶐����^��URL��Ԃ�
�^��URL�ŃL���b�V������
���N�G�X�gURL�ƈ�����烊�_�C���N�g���L���b�V��

�@����: ���̃C���X�^���X�͍ė��p�\�Ȃ̂ŁAcontext ���L���b�V�������肵�Ȃ�����
*/


	public class Handler : IHttpHandler {

		private const string IniXmlPath = "HatomaruIniXml";
		private HatomaruManager myManager = null;

		private const int RetryTime = 10;
		private const int RetryInitInterval = 5000;

		private const string ErrorPathPrefix = "/error?aspxerrorpath=";
		private const string ErrorPathChars = "\"";

// �p�u���b�N�R���X�g���N�^

		// ���̃C���X�^���X�͍ė��p�\
		public bool IsReusable {
			get{return true;}
		}


// �p�u���b�N���\�b�h
		public void ProcessRequest(HttpContext context){

			if(myManager == null || myManager.IsOld){
				string iniFilePath = WebConfigurationManager.AppSettings[IniXmlPath];
				if(!File.Exists(iniFilePath)){
					throw new Exception("�����ݒ�t�@�C����������܂��� :" + iniFilePath);
				}
				myManager = new HatomaruManager(iniFilePath);
			}

			// robots.txt �� 204 ��Ԃ�
			if(context.Request.RawUrl.EndsWith("/robots.txt")){
				context.Response.StatusCode = 204;
				return;
			}

			// �����J�n
			myManager.Log.Clear();
			try{
				SendResponse(context);
			} catch(HatomaruXmlException e){
				string errorLog = GetError(e, context.Request);
				try{
					ThreadPool.QueueUserWorkItem(new WaitCallback(SaveLog), errorLog);
				} catch {
					// ���O�ɕۑ��ł��Ȃ���O�͂��̂܂܃X���[
					throw e;
				}
				SendServiceUnabailableResponse(context, errorLog);
			} catch(Exception e){
				string errorLog = GetError(e, context.Request);
				try{
					ThreadPool.QueueUserWorkItem(new WaitCallback(SaveLog), errorLog);
				} catch {
					// ���O�ɕۑ��ł��Ȃ���O�͂��̂܂܃X���[
					throw e;
				}
				SendInternalServerErrorResponse(context, errorLog);
			}
		}



// �v���C�x�[�g���\�b�h

		// �G���[���O���擾���܂��B
		private string GetError(Exception e, HttpRequest req){
			StringBuilder error = new StringBuilder();
			error.AppendLine(DateTime.Now.ToString());
			error.AppendLine(req.UserHostAddress);
			error.AppendLine(req.HttpMethod + " " + req.RawUrl);
			foreach(string key in req.Headers.Keys){
				error.AppendLine(key + ": " + req.Headers[key]);
			}
			error.AppendLine(e.ToString());
			if(myManager != null && myManager.Log != null){
				error.AppendLine(myManager.Log.ToString());
			}
			return error.ToString();
		}


		// ���O�t�@�C���ɕۑ����܂��B
		private void SaveLog(Object o){
			for(int i = 0; i < RetryTime; i++){
				try{
					if(myManager == null) return;
					string dateName = DateTime.Now.ToString("yyyy-MM-dd") + ".txt";
					FileInfo logfile = new FileInfo(myManager.IniData.LogPath.FullName + '\\' + dateName);
					using(FileStream fs = logfile.Open(FileMode.Append, FileAccess.Write, FileShare.None)){
						using(StreamWriter sw = new StreamWriter(fs)){
							sw.WriteLine(o.ToString());
						}
					}
					break;
				} catch(IOException){
					Thread.Sleep(RetryInitInterval + RetryInitInterval * i);
				}
			}
		}


		// ���X�|���X���擾���A�����o���܂��B
		private void SendResponse(HttpContext context){
			HatomaruResponse hr = myManager.GetResponse(context.Request);
			if(hr == null) throw new Exception("���X�|���X���擾�ł��܂���ł����B");

			// Host:���Ԉ���Ă���200�n�������烊�_�C���N�g
			// ������ localhost �͏���
			if(200 <= hr.StatusCode && hr.StatusCode < 300){
				string hostValue = context.Request.Headers["Host"];
				if(!string.IsNullOrEmpty(hostValue)){
					if(!hostValue.Eq(myManager.IniData.Domain) && !hostValue.Eq("localhost")){
						hr = new RedirectResponse(hr.Path, myManager.IniData.Domain);
					}
				}
			}

			// IMS ���̃��N�G�X�g�� 304 ��Ԃ���?
			string ims = context.Request.Headers["If-Modified-Since"];
			if(ims != null && hr.LastModified != null){
				DateTime imsTime = GetImsTime(ims);
				DateTime responseTime = CutMilliSeconds(hr.LastModified);
				
				if(imsTime >= responseTime && responseTime != default(DateTime)){
					SendNotModifiedResponse(context);
					return;
				}
			}
			try{
				hr.WriteResponse(context.Response);
				if(hr.ContentType == HatomaruResponse.XhtmlMediaType){
					if(IsXhtmlAccept(context.Request)){
						context.Response.Cache.VaryByHeaders.AcceptTypes = true;
					} else {
						context.Response.ContentType = HatomaruResponse.HtmlMediaType;
					}
				}
				return;
			}catch(System.IO.FileNotFoundException){
				hr = null;
			}
		}

		// application/xhtml+xml ��Ԃ��ėǂ����ǂ����𔻒f���܂��B
		private bool IsXhtmlAccept(HttpRequest r){
			if(r.AcceptTypes == null) return false;
			if(r.AcceptTypes.Length == 0) return false;
			foreach(string s in r.AcceptTypes){
				if(s.Eq(HatomaruResponse.XhtmlMediaType)){
					return true;
				}
			}
			return false;
		}


		// 304������Ԃ��܂��B
		private void SendNotModifiedResponse(HttpContext context){
			context.Response.StatusCode = 304;
			return;
		}

		// 204������Ԃ��܂��B
		private void SendNoContentResponse(HttpContext context){
			context.Response.StatusCode = 204;
			return;
		}

		// �G���[���O���w�肵��503������Ԃ��܂��B
		private void SendServiceUnabailableResponse(HttpContext context, string errorLog){
			context.Response.StatusCode = 503;
			context.Response.Write("<!DOCTYPE HTML PUBLIC \"-//W3C//DTD HTML 4.01//EN\">");
			context.Response.Write("<html lang=\"ja\">");
			context.Response.Write("<title>Service Temporarily Unavailable | ");
			context.Response.Write(context.Server.HtmlEncode(myManager.IniData.Domain));
			context.Response.Write("</title>");
			context.Response.Write("<h1>�T�[�r�X�ꎞ��~��</h1>");
			context.Response.Write("<p>���݁A�T�[�r�X��~���ł��B�f�[�^���X�V���Ă���Œ����A���邢�̓����e�i���X���̂��ߏ������ꎞ�I�ɒ�~���Ă���\��������܂��B</p><p>���̂�����������Ǝv���܂��̂ŁA���΂炭�҂��Ă���܂��A�N�Z�X���Ă݂Ă��������B</p>");
			context.Response.Write("<h2>�����󋵂̏ڍ�</h2>");
			context.Response.Write("<pre>");
			context.Response.Write(context.Server.HtmlEncode(errorLog));
			context.Response.Write("</pre>");
			return;
		}

		// �G���[���O���w�肵��500������Ԃ��܂��B
		private void SendInternalServerErrorResponse(HttpContext context, string errorLog){
			context.Response.StatusCode = 500;
			context.Response.ContentType = HatomaruResponse.HtmlMediaType;
			context.Response.Write("<!DOCTYPE HTML PUBLIC \"-//W3C//DTD HTML 4.01//EN\">");
			context.Response.Write("<title>�G���[</title>");
			context.Response.Write("<h1>�G���[</h1>");
			context.Response.Write("<p>�\�z�O�̃G���[�����������悤�ł��B�T�[�o�����ŉ����c�O�Ȃ��Ƃ��N���Ă��܂��c�c�B</p>");
			context.Response.Write("<pre>");
			context.Response.Write(context.Server.HtmlEncode(errorLog));
			context.Response.Write("</pre>");
			return;
		}


		private static DateTime GetImsTime(string imsStr){
			int index = imsStr.IndexOf(';');
			if(index > -1) imsStr = imsStr.Remove(index);
			DateTime result;
			if(DateTime.TryParse(imsStr, out result)) return result;
			return DateTime.MinValue;
		}

		private static DateTime CutMilliSeconds(DateTime sourceTime){
			return new DateTime(sourceTime.Year, sourceTime.Month, sourceTime.Day, sourceTime.Hour, sourceTime.Minute, sourceTime.Second);
		}




	} // End Class

} // End Namespace

