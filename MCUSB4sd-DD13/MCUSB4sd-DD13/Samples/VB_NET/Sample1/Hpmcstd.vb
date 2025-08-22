Option Strict Off
Option Explicit On 

Imports System
Imports System.Runtime.InteropServices

Module Hpmcstd
	' ----------------------------------------------------------------------------
	'
	'   Hpmcstd.vb
	'
    '   Copyright 2006 HI-P Tech Corporation
	'
	' ----------------------------------------------------------------------------
	'   Change history
	'
    '       V1.00   2006-08-20  New creation.
    ' ----------------------------------------------------------------------------
	
	' Error code
	Public Const MCSD_ERROR_SUCCESS As Short = 0
	Public Const MCSD_ERROR_SYSTEM As Short = 1001
	Public Const MCSD_ERROR_NO_DEVICE As Short = 1002
	Public Const MCSD_ERROR_IN_USE As Short = 1003
	Public Const MCSD_ERROR_ID As Short = 1004
	Public Const MCSD_ERROR_AXIS As Short = 1005
	Public Const MCSD_ERROR_PORT As Short = 1006
	Public Const MCSD_ERROR_PARAMETER As Short = 1007
	Public Const MCSD_ERROR_PROC As Short = 1008
	Public Const MCSD_ERROR_CALLBACK As Short = 1009
	Public Const MCSD_ERROR_HANDLE As Short = 1010
	Public Const MCSD_ERROR_USB_TRANS As Short = 1011
	Public Const MCSD_ERROR_USB_RECEIVE As Short = 1012
	Public Const MCSD_ERROR_USB_OFFLINE As Short = 1013
	Public Const MCSD_ERROR_ORG_RETURN As Short = 1014
	
	' Device information structure
    <StructLayout(LayoutKind.Sequential)> _
    Structure MCSDDEVCONFIG
        Public wId As Short
        Public wIOPortBase As Short
        Public wRomVersion As Short
    End Structure

    ' Interrupt information structure
    <StructLayout(LayoutKind.Sequential)> _
    Structure MCSDINTINF
        Public dwId As Integer
        Public dwAxis As Integer
        Public dwIntFactor As Integer
        Public dwDriveStatus As Integer
        Public dwEndStatus As Integer
        Public dwMechanicalSignal As Integer
        Public dwUniversalSignal As Integer
    End Structure

    ' Event set structure
    <StructLayout(LayoutKind.Sequential)> _
    Structure MCSDINTFACTOR
        Public wDrive As Short
        Public wICL As Short
        Public wICG As Short
        Public wECL As Short
        Public wECG As Short
    End Structure

    ' Drive speed set structure
    <StructLayout(LayoutKind.Sequential)> _
    Structure MCSDSPDDATA
        Public dwMode As Integer
        Public dwRange As Integer
        Public dwHighSpeed As Integer
        Public dwLowSpeed As Integer
        <MarshalAs(UnmanagedType.ByValArray, SizeConst:=3)> _
        Public dwRate() As Integer
        <MarshalAs(UnmanagedType.ByValArray, SizeConst:=2)> _
        Public dwRateChgPnt() As Integer
        <MarshalAs(UnmanagedType.ByValArray, SizeConst:=2)> _
        Public dwScw() As Integer
        Public dwRearPulse As Integer
        'この構造体のインスタンスを初期化するには、"Initialize" を呼び出さなければなりません。
        Public Sub Initialize()
            ReDim dwRate(2)
            ReDim dwRateChgPnt(1)
            ReDim dwScw(1)
        End Sub
    End Structure

    ' Drive speed set structure (PPS)
    <StructLayout(LayoutKind.Sequential)> _
    Structure MCSDSPDDATAPPS
        Public dHighSpeed As Double
        Public dLowSpeed As Double
        Public dAccel As Double
        Public dSratio As Double
    End Structure

    ' Status information structure
    <StructLayout(LayoutKind.Sequential)> _
    Structure MCSDSTATUS
        Public bDrive As Byte
        Public bEnd As Byte
        Public bMechanical As Byte
        Public bUniversal As Byte
    End Structure

    ' Set origin return table structure
    <StructLayout(LayoutKind.Sequential)> _
    Structure MCSDORGRET
        Public dwAxis As Integer
        Public dwTable As Integer
        Public dwAction As Integer
        Public dwDir As Integer
        Public dwSPD As Integer
        Public dwEdge As Integer
        Public dwWait As Integer
    End Structure

    '
    ' Prototype of a DLL function.
    '
    Declare Function McsdOpen Lib "Hpmcstd.DLL" (ByVal pszDeviceName As String, ByVal wId As Short) As Integer
    Declare Function McsdClose Lib "Hpmcstd.DLL" (ByVal hDevice As Integer) As Integer
    Declare Function McsdReset Lib "Hpmcstd.DLL" (ByVal hDevice As Integer) As Integer
    Declare Function McsdGetDeviceConfig Lib "Hpmcstd.DLL" (ByVal hDevice As Integer, ByRef pDevConf As MCSDDEVCONFIG) As Integer
    Declare Function McsdSetPulseMode Lib "Hpmcstd.DLL" (ByVal hDevice As Integer, ByVal wAxis As Short, ByVal wMode As Short) As Integer
    Declare Function McsdGetPulseMode Lib "Hpmcstd.DLL" (ByVal hDevice As Integer, ByVal wAxis As Short, ByRef pwMode As Short) As Integer
    Declare Function McsdSetLimit Lib "Hpmcstd.DLL" (ByVal hDevice As Integer, ByVal wAxis As Short, ByVal wLevel As Short) As Integer
    Declare Function McsdGetLimit Lib "Hpmcstd.DLL" (ByVal hDevice As Integer, ByVal wAxis As Short, ByRef pwLevel As Short) As Integer
    Declare Function McsdSetExternalCounterMode Lib "Hpmcstd.DLL" (ByVal hDevice As Integer, ByVal wAxis As Short, ByVal wInputSpec As Short) As Integer
    Declare Function McsdGetExternalCounterMode Lib "Hpmcstd.DLL" (ByVal hDevice As Integer, ByVal wAxis As Short, ByRef pwInputSpec As Short) As Integer
    Declare Function McsdSetExternalCounterClear Lib "Hpmcstd.DLL" (ByVal hDevice As Integer, ByVal wAxis As Short, ByVal wRequest As Short, ByVal wSignal As Short) As Integer
    Declare Function McsdGetExternalCounterClear Lib "Hpmcstd.DLL" (ByVal hDevice As Integer, ByVal wAxis As Short, ByRef pwRequest As Short, ByRef pwSignal As Short) As Integer
    Declare Function McsdSetPreScaler Lib "Hpmcstd.DLL" (ByVal hDevice As Integer, ByVal wAxis As Short, ByVal wCounter As Short, ByVal dwData As Integer) As Integer
    Declare Function McsdGetPreScaler Lib "Hpmcstd.DLL" (ByVal hDevice As Integer, ByVal wAxis As Short, ByVal wCounter As Short, ByRef pdwData As Integer) As Integer
    Declare Function McsdSetComparator Lib "Hpmcstd.DLL" (ByVal hDevice As Integer, ByVal wAxis As Short, ByVal wCounter As Short, ByVal dwData As Integer) As Integer
    Declare Function McsdGetComparator Lib "Hpmcstd.DLL" (ByVal hDevice As Integer, ByVal wAxis As Short, ByVal wCounter As Short, ByRef pdwData As Integer) As Integer
    Declare Function McsdSetSignalStop Lib "Hpmcstd.DLL" (ByVal hDevice As Integer, ByVal wAxis As Short, ByVal wAlmStop As Short, ByVal wInPositionWait As Short, ByVal wLMT As Short) As Integer
    Declare Function McsdGetSignalStop Lib "Hpmcstd.DLL" (ByVal hDevice As Integer, ByVal wAxis As Short, ByRef pwAlmStop As Short, ByRef pwInPositionWait As Short, ByRef pwLMT As Short) As Integer
    Declare Function McsdSetSpeed Lib "Hpmcstd.DLL" (ByVal hDevice As Integer, ByVal wAxis As Short, ByRef pSpdData As MCSDSPDDATA) As Integer
    Declare Function McsdGetSpeed Lib "Hpmcstd.DLL" (ByVal hDevice As Integer, ByVal wAxis As Short, ByRef pSpdData As MCSDSPDDATA) As Integer
    Declare Function McsdSetSpeedPPS Lib "Hpmcstd.DLL" (ByVal hDevice As Integer, ByVal wAxis As Short, ByRef pSpdDataPPS As MCSDSPDDATAPPS) As Integer
    Declare Function McsdGetSpeedPPS Lib "Hpmcstd.DLL" (ByVal hDevice As Integer, ByVal wAxis As Short, ByRef pSpdDataPPS As MCSDSPDDATAPPS) As Integer
    Declare Function McsdGetNowSpeed Lib "Hpmcstd.DLL" (ByVal hDevice As Integer, ByVal wAxis As Short, ByVal wKind As Short, ByRef pdSpeed As Double) As Integer
    Declare Function McsdGetStatus Lib "Hpmcstd.DLL" (ByVal hDevice As Integer, ByVal wAxis As Short, ByRef pStatus As MCSDSTATUS) As Integer
    Declare Function McsdGetAxisBusy Lib "Hpmcstd.DLL" (ByVal hDevice As Integer, ByRef pwStatus As Short) As Integer
    Declare Function McsdSetUniversal Lib "Hpmcstd.DLL" (ByVal hDevice As Integer, ByVal wAxis As Short, ByVal bUniversal As Byte) As Integer
    Declare Function McsdSetCounter Lib "Hpmcstd.DLL" (ByVal hDevice As Integer, ByVal wAxis As Short, ByVal wCounter As Short, ByVal dwData As Integer) As Integer
    Declare Function McsdGetCounter Lib "Hpmcstd.DLL" (ByVal hDevice As Integer, ByVal wAxis As Short, ByVal wCounter As Short, ByRef pdwData As Integer) As Integer
    Declare Function McsdGetDeviation Lib "Hpmcstd.DLL" (ByVal hDevice As Integer, ByVal wAxis As Short, ByRef pwData As Short) As Integer
    Declare Function McsdDriveStart Lib "Hpmcstd.DLL" (ByVal hDevice As Integer, ByVal wAxis As Short, ByVal wDrive As Short, ByVal dwPulse As Integer) As Integer
    Declare Function McsdDriveStop Lib "Hpmcstd.DLL" (ByVal hDevice As Integer, ByVal wAxis As Short, ByVal wStop As Short) As Integer
    Declare Function McsdSyncStart Lib "Hpmcstd.DLL" (ByVal hDevice As Integer, ByVal wSyncSignal As Short) As Integer
    Declare Function McsdSignalScan Lib "Hpmcstd.DLL" (ByVal hDevice As Integer, ByVal wAxis As Short, ByVal wScanSpeed As Short, ByVal wScanSignal As Short) As Integer
    Declare Function McsdOrgReturn Lib "Hpmcstd.DLL" (ByVal hDevice As Integer, ByVal wAxis As Short, ByVal wAction As Short) As Integer
    Declare Function McsdSetOrgReturn Lib "Hpmcstd.DLL" (ByVal hDevice As Integer, ByRef pOrgRet As MCSDORGRET) As Integer
    Declare Function McsdHiSpeedOverride Lib "Hpmcstd.DLL" (ByVal hDevice As Integer, ByVal wAxis As Short, ByVal wKind As Short, ByVal dSpeed As Double) As Integer
    Declare Function McsdIndexOverride Lib "Hpmcstd.DLL" (ByVal hDevice As Integer, ByVal wAxis As Short, ByVal dwData As Integer) As Integer
    Declare Function McsdIntEvent Lib "Hpmcstd.DLL" (ByVal hDevice As Integer, ByVal hWnd As Integer, ByVal uMessage As Integer, ByVal hEvent As Integer, ByVal pfnCallBack As Integer) As Integer
    Declare Function McsdEventEnable Lib "Hpmcstd.DLL" (ByVal hDevice As Integer, ByVal wAxis As Short, ByRef pIntFactor As MCSDINTFACTOR) As Integer
    Declare Function McsdGetIntFactor Lib "Hpmcstd.DLL" (ByVal hDevice As Integer, ByVal wAxis As Short, ByRef pIntInf As MCSDINTINF) As Integer
    Declare Function McsdInP Lib "Hpmcstd.DLL" (ByVal hDevice As Integer, ByVal wPort As Short, ByRef pbData As Byte) As Integer
    Declare Function McsdOutP Lib "Hpmcstd.DLL" (ByVal hDevice As Integer, ByVal wPort As Short, ByVal bData As Byte) As Integer
    Declare Function McsdDataRead Lib "Hpmcstd.DLL" (ByVal hDevice As Integer, ByVal wAxis As Short, ByVal wCommand As Short, ByRef pdwData As Integer) As Integer
    Declare Function McsdDataWrite Lib "Hpmcstd.DLL" (ByVal hDevice As Integer, ByVal wAxis As Short, ByVal wCommand As Short, ByVal dwData As Integer) As Integer
    Declare Function McsdGetUsbVersion Lib "Hpmcstd.DLL" (ByVal hDevice As Integer, ByRef pwVersion As Short) As Integer
    Declare Function McsdSetOfflineMode Lib "Hpmcstd.DLL" (ByVal hDevice As Integer, ByVal wAxis As Short, ByVal wData As Short) As Integer
    Declare Function McsdStartBuffer Lib "Hpmcstd.DLL" (ByVal hDevice As Integer, ByVal wCount As Short) As Integer
    Declare Function McsdEndBuffer Lib "Hpmcstd.DLL" (ByVal hDevice As Integer) As Integer
End Module