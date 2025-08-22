using MicrosupportController;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using HpmcstdCs;

namespace SmoothTrajectoryTest
{
    class Program
    {
        // 轨迹点结构
        struct TrajectoryPoint
        {
            public double X;
            public double Y;
            public double Z;
            public int Index;
        }

        // 基础速度 (um/sec)
        const double BASE_SPEED_UM = 1000;  // 2 mm/s
        const double MAX_SPEED_UM = 8000;   // 8mm/s

        static Microsupport controller = null;
        static List<TrajectoryPoint> trajectoryPoints = new List<TrajectoryPoint>();
        static bool isRunning = false;

        static async Task Main(string[] args)
        {
            Console.WriteLine("=== Smooth Trajectory Tracking Test ===");
            Console.WriteLine($"Test started at: {DateTime.Now:yyyy-MM-dd HH:mm:ss}\n");

            try
            {
                // 1. 初始化控制器
                if (!await InitializeController())
                {
                    Console.WriteLine("Failed to initialize controller!");
                    return;
                }

                // 2. 生成测试轨迹（从原点开始的圆弧）
                GenerateArcTrajectoryFromOrigin();

                // 3. 显示轨迹信息
                DisplayTrajectoryInfo();

                // 4. 自动执行PTP模式测试
                Console.WriteLine("\n--- PTP Mode (Traditional) Test ---");
                await Task.Delay(1000);
                await ExecutePTPTracking();

                // 5. 返回原点准备下一次测试
                Console.WriteLine("\nReturning to origin...");
                await controller.StartOriginAsync();
                await Task.Delay(2000);

                // 6. 自动执行Smooth CP模式测试
                //Console.WriteLine("\n--- Smooth CP Mode (New Algorithm) Test ---");
                //await Task.Delay(1000);
                //await ExecuteSmoothTracking_Improved();

                // 7. 显示测试总结
                DisplayTestSummary();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nError occurred: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
            }
            finally
            {
                Cleanup();
            }

            Console.WriteLine($"\nTest completed at: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
            Console.WriteLine("\nTest finished. Program will exit in 5 seconds...");
            await Task.Delay(5000);
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

        static void GenerateArcTrajectoryFromOrigin()
        {
            Console.WriteLine("\nGenerating arc trajectory from origin...");

            // 生成从原点开始的圆弧轨迹（1/4圆）
            double radius = 10000; // 10mm

            int numPoints = 500;
            for (int i = 0; i <= numPoints; i++)
            {
                double angle = (Math.PI / 2) * i / numPoints;  // 0 to PI/2

                TrajectoryPoint point = new TrajectoryPoint
                {
                    X = radius * (1 - Math.Cos(angle)),  // 从0开始
                    Y = radius * Math.Sin(angle),         // 从0开始
                    Z = 0,                                // Z保持在0
                    Index = i
                };

                trajectoryPoints.Add(point);
            }

            // 验证轨迹
            Console.WriteLine($"First few points:");
            for (int i = 0; i < Math.Min(5, trajectoryPoints.Count); i++)
            {
                var p = trajectoryPoints[i];
                Console.WriteLine($"  Point {i}: ({p.X:F1}, {p.Y:F1}, {p.Z:F1})");
            }

            Console.WriteLine($"Generated {trajectoryPoints.Count} trajectory points");
        }

        static void DisplayTrajectoryInfo()
        {
            Console.WriteLine("\nTrajectory Information:");
            Console.WriteLine($"Start: ({trajectoryPoints[0].X:F1}, {trajectoryPoints[0].Y:F1}, {trajectoryPoints[0].Z:F1}) um");
            Console.WriteLine($"End: ({trajectoryPoints[trajectoryPoints.Count - 1].X:F1}, {trajectoryPoints[trajectoryPoints.Count - 1].Y:F1}, {trajectoryPoints[trajectoryPoints.Count - 1].Z:F1}) um");

            double totalDistance = 0;
            for (int i = 1; i < trajectoryPoints.Count; i++)
            {
                double dx = trajectoryPoints[i].X - trajectoryPoints[i - 1].X;
                double dy = trajectoryPoints[i].Y - trajectoryPoints[i - 1].Y;
                double dz = trajectoryPoints[i].Z - trajectoryPoints[i - 1].Z;
                totalDistance += Math.Sqrt(dx * dx + dy * dy + dz * dz);
            }
            Console.WriteLine($"Total path length: {totalDistance:F1} um");
        }

        static long ptpExecutionTime = 0;
        static long smoothExecutionTime = 0;
        static double ptpFinalError = 0;
        static double smoothFinalError = 0;

        static async Task ExecutePTPTracking()
        {
            Console.WriteLine("Starting PTP tracking...");
            Stopwatch sw = Stopwatch.StartNew();

            controller.SetSpeedAll(BASE_SPEED_UM);

            for (int i = 0; i < trajectoryPoints.Count; i++)
            {
                var point = trajectoryPoints[i];
                controller.StartAbsAll(point.X, point.Y, point.Z);
                await controller.Wait();

                if (i % 10 == 0)
                {
                    var pos = controller.GetPositions();
                    Console.WriteLine($"PTP Progress: {i}/{trajectoryPoints.Count} - Actual: ({pos[0]:F1}, {pos[1]:F1}, {pos[2]:F1})");
                }
            }

            sw.Stop();
            ptpExecutionTime = sw.ElapsedMilliseconds;

            var endPoint = trajectoryPoints[trajectoryPoints.Count - 1];
            var finalPos = controller.GetPositions();
            double errorX = finalPos[0] - endPoint.X;
            double errorY = finalPos[1] - endPoint.Y;
            double errorZ = finalPos[2] - endPoint.Z;
            ptpFinalError = Math.Sqrt(errorX * errorX + errorY * errorY + errorZ * errorZ);

            Console.WriteLine($"PTP tracking completed in {ptpExecutionTime} ms");
            Console.WriteLine($"PTP final position error: {ptpFinalError:F2} um");
            Console.WriteLine($"Final position: ({finalPos[0]:F1}, {finalPos[1]:F1}, {finalPos[2]:F1})");
        }

        static async Task ExecuteSmoothTracking_Improved()
        {
            Console.WriteLine("Starting improved smooth tracking...");
            Stopwatch sw = Stopwatch.StartNew();

            // 1. 确保在起点
            var startPoint = trajectoryPoints[0];
            controller.StartAbsAll(startPoint.X, startPoint.Y, startPoint.Z);
            await controller.Wait();

            Console.WriteLine($"Starting from: ({startPoint.X:F1}, {startPoint.Y:F1}, {startPoint.Z:F1})");

            // 2. 获取终点位置
            var endPoint = trajectoryPoints[trajectoryPoints.Count - 1];
            Console.WriteLine($"Target endpoint: ({endPoint.X:F1}, {endPoint.Y:F1}, {endPoint.Z:F1})");

            // 3. 计算初始运动方向（从第0点到第1点的方向）
            var initialTarget = trajectoryPoints[1];
            double initialDx = initialTarget.X - startPoint.X;
            double initialDy = initialTarget.Y - startPoint.Y;
            double initialDz = initialTarget.Z - startPoint.Z;
            double initialDistance = Math.Sqrt(initialDx * initialDx + initialDy * initialDy + initialDz * initialDz);

            Console.WriteLine($"Initial direction: dX={initialDx:F1}, dY={initialDy:F1}, dZ={initialDz:F1} (Distance: {initialDistance:F1} um)");

            // 4. 根据初始方向设置各轴速度
            if (initialDistance > 0)
            {
                double speedX = BASE_SPEED_UM * Math.Abs(initialDx) / initialDistance;
                double speedY = BASE_SPEED_UM * Math.Abs(initialDy) / initialDistance;
                double speedZ = BASE_SPEED_UM * Math.Abs(initialDz) / initialDistance;

                // 确保速度不会太低
                speedX = Math.Max(speedX, 100);
                speedY = Math.Max(speedY, 100);
                speedZ = Math.Max(speedZ, 100);

                Console.WriteLine($"Initial speeds - X: {speedX:F1}, Y: {speedY:F1}, Z: {speedZ:F1} um/s");
                Console.WriteLine($"Initial displacement - dX: {initialDx:F1}, dY: {initialDy:F1}, dZ: {initialDz:F1}");

                controller.SetSpeed(Microsupport.AXIS.X, speedX);
                controller.SetSpeed(Microsupport.AXIS.Y, speedY);
                controller.SetSpeed(Microsupport.AXIS.Z, speedZ);
            }
            else
            {
                controller.SetSpeedAll(BASE_SPEED_UM);
            }

            // 5. 启动运动到终点
            Console.WriteLine($"Starting smooth motion to endpoint...");

            // 分别启动各轴，确保同时开始
            controller.StartAbs(Microsupport.AXIS.X, endPoint.X);
            controller.StartAbs(Microsupport.AXIS.Y, endPoint.Y);
            controller.StartAbs(Microsupport.AXIS.Z, endPoint.Z);

            // 6. 在运动过程中动态调整速度
            isRunning = true;
            int currentTargetIndex = 1;
            int waypointReached = 0;
            int adjustmentCount = 0;

            // 创建速度调整任务
            while (isRunning)
            {
                // 检查是否还在运动
                if (!controller.IsBusy())
                {
                    Console.WriteLine("Motion completed, stopping speed adjustments.");
                    break;
                }

                // 获取当前位置
                var currentPos = controller.GetPositions();

                // 找到最近的未到达的轨迹点
                while (currentTargetIndex < trajectoryPoints.Count)
                {
                    var targetPoint = trajectoryPoints[currentTargetIndex];
                    double dx = targetPoint.X - currentPos[0];
                    double dy = targetPoint.Y - currentPos[1];
                    double dz = targetPoint.Z - currentPos[2];
                    double distance = Math.Sqrt(dx * dx + dy * dy + dz * dz);

                    if (distance < 10) // 10 um threshold
                    {
                        currentTargetIndex++;
                        waypointReached++;
                        if (waypointReached % 10 == 0)
                        {
                            Console.WriteLine($"Progress: {waypointReached}/{trajectoryPoints.Count-1} waypoints");
                        }
                    }
                    else
                    {
                        break;
                    }
                }

                // 如果还有目标点，调整速度指向下一个目标，直到最后一个点之前（n-1）
                if (currentTargetIndex < trajectoryPoints.Count - 1)
                {
                    var targetPoint = trajectoryPoints[currentTargetIndex];
                    double dx = targetPoint.X - currentPos[0];
                    double dy = targetPoint.Y - currentPos[1];
                    double dz = targetPoint.Z - currentPos[2];
                    double distance = Math.Sqrt(dx * dx + dy * dy + dz * dz);

                    if (distance > 0)
                    {
                        // 计算速度分量
                        double speedX = BASE_SPEED_UM * Math.Abs(dx) / distance;
                        double speedY = BASE_SPEED_UM * Math.Abs(dy) / distance;
                        double speedZ = BASE_SPEED_UM * Math.Abs(dz) / distance;

                        // 限制最小速度
                        speedX = Math.Max(speedX, 50);
                        speedY = Math.Max(speedY, 50);
                        speedZ = Math.Max(speedZ, 50);

                        // 应用速度调整
                        controller.HiSpeedOverride(Microsupport.AXIS.X, speedX);
                        controller.HiSpeedOverride(Microsupport.AXIS.Y, speedY);
                        controller.HiSpeedOverride(Microsupport.AXIS.Z, speedZ);

                        adjustmentCount++;
                        if (adjustmentCount % 10 == 0)
                        {
                            Console.WriteLine($"Pos: ({currentPos[0]:F1}, {currentPos[1]:F1}) -> Target {currentTargetIndex}: ({targetPoint.X:F1}, {targetPoint.Y:F1}) | Speeds: X={speedX:F0}, Y={speedY:F0}");
                        }
                    }
                }
                await Task.Delay(0); // 更频繁的调整
            }

            // 等待运动完成
            await controller.Wait();
            isRunning = false;

            sw.Stop();
            smoothExecutionTime = sw.ElapsedMilliseconds;

            // 计算最终位置误差
            var finalPos = controller.GetPositions();
            double errorX = finalPos[0] - endPoint.X;
            double errorY = finalPos[1] - endPoint.Y;
            double errorZ = finalPos[2] - endPoint.Z;
            smoothFinalError = Math.Sqrt(errorX * errorX + errorY * errorY + errorZ * errorZ);

            Console.WriteLine($"Smooth tracking completed in {smoothExecutionTime} ms");
            Console.WriteLine($"Total speed adjustments: {adjustmentCount}");
            Console.WriteLine($"Waypoints reached: {waypointReached}/{trajectoryPoints.Count - 1}");
            Console.WriteLine($"Smooth final position error: {smoothFinalError:F2} um");
            Console.WriteLine($"Final position: ({finalPos[0]:F1}, {finalPos[1]:F1}, {finalPos[2]:F1})");
        }

        static void DisplayTestSummary()
        {
            Console.WriteLine("\n=== Test Summary ===");
            Console.WriteLine($"Trajectory points: {trajectoryPoints.Count}");
            Console.WriteLine("\nPTP Mode Results:");
            Console.WriteLine($"  - Execution time: {ptpExecutionTime} ms");
            Console.WriteLine($"  - Position error: {ptpFinalError:F2} um");

            Console.WriteLine("\nSmooth CP Mode Results:");
            Console.WriteLine($"  - Execution time: {smoothExecutionTime} ms");
            Console.WriteLine($"  - Position error: {smoothFinalError:F2} um");

            if (smoothExecutionTime > 0 && ptpExecutionTime > 0)
            {
                double timeImprovement = ((double)(ptpExecutionTime - smoothExecutionTime) / ptpExecutionTime) * 100;
                Console.WriteLine($"\nPerformance Improvement:");
                Console.WriteLine($"  - Time reduction: {timeImprovement:F1}%");
                Console.WriteLine($"  - Speed increase: {(double)ptpExecutionTime / smoothExecutionTime:F2}x faster");
            }
        }

        static void Cleanup()
        {
            if (controller != null)
            {
                Console.WriteLine("\nTerminating controller...");
                controller.Terminate();
            }
        }
    }

    // 扩展方法
    public static class MicrosupportExtensions
    {
        public static async Task StartAbsAllAsync(this Microsupport controller, double x, double y, double z)
        {
            await Task.Run(() => controller.StartAbsAll(x, y, z));
        }

        public static uint HiSpeedOverride(this Microsupport controller, Microsupport.AXIS axis, double speedUm)
        {
            if (controller.IsValid)
            {
                ushort axisCode = 0;
                switch (axis)
                {
                    case Microsupport.AXIS.X:
                        axisCode = 1; // MC104_AXIS1
                        break;
                    case Microsupport.AXIS.Y:
                        axisCode = 0; // MC104_AXIS2
                        break;
                    case Microsupport.AXIS.Z:
                        axisCode = 2; // MC104_AXIS3
                        break;
                }

                double resolution = 0.05; // um/pulse
                double pulsesPerSec = speedUm / resolution;

                return Hpmcstd.McsdHiSpeedOverride(
                    controller.GetControllerHandle(),
                    axisCode,
                    1,  // Pulse/Sec格式
                    pulsesPerSec
                );
            }
            return Hpmcstd.MCSD_ERROR_NO_DEVICE;
        }

        private static uint GetControllerHandle(this Microsupport controller)
        {
            var field = typeof(Microsupport).GetField("hController",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            return (uint)field.GetValue(controller);
        }
    }
}