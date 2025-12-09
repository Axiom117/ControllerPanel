using System;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using MC104.server;

namespace MC104.test
{
    public partial class ServerTestPanel : Form
    {
        // UI Controls
        private GroupBox testGroupBox;
        private TextBox testLogTextBox;
        private Button startControllerButton;
        private Button startMatlabButton;
        private Button runTestButton;
        private Button clearLogButton;

        // Test command buttons
        private Button getStatusButton;
        private Button stepMoveButton;
        private Button planPathButton;
        private Button executePathButton;

        // Input controls
        private NumericUpDown moveXInput;
        private NumericUpDown moveYInput;
        private NumericUpDown moveZInput;
        private TextBox id1Input;
        private TextBox id2Input;

        private ToolStrip toolStrip;
        private ToolStripLabel statusLabel;
        private ToolStripLabel connectionIcon;

        // Server instances
        private ControllerServer_v1 controllerServer;
        private MockMatlabServer mockMatlabServer;

        private bool isFormClosing = false;

        public ServerTestPanel()
        {
            InitializeComponent();
            InitializeServers();
        }

        private void InitializeComponent()
        {
            this.Text = "API v1.0 Test Panel - Controller & PathPlanner";
            this.Size = new Size(1000, 800);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.MinimumSize = new Size(900, 600);

            // Toolbar
            toolStrip = new ToolStrip();
            connectionIcon = new ToolStripLabel("●")
            {
                ForeColor = Color.Gray,
                Font = new Font("Arial", 12, FontStyle.Bold)
            };
            statusLabel = new ToolStripLabel("Ready to start servers");
            toolStrip.Items.Add(connectionIcon);
            toolStrip.Items.Add(statusLabel);
            this.Controls.Add(toolStrip);

            // Main panel
            testGroupBox = new GroupBox
            {
                Text = "Controller Server & PathPlanner Test",
                Location = new Point(10, 40),
                Size = new Size(this.ClientSize.Width - 20, this.ClientSize.Height - 50),
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right
            };
            this.Controls.Add(testGroupBox); // Add GroupBox to form's controls first

            // Server control panel
            Panel serverPanel = new Panel
            {
                Location = new Point(10, 20),
                Size = new Size(testGroupBox.Width - 20, 60),
                Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top,
                BorderStyle = BorderStyle.FixedSingle
            };

            startControllerButton = new Button
            {
                Text = "Start Controller Server",
                Location = new Point(10, 15),
                Size = new Size(150, 30),
                BackColor = Color.LightBlue
            };
            startControllerButton.Click += StartControllerButton_Click;

            startMatlabButton = new Button
            {
                Text = "Start PathPlanner",
                Location = new Point(170, 15),
                Size = new Size(150, 30),
                BackColor = Color.LightGreen,
                Enabled = false
            };
            startMatlabButton.Click += StartMatlabButton_Click;

            runTestButton = new Button
            {
                Text = "Run Test Sequence",
                Location = new Point(330, 15),
                Size = new Size(130, 30),
                BackColor = Color.LightCoral,
                Enabled = false
            };
            runTestButton.Click += RunTestButton_Click;

            clearLogButton = new Button
            {
                Text = "Clear Log",
                Location = new Point(470, 15),
                Size = new Size(80, 30)
            };
            clearLogButton.Click += (s, e) => testLogTextBox.Clear();

            serverPanel.Controls.AddRange(new Control[]
            {
                startControllerButton, startMatlabButton, runTestButton, clearLogButton
            });

            // Command test panel
            Panel commandPanel = new Panel
            {
                Location = new Point(10, 90),
                Size = new Size(testGroupBox.Width - 20, 120),
                Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top,
                BorderStyle = BorderStyle.FixedSingle
            };

            Label commandLabel = new Label
            {
                Text = "Manual Test Commands:",
                Location = new Point(10, 5),
                Size = new Size(150, 20),
                Font = new Font("Arial", 9F, FontStyle.Bold)
            };

            // Row 1: Get Status

            Label id1Label = new Label
            {
                Text = "ID1:",
                Location = new Point(10, 30),
                Size = new Size(40, 20)
            };
            id1Input = new TextBox
            {
                Location = new Point(50, 30),
                Size = new Size(60, 23)
            };

            Label id2Label = new Label
            {
                Text = "ID2:",
                Location = new Point(120, 30),
                Size = new Size(40, 20)
            };
            id2Input = new TextBox
            {
                Location = new Point(160, 30),
                Size = new Size(60, 23)
            };

            getStatusButton = new Button
            {
                Text = "Get Status",
                Location = new Point(230, 30),
                Size = new Size(100, 25),
                Enabled = false
            };
            getStatusButton.Click += GetStatusButton_Click;

            // Row 2: Step Move
            stepMoveButton = new Button
            {
                Text = "Step Move",
                Location = new Point(10, 60),
                Size = new Size(100, 25),
                Enabled = false
            };
            stepMoveButton.Click += StepMoveButton_Click;

            Label xLabel = new Label { Text = "X:", Location = new Point(120, 63), Size = new Size(20, 20) };
            moveXInput = new NumericUpDown
            {
                Location = new Point(140, 60),
                Size = new Size(60, 23),
                Minimum = -100,
                Maximum = 100,
                Value = 10,
                DecimalPlaces = 2
            };

            Label yLabel = new Label { Text = "Y:", Location = new Point(210, 63), Size = new Size(20, 20) };
            moveYInput = new NumericUpDown
            {
                Location = new Point(230, 60),
                Size = new Size(60, 23),
                Minimum = -100,
                Maximum = 100,
                Value = 5,
                DecimalPlaces = 2
            };

            Label zLabel = new Label { Text = "Z:", Location = new Point(300, 63), Size = new Size(20, 20) };
            moveZInput = new NumericUpDown
            {
                Location = new Point(320, 60),
                Size = new Size(60, 23),
                Minimum = -100,
                Maximum = 100,
                Value = 2,
                DecimalPlaces = 2
            };

            // Row 3: Path Planning
            planPathButton = new Button
            {
                Text = "Plan Path",
                Location = new Point(10, 90),
                Size = new Size(100, 25),
                Enabled = false
            };
            planPathButton.Click += PlanPathButton_Click;

            executePathButton = new Button
            {
                Text = "Execute Path",
                Location = new Point(120, 90),
                Size = new Size(100, 25),
                Enabled = false
            };
            executePathButton.Click += ExecutePathButton_Click;

            Label pathInfo = new Label
            {
                Text = "Plan path to: X=50, Y=30, Z=20",
                Location = new Point(230, 93),
                Size = new Size(200, 20),
                ForeColor = Color.DarkBlue
            };

            commandPanel.Controls.AddRange(new Control[]
            {
                commandLabel, id1Label, id1Input, id2Label, id2Input, getStatusButton,
                stepMoveButton, xLabel, moveXInput, yLabel, moveYInput, zLabel, moveZInput,
                planPathButton, executePathButton, pathInfo
            });

            // Log area
            Label logLabel = new Label
            {
                Text = "Communication Log:",
                Location = new Point(10, 220),
                Size = new Size(150, 20),
                Font = new Font("Arial", 9F, FontStyle.Bold)
            };

            testLogTextBox = new TextBox
            {
                Multiline = true,
                ScrollBars = ScrollBars.Vertical,
                Location = new Point(10, 240),
                Size = new Size(testGroupBox.Width - 20, testGroupBox.Height - 260),
                ReadOnly = true,
                Font = new Font("Consolas", 9F),
                BackColor = Color.Black,
                ForeColor = Color.LightGreen,
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right
            };

            testGroupBox.Controls.Add(serverPanel);
            testGroupBox.Controls.Add(commandPanel);
            testGroupBox.Controls.Add(logLabel);
            testGroupBox.Controls.Add(testLogTextBox);

            // Window resize handler
            this.Resize += (s, e) =>
            {
                if (testGroupBox != null)
                {
                    testGroupBox.Size = new Size(this.ClientSize.Width - 20, this.ClientSize.Height - 50);
                }
                if (serverPanel != null && commandPanel != null)
                {
                    serverPanel.Width = testGroupBox.Width - 20;
                    commandPanel.Width = testGroupBox.Width - 20;
                    testLogTextBox.Width = testGroupBox.Width - 20;
                    testLogTextBox.Height = testGroupBox.Height - 260;
                }
            };
        }

        private void InitializeServers()
        {
            LogMessage("=== API v1.0 Test Panel Initialized ===");
            LogMessage("1. Start Controller Server (C# WinForms)");
            LogMessage("2. Start PathPlanner (Mock MATLAB)");
            LogMessage("3. Run tests or use manual commands");
            LogMessage("=====================================");
        }

        #region Server Control
        private void StartControllerButton_Click(object sender, EventArgs e)
        {
            try
            {
                // Start with mock controllers for testing
                controllerServer = new ControllerServer_v1(5000, mockMode: false);
                controllerServer.OnClientConnection += LogMessage;
                controllerServer.OnTrajectoryReceived += OnTrajectoryReceived;

                controllerServer.Start();

                startControllerButton.Enabled = false;
                startControllerButton.Text = "Controller Running";
                startControllerButton.BackColor = Color.LightGray;
                startMatlabButton.Enabled = true;

                UpdateStatus("Controller Server running on port 5000", Color.Yellow);
            }
            catch (Exception ex)
            {
                LogMessage($"❌ Failed to start Controller Server: {ex.Message}");
                MessageBox.Show($"Error: {ex.Message}", "Start Failed",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void StartMatlabButton_Click(object sender, EventArgs e)
        {
            try
            {
                mockMatlabServer = new MockMatlabServer();
                mockMatlabServer.OnLogMessage += LogMessage;
                mockMatlabServer.Start();

                startMatlabButton.Enabled = false;
                startMatlabButton.Text = "PathPlanner Running";
                startMatlabButton.BackColor = Color.LightGray;

                // Enable test buttons after a short delay
                Task.Delay(1500).ContinueWith(_ =>
                {
                    Invoke(new Action(() =>
                    {
                        runTestButton.Enabled = true;
                        getStatusButton.Enabled = true;
                        stepMoveButton.Enabled = true;
                        planPathButton.Enabled = true;
                        UpdateStatus("Both servers connected", Color.Green);
                    }));
                });

                UpdateStatus("PathPlanner connecting...", Color.Yellow);
            }
            catch (Exception ex)
            {
                LogMessage($"❌ Failed to start PathPlanner: {ex.Message}");
                MessageBox.Show($"Error: {ex.Message}", "Start Failed",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region Test Commands
        private void GetStatusButton_Click(object sender, EventArgs e)
        {
            if (mockMatlabServer == null) return;

            // 获取两个 ID 的输入值
            string id1 = id1Input.Text.Trim();
            string id2 = id2Input.Text.Trim();

            // 验证输入是否为空
            if (string.IsNullOrEmpty(id1) || string.IsNullOrEmpty(id2))
            {
                LogMessage("❌ Please enter both ID1 and ID2.");
                return;
            }

            // 记录日志并发送请求
            LogMessage($"🔍 Requesting status for {id1} and {id2}...");
            mockMatlabServer.GetStatus(id1, id2);
        }

        private void StepMoveButton_Click(object sender, EventArgs e)
        {
            if (mockMatlabServer == null) return;

            double x = (double)moveXInput.Value;
            double y = (double)moveYInput.Value;
            double z = (double)moveZInput.Value;
            string id1 = id1Input.Text.Trim();
            string id2 = id2Input.Text.Trim();

            LogMessage($"🚀 Sending step move to {id1} & {id2}: X={x:F2}, Y={y:F2}, Z={z:F2}");
            mockMatlabServer.StepMove(x, y, z, id1, id2);
        }

        private void PlanPathButton_Click(object sender, EventArgs e)
        {
            if (mockMatlabServer == null) return;

            LogMessage("📐 Planning path to target position...");
            mockMatlabServer.PlanPath("MC1", "MC2", 50, 30, 20, 0.1, 0, 0);

            // Enable execute button after planning
            Task.Delay(1000).ContinueWith(_ =>
            {
                Invoke(new Action(() => executePathButton.Enabled = true));
            });
        }

        private void ExecutePathButton_Click(object sender, EventArgs e)
        {
            if (mockMatlabServer == null) return;

            LogMessage("▶️ Executing planned path...");
            mockMatlabServer.ExecutePath("MC1", "MC2");
            executePathButton.Enabled = false;
        }

        private void RunTestButton_Click(object sender, EventArgs e)
        {
            if (mockMatlabServer == null) return;

            LogMessage("🧪 Running automatic test sequence...");
            mockMatlabServer.RunTestSequence();

            // Disable during test
            runTestButton.Enabled = false;
            Task.Delay(10000).ContinueWith(_ =>
            {
                Invoke(new Action(() => runTestButton.Enabled = true));
            });
        }
        #endregion

        #region Event Handlers
        private void OnTrajectoryReceived(int pointCount)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => OnTrajectoryReceived(pointCount)));
                return;
            }

            LogMessage($"🎯 Trajectory received: {pointCount} points");
            executePathButton.Enabled = true;
        }

        private void LogMessage(string message)
        {
            if (isFormClosing || testLogTextBox == null) return; // Prevent logging after form is closing

            if (testLogTextBox == null) return;

            if (InvokeRequired)
            {
                if (isFormClosing) return; // Prevent re-entrance during closing
                try
                {
                    Invoke(new Action(() => LogMessage(message)));
                }
                catch (ObjectDisposedException)
                {
                    // Ignore if the form is closing
                    return;
                }
                return;
            }

            // Add timestamp if not already present
            if (!message.StartsWith("[") || !message.Contains("]"))
            {
                message = $"[{DateTime.Now:HH:mm:ss.fff}] {message}";
            }

            testLogTextBox.AppendText(message + Environment.NewLine);
            testLogTextBox.SelectionStart = testLogTextBox.Text.Length;
            testLogTextBox.ScrollToCaret();

            // Update status based on message content
            if (message.Contains("Connected") || message.Contains("connected"))
            {
                UpdateStatus("Connected", Color.Green);
            }
            else if (message.Contains("ERROR") || message.Contains("Error"))
            {
                UpdateStatus("Error detected", Color.Red);
            }
            else if (message.Contains("PATH_COMPLETED"))
            {
                UpdateStatus("Path execution completed", Color.Green);
            }

            // Limit log size
            if (testLogTextBox.Lines.Length > 1000)
            {
                var lines = testLogTextBox.Lines;
                var newLines = lines.Skip(lines.Length - 800).ToArray();
                testLogTextBox.Lines = newLines;
            }
        }

        private void UpdateStatus(string message, Color color)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => UpdateStatus(message, color)));
                return;
            }

            statusLabel.Text = message;
            connectionIcon.ForeColor = color;
        }
        #endregion

        #region Cleanup
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            isFormClosing = true; // Prevent further logging

            LogMessage("=== Shutting down test panel ===");

            // Stop servers
            mockMatlabServer?.Stop();
            controllerServer?.Stop();

            // Unsubscribe events
            if (mockMatlabServer != null)
            {
                mockMatlabServer.OnLogMessage -= LogMessage;
            }

            if (controllerServer != null)
            {
                controllerServer.OnClientConnection -= LogMessage;
                controllerServer.OnTrajectoryReceived -= OnTrajectoryReceived;
            }

            base.OnFormClosing(e);
        }
        #endregion
    }
}