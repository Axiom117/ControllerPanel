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

        /// Configuration
        private readonly int localServerPort = 5000;
        private const double SPEED_DEFAULT = 1000.0; // Default speed for movements
        private const double OVERRIDE_PROGRESS_PERCENT = 0.80; // 80%
        private const double DURATION = 0.2; // 200 ms

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
            public double X, Y, Z, Speed;
            public int Index;
        }

        #endregion

        #region Server Start/Stop Methods
        public ControllerServer(int port = 5000)
        {
            localServerPort = port;
            server = new TcpListener(IPAddress.Any, localServerPort);
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
                        /// Returned status format: STATUS, id1, x1, y1, z1, id2, x2, y2, z2, ...
                        if (parts.Length < 2)
                            return "ERROR, 101, Invalid parameters for GET_STATUS\n";
                        string[] ids = parts.Skip(1).ToArray();
                        return await SendStatus(ids);

                    case "START_STEP":
                        /// Parameters are groups of 5: [id, x, y, z, speed]. Total params must be 1 (command) + n * 5.
                        if (parts.Length < 6 || (parts.Length - 1) % 5 != 0)
                            return "ERROR, 101, Invalid parameters for START_STEP\n";

                        var moveTasks = new List<Task>();
                        var controllerIds = new List<string>();

                        for (int i = 1; i < parts.Length; i += 5)
                        {
                            string id = parts[i];
                            if (!Microsupport.controllers.ContainsKey(id))
                                return $"ERROR, 102, Manipulator ID {id} not found\n";

                            double x = double.Parse(parts[i + 1]);
                            double y = double.Parse(parts[i + 2]);
                            double z = double.Parse(parts[i + 3]);
                            double speed = double.Parse(parts[i + 4]);

                            if (Math.Abs(x) > 20000 || Math.Abs(y) > 20000 || Math.Abs(z) > 30000)
                                return "ERROR, 101, Movement out of bounds\n";

                            controllerIds.Add(id);
                            var controller = Microsupport.controllers[id];
                            moveTasks.Add(controller.StartAbsAllFromCenterAsync(x, y, z, speed));
                        }

                        /// Execute all moves in parallel and wait for completion
                        await Task.WhenAll(moveTasks);

                        return $"STEP_COMPLETED, {string.Join(", ", controllerIds)}\n";

                    case "PATH_DATA":
                        /// Request format: PATH_DATA, id, x1, y1, z1, x2, y2, z2, ...
                        if (parts.Length < 2)
                            return "ERROR, 101, Invalid parameters for PATH_DATA\n";
                        return ProcessPathData(parts[1], request);

                    case "START_PATH_PTP":
                        /// Request format: START_PATH_PTP, duration, id1, id2, ...
                        if (parts.Length < 3)
                            return "ERROR, 101, Invalid parameters for START_PATH_PTP\n";

                        if (!double.TryParse(parts[1], out double ptpDuration))
                            return "ERROR, 101, Invalid duration parameter for START_PATH_PTP\n";

                        var ids_path_ptp = parts.Skip(2).ToList();
                        /// Send immediate acknowledgment for all specified controllers
                        string ackResponse = $"PATH_TRACKING_PTP_STARTED, {string.Join(", ", ids_path_ptp)}\n";
                        byte[] ackData = Encoding.UTF8.GetBytes(ackResponse);
                        await stream.WriteAsync(ackData, 0, ackData.Length);
                        await stream.FlushAsync();
                        NotifyClientConnection($"Controller Sent: {ackResponse.Trim()}");

                        /// Start parallel path tracking asynchronously
                        _ = Task.Run(async () =>
                        {
                            try
                            {
                                /// Execute parallel path tracking
                                await PathTrackingPTP_Parallel(ids_path_ptp, ptpDuration, stream);
                            }
                            catch (Exception ex)
                            {
                                /// This top-level catch might be useful for aggregate errors,
                                /// though individual errors are handled within PathTrackingPTP_Parallel.
                                NotifyClientConnection($"[ERROR] An exception occurred during parallel PTP path execution: {ex.Message}");
                            }
                        });

                        /// Return empty string since we already sent the acknowledgment
                        return string.Empty;

                    case "START_PATH_CP":
                        /// Request format: START_PATH_CP, duration, id1, id2, ...
                        if (parts.Length < 3)
                            return "ERROR, 101, Invalid parameters for START_PATH_CP\n";

                        /// Retrieve duration value into cpDuration
                        if (!double.TryParse(parts[1], out double cpDuration))
                            return "ERROR, 101, Invalid duration parameter for START_PATH_CP\n";

                        var ids_path_cp = parts.Skip(2).ToList();
                        /// Send immediate acknowledgment for all specified controllers
                        string ackResponseCP = $"PATH_TRACKING_CP_STARTED, {string.Join(", ", ids_path_cp)}\n";
                        byte[] ackDataCP = Encoding.UTF8.GetBytes(ackResponseCP);
                        await stream.WriteAsync(ackDataCP, 0, ackDataCP.Length);
                        await stream.FlushAsync();
                        NotifyClientConnection($"Controller Sent: {ackResponseCP.Trim()}");

                        /// Start parallel path tracking asynchronously
                        _ = Task.Run(async () =>
                        {
                            try
                            {
                                /// Execute parallel path tracking
                                await PathTrackingCP_Parallel(ids_path_cp, cpDuration, stream);
                            }
                            catch (Exception ex)
                            {
                                /// This top-level catch might be useful for aggregate errors,
                                /// though individual errors are handled within PathTrackingCP.
                                NotifyClientConnection($"[ERROR] An exception occurred during parallel CP path execution: {ex.Message}");
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
        /// ProcessPathData - Parse and store trajectory data for a specific controller
        /// </summary>
        private string ProcessPathData(string controllerId, string rawData)
        {
            try
            {
                /// Command is "PATH_DATA", controllerId is at index 1, payload starts after that.
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
            if (values.Length < 3) // At least one point (X,Y,Z)
                throw new FormatException("PATH_DATA must include at least one trajectory point.");

            /// Data must be in groups of 3 (X,Y,Z)
            if (values.Length % 3 != 0)
                throw new FormatException("Trajectory data must contain groups of 3 values (X, Y, Z).");

            const int pointValueCount = 3;

            /// Parse trajectory points, use lockObject for thread safety
            lock (lockObject)
            {
                if (!trajectoryData.ContainsKey(controllerId))
                {
                    trajectoryData[controllerId] = new List<TrajectoryPoint>();
                }
                trajectoryData[controllerId].Clear();

                int numPoints = values.Length / pointValueCount;

                for (int i = 0; i < numPoints; i++)
                {
                    int idx = i * pointValueCount;
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
        /// PathTracking - Execute stored trajectory for a single controller using duration-based segments.
        /// </summary>
        public async Task<string> PathTrackingPTP(string id, double duration)
        {
            if (!isPathReady.ContainsKey(id) || !isPathReady[id])
                return $"ERROR, 104, No path data ready for execution for {id}\n";

            try
            {
                NotifyClientConnection($"Starting path tracking for {id}...");

                var controller = Microsupport.controllers[id];

                /// Copy trajectory data locally to avoid locking during execution
                List<TrajectoryPoint> pointsToExecute;
                lock (lockObject)
                {
                    pointsToExecute = new List<TrajectoryPoint>(trajectoryData[id]);
                }

                if (pointsToExecute.Count < 2)
                {
                    NotifyClientConnection($"Path for {id} has less than 2 points. Nothing to execute.");
                    return $"PATH_COMPLETED, {id}\n";
                }

                // Move to the start point before beginning segmented moves
                var startPoint = pointsToExecute[0];
                NotifyClientConnection($"[Controller {id}] Moving to start point ({startPoint.X:F1}, {startPoint.Y:F1}, {startPoint.Z:F1})...");
                await controller.StartAbsAllFromCenterAsync(startPoint.X, startPoint.Y, startPoint.Z, SPEED_DEFAULT);


                /// Execute trajectory segment by segment
                for (int i = 0; i < pointsToExecute.Count - 1; i++)
                {
                    var currentPoint = pointsToExecute[i];
                    var nextPoint = pointsToExecute[i + 1];

                    /// Start the segment move and wait for it to complete
                    await StartSegmentSync(controller, currentPoint, nextPoint, duration);
                    await controller.Wait();

                    /// Progress notification
                    NotifyClientConnection($"Path progress for {id}: {i + 1}/{pointsToExecute.Count - 1} segments completed.");
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
        /// Executes a stored trajectory using Point-to-Point (PTP) motion for multiple controllers in parallel.
        /// </summary>
        public async Task PathTrackingPTP_Parallel(List<string> ids, double duration, NetworkStream stream)
        {
            var trackingTasks = new List<Task>();

            foreach (var id in ids)
            {
                trackingTasks.Add(Task.Run(async () =>
                {
                    string result = await PathTrackingPTP(id, duration);
                    NotifyClientConnection($"Parallel PTP execution result for {id}: {result.Trim()}");

                    if (stream != null && stream.CanWrite)
                    {
                        try
                        {
                            byte[] resultData = Encoding.UTF8.GetBytes(result);
                            await stream.WriteAsync(resultData, 0, resultData.Length);
                            await stream.FlushAsync();
                            NotifyClientConnection($"Controller Sent: {result.Trim()}");
                        }
                        catch (Exception ex)
                        {
                            NotifyClientConnection($"Failed to send parallel PTP result for {id}: {ex.Message}");
                        }
                    }
                }));
            }

            await Task.WhenAll(trackingTasks);
        }

        /// <summary>
        /// Executes a high-precision continuous path (CP) motion for the specified controller identifier.
        /// </summary>
        public async Task<string> PathTrackingCP(string id, double duration)
        {
            if (!isPathReady.ContainsKey(id) || !isPathReady[id])
                return $"ERROR, 104, No path data ready for execution for {id}\n";

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

                /// Move to the first point of the trajectory before starting CP motion.
                var startPoint = trajectoryPoints[0];
                NotifyClientConnection($"[Controller {id}] Moving to start point ({startPoint.X:F1}, {startPoint.Y:F1}, {startPoint.Z:F1})...");
                await controller.StartAbsAllFromCenterAsync(startPoint.X, startPoint.Y, startPoint.Z, SPEED_DEFAULT);

                /// 'chainStartPoint' acts as the reference origin for the current continuous move chain.
                TrajectoryPoint chainStartPoint = trajectoryPoints[0];

                /// Start the initial segment (P0 -> P1).
                NotifyClientConnection($"[Controller {id}] Starting initial segment (Point 0 -> Point 1)...");

                await StartSegmentSync(controller, trajectoryPoints[0], trajectoryPoints[1], duration);

                /// Iterate through subsequent segments.
                for (int i = 1; i < trajectoryPoints.Count - 1; i++)
                {
                    var currentTarget = trajectoryPoints[i];
                    var nextTarget = trajectoryPoints[i + 1];
                    var prevTarget = trajectoryPoints[i - 1];

                    bool isContinuous = CheckContinuity(prevTarget, currentTarget, nextTarget);

                    if (isContinuous)
                    {
                        bool overrideSuccess = await WaitForOverrideWindowAndExecute_ByTime(id, controller, currentTarget, nextTarget, chainStartPoint, duration);
                        if (!overrideSuccess)
                        {
                            NotifyClientConnection($"[Controller {id}] WARNING: Missed override window at Point {i} for {id}. Resyncing...");
                            await controller.Wait();
                            chainStartPoint = currentTarget;
                            await StartSegmentSync(controller, currentTarget, nextTarget, duration);
                        }
                    }
                    else
                    {
                        NotifyClientConnection($"[Controller {id}] Segment {i}->{i + 1}: Discontinuous. Stopping at Point {i}.");
                        await controller.Wait();
                        chainStartPoint = currentTarget;
                        await StartSegmentSync(controller, currentTarget, nextTarget, duration);
                    }
                }

                /// Wait for the final segment to complete.
                await controller.Wait();

                double[] finalPos = controller.GetPositionsFromCenter();
                NotifyClientConnection($"[Controller {id}] Motion Complete. Final Pos: ({finalPos[0]:F1}, {finalPos[1]:F1}, {finalPos[2]:F1}).");

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
        public async Task PathTrackingCP_Parallel(List<string> ids, double duration, NetworkStream stream)
        {
            var trackingTasks = new List<Task>();

            foreach (var id in ids)
            {
                trackingTasks.Add(Task.Run(async () =>
                {
                    string result = await PathTrackingCP(id, duration);
                    NotifyClientConnection($"Parallel CP execution result for {id}: {result.Trim()}");

                    if (stream != null && stream.CanWrite)
                    {
                        try
                        {
                            byte[] resultData = Encoding.UTF8.GetBytes(result);
                            await stream.WriteAsync(resultData, 0, resultData.Length);
                            await stream.FlushAsync();
                            NotifyClientConnection($"Controller Sent: {result.Trim()}");
                        }
                        catch (Exception ex)
                        {
                            NotifyClientConnection($"Failed to send parallel CP result for {id}: {ex.Message}");
                        }
                    }
                }));
            }

            await Task.WhenAll(trackingTasks);
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
        /// Starts a synchronized movement for all axes.
        /// </summary>
        private async Task StartSegmentSync(Microsupport controller, TrajectoryPoint start, TrajectoryPoint end, double duration)
        {
            double dx = end.X - start.X;
            double dy = end.Y - start.Y;
            double dz = end.Z - start.Z;

            /// Determine directions based on displacement signs
            var xDir = dx >= 0 ? Microsupport.DIRECTION.FORWARD : Microsupport.DIRECTION.REVERSE;
            var yDir = dy >= 0 ? Microsupport.DIRECTION.FORWARD : Microsupport.DIRECTION.REVERSE;
            var zDir = dz >= 0 ? Microsupport.DIRECTION.REVERSE : Microsupport.DIRECTION.FORWARD; // Note: Z axis direction is inverted considering the physical setup of Microsupport

            /// Adjust speeds for synchronized movement over the specified duration
            controller.AdjustSpeeds(Math.Abs(dx), Math.Abs(dy), Math.Abs(dz), duration);

            /// Start incremental movement on all axes
            controller.StartIncAll(xDir, Math.Abs(dx),
                                   yDir, Math.Abs(dy),
                                   zDir, Math.Abs(dz));

            await Task.Delay(1);
        }

        /// <summary>
        /// Waits for a calculated time window and then executes the IndexOverride using a time-based approach.
        /// </summary>
        private async Task<bool> WaitForOverrideWindowAndExecute_ByTime(string id, Microsupport controller, TrajectoryPoint currentTarget, TrajectoryPoint nextTarget, TrajectoryPoint chainStart, double duration)
        {
            double segmentDurationMs = duration * 1000;
            if (segmentDurationMs <= 0) return false;

            Stopwatch segmentTimer = Stopwatch.StartNew();
            double waitDurationMs = segmentDurationMs * OVERRIDE_PROGRESS_PERCENT;

            if (!controller.IsBusy())
            {
                NotifyClientConnection($"[Controller {id}] Error: Controller is not busy at the start of time-based wait. Aborting override.");
                return false;
            }

            const int marginMs = 30;
            int delayMs = (int)(waitDurationMs - marginMs);
            if (delayMs > 0)
            {
                await Task.Delay(delayMs);
            }

            while (segmentTimer.Elapsed.TotalMilliseconds < waitDurationMs)
            {
                Thread.SpinWait(1);
            }

            if (!controller.IsBusy())
            {
                NotifyClientConnection($"[Controller {id}] WARNING: Missed override window for Point {nextTarget.Index}. Controller stopped before override time was reached.");
                return false;
            }

            NotifyClientConnection($"[Controller {id}] Override window reached at {segmentTimer.Elapsed.TotalMilliseconds:F1}ms. Executing Override to Point {nextTarget.Index} ({nextTarget.X:F1}, {nextTarget.Y:F1}, {nextTarget.Z:F1})");

            double totalDistX = Math.Abs(nextTarget.X - chainStart.X);
            double totalDistY = Math.Abs(nextTarget.Y - chainStart.Y);
            double totalDistZ = Math.Abs(nextTarget.Z - chainStart.Z);

            uint pulseX = (uint)controller.Um2enc(Microsupport.AXIS.X, totalDistX);
            uint pulseY = (uint)controller.Um2enc(Microsupport.AXIS.Y, totalDistY);
            uint pulseZ = (uint)controller.Um2enc(Microsupport.AXIS.Z, totalDistZ);

            double next_dx = nextTarget.X - currentTarget.X;
            double next_dy = nextTarget.Y - currentTarget.Y;
            double next_dz = nextTarget.Z - currentTarget.Z;

            controller.AdjustSpeeds(next_dx, next_dy, next_dz, duration);

            if (Math.Abs(next_dx) > 0.001) controller.IndexOverride(Microsupport.AXIS.X, pulseX);
            if (Math.Abs(next_dy) > 0.001) controller.IndexOverride(Microsupport.AXIS.Y, pulseY);
            if (Math.Abs(next_dz) > 0.001) controller.IndexOverride(Microsupport.AXIS.Z, pulseZ);

            return true;
        }


        #endregion

        #region Server Helper Methods
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