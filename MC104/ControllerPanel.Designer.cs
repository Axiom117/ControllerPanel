using System;
using System.Drawing;
using System.Windows.Forms;

namespace MC104
{
    partial class ControllerPanel: Form
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ControllerPanel));
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripLabel1 = new System.Windows.Forms.ToolStripLabel();
            this.toolStripProgressBar1 = new System.Windows.Forms.ToolStripProgressBar();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.timer2 = new System.Windows.Forms.Timer(this.components);
            this.Status = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.posZ = new System.Windows.Forms.Label();
            this.posY = new System.Windows.Forms.Label();
            this.posX = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label16 = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.btnEmgStop = new System.Windows.Forms.Button();
            this.deviceStatus = new System.Windows.Forms.Label();
            this.button7 = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.clearLogButton = new System.Windows.Forms.Button();
            this.logTextBox = new System.Windows.Forms.TextBox();
            this.startControllerButton = new System.Windows.Forms.Button();
            this.panel_left = new System.Windows.Forms.Panel();
            this.Devices = new System.Windows.Forms.GroupBox();
            this.controller4 = new System.Windows.Forms.PictureBox();
            this.controller3 = new System.Windows.Forms.PictureBox();
            this.controller2 = new System.Windows.Forms.PictureBox();
            this.controller1 = new System.Windows.Forms.PictureBox();
            this.panel_right = new System.Windows.Forms.Panel();
            this.controlPanel = new System.Windows.Forms.GroupBox();
            this.labelSpeed = new System.Windows.Forms.Label();
            this.plusZ = new System.Windows.Forms.Button();
            this.plusY = new System.Windows.Forms.Button();
            this.minusZ = new System.Windows.Forms.Button();
            this.plusX = new System.Windows.Forms.Button();
            this.minusX = new System.Windows.Forms.Button();
            this.minusY = new System.Windows.Forms.Button();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.label9 = new System.Windows.Forms.Label();
            this.microsupportFront = new System.Windows.Forms.PictureBox();
            this.stepDistance = new System.Windows.Forms.TextBox();
            this.radioButton2 = new System.Windows.Forms.RadioButton();
            this.radioButton1 = new System.Windows.Forms.RadioButton();
            this.trackBar1 = new System.Windows.Forms.TrackBar();
            this.panel1 = new System.Windows.Forms.Panel();
            this.commStatus = new System.Windows.Forms.GroupBox();
            this.statusLabel = new System.Windows.Forms.ToolStripLabel();
            this.speedSetting = new System.Windows.Forms.GroupBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.label17 = new System.Windows.Forms.Label();
            this.label18 = new System.Windows.Forms.Label();
            this.label25 = new System.Windows.Forms.Label();
            this.button2 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.toolStrip1.SuspendLayout();
            this.Status.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.panel_left.SuspendLayout();
            this.Devices.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.controller4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.controller3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.controller2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.controller1)).BeginInit();
            this.panel_right.SuspendLayout();
            this.controlPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.microsupportFront)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar1)).BeginInit();
            this.panel1.SuspendLayout();
            this.commStatus.SuspendLayout();
            this.speedSetting.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStrip1
            // 
            this.toolStrip1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.toolStrip1.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripLabel1,
            this.toolStripProgressBar1,
            this.statusLabel});
            this.toolStrip1.Location = new System.Drawing.Point(0, 525);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(1159, 25);
            this.toolStrip1.TabIndex = 1;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // toolStripLabel1
            // 
            this.toolStripLabel1.Name = "toolStripLabel1";
            this.toolStripLabel1.Size = new System.Drawing.Size(0, 22);
            // 
            // toolStripProgressBar1
            // 
            this.toolStripProgressBar1.Name = "toolStripProgressBar1";
            this.toolStripProgressBar1.Size = new System.Drawing.Size(100, 22);
            // 
            // Status
            // 
            this.Status.Controls.Add(this.button3);
            this.Status.Controls.Add(this.button2);
            this.Status.Controls.Add(this.groupBox2);
            this.Status.Controls.Add(this.groupBox1);
            this.Status.Controls.Add(this.btnEmgStop);
            this.Status.Controls.Add(this.button7);
            this.Status.Controls.Add(this.button1);
            this.Status.Dock = System.Windows.Forms.DockStyle.Top;
            this.Status.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Status.Location = new System.Drawing.Point(0, 0);
            this.Status.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Status.Name = "Status";
            this.Status.Padding = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Status.Size = new System.Drawing.Size(440, 279);
            this.Status.TabIndex = 57;
            this.Status.TabStop = false;
            this.Status.Text = "Status";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.posZ);
            this.groupBox2.Controls.Add(this.posY);
            this.groupBox2.Controls.Add(this.posX);
            this.groupBox2.Controls.Add(this.label6);
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox2.Location = new System.Drawing.Point(12, 32);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(206, 160);
            this.groupBox2.TabIndex = 75;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "From Home Position";
            // 
            // posZ
            // 
            this.posZ.AutoSize = true;
            this.posZ.Font = new System.Drawing.Font("Consolas", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.posZ.Location = new System.Drawing.Point(90, 121);
            this.posZ.Name = "posZ";
            this.posZ.Size = new System.Drawing.Size(46, 24);
            this.posZ.TabIndex = 61;
            this.posZ.Text = "0.0";
            this.posZ.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // posY
            // 
            this.posY.AutoSize = true;
            this.posY.Font = new System.Drawing.Font("Consolas", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.posY.Location = new System.Drawing.Point(90, 76);
            this.posY.Name = "posY";
            this.posY.Size = new System.Drawing.Size(46, 24);
            this.posY.TabIndex = 60;
            this.posY.Text = "0.0";
            this.posY.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // posX
            // 
            this.posX.AutoSize = true;
            this.posX.Font = new System.Drawing.Font("Consolas", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.posX.Location = new System.Drawing.Point(90, 31);
            this.posX.Name = "posX";
            this.posX.Size = new System.Drawing.Size(46, 24);
            this.posX.TabIndex = 59;
            this.posX.Text = "0.0";
            this.posX.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Consolas", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(170, 120);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(32, 23);
            this.label6.TabIndex = 67;
            this.label6.Text = "μm";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Consolas", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(170, 77);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(32, 23);
            this.label5.TabIndex = 66;
            this.label5.Text = "μm";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Consolas", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.Red;
            this.label1.Location = new System.Drawing.Point(14, 30);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(70, 24);
            this.label1.TabIndex = 62;
            this.label1.Text = "X^0: ";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Consolas", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.Color.ForestGreen;
            this.label2.Location = new System.Drawing.Point(14, 74);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(82, 24);
            this.label2.TabIndex = 63;
            this.label2.Text = "Y^0:  ";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Consolas", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(170, 29);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(32, 23);
            this.label4.TabIndex = 65;
            this.label4.Text = "μm";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Consolas", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.ForeColor = System.Drawing.Color.Blue;
            this.label3.Location = new System.Drawing.Point(14, 121);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(82, 24);
            this.label3.TabIndex = 64;
            this.label3.Text = "Z^0:  ";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label16);
            this.groupBox1.Controls.Add(this.label15);
            this.groupBox1.Controls.Add(this.label14);
            this.groupBox1.Controls.Add(this.label13);
            this.groupBox1.Controls.Add(this.label12);
            this.groupBox1.Controls.Add(this.label11);
            this.groupBox1.Controls.Add(this.label10);
            this.groupBox1.Controls.Add(this.label8);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox1.Location = new System.Drawing.Point(224, 33);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(210, 160);
            this.groupBox1.TabIndex = 74;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "From Midpoint of Stoke";
            this.groupBox1.Enter += new System.EventHandler(this.groupBox1_Enter);
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Font = new System.Drawing.Font("Consolas", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label16.Location = new System.Drawing.Point(174, 121);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(32, 23);
            this.label16.TabIndex = 75;
            this.label16.Text = "μm";
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Font = new System.Drawing.Font("Consolas", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label15.Location = new System.Drawing.Point(174, 77);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(32, 23);
            this.label15.TabIndex = 74;
            this.label15.Text = "μm";
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Font = new System.Drawing.Font("Consolas", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label14.Location = new System.Drawing.Point(174, 30);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(32, 23);
            this.label14.TabIndex = 68;
            this.label14.Text = "μm";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Font = new System.Drawing.Font("Consolas", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label13.ForeColor = System.Drawing.Color.Blue;
            this.label13.Location = new System.Drawing.Point(10, 121);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(70, 24);
            this.label13.TabIndex = 68;
            this.label13.Text = "Z^C: ";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Font = new System.Drawing.Font("Consolas", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label12.ForeColor = System.Drawing.Color.ForestGreen;
            this.label12.Location = new System.Drawing.Point(10, 75);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(70, 24);
            this.label12.TabIndex = 68;
            this.label12.Text = "Y^C: ";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Font = new System.Drawing.Font("Consolas", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label11.ForeColor = System.Drawing.Color.Red;
            this.label11.Location = new System.Drawing.Point(9, 28);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(70, 24);
            this.label11.TabIndex = 68;
            this.label11.Text = "X^C: ";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Font = new System.Drawing.Font("Consolas", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label10.Location = new System.Drawing.Point(93, 120);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(46, 24);
            this.label10.TabIndex = 73;
            this.label10.Text = "0.0";
            this.label10.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Consolas", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.Location = new System.Drawing.Point(93, 75);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(46, 24);
            this.label8.TabIndex = 72;
            this.label8.Text = "0.0";
            this.label8.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Consolas", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.Location = new System.Drawing.Point(93, 28);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(46, 24);
            this.label7.TabIndex = 71;
            this.label7.Text = "0.0";
            this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // btnEmgStop
            // 
            this.btnEmgStop.BackColor = System.Drawing.Color.Brown;
            this.btnEmgStop.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnEmgStop.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnEmgStop.Location = new System.Drawing.Point(101, 197);
            this.btnEmgStop.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnEmgStop.Name = "btnEmgStop";
            this.btnEmgStop.Size = new System.Drawing.Size(70, 70);
            this.btnEmgStop.TabIndex = 58;
            this.btnEmgStop.Text = "STOP";
            this.btnEmgStop.UseVisualStyleBackColor = false;
            this.btnEmgStop.Click += new System.EventHandler(this.button_emgstop_Click);
            // 
            // deviceStatus
            // 
            this.deviceStatus.AutoSize = true;
            this.deviceStatus.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.deviceStatus.ForeColor = System.Drawing.Color.Firebrick;
            this.deviceStatus.Location = new System.Drawing.Point(12, 218);
            this.deviceStatus.Name = "deviceStatus";
            this.deviceStatus.Size = new System.Drawing.Size(0, 24);
            this.deviceStatus.TabIndex = 0;
            // 
            // button7
            // 
            this.button7.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button7.Location = new System.Drawing.Point(12, 197);
            this.button7.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.button7.Name = "button7";
            this.button7.Size = new System.Drawing.Size(70, 70);
            this.button7.TabIndex = 51;
            this.button7.Text = "Home";
            this.button7.UseVisualStyleBackColor = true;
            this.button7.Click += new System.EventHandler(this.button_origin_Click);
            // 
            // button1
            // 
            this.button1.BackColor = System.Drawing.Color.Green;
            this.button1.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button1.ForeColor = System.Drawing.SystemColors.Control;
            this.button1.Location = new System.Drawing.Point(275, 197);
            this.button1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(70, 70);
            this.button1.TabIndex = 70;
            this.button1.Text = "Center";
            this.button1.UseVisualStyleBackColor = false;
            this.button1.Click += new System.EventHandler(this.button_center_Click);
            // 
            // clearLogButton
            // 
            this.clearLogButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.clearLogButton.Location = new System.Drawing.Point(141, 40);
            this.clearLogButton.Name = "clearLogButton";
            this.clearLogButton.Size = new System.Drawing.Size(102, 39);
            this.clearLogButton.TabIndex = 81;
            this.clearLogButton.Text = "Clear Log";
            this.clearLogButton.UseVisualStyleBackColor = true;
            // 
            // logTextBox
            // 
            this.logTextBox.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.logTextBox.ForeColor = System.Drawing.Color.LightGreen;
            this.logTextBox.Location = new System.Drawing.Point(14, 85);
            this.logTextBox.Multiline = true;
            this.logTextBox.Name = "logTextBox";
            this.logTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.logTextBox.Size = new System.Drawing.Size(229, 422);
            this.logTextBox.TabIndex = 79;
            // 
            // startControllerButton
            // 
            this.startControllerButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.startControllerButton.Location = new System.Drawing.Point(14, 40);
            this.startControllerButton.Name = "startControllerButton";
            this.startControllerButton.Size = new System.Drawing.Size(109, 39);
            this.startControllerButton.TabIndex = 78;
            this.startControllerButton.Text = "Start Server";
            this.startControllerButton.UseVisualStyleBackColor = true;
            this.startControllerButton.Click += new System.EventHandler(this.StartControllerButton_Click);
            // 
            // panel_left
            // 
            this.panel_left.Controls.Add(this.Devices);
            this.panel_left.Controls.Add(this.Status);
            this.panel_left.Dock = System.Windows.Forms.DockStyle.Left;
            this.panel_left.Location = new System.Drawing.Point(0, 0);
            this.panel_left.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.panel_left.Name = "panel_left";
            this.panel_left.Size = new System.Drawing.Size(440, 525);
            this.panel_left.TabIndex = 0;
            // 
            // Devices
            // 
            this.Devices.Controls.Add(this.controller4);
            this.Devices.Controls.Add(this.controller3);
            this.Devices.Controls.Add(this.controller2);
            this.Devices.Controls.Add(this.deviceStatus);
            this.Devices.Controls.Add(this.controller1);
            this.Devices.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Devices.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Devices.Location = new System.Drawing.Point(0, 279);
            this.Devices.Name = "Devices";
            this.Devices.Size = new System.Drawing.Size(440, 246);
            this.Devices.TabIndex = 58;
            this.Devices.TabStop = false;
            this.Devices.Text = "Devices";
            this.Devices.Enter += new System.EventHandler(this.Devices_Enter);
            // 
            // controller4
            // 
            this.controller4.BackColor = System.Drawing.SystemColors.Window;
            this.controller4.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.controller4.Location = new System.Drawing.Point(330, 32);
            this.controller4.Name = "controller4";
            this.controller4.Size = new System.Drawing.Size(100, 180);
            this.controller4.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.controller4.TabIndex = 3;
            this.controller4.TabStop = false;
            // 
            // controller3
            // 
            this.controller3.BackColor = System.Drawing.SystemColors.Window;
            this.controller3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.controller3.Location = new System.Drawing.Point(224, 32);
            this.controller3.Name = "controller3";
            this.controller3.Size = new System.Drawing.Size(100, 180);
            this.controller3.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.controller3.TabIndex = 2;
            this.controller3.TabStop = false;
            // 
            // controller2
            // 
            this.controller2.BackColor = System.Drawing.SystemColors.Window;
            this.controller2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.controller2.Location = new System.Drawing.Point(118, 32);
            this.controller2.Name = "controller2";
            this.controller2.Size = new System.Drawing.Size(100, 180);
            this.controller2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.controller2.TabIndex = 1;
            this.controller2.TabStop = false;
            // 
            // controller1
            // 
            this.controller1.BackColor = System.Drawing.SystemColors.Window;
            this.controller1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.controller1.Location = new System.Drawing.Point(12, 32);
            this.controller1.Name = "controller1";
            this.controller1.Size = new System.Drawing.Size(100, 180);
            this.controller1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.controller1.TabIndex = 0;
            this.controller1.TabStop = false;
            // 
            // panel_right
            // 
            this.panel_right.Controls.Add(this.controlPanel);
            this.panel_right.Dock = System.Windows.Forms.DockStyle.Left;
            this.panel_right.Location = new System.Drawing.Point(440, 0);
            this.panel_right.Margin = new System.Windows.Forms.Padding(2);
            this.panel_right.Name = "panel_right";
            this.panel_right.Size = new System.Drawing.Size(476, 525);
            this.panel_right.TabIndex = 60;
            // 
            // controlPanel
            // 
            this.controlPanel.Controls.Add(this.groupBox3);
            this.controlPanel.Controls.Add(this.speedSetting);
            this.controlPanel.Controls.Add(this.plusZ);
            this.controlPanel.Controls.Add(this.plusY);
            this.controlPanel.Controls.Add(this.minusZ);
            this.controlPanel.Controls.Add(this.plusX);
            this.controlPanel.Controls.Add(this.minusX);
            this.controlPanel.Controls.Add(this.minusY);
            this.controlPanel.Controls.Add(this.pictureBox2);
            this.controlPanel.Controls.Add(this.microsupportFront);
            this.controlPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.controlPanel.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.controlPanel.Location = new System.Drawing.Point(0, 0);
            this.controlPanel.Name = "controlPanel";
            this.controlPanel.Size = new System.Drawing.Size(476, 525);
            this.controlPanel.TabIndex = 74;
            this.controlPanel.TabStop = false;
            this.controlPanel.Text = "Control Panel";
            // 
            // labelSpeed
            // 
            this.labelSpeed.AutoSize = true;
            this.labelSpeed.Font = new System.Drawing.Font("Consolas", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelSpeed.Location = new System.Drawing.Point(51, 41);
            this.labelSpeed.Name = "labelSpeed";
            this.labelSpeed.Size = new System.Drawing.Size(100, 22);
            this.labelSpeed.TabIndex = 73;
            this.labelSpeed.Text = "1500 μm/s";
            this.labelSpeed.Click += new System.EventHandler(this.labelSpeed_Click);
            // 
            // plusZ
            // 
            this.plusZ.BackColor = System.Drawing.SystemColors.ControlDark;
            this.plusZ.FlatAppearance.MouseOverBackColor = System.Drawing.Color.RoyalBlue;
            this.plusZ.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.plusZ.Font = new System.Drawing.Font("Arial", 19.875F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.plusZ.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.plusZ.Location = new System.Drawing.Point(352, 35);
            this.plusZ.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.plusZ.Name = "plusZ";
            this.plusZ.Size = new System.Drawing.Size(60, 60);
            this.plusZ.TabIndex = 11;
            this.plusZ.Text = "▲";
            this.plusZ.UseVisualStyleBackColor = false;
            this.plusZ.Click += new System.EventHandler(this.plusZ_Click);
            this.plusZ.MouseDown += new System.Windows.Forms.MouseEventHandler(this.buttons_MouseDown);
            this.plusZ.MouseUp += new System.Windows.Forms.MouseEventHandler(this.buttons_MouseUp);
            // 
            // plusY
            // 
            this.plusY.BackColor = System.Drawing.SystemColors.ControlDark;
            this.plusY.FlatAppearance.MouseOverBackColor = System.Drawing.Color.LightGreen;
            this.plusY.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.plusY.Font = new System.Drawing.Font("Arial", 19.875F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.plusY.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.plusY.Location = new System.Drawing.Point(225, 182);
            this.plusY.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.plusY.Name = "plusY";
            this.plusY.Size = new System.Drawing.Size(60, 60);
            this.plusY.TabIndex = 10;
            this.plusY.Text = "▶";
            this.plusY.UseVisualStyleBackColor = false;
            this.plusY.Click += new System.EventHandler(this.plusY_Click);
            this.plusY.MouseDown += new System.Windows.Forms.MouseEventHandler(this.buttons_MouseDown);
            this.plusY.MouseUp += new System.Windows.Forms.MouseEventHandler(this.buttons_MouseUp);
            // 
            // minusZ
            // 
            this.minusZ.BackColor = System.Drawing.SystemColors.ControlDark;
            this.minusZ.FlatAppearance.MouseOverBackColor = System.Drawing.Color.RoyalBlue;
            this.minusZ.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.minusZ.Font = new System.Drawing.Font("Arial", 19.875F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.minusZ.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.minusZ.Location = new System.Drawing.Point(352, 315);
            this.minusZ.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.minusZ.Name = "minusZ";
            this.minusZ.Size = new System.Drawing.Size(60, 60);
            this.minusZ.TabIndex = 12;
            this.minusZ.Text = "▼";
            this.minusZ.UseVisualStyleBackColor = false;
            this.minusZ.MouseDown += new System.Windows.Forms.MouseEventHandler(this.buttons_MouseDown);
            this.minusZ.MouseUp += new System.Windows.Forms.MouseEventHandler(this.buttons_MouseUp);
            // 
            // plusX
            // 
            this.plusX.BackColor = System.Drawing.SystemColors.ControlDark;
            this.plusX.FlatAppearance.MouseOverBackColor = System.Drawing.Color.LightCoral;
            this.plusX.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.plusX.Font = new System.Drawing.Font("Arial", 19.875F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.plusX.ForeColor = System.Drawing.SystemColors.Control;
            this.plusX.Location = new System.Drawing.Point(112, 315);
            this.plusX.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.plusX.Name = "plusX";
            this.plusX.Size = new System.Drawing.Size(60, 60);
            this.plusX.TabIndex = 8;
            this.plusX.Text = "▼";
            this.plusX.UseVisualStyleBackColor = false;
            this.plusX.MouseDown += new System.Windows.Forms.MouseEventHandler(this.buttons_MouseDown);
            this.plusX.MouseUp += new System.Windows.Forms.MouseEventHandler(this.buttons_MouseUp);
            // 
            // minusX
            // 
            this.minusX.BackColor = System.Drawing.SystemColors.ControlDark;
            this.minusX.FlatAppearance.MouseOverBackColor = System.Drawing.Color.LightCoral;
            this.minusX.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.minusX.Font = new System.Drawing.Font("Arial", 19.875F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.minusX.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.minusX.Location = new System.Drawing.Point(112, 35);
            this.minusX.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.minusX.Name = "minusX";
            this.minusX.Size = new System.Drawing.Size(60, 60);
            this.minusX.TabIndex = 0;
            this.minusX.Text = "▲";
            this.minusX.UseVisualStyleBackColor = false;
            this.minusX.Click += new System.EventHandler(this.minusX_Click);
            this.minusX.MouseDown += new System.Windows.Forms.MouseEventHandler(this.buttons_MouseDown);
            this.minusX.MouseUp += new System.Windows.Forms.MouseEventHandler(this.buttons_MouseUp);
            // 
            // minusY
            // 
            this.minusY.BackColor = System.Drawing.SystemColors.ControlDark;
            this.minusY.FlatAppearance.MouseOverBackColor = System.Drawing.Color.LightGreen;
            this.minusY.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.minusY.Font = new System.Drawing.Font("Arial", 19.875F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.minusY.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.minusY.Location = new System.Drawing.Point(6, 182);
            this.minusY.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.minusY.Name = "minusY";
            this.minusY.Size = new System.Drawing.Size(60, 60);
            this.minusY.TabIndex = 9;
            this.minusY.Text = "◀";
            this.minusY.UseVisualStyleBackColor = false;
            this.minusY.Click += new System.EventHandler(this.minusY_Click);
            this.minusY.MouseDown += new System.Windows.Forms.MouseEventHandler(this.buttons_MouseDown);
            this.minusY.MouseUp += new System.Windows.Forms.MouseEventHandler(this.buttons_MouseUp);
            // 
            // pictureBox2
            // 
            this.pictureBox2.BackColor = System.Drawing.SystemColors.Window;
            this.pictureBox2.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox2.Image")));
            this.pictureBox2.Location = new System.Drawing.Point(71, 99);
            this.pictureBox2.Margin = new System.Windows.Forms.Padding(2);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(149, 212);
            this.pictureBox2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox2.TabIndex = 57;
            this.pictureBox2.TabStop = false;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("Consolas", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label9.Location = new System.Drawing.Point(195, 101);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(30, 22);
            this.label9.TabIndex = 72;
            this.label9.Text = "μm";
            this.label9.Click += new System.EventHandler(this.label9_Click);
            // 
            // microsupportFront
            // 
            this.microsupportFront.BackColor = System.Drawing.SystemColors.Window;
            this.microsupportFront.Image = ((System.Drawing.Image)(resources.GetObject("microsupportFront.Image")));
            this.microsupportFront.Location = new System.Drawing.Point(291, 99);
            this.microsupportFront.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.microsupportFront.Name = "microsupportFront";
            this.microsupportFront.Size = new System.Drawing.Size(179, 212);
            this.microsupportFront.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.microsupportFront.TabIndex = 56;
            this.microsupportFront.TabStop = false;
            // 
            // stepDistance
            // 
            this.stepDistance.Font = new System.Drawing.Font("Consolas", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.stepDistance.Location = new System.Drawing.Point(87, 95);
            this.stepDistance.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.stepDistance.Name = "stepDistance";
            this.stepDistance.Size = new System.Drawing.Size(102, 31);
            this.stepDistance.TabIndex = 71;
            this.stepDistance.Text = "0.0";
            this.stepDistance.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // radioButton2
            // 
            this.radioButton2.Appearance = System.Windows.Forms.Appearance.Button;
            this.radioButton2.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.radioButton2.Location = new System.Drawing.Point(132, 35);
            this.radioButton2.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.radioButton2.Name = "radioButton2";
            this.radioButton2.Size = new System.Drawing.Size(115, 45);
            this.radioButton2.TabIndex = 15;
            this.radioButton2.Text = "STEP Mode";
            this.radioButton2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.radioButton2.UseVisualStyleBackColor = true;
            this.radioButton2.CheckedChanged += new System.EventHandler(this.radioButton2_CheckedChanged);
            // 
            // radioButton1
            // 
            this.radioButton1.Appearance = System.Windows.Forms.Appearance.Button;
            this.radioButton1.CheckAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.radioButton1.Checked = true;
            this.radioButton1.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.radioButton1.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.radioButton1.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.radioButton1.Location = new System.Drawing.Point(6, 35);
            this.radioButton1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.radioButton1.Name = "radioButton1";
            this.radioButton1.Size = new System.Drawing.Size(115, 45);
            this.radioButton1.TabIndex = 1;
            this.radioButton1.TabStop = true;
            this.radioButton1.Text = "JOG Mode";
            this.radioButton1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.radioButton1.UseVisualStyleBackColor = true;
            this.radioButton1.CheckedChanged += new System.EventHandler(this.radioButton1_CheckedChanged);
            // 
            // trackBar1
            // 
            this.trackBar1.LargeChange = 500;
            this.trackBar1.Location = new System.Drawing.Point(6, 78);
            this.trackBar1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.trackBar1.Maximum = 2500;
            this.trackBar1.Name = "trackBar1";
            this.trackBar1.Size = new System.Drawing.Size(183, 45);
            this.trackBar1.TabIndex = 50;
            this.trackBar1.TickFrequency = 10000;
            this.trackBar1.Value = 1500;
            this.trackBar1.Scroll += new System.EventHandler(this.trackBar1_Scroll);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.commStatus);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(916, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(243, 525);
            this.panel1.TabIndex = 61;
            // 
            // commStatus
            // 
            this.commStatus.Controls.Add(this.clearLogButton);
            this.commStatus.Controls.Add(this.logTextBox);
            this.commStatus.Controls.Add(this.startControllerButton);
            this.commStatus.Dock = System.Windows.Forms.DockStyle.Fill;
            this.commStatus.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.commStatus.Location = new System.Drawing.Point(0, 0);
            this.commStatus.Name = "commStatus";
            this.commStatus.Size = new System.Drawing.Size(243, 525);
            this.commStatus.TabIndex = 82;
            this.commStatus.TabStop = false;
            this.commStatus.Text = "Comm Status";
            // 
            // statusLabel
            // 
            this.statusLabel.Name = "statusLabel";
            this.statusLabel.Size = new System.Drawing.Size(96, 22);
            this.statusLabel.Text = "toolStripLabel2";
            // 
            // speedSetting
            // 
            this.speedSetting.Controls.Add(this.label18);
            this.speedSetting.Controls.Add(this.label17);
            this.speedSetting.Controls.Add(this.trackBar1);
            this.speedSetting.Controls.Add(this.labelSpeed);
            this.speedSetting.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.speedSetting.Location = new System.Drawing.Point(12, 384);
            this.speedSetting.Name = "speedSetting";
            this.speedSetting.Size = new System.Drawing.Size(195, 135);
            this.speedSetting.TabIndex = 74;
            this.speedSetting.TabStop = false;
            this.speedSetting.Text = "Speed";
            this.speedSetting.Enter += new System.EventHandler(this.groupBox3_Enter);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.radioButton2);
            this.groupBox3.Controls.Add(this.label25);
            this.groupBox3.Controls.Add(this.stepDistance);
            this.groupBox3.Controls.Add(this.label9);
            this.groupBox3.Controls.Add(this.radioButton1);
            this.groupBox3.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox3.Location = new System.Drawing.Point(213, 384);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(257, 135);
            this.groupBox3.TabIndex = 75;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Motion Mode";
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Font = new System.Drawing.Font("Consolas", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label17.Location = new System.Drawing.Point(6, 107);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(27, 19);
            this.label17.TabIndex = 74;
            this.label17.Text = "50";
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Font = new System.Drawing.Font("Consolas", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label18.Location = new System.Drawing.Point(147, 107);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(45, 19);
            this.label18.TabIndex = 75;
            this.label18.Text = "2500";
            this.label18.Click += new System.EventHandler(this.label18_Click);
            // 
            // label25
            // 
            this.label25.Font = new System.Drawing.Font("Consolas", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label25.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.label25.Location = new System.Drawing.Point(26, 99);
            this.label25.Name = "label25";
            this.label25.Size = new System.Drawing.Size(50, 24);
            this.label25.TabIndex = 79;
            this.label25.Text = "∆s: ";
            this.label25.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // button2
            // 
            this.button2.BackColor = System.Drawing.Color.Orange;
            this.button2.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button2.Location = new System.Drawing.Point(185, 197);
            this.button2.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(70, 70);
            this.button2.TabIndex = 76;
            this.button2.Text = "PAUSE";
            this.button2.UseVisualStyleBackColor = false;
            // 
            // button3
            // 
            this.button3.BackColor = System.Drawing.Color.RoyalBlue;
            this.button3.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button3.ForeColor = System.Drawing.SystemColors.Control;
            this.button3.Location = new System.Drawing.Point(364, 197);
            this.button3.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(70, 70);
            this.button3.TabIndex = 77;
            this.button3.Text = "SIM";
            this.button3.UseVisualStyleBackColor = false;
            // 
            // ControllerPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.ClientSize = new System.Drawing.Size(1159, 550);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.panel_right);
            this.Controls.Add(this.panel_left);
            this.Controls.Add(this.toolStrip1);
            this.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.MaximumSize = new System.Drawing.Size(1389, 589);
            this.Name = "ControllerPanel";
            this.Text = "  ";
            this.Load += new System.EventHandler(this.Form2_Load);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.Status.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.panel_left.ResumeLayout(false);
            this.Devices.ResumeLayout(false);
            this.Devices.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.controller4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.controller3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.controller2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.controller1)).EndInit();
            this.panel_right.ResumeLayout(false);
            this.controlPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.microsupportFront)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar1)).EndInit();
            this.panel1.ResumeLayout(false);
            this.commStatus.ResumeLayout(false);
            this.commStatus.PerformLayout();
            this.speedSetting.ResumeLayout(false);
            this.speedSetting.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripLabel toolStripLabel1;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Timer timer2;
        private System.Windows.Forms.GroupBox Status;
        private System.Windows.Forms.Panel panel_right;
        private System.Windows.Forms.Panel panel_left;
        private System.Windows.Forms.Button plusY;
        private System.Windows.Forms.Button minusY;
        private System.Windows.Forms.Button plusX;
        private System.Windows.Forms.Button plusZ;
        private System.Windows.Forms.Button minusX;
        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.Button minusZ;
        private System.Windows.Forms.PictureBox microsupportFront;
        private System.Windows.Forms.RadioButton radioButton2;
        private System.Windows.Forms.RadioButton radioButton1;
        private System.Windows.Forms.TrackBar trackBar1;
        private System.Windows.Forms.Button button7;
        private System.Windows.Forms.Label posX;
        private System.Windows.Forms.Label posZ;
        private System.Windows.Forms.Label posY;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button button1;

        #endregion

        private void PaintBorder(Control control, Color borderColor, PaintEventArgs e)
        {
            /// Obtain the client rectangle of the control
            Rectangle borderRectangle = control.ClientRectangle;

            borderRectangle.Inflate(-1, -1);

            using (Pen borderPen = new Pen(borderColor, 1))
            {
                e.Graphics.DrawRectangle(borderPen, borderRectangle);
            }
        }

        private TextBox stepDistance;
        private Label labelSpeed;
        private Label label9;
        private Label deviceStatus;
        private Button startControllerButton;
        private TextBox logTextBox;
        private Button clearLogButton;
        private ToolStripProgressBar toolStripProgressBar1;
        private Button btnEmgStop;
        private Panel panel1;
        private GroupBox Devices;
        private GroupBox groupBox1;
        private Label label10;
        private Label label8;
        private Label label7;
        private GroupBox groupBox2;
        private Label label13;
        private Label label12;
        private Label label11;
        private Label label16;
        private Label label15;
        private Label label14;
        private GroupBox controlPanel;
        private PictureBox controller4;
        private PictureBox controller3;
        private PictureBox controller2;
        private PictureBox controller1;
        private GroupBox commStatus;
        private ToolStripLabel statusLabel;
        private GroupBox speedSetting;
        private GroupBox groupBox3;
        private Label label18;
        private Label label17;
        private Label label25;
        private Button button3;
        private Button button2;
    }
}