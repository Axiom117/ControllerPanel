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
//　1.日本語名
//　　サンプルプログラムメイン関数。
//　2.パラメタ説明
//　　hInstance：アプリケーションの現在のインスタンスのハンドル
//　　hPrevInstance：アプリケーションの前のインスタンスのハンドル（Win32 アプリケーションでは、常に NULL ）
//　　lpCmdLine：コマンドラインが格納された、NULL で終わる文字列へのポインタ
//　　nCmdShow：ウィンドウの表示状態の指定
//　3.概要
//　　ＤＬＬ関数を使用した軸制御の実行例を示します。
//　4.機能説明
//　　ディップスイッチ０のボードをオープンして、第一軸をドライブします。
//　　ドライブ終了後に、各種情報を取得して表示します。
//　5.戻り値
//　　関数が WM_QUIT メッセージを受け取って正常に終了する場合は、
//　　メッセージの wParam パラメータに格納されている終了コードを返してください。
//　　関数がメッセージループに入る前に終了する場合は、0 を返してください。
/*-----------------------------------------------------------------*/
#pragma argsused
int WINAPI WinMain( HINSTANCE hInstance, HINSTANCE hPrevInstance, LPSTR lpCmdLine, int nCmdShow )
{

	HANDLE			hDevice;
	DWORD			dwRet;
	TCHAR			szBuf[1024];
	
	///////////////////////////////////////////////////////////////////////////
	// ボードをオープンします。例としてスイッチ番号０のボードをオープンします。
	///////////////////////////////////////////////////////////////////////////
	hDevice = McsdOpen( szBoadName, 0 );
	if ( _FALSE == hDevice ) {
		MessageBox( NULL, "ボードのオープンに失敗しました", szTitle, MB_OK | MB_ICONSTOP );
		return 1;
	}
	wsprintf( szBuf, "%s\nDevice Open\n", szBoadName );
	ResultMessage( szBuf );
	///////////////////////////////////////////////////////////////////////////
	// 第１軸の設定を行います。
	///////////////////////////////////////////////////////////////////////////
	if ( FALSE == Mcusb4sdSetupAxis( hDevice, MCUSB4sd_AXIS1 ))
		return 1;
	///////////////////////////////////////////////////////////////////////////
	// 第１軸を＋方向に 10000 パルスドライブします。
	///////////////////////////////////////////////////////////////////////////
	dwRet = McsdDriveStart( hDevice, MCUSB4sd_AXIS1, 0, 10000 );
	if ( dwRet ) {
		ErrorMessage( dwRet );
		return 1;
	}
	wsprintf( szBuf,"%s\n制御軸 %d ＋方向に 10000 パルス出力しています", szBoadName, MCUSB4sd_AXIS1 );
	ResultMessage( szBuf );
	///////////////////////////////////////////////////////////////////////////
	// 第１軸を原点検出ドライブ１により原点復帰します。
	///////////////////////////////////////////////////////////////////////////
/* 原点検出処理は、センサー入力が変化しない状態では停止いたしません。必要に応じて実行してください。
	dwRet = McsdOrgReturn( hDevice, MCUSB4sd_AXIS1, 1);
	if ( dwRet ) {
		ErrorMessage( dwRet );
		return 1;
	}
	wsprintf( szBuf,"%s\n制御軸 %d 原点復帰の実行中です", szBoadName, MCUSB4sd_AXIS1 );
	ResultMessage( szBuf );
*/
	///////////////////////////////////////////////////////////////////////////
	// 第１軸の内部カウンタと各種状態を取得して表示します。
	///////////////////////////////////////////////////////////////////////////
	if ( FALSE == Mcusb4sdStatusPrint( hDevice, MCUSB4sd_AXIS1 ))
		return 1;
	///////////////////////////////////////////////////////////////////////////
	// ボードをクローズします。
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
//　1.日本語名
//　　処理結果を表示。
//　2.パラメタ説明
//　　lpszbuf：表示する文章を格納したNULLで終わる文字列のポインタ
//　3.概要
//　　処理結果をメッセージボックスで表示します。
//　4.機能説明
//　　処理結果が格納された指定の文字列をメッセージボックスで表示します。
//　5.戻り値
//　　なし。
/*-----------------------------------------------------------------*/
static void ResultMessage(LPCTSTR lpszbuf)
{
	MessageBox(NULL, lpszbuf, szTitle, MB_OK | MB_ICONINFORMATION);
}

/*-----------------------------------------------------------------*/
//　1.日本語名
//　　エラーメッセージを表示。
//　2.パラメタ説明
//　　dwErrorCode：エラーコード
//　3.概要
//　　エラーコードをメッセージボックスで表示します。
//　4.機能説明
//　　エラーコードをメッセージボックスで表示します。
//　5.戻り値
//　　なし。
/*-----------------------------------------------------------------*/
static void ErrorMessage(DWORD dwErrorCode)
{
	TCHAR szbuf[50];

	wsprintf(szbuf, "%s error\n\nError code: %08lXh", szBoadName, dwErrorCode);
	MessageBox(NULL, szbuf, szTitle, MB_OK | MB_ICONSTOP);
}

/*-----------------------------------------------------------------*/
//　1.日本語名
//　　ボードの各種情報を表示。
//　2.パラメタ説明
//　　hDevice：デバイスハンドル
//　　wAxis：制御軸
//　3.概要
//　　引数で指定されたデバイスの制御軸の各種情報をメッセージボックスで順番に表示します。
//　4.機能説明
//　　内部カウンター、エンドステータス、メカニカルシグナル、ユニバーサルシグナルを取得して表示します。
//　5.戻り値
//　　TRUE：正常終了
//　　TRUE以外：ＤＬＬ関数実行時に発生したエラーコード
/*-----------------------------------------------------------------*/
static BOOL Mcusb4sdStatusPrint( HANDLE hDevice, WORD wAxis )
{

	DWORD			dwRet;
	DWORD			dwCount;
	TCHAR			szBuf[1024];
	MCSDSTATUS		Status;

	///////////////////////////////////////////////////////////////////////////
	// 内部カウンターを表示します。
	///////////////////////////////////////////////////////////////////////////
	dwRet = McsdGetCounter( hDevice, wAxis, 0, &dwCount );
	if ( dwRet ) {
		ErrorMessage( dwRet );
		return FALSE;
	}
	// 内部カウンタは 28 ビットなので符号拡張します。
	if ( dwCount & 0x08000000 ) dwCount |= 0xF0000000;
	wsprintf( szBuf,"%s\n制御軸 %d 内部カウンター: %d", szBoadName, wAxis, dwCount);
	ResultMessage( szBuf );
	///////////////////////////////////////////////////////////////////////////
	// 外部カウンターを表示します。
	///////////////////////////////////////////////////////////////////////////
	dwRet = McsdGetCounter( hDevice, wAxis, 1, &dwCount );
	if ( dwRet ) {
		ErrorMessage( dwRet );
		return FALSE;
	}
	// 外部カウンタは 28 ビットなので符号拡張します。
	if ( dwCount & 0x08000000 ) dwCount |= 0xF0000000;
	wsprintf( szBuf,"%s\n制御軸 %d 外部カウンター: %d", szBoadName, wAxis, dwCount);
	ResultMessage( szBuf );
	///////////////////////////////////////////////////////////////////////////
	// エンドステータスを取得して表示します。
	// メカニカルシグナルを取得して表示します。
	// ユニバーサルシグナルを取得して表示します。
	///////////////////////////////////////////////////////////////////////////
	dwRet = McsdGetStatus( hDevice, wAxis, &Status );
	if ( dwRet ) {
		ErrorMessage( dwRet );
		return FALSE;
	}
	wsprintf( szBuf,"%s\n制御軸 %d エンドステータス: %Xh", szBoadName, wAxis, Status.bEnd);
	ResultMessage( szBuf );
	wsprintf( szBuf,"%s\n制御軸 %d メカニカルシグナル: %Xh", szBoadName, wAxis, Status.bMechanical);
	ResultMessage( szBuf );
	wsprintf( szBuf,"%s\n制御軸 %d ユニバーサルシグナル: %Xh", szBoadName, wAxis, Status.bUniversal);
	ResultMessage( szBuf );
	// ドライブ中であれば減速停止します。
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
//　1.日本語名
//　　制御軸の各種設定。
//　2.パラメタ説明
//　　hDevice：デバイスハンドル
//　　wAxis：制御軸
//　3.概要
//　　引数で指定されたデバイスの制御軸の設定を行います。
//　4.機能説明
//　　下記の設定を行います。
//		パルス出力方式
//		リミット信号入力アクティブレベル
//　　　カウンタ設定
//		コンパレータ設定
//		速度設定
//　5.戻り値
//　　TRUE：正常終了
//　　TRUE以外：ＤＬＬ関数実行時に発生したエラーコード
/*-----------------------------------------------------------------*/
static BOOL Mcusb4sdSetupAxis( HANDLE hDevice, WORD wAxis )
{

	DWORD			dwRet;
	MCSDSPDDATAPPS	SpdDataPPS;

	///////////////////////////////////////////////////////////////////////////
	// パルス出力方式	２パルス方式
	// DIR   出力端子 	CWパルス  アクティブ Hi
	// PULSE 出力端子	CCWパルス アクティブ Hi
	///////////////////////////////////////////////////////////////////////////
    dwRet = McsdSetPulseMode( hDevice, wAxis, 4);
	if ( dwRet ) {
		ErrorMessage( dwRet );
		return FALSE;
	}
	///////////////////////////////////////////////////////////////////////////
	// +LMT		  入力信号アクティブレベル	Low
	// -LMT		  入力信号アクティブレベル	Low
	// ALARM	  入力信号アクティブレベル	Low
	// INPOSITION 入力信号アクティブレベル	Low
	///////////////////////////////////////////////////////////////////////////
    dwRet = McsdSetLimit( hDevice, wAxis, 0x33);
	if ( dwRet ) {
		ErrorMessage( dwRet );
		return FALSE;
	}
	///////////////////////////////////////////////////////////////////////////
	// EXTERNAL COUNTER 入力仕様	２相信号４逓倍入力
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
	// 自起動速度	100PPS
	// 最高速度 	5000PPS
	// 加速時間 	250mSec
	// Ｓ字加減率	0%（直線加減速）
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
	// カウンタ設定
	//
	// INTERNAL COUNTER 及び EXTERNAL COUNTER に 0h を書き込みます
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
	// コンパレータ設定
	//
	// INTERNAL COMPARATER 及び EXTERNAL COMPARATER に 0h を書き込みます
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
