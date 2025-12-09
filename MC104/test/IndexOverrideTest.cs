using HpmcstdCs;
using MicrosupportController;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms.VisualStyles;

namespace IndexOverrideTest
{
    class Program
    {
        /// 1 um = 20 pulses
        const double PULSE_RATIO = 20.0;

        /// Trajectory point struct
        struct TrajectoryPoint
        {
            public double X;
            public double Y;
            public double Z;
            public int Index;
        }

        /// Speed settings (in um/s)
        const double BASE_SPEED_UM = 1000;  // 2 mm/s
        const double MAX_SPEED_UM = 8000;   // 8 mm/s

        static Microsupport controller = null;
        static List<TrajectoryPoint> trajectoryPoints = new List<TrajectoryPoint>();
        static bool isRunning = false;

        /// trajectory points
        static readonly long[] PointA = { (long)(1000 * PULSE_RATIO), (long)(1000 * PULSE_RATIO), 0 };
        static readonly long[] PointB = { (long)(1000 * PULSE_RATIO), (long)(2000 * PULSE_RATIO), 0 };
        static readonly long[] PointC = { (long)(2000 * PULSE_RATIO), (long)(2000 * PULSE_RATIO), 0 };

        /// Threshold to trigger override (as a fraction of distance)
        const double OVERRIDE_THRESHOLD = 0.8;

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

                /// Home the device before starting tests
                Console.WriteLine("Press ENTER to Home the device, or 'S' to skip...");
                var key = Console.ReadLine();
                if (key?.ToUpper() != "S")
                {
                    Console.WriteLine("Homing...");
                    await controller.StartOriginAsync();
                    Thread.Sleep(2000);
                }
                // TEST 0: Generate Trajectory Points
                Console.WriteLine("Generating trajectory points...");
                trajectoryPoints.Clear();
                trajectoryPoints.Add(new TrajectoryPoint { X = 1000, Y = 1000, Z = 0, Index = 0 });
                trajectoryPoints.Add(new TrajectoryPoint { X = 1000, Y = 2000, Z = 0, Index = 1 });
                trajectoryPoints.Add(new TrajectoryPoint { X = 2000, Y = 2000, Z = 0, Index = 2 });

                // ==========================================
                // TEST 1: Baseline PTP (Point-to-Point)
                // ==========================================
                Console.WriteLine("\n--- Starting TEST 1: Standard PTP (Expect Stops) ---");
                Console.WriteLine("Press ENTER to run PTP mode...");
                Console.ReadLine();
                await Task.Delay(1000);
                await RunPTP();

                // 回到原点准备下一次测试
                Console.WriteLine("Returning to (0,0,0) for next test...");
                MoveToAbs(controller, new long[] { 0, 0, 0 });
                WaitForMotion(controller);

                // ==========================================
                // TEST 2: CP Mode (Index Override)
                // ==========================================
                Console.WriteLine("\n--- Starting TEST 2: CP Mode (Index Override) ---");
                Console.WriteLine("Expect continuous motion without full stops between segments.");
                Console.WriteLine($"Override Trigger Threshold: {OVERRIDE_THRESHOLD * 100}%");
                Console.WriteLine("Press ENTER to run CP mode...");
                Console.ReadLine();

                RunCP(controller);

                Console.WriteLine("\nAll Tests Completed. Press ENTER to exit.");
                controller.Close(); // 假设有 Close()
                Console.ReadLine();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Exception Occurred: {ex.Message}");
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

        /// <summary>
        /// Standard PTP motion (comparison group)
        /// Logic: Move -> Wait for Stop -> Next Move
        /// </summary>
        static async Task RunPTP()
        {
            Console.WriteLine("Starting PTP tracking...");
            Stopwatch sw = Stopwatch.StartNew();

            controller.SetSpeedAll(BASE_SPEED_UM);

            for (int i = 0; i < trajectoryPoints.Count; i++)
            {
                var point = trajectoryPoints[i];
                long[] targetPos = { (long)(point.X * PULSE_RATIO), (long)(point.Y * PULSE_RATIO), (long)(point.Z * PULSE_RATIO) };

                // 执行绝对运动
                MoveToAbs(controller, targetPos);

                // 阻塞等待直到运动停止
                WaitForMotion(controller);

                Console.WriteLine($"[PTP] Reached Point {point.Index}. Motor Stopped.");
                await Task.Delay(500); // 使用 async-friendly 的 Delay
            }
        }

        /// <summary>
        /// 连续路径运动 (实验组)
        /// 逻辑：移动 -> 监控进度 -> 动态 Override 目标
        /// </summary>
        static void RunCP(MicrosupportController.Microsupport ctrl)
        {
            long[][] trajectory = { PointA, PointB, PointC };

            // 1. 启动第一段运动 (Start -> PointA)
            Console.WriteLine($"[CP] Launching initial move to Point A ({PointA[0]}, {PointA[1]})");
            MoveToAbs(ctrl, PointA);

            for (int i = 0; i < trajectory.Length - 1; i++)
            {
                long[] currentTarget = trajectory[i];
                long[] nextTarget = trajectory[i + 1];

                Console.WriteLine($"[CP] Segment {i + 1} Running... Waiting for override window.");

                // 2. 轮询循环：监控当前位置是否接近当前目标
                bool overrideTriggered = false;
                while (!overrideTriggered)
                {
                    // 获取当前位置 (假设 GetPosition 返回数组 [x, y, z])
                    long[] currentPos = GetCurrentPositions(ctrl);

                    // 计算这一段的总距离和当前进度的距离 (这里简化计算，取最大轴距)
                    // 注意：这只是一个简化的判断逻辑，实际项目中可能需要根据矢量距离计算
                    long distToTargetX = Math.Abs(currentTarget[0] - currentPos[0]);
                    long distToTargetY = Math.Abs(currentTarget[1] - currentPos[1]);

                    // 假设只要有一个轴接近目标 (剩余距离 < 总距离的20%)，就触发 Override
                    // 这里我们硬编码判断：如果距离目标小于 200 pulses (假设总长1000)
                    bool closeEnough = (distToTargetX < 200) && (distToTargetY < 200);

                    if (closeEnough)
                    {
                        Console.WriteLine($"[CP] Threshold reached! Overriding target to Next Point ({nextTarget[0]}, {nextTarget[1]})...");

                        // 3. 核心：调用 McsdIndexOverride
                        // 注意：你需要对每个轴分别调用
                        PerformOverride(ctrl, 0, nextTarget[0]); // X轴
                        PerformOverride(ctrl, 1, nextTarget[1]); // Y轴
                        PerformOverride(ctrl, 2, nextTarget[2]); // Z轴

                        overrideTriggered = true; // 跳出循环，进入下一段监控
                    }

                    // 防止死循环占用过多CPU
                    Thread.Sleep(1);

                    // 安全检查：如果电机已经意外停止，则退出循环
                    if (IsMotionDone(ctrl))
                    {
                        Console.WriteLine("[CP] Warning: Motion stopped before override could happen.");
                        break;
                    }
                }
            }

            // 等待最后一段运动完成
            WaitForMotion(ctrl);
            Console.WriteLine("[CP] Trajectory Complete.");
        }

        // ==========================================
        // 下面是 Helper 方法，用于适配您的特定 Controller 类接口
        // 您可能需要根据您的 MicrosupportController.cs 实际定义进行微调
        // ==========================================

        static void MoveToAbs(MicrosupportController.Microsupport ctrl, long[] pos)
        {
            // 假设 DriveMove 接受轴索引和脉冲数
            // 如果您的接口是同时驱动所有轴，请修改此处
            ctrl.DriveMove(0, (int)pos[0]);
            ctrl.DriveMove(1, (int)pos[1]);
            ctrl.DriveMove(2, (int)pos[2]);
        }

        static void PerformOverride(MicrosupportController.Microsupport ctrl, short axisIndex, long newTargetPulse)
        {
            // 这是关键部分：调用底层的 McsdIndexOverride
            // 如果您的 Controller 类没有直接暴露这个方法，您可能需要：
            // 1. 在 Controller 类中添加 public void Override(...)
            // 2. 或者直接调用 Hpmcstd.McsdIndexOverride(...)

            // 示例调用 (假设您封装了一个方法):
            // ctrl.OverrideTarget(axisIndex, (int)newTargetPulse);

            // 示例直接调用 P/Invoke (如果在同一程序集):
            // 0 是 Board ID, axisIndex 是轴号, newTargetPulse 是新数据
            // int ret = Hpmcstd.McsdIndexOverride(0, (ushort)axisIndex, (int)newTargetPulse);

            // 假设您已经在 MicrosupportController.cs 中写了一个 helper:
            ctrl.McsdIndexOverride(0, (ushort)axisIndex, (int)newTargetPulse);
        }

        static long[] GetCurrentPositions(MicrosupportController.Microsupport ctrl)
        {
            // 模拟获取位置
            long x = ctrl.GetPosition(0);
            long y = ctrl.GetPosition(1);
            long z = ctrl.GetPosition(2);
            return new long[] { x, y, z };
        }

        static bool IsMotionDone(MicrosupportController.Microsupport ctrl)
        {
            // 检查 X, Y, Z 是否都停止
            return ctrl.GetStatus(0) == 0 && ctrl.GetStatus(1) == 0 && ctrl.GetStatus(2) == 0;
        }

        static void WaitForMotion(MicrosupportController.Microsupport ctrl)
        {
            while (!IsMotionDone(ctrl))
            {
                Thread.Sleep(10);
            }
        }
    }
}