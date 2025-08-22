/*----------------------------------------------------------------------------*/
//
//	Hpmcstd.h
//
//	Copyright 2003 HI-P Tech Corporation
//
/*----------------------------------------------------------------------------*/
//	Change history
//
//		V1.00   2003-05-10  New creation.
/*----------------------------------------------------------------------------*/

#ifdef __cplusplus
extern "C" {
#endif

// error code
#define MCSD_ERROR_SUCCESS			0
#define MCSD_ERROR_SYSTEM			1001
#define MCSD_ERROR_NO_DEVICE		1002
#define MCSD_ERROR_IN_USE			1003
#define MCSD_ERROR_ID				1004
#define MCSD_ERROR_AXIS				1005
#define MCSD_ERROR_PORT				1006
#define MCSD_ERROR_PARAMETER		1007
#define MCSD_ERROR_PROC				1008
#define MCSD_ERROR_CALLBACK			1009
#define MCSD_ERROR_HANDLE			1010
#define MCSD_ERROR_USB_TRANS		1011
#define MCSD_ERROR_USB_RECEIVE		1012
#define MCSD_ERROR_USB_OFFLINE		1013
#define MCSD_ERROR_ORG_RETURN		1014

// MCSD I/O Map
#define MCSD_PORT_DATA1							0x00
#define MCSD_PORT_DATA2							0x01
#define MCSD_PORT_DATA3							0x02
#define MCSD_PORT_DATA4							0x03
#define MCSD_PORT_COMMAND						0x04
#define MCSD_PORT_MODE_A						0x05
#define MCSD_PORT_MODE_B						0x06
#define MCSD_PORT_UNIVERSAL_SIGNAL				0x07
#define MCSD_PORT_DRIVE_STATUS					0x04
#define MCSD_PORT_END_STATUS					0x05
#define MCSD_PORT_MECHANICAL_SIGNAL				0x06

// Commands
#define MCSD_RANGE_DATA_WRITE					0x00
#define MCSD_RANGE_DATA_READ					0x01
#define MCSD_LOW_SPEED_DATA_WRITE				0x02
#define MCSD_LOW_SPEED_DATA_READ				0x03
#define MCSD_HIGH_SPEED_DATA_WRITE				0x04
#define MCSD_HIGH_SPEED_DATA_READ				0x05
#define MCSD_RATE_A_DATA_WRITE					0x06
#define MCSD_RATE_A_DATA_READ					0x07
#define MCSD_RATE_B_DATA_WRITE					0x08
#define MCSD_RATE_B_DATA_READ					0x09
#define MCSD_RATE_C_DATA_WRITE					0x0A
#define MCSD_RATE_C_DATA_READ					0x0B
#define MCSD_RATE_CHANGE_POINT_A_B_WRITE		0x0C
#define MCSD_RATE_CHANGE_POINT_A_B_READ			0x0D
#define MCSD_RATE_CHANGE_POINT_B_C_WRITE		0x0E
#define MCSD_RATE_CHANGE_POINT_B_C_READ			0x0F
#define MCSD_SLOW_DOWN_REAR_PULSE_DATA_WRITE	0x10
#define MCSD_SLOW_DOWN_REAR_PULSE_DATA_READ		0x11
#define MCSD_NOW_SPEED_DATA_READ				0x12
#define MCSD_DRIVE_PULSE_COUNTER_READ			0x13
#define MCSD_INDEX_PULSE_DATA_OVERRIDE			0x14
#define MCSD_INDEX_PULSE_DATA_READ				0x15
#define MCSD_DEVIATION_DATA_READ				0x16
#define MCSD_INPOSITION_WAIT_MODE_A_SET			0x17
#define MCSD_INPOSITION_WAIT_MODE_B_SET			0x18
#define MCSD_INPOSITION_WAIT_MODE_RESET			0x19
#define MCSD_ALARM_STOP_ENABLE_MODE_SET			0x1A
#define MCSD_ALARM_STOP_ENABLE_MODE_RESET		0x1B
#define MCSD_INTERRUPT_OUT_ENABLE_MODE_SET		0x1C
#define MCSD_INTERRUPT_OUT_ENABLE_MODE_RESET	0x1D
#define MCSD_SLOW_DOWN_STOP						0x1E
#define MCSD_EMERGENCY_STOP						0x1F
#define MCSD_PLUS_INDEX_PULSE_DRIVE				0x20
#define MCSD_MINUS_INDEX_PULSE_DRIVE			0x21
#define MCSD_PLUS_SCAN_DRIVE					0x22
#define MCSD_MINUS_SCAN_DRIVE					0x23
#define MCSD_PLUS_SIGNAL_SCAN_A_DRIVE			0x24
#define MCSD_MINUS_SIGNAL_SCAN_A_DRIVE			0x25
#define MCSD_PLUS_SIGNAL_SCAN_B_DRIVE			0x26
#define MCSD_MINUS_SIGNAL_SCAN_B_DRIVE			0x27
#define MCSD_INTERNAL_COUNTER_WRITE				0x28
#define MCSD_INTERNAL_COUNTER_READ				0x29
#define MCSD_INTERNAL_COMPARATE_DATA_WRITE		0x2A
#define MCSD_INTERNAL_COMPARATE_DATA_READ		0x2B
#define MCSD_EXTERNAL_COUNTER_WRITE				0x2C
#define MCSD_EXTERNAL_COUNTER_READ				0x2D
#define MCSD_EXTERNAL_COMPARATE_DATA_WRITE		0x2E
#define MCSD_EXTERNAL_COMPARATE_DATA_READ		0x2F
#define MCSD_INTERNAL_PRE_SCALE_DATA_WRITE		0x30
#define MCSD_INTERNAL_PRE_SCALE_DATA_READ		0x31
#define MCSD_EXTERNAL_PRE_SCALE_DATA_WRITE		0x32
#define MCSD_EXTERNAL_PRE_SCALE_DATA_READ		0x33
#define MCSD_CLEAR_SIGNAL_SELECT				0x34
#define MCSD_ONE_TIME_CLEAR_REQUEST				0x35
#define MCSD_FULL_TIME_CLEAR_REQUEST			0x36
#define MCSD_CLEAR_REQUEST_RESET				0x37
#define MCSD_REVERSE_COUNT_MODE_SET				0x38
#define MCSD_REVERSE_COUNT_MODE_RESET			0x39
#define MCSD_STRAIGHT_ACCELERATE_MODE_SET		0x84
#define MCSD_ASYM_STRAIGHT_ACCELERATE_MODE_SET	0x85
#define MCSD_S_CURVE_ACCELERATE_MODE_SET		0x86
#define MCSD_ASYM_S_CURVE_ACCELERATE_MODE_SET	0x87
#define MCSD_SCW_A_DATA_WRITE					0x88
#define MCSD_SCW_A_DATA_READ					0x89
#define MCSD_SCW_B_DATA_WRITE					0x8A
#define MCSD_SCW_B_DATA_READ					0x8B
#define MCSD_SLOW_DOWN_LIMIT_ENABLE_MODE_SET	0x8C
#define MCSD_SLOW_DOWN_LIMIT_ENABLE_MODE_RESET	0x8D
#define MCSD_EMERGENCY_LIMIT_ENABLE_MODE_SET	0x8E
#define MCSD_EMERGENCY_LIMIT_ENABLE_MODE_RESET	0x8F
#define MCSD_INITIAL_CLEAR						0x90
#define MCSD_SYNCHRONOUS_SIGNAL_SELECT			0x91
#define MCSD_COMPARATOR_MONITOR_MODE_SET		0x92
#define MCSD_COMPARATOR_MONITOR_MODE_RESET		0x93
#define MCSD_TWO_PHASE_OUTPUT_MODE_SET			0x94
#define MCSD_TWO_PHASE_OUTPUT_MODE_RESET		0x95
#define MCSD_INTERRUPT_SIGNAL_SELECT			0x96


// Device information structure
typedef struct {
    WORD   wId;
    WORD   wIOPortBase;
    WORD   wRomVersion;
} MCSDDEVCONFIG, *PMCSDDEVCONFIG;

// Interrupt information structure
typedef struct {
    DWORD  dwId;
    DWORD  dwAxis;
    DWORD  dwIntFactor;
    DWORD  dwDriveStatus;
    DWORD  dwEndStatus;
    DWORD  dwMechanicalSignal;
    DWORD  dwUniversalSignal;
} MCSDINTINF, *PMCSDINTINF;

// Event set structure
typedef struct {
    WORD   wDrive;
    WORD   wICL;
    WORD   wICG;
    WORD   wECL;
    WORD   wECG;
} MCSDINTFACTOR, *PMCSDINTFACTOR;

// Drive speed set structure
typedef struct {
	DWORD	dwMode;
	DWORD	dwRange;
	DWORD	dwHighSpeed;
	DWORD	dwLowSpeed;
	DWORD	dwRate[3];
	DWORD	dwRateChgPnt[2];
	DWORD	dwScw[2];
	DWORD	dwRearPulse;
} MCSDSPDDATA, *PMCSDSPDDATA;

// Drive speed set structure (PPS)
typedef struct {
	double	dHighSpeed;
	double 	dLowSpeed;
	double 	dAccel;
	double 	dSratio;
} MCSDSPDDATAPPS, *PMCSDSPDDATAPPS;

// Status information structure
typedef struct {
	BYTE    bDrive;
	BYTE    bEnd;
	BYTE    bMechanical;
	BYTE    bUniversal;
} MCSDSTATUS, *PMCSDSTATUS;

// Set origin return table structure
typedef struct {
	DWORD	dwAxis;
	DWORD	dwTable;
	DWORD	dwAction;
	DWORD	dwDir;
	DWORD	dwSPD;
	DWORD	dwEdge;
	DWORD	dwWait;
} MCSDORGRET, *PMCSDORGRET;

//
// Prototype of a DLL function.
//
HANDLE WINAPI McsdOpen( LPCTSTR pszDeviceName, WORD wId);
DWORD  WINAPI McsdClose( HANDLE hDevice);
DWORD  WINAPI McsdReset( HANDLE hDevice);
DWORD  WINAPI McsdGetDeviceConfig( HANDLE hDevice, PMCSDDEVCONFIG pDevConf);
DWORD  WINAPI McsdSetPulseMode(HANDLE hDevice, WORD wAxis, WORD wMode);
DWORD  WINAPI McsdGetPulseMode(HANDLE hDevice, WORD wAxis, PWORD pwMode);
DWORD  WINAPI McsdSetLimit(HANDLE hDevice, WORD wAxis, WORD wLevel);
DWORD  WINAPI McsdGetLimit(HANDLE hDevice, WORD wAxis, PWORD pwLevel);
DWORD  WINAPI McsdSetExternalCounterMode(HANDLE hDevice, WORD wAxis, WORD wInputSpec);
DWORD  WINAPI McsdGetExternalCounterMode(HANDLE hDevice, WORD wAxis, PWORD pwInputSpec);
DWORD  WINAPI McsdSetExternalCounterClear(HANDLE hDevice, WORD wAxis, WORD wRequest, WORD wSignal);
DWORD  WINAPI McsdGetExternalCounterClear(HANDLE hDevice, WORD wAxis, PWORD pwRequest, PWORD pwSignal);
DWORD  WINAPI McsdSetPreScaler(HANDLE hDevice, WORD wAxis, WORD wCounter, DWORD dwData);
DWORD  WINAPI McsdGetPreScaler(HANDLE hDevice, WORD wAxis, WORD wCounter, PDWORD pdwData);
DWORD  WINAPI McsdSetComparator(HANDLE hDevice, WORD wAxis, WORD wCounter, DWORD dwData);
DWORD  WINAPI McsdGetComparator(HANDLE hDevice, WORD wAxis, WORD wCounter, PDWORD pdwData);
DWORD  WINAPI McsdSetSignalStop(HANDLE hDevice, WORD wAxis, WORD wAlmStop, WORD wInPositionWait,WORD wLMT);
DWORD  WINAPI McsdGetSignalStop(HANDLE hDevice, WORD wAxis, PWORD pwAlmStop, PWORD pwInPositionWait, PWORD pwLMT);
DWORD  WINAPI McsdSetSpeed(HANDLE hDevice, WORD wAxis, PMCSDSPDDATA pSpdData);
DWORD  WINAPI McsdGetSpeed(HANDLE hDevice, WORD wAxis, PMCSDSPDDATA pSpdData);
DWORD  WINAPI McsdSetSpeedPPS(HANDLE hDevice, WORD wAxis, PMCSDSPDDATAPPS pSpdDataPPS);
DWORD  WINAPI McsdGetSpeedPPS(HANDLE hDevice, WORD wAxis, PMCSDSPDDATAPPS pSpdDataPPS);
DWORD  WINAPI McsdGetNowSpeed(HANDLE hDevice, WORD wAxis, WORD wKind, double *pdSpeed);
DWORD  WINAPI McsdGetStatus(HANDLE hDevice, WORD wAxis, PMCSDSTATUS pStatus);
DWORD  WINAPI McsdGetAxisBusy(HANDLE hDevice, PWORD pwStatus);
DWORD  WINAPI McsdSetUniversal(HANDLE hDevice, WORD wAxis, BYTE bUniversal);
DWORD  WINAPI McsdSetCounter(HANDLE hDevice, WORD wAxis, WORD wCounter, DWORD dwData);
DWORD  WINAPI McsdGetCounter(HANDLE hDevice, WORD wAxis, WORD wCounter, PDWORD pdwData);
DWORD  WINAPI McsdGetDeviation(HANDLE hDevice, WORD wAxis, PWORD pwData);
DWORD  WINAPI McsdDriveStart(HANDLE hDevice, WORD wAxis, WORD wDrive, DWORD dwPulse);
DWORD  WINAPI McsdDriveStop(HANDLE hDevice, WORD wAxis, WORD wStop);
DWORD  WINAPI McsdSyncStart(HANDLE hDevice, WORD wSyncSignal);
DWORD  WINAPI McsdSignalScan(HANDLE hDevice, WORD wAxis, WORD wScanSpeed, WORD wScanSignal);
DWORD  WINAPI McsdOrgReturn(HANDLE hDevice, WORD wAxis, WORD wAction);
DWORD  WINAPI McsdSetOrgReturn(HANDLE hDevice, PMCSDORGRET pOrgRet);
DWORD  WINAPI McsdHiSpeedOverride(HANDLE hDevice, WORD wAxis, WORD wKind, double dSpeed);
DWORD  WINAPI McsdIndexOverride(HANDLE hDevice, WORD wAxis, DWORD dwData);
DWORD  WINAPI McsdInP( HANDLE hDevice, WORD wPort, PBYTE pbData);
DWORD  WINAPI McsdOutP( HANDLE hDevice, WORD wPort, BYTE bData);
DWORD  WINAPI McsdDataRead( HANDLE hDevice, WORD wAxis, WORD wCommand, PDWORD pdwData);
DWORD  WINAPI McsdDataWrite( HANDLE hDevice, WORD wAxis, WORD wCommand, DWORD dwData);
DWORD  WINAPI McsdSetOfflineMode(HANDLE hDevice, WORD wAxis, WORD wData);
DWORD  WINAPI McsdStartBuffer(HANDLE hDevice, WORD wCount);
DWORD  WINAPI McsdEndBuffer(HANDLE hDevice);

#ifdef __cplusplus
}
#endif
