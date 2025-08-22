///
/// Copyright (c) 2016, Micro Support Co.,Ltd.
/// Web: http://www.microsupport.co.jp/
///
/// MotionController Class
/// 
///----------------------------------------------------------------------------
///	history
/// 
/// version 1.0.0.0     2017/03/24
///                     New creation.
///                     Autherd by Seki,Tomomasa
/// version 2.0.0.0     2025/05/21
///                     Documentation update and refactoring.
///                     Modified by Haoran, Yao
///----------------------------------------------------------------------------

using System;
using HpmcstdCs;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;

namespace MicrosupportController
{
    /// <summary>
    /// Represents a motion control system for managing and operating multiple axes in a three-dimensional coordinate
    /// system.
    /// </summary>
    /// <remarks>The <see cref="Microsupport"/> class provides methods and properties to control motion
    /// controllers,  including initialization, movement, speed control, and status monitoring. It supports operations
    /// such as  homing, jogging, and absolute or incremental movements for X, Y, and Z axes. The class ensures thread
    /// safety  when accessing shared resources and integrates with external libraries for low-level hardware
    /// communication.</remarks>
    public class Microsupport
    {
        /// A static object used to synchronize access to shared resources in a thread-safe manner.
        private static readonly object instanceLock = new object();
        
        #region Class properties and variables


        /// Handler of the motion controller.
        private uint hController = 0xFFFFFFFF;

        private bool _isInitialized = false;
        private string _initializationError = "";

        public bool IsInitialized => _isInitialized;

        public string InitializationError => _initializationError;

        /// <summary>
        /// microsupportInstances are static dictionary of Microsupport objects. It stores the reference of instances of Microsupport, instead of the instance itself. Useing deviceID as key, we can have multiple instances for different devices
        /// </summary>
        public static readonly Dictionary<string, Microsupport> controllers = new Dictionary<string, Microsupport>();

        /// Keep track of controllers that have reported errors to avoid duplicate error messages.
        private static readonly HashSet<ushort> errorReportedControllers = new HashSet<ushort>();

        #endregion

        #region Constants and Enumerators

        /// <summary>
        /// maximum number of axes supported by the controller.
        /// </summary>
        public const int MC104_MAX_AXES = 4;

        /// <summary>
        /// axis code.
        /// </summary>
        public const ushort MC104_AXIS1 = 0;
        public const ushort MC104_AXIS2 = 1;
        public const ushort MC104_AXIS3 = 2;

        /// <summary>
        /// driving modes.
        /// </summary>
        public const ushort MC104_INDEX_FORWARD = 0; // INC
        public const ushort MC104_INDEX_REVERSE = 1;
        public const ushort MC104_SCAN_FORWARD = 2; // JOG
        public const ushort MC104_SCAN_REVERSE = 3;

        /// <summary>
        /// homming patterns.
        /// </summary>
        public const ushort MC104_ORG_0 = 0; // ユーザー定義原点復帰モード
        public const ushort MC104_ORG_1 = 1; // 原点信号の－側エッジ検出
        public const ushort MC104_ORG_2 = 2; // 原点信号の－側エッジ検出後、Z相＋側立ち上がりエッジ検出
        public const ushort MC104_ORG_3 = 3; // －リミット信号のエッジ検出
        public const ushort MC104_ORG_4 = 4; // ＋リミット信号のエッジ検出
        public const ushort MC104_ORG_5 = 5; // －リミット信号のエッジ検出後、Z相＋側立ち上がりエッジ検出
        public const ushort MC104_ORG_6 = 6; // ＋リミット信号のエッジ検出後、Z相－側立ち上がりエッジ検出
        public const ushort MC104_ORG_7 = 7; // －Z相エッジ検出
        public const ushort MC104_ORG_8 = 8; // 原点信号の＋側エッジ検出
        public const ushort MC104_ORG_9 = 9; // 原点信号の＋側エッジ検出後、Z相－側立ち上がりエッジ検出

        /// <summary>
        /// resolution of each axis (um/pulse)
        /// </summary>
        private const double RESOLUTIONS_AXIS_X = 0.05;
        private const double RESOLUTIONS_AXIS_Y = 0.05;
        private const double RESOLUTIONS_AXIS_Z = 0.05;

        /// <summary>
        /// Represents the axes in a three-dimensional coordinate system.
        /// </summary>
        public enum AXIS
        {
            X,
            Y,
            Z,
        }

        /// <summary>
        /// Represents the direction of movement or operation.
        /// </summary>
        public enum DIRECTION
        {
            FORWARD,
            REVERSE
        }

        /// <summary>
        /// max speed (pulse/sec)
        /// </summary>
        private const int MAX_SPEED = 50000;        // X軸, Y軸, Z軸
        private const double MAX_UM_SPEED = MAX_SPEED * RESOLUTIONS_AXIS_X;        // X軸, Y軸, Z軸
        private const double SPEED_DEFAULT = 1000;

        /// Range of movement for each axis in micrometers (um).
        private const double RANGE_X = 20000; // X軸の移動範囲 (um)
        private const double RANGE_Y = 20000; // Y軸の移動範囲 (um)
        private const double RANGE_Z = 30000; // Z軸の移動範囲 (um)

        private string comment = "";

        private int accelerationTimeMs = 100; // Acceleration time in milliseconds for each axis.
        private bool isSmoothingEnabled = false; // Flag to enable or disable smoothing.

        #endregion

        #region Class constructors
        /// <summary>
        /// Retrieves a singleton instance of the <see cref="Microsupport"/> class for the specified device ID.
        /// </summary>
        /// <remarks>This method is thread-safe. If multiple threads attempt to retrieve an instance for
        /// the same <paramref name="deviceId"/> simultaneously, only one instance will be created and shared.</remarks>
        /// <param name="deviceId">The unique identifier of the device for which the <see cref="Microsupport"/> instance is requested.</param>
        /// <returns>The <see cref="Microsupport"/> instance associated with the specified <paramref name="deviceId"/>. If no
        /// instance exists for the given device ID, a new instance is created and returned.</returns>
        public static Microsupport GetInstance(string name, int deviceId)
        {
            /// lock the instanceLock to ensure thread safety when accessing the microsupportInstances dictionary.
            lock (instanceLock)
            {
                /// check if the deviceId already exists in the dictionary.
                if (!controllers.ContainsKey(name))
                {
                    /// Create a new instance of Microsupport and add it to the dictionary.
                    var instance = new Microsupport((ushort)deviceId);
                    
                    if (!instance.IsInitialized)
                    {
                        //if (!errorReportedControllers.Contains((ushort)deviceId))
                        //{
                        //    /// Log the error only once for each device ID.
                        //    Console.WriteLine($"Error initializing Microsupport for device {name}: {instance.InitializationError}");
                        //    errorReportedControllers.Add((ushort)deviceId);
                        //}
                        return null; // or handle the error as needed
                    }

                    controllers[name] = instance; // Add the new instance to the dictionary.
                }
                /// return the instance associated with the deviceId.
                return controllers[name];
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Microsupport"/> class with the specified identifier.
        /// </summary>
        /// <remarks>The constructor initializes the Microsupport instance by performing necessary setup
        /// operations,  including initializing internal components and data structures.</remarks>
        /// <param name="ID">The unique identifier for the Microsupport instance. Must be a valid unsigned 16-bit integer.</param>
        public Microsupport(ushort ID)
        {
            Initialize(ID);
        }

        /// <summary>
        /// when the controller is opened successfully, hController should not be 0xFFFFFFFF
        /// </summary>
        public bool IsValid
        {
            get { return (hController != 0xFFFFFFFF) ? true : false; }
        }

        /// <summary>
        /// initialize the motion controller. This method depends on the external DLL functions defined in Hpmcstd class. These functions are imported from the Hpmcstd.dll dynamic link library.
        /// </summary>

        /// <summary>
        /// Checks if the controller is still connected by querying the hardware status.
        /// </summary>
        /// <returns>True if the controller is connected; otherwise, false.</returns>
        public bool IsConnected()
        {
            if (!this.IsValid)
            {
                return false;
            }

            /// Queries the status of the controller for axis 1 (MC104_AXIS1) and retrieves the status code.
            uint status = Hpmcstd.McsdGetStatus(hController, MC104_AXIS2, out _);
            return status == Hpmcstd.MCSD_ERROR_SUCCESS;
        }

        public void Initialize(ushort ID)
        {
            try
            {
                /// opens the device with the specified ID.
                hController = Hpmcstd.McsdOpen("MCUSB4sd", ID);  //  Hpmcstd.McsdOpen("MCUSB4sd", ID) is calling the Hpmcstd library to open the motion controller with the specified ID.

                /// Check if the controller is opened successfully. hController should not be 0xFFFFFFFF.
                if (this.IsValid)
                {
                    /// err is a variable that stores the error code returned by the Hpmcstd library functions.
                    int err = 0;
                    /// Set the speed of each axe to the default speed.
                    for (ushort i = 0; i < MC104_MAX_AXES; i++)
                    {
                        /// set the pulse mode
                        // set the pulse output mode to 2 pulse mode
                        // DIR   output port 	CW pulse  active High
                        // PULSE output port	CCW pulse active High
                        err = (int)Hpmcstd.McsdSetPulseMode(hController, i, 4);

                        /// set the limit signal to active high
                        // +LMT		  input signal active level	High
                        // -LMT		  input signal active level	High
                        // ALARM	  input signal active level	High
                        // INPOSITION input signal active level	High
                        err = (int)Hpmcstd.McsdSetLimit(hController, i, 0x00);

                        /// set the signal stop mode
                        // アラーム信号       無効
                        // 位置決め完了信号   無効
                        // リミット信号検出時 急停止
                        err = (int)Hpmcstd.McsdSetSignalStop(hController, i, 0, 0, 2);
                    }

                    _isInitialized = true; // Set the initialization flag to true.
                    _initializationError = ""; // Clear any previous initialization error message.
                }
                else
                {
                    _isInitialized = false; // Set the initialization flag to false.
                    _initializationError = $"Failed to open controller with ID {ID}. Device not found or unavailable.";
                }
            }
            catch (Exception ex)
            {
                _isInitialized = false;
                _initializationError = $"Exception during initialization: {ex.Message}";
                hController = 0xFFFFFFFF;
            }
        }

        #endregion

        #region public methods

        /// <summary>
        /// Initiates the origin start process for the specified axis with the given action.
        /// </summary>
        /// <param name="axis">The axis identifier for which the origin start process is initiated.</param>
        /// <param name="action">The action to perform during the origin start process. The valid range and meaning of this value depend on
        /// the device's specifications.</param>
        /// <returns>A status code indicating the result of the operation. Returns a device-specific success code if the
        /// operation succeeds, or <see cref="Hpmcstd.MCSD_ERROR_NO_DEVICE"/> if no valid device is connected.</returns>
        public uint StartOrigin(ushort axis, ushort action)
        {
            if (this.IsValid)
            {
                /// Initiates the origin start process for the specified axis with the given action.
                return Hpmcstd.McsdOrgReturn(hController, axis, action);
            }
            return Hpmcstd.MCSD_ERROR_NO_DEVICE;
        }

        /// <summary>
        /// Initiates the homing process for all axes and waits for the operation to complete asynchronously.
        /// </summary>
        /// <remarks>This method sets the speed for the X, Y, and Z axes to their maximum values before
        /// starting the homing process. It then waits until all axes have completed their homing operation. The method
        /// is asynchronous and will not block the calling thread while waiting for the operation to complete.</remarks>
        /// <returns>A task that represents the asynchronous operation. The task completes when the homing process for all axes
        /// is finished.</returns>
        public async Task StartOriginAsync()
        {
            /// Set the speed for each axis to the maximum speed.
            SetSpeed(AXIS.X, MAX_UM_SPEED);
            SetSpeed(AXIS.Y, MAX_UM_SPEED);
            SetSpeed(AXIS.Z, MAX_UM_SPEED);

            /// Start the homing process for each axis using the specified action (MC104_ORG_3).
            StartOrigin(MC104_AXIS1, MC104_ORG_3);
            StartOrigin(MC104_AXIS2, MC104_ORG_3);
            StartOrigin(MC104_AXIS3, MC104_ORG_3);

            /// Wait for the homing process to complete.
            while (IsBusy())
            {
                /// Wait for a short period to avoid busy waiting.
                await Task.Delay(100);
            }
        }

        public async Task StartCenter()
        {
            /// Set the speed for each axis to the maximum speed.
            SetSpeed(AXIS.X, MAX_UM_SPEED);
            SetSpeed(AXIS.Y, MAX_UM_SPEED);
            SetSpeed(AXIS.Z, MAX_UM_SPEED);

            /// Center the axes by moving them to the middle of their range.
            double centerX = RANGE_X / 2;
            double centerY = RANGE_Y / 2;
            double centerZ = RANGE_Z / 2;
            /// Move each axis to its center position.
            StartAbs(AXIS.X, centerX);
            StartAbs(AXIS.Y, centerY);
            StartAbs(AXIS.Z, centerZ);

            /// Wait for the centering process to complete.
            while (IsBusy())
            {
                /// Wait for a short period to avoid busy waiting.
                await Task.Delay(100);
            }
        }

        /// Enable or disable smoothing mode for the motion controller.
        public void SetSmoothMode(bool enable)
        {
            isSmoothingEnabled = enable;
        }

        /// Set acceleration time for smooth motion.
        public void SetAccelerationTime(int timeMs)
        {
            accelerationTimeMs = Math.Max(1, Math.Min(timeMs, 1000)); // Ensure the time is between 1 and 1000 ms.
        }

        /// <summary>
        /// Waits asynchronously until the operation is no longer busy.
        /// </summary>
        /// <remarks>This method repeatedly checks the <see cref="IsBusy"/> state and delays execution  in
        /// 10-millisecond intervals until the operation completes. Use this method to  ensure that subsequent actions
        /// occur only after the busy state has ended.</remarks>
        /// <returns>A task that represents the asynchronous wait operation.</returns>
        public async Task Wait()
        {
            while (IsBusy())
                await Task.Delay(10);
        }

        /// <summary>
        /// terminate the motion controller. This method closes the device and releases any resources associated with it.
        /// </summary>
        public void Terminate()
        {
            if (this.IsValid)
                Hpmcstd.McsdClose(hController);
        }

        /// <summary>
        /// Convert encoder value to um value.
        /// </summary>
        /// <param name="axis"></param>
        /// <param name="enc"></param>
        /// <returns></returns>
        public int Enc2um(AXIS axis, double enc)
        {
            switch (axis)
            {
                case AXIS.X:
                    return (int)(enc * RESOLUTIONS_AXIS_X);
                case AXIS.Y:
                    return (int)(enc * RESOLUTIONS_AXIS_Y);
                case AXIS.Z:
                    return (int)(enc * RESOLUTIONS_AXIS_Z);
                default:
                    return 0;
            }
        }

        /// <summary>
        /// Converts a distance in micrometers (µm) to encoder units for the specified axis.
        /// </summary>
        /// <param name="axis"></param>
        /// <param name="um"></param>
   
        public int Um2enc(AXIS axis, double um)
        {
            switch (axis)
            {
                case AXIS.X:
                    return (int)(um / RESOLUTIONS_AXIS_X);
                case AXIS.Y:
                    return (int)(um / RESOLUTIONS_AXIS_Y);
                case AXIS.Z:
                    return (int)(um / RESOLUTIONS_AXIS_Z);
                default:
                    return 0;
            }
        }

        /// <summary>
        /// Obtains the current positions (pulse) of the controller in encoder units.
        /// </summary>
        /// <return> An array of integers, where each element represents the position in encoder units </return>
        public int[] GetPositionsEnc()
        {
            int[] pos = null;

            if (this.IsValid)
            {
                pos = new int[MC104_MAX_AXES];
                for (ushort i = 0; i < MC104_MAX_AXES; i++)
                {
                    uint dwData;
                    /// the keyword "out" here is used to indicate that the dwData variable is passed by reference, allowing the method to modify its value.
                    uint err = Hpmcstd.McsdGetCounter(hController, i, 0, out dwData);
                    /// This bitwise operation on a 32-bit unsigned integer is for extracting the position data. The internal counter is 28-bits, hence the upper 4 bits are not used.
                    if ((dwData & 0x08000000) == 0x08000000)
                    {
                        /// This bitwise OR operation sets the top 4 bits of the dwData variable to 1, if bit 27 is 1.
                        dwData |= 0xF0000000; 
                    }

                    pos[i] = (int)dwData;
                }
            }

            return pos;
        }

        /// <summary>
        /// Obtains the current position (um) of the controller in encoder units.
        /// </summary>
        /// <return> An array of integers, where each element represents the position in micrometers </return>
        public double[] GetPositions()
        {
            double[] posum = null;
            int[] pos = GetPositionsEnc();
            if( pos != null )
            {
                posum = new double[3];
                posum[0] = pos[1] * RESOLUTIONS_AXIS_X;
                posum[1] = pos[0] * RESOLUTIONS_AXIS_Y;
                posum[2] = pos[2] * RESOLUTIONS_AXIS_Z;
            }    
            return posum;
        }

        public double[] GetPositionsFromCenter()
        {
            double[] posAbs = GetPositions();
            double[] posFC = null;
            if (posAbs != null)
            {
                posFC = new double[3];
                posFC[0] = posAbs[0] - RANGE_X / 2; // X軸の中心からの距離
                posFC[1] = posAbs[1] - RANGE_Y / 2; // Y軸の中心からの距離
                posFC[2] = - posAbs[2] + RANGE_Z / 2; // Z軸の中心からの距離
            }
                return posFC;
        }

        /// <summary>
        /// Retrieves the encoder position (pulse) for the specified axis.
        /// </summary>
        /// <param name="axis">The axis for which to retrieve the encoder position. Must be one of the defined <see cref="AXIS"/> values.</param>
        public int GetPositionEnc(AXIS axis)
        {
            int[] pos = GetPositionsEnc();
            switch (axis)
            {
                case AXIS.X:
                    return pos[1];
                case AXIS.Y:
                    return pos[0];  
                case AXIS.Z:
                    return pos[2];
                default:
                    return 0;
            }
        }

        /// <summary>
        /// Retrieves the current position of the specified axis in micrometers.
        /// </summary>
        /// <param name="axis"></param>
        /// <returns>The position of the specified axis, expressed in micrometers.</returns>
        /// <returns>The position of the specified axis, expressed in micrometers.</returns>
        public double GetPosition(AXIS axis)
        {
            int pos = GetPositionEnc(axis);
            return Enc2um(axis, pos);
        }

        /// <summary>
        /// Sets the speed for the specified axis in encoder units.
        /// </summary>
        /// <remarks>The method ensures that the speed is set within the valid range for the specified
        /// axis. If the device is not valid, the method will return an error code without performing any operation. The
        /// speed is adjusted internally to ensure that the high speed is not lower than the low speed.</remarks>
        /// <param name="axis"></param>
        /// <param name="speed"></param>
        /// <returns>A status code indicating the result of the operation. Returns <see cref="Hpmcstd.MCSD_ERROR_NO_DEVICE"/> if
        /// the device is not valid, <see cref="Hpmcstd.MCSD_ERROR_AXIS"/> if the specified axis is invalid, or a
        /// success code from the underlying hardware API.</returns>
        public uint SetSpeedEnc(AXIS axis, int speed)
        {
            if (this.IsValid)
            {
                /// Create a new instance of the MCSDSPDDATA structure to hold speed data like max speed, acceleration, etc.
                Hpmcstd.MCSDSPDDATA speedData = new Hpmcstd.MCSDSPDDATA();

                /// Sets scaling factors depending on how large the speed value is.
                int range = (speed > 81910) ? 10 : 100;
                int irange = (speed > 81910) ? 100 : 10;

                /// Sets motion mode.
                speedData.dwMode = 2; // 2 for S-curve mode
                /// Sets the internal range divisor. Smaller values = finer control.
                speedData.dwRange = (uint)range;
                /// dwHighSpeed: max target speed (in PPS)
                speedData.dwHighSpeed = (uint)Math.Abs(speed / irange);

                speedData.dwLowSpeed = (uint)Math.Abs(1000 / irange); // dwLowSpeed: self-starting speed (in PPS)
                speedData.dwRate = new uint[] { 50, 8191, 8191 }; // Acceleration rate for each segment of motion (multi-stage ramp-up).
                speedData.dwRateChgPnt = new uint[] { 8191, 8191 }; // Points where the rate changes. Use 8191 means a simple trapezoidal drive.
                speedData.dwScw = new uint[] { 1024, 1024 }; // S-curve weighting factor.

                /// rear pulse setting, 0 means no extra delay 
                speedData.dwRearPulse = 0;

                /// Safety check: ensures that the max speed is not lower than the min speed.
                if (speedData.dwHighSpeed < speedData.dwLowSpeed)
                    speedData.dwHighSpeed = speedData.dwLowSpeed;

                /// Set speed for the selected axis.
                switch (axis)
                {
                    case AXIS.X:
                        /// Calls the DLL function McsdSetSpeed to set the speed to axis 1.
                        return Hpmcstd.McsdSetSpeed(hController, MC104_AXIS2, ref speedData);
                    case AXIS.Y:
                        return Hpmcstd.McsdSetSpeed(hController, MC104_AXIS1, ref speedData);
                    case AXIS.Z:
                        return Hpmcstd.McsdSetSpeed(hController, MC104_AXIS3, ref speedData);
                    default:
                        /// Fallback if not valid.
                        return Hpmcstd.MCSD_ERROR_AXIS;
                }                
            }
            /// Fallback if not valid.
            return Hpmcstd.MCSD_ERROR_NO_DEVICE;
        }

        /// <summary>
        /// Sets the speed of the specified axis in um/sec.
        /// </summary>
        /// <remarks>The speed is converted to encoder units internally before being applied to the axis.
        /// Ensure that the provided speed is appropriate for the axis configuration.</remarks>
        /// <param name="axis"></param>
        /// <param name="speed"></param>
        /// <returns>A status code indicating the result of the operation.</returns>
        public uint SetSpeed(AXIS axis, double speed)
        {
            return SetSpeedEnc(axis, Um2enc(axis, speed));
        }

        /// <summary>
        /// Sets the movement speed for all axes to the specified value.
        /// </summary>
        /// <remarks>This method applies the same speed to all axes (X, Y, and Z).  If the operation fails
        /// for any axis, the method stops and returns the corresponding error code.</remarks>
        /// <param name="speed"></param>
        /// <returns>A status code indicating the result of the operation.</returns>
        public uint SetSpeedAll(double speed = SPEED_DEFAULT)
        {
            uint[] results = new uint[3];

            results[0] = SetSpeedEnc(AXIS.X, Um2enc(AXIS.X, speed));
            results[1] = SetSpeedEnc(AXIS.Y, Um2enc(AXIS.Y, speed));
            results[2] = SetSpeedEnc(AXIS.Z, Um2enc(AXIS.Z, speed));

            /// Check if all axes were set successfully.
            foreach (uint result in results)
            {
                if (result != Hpmcstd.MCSD_ERROR_SUCCESS)
                {
                    return result; // Return the first error encountered.
                }
            }

            return Hpmcstd.MCSD_ERROR_SUCCESS;
        }

        /// <summary>
        /// Stops the motion of all axes and returns the result of the operation.
        /// </summary>
        public uint Stop()
        {
            AXIS[] axes = new AXIS[] { AXIS.X, AXIS.Y, AXIS.Z }; 
            /// Stop all axes by calling the StopAxis method for each axis.
            
            foreach(AXIS axis in axes)
            {
                uint result = StopAxis(axis);
                if (result != Hpmcstd.MCSD_ERROR_SUCCESS)
                    return result; // Return the first error encountered.
            }

            return Hpmcstd.MCSD_ERROR_SUCCESS;
        }

        /// <summary>
        /// Stops the specified axis immediately in an emergency situation.
        /// </summary>
        /// <remarks>This method performs an emergency stop on the specified axis by decelerating it to a halt.</remarks>
        /// <param name="axis">The axis to stop. Must be one of the defined <see cref="AXIS"/> values.</param>
        /// <returns>A status code indicating the result of the operation.  Returns a success code if the axis was stopped
        /// successfully.  Returns <see cref="Hpmcstd.MCSD_ERROR_AXIS"/> if the specified axis is invalid,  or <see
        /// cref="Hpmcstd.MCSD_ERROR_NO_DEVICE"/> if the controller is not valid.</returns>
        public uint StopAxisEmergency(AXIS axis)
        {
            if (this.IsValid)
            {
                switch (axis)
                {
                    case AXIS.X:
                        return Hpmcstd.McsdDriveStop(hController, MC104_AXIS2, 1); // 減速停止
                    case AXIS.Y:
                        return Hpmcstd.McsdDriveStop(hController, MC104_AXIS1, 1); // 減速停止
                    case AXIS.Z:
                        return Hpmcstd.McsdDriveStop(hController, MC104_AXIS3, 1); // 減速停止
                    default:
                        return Hpmcstd.MCSD_ERROR_AXIS;
                }
            }
            return Hpmcstd.MCSD_ERROR_NO_DEVICE;
        }

        /// <summary>
        /// Stops the emergency operation for all axes and returns the result of the operation.
        /// </summary>
        /// <remarks>This method attempts to stop the emergency operation for the X, Y, and Z axes
        /// sequentially.  If an error occurs while stopping any axis, the method returns the corresponding error code 
        /// without attempting to stop the remaining axes.</remarks>
        /// <returns>A status code indicating the result of the operation.  Returns <see cref="Hpmcstd.MCSD_ERROR_SUCCESS"/> if
        /// all axes are successfully stopped;  otherwise, returns the error code of the first axis that failed to stop.</returns>
        public uint StopEmergency()
        {
            AXIS[] axes = new AXIS[] { AXIS.X, AXIS.Y, AXIS.Z };
            
            foreach(AXIS axis in axes)
            {
                uint result = StopAxisEmergency(axis);
                if (result != Hpmcstd.MCSD_ERROR_SUCCESS)
                    return result; // Return the first error encountered.
            }

            return Hpmcstd.MCSD_ERROR_SUCCESS;
        }

        /// <summary>
        /// Stops the specified axis by performing a deceleration stop.
        /// </summary>
        /// <remarks>This method requires the controller to be in a valid state. If the controller is not
        /// valid,  the method returns an error code indicating that no device is available.</remarks>
        /// <param name="axis">The axis to stop. Must be one of the defined <see cref="AXIS"/> values.</param>
        /// <returns>A status code indicating the result of the operation.  Returns a success code if the axis was stopped
        /// successfully, or an error code if the operation failed.</returns>
        public uint StopAxis(AXIS axis)
        {
            if (this.IsValid)
            {
                switch (axis)
                {
                    case AXIS.X:
                        return Hpmcstd.McsdDriveStop(hController, MC104_AXIS2, 0); // 減速停止
                    case AXIS.Y:
                        return Hpmcstd.McsdDriveStop(hController, MC104_AXIS1, 0); // 減速停止
                    case AXIS.Z:
                        return Hpmcstd.McsdDriveStop(hController, MC104_AXIS3, 0); // 減速停止
                    default:
                        return Hpmcstd.MCSD_ERROR_AXIS;
                }
            }
            return Hpmcstd.MCSD_ERROR_NO_DEVICE;
        }

        /// <summary>
        /// Starts a jog operation on the specified axis in the given direction.
        /// </summary>
        /// <remarks>This method initiates a continuous jog movement on the specified axis in the
        /// specified direction. Ensure that the device is properly initialized and valid before calling this
        /// method.</remarks>
        /// <param name="axis">The axis on which to perform the jog operation. Must be one of the defined <see cref="AXIS"/> values.</param>
        /// <param name="dir">The direction of the jog operation. Must be one of the defined <see cref="DIRECTION"/> values.</param>
        /// <returns>A status code indicating the result of the operation. Returns a non-zero value if the jog operation starts
        /// successfully. Returns <see cref="Hpmcstd.MCSD_ERROR_NO_DEVICE"/> if the device is not valid or available.</returns>
        public uint StartJog(AXIS axis, DIRECTION direction)
        {
            if (this.IsValid)
            {
                ushort axisCode = 0;
                ushort directionCode;

                /// Sets the axis code based on the specified axis.
                switch (axis)
                {
                    case AXIS.X:
                        axisCode = MC104_AXIS2;
                        break;
                    case AXIS.Y:
                        axisCode = MC104_AXIS1;
                        break;
                    case AXIS.Z:
                        axisCode = MC104_AXIS3;
                        break;
                }

                /// Sets the direction code based on the specified direction.
                if (direction == DIRECTION.FORWARD)
                    directionCode = MC104_SCAN_FORWARD;
                else
                    directionCode = MC104_SCAN_REVERSE;

                /// Initiates the jog operation on the specified axis in the specified direction.
                return Hpmcstd.McsdDriveStart(hController, axisCode, directionCode, 0xFFFFFF);
            }

            return Hpmcstd.MCSD_ERROR_NO_DEVICE;
        }

        /// <summary>
        /// Starts a jog operation on the specified axis in the given direction and waits for the operation to complete.
        /// </summary>
        /// <remarks>This method initiates a jog operation on the specified axis and direction using the
        /// controller.  If the controller is not valid, the method will handle the error internally and still wait
        /// asynchronously.</remarks>
        /// <param name="axis">The axis on which to perform the jog operation. Must be one of the defined <see cref="AXIS"/> values.</param>
        /// <param name="dir">The direction of the jog operation. Must be one of the defined <see cref="DIRECTION"/> values.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public async Task StartJogWait(AXIS axis, DIRECTION direction)
        {
            if (this.IsValid)
            {
                ushort axisCode = 0;
                ushort directionCode;
                switch (axis)
                {
                    case AXIS.X:
                        axisCode = MC104_AXIS2;
                        break;
                    case AXIS.Y:
                        axisCode = MC104_AXIS1;
                        break;
                    case AXIS.Z:
                        axisCode = MC104_AXIS3;
                        break;
                }

                if (direction == DIRECTION.FORWARD)
                    directionCode = MC104_SCAN_FORWARD;
                else
                    directionCode = MC104_SCAN_REVERSE;

                Hpmcstd.McsdDriveStart(hController, axisCode, directionCode, 0xFFFFFF);

                await Wait();
                return;
                //return Hpmcstd.McsdDriveStart(hController, _axis, _dir, 0xFFFFFF);
            }

            await Wait();
            return;

            //return Hpmcstd.MCSD_ERROR_NO_DEVICE;
        }

        /// <summary>
        /// Starts an incremental encoder movement on the specified axis in the given direction for a specified
        /// distance.
        /// </summary>
        /// <param name="axis">The axis on which to perform the movement. Must be one of the defined <see cref="AXIS"/> values.</param>
        /// <param name="dir">The direction of the movement. Must be one of the defined <see cref="DIRECTION"/> values.</param>
        /// <param name="distance">The distance to move, in encoder units. Must be a positive value.</param>
        /// <returns>A status code indicating the result of the operation. Returns a non-zero value if the operation is
        /// successful,  or <see cref="Hpmcstd.MCSD_ERROR_NO_DEVICE"/> if the device is not valid or available.</returns>
        public uint StartIncEnc(AXIS axis, DIRECTION direction, uint distance)
        {
            if (this.IsValid)
            {
                ushort axisCode = 0;
                ushort directionCode;
                switch (axis)
                {
                    case AXIS.X:
                        axisCode = MC104_AXIS2;
                        break;
                    case AXIS.Y:
                        axisCode = MC104_AXIS1;
                        break;
                    case AXIS.Z:
                        axisCode = MC104_AXIS3;
                        break;
                }

                if (direction == DIRECTION.FORWARD)
                    directionCode = MC104_INDEX_FORWARD;
                else
                    directionCode = MC104_INDEX_REVERSE;

                return Hpmcstd.McsdDriveStart(hController, axisCode, directionCode, distance);
            }
            return Hpmcstd.MCSD_ERROR_NO_DEVICE;
        }

        /// <summary>
        /// Starts an incremental movement on the specified axis in the given direction for the specified distance.
        /// </summary>
        /// <remarks>The method converts the specified distance from micrometers to encoder units and
        /// initiates the movement.</remarks>
        /// <param name="axis">The axis on which the movement will be performed.</param>
        /// <param name="dir">The direction of the movement.</param>
        /// <param name="umdistance">The distance to move, in micrometers.</param>
        /// <returns>The distance to be moved, in encoder units, as an unsigned integer.</returns>
        public uint StartInc(AXIS axis, DIRECTION direction, double umDistance)
        {
            uint distance = (uint)Um2enc(axis, umDistance);
            return StartIncEnc(axis, direction, distance);
        }

        /// <summary>
        /// Starts the absolute encoder movement for the specified axis to reach the target position.
        /// </summary>
        /// <remarks>The method calculates the distance and direction required to move the specified axis
        /// to the target position and initiates the movement. The direction is determined based on the sign of the
        /// distance.</remarks>
        /// <param name="axis">The axis to move. Must be one of the defined <see cref="AXIS"/> values.</param>
        /// <param name="targetPosition">The target position to move the axis to, specified in encoder units.</param>
        /// <returns>A status code indicating the result of the operation. Returns a non-zero value if the operation is
        /// successful, or <see cref="Hpmcstd.MCSD_ERROR_NO_DEVICE"/> if the device is not valid or initialized.</returns>
        public uint StartAbsEnc(AXIS axis, int targetPosition)
        {
            if (this.IsValid)
            {
                ushort axisCode = 0;
                switch (axis)
                {
                    case AXIS.X:
                        axisCode = MC104_AXIS2;
                        break;
                    case AXIS.Y:
                        axisCode = MC104_AXIS1;
                        break;
                    case AXIS.Z:
                        axisCode = MC104_AXIS3;
                        break;
                }

                /// Get the current position of the specified axis in encoder units.
                int currentPosition = GetPositionEnc(axis);

                /// Calculate the distance to move.
                int distance = targetPosition - currentPosition;

                /// Determine the direction of movement based on the sign of the distance.
                ushort directionCode = (Math.Sign(distance) == -1) ? MC104_INDEX_REVERSE : MC104_INDEX_FORWARD;

                /// Initiate the movement using the calculated distance and direction.
                return Hpmcstd.McsdDriveStart(hController, axisCode, directionCode, (uint)Math.Abs(distance));
            }

            return Hpmcstd.MCSD_ERROR_NO_DEVICE;
        }

        /// <summary>
        /// Starts an absolute movement of the specified axis to the given position in micrometer.
        /// </summary>
        /// <remarks>The position is converted internally to encoder units before initiating the movement.
        /// Ensure that the axis is properly configured and ready for movement before calling this method.</remarks>
        /// <param name="axis">The axis to move. Must be a valid axis identifier.</param>
        /// <param name="targetPosition">The target position for the axis, specified in user-defined units.</param>
        /// <returns>A status code indicating the result of the operation. A value of 0 typically indicates success.</returns>
        public uint StartAbs(AXIS axis, double targetPosition)
        {
            int posEnc = Um2enc(axis, targetPosition);
            return StartAbsEnc(axis, posEnc);
        }

        /// <summary>
        /// Moves the X, Y, and Z axes to the specified absolute positions.
        /// </summary>
        /// <remarks>This method initiates movement for all three axes simultaneously to the specified
        /// positions. Ensure that the system is properly initialized and ready for movement before calling this
        /// method.</remarks>
        /// <param name="x">The target position for the X axis.</param>
        /// <param name="y">The target position for the Y axis.</param>
        /// <param name="z">The target position for the Z axis.</param>
        public void StartAbsAll(double XTarget, double YTarget, double ZTarget)
        {
            _ = StartAbs(AXIS.X, XTarget);
            _ = StartAbs(AXIS.Y, YTarget);
            _ = StartAbs(AXIS.Z, ZTarget);
        }

        /// Relative step movement from the current position of the axes. Origin is the center of the range.
        public async Task StartAbsFromCenter(double x, double y, double z)
        {

            StartAbsAll(x + RANGE_X/2, y + RANGE_Y/2, -z + RANGE_Z/2);

            await Wait();
        }

        /// <summary>
        /// 同时移动三个轴指定的脉冲数(增量式)
        /// </summary>
        /// <param name="xPulse">X轴移动的脉冲数(正值向前,负值向后)</param>
        /// <param name="yPulse">Y轴移动的脉冲数(正值向前,负值向后)</param>
        /// <param name="zPulse">Z轴移动的脉冲数(正值向前,负值向后)</param>
        /// <returns>操作结果代码</returns>
        public uint StepIncEnc(int xPulse, int yPulse, int zPulse)
        {
            if (!this.IsValid)
                return Hpmcstd.MCSD_ERROR_NO_DEVICE;

            try
            {
                // 计算需要添加多少个命令(只为非零位移的轴添加命令)
                int commandCount = (xPulse != 0 ? 1 : 0) + (yPulse != 0 ? 1 : 0) + (zPulse != 0 ? 1 : 0);

                if (commandCount == 0)
                    return Hpmcstd.MCSD_ERROR_SUCCESS; // 没有需要移动的轴

                // 开始缓冲命令
                Hpmcstd.McsdStartBuffer(hController, (ushort)commandCount);

                // X轴移动命令
                if (xPulse != 0)
                {
                    ushort cmd = xPulse > 0 ? (ushort)Hpmcstd.MCSD_PLUS_INDEX_PULSE_DRIVE
                                           : (ushort)Hpmcstd.MCSD_MINUS_INDEX_PULSE_DRIVE;
                    Hpmcstd.McsdDataWrite(hController, MC104_AXIS2, cmd, (uint)Math.Abs(xPulse));
                }

                // Y轴移动命令
                if (yPulse != 0)
                {
                    ushort cmd = yPulse > 0 ? (ushort)Hpmcstd.MCSD_PLUS_INDEX_PULSE_DRIVE
                                           : (ushort)Hpmcstd.MCSD_MINUS_INDEX_PULSE_DRIVE;
                    Hpmcstd.McsdDataWrite(hController, MC104_AXIS1, cmd, (uint)Math.Abs(yPulse));
                }

                // Z轴移动命令
                if (zPulse != 0)
                {
                    ushort cmd = zPulse > 0 ? (ushort)Hpmcstd.MCSD_MINUS_INDEX_PULSE_DRIVE
                                           : (ushort)Hpmcstd.MCSD_PLUS_INDEX_PULSE_DRIVE;
                    Hpmcstd.McsdDataWrite(hController, MC104_AXIS3, cmd, (uint)Math.Abs(zPulse));
                }

                // 结束缓冲并执行命令
                return Hpmcstd.McsdEndBuffer(hController);
            }
            catch
            {
                return Hpmcstd.MCSD_ERROR_SYSTEM;
            }
        }

        /// <summary>
        /// 同时移动三个轴指定的微米数(增量式)
        /// </summary>
        /// <param name="xUm">X轴移动的微米数(正值向前,负值向后)</param>
        /// <param name="yUm">Y轴移动的微米数(正值向前,负值向后)</param>
        /// <param name="zUm">Z轴移动的微米数(正值向前,负值向后)</param>
        /// <returns>操作结果代码</returns>
        public uint StepInc(double xUm, double yUm, double zUm)
        {
            // 将微米值转换为脉冲数
            int xPulse = Um2enc(AXIS.X, xUm);
            int yPulse = Um2enc(AXIS.Y, yUm);
            int zPulse = Um2enc(AXIS.Z, zUm);

            // 调用脉冲版本的方法
            return StepIncEnc(xPulse, yPulse, zPulse);
        }

        /// <summary>
        /// 同时移动三个轴指定的微米数(增量式)并等待完成
        /// </summary>
        /// <param name="xUm">X轴移动的微米数</param>
        /// <param name="yUm">Y轴移动的微米数</param>
        /// <param name="zUm">Z轴移动的微米数</param>
        /// <returns>异步任务</returns>
        public async Task StepIncAsync(double xUm, double yUm, double zUm)
        {
            StepInc(xUm, yUm, zUm);
            await Wait(); // 等待所有轴移动完成
        }

        /// <summary>
        /// Determines whether the axis is currently busy.
        /// </summary>
        /// <remarks>This method checks the status of the axis to determine if it is actively engaged in
        /// an operation.</remarks>
        /// <returns><see langword="true"/> if the axis is busy; otherwise, <see langword="false"/>.</returns>
        public bool IsBusy()
        {
            ushort status;
            /// Calls the DLL function McsdGetAxisBusy to check if the axis is busy.
            Hpmcstd.McsdGetAxisBusy(hController, out status);

            /// Check if the result indicates an error.
            if ((status & 0xFF00) > 0)
                GetEndStatus(status); // Get the end status of the axis.

            /// Check if the status indicates that the axis is busy.
            return ((status & 0x0F) != 0);
        }
        
        /// <summary>
        /// Determines whether the specified axis is currently busy performing an operation.
        /// </summary>
        /// <remarks>This method checks the status of the specified axis and determines if it is actively
        /// engaged in an operation. The result is based on the status flags for the controller.</remarks>
        /// <param name="axis">The axis to check. Must be one of the defined <see cref="AXIS"/> values.</param>
        /// <returns><see langword="true"/> if the specified axis is busy; otherwise, <see langword="false"/>.</returns>
        public bool IsBusy(AXIS axis)
        {
            ushort status;
            /// Calls the DLL function McsdGetAxisBusy to check if the axis is busy.
            Hpmcstd.McsdGetAxisBusy(hController, out status);

            /// Check if the result indicates an error.
            switch (axis)
            {
                case AXIS.X:
                    return (status & 0x01) > 0;
                case AXIS.Y:
                    return (status & 0x02) > 0;
                case AXIS.Z:
                    return (status & 0x04) > 0;
            }

            /// If the axis is not valid, return false.
            return false;
        }

        /// <summary>
        /// Retrieves the status of the controller.
        /// </summary>
        /// <returns></returns>
        public ushort GetStatus()
        {
            ushort status;
            Hpmcstd.McsdGetAxisBusy(hController, out status);

            return status;
        }

        /// <summary>
        /// Retrieves the error message associated with the current instance.
        /// </summary>
        /// <returns>A <see cref="string"/> containing the error message. Returns an empty string if no error message is set.</returns>
        public string GetError()
        {
            return comment;
        }

        /// <summary>
        /// Retrieves a descriptive error message corresponding to the specified error code.
        /// </summary>
        /// <remarks>This method maps predefined error codes from the <see cref="Hpmcstd"/> class to their
        /// respective descriptive messages. If the provided error code does not match any known value, the method
        /// returns an empty string.</remarks>
        /// <param name="errorCode">The error code for which to retrieve the corresponding error message. Must be a valid error code defined in
        /// the <see cref="Hpmcstd"/> class.</param>
        /// <returns>A string containing the error message associated with the specified <paramref name="errorCode"/>. If the
        /// error code is not recognized, an empty string is returned.</returns>
        public string GetError(uint errorCode)
        {
            switch (errorCode)
            {
                case Hpmcstd.MCSD_ERROR_SUCCESS:
                    comment = "MCSD_ERROR_SUCCESS";
                    break;
                case Hpmcstd.MCSD_ERROR_SYSTEM:
                    comment = "MCSD_ERROR_SYSTEM";
                    break;
                case Hpmcstd.MCSD_ERROR_NO_DEVICE:
                    comment = "MCSD_ERROR_NO_DEVICE";
                    break;
                case Hpmcstd.MCSD_ERROR_IN_USE:
                    comment = "MCSD_ERROR_IN_USE";
                    break;
                case Hpmcstd.MCSD_ERROR_ID:
                    comment = "MCSD_ERROR_ID";
                    break;
                case Hpmcstd.MCSD_ERROR_AXIS:
                    comment = "MCSD_ERROR_AXIS";
                    break;
                case Hpmcstd.MCSD_ERROR_PORT:
                    comment = "MCSD_ERROR_PORT";
                    break;
                case Hpmcstd.MCSD_ERROR_PARAMETER:
                    comment = "MCSD_ERROR_PARAMETER";
                    break;
                case Hpmcstd.MCSD_ERROR_PROC:
                    comment = "MCSD_ERROR_PROC";
                    break;
                case Hpmcstd.MCSD_ERROR_CALLBACK:
                    comment = "MCSD_ERROR_CALLBACK";
                    break;
                case Hpmcstd.MCSD_ERROR_HANDLE:
                    comment = "MCSD_ERROR_HANDLE";
                    break;
                case Hpmcstd.MCSD_ERROR_USB_TRANS:
                    comment = "MCSD_ERROR_USB_TRANS";
                    break;
                case Hpmcstd.MCSD_ERROR_USB_RECEIVE:
                    comment = "MCSD_ERROR_USB_RECEIVE";
                    break;
                case Hpmcstd.MCSD_ERROR_USB_OFFLINE:
                    comment = "MCSD_ERROR_USB_OFFLINE";
                    break;
                case Hpmcstd.MCSD_ERROR_ORG_RETURN:
                    comment = "MCSD_ERROR_ORG_RETURN";
                    break;
                default:
                    comment = "";
                    break;
            }
            return comment;
        }

        /// <summary>
        /// Retrieves the end status of the specified axis, indicating various stop or error conditions.
        /// </summary>
        /// <remarks>This method queries the status of the specified axis and interprets the end
        /// conditions based on the status flags. The returned string provides a human-readable description of the
        /// conditions that caused the axis to stop.</remarks>
        /// <param name="axis">The axis for which to retrieve the end status. Must be one of the defined <see cref="AXIS"/> values.</param>
        /// <returns>A string describing the end status of the specified axis. The string may include one or more of the
        /// following conditions: <list type="bullet"> <item><description>"Data Error End" - Indicates a data error
        /// condition.</description></item> <item><description>"Alarm Signal End" - Indicates an alarm signal
        /// condition.</description></item> <item><description>"Emergency Stop Command End" - Indicates an emergency
        /// stop command was issued.</description></item> <item><description>"Slow Down Stop Command End" - Indicates a
        /// slow down stop command was issued.</description></item> <item><description>"Emergency Stop Signal End" -
        /// Indicates an emergency stop signal was received.</description></item> <item><description>"Org Error End" -
        /// Indicates an origin error condition.</description></item> <item><description>"Limit Signal End" - Indicates
        /// a limit signal condition.</description></item> </list> If no conditions are met, the returned string will be
        /// empty.</returns>
        public string GetAxisEndStatus(AXIS axis)
        {
            Hpmcstd.MCSDSTATUS status = new Hpmcstd.MCSDSTATUS();
            uint result;
            switch (axis)
            {
                case AXIS.X:
                    result = Hpmcstd.McsdGetStatus(hController, MC104_AXIS2, out status);
                    comment = "X: ";
                    break;
                case AXIS.Y:
                    result = Hpmcstd.McsdGetStatus(hController, MC104_AXIS1, out status);
                    comment = "Y: ";
                    break;
                case AXIS.Z:
                    result = Hpmcstd.McsdGetStatus(hController, MC104_AXIS3, out status);
                    comment = "Z: ";
                    break;
            }

            if ((status.bEnd & 0b10000000) > 0)
                comment += "Data Error End" + " ";
            if ((status.bEnd & 0b01000000) > 0)
                comment += "Alarm Signal End" + " ";
            if ((status.bEnd & 0b00100000) > 0)
                comment += "Emergency Stop Command End" + " ";
            if ((status.bEnd & 0b00010000) > 0)
                comment += "Slow Down Stop Command End" + " ";
            if ((status.bEnd & 0b00001000) > 0)
                comment += "Emergency Stop Signal End" + " ";
            if ((status.bEnd & 0b00000100) > 0)
                comment += "Org Error End" + " ";
            if ((status.bEnd & 0b10000001) > 0)
                comment += "Limit Signal End" + " ";

            return comment;
        }

        /// <summary>
        /// Retrieves the combined end status of all axes based on the provided status code.
        /// </summary>
        /// <remarks>This method checks specific bits in the <paramref name="status"/> parameter to
        /// determine which axes are active and retrieves their respective end statuses using the
        /// <c>GetAxisEndStatus</c> method.</remarks>
        /// <param name="status">A 32-bit unsigned integer representing the status code. Each bit in the status code corresponds to a
        /// specific axis, where bits 8, 9, and 10 indicate the X, Y, and Z axes, respectively.</param>
        /// <returns>A string containing the end status of the axes that are active in the provided status code. The statuses are
        /// concatenated with a space separator. Returns an empty string if no axes are active.</returns>
        public string GetEndStatus(uint status)
        {
            string allStatus = "";
            if ((status & 0x0100) > 0)
                allStatus += GetAxisEndStatus(AXIS.X) + " ";
            if ((status & 0x0200) > 0)
                allStatus += GetAxisEndStatus(AXIS.Y) + " ";
            if ((status & 0x0400) > 0)
                allStatus += GetAxisEndStatus(AXIS.Z) + " ";

            comment = allStatus;
            return allStatus;
        }

        #endregion

    }

}
