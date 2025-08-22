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
    public partial class Form2 : Form
    {
        #region Class properties and variables

        ///// controllers dictionary type stores the valid controllers.
        //private Dictionary<int, Microsupport> controllers = new Dictionary<int, Microsupport>();

        /// microsupportConfig object stores the configuration settings for the Microsupport device.
        private MicrosupportConfig MConfig = null;
        /// controllerMapping object to stores the mapping information for the controllers.
        private ControllerMapping controllerMapping = null;

        /// List of PictureBox objects to represent the controller icons in the UI.
        private List<PictureBox> controllerIcons = new List<PictureBox>();
        private Label noConnectionLabel;
        private string selectedController = "";

        #endregion

        #region Class Constructor and Initialization


        public Form2()
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

            Form3.ShowForm();

            /// Register the TickTimerEvent to the timeline and trigger it for every 500 ms.
            this.timer1.Tick += new EventHandler(TickTimerEvent);
            this.timer1.Interval = 500;
            this.timer1.Start();

        }

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
            /// Firstly remove any disconnected controller icons from the UI.
            var disconnectedIcons = controllerIcons
                .Where(icon => !Microsupport.controllers.ContainsKey((string)icon.Tag))
                .ToList();

            foreach (var icon in disconnectedIcons)
            {
                /// Remove the associated label if it exists.
                var associatedLabel = Devices.Controls
                    .OfType<Label>()
                    .FirstOrDefault(label => (string)label.Tag == (string)icon.Tag);

                if (associatedLabel != null)
                {
                    Devices.Controls.Remove(associatedLabel);
                }

                /// Remove the icon from the UI and the controllerIcons list.
                Devices.Controls.Remove(icon);
                controllerIcons.Remove(icon);
            }

            /// If no controllers are detected, display a label indicating that no controllers are found.
            if (Microsupport.controllers.Count == 0)
            {
                if (noConnectionLabel == null)
                {
                    noConnectionLabel = new Label
                    {
                        Text = "Controllers not detected",
                        AutoSize = true,
                        Location = new Point(10, 30),
                        ForeColor = Color.Red,
                        Visible = true
                    };
                    Devices.Controls.Add(noConnectionLabel);
                }
                noConnectionLabel.Visible = true;
                return;
            }
            /// If controllers are detected, hide the no connection label if it exists.
            else
            {
                if (noConnectionLabel != null)
                {
                    noConnectionLabel.Visible = false;
                }
            }

            foreach (var kvp in Microsupport.controllers)
            {
                string controllerId = kvp.Key;

                /// Get the next X position for the icon based on the current number of icons and spacing.
                int nextX = MConfig.Params.startX;

                /// Check if the icon for the controller already exists in the list.
                var existingIcon = controllerIcons.FirstOrDefault(icon => (string)icon.Tag == controllerId);

                if (existingIcon == null)
                {
                    PictureBox icon = new PictureBox
                    {
                        Name = $"ControllerIcon_{controllerId}",
                        Size = new Size(MConfig.Params.iconSizeU, MConfig.Params.iconSizeV),
                        BackColor = Color.LightGray,
                        BorderStyle = BorderStyle.FixedSingle,
                        /// Store the controller ID in the Tag property for later reference
                        Tag = controllerId,
                        SizeMode = PictureBoxSizeMode.Zoom
                    };

                    /// Load the controller icon image.
                    string imagePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "microsupport.png");
                    if (File.Exists(imagePath))
                    {
                        icon.Image = Image.FromFile(imagePath);
                    }

                    /// Create the label for the controller icon
                    Label label = new Label
                    {
                        Text = controllerId,
                        AutoSize = true,
                        TextAlign = ContentAlignment.MiddleCenter,
                        ForeColor = Color.Black,
                        Tag = controllerId
                    };

                    icon.Click += controllerIconClick;

                    /// Add the icon and label to the form's controls
                    Devices.Controls.Add(icon);
                    Devices.Controls.Add(label);
                    controllerIcons.Add(icon);
                }

                /// Rearrange the remaining icons to align them to the left.
                /// 
                nextX = MConfig.Params.startX; // Reset nextX to the starting position

                /// Sort controllerIcons by their Tag property in alphabetical order
                controllerIcons = controllerIcons
                    .OrderBy(icon => (string)icon.Tag, StringComparer.Ordinal)
                    .ToList();

                foreach (var icon in controllerIcons)
                {
                    icon.Location = new Point(nextX, MConfig.Params.startY);

                    /// Update the associated label's position
                    var associatedLabel = Devices.Controls
                        .OfType<Label>()
                        .FirstOrDefault(label => (string)label.Tag == (string)icon.Tag);

                    if (associatedLabel != null)
                    {
                        associatedLabel.Location = new Point(icon.Left + (icon.Width / 2) - 20, icon.Top + icon.Height + 5);
                    }

                    nextX += MConfig.Params.iconSizeU + MConfig.Params.spacing; // Update the X position for the next icon
                }
            }
        }

        #endregion

        #region private methods and event handlers

        double CurrentX1, CurrentY1, CurrentZ1;      //um
        double CurrentX2, CurrentY2, CurrentZ2;    //um
        double CurrentX3, CurrentY3, CurrentZ3;    //um
        double CurrentX4, CurrentY4, CurrentZ4;   //um

        double anglespeed = 2500;

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
            /// Clear the previous status message.
            toolStripLabel1.Text = "";

            RefreshControllerArea();

            /// Scan for controllers and attempt to connect to each one based on the configuration.
            for (int i = 0; i < MConfig.Params.maxControllers; i++)
            {
                TryConnectController(i);

                /// Check if the controller is present and update its status.
                if (Microsupport.controllers.ContainsKey($"MC{i + 1}"))
                {
                    var controller = Microsupport.controllers[$"MC{i + 1}"];
                    if (controller != null && controller.IsConnected())
                    {
                        /// Update the status label to indicate that the controller is connected.
                        toolStripLabel1.Text += $"MC{i + 1}: Connected  ";
                    }
                    else
                    {
                        selectedController = ""; // Reset selected controller if disconnected
                        /// Terminate the controller if it is not connected.
                        Microsupport.controllers[$"MC{i + 1}"].Terminate();
                        /// Remove the controller from the dictionary.
                        Microsupport.controllers.Remove($"MC{i + 1}");
                    }
                }
            }

            /// If no controllers are found, update the status label to indicate offline mode.
            if (Microsupport.controllers.Count == 0)
            {
                toolStripLabel1.Text = "Controller not detected. Running in offline mode.";
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
                    int[] pos = controller.GetPositionsEnc();
                    double currentX = (double)pos[0] * MConfig.Resolutions.axisX;
                    double currentY = (double)pos[1] * MConfig.Resolutions.axisY;
                    double currentZ = (double)pos[2] * MConfig.Resolutions.axisZ;

                    posX.Text = currentX.ToString("0.0");
                    posY.Text = currentY.ToString("0.0");
                    posZ.Text = currentZ.ToString("0.0");
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

        private void button3_Click(object sender, EventArgs e)
        {

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
            plusX.Text = "▶";
            minusX.Text = "◀";
            plusY.Text = "▼";
            minusY.Text = "▲";
            plusZ.Text = "▲";
            minusZ.Text = "▼";
        }

        private void minusX_Click(object sender, EventArgs e)
        {

        }

        private void groupBox3_Enter(object sender, EventArgs e)
        {

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void plusZ_Click(object sender, EventArgs e)
        {

        }

        private void plusX_Click(object sender, EventArgs e)
        {

        }

        private void groupBox1_Enter(object sender, EventArgs e)
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
                // Which axis to be moved and the direction is obtained from the sender control.
                switch (axis)
                {
                    case Microsupport.AXIS.X:
                        await Microsupport.controllers[selectedController].StepMoveRelative(double.Parse(textBox5.Text), 0, 0, direction);
                        break;
                    case Microsupport.AXIS.Y:
                        await Microsupport.controllers[selectedController].StepMoveRelative(0, double.Parse(textBox6.Text), 0, direction);
                        break;
                    case Microsupport.AXIS.Z:
                        await Microsupport.controllers[selectedController].StepMoveRelative(0, 0, double.Parse(textBox7.Text), direction);
                        break;
                }
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

    #endregion

    #region File I/O and JSON Serialization

        private void btnOpen_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog())
            {
                if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
                {
                    string selectedPath = folderBrowserDialog.SelectedPath;

                    // 获取文件夹中的所有文件
                    string[] files = Directory.GetFiles(selectedPath);

                    // 清空文件列表并添加新文件
                    fileListBox.Items.Clear();
                    fileListBox.Items.AddRange(files);
                }
            }
        }

        #endregion
    }
}