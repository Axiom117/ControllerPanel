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
//　　ディップスイッチ０のボードをオープンして、第一軸と第三軸をドライブします。
//　　ドライブ終了後に、各種情報を取得して表示します。
//　5.戻り値
//　　関数が WM_QUIT メッセージを受け取って正常に終了する場合は、
//　　メッセージの wParam パラメータに格納されている終了コードを返してください。
//　　関数がメッセージループに入る前に終了する場合は、0 を返してください。
/*-----------------------------------------------------------------*/
int WINAPI WinMain( HINSTANCE hInstance, HINSTANCE hPrevInstance,
										LPSTR lpCmdLine, int nCmdShow )
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
	wsprintf( szBuf, _T("%s\n")
		_T("Device Open\n"), szBoadName );
	ResultMessage( szBuf );
	///////////////////////////////////////////////////////////////////////////
	// 第１軸の設定を行います。
	///////////////////////////////////////////////////////////////////////////
	if ( FALSE == Mcusb4sdSetupAxis( hDevice, MCUSB4sd_AXIS1 ))
		return 1;
	///////////////////////////////////////////////////////////////////////////
	// 第１軸を＋方向に 15000 パルスドライブします。
	///////////////////////////////////////////////////////////////////////////
	dwRet = McsdDataWrite( hDevice, MCUSB4sd_AXIS1, MCSD_PLUS_INDEX_PULSE_DRIVE, 15000 );
	if ( dwRet ) {
		ErrorMessage( dwRet );
		return 1;
	}
	wsprintf( szBuf,
		_T("%s\n")
		_T("制御軸 %d ＋方向に 15000 パルス出力しています"), szBoadName, MCUSB4sd_AXIS1 );
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
	wsprintf( szBuf,
		_T("%s\n")
		_T("制御軸 %d 原点復帰の実行中です"), szBoadName, MCUSB4sd_AXIS1 );
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
	wsprintf( szBuf, _T("%s\n")
		_T("Device Close\n"), szBoadName );
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

	wsprintf(szbuf, _T("%s error\n\n")
					_T("Error code: %08lXh"), szBoadName, dwErrorCode);
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
	BYTE			bData;
	DWORD			dwCount;
	WORD			wPortBase;
	TCHAR			szBuf[1024];

	///////////////////////////////////////////////////////////////////////////
	// MCUSB4sd は 1 軸で 8 ポートアドレスを占有します。
	///////////////////////////////////////////////////////////////////////////
	wPortBase = wAxis * 8;
	///////////////////////////////////////////////////////////////////////////
	// 内部カウンターを表示します。
	///////////////////////////////////////////////////////////////////////////
	dwRet = McsdDataRead( hDevice, wAxis, MCSD_INTERNAL_COUNTER_READ, &dwCount );
	if ( dwRet ) {
		ErrorMessage( dwRet );
		return FALSE;
	}
	// 内部カウンタは 28 ビットなので符号拡張します。
	if ( dwCount & 0x08000000 ) dwCount |= 0xF0000000;
	wsprintf( szBuf,
		_T("%s\n")
		_T("制御軸 %d 内部カウンター: %d"), szBoadName, wAxis, dwCount);
	ResultMessage( szBuf );
	///////////////////////////////////////////////////////////////////////////
	// 外部カウンターを表示します。
	///////////////////////////////////////////////////////////////////////////
	dwRet = McsdDataRead( hDevice, wAxis, MCSD_EXTERNAL_COUNTER_READ, &dwCount );
	if ( dwRet ) {
		ErrorMessage( dwRet );
		return FALSE;
	}
	// 外部カウンタは 28 ビットなので符号拡張します。
	if ( dwCount & 0x08000000 ) dwCount |= 0xF0000000;
	wsprintf( szBuf,
		_T("%s\n")
		_T("制御軸 %d 外部カウンター: %d"), szBoadName, wAxis, dwCount);
	ResultMessage( szBuf );
	///////////////////////////////////////////////////////////////////////////
	// エンドステータスを取得して表示します。
	///////////////////////////////////////////////////////////////////////////
	dwRet = McsdInP( hDevice, (WORD)(wPortBase + MCSD_PORT_END_STATUS), &bData );
	if ( dwRet ) {
		ErrorMessage( dwRet );
		return FALSE;
	}
	wsprintf( szBuf,
		_T("%s\n")
		_T("制御軸 %d エンドステータス: %Xh"), szBoadName, wAxis, bData);
	ResultMessage( szBuf );
	///////////////////////////////////////////////////////////////////////////
	// メカニカルシグナルを取得して表示します。
	///////////////////////////////////////////////////////////////////////////
	dwRet = McsdInP( hDevice, (WORD)(wPortBase + MCSD_PORT_MECHANICAL_SIGNAL), &bData );
	if ( dwRet ) {
		ErrorMessage( dwRet );
		return FALSE;
	}
	wsprintf( szBuf,
		_T("%s\n")
		_T("制御軸 %d メカニカルシグナル: %Xh"), szBoadName, wAxis, bData);
	ResultMessage( szBuf );
	///////////////////////////////////////////////////////////////////////////
	// ユニバーサルシグナルを取得して表示します。
	///////////////////////////////////////////////////////////////////////////
	dwRet = McsdInP( hDevice, (WORD)(wPortBase + MCSD_PORT_UNIVERSAL_SIGNAL), &bData );
	if ( dwRet ) {
		ErrorMessage( dwRet );
		return FALSE;
	}
	wsprintf( szBuf,
		_T("%s\n")
		_T("制御軸 %d ユニバーサルシグナル: %Xh"), szBoadName, wAxis, bData);
	ResultMessage( szBuf );
	///////////////////////////////////////////////////////////////////////////
	// ドライブ中であれば減速停止します。
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
//　1.日本語名
//　　制御軸の各種設定。
//　2.パラメタ説明
//　　hDevice：デバイスハンドル
//　　wAxis：制御軸
//　3.概要
//　　引数で指定されたデバイスの制御軸の設定を行います。
//　4.機能説明
//　　下記の設定を行います。
//　　　MODE-A
//　　　MODE-B
//　　　INPOSITION WAIT MODE RESET
//　　　ALARM STOP ENABLE MODE SET
//　　　RANGE DATA
//　　　START/STOP SPEED DATA
//　　　OBJECT SPEED DATA
//　　　RATE-A DATA
//　　　RATE-B DATA
//　　　RATE-C DATA
//　　　RATE CHANGE POINT A-B
//　　　RATE CHANGE POINT B-C
//　　　カウンタ設定
//　5.戻り値
//　　TRUE：正常終了
//　　TRUE以外：ＤＬＬ関数実行時に発生したエラーコード
/*-----------------------------------------------------------------*/
static BOOL Mcusb4sdSetupAxis( HANDLE hDevice, WORD wAxis )
{

	DWORD			dwRet;
	WORD			wPortBase;

	///////////////////////////////////////////////////////////////////////////
	// MCUSB4sd は 1 軸で 8 ポートアドレスを占有します。
	///////////////////////////////////////////////////////////////////////////
	wPortBase = wAxis * 8;
	///////////////////////////////////////////////////////////////////////////
	// 関数のバッファリングを開始します。
	///////////////////////////////////////////////////////////////////////////
	dwRet = McsdStartBuffer( hDevice, 16 );
	if ( dwRet ) {
		ErrorMessage( dwRet );
		return FALSE;
	}
	///////////////////////////////////////////////////////////////////////////
	// MODE-A SET
	//
	// 減速開始ポイント検出方式				自動
	// パルス出力方式						２パルス方式
	// DIR   出力端子 						CWパルス  アクティブ Hi
	// PULSE 出力端子						CCWパルス アクティブ Hi
	///////////////////////////////////////////////////////////////////////////
	dwRet = McsdOutP( hDevice, (WORD)(wPortBase + MCSD_PORT_MODE_A), 0x40 );
	if ( dwRet ) {
		ErrorMessage( dwRet );
		return FALSE;
	}
	///////////////////////////////////////////////////////////////////////////
	// MODE-B SET
	//
	// EXTERNAL COUNTER 入力仕様			２相信号４逓倍入力
	// DEND 入力信号アクティブレベル		Low
	// DERR 入力信号アクティブレベル		Low
	// -SLM 入力信号アクティブレベル		Low
	// +SLM 入力信号アクティブレベル		Low
	// -ELM 入力信号アクティブレベル		Low
	// +ELM 入力信号アクティブレベル		Low
	///////////////////////////////////////////////////////////////////////////
	dwRet = McsdOutP( hDevice, (WORD)(wPortBase + MCSD_PORT_MODE_B), 0xFF );
	if ( dwRet ) {
		ErrorMessage( dwRet );
		return FALSE;
	}
	///////////////////////////////////////////////////////////////////////////
	// モード設定
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
	// データ設定
	//
	// RANGE DATA				250		出力周波数設定単位 	500÷250=2PPS
	// LOW SPEED DATA			500		自起動速度	 		500×2PPS=1000PPS
	// HIGH SPEED DATA			3000	最高速度 			3000×2PPS=6000PPS
	// RATE-A DATA              1024	加速時間設定単位 	1024÷(4.096×10^6)=0.25mSec
	//									加速時間 			(3000-500)×0.25mSec = 625mSec
	// RATE-B DATA        				デフォルト値 8191(1FFFh)
	// RATE-C DATA						デフォルト値 8191(1FFFh)
	// RATE CHANGE POINT A-B			デフォルト値 8191(1FFFh)
	// RATE CHANGE POINT B-C			デフォルト値 8191(1FFFh)
	//
	// この設定により RATE-A DATA による直線加減速となります
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
	// カウンタ設定
	//
	// INTERNAL COUNTER 及び EXTERNAL COUNTER に 0h を書き込みます
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
	// コンパレータ設定
	//
	// INTERNAL COMPARATER 及び EXTERNAL COMPARATER に 0h を書き込みます
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
	// バッファリングした関数を実行し、バッファリングを終了します。
	///////////////////////////////////////////////////////////////////////////
	dwRet = McsdStartBuffer( hDevice, 1 );
	if ( dwRet ) {
		ErrorMessage( dwRet );
		return FALSE;
	}

	return TRUE;
}
