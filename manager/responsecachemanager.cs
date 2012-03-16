using System;
using System.Collections.Generic;
using System.Data;
using System.IO;

namespace Bakera.Hatomaru{

/*
 ���X�|���X�̃L���b�V���t�@�C���� �p�X + �u_cache�v
 �L���b�V���t�@�C���� Last-Modified �̊Ǘ��ɕK�v

���V�K�쐬�E�L���b�V���t�@�C�����Ȃ��ꍇ
 ���X�|���X�𐶐��A�ŐV�� DataSource �� Last-Modified
 ���X�|���X���L���b�V���Ɋi�[
 �L���b�V���t�@�C���쐬�A�L���b�V���t�@�C���� LastWriteTime �����X�|���X�� Last-Modified �̒l�ɂ���

���V�K�쐬�E�L���b�V���t�@�C��������ꍇ
 ���X�|���X�𐶐��A�ŐV�� DataSource �� Last-Modified
 ���X�|���X���L���b�V���Ɋi�[
 �L���b�V���t�@�C���Ɠ��e���r
 ���e������Ă���΃L���b�V���t�@�C�����㏑���A�L���b�V���t�@�C���� LastWriteTime ��ݒ肵����

 ���e����v���Ă���΃L���b�V���t�@�C���͂��̂܂܁A���X�|���X�� Last-Modified �̒l���L���b�V���t�@�C���̂��̂ɂ���
 ���̂܂܂��� IsNewest �� false �ɂȂ��Ă��܂��̂ŁACheckedTime �Ɍ��ݎ������L������B

 �ȍ~�AIsNewest �� CheckedTime �� DataSource �̎������r���ACheckedTime �̕����V������� Newest �Ƃ݂Ȃ��B

���L���b�V���̏Ɖ�
 IsNewest = true �Ȃ炻�̃L���b�V����Ԃ�

 IsNewest �̓����?
 DataSource ���ׂĂ��ŐV�ł���� Newest

 �����łȂ��ꍇ��?
 CheckedTime �� DataSource �̎������r����
 CheckedTime �̕����V������� Newest

 �����łȂ��ꍇ��?
 Refresh �����݂�
 ����������u�V�K�쐬�E�L���b�V���t�@�C��������ꍇ�v�̏������s��

*/


	/// <summary>
	/// hatomaru.dll �̃��X�|���X�L���b�V�����Ǘ�����N���X�ł��B
	/// </summary>
	public class ResponseCacheManager : CacheManager<HatomaruResponse>{

		/// <summary>
		/// �L���b�V���}�l�[�W���̃C���X�^���X���쐬���܂��B
		/// </summary>
		public ResponseCacheManager(){}


		/// <summary>
		/// �L�[�ƒl���w�肵�ăL���b�V���f�[�^��ǉ����܂��B
		/// �������ݎ��ɂ̓��b�N���s���܂��B���̃��\�b�h�̓X���b�h�Z�[�t�̂���ł��B
		/// </summary>
		public void Add(AbsPath keyPath, HatomaruResponse hr){
			if(hr.Path == null) hr.Path = keyPath;
			string key = keyPath.ToString();
			base.Add(key, hr);
		}


		public bool Contains(AbsPath keyPath){
			return Contains(keyPath.ToString());
		}


		/// <summary>
		/// �w�肳�ꂽ�L�[�ɑΉ�����L���b�V���f�[�^��T���A�f�[�^���L���ł���ΕԂ��܂��B
		/// �f�[�^���Ȃ��ꍇ�A�Â��ꍇ�� null ��Ԃ��܂��B
		/// </summary>
		public HatomaruResponse GetEneabledCache(AbsPath keyPath){
			return GetEneabledCache(keyPath.ToString());
		}


	} // End Class ResponseCacheManager


} // End Namespace 
