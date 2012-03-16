using System;
using System.Collections.Generic;
using System.Data;
using System.IO;

namespace Bakera.Hatomaru{

/*
 ���X�|���X�� "/" �Ŏn�܂�p�X������̃L�[�ŊǗ�����B
 �f�[�^�� FileInfo.FullName �ŊǗ�����
*/


	/// <summary>
	/// hatomaru.dll �̃��X�|���X�L���b�V�����Ǘ�����N���X�ł��B
	/// </summary>
	public class CacheManager<T> where T : ICacheData{

		// �����L���b�V�� : ���X�|���X�f�[�^�̃L���b�V��
		private readonly Dictionary<string, T> myCache = new Dictionary<string, T>();

// �R���X�g���N�^
		/// <summary>
		/// �L���b�V���}�l�[�W���̃C���X�^���X���쐬���܂��B
		/// </summary>
		public CacheManager(){}


// �p�u���b�N���\�b�h

		/// <summary>
		/// �w�肳�ꂽ�L�[�ɑΉ�����L���b�V���f�[�^������� true ��Ԃ��܂��B
		/// </summary>
		public bool Contains(string key){
			return myCache.ContainsKey(key);
		}
		/// <summary>
		/// �w�肳�ꂽ�L�[�ɑΉ�����L���b�V���f�[�^������� true ��Ԃ��܂��B
		/// </summary>
		public bool Contains(FileInfo f){
			return Contains(f.FullName);
		}

		/// <summary>
		/// �w�肳�ꂽ�L�[�ɑΉ�����L���b�V���f�[�^��T���A�f�[�^��Ԃ��܂��B
		/// �f�[�^���Ȃ���� null ��Ԃ��܂��B
		/// ���̃��\�b�h�ł̓f�[�^���L�����ǂ����̓`�F�b�N���܂���B
		/// �f�[�^�̗L�������`�F�b�N���Ď擾����ɂ� GetEneabledCache ���\�b�h���g�p���܂��B
		/// </summary>
		protected T Get(string key){
			if(!Contains(key)) return default(T);
			return myCache[key];
		}

		/// <summary>
		/// �w�肳�ꂽ�L�[�ɑΉ�����L���b�V���f�[�^��T���A�f�[�^���L���ł���ΕԂ��܂��B
		/// �f�[�^���Ȃ��ꍇ�� null ��Ԃ��܂��B
		/// </summary>
		public virtual T GetEneabledCache(string key){
			T result = Get(key);
			if(result == null) return default(T);
			if(result.IsExpired || !result.IsNewest){
				myCache.Remove(key);
				return default(T);
			}
			return result;
		}

		/// <summary>
		/// �L�[�ƒl���w�肵�ăL���b�V���f�[�^��ǉ����܂��B
		/// �������ݎ��ɂ̓��b�N���s���܂��B���̃��\�b�h�̓X���b�h�Z�[�t�̂���ł��B
		/// </summary>
		public virtual void Add(string key, T value){
			if(!value.IsCacheable) return;
			lock(myCache){
				myCache[key] = value;
			}
		}

		/// <summary>
		/// �L�[���w�肵�ăL���b�V���f�[�^���폜���܂��B
		/// �������ݎ��ɂ̓��b�N���s���܂��B���̃��\�b�h�̓X���b�h�Z�[�t�̂���ł��B
		/// </summary>
		public void Remove(string key){
			lock(myCache){
				myCache.Remove(key);
			}
		}


		/// <summary>
		/// ���ׂẴL���b�V���f�[�^���擾���܂��B
		/// </summary>
		public T[] GetAllData(){
			T[] result = new T[myCache.Count];
			int i = 0;
			foreach(KeyValuePair<string, T> pair in myCache){
				result[i++] = (T)pair.Value;
			}
			return result;
		}


	} // End Class CacheManager

} // End Namespace 
