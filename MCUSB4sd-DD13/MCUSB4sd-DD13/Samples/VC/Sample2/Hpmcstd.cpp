/*----------------------------------------------------------------------------*/
//
//	Hpmcstd.c
//
//	Copyright 2003 HI-P Tech Corporation
//
/*----------------------------------------------------------------------------*/
//	Change history
//
//		V1.00   2003-05-10  New creation.
//		V1.01   2011-05-01  Change define and global variable.
/*----------------------------------------------------------------------------*/
#include <windows.h>
#include <tchar.h>
#include <conio.h>
#include "Hpmcstd.h"

// define
#define _FALSE			(HANDLE)-1
#define MCUSB4sd_AXIS1	0
#define MCUSB4sd_AXIS2	1
#define MCUSB4sd_AXIS3	2
#define MCUSB4sd_AXIS4	3

// Global variable.
const CHAR	szTitle[]	 = "Sample Program";
const CHAR	szBoadName[] = "MCUSB4sd";

// Prototype of a function.
static void ResultMessage(LPCTSTR lpszbuf);
static void ErrorMessage(DWORD dwErrorCode);
static BOOL Mcusb4sdSetupAxis(HANDLE hDevice,WORD wAxis);
static BOOL Mcusb4sdStatusPrint(HANDLE hDevice,WORD wAxis);

/*-----------------------------------------------------------------*/
//�@1.���{�ꖼ
//�@�@�T���v���v���O�������C���֐��B
//�@2.�p�����^����
//�@�@hInstance�F�A�v���P�[�V�����̌��݂̃C���X�^���X�̃n���h��
//�@�@hPrevInstance�F�A�v���P�[�V�����̑O�̃C���X�^���X�̃n���h���iWin32 �A�v���P�[�V�����ł́A��� NULL �j
//�@�@lpCmdLine�F�R�}���h���C�����i�[���ꂽ�ANULL �ŏI��镶����ւ̃|�C���^
//�@�@nCmdShow�F�E�B���h�E�̕\����Ԃ̎w��
//�@3.�T�v
//�@�@�c�k�k�֐����g�p����������̎��s��������܂��B
//�@4.�@�\����
//�@�@�f�B�b�v�X�C�b�`�O�̃{�[�h���I�[�v�����āA��ꎲ�Ƒ�O�����h���C�u���܂��B
//�@�@�h���C�u�I����ɁA�e������擾���ĕ\�����܂��B
//�@5.�߂�l
//�@�@�֐��� WM_QUIT ���b�Z�[�W���󂯎���Đ���ɏI������ꍇ�́A
//�@�@���b�Z�[�W�� wParam �p�����[�^�Ɋi�[����Ă���I���R�[�h��Ԃ��Ă��������B
//�@�@�֐������b�Z�[�W���[�v�ɓ���O�ɏI������ꍇ�́A0 ��Ԃ��Ă��������B
/*-----------------------------------------------------------------*/
int WINAPI WinMain( HINSTANCE hInstance, HINSTANCE hPrevInstance,
										LPSTR lpCmdLine, int nCmdShow )
{

	HANDLE			hDevice;
	DWORD			dwRet;
	TCHAR			szBuf[1024];
	
	///////////////////////////////////////////////////////////////////////////
	// �{�[�h���I�[�v�����܂��B��Ƃ��ăX�C�b�`�ԍ��O�̃{�[�h���I�[�v�����܂��B
	///////////////////////////////////////////////////////////////////////////
	hDevice = McsdOpen( szBoadName, 0 );
	if ( _FALSE == hDevice ) {
		MessageBox( NULL, "�{�[�h�̃I�[�v���Ɏ��s���܂���", szTitle, MB_OK | MB_ICONSTOP );
		return 1;
	}
	wsprintf( szBuf, _T("%s\n")
		_T("Device Open\n"), szBoadName );
	ResultMessage( szBuf );
	///////////////////////////////////////////////////////////////////////////
	// ��P���̐ݒ���s���܂��B
	///////////////////////////////////////////////////////////////////////////
	if ( FALSE == Mcusb4sdSetupAxis( hDevice, MCUSB4sd_AXIS1 ))
		return 1;
	///////////////////////////////////////////////////////////////////////////
	// ��P�����{������ 15000 �p���X�h���C�u���܂��B
	///////////////////////////////////////////////////////////////////////////
	dwRet = McsdDataWrite( hDevice, MCUSB4sd_AXIS1, MCSD_PLUS_INDEX_PULSE_DRIVE, 15000 );
	if ( dwRet ) {
		ErrorMessage( dwRet );
		return 1;
	}
	wsprintf( szBuf,
		_T("%s\n")
		_T("���䎲 %d �{������ 15000 �p���X�o�͂��Ă��܂�"), szBoadName, MCUSB4sd_AXIS1 );
	ResultMessage( szBuf );
	///////////////////////////////////////////////////////////////////////////
	// ��P�������_���o�h���C�u�P�ɂ�茴�_���A���܂��B
	///////////////////////////////////////////////////////////////////////////
/* ���_���o�����́A�Z���T�[���͂��ω����Ȃ���Ԃł͒�~�������܂���B�K�v�ɉ����Ď��s���Ă��������B
	dwRet = McsdOrgReturn( hDevice, MCUSB4sd_AXIS1, 1);
	if ( dwRet ) {
		ErrorMessage( dwRet );
		return 1;
	}
	wsprintf( szBuf,
		_T("%s\n")
		_T("���䎲 %d ���_���A�̎��s���ł�"), szBoadName, MCUSB4sd_AXIS1 );
	ResultMessage( szBuf );
*/
	///////////////////////////////////////////////////////////////////////////
	// ��P���̓����J�E���^�Ɗe���Ԃ��擾���ĕ\�����܂��B
	///////////////////////////////////////////////////////////////////////////
	if ( FALSE == Mcusb4sdStatusPrint( hDevice, MCUSB4sd_AXIS1 ))
		return 1;
	///////////////////////////////////////////////////////////////////////////
	// �{�[�h���N���[�Y���܂��B
	///////////////////////////////////////////////////////////////////////////
	dwRet = McsdClose( hDevice );
	if ( dwRet ) {
		ErrorMessage( dwRet );
		return 1;
	}
	wsprintf( szBuf, _T("%s\n")
		_T("Device Close\n"), szBoadName );
	ResultMessage( szBuf );

	return 0;
}

/*-----------------------------------------------------------------*/
//�@1.���{�ꖼ
//�@�@�������ʂ�\���B
//�@2.�p�����^����
//�@�@lpszbuf�F�\�����镶�͂��i�[����NULL�ŏI��镶����̃|�C���^
//�@3.�T�v
//�@�@�������ʂ����b�Z�[�W�{�b�N�X�ŕ\�����܂��B
//�@4.�@�\����
//�@�@�������ʂ��i�[���ꂽ�w��̕���������b�Z�[�W�{�b�N�X�ŕ\�����܂��B
//�@5.�߂�l
//�@�@�Ȃ��B
/*-----------------------------------------------------------------*/
static void ResultMessage(LPCTSTR lpszbuf)
{
	MessageBox(NULL, lpszbuf, szTitle, MB_OK | MB_ICONINFORMATION);
}

/*-----------------------------------------------------------------*/
//�@1.���{�ꖼ
//�@�@�G���[���b�Z�[�W��\���B
//�@2.�p�����^����
//�@�@dwErrorCode�F�G���[�R�[�h
//�@3.�T�v
//�@�@�G���[�R�[�h�����b�Z�[�W�{�b�N�X�ŕ\�����܂��B
//�@4.�@�\����
//�@�@�G���[�R�[�h�����b�Z�[�W�{�b�N�X�ŕ\�����܂��B
//�@5.�߂�l
//�@�@�Ȃ��B
/*-----------------------------------------------------------------*/
static void ErrorMessage(DWORD dwErrorCode)
{
	TCHAR szbuf[50];

	wsprintf(szbuf, _T("%s error\n\n")
					_T("Error code: %08lXh"), szBoadName, dwErrorCode);
	MessageBox(NULL, szbuf, szTitle, MB_OK | MB_ICONSTOP);
}

/*-----------------------------------------------------------------*/
//�@1.���{�ꖼ
//�@�@�{�[�h�̊e�����\���B
//�@2.�p�����^����
//�@�@hDevice�F�f�o�C�X�n���h��
//�@�@wAxis�F���䎲
//�@3.�T�v
//�@�@�����Ŏw�肳�ꂽ�f�o�C�X�̐��䎲�̊e��������b�Z�[�W�{�b�N�X�ŏ��Ԃɕ\�����܂��B
//�@4.�@�\����
//�@�@�����J�E���^�[�A�G���h�X�e�[�^�X�A���J�j�J���V�O�i���A���j�o�[�T���V�O�i�����擾���ĕ\�����܂��B
//�@5.�߂�l
//�@�@TRUE�F����I��
//�@�@TRUE�ȊO�F�c�k�k�֐����s���ɔ��������G���[�R�[�h
/*-----------------------------------------------------------------*/
static BOOL Mcusb4sdStatusPrint( HANDLE hDevice, WORD wAxis )
{

	DWORD			dwRet;
	BYTE			bData;
	DWORD			dwCount;
	WORD			wPortBase;
	TCHAR			szBuf[1024];

	///////////////////////////////////////////////////////////////////////////
	// MCUSB4sd �� 1 ���� 8 �|�[�g�A�h���X���L���܂��B
	///////////////////////////////////////////////////////////////////////////
	wPortBase = wAxis * 8;
	///////////////////////////////////////////////////////////////////////////
	// �����J�E���^�[��\�����܂��B
	///////////////////////////////////////////////////////////////////////////
	dwRet = McsdDataRead( hDevice, wAxis, MCSD_INTERNAL_COUNTER_READ, &dwCount );
	if ( dwRet ) {
		ErrorMessage( dwRet );
		return FALSE;
	}
	// �����J�E���^�� 28 �r�b�g�Ȃ̂ŕ����g�����܂��B
	if ( dwCount & 0x08000000 ) dwCount |= 0xF0000000;
	wsprintf( szBuf,
		_T("%s\n")
		_T("���䎲 %d �����J�E���^�[: %d"), szBoadName, wAxis, dwCount);
	ResultMessage( szBuf );
	///////////////////////////////////////////////////////////////////////////
	// �O���J�E���^�[��\�����܂��B
	///////////////////////////////////////////////////////////////////////////
	dwRet = McsdDataRead( hDevice, wAxis, MCSD_EXTERNAL_COUNTER_READ, &dwCount );
	if ( dwRet ) {
		ErrorMessage( dwRet );
		return FALSE;
	}
	// �O���J�E���^�� 28 �r�b�g�Ȃ̂ŕ����g�����܂��B
	if ( dwCount & 0x08000000 ) dwCount |= 0xF0000000;
	wsprintf( szBuf,
		_T("%s\n")
		_T("���䎲 %d �O���J�E���^�[: %d"), szBoadName, wAxis, dwCount);
	ResultMessage( szBuf );
	///////////////////////////////////////////////////////////////////////////
	// �G���h�X�e�[�^�X���擾���ĕ\�����܂��B
	///////////////////////////////////////////////////////////////////////////
	dwRet = McsdInP( hDevice, (WORD)(wPortBase + MCSD_PORT_END_STATUS), &bData );
	if ( dwRet ) {
		ErrorMessage( dwRet );
		return FALSE;
	}
	wsprintf( szBuf,
		_T("%s\n")
		_T("���䎲 %d �G���h�X�e�[�^�X: %Xh"), szBoadName, wAxis, bData);
	ResultMessage( szBuf );
	///////////////////////////////////////////////////////////////////////////
	// ���J�j�J���V�O�i�����擾���ĕ\�����܂��B
	///////////////////////////////////////////////////////////////////////////
	dwRet = McsdInP( hDevice, (WORD)(wPortBase + MCSD_PORT_MECHANICAL_SIGNAL), &bData );
	if ( dwRet ) {
		ErrorMessage( dwRet );
		return FALSE;
	}
	wsprintf( szBuf,
		_T("%s\n")
		_T("���䎲 %d ���J�j�J���V�O�i��: %Xh"), szBoadName, wAxis, bData);
	ResultMessage( szBuf );
	///////////////////////////////////////////////////////////////////////////
	// ���j�o�[�T���V�O�i�����擾���ĕ\�����܂��B
	///////////////////////////////////////////////////////////////////////////
	dwRet = McsdInP( hDevice, (WORD)(wPortBase + MCSD_PORT_UNIVERSAL_SIGNAL), &bData );
	if ( dwRet ) {
		ErrorMessage( dwRet );
		return FALSE;
	}
	wsprintf( szBuf,
		_T("%s\n")
		_T("���䎲 %d ���j�o�[�T���V�O�i��: %Xh"), szBoadName, wAxis, bData);
	ResultMessage( szBuf );
	///////////////////////////////////////////////////////////////////////////
	// �h���C�u���ł���Ό�����~���܂��B
	///////////////////////////////////////////////////////////////////////////
	dwRet = McsdInP( hDevice, (WORD)(wPortBase + MCSD_PORT_DRIVE_STATUS), &bData );
	if ( dwRet ) {
		ErrorMessage( dwRet );
		return FALSE;
	}
	if ( bData & 0x01 ) {
		dwRet = McsdOutP( hDevice, (WORD)(wPortBase + MCSD_PORT_COMMAND), MCSD_SLOW_DOWN_STOP);
		if ( dwRet ) {
			ErrorMessage( dwRet );
			return FALSE;
		}
	}

	return TRUE;
}

/*-----------------------------------------------------------------*/
//�@1.���{�ꖼ
//�@�@���䎲�̊e��ݒ�B
//�@2.�p�����^����
//�@�@hDevice�F�f�o�C�X�n���h��
//�@�@wAxis�F���䎲
//�@3.�T�v
//�@�@�����Ŏw�肳�ꂽ�f�o�C�X�̐��䎲�̐ݒ���s���܂��B
//�@4.�@�\����
//�@�@���L�̐ݒ���s���܂��B
//�@�@�@MODE-A
//�@�@�@MODE-B
//�@�@�@INPOSITION WAIT MODE RESET
//�@�@�@ALARM STOP ENABLE MODE SET
//�@�@�@RANGE DATA
//�@�@�@START/STOP SPEED DATA
//�@�@�@OBJECT SPEED DATA
//�@�@�@RATE-A DATA
//�@�@�@RATE-B DATA
//�@�@�@RATE-C DATA
//�@�@�@RATE CHANGE POINT A-B
//�@�@�@RATE CHANGE POINT B-C
//�@�@�@�J�E���^�ݒ�
//�@5.�߂�l
//�@�@TRUE�F����I��
//�@�@TRUE�ȊO�F�c�k�k�֐����s���ɔ��������G���[�R�[�h
/*-----------------------------------------------------------------*/
static BOOL Mcusb4sdSetupAxis( HANDLE hDevice, WORD wAxis )
{

	DWORD			dwRet;
	WORD			wPortBase;

	///////////////////////////////////////////////////////////////////////////
	// MCUSB4sd �� 1 ���� 8 �|�[�g�A�h���X���L���܂��B
	///////////////////////////////////////////////////////////////////////////
	wPortBase = wAxis * 8;
	///////////////////////////////////////////////////////////////////////////
	// �֐��̃o�b�t�@�����O���J�n���܂��B
	///////////////////////////////////////////////////////////////////////////
	dwRet = McsdStartBuffer( hDevice, 16 );
	if ( dwRet ) {
		ErrorMessage( dwRet );
		return FALSE;
	}
	///////////////////////////////////////////////////////////////////////////
	// MODE-A SET
	//
	// �����J�n�|�C���g���o����				����
	// �p���X�o�͕���						�Q�p���X����
	// DIR   �o�͒[�q 						CW�p���X  �A�N�e�B�u Hi
	// PULSE �o�͒[�q						CCW�p���X �A�N�e�B�u Hi
	///////////////////////////////////////////////////////////////////////////
	dwRet = McsdOutP( hDevice, (WORD)(wPortBase + MCSD_PORT_MODE_A), 0x40 );
	if ( dwRet ) {
		ErrorMessage( dwRet );
		return FALSE;
	}
	///////////////////////////////////////////////////////////////////////////
	// MODE-B SET
	//
	// EXTERNAL COUNTER ���͎d�l			�Q���M���S���{����
	// DEND ���͐M���A�N�e�B�u���x��		Low
	// DERR ���͐M���A�N�e�B�u���x��		Low
	// -SLM ���͐M���A�N�e�B�u���x��		Low
	// +SLM ���͐M���A�N�e�B�u���x��		Low
	// -ELM ���͐M���A�N�e�B�u���x��		Low
	// +ELM ���͐M���A�N�e�B�u���x��		Low
	///////////////////////////////////////////////////////////////////////////
	dwRet = McsdOutP( hDevice, (WORD)(wPortBase + MCSD_PORT_MODE_B), 0xFF );
	if ( dwRet ) {
		ErrorMessage( dwRet );
		return FALSE;
	}
	///////////////////////////////////////////////////////////////////////////
	// ���[�h�ݒ�
	//
	// INPOSITION WAIT MODE RESET
	// ALARM STOP ENABLE MODE SET
	///////////////////////////////////////////////////////////////////////////
	dwRet = McsdOutP( hDevice, (WORD)(wPortBase + MCSD_PORT_COMMAND), MCSD_INPOSITION_WAIT_MODE_RESET );
	if ( dwRet ) {
		ErrorMessage( dwRet );
		return FALSE;
	}
	dwRet = McsdOutP( hDevice, (WORD)(wPortBase + MCSD_PORT_COMMAND), MCSD_ALARM_STOP_ENABLE_MODE_SET );
	if ( dwRet ) {
		ErrorMessage( dwRet );
		return FALSE;
	}
	///////////////////////////////////////////////////////////////////////////
	// �f�[�^�ݒ�
	//
	// RANGE DATA				250		�o�͎��g���ݒ�P�� 	500��250=2PPS
	// LOW SPEED DATA			500		���N�����x	 		500�~2PPS=1000PPS
	// HIGH SPEED DATA			3000	�ō����x 			3000�~2PPS=6000PPS
	// RATE-A DATA              1024	�������Ԑݒ�P�� 	1024��(4.096�~10^6)=0.25mSec
	//									�������� 			(3000-500)�~0.25mSec = 625mSec
	// RATE-B DATA        				�f�t�H���g�l 8191(1FFFh)
	// RATE-C DATA						�f�t�H���g�l 8191(1FFFh)
	// RATE CHANGE POINT A-B			�f�t�H���g�l 8191(1FFFh)
	// RATE CHANGE POINT B-C			�f�t�H���g�l 8191(1FFFh)
	//
	// ���̐ݒ�ɂ�� RATE-A DATA �ɂ�钼���������ƂȂ�܂�
	///////////////////////////////////////////////////////////////////////////
	dwRet = McsdDataWrite( hDevice, wAxis, MCSD_RANGE_DATA_WRITE, 250 );
	if ( dwRet ) {
		ErrorMessage( dwRet );
		return FALSE;
	}
	dwRet = McsdDataWrite( hDevice, wAxis, MCSD_LOW_SPEED_DATA_WRITE, 500 );
	if ( dwRet ) {
		ErrorMessage( dwRet );
		return FALSE;
	}
	dwRet = McsdDataWrite( hDevice, wAxis, MCSD_HIGH_SPEED_DATA_WRITE, 3000 );
	if ( dwRet ) {
		ErrorMessage( dwRet );
		return FALSE;
	}
	dwRet = McsdDataWrite( hDevice, wAxis, MCSD_RATE_A_DATA_WRITE, 1024 );
	if ( dwRet ) {
		ErrorMessage( dwRet );
		return FALSE;
	}
	///////////////////////////////////////////////////////////////////////////
	// �J�E���^�ݒ�
	//
	// INTERNAL COUNTER �y�� EXTERNAL COUNTER �� 0h ���������݂܂�
	///////////////////////////////////////////////////////////////////////////
	dwRet = McsdDataWrite( hDevice, wAxis, MCSD_INTERNAL_COUNTER_WRITE , 0 );
	if ( dwRet ) {
		ErrorMessage( dwRet );
		return FALSE;
	}
	dwRet = McsdDataWrite( hDevice, wAxis, MCSD_EXTERNAL_COUNTER_WRITE, 0 );
	if ( dwRet ) {
		ErrorMessage( dwRet );
		return FALSE;
	}
	///////////////////////////////////////////////////////////////////////////
	// �R���p���[�^�ݒ�
	//
	// INTERNAL COMPARATER �y�� EXTERNAL COMPARATER �� 0h ���������݂܂�
	///////////////////////////////////////////////////////////////////////////
	dwRet = McsdDataWrite( hDevice, wAxis, MCSD_INTERNAL_COMPARATE_DATA_WRITE , 0 );
	if ( dwRet ) {
		ErrorMessage( dwRet );
		return FALSE;
	}
	dwRet = McsdDataWrite( hDevice, wAxis, MCSD_EXTERNAL_COMPARATE_DATA_WRITE, 0 );
	if ( dwRet ) {
		ErrorMessage( dwRet );
		return FALSE;
	}
	///////////////////////////////////////////////////////////////////////////
	// �o�b�t�@�����O�����֐������s���A�o�b�t�@�����O���I�����܂��B
	///////////////////////////////////////////////////////////////////////////
	dwRet = McsdStartBuffer( hDevice, 1 );
	if ( dwRet ) {
		ErrorMessage( dwRet );
		return FALSE;
	}

	return TRUE;
}
