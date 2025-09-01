﻿///
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
using MC104.test;
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

        private MC104.test.ServerTestPanel testPanel;

        /// The controllerServer object to manage the controller server operations.
        private ControllerServer_v1 controllerServer;

        /// Range of movement for each axis in micrometers (um).
        private const double RANGE_X = 20000; // X軸の移動範囲 (um)
        private const double RANGE_Y = 20000; // Y軸の移動範囲 (um)
        private const double RANGE_Z = 30000; // Z軸の移動範囲 (um)

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

            /// Register the TickTimerEvent to the timeline and trigger it for every 500 ms.
            this.timer1.Tick += new EventHandler(TickTimerEvent);
            this.timer1.Interval = 500;
            this.timer1.Start();

            OpenTestPanel();

        }

        private void OpenTestPanel()
        {
            // 检查测试窗口是否已经打开
            if (testPanel == null || testPanel.IsDisposed)
            {
                testPanel = new MC104.test.ServerTestPanel();
                testPanel.Show(); // 使用 Show() 而不是 ShowDialog() 以非模态方式显示
            }
            else
            {
                // 如果窗口已存在，将其带到前台
                testPanel.BringToFront();
                testPanel.WindowState = FormWindowState.Normal;
            }
        }

        private void Form2_FormClosed(object sender, FormClosedEventArgs e)
        {
            testPanel?.Close();

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
            // 将预置的PictureBox控件统一成列表（确保这几个控件已经在Designer中创建）
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
        /// <remarks>This method is triggered by a timer event and performs the following tasks: - Updates
        /// the speed values displayed in the UI. - Retrieves and displays the current positions of connected
        /// controllers. - Calculates and displays relative positions and angles for paired controllers. - Updates the
        /// status of controllers and displays error messages if any controller is not found or decoupled.  The method
        /// assumes that all controllers and UI elements are properly initialized and accessible.</remarks>
        /// <param name="sender">The source of the event, typically the timer triggering the update.</param>
        /// <param name="e">The event data associated with the timer event.</param>
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
                    var controller = Microsupport.controllers[$"MC{i + 1}"];
                    if (controller == null || !controller.IsConnected())
                    {
                        /// Terminate the controller if it is not connected.
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

                    posXC.Text = (currentX - RANGE_X/2).ToString("0.0");
                    posYC.Text = (currentY - RANGE_Y/2).ToString("0.0");
                    posZC.Text = (- currentZ + RANGE_Z/2).ToString("0.0");
                }
            }
        }

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
                    foreach (var icon in controllerIcons)
                    {
                        icon.BackColor = Color.LightGray;
                    }
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

        private void minusX_Click(object sender, EventArgs e)
        {

        }

        private void plusZ_Click(object sender, EventArgs e)
        {

        }

        private void plusY_Click(object sender, EventArgs e)
        {

        }

        private void minusY_Click(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// Determines the axis and direction based on the button that triggered the event.
        /// </summary>
        /// <remarks>The method maps specific button names to corresponding axis and direction values. 
        /// Ensure that the <paramref name="sender"/> is a <see cref="Button"/> with a valid name  corresponding to one
        /// of the predefined mappings.</remarks>
        /// <param name="sender">The object that triggered the event, expected to be a <see cref="Button"/>.</param>
        /// <param name="axis">When the method returns, contains the axis associated with the button. This parameter is passed by
        /// reference.</param>
        /// <param name="dir">When the method returns, contains the direction associated with the button. This parameter is passed by
        /// reference.</param>
        private void GetAxisDirection(object sender, ref Microsupport.AXIS axis, ref Microsupport.DIRECTION direction)
        {
            /// 
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
        /// <remarks>This method determines the axis and direction based on the sender control, sets the
        /// speed for the motion controllers,  and initiates either a continuous jog or a relative movement operation
        /// depending on the selected mode. The operation is performed asynchronously to ensure
        /// responsiveness.</remarks>
        /// <param name="sender">The source of the event, typically a button control.</param>
        /// <param name="e">The <see cref="MouseEventArgs"/> containing event data, such as mouse button state.</param>
        private async void buttons_MouseDown(object sender, MouseEventArgs e)
        {
            Microsupport.AXIS axis = 0;
            Microsupport.DIRECTION direction = 0;

            // Obtain the axis and direction of the button clicked.
            GetAxisDirection(sender, ref axis, ref direction);

            // Obtain the speed value from the trackBar.
            int speedJog = GetSpeed();

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
        /// <remarks>This method stops the motion of an axis when the associated button is released,
        /// provided that <see cref="radioButton1"/> is checked. The axis and direction are determined based on the
        /// button that triggered the event.</remarks>
        /// <param name="sender">The button control that triggered the event.</param>
        /// <param name="e">The mouse event data associated with the MouseUp event.</param>
        private void buttons_MouseUp(object sender, MouseEventArgs e)
        {
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
            while (true)
            {
                Microsupport.controllers[selectedController].StopEmergency();
            }
        }

        private async void button_center_Click(object sender, EventArgs e)
        {
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
        private void StartControllerButton_Click(object sender, EventArgs e)
        {
            try
            {
                // Start with mock server if mockMode is true, otherwise start the real server
                controllerServer = new ControllerServer_v1(5000, mockMode: true);
                controllerServer.OnClientConnection += LogMessage;
                controllerServer.OnTrajectoryReceived += OnTrajectoryReceived;

                controllerServer.Start();

                startControllerButton.Enabled = false;
                startControllerButton.Text = "Controller Running";
                startControllerButton.BackColor = Color.LightGray;

                UpdateStatus("Controller Server running on port 5000", Color.Yellow);
            }
            catch (Exception ex)
            {
                LogMessage($"❌ Failed to start Controller Server: {ex.Message}");
                MessageBox.Show($"Error: {ex.Message}", "Start Failed",
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
        #endregion
    }
}