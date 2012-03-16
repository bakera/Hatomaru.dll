using System;
using System.IO;

namespace Bakera.Hatomaru{

/*
 �L���b�V���f�[�^�ɕK�v�ȋ@�\ : ���̃L���b�V�����L�����ǂ��������� bool�l

���X�V��������ꍇ
A: ���̃L���b�V���̐�������
B: ���̃L���b�V���̍\���f�[�^�̍ŏI�X�V��
 ���r���āAA < B �Ȃ炱�̃L���b�V���͖����ƂȂ�B
 ���[�J���f�[�^�̃L���b�V���̏ꍇ�͂��̔�r���s���B

���L���b�V���� TTL ������ꍇ
A: ���̃L���b�V���̐�������
B: ���̃L���b�V���� TTL
C: ���ݎ���
 ���擾���āAA+B > C �ł���΃L���b�V���͖����ƂȂ�B
 Web �T�[�r�X�̃L���b�V���͂�����B

*/

/*
�@���X�|���X���ŐV���ǂ���?
�@���f�[�^�\�[�X���ׂĂ��ŐV���ǂ���?
�@�ŐV�łȂ��I�u�W�F�N�g�� IsNewest �� false ��Ԃ��B

�@���X�|���X�� LastModified ��?
�@�� *�������_��* �f�[�^�\�[�X���ׂĂ� LastModified �̒��ł����Ƃ��V��������
�@�L���b�V������O�ɂ�����o���Ă����B

*/

	/// <summary>
	/// CacehManager �ŏ������邱�Ƃ��ł���L���b�V���f�[�^�����C���^�[�t�F�C�X���`���܂��B
	/// </summary>
	public interface ICacheData{

		/// <summary>
		/// ���̃f�[�^���L���b�V�����ėǂ��������l��ݒ�E�擾���܂��B
		/// �L���b�V�����Ă��ǂ���� true �ƂȂ�܂��B
		/// </summary>
		bool IsCacheable { get; }

		/// <summary>
		/// ���̃L���b�V���̐����������擾���܂��B
		/// </summary>
		DateTime LastModified{ get; }

		/// <summary>
		/// ���̃L���b�V�����ŐV�̃f�[�^�ƈ�v���Ă��邩�ǂ������f���A�ŐV�ł���� ture ��Ԃ��܂��B
		/// </summary>
		bool IsNewest{ get; }

		/// <summary>
		/// ���̃L���b�V�����j������Ă��邩�ǂ������f���A�j������Ă���� ture ��Ԃ��܂��B
		/// </summary>
		bool IsExpired{ get; }


	} // End class HatomaruData
} // End Namespace Bakera







