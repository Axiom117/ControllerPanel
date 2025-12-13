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
using System.IO;

namespace MC104.server
{
    /// <summary>
    /// Controller Server implementation following API v1.2 specification
    /// </summary>
    public class ControllerServer
    {
        #region Class Properties and variables
        private TcpListener server;
        private bool isRunning;
        private readonly object lockObject = new object();
        private CancellationTokenSource cancellationTokenSource;

        /// Store parsed trajectory data per controller
        private Dictionary<string, List<TrajectoryPoint>> trajectoryData = new Dictionary<string, List<TrajectoryPoint>>();
        private Dictionary<string, bool> isPathReady = new Dictionary<string, bool>();

        /// Mock mode for testing without real controllers
        private bool useMockControllers = false;
        private Dictionary<string, MockController> mockControllers = new Dictionary<string, MockController>();

        /// Configuration
        private readonly int localServerPort = 5000;
        private const double BASE_SPEED_UM = 2500;  // 5 mm/s
        private const double OVERRIDE_DISTANCE_THRESHOLD = 250.0; // um


        /// Active client connections
        private List<TcpClient> activeClients = new List<TcpClient>();

        /// Events
        public delegate void ClientConnectionHandler(string message);
        public event ClientConnectionHandler OnClientConnection;

        public delegate void TrajectoryReceivedHandler(string controllerId, int pointCount);
        public event TrajectoryReceivedHandler OnTrajectoryReceived;

        /// TrajectoryPoint structure
        public struct TrajectoryPoint
        {
            public double X, Y, Z;
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
                // In mock mode, we can pre-define controllers that the server will manage.
                var mockIds = new[] { "MC1", "MC2" };
                foreach (var id in mockIds)
                {
                    mockControllers[id] = new MockController(id);
                    isPathReady[id] = false;
                    trajectoryData[id] = new List<TrajectoryPoint>();
                }
                NotifyClientConnection($"Using MOCK controllers for testing: {string.Join(", ", mockIds)}");
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
                    catch (System.IO.IOException)
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
                        if (parts.Length < 2)
                            return "ERROR, 101, Invalid parameters for GET_STATUS\n";
                        return await SendStatus(parts[1]);

                    case "START_STEP":
                        if (parts.Length < 5)
                            return "ERROR, 101, Invalid parameters for START_STEP\n";

                        string id_step = parts[1];
                        double x_step = double.Parse(parts[2]);
                        double y_step = double.Parse(parts[3]);
                        double z_step = double.Parse(parts[4]);

                        return await StepAbsFromCenter(id_step, x_step, y_step, z_step);

                    case "PATH_DATA":
                        if (parts.Length < 2)
                            return "ERROR, 101, Invalid parameters for PATH_DATA\n";
                        return ProcessPathData(parts[1], request);

                    case "START_PATH":
                        if (parts.Length < 2)
                            return "ERROR, 101, Invalid parameters for START_PATH\n";

                        string id_path = parts[1];
                        /// Send immediate acknowledgment
                        string ackResponse = $"PATH_TRACKING_STARTED, {id_path}\n";
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
                                string result = await PathTracking(id_path);

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
                    case "START_PATH_CP":
                        if (parts.Length < 2)
                            return "ERROR, 101, Invalid parameters for START_PATH_CP\n";

                        string id_path_cp = parts[1];
                        /// Send immediate acknowledgment
                        string ackResponseCP = $"PATH_TRACKING_CP_STARTED, {id_path_cp}\n";
                        byte[] ackDataCP = Encoding.UTF8.GetBytes(ackResponseCP);
                        await stream.WriteAsync(ackDataCP, 0, ackDataCP.Length);
                        await stream.FlushAsync();
                        NotifyClientConnection($"Controller Sent: {ackResponseCP.Trim()}");

                        /// Start path tracking asynchronously
                        _ = Task.Run(async () =>
                        {
                            try
                            {
                                /// Execute path tracking
                                string result = await PathTrackingCP(id_path_cp);

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

        /// Send status of a manipulator with a given ID
        private async Task<string> SendStatus(string id)
        {
            try
            {
                double x = 0, y = 0, z = 0;

                if (useMockControllers)
                {
                    if (mockControllers.ContainsKey(id))
                    {
                        var controller = mockControllers[id];
                        x = controller.X;
                        y = controller.Y;
                        z = controller.Z;

                        NotifyClientConnection($"[MOCK] Status for {id}: X={x:F2}, Y={y:F2}, Z={z:F2}");
                    }
                    else
                    {
                        return $"ERROR, 102, Manipulator ID {id} not found\n";
                    }
                }
                else
                {
                    if (Microsupport.controllers.ContainsKey(id))
                    {
                        var controller = Microsupport.controllers[id];
                        var pos = await Task.Run(() => controller.GetPositionsFromCenter());
                        x = pos[0];
                        y = pos[1];
                        z = pos[2];
                    }
                    else
                    {
                        return $"ERROR, 102, Manipulator ID {id} not found\n";
                    }
                }

                return $"STATUS, {id}, {x:F2}, {y:F2}, {z:F2}\n";
            }
            catch (Exception ex)
            {
                return $"ERROR, 104, Motion execution failure: {ex.Message}\n";
            }
        }

        /// <summary>
        /// StepAbsFromCenter - Perform absolute move for a single controller
        /// </summary>
        private async Task<string> StepAbsFromCenter(string id, double x, double y, double z)
        {
            try
            {
                /// Validate bounds (example: XY: ±20000um, Z: ±30000um)
                if (Math.Abs(x) > 20000 || Math.Abs(y) > 20000 || Math.Abs(z) > 30000)
                {
                    return "ERROR, 101, Movement out of bounds\n";
                }

                if (useMockControllers)
                {
                    if (mockControllers.ContainsKey(id))
                    {
                        var controller = mockControllers[id];
                        controller.X = x;
                        controller.Y = y;
                        controller.Z = z;

                        NotifyClientConnection($"[MOCK] Absolute move {id}: X={x:F2}, Y={y:F2}, Z={z:F2}.");

                        /// Simulate movement time
                        await Task.Delay(10);
                    }
                    else
                    {
                        return $"ERROR, 102, Manipulator ID {id} not found\n";
                    }
                }
                else
                {
                    if (Microsupport.controllers.ContainsKey(id))
                    {
                        var controller = Microsupport.controllers[id];
                        await controller.StartAbsAllFromCenterAsync(x, y, z);
                    }
                    else
                    {
                        return $"ERROR, 102, Manipulator ID {id} not found\n";
                    }
                }

                /// After successful move, send completion status
                return $"STEP_COMPLETED, {id}\n";
            }
            catch (Exception ex)
            {
                return $"ERROR, 104, Motion execution failure: {ex.Message}\n";
            }
        }

        /// <summary>
        /// ProcessPathData - Parse and store trajectory data for a specific controller
        /// </summary>
        private string ProcessPathData(string controllerId, string rawData)
        {
            try
            {
                // Command is "PATH_DATA", controllerId is at index 1, payload starts after that.
                int firstComma = rawData.IndexOf(',');
                int secondComma = rawData.IndexOf(',', firstComma + 1);
                if (secondComma == -1)
                    return "ERROR, 103, Invalid PATH_DATA format\n";

                string payload = rawData.Substring(secondComma + 1).Trim();

                ParseAndStoreTrajectory(controllerId, payload);

                return $"PATH_DATA_RECEIVED, {controllerId}\n";
            }
            catch (Exception ex)
            {
                lock (lockObject)
                {
                    isPathReady[controllerId] = false;
                }
                return $"ERROR, 103, Trajectory parse failure for {controllerId}: {ex.Message}\n";
            }
        }

        /// <summary>
        /// Loads trajectory data from a local CSV file and stores it for execution.
        /// This is a helper for server-side path loading and assumes a single controller format.
        /// </summary>
        public void LoadPathDataFromFile(string controllerId, string filePath)
        {
            try
            {
                NotifyClientConnection($"Loading path data for {controllerId} from: {Path.GetFileName(filePath)}");

                // Read all lines, skip the header, and join the rest into a single comma-separated string.
                string payload = string.Join(",", File.ReadAllLines(filePath).Skip(1));

                ParseAndStoreTrajectory(controllerId, payload);
            }
            catch (Exception ex)
            {
                lock (lockObject)
                {
                    isPathReady[controllerId] = false;
                }
                NotifyClientConnection($"ERROR: Failed to load path for {controllerId} from file. {ex.Message}");
            }
        }

        /// <summary>
        /// Parses a string of comma-separated values into trajectory points and stores them for a specific controller.
        /// </summary>
        private void ParseAndStoreTrajectory(string controllerId, string payload)
        {
            string[] values = payload.Split(new[] { ',', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries)
                                     .Select(v => v.Trim())
                                     .ToArray();

            /// Validate at least trajectory data are present
            if (values.Length < 3)
                throw new FormatException("PATH_DATA must include at least one trajectory point (3 values).");

            /// Validate: must be groups of 3 floats
            if (values.Length % 3 != 0)
                throw new FormatException("Trajectory data must contain groups of 3 values.");

            /// Parse trajectory points, use lockObject for thread safety
            lock (lockObject)
            {
                if (!trajectoryData.ContainsKey(controllerId))
                {
                    trajectoryData[controllerId] = new List<TrajectoryPoint>();
                }
                trajectoryData[controllerId].Clear();

                int numPoints = values.Length / 3;

                for (int i = 0; i < numPoints; i++)
                {
                    int idx = i * 3;
                    TrajectoryPoint point = new TrajectoryPoint
                    {
                        X = double.Parse(values[idx]),
                        Y = double.Parse(values[idx + 1]),
                        Z = double.Parse(values[idx + 2]),
                        Index = i
                    };
                    trajectoryData[controllerId].Add(point);
                }

                isPathReady[controllerId] = true;
                NotifyClientConnection($"Parsed {numPoints} trajectory points for {controllerId}. Path is ready.");
            }

            /// Trigger event
            OnTrajectoryReceived?.Invoke(controllerId, trajectoryData[controllerId].Count);
        }

        /// <summary>
        /// PathTracking - Execute stored trajectory for a single controller
        /// </summary>
        public async Task<string> PathTracking(string id)
        {
            if (!isPathReady.ContainsKey(id) || !isPathReady[id])
                return $"ERROR, 104, No path data ready for execution for {id}\n";

            try
            {
                NotifyClientConnection($"Starting path tracking for {id}...");

                if (!useMockControllers)
                {
                    var controller = Microsupport.controllers[id];
                    controller.SetSpeedAll(5000);
                }

                /// Copy trajectory data locally to avoid locking during execution
                List<TrajectoryPoint> pointsToExecute;
                lock (lockObject)
                {
                    pointsToExecute = new List<TrajectoryPoint>(trajectoryData[id]);
                }

                foreach (var point in pointsToExecute)
                {
                    /// Move manipulator to absolute positions from center
                    string result = await StepAbsFromCenter(id, point.X, point.Y, point.Z);
                    if (result.StartsWith("ERROR"))
                    {
                        isPathReady[id] = false;
                        return result;
                    }

                    /// Progress notification
                    if (point.Index % 10 == 0)
                        NotifyClientConnection($"Path progress for {id}: {point.Index + 1}/{pointsToExecute.Count}");
                }

                isPathReady[id] = false; // Path consumed
                NotifyClientConnection($"Path tracking completed for {id}");

                return $"PATH_COMPLETED, {id}\n";
            }
            catch (Exception ex)
            {
                isPathReady[id] = false;
                return $"ERROR, 104, Motion execution failure for {id}: {ex.Message}\n";
            }
        }

        /// <summary>
        /// PathTrackingCP - Execute stored trajectory using Continuous Path (Index Override)
        /// </summary>
        public async Task<string> PathTrackingCP(string id)
        {
            if (!isPathReady.ContainsKey(id) || !isPathReady[id])
                return $"ERROR, 104, No path data ready for execution for {id}\n";

            List<TrajectoryPoint> pointsToExecute;
            lock (lockObject)
            {
                pointsToExecute = new List<TrajectoryPoint>(trajectoryData[id]);
            }

            if (pointsToExecute.Count < 2)
                return $"ERROR, 104, Not enough points for CP trajectory for {id}.\n";

            try
            {
                NotifyClientConnection($"Starting CP path tracking for {id}...");

                if (useMockControllers)
                {
                    NotifyClientConnection("[MOCK] CP mode is not supported, falling back to PTP.");
                    return await PathTracking(id);
                }

                var controller = Microsupport.controllers[id];

                /// 1. Start first segment to kick off motion
                var firstPoint = pointsToExecute[0];
                NotifyClientConnection($"[CP] {id}: Launching initial move to Point 0.");
                controller.SetSpeedAll(BASE_SPEED_UM);
                controller.StartAbsAllFromCenter(firstPoint.X, firstPoint.Y, firstPoint.Z);

                /// 2. Loop through each segment in the trajectory
                for (int i = 0; i < pointsToExecute.Count - 1; i++)
                {
                    var currentTarget = pointsToExecute[i];
                    var nextTarget = pointsToExecute[i + 1];

                    NotifyClientConnection($"[CP] {id}: Segment {i}->{i + 1} running. Waiting for override window.");

                    bool overrideTriggered = false;
                    while (!overrideTriggered && controller.IsBusy())
                    {
                        double[] pos = controller.GetPositionsFromCenter();
                        double dist = Math.Sqrt(Math.Pow(currentTarget.X - pos[0], 2) + Math.Pow(currentTarget.Y - pos[1], 2) + Math.Pow(currentTarget.Z - pos[2], 2));

                        if (dist < OVERRIDE_DISTANCE_THRESHOLD)
                        {
                            NotifyClientConnection($"[CP] {id}: In override window. Overriding to Point {i + 1}.");
                            ApplyIndexOverride(controller, currentTarget.X, currentTarget.Y, currentTarget.Z, nextTarget.X, nextTarget.Y, nextTarget.Z);
                            overrideTriggered = true;
                        }

                        await Task.Delay(1);
                    }

                    NotifyClientConnection($"[CP] {id}: Segment {i}->{i + 1} override complete.");

                    if (!controller.IsBusy() && i < pointsToExecute.Count - 2)
                    {
                        NotifyClientConnection($"[CP] {id}: Motion stopped unexpectedly after segment {i}.");
                        return $"ERROR, 104, Motion stopped unexpectedly during CP path for {id}.\n";
                    }
                }

                await controller.Wait();
                isPathReady[id] = false; // Path consumed
                NotifyClientConnection($"Path tracking completed for {id}");

                return $"PATH_COMPLETED, {id}\n";
            }
            catch (Exception ex)
            {
                isPathReady[id] = false;
                return $"ERROR, 104, Motion execution failure for {id}: {ex.Message}\n";
            }
        }

        private void ApplyIndexOverride(Microsupport controller, double currentX, double currentY, double currentZ, double nextX, double nextY, double nextZ)
        {
            const double MIN_SPEED_UM = 100.0;
            const double MOTION_TOLERANCE = 1.0;

            /// Speed override logic (using absolute coordinates)
            double nextDx = nextX - currentX;
            double nextDy = nextY - currentY;
            double nextDz = nextZ - currentZ;
            double maxDisplacement = Math.Max(Math.Abs(nextDx), Math.Max(Math.Abs(nextDy), Math.Abs(nextDz)));

            /// If displacement is within tolerance, no need to override
            if (maxDisplacement <= MOTION_TOLERANCE)
            {
                return;
            }

            if (maxDisplacement > 1e-6)
            {
                double speedX = Math.Abs(nextDx) > MOTION_TOLERANCE ? BASE_SPEED_UM * (Math.Abs(nextDx) / maxDisplacement) : MIN_SPEED_UM;
                double speedY = Math.Abs(nextDy) > MOTION_TOLERANCE ? BASE_SPEED_UM * (Math.Abs(nextDy) / maxDisplacement) : MIN_SPEED_UM;
                double speedZ = Math.Abs(nextDz) > MOTION_TOLERANCE ? BASE_SPEED_UM * (Math.Abs(nextDz) / maxDisplacement) : MIN_SPEED_UM;

                if (Math.Abs(nextDx) > MOTION_TOLERANCE) controller.SpeedOverride(Microsupport.AXIS.X, (uint)Math.Max(speedX, MIN_SPEED_UM));
                if (Math.Abs(nextDy) > MOTION_TOLERANCE) controller.SpeedOverride(Microsupport.AXIS.Y, (uint)Math.Max(speedY, MIN_SPEED_UM));
                if (Math.Abs(nextDz) > MOTION_TOLERANCE) controller.SpeedOverride(Microsupport.AXIS.Z, (uint)Math.Max(speedZ, MIN_SPEED_UM));

                NotifyClientConnection($"[CP] Speed override applied: X={Math.Max(speedX, MIN_SPEED_UM):F2} um/s, Y={Math.Max(speedY, MIN_SPEED_UM):F2} um/s, Z={Math.Max(speedZ, MIN_SPEED_UM):F2} um/s.");
            }

            /// Index override logic (using absolute coordinates)
            if (Math.Abs(nextDx) > 1e-6)
            {
                if (controller.IsBusy(Microsupport.AXIS.X))
                {
                    controller.IndexOverride(Microsupport.AXIS.X, (uint)controller.Um2enc(Microsupport.AXIS.X, nextX));
                    NotifyClientConnection($"[CP] X Axis busy, applying index override to {nextX:F2} um absolute.");
                }
                else
                {
                    controller.StartAbsFromCenter(Microsupport.AXIS.X, nextX);
                    NotifyClientConnection($"[CP] X Axis not busy, starting absolute move to {nextX:F2} um from center.");
                }
            }
            else
            {
                controller.StopAxis(Microsupport.AXIS.X);
            }

            if (Math.Abs(nextDy) > 1e-6)
            {
                if (controller.IsBusy(Microsupport.AXIS.Y))
                {
                    controller.IndexOverride(Microsupport.AXIS.Y, (uint)controller.Um2enc(Microsupport.AXIS.Y, nextY));
                    NotifyClientConnection($"[CP] Y Axis busy, applying index override to {nextY:F2} um absolute.");
                }
                else
                {
                    controller.StartAbsFromCenter(Microsupport.AXIS.Y, nextY);
                    NotifyClientConnection($"[CP] Y Axis not busy, starting absolute move to {nextY:F2} um from center.");
                }
            }
            else
            {
                controller.StopAxis(Microsupport.AXIS.Y);
            }

            if (Math.Abs(nextDz) > 1e-6)
            {
                if (controller.IsBusy(Microsupport.AXIS.Z))
                {
                    controller.IndexOverride(Microsupport.AXIS.Z, (uint)controller.Um2enc(Microsupport.AXIS.Z, nextZ));
                    NotifyClientConnection($"[CP] Z Axis busy, applying index override to {nextZ:F2} um absolute.");
                }
                else
                {
                    controller.StartAbsFromCenter(Microsupport.AXIS.Z, nextZ);
                    NotifyClientConnection($"[CP] Z Axis not busy, starting absolute move to {nextZ:F2} um from center.");
                }
            }
            else
            {
                controller.StopAxis(Microsupport.AXIS.Z);
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
                        control.Invoke(new Action(() => handler.DynamicInvoke(timestampedMessage)));
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