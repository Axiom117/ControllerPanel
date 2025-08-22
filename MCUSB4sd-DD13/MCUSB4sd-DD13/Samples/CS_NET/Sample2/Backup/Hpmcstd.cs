using System;
using System.Runtime.InteropServices;
//----------------------------------------------------------------------------
//
//	Hpmcstd.cs
//
//	Copyright 2006 HI-P Tech Corporation
//
//----------------------------------------------------------------------------
//	Change history
//
//		V1.00   2006-08-10  New creation.
//----------------------------------------------------------------------------

namespace HpmcstdCs
{

	public class Hpmcstd
	{
		//constructor
		public Hpmcstd() {}

		// error code
		public const uint MCSD_ERROR_SUCCESS		= 0;
		public const uint MCSD_ERROR_SYSTEM			= 1001;
		public const uint MCSD_ERROR_NO_DEVICE		= 1002;
		public const uint MCSD_ERROR_IN_USE			= 1003;
		public const uint MCSD_ERROR_ID				= 1004;
		public const uint MCSD_ERROR_AXIS			= 1005;
		public const uint MCSD_ERROR_PORT			= 1006;
		public const uint MCSD_ERROR_PARAMETER		= 1007;
		public const uint MCSD_ERROR_PROC			= 1008;
		public const uint MCSD_ERROR_CALLBACK		= 1009;
		public const uint MCSD_ERROR_HANDLE			= 1010;
		public const uint MCSD_ERROR_USB_TRANS		= 1011;
		public const uint MCSD_ERROR_USB_RECEIVE	= 1012;
		public const uint MCSD_ERROR_USB_OFFLINE	= 1013;
		public const uint MCSD_ERROR_ORG_RETURN		= 1014;

		// MCSD I/O Map
		public const uint MCSD_PORT_DATA1					= 0x00;
		public const uint MCSD_PORT_DATA2					= 0x01;
		public const uint MCSD_PORT_DATA3					= 0x02;
		public const uint MCSD_PORT_DATA4					= 0x03;
		public const uint MCSD_PORT_COMMAND					= 0x04;
		public const uint MCSD_PORT_MODE_A					= 0x05;
		public const uint MCSD_PORT_MODE_B					= 0x06;
		public const uint MCSD_PORT_UNIVERSAL_SIGNAL		= 0x07;
		public const uint MCSD_PORT_DRIVE_STATUS			= 0x04;
		public const uint MCSD_PORT_END_STATUS				= 0x05;
		public const uint MCSD_PORT_MECHANICAL_SIGNAL		= 0x06;

		// Commands
		public const uint MCSD_RANGE_DATA_WRITE						= 0x00;
		public const uint MCSD_RANGE_DATA_READ						= 0x01;
		public const uint MCSD_LOW_SPEED_DATA_WRITE					= 0x02;
		public const uint MCSD_LOW_SPEED_DATA_READ					= 0x03;
		public const uint MCSD_HIGH_SPEED_DATA_WRITE				= 0x04;
		public const uint MCSD_HIGH_SPEED_DATA_READ					= 0x05;
		public const uint MCSD_RATE_A_DATA_WRITE					= 0x06;
		public const uint MCSD_RATE_A_DATA_READ						= 0x07;
		public const uint MCSD_RATE_B_DATA_WRITE					= 0x08;
		public const uint MCSD_RATE_B_DATA_READ						= 0x09;
		public const uint MCSD_RATE_C_DATA_WRITE					= 0x0A;
		public const uint MCSD_RATE_C_DATA_READ						= 0x0B;
		public const uint MCSD_RATE_CHANGE_POINT_A_B_WRITE			= 0x0C;
		public const uint MCSD_RATE_CHANGE_POINT_A_B_READ			= 0x0D;
		public const uint MCSD_RATE_CHANGE_POINT_B_C_WRITE			= 0x0E;
		public const uint MCSD_RATE_CHANGE_POINT_B_C_READ			= 0x0F;
		public const uint MCSD_SLOW_DOWN_REAR_PULSE_DATA_WRITE		= 0x10;
		public const uint MCSD_SLOW_DOWN_REAR_PULSE_DATA_READ		= 0x11;
		public const uint MCSD_NOW_SPEED_DATA_READ					= 0x12;
		public const uint MCSD_DRIVE_PULSE_COUNTER_READ				= 0x13;
		public const uint MCSD_INDEX_PULSE_DATA_OVERRIDE			= 0x14;
		public const uint MCSD_INDEX_PULSE_DATA_READ				= 0x15;
		public const uint MCSD_DEVIATION_DATA_READ					= 0x16;
		public const uint MCSD_INPOSITION_WAIT_MODE_A_SET			= 0x17;
		public const uint MCSD_INPOSITION_WAIT_MODE_B_SET			= 0x18;
		public const uint MCSD_INPOSITION_WAIT_MODE_RESET			= 0x19;
		public const uint MCSD_ALARM_STOP_ENABLE_MODE_SET			= 0x1A;
		public const uint MCSD_ALARM_STOP_ENABLE_MODE_RESET			= 0x1B;
		public const uint MCSD_INTERRUPT_OUT_ENABLE_MODE_SET		= 0x1C;
		public const uint MCSD_INTERRUPT_OUT_ENABLE_MODE_RESET		= 0x1D;
		public const uint MCSD_SLOW_DOWN_STOP						= 0x1E;
		public const uint MCSD_EMERGENCY_STOP						= 0x1F;
		public const uint MCSD_PLUS_INDEX_PULSE_DRIVE				= 0x20;
		public const uint MCSD_MINUS_INDEX_PULSE_DRIVE				= 0x21;
		public const uint MCSD_PLUS_SCAN_DRIVE						= 0x22;
		public const uint MCSD_MINUS_SCAN_DRIVE						= 0x23;
		public const uint MCSD_PLUS_SIGNAL_SCAN_A_DRIVE				= 0x24;
		public const uint MCSD_MINUS_SIGNAL_SCAN_A_DRIVE			= 0x25;
		public const uint MCSD_PLUS_SIGNAL_SCAN_B_DRIVE				= 0x26;
		public const uint MCSD_MINUS_SIGNAL_SCAN_B_DRIVE			= 0x27;
		public const uint MCSD_INTERNAL_COUNTER_WRITE				= 0x28;
		public const uint MCSD_INTERNAL_COUNTER_READ				= 0x29;
		public const uint MCSD_INTERNAL_COMPARATE_DATA_WRITE		= 0x2A;
		public const uint MCSD_INTERNAL_COMPARATE_DATA_READ			= 0x2B;
		public const uint MCSD_EXTERNAL_COUNTER_WRITE				= 0x2C;
		public const uint MCSD_EXTERNAL_COUNTER_READ				= 0x2D;
		public const uint MCSD_EXTERNAL_COMPARATE_DATA_WRITE		= 0x2E;
		public const uint MCSD_EXTERNAL_COMPARATE_DATA_READ			= 0x2F;
		public const uint MCSD_INTERNAL_PRE_SCALE_DATA_WRITE		= 0x30;
		public const uint MCSD_INTERNAL_PRE_SCALE_DATA_READ			= 0x31;
		public const uint MCSD_EXTERNAL_PRE_SCALE_DATA_WRITE		= 0x32;
		public const uint MCSD_EXTERNAL_PRE_SCALE_DATA_READ			= 0x33;
		public const uint MCSD_CLEAR_SIGNAL_SELECT					= 0x34;
		public const uint MCSD_ONE_TIME_CLEAR_REQUEST				= 0x35;
		public const uint MCSD_FULL_TIME_CLEAR_REQUEST				= 0x36;
		public const uint MCSD_CLEAR_REQUEST_RESET					= 0x37;
		public const uint MCSD_REVERSE_COUNT_MODE_SET				= 0x38;
		public const uint MCSD_REVERSE_COUNT_MODE_RESET				= 0x39;
		public const uint MCSD_STRAIGHT_ACCELERATE_MODE_SET			= 0x84;
		public const uint MCSD_ASYM_STRAIGHT_ACCELERATE_MODE_SET	= 0x85;
		public const uint MCSD_S_CURVE_ACCELERATE_MODE_SET			= 0x86;
		public const uint MCSD_ASYM_S_CURVE_ACCELERATE_MODE_SET		= 0x87;
		public const uint MCSD_SCW_A_DATA_WRITE						= 0x88;
		public const uint MCSD_SCW_A_DATA_READ						= 0x89;
		public const uint MCSD_SCW_B_DATA_WRITE						= 0x8A;
		public const uint MCSD_SCW_B_DATA_READ						= 0x8B;
		public const uint MCSD_SLOW_DOWN_LIMIT_ENABLE_MODE_SET		= 0x8C;
		public const uint MCSD_SLOW_DOWN_LIMIT_ENABLE_MODE_RESET	= 0x8D;
		public const uint MCSD_EMERGENCY_LIMIT_ENABLE_MODE_SET		= 0x8E;
		public const uint MCSD_EMERGENCY_LIMIT_ENABLE_MODE_RESET	= 0x8F;
		public const uint MCSD_INITIAL_CLEAR						= 0x90;
		public const uint MCSD_SYNCHRONOUS_SIGNAL_SELECT			= 0x91;
		public const uint MCSD_COMPARATOR_MONITOR_MODE_SET			= 0x92;
		public const uint MCSD_COMPARATOR_MONITOR_MODE_RESET		= 0x93;
		public const uint MCSD_TWO_PHASE_OUTPUT_MODE_SET			= 0x94;
		public const uint MCSD_TWO_PHASE_OUTPUT_MODE_RESET			= 0x95;
		public const uint MCSD_INTERRUPT_SIGNAL_SELECT				= 0x96;

		// Interrupt Callback Function
		public delegate void MCSDCALLBACK(ref MCSDINTINF pIntInf);

		// Device information structure
		[StructLayout(LayoutKind.Sequential)]
		public struct MCSDDEVCONFIG 
		{
			public ushort	wId;
			public ushort	wIOPortBase;
			public ushort	wRomVersion;
		}

		// Interrupt information structure
		[StructLayout(LayoutKind.Sequential)]
		public struct MCSDINTINF 
		{
			public uint		dwId;
			public uint		dwAxis;
			public uint		dwIntFactor;
			public uint		dwDriveStatus;
			public uint		dwEndStatus;
			public uint		dwMechanicalSignal;
			public uint		dwUniversalSignal;
		}

		// Event set structure
		[StructLayout(LayoutKind.Sequential)]
		public struct MCSDINTFACTOR 
		{
			public ushort	wDrive;
			public ushort	wICL;
			public ushort	wICG;
			public ushort	wECL;
			public ushort	wECG;
		}

		// Drive speed set structure
		[StructLayout(LayoutKind.Sequential)]
		public struct MCSDSPDDATA 
		{
			public uint		dwMode;
			public uint		dwRange;
			public uint		dwHighSpeed;
			public uint		dwLowSpeed;
			[MarshalAs(UnmanagedType.ByValArray, SizeConst=3)]
			public uint[]	dwRate;
			[MarshalAs(UnmanagedType.ByValArray, SizeConst=2)]
			public uint[]	dwRateChgPnt;
			[MarshalAs(UnmanagedType.ByValArray, SizeConst=2)]
			public uint[]	dwScw;
			public uint		dwRearPulse;
			//
			public void InitSTR()
			{
				dwRate			= new uint[3];
				dwRateChgPnt	= new uint[2];
				dwScw			= new uint[2];
			}
		}

		// Drive speed set structure (PPS)
		[StructLayout(LayoutKind.Sequential)]
		public struct MCSDSPDDATAPPS 
		{
			public double	dHighSpeed;
			public double	dLowSpeed;
			public double	dAccel;
			public double	dSratio;
		}

		// Status information structure
		[StructLayout(LayoutKind.Sequential)]
		public struct MCSDSTATUS 
		{
			public byte		bDrive;
			public byte		bEnd;
			public byte		bMechanical;
			public byte		bUniversal;
		}

		// Set origin return table structure
		[StructLayout(LayoutKind.Sequential)]	
		public struct MCSDORGRET 
		{
			public uint		dwAxis;
			public uint		dwTable;
			public uint		dwAction;
			public uint		dwDir;
			public uint		dwSPD;
			public uint		dwEdge;
			public uint		dwWait;
		}

		//-----------------------------
		// Hpmcstd.dll Function
		//-----------------------------
		[DllImport("Hpmcstd.dll")]
			public static extern uint McsdOpen( string pszDeviceName, ushort wId);
		[DllImport("Hpmcstd.dll")]
			public static extern uint McsdClose( uint hDevice);
		[DllImport("Hpmcstd.dll")]
			public static extern uint McsdReset( uint hDevice);
		[DllImport("Hpmcstd.dll")]
			public static extern uint McsdGetDeviceConfig( uint hDevice, out MCSDDEVCONFIG pDevConf);
		[DllImport("Hpmcstd.dll")]
			public static extern uint McsdSetPulseMode(uint hDevice, ushort wAxis, ushort wMode);
		[DllImport("Hpmcstd.dll")]	
			public static extern uint McsdGetPulseMode(uint hDevice, ushort wAxis, out ushort pwMode);
		[DllImport("Hpmcstd.dll")]
			public static extern uint McsdSetLimit(uint hDevice, ushort wAxis, ushort wLevel);
		[DllImport("Hpmcstd.dll")]
			public static extern uint McsdGetLimit(uint hDevice, ushort wAxis, out ushort pwLevel);
		[DllImport("Hpmcstd.dll")]
			public static extern uint McsdSetExternalCounterMode(uint hDevice, ushort wAxis, ushort wInputSpec);
		[DllImport("Hpmcstd.dll")]
			public static extern uint McsdGetExternalCounterMode(uint hDevice, ushort wAxis, out ushort pwInputSpec);
		[DllImport("Hpmcstd.dll")]
			public static extern uint McsdSetExternalCounterClear(uint hDevice, ushort wAxis, ushort wRequest, ushort wSignal);
		[DllImport("Hpmcstd.dll")]
			public static extern uint McsdGetExternalCounterClear(uint hDevice, ushort wAxis, out ushort pwRequest, out ushort pwSignal);
		[DllImport("Hpmcstd.dll")]
			public static extern uint McsdSetPreScaler(uint hDevice, ushort wAxis, ushort wCounter, uint dwData);
		[DllImport("Hpmcstd.dll")]
			public static extern uint McsdGetPreScaler(uint hDevice, ushort wAxis, ushort wCounter, out uint pdwData);
		[DllImport("Hpmcstd.dll")]
			public static extern uint McsdSetComparator(uint hDevice, ushort wAxis, ushort wCounter, uint dwData);
		[DllImport("Hpmcstd.dll")]
			public static extern uint McsdGetComparator(uint hDevice, ushort wAxis, ushort wCounter, out uint pdwData);
		[DllImport("Hpmcstd.dll")]
			public static extern uint McsdSetSignalStop(uint hDevice, ushort wAxis, ushort wAlmStop, ushort wInPositionWait,ushort wLMT);
		[DllImport("Hpmcstd.dll")]
			public static extern uint McsdGetSignalStop(uint hDevice, ushort wAxis, out ushort pwAlmStop, out ushort pwInPositionWait, out ushort pwLMT);
		[DllImport("Hpmcstd.dll")]
			public static extern uint McsdSetSpeed(uint hDevice, ushort wAxis, ref MCSDSPDDATA pSpdData);
		[DllImport("Hpmcstd.dll")]
			public static extern uint McsdGetSpeed(uint hDevice, ushort wAxis, out MCSDSPDDATA pSpdData);
		[DllImport("Hpmcstd.dll")]
			public static extern uint McsdSetSpeedPPS(uint hDevice, ushort wAxis, ref MCSDSPDDATAPPS pSpdDataPPS);
		[DllImport("Hpmcstd.dll")]
			public static extern uint McsdGetSpeedPPS(uint hDevice, ushort wAxis, out MCSDSPDDATAPPS pSpdDataPPS);
		[DllImport("Hpmcstd.dll")]
			public static extern uint McsdGetNowSpeed(uint hDevice, ushort wAxis, ushort wKind, out double pdSpeed);
		[DllImport("Hpmcstd.dll")]
			public static extern uint McsdGetStatus(uint hDevice, ushort wAxis, out MCSDSTATUS pStatus);
		[DllImport("Hpmcstd.dll")]
			public static extern uint McsdGetAxisBusy(uint hDevice, out ushort pwStatus);
		[DllImport("Hpmcstd.dll")]
			public static extern uint McsdSetUniversal(uint hDevice, ushort wAxis, byte bUniversal);
		[DllImport("Hpmcstd.dll")]
			public static extern uint McsdSetCounter(uint hDevice, ushort wAxis, ushort wCounter, uint dwData);
		[DllImport("Hpmcstd.dll")]
			public static extern uint McsdGetCounter(uint hDevice, ushort wAxis, ushort wCounter, out uint pdwData);
		[DllImport("Hpmcstd.dll")]
			public static extern uint McsdGetDeviation(uint hDevice, ushort wAxis, out ushort pwData);
		[DllImport("Hpmcstd.dll")]
			public static extern uint McsdDriveStart(uint hDevice, ushort wAxis, ushort wDrive, uint dwPulse);
		[DllImport("Hpmcstd.dll")]
			public static extern uint McsdDriveStop(uint hDevice, ushort wAxis, ushort wStop);
		[DllImport("Hpmcstd.dll")]
			public static extern uint McsdSyncStart(uint hDevice, ushort wSyncSignal);
		[DllImport("Hpmcstd.dll")]
			public static extern uint McsdSignalScan(uint hDevice, ushort wAxis, ushort wScanSpeed, ushort wScanSignal);
		[DllImport("Hpmcstd.dll")]
			public static extern uint McsdOrgReturn(uint hDevice, ushort wAxis, ushort wAction);
		[DllImport("Hpmcstd.dll")]
			public static extern uint McsdSetOrgReturn(uint hDevice, ref MCSDORGRET pOrgRet);
		[DllImport("Hpmcstd.dll")]
			public static extern uint McsdHiSpeedOverride(uint hDevice, ushort wAxis, ushort wKind, double dSpeed);
		[DllImport("Hpmcstd.dll")]
			public static extern uint McsdIndexOverride(uint hDevice, ushort wAxis, uint dwData);
		[DllImport("Hpmcstd.dll")]
			public static extern uint McsdIntEvent(uint hDevice, int hWnd, uint uMessage, uint hEvent, MCSDCALLBACK pfnCallBack);
		[DllImport("Hpmcstd.dll")]
			public static extern uint McsdEventEnable( uint hDevice, ushort wAxis, ref MCSDINTFACTOR pIntFactor);
		[DllImport("Hpmcstd.dll")]
			public static extern uint McsdGetIntFactor( uint hDevice, ushort wAxis, out MCSDINTINF pIntInf);
		[DllImport("Hpmcstd.dll")]
			public static extern uint McsdInP( uint hDevice, ushort wPort, out byte pbData);
		[DllImport("Hpmcstd.dll")]
			public static extern uint McsdOutP( uint hDevice, ushort wPort, byte bData);
		[DllImport("Hpmcstd.dll")]
			public static extern uint McsdDataRead( uint hDevice, ushort wAxis, ushort wCommand, out uint pdwData);
		[DllImport("Hpmcstd.dll")]
			public static extern uint McsdDataWrite( uint hDevice, ushort wAxis, ushort wCommand, uint dwData);
		[DllImport("Hpmcstd.dll")]
			public static extern uint McsdGetUsbVersion( uint hDevice, out ushort pwVersion);
		[DllImport("Hpmcstd.dll")]
			public static extern uint McsdSetOfflineMode( uint hDevice, ushort wAxis, ushort wData);
		[DllImport("Hpmcstd.dll")]
			public static extern uint McsdStartBuffer( uint hDevice, ushort wCount);
		[DllImport("Hpmcstd.dll")]
			public static extern uint McsdEndBuffer( uint hDevice);
	}

}
