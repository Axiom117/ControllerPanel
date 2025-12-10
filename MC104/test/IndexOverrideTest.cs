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
        const double BASE_SPEED_UM = 6000;  // 5 mm/s

        static Microsupport controller = null;
        static List<TrajectoryPoint> trajectoryPoints = new List<TrajectoryPoint>();
        static bool isRunning = false;

        /// <summary>
        /// The vector distance (in micrometers) from the current target point at which the controller
        /// will override to the next target point.
        /// </summary>
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
                trajectoryPoints.Add(new TrajectoryPoint { X = 1000, Y = 1000, Z = 1000, Index = 0 });
                trajectoryPoints.Add(new TrajectoryPoint { X = 1500, Y = 1500, Z = 1500, Index = 1 });
                trajectoryPoints.Add(new TrajectoryPoint { X = 2000, Y = 1500, Z = 2000, Index = 2 });
                trajectoryPoints.Add(new TrajectoryPoint { X = 2500, Y = 2500, Z = 2500, Index = 3 });
                trajectoryPoints.Add(new TrajectoryPoint { X = 3000, Y = 3000, Z = 3000, Index = 4 });
                trajectoryPoints.Add(new TrajectoryPoint { X = 4000, Y = 3000, Z = 4000, Index = 5 });
                trajectoryPoints.Add(new TrajectoryPoint { X = 5000, Y = 4000, Z = 5000, Index = 6 });

                // ==========================================
                // TEST 1: Baseline PTP (Point-to-Point)
                // ==========================================
                Console.WriteLine("\n--- Starting TEST 1: Standard PTP (Expect Stops) ---");
                Console.WriteLine("Press ENTER to run PTP mode...");
                Console.ReadLine();
                await Task.Delay(1000);
                await RunPTP();

                /// Return to origin
                Console.WriteLine("Returning to (0,0,0) for next test...");
                await Task.Delay(1000);
                await controller.StartOriginAsync();
                Console.WriteLine("Homing completed.");

                await Task.Delay(2000);
                // ==========================================
                // TEST 2: CP Mode (Index Override)
                // ==========================================
                Console.WriteLine("\n--- Starting TEST 2: CP Mode (Index Override) ---");
                Console.WriteLine("Expect continuous motion without full stops between segments.");
                Console.WriteLine($"Override Trigger Distance: {OVERRIDE_DISTANCE_THRESHOLD} um");
                Console.WriteLine("Press ENTER to run CP mode...");
                Console.ReadLine();

                await RunCP();

                Console.WriteLine("\nAll Tests Completed. Press ENTER to exit.");
                Console.ReadLine();
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
        static async Task RunCP()
        {
            if (trajectoryPoints.Count < 2)
            {
                Console.WriteLine("[CP] Not enough points for trajectory.");
                return;
            }

            Stopwatch sw = Stopwatch.StartNew();

            // 1. Start first segment (Start -> Point 0)
            var firstPoint = trajectoryPoints[0];
            Console.WriteLine($"[CP] Launching initial move to Point 0 ({firstPoint.X}, {firstPoint.Y}, {firstPoint.Z}).");
            controller.SetSpeedAll(BASE_SPEED_UM);
            controller.StartIncAbsAll(firstPoint.X, firstPoint.Y, firstPoint.Z);

            for (int i = 0; i < trajectoryPoints.Count - 1; i++)
            {
                var currentTarget = trajectoryPoints[i];
                var nextTarget = trajectoryPoints[i + 1];

                Console.WriteLine($"[CP] Segment {i}->{i + 1} running. Target: ({currentTarget.X}, {currentTarget.Y}, {currentTarget.Z}). Next: ({nextTarget.X}, {nextTarget.Y}, {nextTarget.Z}). Waiting for override window.");

                /// 2. Monitor position until reaching threshold
                bool overrideTriggered = false;
                while (!overrideTriggered && controller.IsBusy())
                {
                    double[] currentPos = controller.GetPositions();

                    /// Calculate the vector distance to the current target
                    double dx = currentTarget.X - currentPos[0];
                    double dy = currentTarget.Y - currentPos[1];
                    double dz = currentTarget.Z - currentPos[2];
                    double vectorDistance = Math.Sqrt(dx * dx + dy * dy + dz * dz);

                    /// Check if the vector distance is within the override threshold
                    if (vectorDistance < OVERRIDE_DISTANCE_THRESHOLD)
                    {
                        Console.WriteLine($"[CP] Threshold reached! (Distance: {vectorDistance:F1} um). Overriding target to Point {i + 1} ({nextTarget.X}, {nextTarget.Y}, {nextTarget.Z})...");

                        /// Calculate the delta for each axix
                        double nextDx = nextTarget.X - currentTarget.X;
                        double nextDy = nextTarget.Y - currentTarget.Y;
                        double nextDz = nextTarget.Z - currentTarget.Z;

                        /// Find the largest delta to scale speed
                        double maxDisplacement = Math.Max(Math.Abs(nextDx), Math.Max(Math.Abs(nextDy), Math.Abs(nextDz)));

                        if (maxDisplacement > 1e-6) // Avoid division by zero
                        {
                            /// Calculate speed for each axis based on its displacement relative to the max displacement.
                            /// The axis with the largest displacement will move at MAX_SPEED_UM.
                            double speedX = BASE_SPEED_UM * (Math.Abs(nextDx) / maxDisplacement);
                            double speedY = BASE_SPEED_UM * (Math.Abs(nextDy) / maxDisplacement);
                            double speedZ = BASE_SPEED_UM * (Math.Abs(nextDz) / maxDisplacement);

                            /// Ensure speeds are not below a minimum threshold to prevent stalling.
                            const double MIN_SPEED_UM = 100.0;
                            if (Math.Abs(nextDx) > 1e-6) controller.HiSpeedOverride(Microsupport.AXIS.X, Math.Max(speedX, MIN_SPEED_UM));
                            if (Math.Abs(nextDy) > 1e-6) controller.HiSpeedOverride(Microsupport.AXIS.Y, Math.Max(speedY, MIN_SPEED_UM));
                            if (Math.Abs(nextDz) > 1e-6) controller.HiSpeedOverride(Microsupport.AXIS.Z, Math.Max(speedZ, MIN_SPEED_UM));

                            Console.WriteLine($"[CP] Speeds adjusted for next segment: X={speedX:F0}, Y={speedY:F0}, Z={speedZ:F0} um/s");
                        }
                        /// 3. Core: Call McsdIndexOverride for any axis where the target position changes.
                        /// This is more robust than checking IsBusy(), as an axis might be idle if its
                        /// coordinate hasn't changed in the previous segment.

                        double[] overridePos = controller.GetPositions();
                        Console.WriteLine($"[CP] Current Position before override: {overridePos[0]}, {overridePos[1]}, {overridePos[2]}.");

                        // Override X-axis if its target position changes.
                        if (Math.Abs(nextTarget.X - currentTarget.X) > 1e-6)
                        {
                            if (controller.IsBusy(Microsupport.AXIS.X))
                            {
                                controller.IndexOverride(Microsupport.AXIS.X, (uint)controller.Um2enc(Microsupport.AXIS.X, nextTarget.X));
                            }
                            else
                            {
                                // If the axis is idle, we need to start a new move to the next target
                                controller.StartIncAbs(Microsupport.AXIS.X, nextTarget.X);
                                Console.WriteLine($"[CP] X-axis was idle. Started new move to {nextTarget.X} um.");
                            }
                        }
                        else
                        {
                            controller.StopAxis(Microsupport.AXIS.X);
                            Console.WriteLine("[CP] X-axis has no displacement. Commanding deceleration stop.");
                        }

                        // Override Y-axis if its target position changes.
                        if (Math.Abs(nextTarget.Y - currentTarget.Y) > 1e-6)
                        {
                            if (controller.IsBusy(Microsupport.AXIS.Y))
                            {
                                controller.IndexOverride(Microsupport.AXIS.Y, (uint)controller.Um2enc(Microsupport.AXIS.Y, nextTarget.Y));
                            }
                            else
                            {
                                controller.StartIncAbs(Microsupport.AXIS.Y, nextTarget.Y);
                                Console.WriteLine("[CP] Y-axis was idle. Started new move with StartIncAbs.");
                            }
                        }
                        else
                        {
                            controller.StopAxis(Microsupport.AXIS.Y);
                            Console.WriteLine("[CP] Y-axis has no displacement. Commanding deceleration stop.");
                        }

                        // Override Z-axis if its target position changes.
                        if (Math.Abs(nextTarget.Z - currentTarget.Z) > 1e-6)
                        {
                            if (controller.IsBusy(Microsupport.AXIS.Z))
                            {
                                controller.IndexOverride(Microsupport.AXIS.Z, (uint)controller.Um2enc(Microsupport.AXIS.Z, nextTarget.Z));
                            }
                            else
                            {
                                controller.StartIncAbs(Microsupport.AXIS.Z, nextTarget.Z);
                                Console.WriteLine("[CP] Z-axis was idle. Started new move with StartIncAbs.");
                            }
                        }
                        else
                        {
                            controller.StopAxis(Microsupport.AXIS.Z);
                            Console.WriteLine("[CP] Z-axis has no displacement. Commanding deceleration stop.");
                        }

                        overrideTriggered = true; // Exit loop and monitor next segment
                    }

                    await Task.Delay(1);
                }

                if (!controller.IsBusy())
                {
                    double[] finalPosErr = controller.GetPositions();
                    Console.WriteLine($"Last position reached before controller went idle:{finalPosErr[0]}, {finalPosErr[1]}, {finalPosErr[2]}. Exiting.");
                    Console.WriteLine("[CP] Motion stopped unexpectedly.");
                    break;
                }

                double[] pos = controller.GetPositions();
                Console.WriteLine($"[CP] Segment {i}->{i + 1} override complete at position: {pos[0]}, {pos[1]}, {pos[2]}.");
            }

            // Wait for the final segment to complete
            await controller.Wait();
            sw.Stop();
            double[] finalPos = controller.GetPositions();
            Console.WriteLine($"Last position reached:{finalPos[0]}, {finalPos[1]}, {finalPos[2]}. Exiting.");
            Console.WriteLine($"[CP] Trajectory Complete. Total time: {sw.ElapsedMilliseconds} ms");
        }
    }
}