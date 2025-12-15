using HpmcstdCs;
using MicrosupportController;
using SmoothTrajectoryTest;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace IndexOverrideTest
{
    class Program
    {
        /// Trajectory point struct
        struct TrajectoryPoint
        {
            public double X;
            public double Y;
            public double Z;
            public int Index;
        }

        /// Speed settings (in um/s)
        const double BASE_SPEED_UM = 2500;  // 2500 mm/s

        static Microsupport controller = null;
        static List<TrajectoryPoint> trajectoryPoints = new List<TrajectoryPoint>();

        /// The vector distance (in micrometers) from the current target point at which the controller will override to the next target point.
        const double OVERRIDE_DISTANCE_THRESHOLD = 250.0; // um

        static async Task Main(string[] args)
        {
            Console.WriteLine("========================================");
            Console.WriteLine("   McsdIndexOverride Unit Test Script   ");
            Console.WriteLine("========================================");

            try
            {
                if (!await InitializeController())
                {
                    Console.WriteLine("❌ Controller Initialization Failed. Please check connection.");
                    return;
                }

                Console.WriteLine("✅ Controller Initialized.");

                // ==========================================
                // TEST 0: Generate Trajectory Points
                // ==========================================
                Console.WriteLine("Generating trajectory points...");
                trajectoryPoints.Clear();
                trajectoryPoints.Add(new TrajectoryPoint { X = 0, Y = 0, Z = 0, Index = 0 }); // Origin
                trajectoryPoints.Add(new TrajectoryPoint { X = 245, Y = 681, Z = 245, Index = 1 });
                trajectoryPoints.Add(new TrajectoryPoint { X = 507, Y = 1376, Z = 597, Index = 2 });
                trajectoryPoints.Add(new TrajectoryPoint { X = 776, Y = 2059, Z = 776, Index = 3 });
                trajectoryPoints.Add(new TrajectoryPoint { X = 1052, Y = 2734, Z = 1052, Index = 4 });
                trajectoryPoints.Add(new TrajectoryPoint { X = 1340, Y = 3410, Z = 1340, Index = 5 });
                trajectoryPoints.Add(new TrajectoryPoint { X = 1633, Y = 4071, Z = 1633, Index = 6 });
                trajectoryPoints.Add(new TrajectoryPoint { X = 1934, Y = 4725, Z = 1934, Index = 7 });
                trajectoryPoints.Add(new TrajectoryPoint { X = 2246, Y = 5378, Z = 2246, Index = 8 });
                trajectoryPoints.Add(new TrajectoryPoint { X = 2563, Y = 6017, Z = 2563, Index = 9 });

                // ==========================================
                // TEST 1: Baseline PTP (Point-to-Point)
                // ==========================================
                Console.WriteLine("\n--- Starting TEST 1: Standard PTP (Expect Stops) ---");
                await Task.Delay(1000);
                await RunPTP();

                /// Return to origin for next test
                await Task.Delay(1000);
                await controller.StartOriginAsync();
                Console.WriteLine("Homing completed.");

                // ==========================================
                // TEST 2: CP Mode (Index Override)
                // ==========================================
                Console.WriteLine("\n--- Starting TEST 2: CP Mode (Index Override) ---");
                await Task.Delay(1000);
                await RunCP();

                /// Return to origin after test
                await Task.Delay(1000);
                await controller.StartOriginAsync();
                Console.WriteLine("Homing completed.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Exception Occurred: {ex.Message}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");
            }
            finally
            {
                if (controller != null)
                {
                    controller.Terminate();
                }
            }
        }

        /// <summary>
        /// Initializes the controller and performs homing of all axes asynchronously.
        /// </summary>
        static async Task<bool> InitializeController()
        {
            Console.WriteLine("Initializing controller...");

            controller = Microsupport.GetInstance("MC2", 1);
            if (controller == null || !controller.IsInitialized)
            {
                Console.WriteLine("Failed to get controller instance!");
                return false;
            }

            if (!controller.IsConnected())
            {
                Console.WriteLine("Controller is not connected!");
                return false;
            }

            Console.WriteLine("Controller initialized successfully!");

            Console.WriteLine("Homing axes...");
            await controller.StartOriginAsync();
            Console.WriteLine("Homing completed!");

            return true;
        }

        /// <summary>
        /// Moves all axes to the specified absolute positions asynchronously.
        /// </summary>
        static async Task Step(double x, double y, double z)
        {
            controller.SetSpeedAll(BASE_SPEED_UM);
            controller.StartIncAbsAll(x, y, z);
            await controller.Wait();
        }

        /// <summary>
        /// Standard PTP motion (comparison group)
        /// Logic: Move -> Wait for Stop -> Next Move
        /// </summary>
        static async Task RunPTP()
        {
            Console.WriteLine("Starting PTP tracking...");
            Stopwatch sw = Stopwatch.StartNew();

            for (int i = 0; i < trajectoryPoints.Count; i++)
            {
                await Step(trajectoryPoints[i].X, trajectoryPoints[i].Y, trajectoryPoints[i].Z);

                Console.WriteLine($"[PTP] Reached Point {i}. Motor Stopped.");
            }
            sw.Stop();
            Console.WriteLine($"[PTP] Total time: {sw.ElapsedMilliseconds} ms");
        }

        /// <summary>
        /// Continuous Path (CP) motion using Index Override
        /// Logic: Move -> Monitor Position -> Override Target -> Next Segment
        /// </summary>
        /// <summary>
        /// Executes Continuous Path (CP) motion optimized for dense trajectories.
        /// Features:
        /// 1. Pure high-frequency polling (no sleep) for maximum responsiveness.
        /// 2. "Stop-and-Restart" strategy for non-monotonic segments to ensure synchronization.
        /// 3. Cumulative pulse calculation to prevent position drift.
        /// </summary>
        static async Task RunCP()
        {
            if (trajectoryPoints.Count < 2)
            {
                Console.WriteLine("[CP] Not enough points for trajectory.");
                return;
            }

            Console.WriteLine("[CP] Starting High-Precision CP Motion...");
            Stopwatch sw = Stopwatch.StartNew();

            /// 1. Configure linear acceleration mode
            /// Crucial: S-Curve mode causes vibrations during overrides. Linear mode is required.
            controller.SetSpeedAll(BASE_SPEED_UM);

            /// 'chainStartPoint' acts as the reference origin for the current continuous move chain.
            /// It resets only when the motion fully stops and restarts.
            TrajectoryPoint chainStartPoint = trajectoryPoints[0];

            /// 2. Start the initial segment (P0 -> P1) immediately
            /// Assumption: The end-effector is already physically at Point 0.
            Console.WriteLine($"[CP] Starting initial segment (Point 0 -> Point 1)...");
            await StartSegmentSync(trajectoryPoints[0], trajectoryPoints[1]);

            /// 3. Iterate through subsequent segments
            for (int i = 1; i < trajectoryPoints.Count - 1; i++)
            {
                var currentTarget = trajectoryPoints[i];     // Target of the currently active move
                var nextTarget = trajectoryPoints[i + 1];    // Target to override TO
                var prevTarget = trajectoryPoints[i - 1];    // Origin of the current move

                /// --- Continuity Check ---
                /// Evaluate if the transition from (Prev->Curr) to (Curr->Next) maintains directional consistency.
                bool isContinuous = CheckContinuity(prevTarget, currentTarget, nextTarget);

                if (isContinuous)
                {
                    /// [CASE A] Continuous Motion
                    // Direction is maintained. We can override "on the fly".
                    // Console.WriteLine($"[CP] Segment {i}->{i+1}: Continuous. Polling for override...");

                    // Use pure busy-polling to catch the override window
                    bool overrideSuccess = WaitForOverrideWindowAndExecute_HighFreq(currentTarget, nextTarget, chainStartPoint);

                    if (!overrideSuccess)
                    {
                        // Fallback: If we missed the window (e.g., system lag), the motor will stop at 'currentTarget'.
                        // We must treat this as a forced restart to regain synchronization.
                        Console.WriteLine($"[CP] WARNING: Missed override window at Point {i}. Resyncing...");
                        await controller.Wait(); // Ensure full stop
                        chainStartPoint = currentTarget; // Reset reference
                        await StartSegmentSync(currentTarget, nextTarget);
                    }
                }
                else
                {
                    // [CASE B] Discontinuous Motion (Stop-and-Restart)
                    // Axis reversal or stop detected. We MUST stop at 'currentTarget' to ensure physical synchronization.
                    Console.WriteLine($"[CP] Segment {i}->{i + 1}: Discontinuous (Inflection/Stop). Stopping at Point {i}.");

                    // 1. Let the current move finish naturally (reach P_curr and stop).
                    await controller.Wait();

                    // 2. Update the chain reference to the current stopped position.
                    chainStartPoint = currentTarget;

                    // 3. Restart synchronized motion for the next segment.
                    // Console.WriteLine($"[CP] Restarting motion to Point {i+1}...");
                    await StartSegmentSync(currentTarget, nextTarget);
                }
            }

            // Wait for the final segment to complete
            await controller.Wait();
            sw.Stop();

            double[] finalPos = controller.GetPositions();
            Console.WriteLine($"[CP] Motion Complete. Final Pos: ({finalPos[0]:F1}, {finalPos[1]:F1}, {finalPos[2]:F1}). Total Time: {sw.ElapsedMilliseconds} ms");
        }

        /// <summary>
        /// Checks if the motion vector allows for a continuous override.
        /// Returns false if any axis changes direction (Sign), stops, or starts from a stop.
        /// </summary>
        static bool CheckContinuity(TrajectoryPoint p1, TrajectoryPoint p2, TrajectoryPoint p3)
        {
            // Calculate delta vectors
            double dx1 = p2.X - p1.X;
            double dy1 = p2.Y - p1.Y;
            double dz1 = p2.Z - p1.Z;

            double dx2 = p3.X - p2.X;
            double dy2 = p3.Y - p2.Y;
            double dz2 = p3.Z - p2.Z;

            // Math.Sign returns: 1 (positive), -1 (negative), 0 (no movement).
            // Any change in sign implies a necessary velocity zero-crossing (stop).
            if (Math.Sign(dx1) != Math.Sign(dx2)) return false;
            if (Math.Sign(dy1) != Math.Sign(dy2)) return false;
            if (Math.Sign(dz1) != Math.Sign(dz2)) return false;

            return true;
        }

        /// <summary>
        /// Calculates and applies speed overrides for each axis based on displacement, but only if the speed differences are significant.
        /// </summary>
        static void AdjustSpeedsConditionally(double dx, double dy, double dz)
        {
            const double MIN_SPEED_UM = 100.0;
            const double SPEED_CHANGE_THRESHOLD = 0.10; // 10%

            double maxDisplacement = Math.Max(Math.Abs(dx), Math.Max(Math.Abs(dy), Math.Abs(dz)));
            if (maxDisplacement < 1e-6) return; // No movement

            // Calculate target speeds
            double speedX = BASE_SPEED_UM * (Math.Abs(dx) / maxDisplacement);
            double speedY = BASE_SPEED_UM * (Math.Abs(dy) / maxDisplacement);
            double speedZ = BASE_SPEED_UM * (Math.Abs(dz) / maxDisplacement);

            // Find min/max of the calculated speeds to check their relative difference
            double minSpeed = Math.Min(speedX, Math.Min(speedY, speedZ));
            double maxSpeed = Math.Max(speedX, Math.Max(speedY, speedZ));

            // If max speed is zero, all speeds are zero, no need to proceed
            if (maxSpeed < 1e-6) return;

            // Check if the relative difference between min and max speed is less than the threshold
            if ((maxSpeed - minSpeed) / maxSpeed < SPEED_CHANGE_THRESHOLD)
            {
                Console.WriteLine($"[CP] Speed differences are within {SPEED_CHANGE_THRESHOLD:P0}, skipping override.");
                return; // Speeds are similar, no need to override
            }

            // Apply speed override as differences are significant
            Console.WriteLine($"[CP] Speeds adjusted for next segment: X={speedX:F0}, Y={speedY:F0}, Z={speedZ:F0} um/s");
            if (Math.Abs(dx) > 1e-6) controller.SpeedOverride(Microsupport.AXIS.X, (uint)Math.Max(speedX, MIN_SPEED_UM));
            if (Math.Abs(dy) > 1e-6) controller.SpeedOverride(Microsupport.AXIS.Y, (uint)Math.Max(speedY, MIN_SPEED_UM));
            if (Math.Abs(dz) > 1e-6) controller.SpeedOverride(Microsupport.AXIS.Z, (uint)Math.Max(speedZ, MIN_SPEED_UM));
        }

        /// <summary>
        /// Starts a synchronized movement for all axes using StartIncAbsAll.
        /// This ensures all axes trigger their acceleration ramp simultaneously.
        /// </summary>
        static async Task StartSegmentSync(TrajectoryPoint start, TrajectoryPoint end)
        {
            // Calculate relative increment for StartIncAbsAll
            double dx = end.X - start.X;
            double dy = end.Y - start.Y;
            double dz = end.Z - start.Z;

            AdjustSpeedsConditionally(dx, dy, dz);

            controller.StartIncAll(Microsupport.DIRECTION.FORWARD, dx,
                Microsupport.DIRECTION.FORWARD, dy,
                Microsupport.DIRECTION.FORWARD, dz);

            // Minimal yield to ensure command is flushed to the controller
            await Task.Delay(1);
        }

        /// <summary>
        /// High-frequency polling loop to trigger IndexOverride.
        /// No Sleep/Delay strategies are used; prioritizes timing accuracy over CPU usage.
        /// </summary>
        /// <param name="currentTarget">The target of the current active command.</param>
        /// <param name="nextTarget">The target coordinate for the override.</param>
        /// <param name="chainStart">The absolute reference point for pulse accumulation.</param>
        /// <returns>True if override was triggered, False if timed out or missed.</returns>
        static bool WaitForOverrideWindowAndExecute_HighFreq(TrajectoryPoint currentTarget, TrajectoryPoint nextTarget, TrajectoryPoint chainStart)
        {
            // Safety timeout to prevent infinite locking if hardware hangs
            Stopwatch safetyTimer = Stopwatch.StartNew();
            long timeoutLimit = 2000; // 2 seconds safety limit

            // Pre-calculate cumulative override values outside the loop to save CPU cycles during polling
            // McsdIndexOverride requires: Total Pulses from Chain Start
            uint pulseX = 0, pulseY = 0, pulseZ = 0;

            // Only calculate if the axis is actually part of the movement chain (dx != 0)
            // (Though CheckContinuity ensures we don't get here if state changes, calculating 0 is harmless)
            double totalDistX = Math.Abs(nextTarget.X - chainStart.X);
            double totalDistY = Math.Abs(nextTarget.Y - chainStart.Y);
            double totalDistZ = Math.Abs(nextTarget.Z - chainStart.Z);

            pulseX = (uint)controller.Um2enc(Microsupport.AXIS.X, totalDistX);
            pulseY = (uint)controller.Um2enc(Microsupport.AXIS.Y, totalDistY);
            pulseZ = (uint)controller.Um2enc(Microsupport.AXIS.Z, totalDistZ);

            // Pre-calculate deltas for loop checking
            // We use Squared Distance to avoid Math.Sqrt() inside the hot loop for performance
            double thresholdSq = OVERRIDE_DISTANCE_THRESHOLD * OVERRIDE_DISTANCE_THRESHOLD;

            while (controller.IsBusy())
            {
                if (safetyTimer.ElapsedMilliseconds > timeoutLimit) return false;

                double[] currentPos = controller.GetPositions();

                double dx = currentTarget.X - currentPos[0];
                double dy = currentTarget.Y - currentPos[1];
                double dz = currentTarget.Z - currentPos[2];

                // Vector distance squared check
                double distSq = dx * dx + dy * dy + dz * dz;

                if (distSq <= thresholdSq)
                {
                    /// --- WINDOW REACHED: EXECUTE OVERRIDE IMMEDIATELY ---
                    Console.WriteLine($"[CP] Override Window Reached at Pos ({currentPos[0]:F1}, {currentPos[1]:F1}, {currentPos[2]:F1}. Executing Override to ({nextTarget.X}, {nextTarget.Y}, {nextTarget.Z})");

                    // Calculate displacement for the *next* segment to determine speeds
                    double next_dx = nextTarget.X - currentTarget.X;
                    double next_dy = nextTarget.Y - currentTarget.Y;
                    double next_dz = nextTarget.Z - currentTarget.Z;

                    AdjustSpeedsConditionally(next_dx, next_dy, next_dz);

                    // We only send override commands to axes that are moving.
                    // Static axes (delta approx 0) are ignored to prevent errors.

                    if (Math.Abs(nextTarget.X - currentTarget.X) > 0.001)
                        controller.IndexOverride(Microsupport.AXIS.X, pulseX);

                    if (Math.Abs(nextTarget.Y - currentTarget.Y) > 0.001)
                        controller.IndexOverride(Microsupport.AXIS.Y, pulseY);

                    if (Math.Abs(nextTarget.Z - currentTarget.Z) > 0.001)
                        controller.IndexOverride(Microsupport.AXIS.Z, pulseZ);

                    // Console.WriteLine($"[CP] Override >> Point {nextTarget.Index}");
                    return true;
                }
                Console.WriteLine($"[CP] Polling... Current Pos ({currentPos[0]:F1}, {currentPos[1]:F1}, {currentPos[2]:F1}), DistSq: {distSq:F1}");

                // Extremely short spin to prevent complete CPU starvation while maintaining near-RT response
                Thread.SpinWait(1);
            }

            return false; // Controller stopped before threshold was reached
        }

    }
}