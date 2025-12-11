///
/// Copyright (c) 2017, Micro Support Co.,Ltd.
/// Web: http://www.microsupport.co.jp/
///
/// Form1 Class 
/// 
///----------------------------------------------------------------------------
///	history
/// 
/// version 1.0.0.0     2017/03/24
///                     New creation.
///                     Autherd by Seki,Tomomasa
/// version 2.0.0.0     2025/05/21
///                     Upated documentation and refactoring
///                     Modified by Haoran Yao
///----------------------------------------------------------------------------

using HpmcstdCs;
using MC104.Models;
using MC104.server;
using MicrosupportController;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MC104
{
    public partial class ControllerPanel : Form
    {
        #region Class Properties and Variables

        /// microsupportConfig object stores the configuration settings for the Microsupport device.
        private MicrosupportConfig MConfig = null;
        /// controllerMapping object to stores the mapping information for the controllers.
        private ControllerMapping controllerMapping = null;

        /// List of PictureBox objects to represent the controller icons in the UI.
        private List<PictureBox> controllerIcons = new List<PictureBox>();
        private string selectedController = "";

        /// The controllerServer object to manage the controller server operations.
        private ControllerServer controllerServer;

        /// Range of movement for each axis in micrometers (um).
        private const double RANGE_X = 20000; // Motion range of X axis (um)
        private const double RANGE_Y = 20000; // Motion range of Y axis (um)
        private const double RANGE_Z = 30000; // Motion range of Z axis (um)

        #endregion

        #region Class Constructor and Initialization
        public ControllerPanel()
        {
            InitializeComponent();

            /// Load the robot configuration from a JSON file.
            LoadConfiguration(ref MConfig);

            /// Load the controller mapping from a JSON file.
            LoadControllerMapping(ref controllerMapping);
        }

        #endregion

        #region Application Loading and Closing Events

        private void Form2_Load(object sender, EventArgs e)
        {
            /// Populate the path data list box on startup
            RefreshPathDataList();

            /// Register the TickTimerEvent to the timeline and trigger it for every 500 ms.
            this.timer1.Tick += new EventHandler(TickTimerEvent);
            this.timer1.Interval = 500;
            this.timer1.Start();

        }

        private void Form2_FormClosed(object sender, FormClosedEventArgs e)
        {
            /// Stop the timer
            if (this.timer1 != null)
            {
                this.timer1.Stop();
            }
            /// Traverse the controllers dictionary and terminate each controller if it is valid.
            foreach (var kvp in Microsupport.controllers)
            {
                var controller = kvp.Value;
                if (controller != null && controller.IsValid)
                {
                    controller.Terminate();
                }
            }
            Microsupport.controllers.Clear();
        }

        #endregion

        #region Event Handlers and UI Refresh Methods

        /// Draw the connected controller icons and labels in the UI. If the user makes a selection, highlight the selected controller icon.
        private void RefreshControllerArea()
        {
            /// Organize the predefined PictureBox controls into a list (ensure these controls are created in the Designer)
            List<PictureBox> predefinedBoxes = new List<PictureBox> { controller1, controller2, controller3, controller4 };

            // If no controllers are detected, show a warning message and hide the PictureBox controls
            if (Microsupport.controllers.Count == 0)
            {
                deviceStatus.Text = "No controllers detected, running in offline mode";
                deviceStatus.ForeColor = Color.Red;
                deviceStatus.Visible = true;

                foreach (var box in predefinedBoxes)
                {
                    box.Visible = false;
                }
                return;
            }
            else
            {
                deviceStatus.Visible = false;
            }

            // Sort the controller IDs in alphabetical order
            var sortedControllerIds = Microsupport.controllers.Keys
                .OrderBy(id => id, StringComparer.Ordinal)
                .ToList();

            // Traverse the predefined PictureBox controls and update them based on the sorted controller IDs
            for (int i = 0; i < predefinedBoxes.Count; i++)
            {
                PictureBox box = predefinedBoxes[i];
                if (i < sortedControllerIds.Count)
                {
                    string controllerId = sortedControllerIds[i];
                    box.Tag = controllerId;
                    box.Visible = true;

                    // Load the controller icon image from the Assets folder
                    string imagePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "microsupport_kit.png");
                    if (File.Exists(imagePath))
                    {
                        box.Image = Image.FromFile(imagePath);
                    }
                    // Set the background color based on selection
                    box.BackColor = (selectedController == controllerId) ? Color.LightBlue : Color.LightGray;
                    // Bind the click event handler to the PictureBox
                    box.Click -= controllerIconClick; // Unsubscribe to avoid multiple event handlers
                    box.Click += controllerIconClick;

                    Label tagLabel = Devices.Controls.OfType<Label>().FirstOrDefault(l => (l.Tag as string) == controllerId);
                    if (tagLabel == null)
                    {
                        // Create a new label for the controller ID
                        tagLabel = new Label
                        {
                            Text = controllerId,
                            AutoSize = true,
                            TextAlign = ContentAlignment.MiddleCenter,
                            ForeColor = Color.Black,
                            Tag = controllerId
                        };
                        // Position the label below the PictureBox
                        Devices.Controls.Add(tagLabel);
                    }
                    else
                    {
                        // Update the label text if it already exists
                        tagLabel.Text = controllerId;
                        tagLabel.Visible = true;
                    }
                    tagLabel.Location = new Point(box.Left + (box.Width - tagLabel.Width) / 2, box.Bottom + 5);
                }
                else
                {
                    // Hide the PictureBox if there are no more controllers
                    box.Visible = false;
                    var tagLabel = this.Controls.OfType<Label>().FirstOrDefault(l => (l.Tag as string) == box.Tag as string);
                    if (tagLabel != null)
                    {
                        // Hide the label if it exists
                        tagLabel.Visible = false;
                    }
                }
            }
        }

        #endregion

        #region Private methods and event handlers

        /// <summary>
        /// Handles periodic updates for the system's controllers and UI elements.
        /// </summary>
        private void TickTimerEvent(object sender, EventArgs e)
        {
            RefreshControllerArea();

            /// Scan for controllers and attempt to connect to each one based on the configuration.
            for (int i = 0; i < MConfig.Params.maxControllers; i++)
            {
                TryConnectController(i);

                /// Check if the controller is present and update its status.
                if (Microsupport.controllers.ContainsKey($"MC{i + 1}"))
                {
                    /// Get the controller instance.
                    var controller = Microsupport.controllers[$"MC{i + 1}"];
                    /// Terminate the controller if it is not connected.
                    if (controller == null || !controller.IsConnected())
                    {
                        Microsupport.controllers[$"MC{i + 1}"].Terminate();
                        /// Remove the controller from the dictionary.
                        Microsupport.controllers.Remove($"MC{i + 1}");
                    }
                }
            }

            /// If there are valid controllers, update the UI with their current positions and speed.
            if (Microsupport.controllers.Count > 0 && selectedController != "")
            {
                /// Display the current positions of the selected controller.
                var deviceId = selectedController;
                var controller = Microsupport.controllers[deviceId];

                if (controller != null && controller.IsValid)
                {
                    /// Get the current position and update the UI.
                    double[] pos = controller.GetPositions();
                    double currentX = pos[0];
                    double currentY = pos[1];
                    double currentZ = pos[2];

                    posX.Text = currentX.ToString("0.0");
                    posY.Text = currentY.ToString("0.0");
                    posZ.Text = currentZ.ToString("0.0");

                    posXC.Text = (currentX - RANGE_X / 2).ToString("0.0");
                    posYC.Text = (currentY - RANGE_Y / 2).ToString("0.0");
                    posZC.Text = (-currentZ + RANGE_Z / 2).ToString("0.0");
                }
            }
        }

        /// <summary>
        /// Attempts to establish a connection to the specified controller using its mapping information.
        /// </summary>
        private void TryConnectController(int controllerId)
        {
            try
            {
                /// Attempt to connect to the controller using the mapping information.
                var controller = Microsupport.GetInstance($"MC{controllerId + 1}", controllerMapping.Controllers[$"MC{controllerId + 1}"]);
            }
            catch (Exception ex)
            {
                /// If an exception occurs while trying to connect, show an error message.
                MessageBox.Show($"Error connecting to MC{controllerId + 1}: {ex.Message}", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void controllerIconClick(object sender, EventArgs e)
        {
            PictureBox clickedIcon = sender as PictureBox;
            if (clickedIcon != null)
            {
                string controllerId = (string)clickedIcon.Tag;

                /// Toggle the selection state of the clicked icon
                if (selectedController == controllerId)
                {
                    clickedIcon.BackColor = Color.LightGray;
                    selectedController = "";
                }
                else
                {
                    // Deselect all icons first
                    foreach (Control control in Devices.Controls)
                    {
                        if (control is PictureBox)
                        {
                            control.BackColor = Color.LightGray;
                        }
                    }
                    // Select the clicked icon
                    clickedIcon.BackColor = Color.LightBlue;
                    selectedController = controllerId;
                }
            }
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            plusX.Text = "X+";
            minusX.Text = "X-";
            plusY.Text = "Y+";
            minusY.Text = "Y-";
            plusZ.Text = "Z+";
            minusZ.Text = "Z-";
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            plusX.Text = "▼";
            minusX.Text = "▲";
            plusY.Text = "▶";
            minusY.Text = "◀";
            plusZ.Text = "▲";
            minusZ.Text = "▼";
        }

        /// <summary>
        /// Determines the axis and direction based on the button that triggered the event.
        /// </summary>
        private void GetAxisDirection(object sender, ref Microsupport.AXIS axis, ref Microsupport.DIRECTION direction)
        {
            /// Get the direction and axis based on the button name.
            Button button = (Button)sender;
            switch (button.Name)
            {
                case "minusX": axis = Microsupport.AXIS.X; direction = Microsupport.DIRECTION.REVERSE; break;
                case "plusX": axis = Microsupport.AXIS.X; direction = Microsupport.DIRECTION.FORWARD; break;
                case "minusY": axis = Microsupport.AXIS.Y; direction = Microsupport.DIRECTION.REVERSE; break;
                case "plusY": axis = Microsupport.AXIS.Y; direction = Microsupport.DIRECTION.FORWARD; break;
                case "plusZ": axis = Microsupport.AXIS.Z; direction = Microsupport.DIRECTION.REVERSE; break;
                case "minusZ": axis = Microsupport.AXIS.Z; direction = Microsupport.DIRECTION.FORWARD; break;
            }
        }

        /// <summary>
        /// Retrieves the current speed value, ensuring it meets the minimum threshold.
        /// </summary>
        /// <returns>The speed value, which is guaranteed to be at least 50.</returns>
        private int GetSpeed()
        {
            int speed = trackBar1.Value;
            if (speed < 50)
                speed = 50;
            return speed;
        }

        /// <summary>
        /// Handles the <see cref="MouseEventArgs"/> for mouse down events on control buttons,  initiating movement or
        /// jogging operations for the specified axis and direction.
        /// </summary>
        private async void buttons_MouseDown(object sender, MouseEventArgs e)
        {
            if (string.IsNullOrEmpty(selectedController))
            {
                MessageBox.Show("Error: Please select a target controller before executing an action.", "No Controller Selected", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            Microsupport.AXIS axis = 0;
            Microsupport.DIRECTION direction = 0;

            /// Obtain the axis and direction of the button clicked.
            GetAxisDirection(sender, ref axis, ref direction);

            /// Obtain the speed value from the trackBar.
            int speedJog = GetSpeed();

            /// Set the speed for the specified axis.
            Microsupport.controllers[selectedController].SetSpeed(axis, speedJog);

            // If jog mode is selected, start the jog operation.
            if (radioButton1.Checked)
            {
                await Task.Run(() => Microsupport.controllers[selectedController].StartJog(axis, direction));
            }
            // If step mode is selected, perform a relative move operation.
            else if (radioButton2.Checked)
            {
                await Task.Run(() => Microsupport.controllers[selectedController].StartInc(axis, direction, double.Parse(stepDistance.Text)));
            }
        }

        /// <summary>
        /// Handles the MouseUp event only for job mode, stopping the motion of the specified axis.
        /// </summary>
        private void buttons_MouseUp(object sender, MouseEventArgs e)
        {
            if (string.IsNullOrEmpty(selectedController))
            {
                /// Silently return as the error message would have been shown on MouseDown.
                return;
            }

            if (radioButton1.Checked)
            {
                Microsupport.AXIS axis = 0;
                Microsupport.DIRECTION dir = 0;
                GetAxisDirection(sender, ref axis, ref dir);

                /// Stop the jog operation for the specified axis.
                Microsupport.controllers[selectedController].StopAxis(axis);
            }
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(selectedController))
            {
                MessageBox.Show("Error: Please select a target controller before executing an action.", "No Controller Selected", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                /// Reset trackbar to a default/safe value if needed
                trackBar1.Value = trackBar1.Minimum;
                labelSpeed.Text = "0 (μm/s)";
                return;
            }

            int speed = trackBar1.Value;
            if (speed < 50)
                speed = 50;

            Microsupport.controllers[selectedController].SetSpeedAll(speed);

            labelSpeed.Text = speed.ToString("0") + " (μm/s)";
        }

        /// <summary>
        /// Handles the click event for the "Origin" button, initiating asynchronous operations.
        /// </summary>
        /// <remarks>This method starts two asynchronous operations and waits for both to complete. 
        /// Ensure that the associated asynchronous methods are properly configured to handle concurrent
        /// execution.</remarks>
        /// <param name="sender">The source of the event, typically the button that was clicked.</param>
        /// <param name="e">An <see cref="EventArgs"/> instance containing the event data.</param>
        private async void button_origin_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(selectedController))
            {
                MessageBox.Show("Error: Please select a target controller before executing an action.", "No Controller Selected", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            Task MCSO = Microsupport.controllers[selectedController].StartOriginAsync();

            await MCSO;
        }

        /// <summary>
        /// Handles the click event for the emergency stop button.
        /// </summary>
        /// <remarks>This method initiates an emergency stop for all connected machines.  Use this method
        /// to immediately halt operations in case of an emergency.</remarks>
        /// <param name="sender">The source of the event, typically the button that was clicked.</param>
        /// <param name="e">An <see cref="EventArgs"/> instance containing the event data.</param>
        private void button_emgstop_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(selectedController))
            {
                MessageBox.Show("Error: Please select a target controller before executing an action.", "No Controller Selected", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            while (true)
            {
                Microsupport.controllers[selectedController].StopEmergency();
            }
        }

        private async void button_center_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(selectedController))
            {
                MessageBox.Show("Error: Please select a target controller before executing an action.", "No Controller Selected", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (Microsupport.controllers.ContainsKey(selectedController))
            {
                Task MCCO = Microsupport.controllers[selectedController].StartCenter();

                await MCCO;
            }
            else
            {
                MessageBox.Show("No controller selected or connected.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion

        #region Server Control
        private void StartServerButton_Click(object sender, EventArgs e)
        {
            try
            {
                /// Start with mock server if mockMode is true, otherwise start the real server
                controllerServer = new ControllerServer(5000, mockMode: false);
                controllerServer.OnClientConnection += LogMessage;
                controllerServer.OnTrajectoryReceived += OnTrajectoryReceived;

                controllerServer.Start();

                startServerButton.Enabled = false;
                startServerButton.Text = "Running";
                startServerButton.BackColor = Color.LightGreen;
                stopServerButton.Enabled = true;
                loadPathDataButton.Enabled = true;
                startPathTrackingButton.Enabled = true;
                addPathDataButton.Enabled = true;

                UpdateStatus("Controller Server running on port 5000", Color.Yellow);
            }
            catch (Exception ex)
            {
                LogMessage($"❌ Failed to start Controller Server: {ex.Message}");
                MessageBox.Show($"Error: {ex.Message}", "Start Failed",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Handles the click event for the Stop Server button, stopping the controller server and updating the user
        /// interface to reflect the server's stopped state.
        /// </summary>
        private void StopServerButton_Click(object sender, EventArgs e)
        {
            try
            {
                controllerServer.Stop();
                startServerButton.Enabled = true;
                startServerButton.Text = "Start";
                startServerButton.BackColor = Color.LightGray;
                stopServerButton.Enabled = false;
                loadPathDataButton.Enabled = false;
                startPathTrackingButton.Enabled = false;
                addPathDataButton.Enabled = false;

                UpdateStatus("Controller Server stopped", Color.Red);
            }
            catch (Exception ex)
            {
                LogMessage($"❌ Failed to stop Controller Server: {ex.Message}");
                MessageBox.Show($"Error: {ex.Message}", "Stop Failed",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
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
        }

        private void LogMessage(string message)
        {
            if (InvokeRequired)
            {
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

            logTextBox.AppendText(message + Environment.NewLine);
            logTextBox.SelectionStart = logTextBox.Text.Length;
            logTextBox.ScrollToCaret();

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
            if (logTextBox.Lines.Length > 1000)
            {
                var lines = logTextBox.Lines;
                var newLines = lines.Skip(lines.Length - 800).ToArray();
                logTextBox.Lines = newLines;
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
        }

        #endregion

        #region File I/O and JSON Serialization
        private void LoadConfiguration(ref MicrosupportConfig microsupportConfig)
        {
            /// Load the configuration file path from the application base directory.
            var configFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Configs", "microsupport_config.json");

            try
            {
                /// Load the configuration file using the MicrosupportConfig class.
                microsupportConfig = MicrosupportConfig.LoadFromFile(configFilePath);

                /// Check if the Resolutions section is present in the configuration file.
                if (microsupportConfig?.Resolutions == null)
                {
                    throw new Exception("Resolutions section is missing in the configuration file.");
                }
            }
            catch (Exception ex)
            {
                /// If an error occurs while loading the configuration file, show an error message and exit the application.
                MessageBox.Show($"Failed to load configuration file: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit();
            }
        }

        private void LoadControllerMapping(ref ControllerMapping controllerMapping)
        {
            /// Load the controller mapping configuration from external JSON file.
            var mappingFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Configs", "controller_mapping.json");

            try
            {
                controllerMapping = ControllerMapping.LoadFromFile(mappingFilePath);
                /// Check if the Controllers dictionary is not null and has elements.
                if (controllerMapping.Controllers != null && controllerMapping.Controllers.Count > 0)
                {
                    string debugInfo = "Controllers Mapping Information:\n";
                    /// Iterate through the dictionary and append each key-value pair to the debugInfo string.
                    foreach (var kvp in controllerMapping.Controllers)
                    {
                        /// Format the key-value pair as a string and append it to the debugInfo string.
                        debugInfo += $"{kvp.Key}: Controller #{kvp.Value}\n";
                    }
                    /// Display the debug information in a message box.
                    /// MessageBox.Show(debugInfo, "DEBUG INFO", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    /// If the Controllers dictionary is null or empty, show a warning message.
                    MessageBox.Show("Controllers Dictionary is NULL or EMPTY", "WARNING", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                /// If an exception occurs while loading the mapping file, show an error message and exit the application.
                MessageBox.Show($"Loading controller mapping failed: {ex.Message}", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit();
                return;
            }
        }

        /// <summary>
        /// Refreshes the list of available path data files displayed in the user interface.
        /// </summary>
        private void RefreshPathDataList()
        {
            try
            {
                string dataDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data");
                if (!Directory.Exists(dataDirectory))
                {
                    Directory.CreateDirectory(dataDirectory);
                }

                pathDataListBox.Items.Clear();

                var csvFiles = Directory.GetFiles(dataDirectory, "*.csv")
                                        .Select(Path.GetFileName)
                                        .ToArray();

                pathDataListBox.Items.AddRange(csvFiles);
                LogMessage($"Found {csvFiles.Length} path data files in 'data/' directory.");
            }
            catch (Exception ex)
            {
                LogMessage($"ERROR: Could not refresh path data list: {ex.Message}");
            }
        }

        private void addPathDataButton_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "CSV files (*.csv)|*.csv|All files (*.*)|*.*";
                openFileDialog.Title = "Add Path Data File";

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        string sourceFile = openFileDialog.FileName;
                        string dataDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data");
                        string destinationFile = Path.Combine(dataDirectory, Path.GetFileName(sourceFile));

                        if (!Directory.Exists(dataDirectory))
                        {
                            Directory.CreateDirectory(dataDirectory);
                        }

                        File.Copy(sourceFile, destinationFile, true); // true to overwrite if exists
                        LogMessage($"Successfully added '{Path.GetFileName(sourceFile)}' to data directory.");

                        RefreshPathDataList();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error adding file: {ex.Message}", "File Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        LogMessage($"ERROR: Failed to add file. {ex.Message}");
                    }
                }
            }
        }

        private void loadPathDataButton_Click(object sender, EventArgs e)
        {
            if (controllerServer == null)
            {
                MessageBox.Show("Please start the server first.", "Server Not Running", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (pathDataListBox.SelectedItem == null)
            {
                MessageBox.Show("Please select a path data file from the list.", "No File Selected", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                string fileName = pathDataListBox.SelectedItem.ToString();
                string dataDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data");
                string filePath = Path.Combine(dataDirectory, fileName);

                controllerServer.LoadPathDataFromFile(filePath);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading file: {ex.Message}", "File Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                LogMessage($"ERROR: Failed to load path file. {ex.Message}");
            }
        }

        private void startPathTrackingButton_Click(object sender, EventArgs e)
        {
            if (controllerServer == null)
            {
                MessageBox.Show("Please start the server first.", "Server Not Running", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // In mock mode, we can assume MC1 and MC2. For real mode, you might need to select them.
            _ = controllerServer.PathTrackingCP("MC1", "MC2");
        }
        #endregion
    }
}