Attribute VB_Name = "Module1"
' ----------------------------------------------------------------------------
'
'   Hpmcstd.bas
'
'   Copyright 2003 HI-P Tech Corporation
'
' ----------------------------------------------------------------------------
'   Change history
'
'       V1.00   2003-05-10  New creation.
' ----------------------------------------------------------------------------

'
Public Const G_FALSE = &HFFFFFFFF
Public Const MCUSB4sd_AXIS1 = 0
Public Const MCUSB4sd_AXIS2 = 1
Public Const MCUSB4sd_AXIS3 = 2
Public Const MCUSB4sd_AXIS4 = 3

' Error code
Public Const MCSD_ERROR_SUCCESS = 0
Public Const MCSD_ERROR_SYSTEM = 1001
Public Const MCSD_ERROR_NO_DEVICE = 1002
Public Const MCSD_ERROR_IN_USE = 1003
Public Const MCSD_ERROR_ID = 1004
Public Const MCSD_ERROR_AXIS = 1005
Public Const MCSD_ERROR_PORT = 1006
Public Const MCSD_ERROR_PARAMETER = 1007
Public Const MCSD_ERROR_PROC = 1008
Public Const MCSD_ERROR_CALLBACK = 1009
Public Const MCSD_ERROR_HANDLE = 1010
Public Const MCSD_ERROR_USB_TRANS = 1011
Public Const MCSD_ERROR_USB_RECEIVE = 1012
Public Const MCSD_ERROR_USB_OFFLINE = 1013
Public Const MCSD_ERROR_ORG_RETURN = 1014

' Device information structure
Type MCSDDEVCONFIG
    wId As Integer
    wIOPortBase As Integer
    wRomVersion As Integer
End Type

' Interrupt information structure
Type MCSDINTINF
    dwId As Long
    dwAxis As Long
    dwIntFactor As Long
    dwDriveStatus As Long
    dwEndStatus As Long
    dwMechanicalSignal As Long
    dwUniversalSignal As Long
End Type

' Event set structure
Type MCSDINTFACTOR
    wDrive As Integer
    wICL As Integer
    wICG As Integer
    wECL As Integer
    wECG As Integer
End Type

' Drive speed set structure
Type MCSDSPDDATA
    dwMode As Long
    dwRange As Long
    dwHighSpeed As Long
    dwLowSpeed As Long
    dwRate(0 To 2) As Long
    dwRateChgPnt(0 To 1) As Long
    dwScw(0 To 1) As Long
    dwRearPulse As Long
End Type

' Drive speed set structure (PPS)
Type MCSDSPDDATAPPS
    dHighSpeed As Double
    dLowSpeed As Double
    dAccel As Double
    dSratio As Double
End Type

' Status information structure
Type MCSDSTATUS
    bDrive As Byte
    bEnd As Byte
    bMechanical As Byte
    bUniversal As Byte
End Type

' Set origin return table structure
Type MCSDORGRET
    dwAxis As Long
    dwTable As Long
    dwAction As Long
    dwDir As Long
    dwSPD As Long
    dwEdge As Long
    dwWait As Long
End Type

'
' Prototype of a DLL function.
'
Declare Function McsdOpen Lib "Hpmcstd.DLL" (ByVal pszDeviceName As String, ByVal wId As Integer) As Long
Declare Function McsdClose Lib "Hpmcstd.DLL" (ByVal hDevice As Long) As Long
Declare Function McsdReset Lib "Hpmcstd.DLL" (ByVal hDevice As Long) As Long
Declare Function McsdGetDeviceConfig Lib "Hpmcstd.DLL" (ByVal hDevice As Long, ByRef pDevConf As MCSDDEVCONFIG) As Long
Declare Function McsdSetPulseMode Lib "Hpmcstd.DLL" (ByVal hDevice As Long, ByVal wAxis As Integer, ByVal wMode As Integer) As Long
Declare Function McsdGetPulseMode Lib "Hpmcstd.DLL" (ByVal hDevice As Long, ByVal wAxis As Integer, ByRef pwMode As Integer) As Long
Declare Function McsdSetLimit Lib "Hpmcstd.DLL" (ByVal hDevice As Long, ByVal wAxis As Integer, ByVal wLevel As Integer) As Long
Declare Function McsdGetLimit Lib "Hpmcstd.DLL" (ByVal hDevice As Long, ByVal wAxis As Integer, ByRef pwLevel As Integer) As Long
Declare Function McsdSetExternalCounterMode Lib "Hpmcstd.DLL" (ByVal hDevice As Long, ByVal wAxis As Integer, ByVal wInputSpec As Integer) As Long
Declare Function McsdGetExternalCounterMode Lib "Hpmcstd.DLL" (ByVal hDevice As Long, ByVal wAxis As Integer, ByRef pwInputSpec As Integer) As Long
Declare Function McsdSetExternalCounterClear Lib "Hpmcstd.DLL" (ByVal hDevice As Long, ByVal wAxis As Integer, ByVal wRequest As Integer, ByVal wSignal As Integer) As Long
Declare Function McsdGetExternalCounterClear Lib "Hpmcstd.DLL" (ByVal hDevice As Long, ByVal wAxis As Integer, ByRef pwRequest As Integer, ByRef pwSignal As Integer) As Long
Declare Function McsdSetPreScaler Lib "Hpmcstd.DLL" (ByVal hDevice As Long, ByVal wAxis As Integer, ByVal wCounter As Integer, ByVal dwData As Long) As Long
Declare Function McsdGetPreScaler Lib "Hpmcstd.DLL" (ByVal hDevice As Long, ByVal wAxis As Integer, ByVal wCounter As Integer, ByRef pdwData As Long) As Long
Declare Function McsdSetComparator Lib "Hpmcstd.DLL" (ByVal hDevice As Long, ByVal wAxis As Integer, ByVal wCounter As Integer, ByVal dwData As Long) As Long
Declare Function McsdGetComparator Lib "Hpmcstd.DLL" (ByVal hDevice As Long, ByVal wAxis As Integer, ByVal wCounter As Integer, ByRef pdwData As Long) As Long
Declare Function McsdSetSignalStop Lib "Hpmcstd.DLL" (ByVal hDevice As Long, ByVal wAxis As Integer, ByVal wAlmStop As Integer, ByVal wInPositionWait As Integer, ByVal wLMT As Integer) As Long
Declare Function McsdGetSignalStop Lib "Hpmcstd.DLL" (ByVal hDevice As Long, ByVal wAxis As Integer, ByRef pwAlmStop As Integer, ByRef pwInPositionWait As Integer, ByRef pwLMT As Integer) As Long
Declare Function McsdSetSpeed Lib "Hpmcstd.DLL" (ByVal hDevice As Long, ByVal wAxis As Integer, ByRef pSpdData As MCSDSPDDATA) As Long
Declare Function McsdGetSpeed Lib "Hpmcstd.DLL" (ByVal hDevice As Long, ByVal wAxis As Integer, ByRef pSpdData As MCSDSPDDATA) As Long
Declare Function McsdSetSpeedPPS Lib "Hpmcstd.DLL" (ByVal hDevice As Long, ByVal wAxis As Integer, ByRef pSpdDataPPS As MCSDSPDDATAPPS) As Long
Declare Function McsdGetSpeedPPS Lib "Hpmcstd.DLL" (ByVal hDevice As Long, ByVal wAxis As Integer, ByRef pSpdDataPPS As MCSDSPDDATAPPS) As Long
Declare Function McsdGetNowSpeed Lib "Hpmcstd.DLL" (ByVal hDevice As Long, ByVal wAxis As Integer, ByVal wKind As Integer, ByRef pdSpeed As Double) As Long
Declare Function McsdGetStatus Lib "Hpmcstd.DLL" (ByVal hDevice As Long, ByVal wAxis As Integer, ByRef pStatus As MCSDSTATUS) As Long
Declare Function McsdGetAxisBusy Lib "Hpmcstd.DLL" (ByVal hDevice As Long, ByRef pwStatus As Integer) As Long
Declare Function McsdSetUniversal Lib "Hpmcstd.DLL" (ByVal hDevice As Long, ByVal wAxis As Integer, ByVal bUniversal As Byte) As Long
Declare Function McsdSetCounter Lib "Hpmcstd.DLL" (ByVal hDevice As Long, ByVal wAxis As Integer, ByVal wCounter As Integer, ByVal dwData As Long) As Long
Declare Function McsdGetCounter Lib "Hpmcstd.DLL" (ByVal hDevice As Long, ByVal wAxis As Integer, ByVal wCounter As Integer, ByRef pdwData As Long) As Long
Declare Function McsdGetDeviation Lib "Hpmcstd.DLL" (ByVal hDevice As Long, ByVal wAxis As Integer, ByRef pwData As Integer) As Long
Declare Function McsdDriveStart Lib "Hpmcstd.DLL" (ByVal hDevice As Long, ByVal wAxis As Integer, ByVal wDrive As Integer, ByVal dwPulse As Long) As Long
Declare Function McsdDriveStop Lib "Hpmcstd.DLL" (ByVal hDevice As Long, ByVal wAxis As Integer, ByVal wStop As Integer) As Long
Declare Function McsdSyncStart Lib "Hpmcstd.DLL" (ByVal hDevice As Long, ByVal wSyncSignal As Integer) As Long
Declare Function McsdSignalScan Lib "Hpmcstd.DLL" (ByVal hDevice As Long, ByVal wAxis As Integer, ByVal wScanSpeed As Integer, ByVal wScanSignal As Integer) As Long
Declare Function McsdOrgReturn Lib "Hpmcstd.DLL" (ByVal hDevice As Long, ByVal wAxis As Integer, ByVal wAction As Integer) As Long
Declare Function McsdSetOrgReturn Lib "Hpmcstd.DLL" (ByVal hDevice As Long, ByRef pOrgRet As MCSDORGRET) As Long
Declare Function McsdHiSpeedOverride Lib "Hpmcstd.DLL" (ByVal hDevice As Long, ByVal wAxis As Integer, ByVal wKind As Integer, ByVal dSpeed As Double) As Long
Declare Function McsdIndexOverride Lib "Hpmcstd.DLL" (ByVal hDevice As Long, ByVal wAxis As Integer, ByVal dwData As Long) As Long
'Declare Function McsdIntEvent Lib "Hpmcstd.DLL" (ByVal hDevice As Long, ByVal hWnd As Long, ByVal uMessage As Long, ByVal hEvent As Long, ByVal pfnCallBack As Long) As Long
'Declare Function McsdEventEnable Lib "Hpmcstd.DLL" (ByVal hDevice As Long, ByVal wAxis As Integer, ByRef pIntFactor As MCSDINTFACTOR) As Long
'Declare Function McsdGetIntFactor Lib "Hpmcstd.DLL" (ByVal hDevice As Long, ByVal wAxis As Integer, ByRef pIntInf As MCSDINTINF) As Long
Declare Function McsdInP Lib "Hpmcstd.DLL" (ByVal hDevice As Long, ByVal wPort As Integer, ByRef pbData As Byte) As Long
Declare Function McsdOutP Lib "Hpmcstd.DLL" (ByVal hDevice As Long, ByVal wPort As Integer, ByVal bData As Byte) As Long
Declare Function McsdDataRead Lib "Hpmcstd.DLL" (ByVal hDevice As Long, ByVal wAxis As Integer, ByVal wCommand As Integer, ByRef pdwData As Long) As Long
Declare Function McsdDataWrite Lib "Hpmcstd.DLL" (ByVal hDevice As Long, ByVal wAxis As Integer, ByVal wCommand As Integer, ByVal dwData As Long) As Long
Declare Function McsdSetOfflineMode Lib "Hpmcstd.DLL" (ByVal hDevice As Long, ByVal wAxis As Integer, ByVal wData As Integer) As Long
Declare Function McsdStartBuffer Lib "Hpmcstd.DLL" (ByVal hDevice As Long, ByVal wCount As Integer) As Long
Declare Function McsdEndBuffer Lib "Hpmcstd.DLL" (ByVal hDevice As Long) As Long

