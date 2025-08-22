using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MC104.server
{
    /// <summary>
    /// Mock MATLAB PathPlanner Server for testing
    /// </summary>
    public class MockMatlabServer
    {
        private TcpClient controllerClient;
        private NetworkStream controllerStream;
        private bool isRunning;
        private bool isConnected;

        // Store current pose
        private double X0 = 0, Y0 = 0, Z0 = 0;
        private double Phi0 = 0, Theta0 = 0, Psi0 = 0;

        public delegate void LogMessageHandler(string message);
        public event LogMessageHandler OnLogMessage;

        private readonly string controllerHost = "127.0.0.1";
        private readonly int controllerPort = 5000;

        public MockMatlabServer()
        {
        }

        public void Start()
        {
            isRunning = true;
            OnLogMessage?.Invoke("Mock MATLAB PathPlanner starting...");

            // Connect to Controller Server
            Task.Run(() => ConnectToController());
        }

        public void Stop()
        {
            isRunning = false;
            isConnected = false;

            controllerStream?.Close();
            controllerClient?.Close();

            OnLogMessage?.Invoke("Mock MATLAB PathPlanner stopped");
        }

        private async Task ConnectToController()
        {
            while (isRunning && !isConnected)
            {
                try
                {
                    OnLogMessage?.Invoke($"Connecting to Controller Server at {controllerHost}:{controllerPort}...");

                    controllerClient = new TcpClient();
                    await controllerClient.ConnectAsync(controllerHost, controllerPort);
                    controllerStream = controllerClient.GetStream();
                    isConnected = true;

                    OnLogMessage?.Invoke("Connected to Controller Server!");

                    // Start receiving responses
                    Task.Run(() => ReceiveFromController());

                    // Send initial heartbeat
                    SendCommand("HEARTBEAT");
                }
                catch (Exception ex)
                {
                    OnLogMessage?.Invoke($"Connection failed: {ex.Message}. Retrying in 3s...");
                    await Task.Delay(3000);
                }
            }
        }

        private void ReceiveFromController()
        {
            byte[] buffer = new byte[4096];

            while (isConnected && controllerClient.Connected)
            {
                try
                {
                    int bytesRead = controllerStream.Read(buffer, 0, buffer.Length);
                    if (bytesRead > 0)
                    {
                        string response = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                        ProcessControllerResponse(response);
                    }
                }
                catch (Exception ex)
                {
                    OnLogMessage?.Invoke($"Receive error: {ex.Message}");
                    isConnected = false;

                    // Try to reconnect
                    Task.Run(() => ConnectToController());
                    break;
                }
            }
        }

        private void ProcessControllerResponse(string response)
        {
            OnLogMessage?.Invoke($"Controller Response: {response.Trim()}");

            // Handle multiple responses in one message
            string[] lines = response.Split('\n');
            foreach (string line in lines)
            {
                if (string.IsNullOrWhiteSpace(line)) continue;

                string[] parts = line.Split(',').Select(p => p.Trim()).ToArray();
                string responseType = parts[0];

                switch (responseType)
                {
                    case "HEARTBEAT_OK":
                        OnLogMessage?.Invoke("✓ Heartbeat acknowledged");
                        break;

                    case "STATUS":
                        if (parts.Length >= 9)
                        {
                            string id1 = parts[1];
                            double x1 = double.Parse(parts[2]);
                            double y1 = double.Parse(parts[3]);
                            double z1 = double.Parse(parts[4]);
                            string id2 = parts[5];
                            double x2 = double.Parse(parts[6]);
                            double y2 = double.Parse(parts[7]);
                            double z2 = double.Parse(parts[8]);
                            OnLogMessage?.Invoke($"✓ Status {id1}: X={x1:F2}, Y={y1:F2}, Z={z1:F2}, {id2}: X={x2:F2}, Y={y2:F2}, Z={z2:F2}");
                        }
                        break;

                    case "STEP_COMPLETED":
                        OnLogMessage?.Invoke("✓ Step move completed");
                        SendCommand($"GET_STATUS, {parts[1]}, {parts[2]}");
                        break;

                    case "PATH_DATA_RECEIVED":
                        OnLogMessage?.Invoke("✓ Path data received by controller and ready for execution!");
                        // In real MATLAB, this would enable ExecutePath button
                        break;

                    case "PATH_COMPLETED":
                        OnLogMessage?.Invoke("✓ Path execution completed!");
                        SendCommand($"GET_STATUS, {parts[1]}, {parts[2]}");
                        break;

                    case "ERROR":
                        if (parts.Length >= 3)
                        {
                            OnLogMessage?.Invoke($"❌ ERROR {parts[1]}: {string.Join(",", parts.Skip(2))}");
                        }
                        break;
                }
            }
        }

        private void SendCommand(string command)
        {
            if (!isConnected || controllerStream == null)
            {
                OnLogMessage?.Invoke("Not connected to controller");
                return;
            }

            try
            {
                byte[] data = Encoding.UTF8.GetBytes(command + "\n");
                controllerStream.Write(data, 0, data.Length);
                controllerStream.Flush();
                OnLogMessage?.Invoke($"Sent: {command}");
            }
            catch (Exception ex)
            {
                OnLogMessage?.Invoke($"Send error: {ex.Message}");
                isConnected = false;
            }
        }

        #region Public Test Methods

        /// <summary>
        /// GetStatus - Request status from specific manipulator
        /// </summary>
        public void GetStatus(string id1, string id2)
        {
            SendCommand($"GET_STATUS, {id1}, {id2}");
        }

        /// <summary>
        /// StepMove - Send incremental move command
        /// </summary>
        public void StepMove(double x, double y, double z, string id1, string id2)
        {
            SendCommand($"START_STEP, {id1}, {id2}, {x:F2}, {y:F2}, {z:F2}");
        }

        /// <summary>
        /// PlanPath - Simulate path planning and send trajectory
        /// </summary>
        public void PlanPath(string id1, string id2, double targetX, double targetY, double targetZ,
                            double targetPhi, double targetTheta, double targetPsi)
        {
            OnLogMessage?.Invoke($"Planning path to [{targetX:F2}, {targetY:F2}, {targetZ:F2}]...");

            // Step 1: Get current status
            GetStatus(id1, id2);

            // Step 2: Simulate IK calculation (simplified circular path)
            Task.Delay(500).ContinueWith(_ =>
            {
                // Generate simple trajectory (10 points)
                StringBuilder pathData = new StringBuilder($"PATH_DATA, {id1}, {id2}, ");
                int numPoints = 10;

                for (int i = 0; i < numPoints; i++)
                {
                    double t = (double)i / (numPoints - 1);

                    // Simple interpolation for demo
                    double x1 = X0 + (targetX - X0) * t * 0.1;
                    double y1 = Y0 + (targetY - Y0) * t * 0.1;
                    double z1 = Z0 + (targetZ - Z0) * t * 0.1;

                    // MC2 moves opposite
                    double x2 = -x1;
                    double y2 = -y1;
                    double z2 = z1;

                    if (i > 0) pathData.Append(", ");
                    pathData.Append($"{x1:F2}, {y1:F2}, {z1:F2}, {x2:F2}, {y2:F2}, {z2:F2}");
                }

                SendCommand(pathData.ToString());
            });
        }

        /// <summary>
        /// ExecutePath - Start path execution
        /// </summary>
        public void ExecutePath(string id1, string id2)
        {
            SendCommand($"START_PATH, {id1}, {id2}\n");
        }

        /// <summary>
        /// Simple test sequence
        /// </summary>
        public void RunTestSequence()
        {
            Task.Run(async () =>
            {
                OnLogMessage?.Invoke("=== Starting Test Sequence ===");

                // Test 1: Heartbeat
                OnLogMessage?.Invoke("Test 1: Heartbeat");
                SendCommand("HEARTBEAT");
                await Task.Delay(1000);

                // Test 2: Get Status
                OnLogMessage?.Invoke("Test 2: Get Status");
                GetStatus("MC1", "MC2");
                await Task.Delay(1000);

                // Test 3: Simple moves
                OnLogMessage?.Invoke("Test 3: Step Moves");
                StepMove(1000, 5000, 2000, "MC1", "MC2");
                await Task.Delay(1000);

                // Test 4: Path planning
                OnLogMessage?.Invoke("Test 4: Path Planning");
                PlanPath("MC1", "MC2", 50, 30, 20, 0.1, 0, 0);
                await Task.Delay(2000);

                // Test 5: Execute path
                OnLogMessage?.Invoke("Test 5: Execute Path");
                ExecutePath("MC1", "MC2");

                OnLogMessage?.Invoke("=== Test Sequence Complete ===");
            });
        }

        #endregion
    }
}