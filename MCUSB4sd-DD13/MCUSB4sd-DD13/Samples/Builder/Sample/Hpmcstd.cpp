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
/*----------------------------------------------------------------------------*/
#include <windows.h>
#pragma hdrstop
#include <condefs.h>
#include "Hpmcstd.h"

//-------------------------------------------------------------------
USEFILE("Hpmcstd.h");
USELIB("..\Lib\mcstdbo.lib");
//-------------------------------------------------------------------

// define
#define _FALSE			(HANDLE)0xFFFFFFFF
#define MCUSB4sd_AXIS1	0
#define MCUSB4sd_AXIS2	1
#define MCUSB4sd_AXIS3	2
#define MCUSB4sd_AXIS4	3

// Global variable.
const TCHAR szTitle[]		= "Sample Program";
const TCHAR szBoadName[]	= "MCUSB4sd";

// Prototype of a function.
static void ResultMessage(LPCTSTR lpszbuf);
static void ErrorMessage(DWORD dwErrorCode);
static BOOL Mcusb4sdSetupAxis(HANDLE hDevice,WORD wAxis);
static BOOL Mcusb4sdStatusPrint(HANDLE hDevice,WORD wAxis);
void WINAPI Mcusb4CallBack(PMCSDINTINF pIntInf);

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
//�@�@�f�B�b�v�X�C�b�`�O�̃{�[�h���I�[�v�����āA��ꎲ���h���C�u���܂��B
//�@�@�h���C�u�I����ɁA�e������擾���ĕ\�����܂��B
//�@5.�߂�l
//�@�@�֐��� WM_QUIT ���b�Z�[�W���󂯎���Đ���ɏI������ꍇ�́A
//�@�@���b�Z�[�W�� wParam �p�����[�^�Ɋi�[����Ă���I���R�[�h��Ԃ��Ă��������B
//�@�@�֐������b�Z�[�W���[�v�ɓ���O�ɏI������ꍇ�́A0 ��Ԃ��Ă��������B
/*-----------------------------------------------------------------*/
#pragma argsused
int WINAPI WinMain( HINSTANCE hInstance, HINSTANCE hPrevInstance, LPSTR lpCmdLine, int nCmdShow )
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
	wsprintf( szBuf, "%s\nDevice Open\n", szBoadName );
	ResultMessage( szBuf );
	///////////////////////////////////////////////////////////////////////////
	// ��P���̐ݒ���s���܂��B
	///////////////////////////////////////////////////////////////////////////
	if ( FALSE == Mcusb4sdSetupAxis( hDevice, MCUSB4sd_AXIS1 ))
		return 1;
	///////////////////////////////////////////////////////////////////////////
	// ��P�����{������ 10000 �p���X�h���C�u���܂��B
	///////////////////////////////////////////////////////////////////////////
	dwRet = McsdDriveStart( hDevice, MCUSB4sd_AXIS1, 0, 10000 );
	if ( dwRet ) {
		ErrorMessage( dwRet );
		return 1;
	}
	wsprintf( szBuf,"%s\n���䎲 %d �{������ 10000 �p���X�o�͂��Ă��܂�", szBoadName, MCUSB4sd_AXIS1 );
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
	wsprintf( szBuf,"%s\n���䎲 %d ���_���A�̎��s���ł�", szBoadName, MCUSB4sd_AXIS1 );
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
	wsprintf( szBuf, "%s\nDevice Close\n", szBoadName );
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

	wsprintf(szbuf, "%s error\n\nError code: %08lXh", szBoadName, dwErrorCode);
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
	DWORD			dwCount;
	TCHAR			szBuf[1024];
	MCSDSTATUS		Status;

	///////////////////////////////////////////////////////////////////////////
	// �����J�E���^�[��\�����܂��B
	///////////////////////////////////////////////////////////////////////////
	dwRet = McsdGetCounter( hDevice, wAxis, 0, &dwCount );
	if ( dwRet ) {
		ErrorMessage( dwRet );
		return FALSE;
	}
	// �����J�E���^�� 28 �r�b�g�Ȃ̂ŕ����g�����܂��B
	if ( dwCount & 0x08000000 ) dwCount |= 0xF0000000;
	wsprintf( szBuf,"%s\n���䎲 %d �����J�E���^�[: %d", szBoadName, wAxis, dwCount);
	ResultMessage( szBuf );
	///////////////////////////////////////////////////////////////////////////
	// �O���J�E���^�[��\�����܂��B
	///////////////////////////////////////////////////////////////////////////
	dwRet = McsdGetCounter( hDevice, wAxis, 1, &dwCount );
	if ( dwRet ) {
		ErrorMessage( dwRet );
		return FALSE;
	}
	// �O���J�E���^�� 28 �r�b�g�Ȃ̂ŕ����g�����܂��B
	if ( dwCount & 0x08000000 ) dwCount |= 0xF0000000;
	wsprintf( szBuf,"%s\n���䎲 %d �O���J�E���^�[: %d", szBoadName, wAxis, dwCount);
	ResultMessage( szBuf );
	///////////////////////////////////////////////////////////////////////////
	// �G���h�X�e�[�^�X���擾���ĕ\�����܂��B
	// ���J�j�J���V�O�i�����擾���ĕ\�����܂��B
	// ���j�o�[�T���V�O�i�����擾���ĕ\�����܂��B
	///////////////////////////////////////////////////////////////////////////
	dwRet = McsdGetStatus( hDevice, wAxis, &Status );
	if ( dwRet ) {
		ErrorMessage( dwRet );
		return FALSE;
	}
	wsprintf( szBuf,"%s\n���䎲 %d �G���h�X�e�[�^�X: %Xh", szBoadName, wAxis, Status.bEnd);
	ResultMessage( szBuf );
	wsprintf( szBuf,"%s\n���䎲 %d ���J�j�J���V�O�i��: %Xh", szBoadName, wAxis, Status.bMechanical);
	ResultMessage( szBuf );
	wsprintf( szBuf,"%s\n���䎲 %d ���j�o�[�T���V�O�i��: %Xh", szBoadName, wAxis, Status.bUniversal);
	ResultMessage( szBuf );
	// �h���C�u���ł���Ό�����~���܂��B
	if ( Status.bDrive & 0x01 ) {
		dwRet = McsdDriveStop( hDevice, wAxis, 0);
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
//		�p���X�o�͕���
//		���~�b�g�M�����̓A�N�e�B�u���x��
//�@�@�@�J�E���^�ݒ�
//		�R���p���[�^�ݒ�
//		���x�ݒ�
//�@5.�߂�l
//�@�@TRUE�F����I��
//�@�@TRUE�ȊO�F�c�k�k�֐����s���ɔ��������G���[�R�[�h
/*-----------------------------------------------------------------*/
static BOOL Mcusb4sdSetupAxis( HANDLE hDevice, WORD wAxis )
{

	DWORD			dwRet;
	MCSDSPDDATAPPS	SpdDataPPS;

	///////////////////////////////////////////////////////////////////////////
	// �p���X�o�͕���	�Q�p���X����
	// DIR   �o�͒[�q 	CW�p���X  �A�N�e�B�u Hi
	// PULSE �o�͒[�q	CCW�p���X �A�N�e�B�u Hi
	///////////////////////////////////////////////////////////////////////////
    dwRet = McsdSetPulseMode( hDevice, wAxis, 4);
	if ( dwRet ) {
		ErrorMessage( dwRet );
		return FALSE;
	}
	///////////////////////////////////////////////////////////////////////////
	// +LMT		  ���͐M���A�N�e�B�u���x��	Low
	// -LMT		  ���͐M���A�N�e�B�u���x��	Low
	// ALARM	  ���͐M���A�N�e�B�u���x��	Low
	// INPOSITION ���͐M���A�N�e�B�u���x��	Low
	///////////////////////////////////////////////////////////////////////////
    dwRet = McsdSetLimit( hDevice, wAxis, 0x33);
	if ( dwRet ) {
		ErrorMessage( dwRet );
		return FALSE;
	}
	///////////////////////////////////////////////////////////////////////////
	// EXTERNAL COUNTER ���͎d�l	�Q���M���S���{����
	///////////////////////////////////////////////////////////////////////////
	dwRet = McsdSetExternalCounterMode( hDevice, wAxis, 3);
	if ( dwRet ) {
		ErrorMessage( dwRet );
		return FALSE;
	}
	///////////////////////////////////////////////////////////////////////////
	// Alarm stop mode set
	// Inposition wait mode reset
	// Limit input stop enable (Emergency stop)
	///////////////////////////////////////////////////////////////////////////
    dwRet = McsdSetSignalStop( hDevice, wAxis, 1, 0, 2);
	if ( dwRet ) {
		ErrorMessage( dwRet );
		return FALSE;
	}
	///////////////////////////////////////////////////////////////////////////
	// ���N�����x	100PPS
	// �ō����x 	5000PPS
	// �������� 	250mSec
	// �r��������	0%�i�����������j
	///////////////////////////////////////////////////////////////////////////
    SpdDataPPS.dAccel = 250;
    SpdDataPPS.dHighSpeed = 5000;
    SpdDataPPS.dLowSpeed = 100;
    SpdDataPPS.dSratio = 0;
    dwRet = McsdSetSpeedPPS( hDevice, wAxis, &SpdDataPPS);
	if ( dwRet ) {
		ErrorMessage( dwRet );
		return FALSE;
	}
	///////////////////////////////////////////////////////////////////////////
	// �J�E���^�ݒ�
	//
	// INTERNAL COUNTER �y�� EXTERNAL COUNTER �� 0h ���������݂܂�
	///////////////////////////////////////////////////////////////////////////
    dwRet = McsdSetCounter( hDevice, wAxis, 0, 0);
	if ( dwRet ) {
		ErrorMessage( dwRet );
		return FALSE;
	}
    dwRet = McsdSetCounter( hDevice, wAxis, 1, 0);
	if ( dwRet ) {
		ErrorMessage( dwRet );
		return FALSE;
	}
	///////////////////////////////////////////////////////////////////////////
	// �R���p���[�^�ݒ�
	//
	// INTERNAL COMPARATER �y�� EXTERNAL COMPARATER �� 0h ���������݂܂�
	///////////////////////////////////////////////////////////////////////////
    dwRet = McsdSetComparator( hDevice, wAxis, 0, 0);
	if ( dwRet ) {
		ErrorMessage( dwRet );
		return FALSE;
	}
    dwRet = McsdSetComparator( hDevice, wAxis, 1, 0);
	if ( dwRet ) {
		ErrorMessage( dwRet );
		return FALSE;
	}

	return TRUE;
}
