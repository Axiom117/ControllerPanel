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
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.timer2 = new System.Windows.Forms.Timer(this.components);
            this.Status = new System.Windows.Forms.GroupBox();
            this.button3 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
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
            this.posZC = new System.Windows.Forms.Label();
            this.posYC = new System.Windows.Forms.Label();
            this.posXC = new System.Windows.Forms.Label();
            this.btnEmgStop = new System.Windows.Forms.Button();
            this.button7 = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.deviceStatus = new System.Windows.Forms.Label();
            this.clearLogButton = new System.Windows.Forms.Button();
            this.logTextBox = new System.Windows.Forms.TextBox();
            this.startServerButton = new System.Windows.Forms.Button();
            this.panel_left = new System.Windows.Forms.Panel();
            this.Devices = new System.Windows.Forms.GroupBox();
            this.controller4 = new System.Windows.Forms.PictureBox();
            this.controller3 = new System.Windows.Forms.PictureBox();
            this.controller2 = new System.Windows.Forms.PictureBox();
            this.controller1 = new System.Windows.Forms.PictureBox();
            this.panel_right = new System.Windows.Forms.Panel();
            this.controlPanel = new System.Windows.Forms.GroupBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.radioButton2 = new System.Windows.Forms.RadioButton();
            this.label25 = new System.Windows.Forms.Label();
            this.stepDistance = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.radioButton1 = new System.Windows.Forms.RadioButton();
            this.speedSetting = new System.Windows.Forms.GroupBox();
            this.label18 = new System.Windows.Forms.Label();
            this.label17 = new System.Windows.Forms.Label();
            this.trackBar1 = new System.Windows.Forms.TrackBar();
            this.labelSpeed = new System.Windows.Forms.Label();
            this.plusZ = new System.Windows.Forms.Button();
            this.plusY = new System.Windows.Forms.Button();
            this.minusZ = new System.Windows.Forms.Button();
            this.plusX = new System.Windows.Forms.Button();
            this.minusX = new System.Windows.Forms.Button();
            this.minusY = new System.Windows.Forms.Button();
            this.manipulatorTopView = new System.Windows.Forms.PictureBox();
            this.manipulatorFrontView = new System.Windows.Forms.PictureBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.commStatus = new System.Windows.Forms.GroupBox();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.cpModeButton = new System.Windows.Forms.RadioButton();
            this.ptpModeButton = new System.Windows.Forms.RadioButton();
            this.addPathDataButton = new System.Windows.Forms.Button();
            this.startPathTrackingButton = new System.Windows.Forms.Button();
            this.removePathDataButton = new System.Windows.Forms.Button();
            this.loadPathDataButton = new System.Windows.Forms.Button();
            this.pathDataListBox = new System.Windows.Forms.ListBox();
            this.saveLogButton = new System.Windows.Forms.Button();
            this.stopServerButton = new System.Windows.Forms.Button();
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
            this.groupBox3.SuspendLayout();
            this.speedSetting.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.manipulatorTopView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.manipulatorFrontView)).BeginInit();
            this.panel1.SuspendLayout();
            this.commStatus.SuspendLayout();
            this.groupBox4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // toolStrip1
            // 
            this.toolStrip1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.toolStrip1.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripLabel1});
            this.toolStrip1.Location = new System.Drawing.Point(0, 528);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(1482, 25);
            this.toolStrip1.TabIndex = 1;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // toolStripLabel1
            // 
            this.toolStripLabel1.Name = "toolStripLabel1";
            this.toolStripLabel1.Size = new System.Drawing.Size(0, 22);
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
            this.posZ.Size = new System.Drawing.Size(59, 32);
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
            this.posY.Size = new System.Drawing.Size(59, 32);
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
            this.posX.Size = new System.Drawing.Size(59, 32);
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
            this.label6.Size = new System.Drawing.Size(41, 29);
            this.label6.TabIndex = 67;
            this.label6.Text = "μm";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Consolas", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(170, 77);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(41, 29);
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
            this.label1.Size = new System.Drawing.Size(89, 32);
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
            this.label2.Size = new System.Drawing.Size(104, 32);
            this.label2.TabIndex = 63;
            this.label2.Text = "Y^0:  ";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Consolas", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(170, 29);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(41, 29);
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
            this.label3.Size = new System.Drawing.Size(104, 32);
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
            this.groupBox1.Controls.Add(this.posZC);
            this.groupBox1.Controls.Add(this.posYC);
            this.groupBox1.Controls.Add(this.posXC);
            this.groupBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox1.Location = new System.Drawing.Point(224, 33);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(210, 160);
            this.groupBox1.TabIndex = 74;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "From Midpoint of Stoke";
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Font = new System.Drawing.Font("Consolas", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label16.Location = new System.Drawing.Point(174, 121);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(41, 29);
            this.label16.TabIndex = 75;
            this.label16.Text = "μm";
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Font = new System.Drawing.Font("Consolas", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label15.Location = new System.Drawing.Point(174, 77);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(41, 29);
            this.label15.TabIndex = 74;
            this.label15.Text = "μm";
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Font = new System.Drawing.Font("Consolas", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label14.Location = new System.Drawing.Point(174, 30);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(41, 29);
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
            this.label13.Size = new System.Drawing.Size(89, 32);
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
            this.label12.Size = new System.Drawing.Size(89, 32);
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
            this.label11.Size = new System.Drawing.Size(89, 32);
            this.label11.TabIndex = 68;
            this.label11.Text = "X^C: ";
            // 
            // posZC
            // 
            this.posZC.AutoSize = true;
            this.posZC.Font = new System.Drawing.Font("Consolas", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.posZC.Location = new System.Drawing.Point(93, 120);
            this.posZC.Name = "posZC";
            this.posZC.Size = new System.Drawing.Size(59, 32);
            this.posZC.TabIndex = 73;
            this.posZC.Text = "0.0";
            this.posZC.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // posYC
            // 
            this.posYC.AutoSize = true;
            this.posYC.Font = new System.Drawing.Font("Consolas", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.posYC.Location = new System.Drawing.Point(93, 75);
            this.posYC.Name = "posYC";
            this.posYC.Size = new System.Drawing.Size(59, 32);
            this.posYC.TabIndex = 72;
            this.posYC.Text = "0.0";
            this.posYC.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // posXC
            // 
            this.posXC.AutoSize = true;
            this.posXC.Font = new System.Drawing.Font("Consolas", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.posXC.Location = new System.Drawing.Point(93, 28);
            this.posXC.Name = "posXC";
            this.posXC.Size = new System.Drawing.Size(59, 32);
            this.posXC.TabIndex = 71;
            this.posXC.Text = "0.0";
            this.posXC.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
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
            // deviceStatus
            // 
            this.deviceStatus.AutoSize = true;
            this.deviceStatus.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.deviceStatus.ForeColor = System.Drawing.Color.Firebrick;
            this.deviceStatus.Location = new System.Drawing.Point(12, 218);
            this.deviceStatus.Name = "deviceStatus";
            this.deviceStatus.Size = new System.Drawing.Size(0, 29);
            this.deviceStatus.TabIndex = 0;
            // 
            // clearLogButton
            // 
            this.clearLogButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.clearLogButton.Location = new System.Drawing.Point(16, 474);
            this.clearLogButton.Name = "clearLogButton";
            this.clearLogButton.Size = new System.Drawing.Size(137, 39);
            this.clearLogButton.TabIndex = 81;
            this.clearLogButton.Text = "Clear Log";
            this.clearLogButton.UseVisualStyleBackColor = true;
            this.clearLogButton.Click += new System.EventHandler(this.clearLogButton_Click);
            // 
            // logTextBox
            // 
            this.logTextBox.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.logTextBox.Font = new System.Drawing.Font("Consolas", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.logTextBox.ForeColor = System.Drawing.Color.LightGreen;
            this.logTextBox.Location = new System.Drawing.Point(16, 82);
            this.logTextBox.Multiline = true;
            this.logTextBox.Name = "logTextBox";
            this.logTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.logTextBox.Size = new System.Drawing.Size(289, 386);
            this.logTextBox.TabIndex = 79;
            // 
            // startServerButton
            // 
            this.startServerButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.startServerButton.Location = new System.Drawing.Point(16, 37);
            this.startServerButton.Name = "startServerButton";
            this.startServerButton.Size = new System.Drawing.Size(137, 39);
            this.startServerButton.TabIndex = 78;
            this.startServerButton.Text = "Start Server";
            this.startServerButton.UseVisualStyleBackColor = true;
            this.startServerButton.Click += new System.EventHandler(this.StartServerButton_Click);
            // 
            // panel_left
            // 
            this.panel_left.Controls.Add(this.Devices);
            this.panel_left.Controls.Add(this.Status);
            this.panel_left.Dock = System.Windows.Forms.DockStyle.Left;
            this.panel_left.Location = new System.Drawing.Point(0, 0);
            this.panel_left.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.panel_left.Name = "panel_left";
            this.panel_left.Size = new System.Drawing.Size(440, 528);
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
            this.Devices.Size = new System.Drawing.Size(440, 249);
            this.Devices.TabIndex = 58;
            this.Devices.TabStop = false;
            this.Devices.Text = "Devices";
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
            this.panel_right.Size = new System.Drawing.Size(476, 528);
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
            this.controlPanel.Controls.Add(this.manipulatorTopView);
            this.controlPanel.Controls.Add(this.manipulatorFrontView);
            this.controlPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.controlPanel.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.controlPanel.Location = new System.Drawing.Point(0, 0);
            this.controlPanel.Name = "controlPanel";
            this.controlPanel.Size = new System.Drawing.Size(476, 528);
            this.controlPanel.TabIndex = 74;
            this.controlPanel.TabStop = false;
            this.controlPanel.Text = "Control Panel";
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
            // stepDistance
            // 
            this.stepDistance.Font = new System.Drawing.Font("Consolas", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.stepDistance.Location = new System.Drawing.Point(87, 95);
            this.stepDistance.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.stepDistance.Name = "stepDistance";
            this.stepDistance.Size = new System.Drawing.Size(102, 37);
            this.stepDistance.TabIndex = 71;
            this.stepDistance.Text = "0.0";
            this.stepDistance.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("Consolas", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label9.Location = new System.Drawing.Point(195, 101);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(38, 28);
            this.label9.TabIndex = 72;
            this.label9.Text = "μm";
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
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Font = new System.Drawing.Font("Consolas", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label18.Location = new System.Drawing.Point(147, 107);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(54, 23);
            this.label18.TabIndex = 75;
            this.label18.Text = "2500";
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Font = new System.Drawing.Font("Consolas", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label17.Location = new System.Drawing.Point(6, 107);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(32, 23);
            this.label17.TabIndex = 74;
            this.label17.Text = "50";
            // 
            // trackBar1
            // 
            this.trackBar1.LargeChange = 500;
            this.trackBar1.Location = new System.Drawing.Point(6, 78);
            this.trackBar1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.trackBar1.Maximum = 2500;
            this.trackBar1.Name = "trackBar1";
            this.trackBar1.Size = new System.Drawing.Size(183, 56);
            this.trackBar1.TabIndex = 50;
            this.trackBar1.TickFrequency = 10000;
            this.trackBar1.Value = 1500;
            this.trackBar1.Scroll += new System.EventHandler(this.trackBar1_Scroll);
            // 
            // labelSpeed
            // 
            this.labelSpeed.AutoSize = true;
            this.labelSpeed.Font = new System.Drawing.Font("Consolas", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelSpeed.Location = new System.Drawing.Point(51, 41);
            this.labelSpeed.Name = "labelSpeed";
            this.labelSpeed.Size = new System.Drawing.Size(129, 28);
            this.labelSpeed.TabIndex = 73;
            this.labelSpeed.Text = "1500 μm/s";
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
            this.minusY.MouseDown += new System.Windows.Forms.MouseEventHandler(this.buttons_MouseDown);
            this.minusY.MouseUp += new System.Windows.Forms.MouseEventHandler(this.buttons_MouseUp);
            // 
            // manipulatorTopView
            // 
            this.manipulatorTopView.BackColor = System.Drawing.SystemColors.Window;
            this.manipulatorTopView.Image = ((System.Drawing.Image)(resources.GetObject("manipulatorTopView.Image")));
            this.manipulatorTopView.Location = new System.Drawing.Point(71, 99);
            this.manipulatorTopView.Margin = new System.Windows.Forms.Padding(2);
            this.manipulatorTopView.Name = "manipulatorTopView";
            this.manipulatorTopView.Size = new System.Drawing.Size(149, 212);
            this.manipulatorTopView.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.manipulatorTopView.TabIndex = 57;
            this.manipulatorTopView.TabStop = false;
            // 
            // manipulatorFrontView
            // 
            this.manipulatorFrontView.BackColor = System.Drawing.SystemColors.Window;
            this.manipulatorFrontView.Image = ((System.Drawing.Image)(resources.GetObject("manipulatorFrontView.Image")));
            this.manipulatorFrontView.Location = new System.Drawing.Point(291, 99);
            this.manipulatorFrontView.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.manipulatorFrontView.Name = "manipulatorFrontView";
            this.manipulatorFrontView.Size = new System.Drawing.Size(179, 212);
            this.manipulatorFrontView.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.manipulatorFrontView.TabIndex = 56;
            this.manipulatorFrontView.TabStop = false;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.commStatus);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(916, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(566, 528);
            this.panel1.TabIndex = 61;
            // 
            // commStatus
            // 
            this.commStatus.Controls.Add(this.groupBox4);
            this.commStatus.Controls.Add(this.saveLogButton);
            this.commStatus.Controls.Add(this.stopServerButton);
            this.commStatus.Controls.Add(this.clearLogButton);
            this.commStatus.Controls.Add(this.logTextBox);
            this.commStatus.Controls.Add(this.startServerButton);
            this.commStatus.Dock = System.Windows.Forms.DockStyle.Fill;
            this.commStatus.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.commStatus.Location = new System.Drawing.Point(0, 0);
            this.commStatus.Name = "commStatus";
            this.commStatus.Size = new System.Drawing.Size(566, 528);
            this.commStatus.TabIndex = 82;
            this.commStatus.TabStop = false;
            this.commStatus.Text = "Comm Status";
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.pictureBox2);
            this.groupBox4.Controls.Add(this.pictureBox1);
            this.groupBox4.Controls.Add(this.cpModeButton);
            this.groupBox4.Controls.Add(this.ptpModeButton);
            this.groupBox4.Controls.Add(this.addPathDataButton);
            this.groupBox4.Controls.Add(this.startPathTrackingButton);
            this.groupBox4.Controls.Add(this.removePathDataButton);
            this.groupBox4.Controls.Add(this.loadPathDataButton);
            this.groupBox4.Controls.Add(this.pathDataListBox);
            this.groupBox4.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox4.Location = new System.Drawing.Point(312, 37);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(248, 476);
            this.groupBox4.TabIndex = 90;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Path Data List";
            // 
            // pictureBox2
            // 
            this.pictureBox2.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox2.Image")));
            this.pictureBox2.Location = new System.Drawing.Point(10, 291);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(90, 45);
            this.pictureBox2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox2.TabIndex = 93;
            this.pictureBox2.TabStop = false;
            // 
            // pictureBox1
            // 
            this.pictureBox1.ErrorImage = null;
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.InitialImage = ((System.Drawing.Image)(resources.GetObject("pictureBox1.InitialImage")));
            this.pictureBox1.Location = new System.Drawing.Point(16, 345);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(80, 80);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox1.TabIndex = 92;
            this.pictureBox1.TabStop = false;
            // 
            // cpModeButton
            // 
            this.cpModeButton.Appearance = System.Windows.Forms.Appearance.Button;
            this.cpModeButton.Location = new System.Drawing.Point(108, 366);
            this.cpModeButton.Name = "cpModeButton";
            this.cpModeButton.Size = new System.Drawing.Size(130, 40);
            this.cpModeButton.TabIndex = 91;
            this.cpModeButton.Text = "CP Mode";
            this.cpModeButton.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.cpModeButton.UseVisualStyleBackColor = true;
            // 
            // ptpModeButton
            // 
            this.ptpModeButton.Appearance = System.Windows.Forms.Appearance.Button;
            this.ptpModeButton.Checked = true;
            this.ptpModeButton.Location = new System.Drawing.Point(108, 292);
            this.ptpModeButton.Name = "ptpModeButton";
            this.ptpModeButton.Size = new System.Drawing.Size(130, 40);
            this.ptpModeButton.TabIndex = 90;
            this.ptpModeButton.TabStop = true;
            this.ptpModeButton.Text = "PTP Mode";
            this.ptpModeButton.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.ptpModeButton.UseVisualStyleBackColor = true;
            // 
            // addPathDataButton
            // 
            this.addPathDataButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.addPathDataButton.Location = new System.Drawing.Point(10, 198);
            this.addPathDataButton.Name = "addPathDataButton";
            this.addPathDataButton.Size = new System.Drawing.Size(115, 39);
            this.addPathDataButton.TabIndex = 88;
            this.addPathDataButton.Text = "Add";
            this.addPathDataButton.UseVisualStyleBackColor = true;
            this.addPathDataButton.Click += new System.EventHandler(this.addPathDataButton_Click);
            // 
            // startPathTrackingButton
            // 
            this.startPathTrackingButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.startPathTrackingButton.Location = new System.Drawing.Point(10, 431);
            this.startPathTrackingButton.Name = "startPathTrackingButton";
            this.startPathTrackingButton.Size = new System.Drawing.Size(228, 39);
            this.startPathTrackingButton.TabIndex = 86;
            this.startPathTrackingButton.Text = "Start Path Tracking";
            this.startPathTrackingButton.UseVisualStyleBackColor = true;
            this.startPathTrackingButton.Click += new System.EventHandler(this.startPathTrackingButton_Click);
            // 
            // removePathDataButton
            // 
            this.removePathDataButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.removePathDataButton.Location = new System.Drawing.Point(131, 198);
            this.removePathDataButton.Name = "removePathDataButton";
            this.removePathDataButton.Size = new System.Drawing.Size(107, 39);
            this.removePathDataButton.TabIndex = 89;
            this.removePathDataButton.Text = "Remove";
            this.removePathDataButton.UseVisualStyleBackColor = true;
            this.removePathDataButton.Click += new System.EventHandler(this.removePathDataButton_Click);
            // 
            // loadPathDataButton
            // 
            this.loadPathDataButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.loadPathDataButton.Location = new System.Drawing.Point(10, 243);
            this.loadPathDataButton.Name = "loadPathDataButton";
            this.loadPathDataButton.Size = new System.Drawing.Size(228, 39);
            this.loadPathDataButton.TabIndex = 85;
            this.loadPathDataButton.Text = "Load Path Data";
            this.loadPathDataButton.UseVisualStyleBackColor = true;
            this.loadPathDataButton.Click += new System.EventHandler(this.loadPathDataButton_Click);
            // 
            // pathDataListBox
            // 
            this.pathDataListBox.Font = new System.Drawing.Font("Consolas", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.pathDataListBox.FormattingEnabled = true;
            this.pathDataListBox.ItemHeight = 22;
            this.pathDataListBox.Location = new System.Drawing.Point(10, 29);
            this.pathDataListBox.Name = "pathDataListBox";
            this.pathDataListBox.Size = new System.Drawing.Size(228, 158);
            this.pathDataListBox.TabIndex = 87;
            // 
            // saveLogButton
            // 
            this.saveLogButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.saveLogButton.Location = new System.Drawing.Point(159, 474);
            this.saveLogButton.Name = "saveLogButton";
            this.saveLogButton.Size = new System.Drawing.Size(146, 39);
            this.saveLogButton.TabIndex = 83;
            this.saveLogButton.Text = "Save Log";
            this.saveLogButton.UseVisualStyleBackColor = true;
            this.saveLogButton.Click += new System.EventHandler(this.saveLogButton_Click);
            // 
            // stopServerButton
            // 
            this.stopServerButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.stopServerButton.Location = new System.Drawing.Point(159, 37);
            this.stopServerButton.Name = "stopServerButton";
            this.stopServerButton.Size = new System.Drawing.Size(146, 39);
            this.stopServerButton.TabIndex = 82;
            this.stopServerButton.Text = "Stop Server";
            this.stopServerButton.UseVisualStyleBackColor = true;
            this.stopServerButton.Click += new System.EventHandler(this.StopServerButton_Click);
            // 
            // ControllerPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.ClientSize = new System.Drawing.Size(1482, 553);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.panel_right);
            this.Controls.Add(this.panel_left);
            this.Controls.Add(this.toolStrip1);
            this.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.MaximumSize = new System.Drawing.Size(1500, 600);
            this.Name = "ControllerPanel";
            this.Text = "  Microsupport Quick Pro Controller Panel (Developed in Arai Biorobotics Lab)";
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
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.speedSetting.ResumeLayout(false);
            this.speedSetting.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.manipulatorTopView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.manipulatorFrontView)).EndInit();
            this.panel1.ResumeLayout(false);
            this.commStatus.ResumeLayout(false);
            this.commStatus.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
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
        private System.Windows.Forms.PictureBox manipulatorTopView;
        private System.Windows.Forms.Button minusZ;
        private System.Windows.Forms.PictureBox manipulatorFrontView;
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
        private Button startServerButton;
        private TextBox logTextBox;
        private Button clearLogButton;
        private Button btnEmgStop;
        private Panel panel1;
        private GroupBox Devices;
        private GroupBox groupBox1;
        private Label posZC;
        private Label posYC;
        private Label posXC;
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
        private GroupBox speedSetting;
        private GroupBox groupBox3;
        private Label label18;
        private Label label17;
        private Label label25;
        private Button button3;
        private Button button2;
        private Button stopServerButton;
        private Button saveLogButton;
        private Button startPathTrackingButton;
        private Button loadPathDataButton;
        private ListBox pathDataListBox;
        private Button addPathDataButton;
        private Button removePathDataButton;
        private GroupBox groupBox4;
        private RadioButton cpModeButton;
        private RadioButton ptpModeButton;
        private PictureBox pictureBox1;
        private PictureBox pictureBox2;
    }
}