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
    public class ControllerServer
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
        public ControllerServer(int port = 5000, bool mockMode = false)
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

        /// <summary>
        /// Starts the server and begins accepting incoming client connections.
        /// </summary>
        public void Start()
        {
            server.Start();
            isRunning = true;
            cancellationTokenSource = new CancellationTokenSource();
            NotifyClientConnection($"Controller Server started on port {localServerPort}");

            /// Accept clients on background thread
            Task.Run(() => AcceptClients(cancellationTokenSource.Token));
        }

        /// <summary>
        /// Stops the server and terminates all active client connections.
        /// </summary>
        public void Stop()
        {
            isRunning = false;
            cancellationTokenSource?.Cancel();

            /// Close all active client connections
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
        /// <summary>
        /// Asynchronously accepts incoming TCP client connections until cancellation is requested or the server is
        /// stopped.
        /// </summary>
        private async Task AcceptClients(CancellationToken cancellationToken)
        {
            while (isRunning && !cancellationToken.IsCancellationRequested)
            {
                try
                {
                    /// Use AcceptTcpClientAsync with cancellation
                    var tcpClientTask = server.AcceptTcpClientAsync();
                    var tcs = new TaskCompletionSource<bool>();

                    using (cancellationToken.Register(() => tcs.TrySetCanceled()))
                    {
                        var completedTask = await Task.WhenAny(tcpClientTask, tcs.Task);

                        if (completedTask == tcpClientTask && tcpClientTask.Result != null)
                        {
                            TcpClient client = tcpClientTask.Result;

                            /// Add to active clients list
                            lock (activeClients)
                            {
                                activeClients.Add(client);
                            }

                            NotifyClientConnection($"PathPlanner client connected from {client.Client.RemoteEndPoint}");

                            /// Handle each client in separate task
                            _ = Task.Run(async () => await HandleClient(client, cancellationToken));
                        }
                    }
                }
                catch (ObjectDisposedException)
                {
                    /// Server was stopped
                    break;
                }
                catch (OperationCanceledException)
                {
                    /// Cancellation requested
                    break;
                }
                catch (Exception ex)
                {
                    if (isRunning)
                        NotifyClientConnection($"Error accepting client: {ex.Message}");
                }
            }
        }

        /// <summary>
        /// Handles communication with a connected TCP client, processing incoming requests and sending responses
        /// asynchronously.
        /// </summary>
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
                        /// Check if client is still connected
                        if (!IsSocketConnected(client.Client))
                        {
                            break;
                        }

                        /// Read data with timeout
                        var readTask = stream.ReadAsync(buffer, 0, buffer.Length, cancellationToken);
                        var bytesRead = await readTask;

                        if (bytesRead == 0)
                        {
                            /// Client disconnected gracefully
                            break;
                        }

                        /// Decode request
                        string request = Encoding.UTF8.GetString(buffer, 0, bytesRead).Trim();
                        NotifyClientConnection($"Controller Received: {request.Substring(0, Math.Min(100, request.Length))}...");

                        /// Process request and get response
                        string response = await ProcessRequest(request, stream);

                        /// Send response only if not empty (START_PATH already sent its acknowledgment)
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
                        /// Read timeout or connection issue
                        if (!IsSocketConnected(client.Client))
                        {
                            break;
                        }
                    }
                }
            }
            catch (OperationCanceledException)
            {
                /// Server is shutting down
            }
            catch (Exception ex)
            {
                NotifyClientConnection($"Client error: {ex.Message}");
            }
            finally
            {
                /// Remove from active clients and close connection
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

        /// <summary>
        /// Processes a path planner request received from a client and returns the appropriate response message.
        /// </summary>
        private async Task<string> ProcessRequest(string request, NetworkStream stream)
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

                        /// Send immediate acknowledgment
                        string ackResponse = "PATH_TRACKING_STARTED\n";
                        byte[] ackData = Encoding.UTF8.GetBytes(ackResponse);
                        await stream.WriteAsync(ackData, 0, ackData.Length);
                        await stream.FlushAsync();
                        NotifyClientConnection($"Controller Sent: {ackResponse.Trim()}");

                        /// Start path tracking asynchronously
                        _ = Task.Run(async () =>
                        {
                            try
                            {
                                /// Execute path tracking
                                string result = await PathTracking(parts[1], parts[2]);

                                /// Check if stream is still valid before sending
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

                        /// Return empty string since we already sent the acknowledgment
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
                /// Validate bounds (example: XY: ±20000um, Z: ±30000um)
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

                        /// Simulate movement time
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

                        /// Use Task.WhenAll to run both movements concurrently
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

                /// After successful move, send updated status
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
                /// Remove "PATH_DATA," prefix
                int firstComma = rawData.IndexOf(',');
                if (firstComma == -1)
                    return "ERROR, 103, Invalid PATH_DATA format\n";

                string payload = rawData.Substring(firstComma + 1).Trim();
                string[] values = payload.Split(',').Select(v => v.Trim()).ToArray();

                /// Validate at least trajectory data are present
                if (values.Length < 6)
                    return "ERROR, 103, PATH_DATA must include at least one trajectory data\n";


                /// Validate: must be groups of 6 floats
                if (values.Length % 6 != 0)
                    return "ERROR, 103, Trajectory data must contain groups of 6 values\n";

                /// Parse trajectory points, use lockObject for thread safety
                lock (lockObject)
                {
                    /// Clear existing data
                    trajectoryData.Clear();
                    /// Calculate number of points
                    int numPoints = values.Length / 6;

                    /// Parse each trajectory point
                    for (int i = 0; i < numPoints; i++)
                    {
                        int idx = i * 6;
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

                /// Trigger event
                OnTrajectoryReceived?.Invoke(trajectoryData.Count);

                /// Send both responses
                return "PATH_DATA_RECEIVED\n";
            }
            catch (Exception ex)
            {
                isPathReady = false;
                return $"ERROR, 103, Trajectory parse failure: {ex.Message}\n";
            }
        }

        /// <summary>
        /// PathTracking - Execute stored trajectory
        /// </summary>
        private async Task<string> PathTracking(string id1, string id2)
        {
            if (!isPathReady)
                return "ERROR, 104, No path data ready for execution\n";

            try
            {
                NotifyClientConnection("Starting path tracking...");

                var controller1 = Microsupport.controllers[id1];
                var controller2 = Microsupport.controllers[id2];

                controller1.SetSpeedAll(5000);
                controller2.SetSpeedAll(5000);

                foreach (var point in trajectoryData)
                {
                    /// Move two manipulators incrementally
                    string result1 = await StepAbsFromCenter(id1, id2, point.X1, point.Y1, point.Z1, point.X2, point.Y2, point.Z2);
                    if (result1.StartsWith("ERROR"))
                    {
                        isPathReady = false;
                        return result1;
                    }

                    /// Progress notification
                    if (point.Index % 10 == 0)
                        NotifyClientConnection($"Path progress: {point.Index + 1}/{trajectoryData.Count}");
                }

                isPathReady = false; // Path consumed
                NotifyClientConnection("Path tracking completed");

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