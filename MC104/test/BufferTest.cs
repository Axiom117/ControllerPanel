using System;
using System.Threading.Tasks;
using MicrosupportController;
using HpmcstdCs;
using System.Collections.Generic;

namespace MC104.Tests
{
    /// <summary>
    /// 验证Buffer同轴命令覆盖问题
    /// </summary>
    public class BufferVerificationTest
    {
        private Microsupport controller;
        private const string CONTROLLER_NAME = "MC1";
        private const int DEVICE_ID = 3;

        public BufferVerificationTest()
        {
            controller = Microsupport.GetInstance(CONTROLLER_NAME, DEVICE_ID);
            if (controller == null || !controller.IsInitialized)
            {
                throw new Exception($"Failed to initialize controller: {controller?.InitializationError}");
            }
        }

        private void PrintPosition(string context)
        {
            var pos = controller.GetPositions();
            var posEnc = controller.GetPositionsEnc();
            Console.WriteLine($"{context}:");
            Console.WriteLine($"  位置(μm): X={pos[0]:F2}, Y={pos[1]:F2}, Z={pos[2]:F2}");
            Console.WriteLine($"  编码器: X={posEnc[1]}, Y={posEnc[0]}, Z={posEnc[2]}");
        }

        /// <summary>
        /// 测试1：验证同轴命令覆盖
        /// </summary>
        public async Task TestSameAxisOverwrite()
        {
            Console.WriteLine("\n=== 测试1：验证同轴命令覆盖 ===");

            uint hController = GetControllerHandle();

            // 测试1.1：单轴多命令
            Console.WriteLine("\n1.1 单轴3个命令测试：");
            await controller.StartOriginAsync();
            PrintPosition("初始位置");

            Hpmcstd.McsdStartBuffer(hController, 3);

            // 对X轴发送3个命令：+1000, +2000, +3000
            Console.WriteLine("缓冲3个X轴命令：");
            Hpmcstd.McsdDataWrite(hController, Microsupport.MC104_AXIS2,
                (ushort)Hpmcstd.MCSD_PLUS_INDEX_PULSE_DRIVE, 1000);
            Console.WriteLine("  命令1: X+1000脉冲（50μm）");

            Hpmcstd.McsdDataWrite(hController, Microsupport.MC104_AXIS2,
                (ushort)Hpmcstd.MCSD_PLUS_INDEX_PULSE_DRIVE, 2000);
            Console.WriteLine("  命令2: X+2000脉冲（100μm）");

            Hpmcstd.McsdDataWrite(hController, Microsupport.MC104_AXIS2,
                (ushort)Hpmcstd.MCSD_PLUS_INDEX_PULSE_DRIVE, 3000);
            Console.WriteLine("  命令3: X+3000脉冲（150μm）");

            Hpmcstd.McsdEndBuffer(hController);
            await controller.Wait();

            PrintPosition("执行后");
            Console.WriteLine("分析：如果只执行最后一个命令，X应该是150μm");
            Console.WriteLine("      如果执行所有命令，X应该是300μm（50+100+150）");

            // 测试1.2：验证最后一个命令有效
            Console.WriteLine("\n1.2 同轴正负命令测试：");
            await controller.StartOriginAsync();

            Hpmcstd.McsdStartBuffer(hController, 4);

            Console.WriteLine("缓冲4个X轴命令：");
            Hpmcstd.McsdDataWrite(hController, Microsupport.MC104_AXIS2,
                (ushort)Hpmcstd.MCSD_PLUS_INDEX_PULSE_DRIVE, 5000);
            Console.WriteLine("  命令1: X+5000脉冲（250μm）");

            Hpmcstd.McsdDataWrite(hController, Microsupport.MC104_AXIS2,
                (ushort)Hpmcstd.MCSD_MINUS_INDEX_PULSE_DRIVE, 2000);
            Console.WriteLine("  命令2: X-2000脉冲（-100μm）");

            Hpmcstd.McsdDataWrite(hController, Microsupport.MC104_AXIS2,
                (ushort)Hpmcstd.MCSD_PLUS_INDEX_PULSE_DRIVE, 3000);
            Console.WriteLine("  命令3: X+3000脉冲（150μm）");

            Hpmcstd.McsdDataWrite(hController, Microsupport.MC104_AXIS2,
                (ushort)Hpmcstd.MCSD_MINUS_INDEX_PULSE_DRIVE, 1000);
            Console.WriteLine("  命令4: X-1000脉冲（-50μm）");

            Hpmcstd.McsdEndBuffer(hController);
            await controller.Wait();

            PrintPosition("执行后");
            Console.WriteLine("分析：如果只执行最后一个命令，X应该是-50μm");
            Console.WriteLine("      如果执行所有命令，X应该是250μm（250-100+150-50）");
        }

        /// <summary>
        /// 测试2：验证不同轴可以并行
        /// </summary>
        public async Task TestDifferentAxisParallel()
        {
            Console.WriteLine("\n=== 测试2：验证不同轴并行执行 ===");

            uint hController = GetControllerHandle();
            await controller.StartOriginAsync();

            Console.WriteLine("\n2.1 每个轴一个命令：");
            PrintPosition("初始位置");

            Hpmcstd.McsdStartBuffer(hController, 3);

            Hpmcstd.McsdDataWrite(hController, Microsupport.MC104_AXIS2,
                (ushort)Hpmcstd.MCSD_PLUS_INDEX_PULSE_DRIVE, 10000);
            Console.WriteLine("命令1: X+10000脉冲（500μm）");

            Hpmcstd.McsdDataWrite(hController, Microsupport.MC104_AXIS1,
                (ushort)Hpmcstd.MCSD_PLUS_INDEX_PULSE_DRIVE, 6000);
            Console.WriteLine("命令2: Y+6000脉冲（300μm）");

            Hpmcstd.McsdDataWrite(hController, Microsupport.MC104_AXIS3,
                (ushort)Hpmcstd.MCSD_PLUS_INDEX_PULSE_DRIVE, 4000);
            Console.WriteLine("命令3: Z+4000脉冲（200μm）");

            var startTime = DateTime.Now;
            Hpmcstd.McsdEndBuffer(hController);

            // 监控并行执行
            while (controller.IsBusy())
            {
                await Task.Delay(50);
                var elapsed = (DateTime.Now - startTime).TotalMilliseconds;
                PrintPosition($"T+{elapsed:F0}ms");
            }

            PrintPosition("最终位置");
            Console.WriteLine("预期：X=500μm, Y=300μm, Z=200μm（所有轴都应该移动）");
        }

        /// <summary>
        /// 测试3：优化的轨迹执行方案
        /// </summary>
        public async Task TestOptimizedTrajectory()
        {
            Console.WriteLine("\n=== 测试3：优化的轨迹执行方案 ===");

            uint hController = GetControllerHandle();
            await controller.StartOriginAsync();

            Console.WriteLine("\n3.1 方案A - 每个Buffer批次每个轴只发一个命令：");

            // 创建一个正方形轨迹
            var squarePath = new List<(int x, int y, string desc)>
            {
                (10000, 0, "右移500μm"),
                (10000, 0, "右移500μm"),
                (10000, 0, "右移500μm"),
                (10000, 0, "右移500μm"),
                (0, 10000, "上移500μm"),
                (0, 10000, "上移500μm"),
                (0, 10000, "上移500μm"),
                (0, 10000, "上移500μm"),
                (0, 10000, "上移500μm"),
                (-10000, 0, "左移500μm"),
                (-10000, 0, "左移500μm"),
                (-10000, 0, "左移500μm"),
                (-10000, 0, "左移500μm"),
                (0, -10000, "下移500μm"),
                (0, -10000, "下移500μm"),
                (0, -10000, "下移500μm"),
                (0, -10000, "下移500μm")
            };

            PrintPosition("起始位置");

            foreach (var (x, y, desc) in squarePath)
            {
                Console.WriteLine($"\n执行: {desc}");

                // 每次只缓冲需要移动的轴
                int commandCount = (x != 0 ? 1 : 0) + (y != 0 ? 1 : 0);
                Hpmcstd.McsdStartBuffer(hController, (ushort)commandCount);

                if (x != 0)
                {
                    ushort cmd = x > 0 ? (ushort)Hpmcstd.MCSD_PLUS_INDEX_PULSE_DRIVE
                                      : (ushort)Hpmcstd.MCSD_MINUS_INDEX_PULSE_DRIVE;
                    Hpmcstd.McsdDataWrite(hController, Microsupport.MC104_AXIS2, cmd, (uint)Math.Abs(x));
                }

                if (y != 0)
                {
                    ushort cmd = y > 0 ? (ushort)Hpmcstd.MCSD_PLUS_INDEX_PULSE_DRIVE
                                      : (ushort)Hpmcstd.MCSD_MINUS_INDEX_PULSE_DRIVE;
                    Hpmcstd.McsdDataWrite(hController, Microsupport.MC104_AXIS1, cmd, (uint)Math.Abs(y));
                }

                Hpmcstd.McsdEndBuffer(hController);
                await controller.Wait();
                PrintPosition("完成");
            }
        }

        private uint GetControllerHandle()
        {
            var field = controller.GetType().GetField("hController",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (field != null)
            {
                return (uint)field.GetValue(controller);
            }
            throw new Exception("无法获取控制器句柄");
        }

        public async Task RunAllTests()
        {
            Console.WriteLine("=== Buffer同轴命令覆盖验证测试 ===\n");

            await TestSameAxisOverwrite();
            await Task.Delay(1000);

            await TestDifferentAxisParallel();
            await Task.Delay(1000);

            await TestOptimizedTrajectory();

            Console.WriteLine("\n=== 结论 ===");
            Console.WriteLine("1. Buffer中同一个轴的多个命令会被覆盖，只执行最后一个");
            Console.WriteLine("2. 不同轴的命令可以并行执行");
            Console.WriteLine("3. 实现连续轨迹的方案：");
            Console.WriteLine("   - 每个Buffer批次中，每个轴只发送一个命令");
            Console.WriteLine("   - 使用多个Buffer批次来实现复杂轨迹");
            Console.WriteLine("   - 可以通过提前发送下一批命令实现Look-Ahead效果");

            await controller.StartOriginAsync();
        }
    }

    class Program
    {
        static async Task Main(string[] args)
        {
            try
            {
                var test = new BufferVerificationTest();
                await test.RunAllTests();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"测试错误: {ex.Message}");
            }

            Console.WriteLine("\n按任意键退出...");
            Console.ReadKey();
        }
    }
}