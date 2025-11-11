using MicrosupportController;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Linq;
using System.Numerics;

namespace MC104.server
{
    /// <summary>
    /// Controller Server implementation following API v1.0 specification
    /// </summary>
    public class ControllerServer_v1
    {
        #region Class Properties and variables
        private TcpListener server;
        private bool isRunning;
        private readonly object lockObject = new object();
        private CancellationTokenSource cancellationTokenSource;

        /// Store parsed trajectory data
        private List<TrajectoryPoint> trajectoryData = new List<TrajectoryPoint>();
        private bool isPathReady = false;

        /// Mock mode for testing without real controllers
        private bool useMockControllers = false;
        private Dictionary<string, MockController> mockControllers = new Dictionary<string, MockController>();

        /// Configuration
        private readonly int localServerPort = 5000;

        /// Active client connections
        private List<TcpClient> activeClients = new List<TcpClient>();

        /// Configuration for PathTracking_v1
        private const int FINAL_POINTS_COUNT = 3; // Use absolute positioning for last N points
        private const double POSITION_TOLERANCE = 5.0; // Position error tolerance in micrometers
        private const int CORRECTION_INTERVAL = 50; // Apply position correction every N points

        /// Events
        public delegate void ClientConnectionHandler(string message);
        public event ClientConnectionHandler OnClientConnection;

        public delegate void TrajectoryReceivedHandler(int pointCount);
        public event TrajectoryReceivedHandler OnTrajectoryReceived;

        /// TrajectoryPoint structure
        public struct TrajectoryPoint
        {
            public double X1, Y1, Z1;
            public double X2, Y2, Z2;
            public int Index;
        }

        /// Mock controller for testing
        private class MockController
        {
            public double X { get; set; }
            public double Y { get; set; }
            public double Z { get; set; }
            public string Id { get; set; }

            public MockController(string id)
            {
                Id = id;
                X = Y = Z = 0;
            }
        }

        #endregion

        #region Server Start/Stop Methods
        public ControllerServer_v1(int port = 5000, bool mockMode = false)
        {
            localServerPort = port;
            server = new TcpListener(IPAddress.Any, localServerPort);
            useMockControllers = mockMode;

            if (useMockControllers)
            {
                mockControllers["MC1"] = new MockController("MC1");
                mockControllers["MC2"] = new MockController("MC2");
                NotifyClientConnection("Using MOCK controllers for testing");
            }
        }

        public void Start()
        {
            server.Start();
            isRunning = true;
            cancellationTokenSource = new CancellationTokenSource();
            NotifyClientConnection($"Controller Server started on port {localServerPort}");

            /// Accept clients on background thread
            Task.Run(() => AcceptClients(cancellationTokenSource.Token));
        }

        public void Stop()
        {
            isRunning = false;
            cancellationTokenSource?.Cancel();

            // Close all active client connections
            lock (activeClients)
            {
                foreach (var client in activeClients)
                {
                    try
                    {
                        client.Close();
                    }
                    catch { }
                }
                activeClients.Clear();
            }

            server.Stop();
            NotifyClientConnection("Controller Server stopped.");
        }
        #endregion

        #region Connection handling
        private async Task AcceptClients(CancellationToken cancellationToken)
        {
            while (isRunning && !cancellationToken.IsCancellationRequested)
            {
                try
                {
                    // Use AcceptTcpClientAsync with cancellation
                    var tcpClientTask = server.AcceptTcpClientAsync();
                    var tcs = new TaskCompletionSource<bool>();

                    using (cancellationToken.Register(() => tcs.TrySetCanceled()))
                    {
                        var completedTask = await Task.WhenAny(tcpClientTask, tcs.Task);

                        if (completedTask == tcpClientTask && tcpClientTask.Result != null)
                        {
                            TcpClient client = tcpClientTask.Result;

                            // Add to active clients list
                            lock (activeClients)
                            {
                                activeClients.Add(client);
                            }

                            NotifyClientConnection($"PathPlanner client connected from {client.Client.RemoteEndPoint}");

                            // Handle each client in separate task
                            _ = Task.Run(async () => await HandleClient(client, cancellationToken));
                        }
                    }
                }
                catch (ObjectDisposedException)
                {
                    // Server was stopped
                    break;
                }
                catch (OperationCanceledException)
                {
                    // Cancellation requested
                    break;
                }
                catch (Exception ex)
                {
                    if (isRunning)
                        NotifyClientConnection($"Error accepting client: {ex.Message}");
                }
            }
        }

        private async Task HandleClient(TcpClient client, CancellationToken cancellationToken)
        {
            NetworkStream stream = null;
            byte[] buffer = new byte[65536]; // 64KB buffer

            try
            {
                stream = client.GetStream();
                stream.ReadTimeout = 1000; // 1 second read timeout

                while (client.Connected && isRunning && !cancellationToken.IsCancellationRequested)
                {
                    try
                    {
                        // Check if client is still connected
                        if (!IsSocketConnected(client.Client))
                        {
                            break;
                        }

                        // Read data with timeout
                        var readTask = stream.ReadAsync(buffer, 0, buffer.Length, cancellationToken);
                        var bytesRead = await readTask;

                        if (bytesRead == 0)
                        {
                            // Client disconnected gracefully
                            break;
                        }

                        // Decode request
                        string request = Encoding.UTF8.GetString(buffer, 0, bytesRead).Trim();
                        NotifyClientConnection($"Controller Received: {request.Substring(0, Math.Min(100, request.Length))}...");

                        // Process request and get response
                        string response = await ProcessPathPlannerRequest(request, stream);

                        // Send response only if not empty (START_PATH already sent its acknowledgment)
                        if (!string.IsNullOrEmpty(response))
                        {
                            byte[] responseData = Encoding.UTF8.GetBytes(response);
                            await stream.WriteAsync(responseData, 0, responseData.Length, cancellationToken);
                            await stream.FlushAsync();
                            NotifyClientConnection($"Controller Sent: {response.Trim()}");
                        }
                    }
                    catch (System.IO.IOException ioEx)
                    {
                        // Read timeout or connection issue
                        if (!IsSocketConnected(client.Client))
                        {
                            break;
                        }
                    }
                }
            }
            catch (OperationCanceledException)
            {
                // Server is shutting down
            }
            catch (Exception ex)
            {
                NotifyClientConnection($"Client error: {ex.Message}");
            }
            finally
            {
                // Remove from active clients and close connection
                lock (activeClients)
                {
                    activeClients.Remove(client);
                }

                try
                {
                    stream?.Dispose();
                    client.Close();
                }
                catch { }

                NotifyClientConnection("PathPlanner client disconnected");
            }
        }

        /// <summary>
        /// Check if socket is still connected
        /// </summary>
        private bool IsSocketConnected(Socket socket)
        {
            try
            {
                return !(socket.Poll(1, SelectMode.SelectRead) && socket.Available == 0);
            }
            catch (SocketException)
            {
                return false;
            }
        }

        private async Task<string> ProcessPathPlannerRequest(string request, NetworkStream stream)
        {
            try
            {
                string[] parts = request.Split(',').Select(p => p.Trim()).ToArray();
                string command = parts[0];

                switch (command)
                {
                    case "HEARTBEAT":
                        return "HEARTBEAT_OK\n";

                    case "GET_STATUS":
                        if (parts.Length < 3)
                            return "ERROR, 101, Invalid parameters for GET_STATUS\n";
                        return await SendStatus(parts[1], parts[2]);

                    case "START_STEP":
                        if (parts.Length < 6)
                            return "ERROR, 101, Invalid parameters for START_STEP\n";

                        string id1 = parts[1];
                        string id2 = parts[2];
                        double x = double.Parse(parts[3]);
                        double y = double.Parse(parts[4]);
                        double z = double.Parse(parts[5]);

                        return await StepAbsFromCenter(id1, id2, x, y, z, x, y, z);

                    case "PATH_DATA":
                        return ProcessPathData(request);

                    case "START_PATH":
                        if (parts.Length < 3)
                            return "ERROR, 101, Invalid parameters for START_PATH\n";

                        // Send immediate acknowledgment
                        string ackResponse = "PATH_TRACKING_STARTED\n";
                        byte[] ackData = Encoding.UTF8.GetBytes(ackResponse);
                        await stream.WriteAsync(ackData, 0, ackData.Length);
                        await stream.FlushAsync();
                        NotifyClientConnection($"Controller Sent: {ackResponse.Trim()}");

                        // Start path tracking asynchronously
                        _ = Task.Run(async () =>
                        {
                            try
                            {
                                // Execute path tracking using the new smooth method
                                string result = await PathTracking_v1(parts[1], parts[2]);

                                // Check if stream is still valid before sending
                                if (stream.CanWrite)
                                {
                                    byte[] resultData = Encoding.UTF8.GetBytes(result);
                                    await stream.WriteAsync(resultData, 0, resultData.Length);
                                    await stream.FlushAsync();
                                    NotifyClientConnection($"Controller Sent: {result.Trim()}");
                                }
                            }
                            catch (Exception ex)
                            {
                                if (stream.CanWrite)
                                {
                                    string errorResult = $"ERROR, 104, Path tracking exception: {ex.Message}\n";
                                    byte[] errorData = Encoding.UTF8.GetBytes(errorResult);
                                    await stream.WriteAsync(errorData, 0, errorData.Length);
                                    await stream.FlushAsync();
                                    NotifyClientConnection($"Controller Sent: {errorResult.Trim()}");
                                }
                            }
                        });

                        // Return empty string since we already sent the acknowledgment
                        return string.Empty;

                    case "START_PATH_LEGACY":
                        if (parts.Length < 3)
                            return "ERROR, 101, Invalid parameters for START_PATH_LEGACY\n";

                        // Send immediate acknowledgment
                        string legacyAckResponse = "PATH_TRACKING_STARTED\n";
                        byte[] legacyAckData = Encoding.UTF8.GetBytes(legacyAckResponse);
                        await stream.WriteAsync(legacyAckData, 0, legacyAckData.Length);
                        await stream.FlushAsync();
                        NotifyClientConnection($"Controller Sent: {legacyAckResponse.Trim()}");

                        // Start legacy path tracking asynchronously
                        _ = Task.Run(async () =>
                        {
                            try
                            {
                                // Execute legacy path tracking
                                string result = await PathTracking(parts[1], parts[2]);

                                // Check if stream is still valid before sending
                                if (stream.CanWrite)
                                {
                                    byte[] resultData = Encoding.UTF8.GetBytes(result);
                                    await stream.WriteAsync(resultData, 0, resultData.Length);
                                    await stream.FlushAsync();
                                    NotifyClientConnection($"Controller Sent: {result.Trim()}");
                                }
                            }
                            catch (Exception ex)
                            {
                                if (stream.CanWrite)
                                {
                                    string errorResult = $"ERROR, 104, Path tracking exception: {ex.Message}\n";
                                    byte[] errorData = Encoding.UTF8.GetBytes(errorResult);
                                    await stream.WriteAsync(errorData, 0, errorData.Length);
                                    await stream.FlushAsync();
                                    NotifyClientConnection($"Controller Sent: {errorResult.Trim()}");
                                }
                            }
                        });

                        return string.Empty;

                    default:
                        return $"ERROR, 100, Unknown request: {command}\n";
                }
            }
            catch (Exception ex)
            {
                return $"ERROR, 101, Exception processing request: {ex.Message}\n";
            }
        }
        #endregion

        #region API Methods Implementation

        /// Send status of two manipulators with given IDs
        private async Task<string> SendStatus(string id1, string id2)
        {
            try
            {
                double x1 = 0, y1 = 0, z1 = 0, x2 = 0, y2 = 0, z2 = 0;

                if (useMockControllers)
                {
                    if (mockControllers.ContainsKey(id1) && mockControllers.ContainsKey(id2))
                    {
                        var controller = mockControllers[id1];
                        x1 = controller.X;
                        y1 = controller.Y;
                        z1 = controller.Z;

                        controller = mockControllers[id2];
                        x2 = controller.X;
                        y2 = controller.Y;
                        z2 = controller.Z;

                        NotifyClientConnection($"[MOCK] Status for {id1}: X={x1:F2}, Y={y1:F2}, Z={z1:F2}, {id2}: X={x2:F2}, Y={y2:F2}, Z={z2:F2}");
                    }
                    else
                    {
                        return $"ERROR, 102, Manipulator ID {id1} or {id2} out of range\n";
                    }
                }
                else
                {
                    if (Microsupport.controllers.ContainsKey(id1) && Microsupport.controllers.ContainsKey(id2))
                    {
                        var controller = Microsupport.controllers[id1];
                        var pos = await Task.Run(() => controller.GetPositionsFromCenter());
                        x1 = pos[0];
                        y1 = pos[1];
                        z1 = pos[2];
                        controller = Microsupport.controllers[id2];
                        pos = await Task.Run(() => controller.GetPositionsFromCenter());
                        x2 = pos[0];
                        y2 = pos[1];
                        z2 = pos[2];
                    }
                    else
                    {
                        return $"ERROR, 102, Manipulator ID {id1} out of range\n";
                    }
                }

                return $"STATUS, {id1}, {x1:F2}, {y1:F2}, {z1:F2}, {id2}, {x2:F2}, {y2:F2}, {z2:F2}\n";
            }
            catch (Exception ex)
            {
                return $"ERROR, 104, Motion execution failure: {ex.Message}\n";
            }
        }

        /// <summary>
        /// StepInc - Perform incremental move
        /// </summary>
        private async Task<string> StepAbsFromCenter(string id1, string id2, double x1, double y1, double z1, double x2, double y2, double z2)
        {
            try
            {
                // Validate bounds (example: XY: ±20000um, Z: ±30000um)
                if (Math.Abs(x1) > 20000 || Math.Abs(y1) > 20000 || Math.Abs(z1) > 30000)
                {
                    return "ERROR, 101, Movement out of bounds\n";
                }

                if (useMockControllers)
                {
                    if (mockControllers.ContainsKey(id1) && mockControllers.ContainsKey(id2))
                    {
                        var controller = mockControllers[id1];
                        controller.X = x1;
                        controller.Y = y1;
                        controller.Z = z1;

                        controller = mockControllers[id2];
                        controller.X = x2;
                        controller.Y = y2;
                        controller.Z = z2;

                        NotifyClientConnection($"[MOCK] Incremental move {id1}: ΔX1={x1:F2}, ΔY1={y1:F2}, ΔZ1={z1:F2}.\n {id2}: ΔX1={x2:F2}, ΔY1={y2:F2}, ΔZ1={z2:F2}");

                        // Simulate movement time
                        await Task.Delay(10);
                    }
                    else
                    {
                        return $"ERROR, 102, Manipulator ID {id1} or {id2} out of range\n";
                    }
                }
                else
                {
                    if (Microsupport.controllers.ContainsKey(id1) && Microsupport.controllers.ContainsKey(id2))
                    {
                        var controller1 = Microsupport.controllers[id1];
                        var controller2 = Microsupport.controllers[id2];

                        // Use Task.WhenAll to run both movements concurrently
                        await Task.WhenAll(
                            controller1.StartAbsFromCenter(x1, y1, z1),
                            controller2.StartAbsFromCenter(x2, y2, z2)
                        );
                    }
                    else
                    {
                        return $"ERROR, 102, Manipulator ID {id1} or {id2} out of range\n";
                    }
                }

                // After successful move, send updated status
                return $"STEP_COMPLETED, {id1}, {id2}\n";
            }
            catch (Exception ex)
            {
                return $"ERROR, 104, Motion execution failure: {ex.Message}\n";
            }
        }

        /// <summary>
        /// ProcessPathData - Parse and store trajectory data
        /// </summary>
        private string ProcessPathData(string rawData)
        {
            try
            {
                // Remove "PATH_DATA," prefix
                int firstComma = rawData.IndexOf(',');
                if (firstComma == -1)
                    return "ERROR, 103, Invalid PATH_DATA format\n";

                string payload = rawData.Substring(firstComma + 1).Trim();
                string[] values = payload.Split(',').Select(v => v.Trim()).ToArray();

                // Validate at least two IDs and trajectory data are present
                if (values.Length < 2)
                    return "ERROR, 103, PATH_DATA must include two IDs and trajectory data\n";

                string id1 = values[0];
                string id2 = values[1];

                // Validate: must be groups of 6 floats
                if ((values.Length - 2) % 6 != 0)
                    return "ERROR, 103, Trajectory data must contain groups of 6 values\n";

                lock (lockObject)
                {
                    trajectoryData.Clear();
                    int numPoints = (values.Length - 2) / 6;

                    for (int i = 0; i < numPoints; i++)
                    {
                        int idx = 2 + i * 6;
                        TrajectoryPoint point = new TrajectoryPoint
                        {
                            X1 = double.Parse(values[idx]),
                            Y1 = double.Parse(values[idx + 1]),
                            Z1 = double.Parse(values[idx + 2]),
                            X2 = double.Parse(values[idx + 3]),
                            Y2 = double.Parse(values[idx + 4]),
                            Z2 = double.Parse(values[idx + 5]),
                            Index = i
                        };
                        trajectoryData.Add(point);
                    }

                    isPathReady = true;
                    NotifyClientConnection($"Parsed {numPoints} trajectory points");
                }

                // Trigger event
                OnTrajectoryReceived?.Invoke(trajectoryData.Count);

                // Send both responses
                return "PATH_DATA_RECEIVED\n";
            }
            catch (Exception ex)
            {
                isPathReady = false;
                return $"ERROR, 103, Trajectory parse failure: {ex.Message}\n";
            }
        }

        /// <summary>
        /// PathTracking_v1 - Smooth trajectory execution using StepInc method
        /// Features:
        /// 1. Uses relative movements (StepInc) for smooth motion without start/stop cycles
        /// 2. Real-time position monitoring and correction
        /// 3. Absolute positioning for final points to ensure accuracy
        /// </summary>
        private async Task<string> PathTracking_v1(string id1, string id2)
        {
            if (!isPathReady)
                return "ERROR, 104, No path data ready for execution\n";

            try
            {
                NotifyClientConnection("Starting smooth path tracking (v1)...");

                if (useMockControllers)
                {
                    return await PathTracking_v1_Mock(id1, id2);
                }

                if (!Microsupport.controllers.ContainsKey(id1) || !Microsupport.controllers.ContainsKey(id2))
                {
                    return $"ERROR, 102, Manipulator ID {id1} or {id2} not found\n";
                }

                var controller1 = Microsupport.controllers[id1];
                var controller2 = Microsupport.controllers[id2];

                // Set higher speed for smooth motion
                controller1.SetSpeedAll(5000);
                controller2.SetSpeedAll(5000);

                // Get initial positions for reference
                double[] prevPos1 = controller1.GetPositionsFromCenter();
                double[] prevPos2 = controller2.GetPositionsFromCenter();

                NotifyClientConnection($"Initial positions - {id1}: [{prevPos1[0]:F2}, {prevPos1[1]:F2}, {prevPos1[2]:F2}], {id2}: [{prevPos2[0]:F2}, {prevPos2[1]:F2}, {prevPos2[2]:F2}]");

                int totalPoints = trajectoryData.Count;
                int finalPointsStart = Math.Max(0, totalPoints - FINAL_POINTS_COUNT);

                for (int i = 0; i < totalPoints; i++)
                {
                    var point = trajectoryData[i];

                    // Use absolute positioning for final points to ensure accuracy
                    if (i >= finalPointsStart)
                    {
                        NotifyClientConnection($"Using absolute positioning for final point {i + 1}/{totalPoints}");

                        await Task.WhenAll(
                            controller1.StartAbsFromCenter(point.X1, point.Y1, point.Z1),
                            controller2.StartAbsFromCenter(point.X2, point.Y2, point.Z2)
                        );

                        // Update previous positions
                        prevPos1 = new double[] { point.X1, point.Y1, point.Z1 };
                        prevPos2 = new double[] { point.X2, point.Y2, point.Z2 };
                    }
                    else
                    {
                        // Calculate relative movements from previous positions
                        double deltaX1 = point.X1 - prevPos1[0];
                        double deltaY1 = point.Y1 - prevPos1[1];
                        double deltaZ1 = point.Z1 - prevPos1[2];

                        double deltaX2 = point.X2 - prevPos2[0];
                        double deltaY2 = point.Y2 - prevPos2[1];
                        double deltaZ2 = point.Z2 - prevPos2[2];

                        var startTime = DateTime.Now;

                        // Execute relative movements concurrently
                        await Task.WhenAll(
                            controller1.StartIncBufferAsync(deltaX1, deltaY1, deltaZ1),
                            controller2.StartIncBufferAsync(deltaX2, deltaY2, deltaZ2)
                        );

                        var executionTime = (DateTime.Now - startTime).TotalMilliseconds;

                        // Update expected positions
                        prevPos1[0] = point.X1;
                        prevPos1[1] = point.Y1;
                        prevPos1[2] = point.Z1;

                        prevPos2[0] = point.X2;
                        prevPos2[1] = point.Y2;
                        prevPos2[2] = point.Z2;

                        // Periodic position correction
                        if (i % CORRECTION_INTERVAL == 0 && i > 0)
                        {
                            var correctionResult = await ApplyPositionCorrection(controller1, controller2, id1, id2,
                                                                                point.X1, point.Y1, point.Z1,
                                                                                point.X2, point.Y2, point.Z2,
                                                                                prevPos1, prevPos2);
                            prevPos1 = correctionResult.newPrevPos1;
                            prevPos2 = correctionResult.newPrevPos2;
                        }

                        // Progress notification
                        if (i % 10 == 0 || i >= finalPointsStart)
                        {
                            NotifyClientConnection($"Smooth path progress: {i + 1}/{totalPoints} (Δ: [{deltaX1:F1},{deltaY1:F1},{deltaZ1:F1}] [{deltaX2:F1},{deltaY2:F1},{deltaZ2:F1}] - Execution Time: {executionTime:F1}ms)");
                        }
                    }
                }

                // Final position verification
                double[] finalPos1 = controller1.GetPositionsFromCenter();
                double[] finalPos2 = controller2.GetPositionsFromCenter();

                var lastPoint = trajectoryData[totalPoints - 1];
                double error1 = Math.Sqrt(Math.Pow(finalPos1[0] - lastPoint.X1, 2) +
                                        Math.Pow(finalPos1[1] - lastPoint.Y1, 2) +
                                        Math.Pow(finalPos1[2] - lastPoint.Z1, 2));
                double error2 = Math.Sqrt(Math.Pow(finalPos2[0] - lastPoint.X2, 2) +
                                        Math.Pow(finalPos2[1] - lastPoint.Y2, 2) +
                                        Math.Pow(finalPos2[2] - lastPoint.Z2, 2));

                NotifyClientConnection($"Final position errors: {id1}={error1:F2}μm, {id2}={error2:F2}μm");

                isPathReady = false; // Path consumed
                NotifyClientConnection("Smooth path tracking completed successfully");

                return $"PATH_COMPLETED, {id1}, {id2}\n";
            }
            catch (Exception ex)
            {
                isPathReady = false;
                NotifyClientConnection($"Smooth path tracking failed: {ex.Message}");
                return $"ERROR, 104, Motion execution failure: {ex.Message}\n";
            }
        }

        /// <summary>
        /// Apply position correction during trajectory execution
        /// </summary>
        private async Task<(double[] newPrevPos1, double[] newPrevPos2)> ApplyPositionCorrection(
            Microsupport controller1, Microsupport controller2,
            string id1, string id2,
            double targetX1, double targetY1, double targetZ1,
            double targetX2, double targetY2, double targetZ2,
            double[] prevPos1, double[] prevPos2)
        {
            try
            {
                // Get actual positions
                double[] actualPos1 = controller1.GetPositionsFromCenter();
                double[] actualPos2 = controller2.GetPositionsFromCenter();

                // Calculate position errors
                double errorX1 = targetX1 - actualPos1[0];
                double errorY1 = targetY1 - actualPos1[1];
                double errorZ1 = targetZ1 - actualPos1[2];

                double errorX2 = targetX2 - actualPos2[0];
                double errorY2 = targetY2 - actualPos2[1];
                double errorZ2 = targetZ2 - actualPos2[2];

                double totalError1 = Math.Sqrt(errorX1 * errorX1 + errorY1 * errorY1 + errorZ1 * errorZ1);
                double totalError2 = Math.Sqrt(errorX2 * errorX2 + errorY2 * errorY2 + errorZ2 * errorZ2);

                // Apply correction if error exceeds tolerance
                if (totalError1 > POSITION_TOLERANCE || totalError2 > POSITION_TOLERANCE)
                {
                    NotifyClientConnection($"Applying position correction: {id1} error={totalError1:F2}μm, {id2} error={totalError2:F2}μm");

                    await Task.WhenAll(
                        controller1.StartIncBufferAsync(errorX1, errorY1, errorZ1),
                        controller2.StartIncBufferAsync(errorX2, errorY2, errorZ2)
                    );

                    // Return actual corrected positions
                    return (controller1.GetPositionsFromCenter(), controller2.GetPositionsFromCenter());
                }

                // No correction needed, return current positions
                return (actualPos1, actualPos2);
            }
            catch (Exception ex)
            {
                NotifyClientConnection($"Position correction failed: {ex.Message}");
                // Return original positions if correction fails
                return (prevPos1, prevPos2);
            }
        }

        /// <summary>
        /// Mock version of PathTracking_v1 for testing
        /// </summary>
        private async Task<string> PathTracking_v1_Mock(string id1, string id2)
        {
            if (!mockControllers.ContainsKey(id1) || !mockControllers.ContainsKey(id2))
            {
                return $"ERROR, 102, Mock manipulator ID {id1} or {id2} not found\n";
            }

            var controller1 = mockControllers[id1];
            var controller2 = mockControllers[id2];

            double prevX1 = controller1.X, prevY1 = controller1.Y, prevZ1 = controller1.Z;
            double prevX2 = controller2.X, prevY2 = controller2.Y, prevZ2 = controller2.Z;

            int totalPoints = trajectoryData.Count;
            int finalPointsStart = Math.Max(0, totalPoints - FINAL_POINTS_COUNT);

            for (int i = 0; i < totalPoints; i++)
            {
                var point = trajectoryData[i];

                if (i >= finalPointsStart)
                {
                    // Absolute positioning for final points
                    controller1.X = point.X1;
                    controller1.Y = point.Y1;
                    controller1.Z = point.Z1;

                    controller2.X = point.X2;
                    controller2.Y = point.Y2;
                    controller2.Z = point.Z2;
                }
                else
                {
                    // Relative positioning with small simulation errors
                    Random rand = new Random();
                    double errorFactor = 0.05; // 5% positioning error simulation

                    double deltaX1 = point.X1 - prevX1;
                    double deltaY1 = point.Y1 - prevY1;
                    double deltaZ1 = point.Z1 - prevZ1;

                    controller1.X += deltaX1 * (1 + (rand.NextDouble() - 0.5) * errorFactor);
                    controller1.Y += deltaY1 * (1 + (rand.NextDouble() - 0.5) * errorFactor);
                    controller1.Z += deltaZ1 * (1 + (rand.NextDouble() - 0.5) * errorFactor);

                    double deltaX2 = point.X2 - prevX2;
                    double deltaY2 = point.Y2 - prevY2;
                    double deltaZ2 = point.Z2 - prevZ2;

                    controller2.X += deltaX2 * (1 + (rand.NextDouble() - 0.5) * errorFactor);
                    controller2.Y += deltaY2 * (1 + (rand.NextDouble() - 0.5) * errorFactor);
                    controller2.Z += deltaZ2 * (1 + (rand.NextDouble() - 0.5) * errorFactor);
                }

                prevX1 = point.X1; prevY1 = point.Y1; prevZ1 = point.Z1;
                prevX2 = point.X2; prevY2 = point.Y2; prevZ2 = point.Z2;

                // Simulate movement time (much faster than real hardware)
                await Task.Delay(1);

                if (i % 50 == 0)
                {
                    NotifyClientConnection($"[MOCK] Smooth path progress: {i + 1}/{totalPoints}");
                }
            }

            NotifyClientConnection("[MOCK] Smooth path tracking completed");
            return $"PATH_COMPLETED, {id1}, {id2}\n";
        }

        /// <summary>
        /// PathTracking - Legacy trajectory execution using absolute positioning
        /// </summary>
        private async Task<string> PathTracking(string id1, string id2)
        {
            if (!isPathReady)
                return "ERROR, 104, No path data ready for execution\n";

            try
            {
                NotifyClientConnection("Starting legacy path tracking...");

                var controller1 = Microsupport.controllers[id1];
                var controller2 = Microsupport.controllers[id2];

                controller1.SetSpeedAll(10000);
                controller2.SetSpeedAll(10000);

                foreach (var point in trajectoryData)
                {
                    // Move two manipulators incrementally
                    string result1 = await StepAbsFromCenter(id1, id2, point.X1, point.Y1, point.Z1, point.X2, point.Y2, point.Z2);
                    if (result1.StartsWith("ERROR"))
                    {
                        isPathReady = false;
                        return result1;
                    }

                    // Progress notification
                    if (point.Index % 10 == 0)
                        NotifyClientConnection($"Legacy path progress: {point.Index + 1}/{trajectoryData.Count}");
                }

                isPathReady = false; // Path consumed
                NotifyClientConnection("Legacy path tracking completed");

                return $"PATH_COMPLETED, {id1}, {id2}\n";
            }
            catch (Exception ex)
            {
                isPathReady = false;
                return $"ERROR, 104, Motion execution failure: {ex.Message}\n";
            }
        }

        #endregion

        #region Helper Methods
        private void NotifyClientConnection(string message)
        {
            string timestampedMessage = $"[{DateTime.Now:HH:mm:ss}] {message}";

            if (OnClientConnection != null)
            {
                foreach (var handler in OnClientConnection.GetInvocationList())
                {
                    if (handler.Target is Control control && control.InvokeRequired)
                    {
                        control.Invoke(handler, timestampedMessage);
                    }
                    else
                    {
                        handler.DynamicInvoke(timestampedMessage);
                    }
                }
            }
        }
        #endregion
    }
}