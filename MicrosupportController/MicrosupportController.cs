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
/// version 2.0.0.1     2025/11/11
///                     Removing redundant usings.
///                     Modified by Haoran, Yao
///----------------------------------------------------------------------------

using System;
using HpmcstdCs;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Diagnostics;

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

        /// Initialization status and error message.
        private bool _isInitialized = false; // flag
        private string _initializationError = ""; // error message
        public bool IsInitialized => _isInitialized; // read-only properties
        public string InitializationError => _initializationError; // read-only properties

        /// Dictionary to hold Microsupport instances for different device IDs.
        public static readonly Dictionary<string, Microsupport> controllers = new Dictionary<string, Microsupport>();

        /// Keep track of controllers that have reported errors to avoid duplicate error messages.
        private static readonly HashSet<ushort> errorReportedControllers = new HashSet<ushort>();

        #endregion

        #region Constants and Enumerators

        /// Maximum number of axes supported by the controller
        public const int MC104_MAX_AXES = 4;

        /// Axis code
        public const ushort MC104_AXIS1 = 0;
        public const ushort MC104_AXIS2 = 1;
        public const ushort MC104_AXIS3 = 2;

        /// Driving modes
        public const ushort MC104_INDEX_FORWARD = 0; // INC
        public const ushort MC104_INDEX_REVERSE = 1;
        public const ushort MC104_SCAN_FORWARD = 2; // JOG
        public const ushort MC104_SCAN_REVERSE = 3;

        /// Homming patterns.
        public const ushort MC104_ORG_0 = 0; // User defined origin return mode
        public const ushort MC104_ORG_1 = 1; // Origin signal - side edge detection
        public const ushort MC104_ORG_2 = 2; // Origin signal - side edge detection after Z phase + side rising edge detection
        public const ushort MC104_ORG_3 = 3; // - Limit signal edge detection
        public const ushort MC104_ORG_4 = 4; // + Limit signal edge detection
        public const ushort MC104_ORG_5 = 5; // - Limit signal edge detection after Z phase + side rising edge detection
        public const ushort MC104_ORG_6 = 6; // + Limit signal edge detection after Z phase - side rising edge detection
        public const ushort MC104_ORG_7 = 7; // - Z phase edge detection
        public const ushort MC104_ORG_8 = 8; // Origin signal + side edge detection
        public const ushort MC104_ORG_9 = 9; // Origin signal + side edge detection after Z phase - side rising edge detection

        /// Resolution of each axis (um/pulse)
        private const double RESOLUTIONS_AXIS_X = 0.05;
        private const double RESOLUTIONS_AXIS_Y = 0.05;
        private const double RESOLUTIONS_AXIS_Z = 0.05;

        /// Represents the axes in a three-dimensional coordinate system.
        public enum AXIS
        {
            X,
            Y,
            Z,
        }

        /// Represents the direction of movement or operation.
        public enum DIRECTION
        {
            FORWARD,
            REVERSE
        }

        private static readonly AXIS[] AllAxes = new AXIS[] { AXIS.X, AXIS.Y, AXIS.Z };

        /// Max speed (pulse/sec)
        private const int MAX_SPEED = 50000; // X axis, Y axis, Z axis
        private const double MAX_UM_SPEED = MAX_SPEED * RESOLUTIONS_AXIS_X; // X axis, Y axis, Z axis
        private const double SPEED_DEFAULT = 1000;

        /// Range of movement for each axis in micrometers (um).
        private const double RANGE_X = 20000; // X axis movement range (um)
        private const double RANGE_Y = 20000; // Y axis movement range (um)
        private const double RANGE_Z = 30000; // Z axis movement range (um)

        private string comment = "";

        private int accelerationTimeMs = 100; // Acceleration time in milliseconds for each axis.
        private bool isSmoothingEnabled = false; // Flag to enable or disable smoothing.

        #endregion

        #region Class constructors and utilities
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

                    /// Check if the instance was initialized successfully.
                    if (!instance.IsInitialized)
                    {
                        /// Log the error only once for each device ID.
                        if (!errorReportedControllers.Contains((ushort)deviceId))
                        {
                            Console.WriteLine($"Error initializing Microsupport for device {name}: {instance.InitializationError}");
                            errorReportedControllers.Add((ushort)deviceId);
                        }
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

        /// <summary>
        /// Initializes the controller with the specified device ID and configures its axes.
        /// </summary>
        /// <remarks>This method attempts to open a connection to the device using the provided ID. If the
        /// connection is successful, it configures each axis of the device with predefined settings, including pulse
        /// output mode, limit switch mode, and signal stop mode. If the initialization fails, the method sets an error
        /// message and marks the controller as uninitialized.</remarks>
        /// <param name="ID">The unique identifier of the device to initialize. Must be a valid device ID.</param>
        public void Initialize(ushort ID)
        {
            try
            {
                /// opens the device with the specified ID.
                hController = Hpmcstd.McsdOpen("MCUSB4sd", ID); // McsdOpen function opens the device and returns a handle.

                if (this.IsValid)
                {
                    int err = 0; // stores the error code returned by the Hpmcstd library functions.

                    /// Sets up each axis with specific configurations.
                    for (ushort i = 0; i < MC104_MAX_AXES; i++)
                    {
                        /// McsdSetPulseMode(hDevice, wAxis, wMode) function sets the pulse output mode for the specified axis.
                        // set the pulse output mode to 4 (CW/CCW pulse active High )
                        // DIR   output port - CW pulse  active High
                        // PULSE output port - CCW pulse active High
                        err = (int)Hpmcstd.McsdSetPulseMode(hController, i, 4);

                        /// McsdSetLimit(hDevice, wAxis, pwLevel) function sets the limit switch mode for the specified axis.
                        // +LMT		  input signal active level	High
                        // -LMT		  input signal active level	High
                        // ALARM	  input signal active level	High
                        // INPOSITION input signal active level	High
                        err = (int)Hpmcstd.McsdSetLimit(hController, i, 0x00);

                        /// McsdSetSignalStop(hDevice, wAxis, wAlarmStop, wInposStop, wLimitStop) function sets the signal stop mode for the specified axis.
                        // ALARM signal stop   enabled
                        // INPOSITION signal stop enabled
                        // LIMIT signal stop    enabled
                        err = (int)Hpmcstd.McsdSetSignalStop(hController, i, 0, 0, 2);
                    }

                    _isInitialized = true;
                    _initializationError = ""; // Clear any previous initialization error message.
                }
                else
                {
                    _isInitialized = false;
                    _initializationError = $"Failed to open controller with ID {ID}. Device not found or unavailable.";
                }
            }
            catch (Exception ex)
            {
                _isInitialized = false;
                _initializationError = $"Exception during initialization: {ex.Message}";
                hController = 0xFFFFFFFF;
                Debug.WriteLine($"[Microsupport] Exception during initialization for ID {ID}: {ex.Message}");
            }
        }

        #endregion

        #region Basic control methods

        /// <summary>
        /// Initiates the origin start process for the specified axis with the given action.
        /// </summary>
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

        /// <summary>
        /// This method centers all axes (X, Y, Z) of the motion controller by moving them to the midpoint of their
        /// </summary>
        /// <returns></returns>
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
            StartIncAbs(AXIS.X, centerX);
            StartIncAbs(AXIS.Y, centerY);
            StartIncAbs(AXIS.Z, centerZ);

            /// Wait for the centering process to complete.
            while (IsBusy())
            {
                /// Wait for a short period to avoid busy waiting.
                await Task.Delay(100);
            }
        }
        #endregion

        #region Position and speed setting methods
        /// <summary>
        /// Executes an action for all axes and returns the first error encountered.
        /// </summary>
        private uint ForEachAxis(Func<AXIS, uint> action)
        {
            if (!IsValid) return Hpmcstd.MCSD_ERROR_NO_DEVICE;

            foreach (var axis in AllAxes)
            {
                var result = action(axis);
                if (result != Hpmcstd.MCSD_ERROR_SUCCESS)
                    return result;
            }

            return Hpmcstd.MCSD_ERROR_SUCCESS;
        }

        /// <summary>
        /// Convert encoder value to um value.
        /// </summary>
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
                /// Initialize the position array to hold the positions of all axes.
                pos = new int[MC104_MAX_AXES];
                for (ushort i = 0; i < MC104_MAX_AXES; i++)
                {
                    /// Declare a variable to hold the counter data for the current axis.
                    uint dwData;
                    /// McsdGetCounter(hDevice, wAxis, wCounter, out pwData) function retrieves the current counter value for the specified axis.
                    //  wCounter: 0 for internal counter
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
        /// Obtains the current position (um) of the controller in encoder units.
        /// </summary>
        /// <return> An array of integers, where each element represents the position in micrometers </return>
        public double[] GetPositions()
        {
            /// Position array in micrometers.
            double[] posum = null;

            int[] pos = GetPositionsEnc();

            /// Convert encoder positions to micrometers for each axis.
            if (pos != null)
            {
                posum = new double[3];
                posum[0] = Enc2um(AXIS.X, pos[1]);
                posum[1] = Enc2um(AXIS.Y, pos[0]);
                posum[2] = Enc2um(AXIS.Z, pos[2]);
            }
            return posum;
        }

        /// <summary>
        /// Calculates the positions relative to the center of the stoke.
        /// </summary>
        public double[] GetPositionsFromCenter()
        {
            double[] posAbs = GetPositions();
            double[] posFromCenter = null;
            if (posAbs != null)
            {
                posFromCenter = new double[3];
                posFromCenter[0] = posAbs[0] - RANGE_X / 2; // Distance from center of X axis
                posFromCenter[1] = posAbs[1] - RANGE_Y / 2; // Distance from center of Y axis
                posFromCenter[2] = - posAbs[2] + RANGE_Z / 2; // Distance from center of Z axis
            }
            return posFromCenter;
        }

        /// <summary>
        /// Sets the speed for the specified axis in encoder units.
        /// </summary>
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
                speedData.dwMode = 2; // 1 for trapezoidal speed profile
                /// Sets the internal range divisor. Smaller values = finer control.
                speedData.dwRange = (uint)range;
                /// dwHighSpeed: max target speed (in PPS)
                speedData.dwHighSpeed = (uint)Math.Abs(speed / irange);

                /// dwLowSpeed: min target speed (in PPS)
                uint lowSpeedData = (uint)(speedData.dwHighSpeed * 0.20); // 20% of max speed
                speedData.dwLowSpeed = lowSpeedData;
                //  speedData.dwLowSpeed = (uint)Math.Abs(1000 / irange);

                /// Safety check: ensures that the max speed is not lower than the min speed.
                if (speedData.dwHighSpeed < speedData.dwLowSpeed)
                    speedData.dwHighSpeed = speedData.dwLowSpeed;

                speedData.dwRate = new uint[] { 50, 8191, 8191 }; // Acceleration rate for each segment of motion (multi-stage ramp-up).
                speedData.dwRateChgPnt = new uint[] { 8191, 8191 }; // Points where the rate changes. Use 8191 means a simple trapezoidal drive.

                /// Compute S-curve weighting factor based on speed difference.
                uint speedDiff = speedData.dwHighSpeed - speedData.dwLowSpeed;
                uint scwValue = (uint)(speedDiff / 2.0); 

                if (scwValue > 4095) scwValue = 4095; // Cap the value to a maximum of 4095.
                speedData.dwScw = new uint[] { scwValue, scwValue }; // S-curve weighting factor.
                //  speedData.dwScw = new uint[] { 1024, 1024 }; // S-curve weighting factor.


                /// rear pulse setting, 0 means no extra delay 
                speedData.dwRearPulse = 0;

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
        public uint SetSpeed(AXIS axis, double speed)
        {
            return SetSpeedEnc(axis, Um2enc(axis, speed));
        }

        /// <summary>
        /// Sets the movement speed for all axes to the specified value.
        /// </summary>
        public uint SetSpeedAll(double speed = SPEED_DEFAULT)
        {
            uint[] results = new uint[3];

            results[0] = SetSpeed(AXIS.X, speed);
            results[1] = SetSpeed(AXIS.Y, speed);
            results[2] = SetSpeed(AXIS.Z, speed);

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
        /// Stops the specified axis by performing a deceleration stop.
        /// </summary>
        public uint StopAxis(AXIS axis)
        {
            if (this.IsValid)
            {
                switch (axis)
                {
                    case AXIS.X:
                        return Hpmcstd.McsdDriveStop(hController, MC104_AXIS2, 0); // Deceleration stop
                    case AXIS.Y:
                        return Hpmcstd.McsdDriveStop(hController, MC104_AXIS1, 0);
                    case AXIS.Z:
                        return Hpmcstd.McsdDriveStop(hController, MC104_AXIS3, 0);
                    default:
                        return Hpmcstd.MCSD_ERROR_AXIS;
                }
            }
            return Hpmcstd.MCSD_ERROR_NO_DEVICE;
        }

        /// <summary>
        /// Stops the motion of all axes and returns the result of the operation.
        /// </summary>
        public uint Stop() => ForEachAxis(StopAxis);

        /// <summary>
        /// Stops the specified axis immediately in an emergency situation.
        /// </summary>
        public uint StopAxisEmergency(AXIS axis)
        {
            if (this.IsValid)
            {
                switch (axis)
                {
                    case AXIS.X:
                        return Hpmcstd.McsdDriveStop(hController, MC104_AXIS2, 1); // Deceleration stop
                    case AXIS.Y:
                        return Hpmcstd.McsdDriveStop(hController, MC104_AXIS1, 1);
                    case AXIS.Z:
                        return Hpmcstd.McsdDriveStop(hController, MC104_AXIS3, 1);
                    default:
                        return Hpmcstd.MCSD_ERROR_AXIS;
                }
            }
            return Hpmcstd.MCSD_ERROR_NO_DEVICE;
        }

        /// <summary>
        /// Stops the emergency operation for all axes and returns the result of the operation.
        /// </summary>
        public uint StopEmergency() => ForEachAxis(StopAxisEmergency);

        #endregion

        #region Motion control methods
        /// <summary>
        /// Starts a jog operation on the specified axis in the given direction.
        /// </summary>
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
                        axisCode = MC104_AXIS2; // X axis is mapped to MC104_AXIS2
                        break;
                    case AXIS.Y:
                        axisCode = MC104_AXIS1; // Y axis is mapped to MC104_AXIS1
                        break;
                    case AXIS.Z:
                        axisCode = MC104_AXIS3; // Z axis is mapped to MC104_AXIS3
                        break;
                }

                /// Sets the direction code based on the specified direction.
                if (direction == DIRECTION.FORWARD)
                    directionCode = MC104_SCAN_FORWARD; // Jog forward
                else
                    directionCode = MC104_SCAN_REVERSE;

                /// McsdDriveStart(hDevice, wAxis, wDrive, dwPulse) starts jog motion as directionCode is set to SCAN
                //  INDEX PULSE DRIVE - move by specified pulse count
                //  SCAN  DRIVE  - move continuously until a stop command is received
                return Hpmcstd.McsdDriveStart(hController, axisCode, directionCode, 0xFFFFFF);
            }

            return Hpmcstd.MCSD_ERROR_NO_DEVICE;
        }

        /// <summary>
        /// Starts an incremental encoder movement on the specified axis in the given direction for a specified
        /// distance.
        /// </summary>
        public uint StartIncEnc(AXIS axis, DIRECTION direction, uint distance)
        {
            if (this.IsValid)
            {
                ushort axisCode = 0;
                ushort directionCode;
                switch (axis)
                {
                    case AXIS.X:
                        axisCode = MC104_AXIS2; // X axis is mapped to MC104_AXIS2
                        break;
                    case AXIS.Y:
                        axisCode = MC104_AXIS1; // Y axis is mapped to MC104_AXIS1
                        break;
                    case AXIS.Z:
                        axisCode = MC104_AXIS3; // Z axis is mapped to MC104_AXIS3
                        break;
                }

                if (direction == DIRECTION.FORWARD)
                    directionCode = MC104_INDEX_FORWARD;
                else
                    directionCode = MC104_INDEX_REVERSE;

                /// McsdDriveStart(hDevice, wAxis, wDrive, dwPulse) starts incremental motion as directionCode is set to INDEX
                return Hpmcstd.McsdDriveStart(hController, axisCode, directionCode, distance);
            }
            return Hpmcstd.MCSD_ERROR_NO_DEVICE;
        }

        /// <summary>
        /// Starts an incremental movement on the specified axis in the given direction for the specified distance.
        /// </summary>
        public uint StartInc(AXIS axis, DIRECTION direction, double umDistance)
        {
            uint distance = (uint)Um2enc(axis, umDistance);
            return StartIncEnc(axis, direction, distance);
        }

        /// <summary>
        /// Starts the absolute encoder movement for the specified axis to reach the target position. Note that GetPosition() is used as the reference point.
        /// </summary>
        public uint StartIncAbsEnc(AXIS axis, int targetPosition)
        {
            if (this.IsValid)
            {
                ushort axisCode = 0;
                switch (axis)
                {
                    case AXIS.X:
                        axisCode = MC104_AXIS2; // X axis is mapped to MC104_AXIS2
                        break;
                    case AXIS.Y:
                        axisCode = MC104_AXIS1; // Y axis is mapped to MC104_AXIS1
                        break;
                    case AXIS.Z:
                        axisCode = MC104_AXIS3; // Z axis is mapped to MC104_AXIS3
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
        public uint StartIncAbs(AXIS axis, double targetPosition)
        {
            int posEnc = Um2enc(axis, targetPosition);
            return StartIncAbsEnc(axis, posEnc);
        }

        /// <summary>
        /// Moves the X, Y, and Z axes to the specified absolute positions.
        /// </summary>
        public void StartIncAbsAll(double XTarget, double YTarget, double ZTarget)
        {
            if (!this.IsValid) return;

            _ = StartIncAbs(AXIS.X, XTarget);
            _ = StartIncAbs(AXIS.Y, YTarget);
            _ = StartIncAbs(AXIS.Z, ZTarget);
        }

        public void StartAbsFromCenter(AXIS axis, double position)
        {
            switch (axis)
            {
                case AXIS.X:
                    StartIncAbs(AXIS.X, position + RANGE_X / 2);
                    break;
                case AXIS.Y:
                    StartIncAbs(AXIS.Y, position + RANGE_Y / 2);
                    break;
                case AXIS.Z:
                    StartIncAbs(AXIS.Z, -position + RANGE_Z / 2);
                    break;
            }
        }

        public void StartAbsAllFromCenter(double x, double y, double z)
        {
            StartIncAbsAll(x + RANGE_X/2, y + RANGE_Y/2, -z + RANGE_Z/2);
        }

        /// Relative step movement from the current position of the axes. Origin is the center of the stoke.
        public async Task StartAbsAllFromCenterAsync(double x, double y, double z)
        {

            StartIncAbsAll(x + RANGE_X/2, y + RANGE_Y/2, -z + RANGE_Z/2);

            await Wait();
        }

        /// <summary>
        /// Moves the X, Y, and Z axes by the specified number of pulses (incremental) with buffer. Positive values move forward, negative values move backward.
        /// </summary>
        public uint StartIncBufferEnc(int xPulse, int yPulse, int zPulse)
        {
            if (!this.IsValid)
                return Hpmcstd.MCSD_ERROR_NO_DEVICE;

            try
            {
                /// Calculate the number of movement commands to be issued based on non-zero pulse values.
                int commandCount = (xPulse != 0 ? 1 : 0) + (yPulse != 0 ? 1 : 0) + (zPulse != 0 ? 1 : 0);

                if (commandCount == 0)
                    return Hpmcstd.MCSD_ERROR_SUCCESS;

                /// Start buffering movement commands.
                Hpmcstd.McsdStartBuffer(hController, (ushort)commandCount);

                /// X axis movement command
                if (xPulse != 0)
                {
                    ushort cmd = xPulse > 0 ? (ushort)Hpmcstd.MCSD_PLUS_INDEX_PULSE_DRIVE
                                           : (ushort)Hpmcstd.MCSD_MINUS_INDEX_PULSE_DRIVE;
                    Hpmcstd.McsdDataWrite(hController, MC104_AXIS2, cmd, (uint)Math.Abs(xPulse));
                }

                /// Y axis movement command
                if (yPulse != 0)
                {
                    ushort cmd = yPulse > 0 ? (ushort)Hpmcstd.MCSD_PLUS_INDEX_PULSE_DRIVE
                                           : (ushort)Hpmcstd.MCSD_MINUS_INDEX_PULSE_DRIVE;
                    Hpmcstd.McsdDataWrite(hController, MC104_AXIS1, cmd, (uint)Math.Abs(yPulse));
                }

                /// Z axis movement command
                if (zPulse != 0)
                {
                    ushort cmd = zPulse > 0 ? (ushort)Hpmcstd.MCSD_MINUS_INDEX_PULSE_DRIVE
                                           : (ushort)Hpmcstd.MCSD_PLUS_INDEX_PULSE_DRIVE;
                    Hpmcstd.McsdDataWrite(hController, MC104_AXIS3, cmd, (uint)Math.Abs(zPulse));
                }

                /// Execute the buffered movement commands.
                return Hpmcstd.McsdEndBuffer(hController);
            }
            catch
            {
                return Hpmcstd.MCSD_ERROR_SYSTEM;
            }
        }

        /// <summary>
        /// Moves simultaneously the X, Y, and Z axes by the specified number of micrometers (incremental) with buffer.
        /// </summary>
        public uint StartIncBuffer(double xUm, double yUm, double zUm)
        {
            /// Convert micrometer values to pulse values for each axis.
            int xPulse = Um2enc(AXIS.X, xUm);
            int yPulse = Um2enc(AXIS.Y, yUm);
            int zPulse = Um2enc(AXIS.Z, zUm);

            /// Call the StartIncBufferEnc method with the calculated pulse values.
            return StartIncBufferEnc(xPulse, yPulse, zPulse);
        }

        /// <summary>
        /// Moves simultaneously the X, Y, and Z axes by the specified number of micrometers (incremental) with buffer and waits for the movement to complete.
        /// </summary>
        public async Task StartIncBufferAsync(double xUm, double yUm, double zUm)
        {
            StartIncBuffer(xUm, yUm, zUm);
            await Wait(); // Wait for the movement to complete
        }

        public uint IndexOverride(AXIS axis, uint newTargetPulse)
        {
            if (!this.IsValid)
                return Hpmcstd.MCSD_ERROR_NO_DEVICE;

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
                default:
                    return Hpmcstd.MCSD_ERROR_AXIS;
            }

            return Hpmcstd.McsdIndexOverride(hController, axisCode, newTargetPulse);
        }

        public uint SpeedOverride(AXIS axis, uint newSpeedUm)
        {
            if (!this.IsValid)
                return Hpmcstd.MCSD_ERROR_NO_DEVICE;

            ushort axisCode = 0;
            double resolution;

            switch (axis)
            {
                case AXIS.X:
                    axisCode = MC104_AXIS2;
                    resolution = RESOLUTIONS_AXIS_X;
                    break;
                case AXIS.Y:
                    axisCode = MC104_AXIS1;
                    resolution = RESOLUTIONS_AXIS_Y;
                    break;
                case AXIS.Z:
                    axisCode = MC104_AXIS3;
                    resolution = RESOLUTIONS_AXIS_Z;
                    break;
                default:
                    return Hpmcstd.MCSD_ERROR_AXIS;
            }

            /// Convert speed from um/sec to pulses/sec
            double pulsesPerSec = newSpeedUm / resolution;

            /// wKind = 1 specifies that dSpeed is in Pulse/sec format.
            return Hpmcstd.McsdHiSpeedOverride(hController, axisCode, 1, pulsesPerSec);
        }
        #endregion

        #region Status Query and Error Handling
        /// <summary>
        /// Waits asynchronously until the operation is no longer busy.
        /// </summary>
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
                /// McsdClose(hDevice) function closes the device and releases any associated resources.
                Hpmcstd.McsdClose(hController);
        }

        /// <summary>
        /// Determines whether the axis is currently busy.
        /// </summary>
        public bool IsBusy()
        {
            ushort status;
            /// Calls McsdGetAxisBusy(hDevice, pwStatus) to check if the axis is busy.
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
                case AXIS.Y:
                    return (status & 0x01) > 0;
                case AXIS.X:
                    return (status & 0x02) > 0;
                case AXIS.Z:
                    return (status & 0x04) > 0;
            }

            /// If the axis is not valid, return false.
            return false;
        }

        /// <summary>
        /// Retrieves the error message corresponding to the specified error code.
        /// </summary>
        /// <param name="errorCode"></param>
        /// <returns></returns>
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
        /// Retrieves the end status for the specified axis.
        /// </summary>
        /// <param name="axis"></param>
        /// <returns></returns>
        public string GetAxisEndStatus(AXIS axis)
        {
            /// Create a new instance of the MCSDSTATUS structure to hold the status information.
            Hpmcstd.MCSDSTATUS status = new Hpmcstd.MCSDSTATUS();
            uint result;

            /// Determine which axis to query and retrieve its status.
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
        /// Retrieves the end status for all axes based on the provided status code.
        /// </summary>
        /// <param name="status"></param>
        /// <returns></returns>
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
