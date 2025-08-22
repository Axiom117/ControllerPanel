using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;

using HpmcstdCs;

namespace Sample1
{
	/// <summary>
	/// Form1 �̊T�v�̐����ł��B
	///	�P.�uBoard open�v�{�^���Ń{�[�h���I�[�v�����܂��B��Ƃ��ăX�C�b�`�ԍ��O�̃{�[�h���I�[�v�����܂��B
	/// �Q.�uBoard set up�v�{�^���ő�P���̊e��ݒ���s���܂��B
	/// �R.�uDrive start�v�{�^���ő�P���̃h���C�u���J�n���܂��B
	/// �S.�uStatus read�v�{�^���ő�P���̊e���Ԃ�\�����܂��B
	/// �T.�uBoard close�v�{�^���Ń{�[�h���N���[�Y���܂��B
	/// </summary>
	public class Form1 : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Button button1;
		private System.Windows.Forms.Button button2;
		private System.Windows.Forms.Button button3;
		private System.Windows.Forms.Button button4;
		private System.Windows.Forms.Button button5;
		/// <summary>
		/// �K�v�ȃf�U�C�i�ϐ��ł��B
		/// </summary>
		private System.ComponentModel.Container components = null;

		//
		string szBoardName = "MCUSB4sd";
		public const ushort MCUSB4sd_AXIS1 = 0;
		public const ushort MCUSB4sd_AXIS2 = 1;
		public const ushort MCUSB4sd_AXIS3 = 2;
		public const ushort MCUSB4sd_AXIS4 = 3;
		public const int WM_USER = 0x400;
		public uint hDevice;

		public Form1()
		{
			//
			// Windows �t�H�[�� �f�U�C�i �T�|�[�g�ɕK�v�ł��B
			//
			InitializeComponent();

			//
			// TODO: InitializeComponent �Ăяo���̌�ɁA�R���X�g���N�^ �R�[�h��ǉ����Ă��������B
			//
		}

		/// <summary>
		/// �g�p����Ă��郊�\�[�X�Ɍ㏈�������s���܂��B
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if (components != null) 
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows �t�H�[�� �f�U�C�i�Ő������ꂽ�R�[�h 
		/// <summary>
		/// �f�U�C�i �T�|�[�g�ɕK�v�ȃ��\�b�h�ł��B���̃��\�b�h�̓��e��
		/// �R�[�h �G�f�B�^�ŕύX���Ȃ��ł��������B
		/// </summary>
		private void InitializeComponent()
		{
			this.button1 = new System.Windows.Forms.Button();
			this.button2 = new System.Windows.Forms.Button();
			this.button3 = new System.Windows.Forms.Button();
			this.button4 = new System.Windows.Forms.Button();
			this.button5 = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// button1
			// 
			this.button1.Location = new System.Drawing.Point(24, 24);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(88, 24);
			this.button1.TabIndex = 0;
			this.button1.Text = "Board open";
			this.button1.Click += new System.EventHandler(this.button1_Click);
			// 
			// button2
			// 
			this.button2.Location = new System.Drawing.Point(24, 64);
			this.button2.Name = "button2";
			this.button2.Size = new System.Drawing.Size(88, 24);
			this.button2.TabIndex = 1;
			this.button2.Text = "Board close";
			this.button2.Click += new System.EventHandler(this.button2_Click);
			// 
			// button3
			// 
			this.button3.Location = new System.Drawing.Point(24, 104);
			this.button3.Name = "button3";
			this.button3.Size = new System.Drawing.Size(88, 24);
			this.button3.TabIndex = 2;
			this.button3.Text = "Board set up";
			this.button3.Click += new System.EventHandler(this.button3_Click);
			// 
			// button4
			// 
			this.button4.Location = new System.Drawing.Point(24, 144);
			this.button4.Name = "button4";
			this.button4.Size = new System.Drawing.Size(88, 24);
			this.button4.TabIndex = 3;
			this.button4.Text = "Drive start";
			this.button4.Click += new System.EventHandler(this.button4_Click);
			// 
			// button5
			// 
			this.button5.Location = new System.Drawing.Point(24, 184);
			this.button5.Name = "button5";
			this.button5.Size = new System.Drawing.Size(88, 24);
			this.button5.TabIndex = 4;
			this.button5.Text = "Status read";
			this.button5.Click += new System.EventHandler(this.button5_Click);
			// 
			// Form1
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 12);
			this.ClientSize = new System.Drawing.Size(136, 224);
			this.Controls.Add(this.button5);
			this.Controls.Add(this.button4);
			this.Controls.Add(this.button3);
			this.Controls.Add(this.button2);
			this.Controls.Add(this.button1);
			this.Name = "Form1";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Form1";
			this.ResumeLayout(false);

		}
		#endregion

		/// <summary>
		/// �A�v���P�[�V�����̃��C�� �G���g�� �|�C���g�ł��B
		/// </summary>
		[STAThread]
		static void Main() 
		{
			Application.Run(new Form1());
		}

		///////////////////////////////////////////////////////////////////////////
		// Board open
		///////////////////////////////////////////////////////////////////////////
		private void button1_Click(object sender, System.EventArgs e)
		{
			// �{�[�h���I�[�v�����܂��B��Ƃ��ăX�C�b�`�ԍ��O�̃{�[�h���I�[�v�����܂��B
			hDevice = Hpmcstd.McsdOpen( szBoardName, 0);
			if ( 0xFFFFFFFF == hDevice ) 
			{
				MessageBox.Show( "�{�[�h�̃I�[�v���Ɏ��s���܂���\n", szBoardName, MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
			else
			{
				MessageBox.Show( "Successful Device Open\n", szBoardName, MessageBoxButtons.OK, MessageBoxIcon.Information);
			}
		}

		///////////////////////////////////////////////////////////////////////////
		// Board close
		///////////////////////////////////////////////////////////////////////////
		private void button2_Click(object sender, System.EventArgs e)
		{
			uint	dwRet = Hpmcstd.McsdClose(hDevice);
			if ( dwRet==0 ) 
			{
				MessageBox.Show( "Device Close\n", szBoardName, MessageBoxButtons.OK, MessageBoxIcon.Information);
			}
			else
			{
				string szStr = String.Format(
					"{0} error\n\n" + 
					"Error code: {1:D04}", szBoardName, dwRet);
				MessageBox.Show( szStr, szBoardName, MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		///////////////////////////////////////////////////////////////////////////
		// Drive start
        //	��P���̐ݒ���s���܂��B
		///////////////////////////////////////////////////////////////////////////
		private void button3_Click(object sender, System.EventArgs e)
		{
			uint	dwRet;
			string	szStr;

			Hpmcstd.MCSDSPDDATAPPS	SpeedPPS = new Hpmcstd.MCSDSPDDATAPPS();

			// �p���X�o�͕���	�Q�p���X����
			// DIR   �o�͒[�q 	CW�p���X  �A�N�e�B�u Hi
			// PULSE �o�͒[�q	CCW�p���X �A�N�e�B�u Hi
			dwRet = Hpmcstd.McsdSetPulseMode( hDevice, MCUSB4sd_AXIS1, 4);
			if ( dwRet!=0 ) 
			{
				szStr = String.Format( "{0} error\n\nError code: {1:D04}", szBoardName, dwRet);
				MessageBox.Show( szStr, szBoardName, MessageBoxButtons.OK, MessageBoxIcon.Error);
				return;
			}

			// +LMT		  ���͐M���A�N�e�B�u���x��	Low
			// -LMT		  ���͐M���A�N�e�B�u���x��	Low
			// ALARM	  ���͐M���A�N�e�B�u���x��	Low
			// INPOSITION ���͐M���A�N�e�B�u���x��	Low
			dwRet = Hpmcstd.McsdSetLimit( hDevice, MCUSB4sd_AXIS1, 0x33);
			if ( dwRet!=0 ) 
			{
				szStr = String.Format( "{0} error\n\nError code: {1:D04}", szBoardName, dwRet);
				MessageBox.Show( szStr, szBoardName, MessageBoxButtons.OK, MessageBoxIcon.Error);
				return;
			}

			// EXTERNAL COUNTER ���͎d�l	�Q���M���S���{����
			dwRet = Hpmcstd.McsdSetExternalCounterMode( hDevice, MCUSB4sd_AXIS1, 3);
			if ( dwRet!=0 ) 
			{
				szStr = String.Format( "{0} error\n\nError code: {1:D04}", szBoardName, dwRet);
				MessageBox.Show( szStr, szBoardName, MessageBoxButtons.OK, MessageBoxIcon.Error);
				return;
			}

			// Alarm stop mode set
			// Inposition wait mode reset
			// Limit input stop enable (Emergency stop)
			dwRet = Hpmcstd.McsdSetSignalStop( hDevice, MCUSB4sd_AXIS1, 1, 0, 2);
			if ( dwRet!=0 ) 
			{
				szStr = String.Format( "{0} error\n\nError code: {1:D04}", szBoardName, dwRet);
				MessageBox.Show( szStr, szBoardName, MessageBoxButtons.OK, MessageBoxIcon.Error);
				return;
			}

			// ���N�����x	100PPS
			// �ō����x 	5000PPS
			// �������� 	250mSec
			// �r��������	0%�i�����������j
			SpeedPPS.dAccel = 250;
			SpeedPPS.dHighSpeed = 5000;
			SpeedPPS.dLowSpeed = 100;
			SpeedPPS.dSratio = 0;
			dwRet = Hpmcstd.McsdSetSpeedPPS( hDevice, MCUSB4sd_AXIS1, ref SpeedPPS);
			if ( dwRet!=0 ) 
			{
				szStr = String.Format( "{0} error\n\nError code: {1:D04}", szBoardName, dwRet);
				MessageBox.Show( szStr, szBoardName, MessageBoxButtons.OK, MessageBoxIcon.Error);
				return;
			}

			// �J�E���^�ݒ�
			//
			// INTERNAL COUNTER �y�� EXTERNAL COUNTER �� 0h ���������݂܂�
			dwRet = Hpmcstd.McsdSetCounter( hDevice, MCUSB4sd_AXIS1, 0, 0);
			if ( dwRet!=0 ) 
			{
				szStr = String.Format( "{0} error\n\nError code: {1:D04}", szBoardName, dwRet);
				MessageBox.Show( szStr, szBoardName, MessageBoxButtons.OK, MessageBoxIcon.Error);
				return;
			}
			dwRet = Hpmcstd.McsdSetCounter( hDevice, MCUSB4sd_AXIS1, 1, 0);
			if ( dwRet!=0 ) 
			{
				szStr = String.Format( "{0} error\n\nError code: {1:D04}", szBoardName, dwRet);
				MessageBox.Show( szStr, szBoardName, MessageBoxButtons.OK, MessageBoxIcon.Error);
				return;
			}

			// �R���p���[�^�ݒ�
			//
			// INTERNAL COMPARATER �y�� EXTERNAL COMPARATER �� 0h ���������݂܂�
			dwRet = Hpmcstd.McsdSetComparator( hDevice, MCUSB4sd_AXIS1, 0, 0);
			if ( dwRet!=0 ) 
			{
				szStr = String.Format( "{0} error\n\nError code: {1:D04}", szBoardName, dwRet);
				MessageBox.Show( szStr, szBoardName, MessageBoxButtons.OK, MessageBoxIcon.Error);
				return;
			}
			dwRet = Hpmcstd.McsdSetComparator( hDevice, MCUSB4sd_AXIS1, 1, 0);
			if ( dwRet!=0 ) 
			{
				szStr = String.Format( "{0} error\n\nError code: {1:D04}", szBoardName, dwRet);
				MessageBox.Show( szStr, szBoardName, MessageBoxButtons.OK, MessageBoxIcon.Error);
				return;
			}

			//
			MessageBox.Show( "��P���̐ݒ肪�������܂���\n", szBoardName, MessageBoxButtons.OK, MessageBoxIcon.Information);
		}

		///////////////////////////////////////////////////////////////////////////
		// Drive start
		//	��P���̃h���C�u���s���܂��B
		///////////////////////////////////////////////////////////////////////////
		private void button4_Click(object sender, System.EventArgs e)
		{
			uint	dwRet;
			string	szStr;

			// ��P�����{������ 10000 �p���X�h���C�u���܂��B
			dwRet = Hpmcstd.McsdDriveStart( hDevice, MCUSB4sd_AXIS1, 0, 10000 );
			if ( dwRet!=0 ) 
			{
				szStr = String.Format( "{0} error\n\nError code: {1:D04}", szBoardName, dwRet);
				MessageBox.Show( szStr, szBoardName, MessageBoxButtons.OK, MessageBoxIcon.Error);
				return;
			}
			szStr = String.Format( "{0}\n" + "���䎲 {1} �{������ 10000 �p���X�o�͂��Ă��܂�", szBoardName, MCUSB4sd_AXIS1 );
			MessageBox.Show( szStr, szBoardName, MessageBoxButtons.OK, MessageBoxIcon.Information);
		}

		///////////////////////////////////////////////////////////////////////////
		// status read
		//	��P���̊e�����\�����܂��B
		///////////////////////////////////////////////////////////////////////////
		private void button5_Click(object sender, System.EventArgs e)
		{
			uint	dwRet;
			string	szStr;
			uint	dwData;
			Hpmcstd.MCSDSTATUS	Status	= new Hpmcstd.MCSDSTATUS();

			// �����J�E���^�[��\�����܂��B
			dwRet = Hpmcstd.McsdGetCounter( hDevice, MCUSB4sd_AXIS1, 0, out dwData );
			if ( dwRet!=0 ) 
			{
				szStr = String.Format( "{0} error\n\nError code: {1:D04}", szBoardName, dwRet);
				MessageBox.Show( szStr, szBoardName, MessageBoxButtons.OK, MessageBoxIcon.Error);
				return;
			}
			// �����J�E���^�� 28 �r�b�g�Ȃ̂ŕ����g�����܂��B
			if ( (dwData & 0x08000000) == 0x08000000 ) dwData |= 0xF0000000;
			szStr = String.Format("{0}\n" + "���䎲 {1} �����J�E���^�[: {2:D}", szBoardName, MCUSB4sd_AXIS1, dwData);
			MessageBox.Show( szStr, szBoardName, MessageBoxButtons.OK, MessageBoxIcon.Information);

			// �O���J�E���^�[��\�����܂��B
			dwRet = Hpmcstd.McsdGetCounter( hDevice, MCUSB4sd_AXIS1, 1, out dwData );
			if ( dwRet!=0 ) 
			{
				szStr = String.Format( "{0} error\n\nError code: {1:D04}", szBoardName, dwRet);
				MessageBox.Show( szStr, szBoardName, MessageBoxButtons.OK, MessageBoxIcon.Error);
				return;
			}
			// �O���J�E���^�� 28 �r�b�g�Ȃ̂ŕ����g�����܂��B
			if ( (dwData & 0x08000000) == 0x08000000 ) dwData |= 0xF0000000;
			szStr = String.Format("{0}\n" + "���䎲 {1} �O���J�E���^�[: {2:D}", szBoardName, MCUSB4sd_AXIS1, dwData);
			MessageBox.Show( szStr, szBoardName, MessageBoxButtons.OK, MessageBoxIcon.Information);

			// �G���h�X�e�[�^�X���擾���ĕ\�����܂��B
			// ���J�j�J���V�O�i�����擾���ĕ\�����܂��B
			// ���j�o�[�T���V�O�i�����擾���ĕ\�����܂��B
			dwRet = Hpmcstd.McsdGetStatus( hDevice, MCUSB4sd_AXIS1, out Status);
			if ( dwRet!=0 ) 
			{
				szStr = String.Format( "{0} error\n\nError code: {1:D04}", szBoardName, dwRet);
				MessageBox.Show( szStr, szBoardName, MessageBoxButtons.OK, MessageBoxIcon.Error);
				return;
			}
			szStr = String.Format(
				"{0}\n" + "���䎲 {1}\n\n" +
				"�G���h�X�e�[�^�X : {2:X02}h\n" +
				"���J�j�J���V�O�i�� : {3:X02}h\n" +
				"���j�o�[�T���V�O�i�� : {4:X02}h\n"
				, szBoardName, MCUSB4sd_AXIS1, Status.bEnd, Status.bMechanical, Status.bUniversal);
			MessageBox.Show( szStr, szBoardName, MessageBoxButtons.OK, MessageBoxIcon.Information);

			// �h���C�u���ł���Ό�����~���܂��B
			if ( (Status.bDrive & 0x01) == 0x01 ) 
			{
				dwRet = Hpmcstd.McsdDriveStop( hDevice, MCUSB4sd_AXIS1, 0);
				if ( dwRet!=0 ) 
				{
					szStr = String.Format( "{0} error\n\nError code: {1:D04}", szBoardName, dwRet);
					MessageBox.Show( szStr, szBoardName, MessageBoxButtons.OK, MessageBoxIcon.Error);
					return;
				}
			}
		}
	}
}
