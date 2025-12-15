using MicrosupportController;
using System;
using SmoothTrajectoryTest;
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
using System.Diagnostics;

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
        private const double BASE_SPEED_UM = 2500;  // 2.5 mm/s
        private const double OVERRIDE_DISTANCE_THRESHOLD = 300.0; // um


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
                        string[] ids = parts.Skip(1).ToArray();
                        return await SendStatus(ids);

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
        private async Task<string> SendStatus(params string[] ids)
        {
            try
            {
                var statusParts = new List<string> { "STATUS" };

                foreach (var id in ids)
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
                    statusParts.Add(id);
                    statusParts.Add($"{x:F2}");
                    statusParts.Add($"{y:F2}");
                    statusParts.Add($"{z:F2}");
                }

                return string.Join(", ", statusParts) + "\n";
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
        /// Executes a stored trajectory using Continuous Path (CP) motion with index override.
        /// </summary>
        public async Task<string> PathTrackingCP(string id)
        {
            if (!isPathReady.ContainsKey(id) || !isPathReady[id])
                return $"ERROR, 104, No path data ready for execution for {id}\n";

            // In mock mode, fall back to simple PTP tracking as CP logic is hardware-dependent.
            if (useMockControllers)
            {
                NotifyClientConnection($"[MOCK] CP mode not supported, falling back to standard PTP for {id}.");
                return await PathTracking(id);
            }

            try
            {
                NotifyClientConnection($"[CP] Starting High-Precision CP Motion for {id}...");

                var controller = Microsupport.controllers[id];
                List<TrajectoryPoint> trajectoryPoints;
                lock (lockObject)
                {
                    trajectoryPoints = new List<TrajectoryPoint>(trajectoryData[id]);
                }

                if (trajectoryPoints.Count < 2)
                {
                    return $"ERROR, 104, Not enough points for CP trajectory for {id}.\n";
                }

                // 1. Configure speed and ensure axes are homed and ready.
                controller.SetSpeedAll(BASE_SPEED_UM);

                // 'chainStartPoint' acts as the reference origin for the current continuous move chain.
                TrajectoryPoint chainStartPoint = trajectoryPoints[0];

                // 2. Start the initial segment (P0 -> P1).
                NotifyClientConnection($"[CP] Starting initial segment (Point 0 -> Point 1) for {id}...");
                await StartSegmentSync(controller, trajectoryPoints[0], trajectoryPoints[1]);

                // 3. Iterate through subsequent segments.
                for (int i = 1; i < trajectoryPoints.Count - 1; i++)
                {
                    var currentTarget = trajectoryPoints[i];
                    var nextTarget = trajectoryPoints[i + 1];
                    var prevTarget = trajectoryPoints[i - 1];

                    bool isContinuous = CheckContinuity(prevTarget, currentTarget, nextTarget);

                    if (isContinuous)
                    {
                        bool overrideSuccess = await WaitForOverrideWindowAndExecute_HighFreq(controller, currentTarget, nextTarget, chainStartPoint);
                        if (!overrideSuccess)
                        {
                            NotifyClientConnection($"[CP] WARNING: Missed override window at Point {i} for {id}. Resyncing...");
                            await controller.Wait();
                            chainStartPoint = currentTarget;
                            await StartSegmentSync(controller, currentTarget, nextTarget);
                        }
                    }
                    else
                    {
                        NotifyClientConnection($"[CP] Segment {i}->{i + 1}: Discontinuous. Stopping at Point {i} for {id}.");
                        await controller.Wait();
                        chainStartPoint = currentTarget;
                        await StartSegmentSync(controller, currentTarget, nextTarget);
                    }
                }

                // Wait for the final segment to complete.
                await controller.Wait();

                double[] finalPos = controller.GetPositionsFromCenter();
                NotifyClientConnection($"[CP] Motion Complete for {id}. Final Pos: ({finalPos[0]:F1}, {finalPos[1]:F1}, {finalPos[2]:F1}).");

                isPathReady[id] = false; // Path consumed
                return $"PATH_COMPLETED, {id}\n";
            }
            catch (Exception ex)
            {
                isPathReady[id] = false;
                return $"ERROR, 104, CP motion execution failure for {id}: {ex.Message}\n";
            }
        }

        /// <summary>
        /// Executes a stored trajectory using Continuous Path (CP) motion for multiple controllers in parallel.
        /// </summary>
        public async Task PathTrackingCP_Parallel(List<string> ids)
        {
            var trackingTasks = new List<Task<string>>();

            foreach (var id in ids)
            {
                // For each controller, create a new task to run its path tracking logic.
                // This ensures each controller's logic runs on a separate thread from the thread pool,
                // allowing true parallel execution.
                trackingTasks.Add(Task.Run(() => PathTrackingCP(id)));
            }

            // Wait for all path tracking tasks to complete.
            var results = await Task.WhenAll(trackingTasks);

            // Log the results for each controller.
            for (int i = 0; i < ids.Count; i++)
            {
                NotifyClientConnection($"Parallel execution result for {ids[i]}: {results[i].Trim()}");
            }
        }

        #endregion

        #region CP Motion Helper Methods

        /// <summary>
        /// Checks if the motion vector allows for a continuous override.
        /// </summary>
        private bool CheckContinuity(TrajectoryPoint p1, TrajectoryPoint p2, TrajectoryPoint p3)
        {
            double dx1 = p2.X - p1.X;
            double dy1 = p2.Y - p1.Y;
            double dz1 = p2.Z - p1.Z;

            double dx2 = p3.X - p2.X;
            double dy2 = p3.Y - p2.Y;
            double dz2 = p3.Z - p2.Z;

            if (Math.Sign(dx1) != Math.Sign(dx2)) return false;
            if (Math.Sign(dy1) != Math.Sign(dy2)) return false;
            if (Math.Sign(dz1) != Math.Sign(dz2)) return false;

            return true;
        }

       /// <summary>
        /// Calculates and applies speed overrides for each axis based on displacement, but only if the speed differences are significant.
        /// </summary>
        static void AdjustSpeeds(Microsupport controller, double dx, double dy, double dz)
        {
            const double MIN_SPEED_UM = 100.0;
            const double SPEED_CHANGE_THRESHOLD = 0.10; // 10%

            double maxDisplacement = Math.Max(Math.Abs(dx), Math.Max(Math.Abs(dy), Math.Abs(dz)));
            if (maxDisplacement < 1e-6) return; // No movement

            // Calculate new target speeds
            double targetSpeedX = BASE_SPEED_UM * (Math.Abs(dx) / maxDisplacement);
            double targetSpeedY = BASE_SPEED_UM * (Math.Abs(dy) / maxDisplacement);
            double targetSpeedZ = BASE_SPEED_UM * (Math.Abs(dz) / maxDisplacement);

            // Get current speeds from the controller
            double currentSpeedX = controller.GetSpeed(Microsupport.AXIS.X);
            double currentSpeedY = controller.GetSpeed(Microsupport.AXIS.Y);
            double currentSpeedZ = controller.GetSpeed(Microsupport.AXIS.Z);

            // Check each axis individually
            bool needsUpdateX = Math.Abs(targetSpeedX - currentSpeedX) > currentSpeedX * SPEED_CHANGE_THRESHOLD;
            bool needsUpdateY = Math.Abs(targetSpeedY - currentSpeedY) > currentSpeedY * SPEED_CHANGE_THRESHOLD;
            bool needsUpdateZ = Math.Abs(targetSpeedZ - currentSpeedZ) > currentSpeedZ * SPEED_CHANGE_THRESHOLD;

            if (needsUpdateX || needsUpdateY || needsUpdateZ)
            {
                Console.WriteLine($"[CP] Speeds adjusted for next segment: X={targetSpeedX:F0}, Y={targetSpeedY:F0}, Z={targetSpeedZ:F0} um/s");
                if (Math.Abs(dx) > 1e-6) controller.SpeedOverride(Microsupport.AXIS.X, (uint)Math.Max(targetSpeedX, MIN_SPEED_UM));
                if (Math.Abs(dy) > 1e-6) controller.SpeedOverride(Microsupport.AXIS.Y, (uint)Math.Max(targetSpeedY, MIN_SPEED_UM));
                if (Math.Abs(dz) > 1e-6) controller.SpeedOverride(Microsupport.AXIS.Z, (uint)Math.Max(targetSpeedZ, MIN_SPEED_UM));
            }
            else
            {
                Console.WriteLine($"[CP] Speed differences are within {SPEED_CHANGE_THRESHOLD:P0}, skipping override.");
            }
        }

        /// <summary>
        /// Starts a synchronized movement for all axes.
        /// </summary>
        private async Task StartSegmentSync(Microsupport controller, TrajectoryPoint start, TrajectoryPoint end)
        {
            double dx = end.X - start.X;
            double dy = end.Y - start.Y;
            double dz = end.Z - start.Z;

            AdjustSpeeds(controller, dx, dy, dz);

            /// Determine directions based on displacement signs
            var xDir = dx >= 0 ? Microsupport.DIRECTION.FORWARD : Microsupport.DIRECTION.REVERSE;
            var yDir = dy >= 0 ? Microsupport.DIRECTION.FORWARD : Microsupport.DIRECTION.REVERSE;
            var zDir = dz >= 0 ? Microsupport.DIRECTION.REVERSE : Microsupport.DIRECTION.FORWARD; // Note: Z axis direction is inverted considering the physical setup of Microsupport

            controller.StartIncAll(xDir, Math.Abs(dx),
                                   yDir, Math.Abs(dy),
                                   zDir, Math.Abs(dz));

            await Task.Delay(1);
        }

        /// <summary>
        /// High-frequency polling loop to trigger IndexOverride.
        /// </summary>
        private async Task<bool> WaitForOverrideWindowAndExecute_HighFreq(Microsupport controller, TrajectoryPoint currentTarget, TrajectoryPoint nextTarget, TrajectoryPoint chainStart)
        {
            Stopwatch safetyTimer = Stopwatch.StartNew();
            long timeoutLimit = 2000; // 2 seconds safety limit

            double totalDistX = Math.Abs(nextTarget.X - chainStart.X);
            double totalDistY = Math.Abs(nextTarget.Y - chainStart.Y);
            double totalDistZ = Math.Abs(nextTarget.Z - chainStart.Z);

            uint pulseX = (uint)controller.Um2enc(Microsupport.AXIS.X, totalDistX);
            uint pulseY = (uint)controller.Um2enc(Microsupport.AXIS.Y, totalDistY);
            uint pulseZ = (uint)controller.Um2enc(Microsupport.AXIS.Z, totalDistZ);

            double thresholdSq = OVERRIDE_DISTANCE_THRESHOLD * OVERRIDE_DISTANCE_THRESHOLD;

            while (controller.IsBusy())
            {
                if (safetyTimer.ElapsedMilliseconds > timeoutLimit) return false;

                double[] currentPos = controller.GetPositionsFromCenter();
                double dx = currentTarget.X - currentPos[0];
                double dy = currentTarget.Y - currentPos[1];
                double dz = currentTarget.Z - currentPos[2];
                double distSq = dx * dx + dy * dy + dz * dz;

                if (distSq <= thresholdSq)
                {
                    NotifyClientConnection($"[CP] Override Window Reached. Executing Override to ({nextTarget.X}, {nextTarget.Y}, {nextTarget.Z})");

                    double next_dx = nextTarget.X - currentTarget.X;
                    double next_dy = nextTarget.Y - currentTarget.Y;
                    double next_dz = nextTarget.Z - currentTarget.Z;

                    AdjustSpeeds(controller, next_dx, next_dy, next_dz);

                    if (Math.Abs(next_dx) > 0.001) controller.IndexOverride(Microsupport.AXIS.X, pulseX);
                    if (Math.Abs(next_dy) > 0.001) controller.IndexOverride(Microsupport.AXIS.Y, pulseY);
                    if (Math.Abs(next_dz) > 0.001) controller.IndexOverride(Microsupport.AXIS.Z, pulseZ);

                    return true;
                }

                NotifyClientConnection($"[CP] Waiting for Override Window. Current Position: ({currentPos[0]:F1}, {currentPos[1]:F1}, {currentPos[2]:F1})");

                Thread.SpinWait(1);
            }

            return false; // Controller stopped before threshold was reached
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