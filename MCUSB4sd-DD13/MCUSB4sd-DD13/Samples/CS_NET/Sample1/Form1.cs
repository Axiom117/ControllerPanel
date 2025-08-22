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
	/// Form1 の概要の説明です。
	///	１.「Board open」ボタンでボードをオープンします。例としてスイッチ番号０のボードをオープンします。
	/// ２.「Board set up」ボタンで第１軸の各種設定を行います。
	/// ３.「Drive start」ボタンで第１軸のドライブを開始します。
	/// ４.「Status read」ボタンで第１軸の各種状態を表示します。
	/// ５.「Board close」ボタンでボードをクローズします。
	/// </summary>
	public class Form1 : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Button button1;
		private System.Windows.Forms.Button button2;
		private System.Windows.Forms.Button button3;
		private System.Windows.Forms.Button button4;
		private System.Windows.Forms.Button button5;
		/// <summary>
		/// 必要なデザイナ変数です。
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
			// Windows フォーム デザイナ サポートに必要です。
			//
			InitializeComponent();

			//
			// TODO: InitializeComponent 呼び出しの後に、コンストラクタ コードを追加してください。
			//
		}

		/// <summary>
		/// 使用されているリソースに後処理を実行します。
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

		#region Windows フォーム デザイナで生成されたコード 
		/// <summary>
		/// デザイナ サポートに必要なメソッドです。このメソッドの内容を
		/// コード エディタで変更しないでください。
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
		/// アプリケーションのメイン エントリ ポイントです。
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
			// ボードをオープンします。例としてスイッチ番号０のボードをオープンします。
			hDevice = Hpmcstd.McsdOpen( szBoardName, 0);
			if ( 0xFFFFFFFF == hDevice ) 
			{
				MessageBox.Show( "ボードのオープンに失敗しました\n", szBoardName, MessageBoxButtons.OK, MessageBoxIcon.Error);
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
        //	第１軸の設定を行います。
		///////////////////////////////////////////////////////////////////////////
		private void button3_Click(object sender, System.EventArgs e)
		{
			uint	dwRet;
			string	szStr;

			Hpmcstd.MCSDSPDDATAPPS	SpeedPPS = new Hpmcstd.MCSDSPDDATAPPS();

			// パルス出力方式	２パルス方式
			// DIR   出力端子 	CWパルス  アクティブ Hi
			// PULSE 出力端子	CCWパルス アクティブ Hi
			dwRet = Hpmcstd.McsdSetPulseMode( hDevice, MCUSB4sd_AXIS1, 4);
			if ( dwRet!=0 ) 
			{
				szStr = String.Format( "{0} error\n\nError code: {1:D04}", szBoardName, dwRet);
				MessageBox.Show( szStr, szBoardName, MessageBoxButtons.OK, MessageBoxIcon.Error);
				return;
			}

			// +LMT		  入力信号アクティブレベル	Low
			// -LMT		  入力信号アクティブレベル	Low
			// ALARM	  入力信号アクティブレベル	Low
			// INPOSITION 入力信号アクティブレベル	Low
			dwRet = Hpmcstd.McsdSetLimit( hDevice, MCUSB4sd_AXIS1, 0x33);
			if ( dwRet!=0 ) 
			{
				szStr = String.Format( "{0} error\n\nError code: {1:D04}", szBoardName, dwRet);
				MessageBox.Show( szStr, szBoardName, MessageBoxButtons.OK, MessageBoxIcon.Error);
				return;
			}

			// EXTERNAL COUNTER 入力仕様	２相信号４逓倍入力
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

			// 自起動速度	100PPS
			// 最高速度 	5000PPS
			// 加速時間 	250mSec
			// Ｓ字加減率	0%（直線加減速）
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

			// カウンタ設定
			//
			// INTERNAL COUNTER 及び EXTERNAL COUNTER に 0h を書き込みます
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

			// コンパレータ設定
			//
			// INTERNAL COMPARATER 及び EXTERNAL COMPARATER に 0h を書き込みます
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
			MessageBox.Show( "第１軸の設定が完了しました\n", szBoardName, MessageBoxButtons.OK, MessageBoxIcon.Information);
		}

		///////////////////////////////////////////////////////////////////////////
		// Drive start
		//	第１軸のドライブを行います。
		///////////////////////////////////////////////////////////////////////////
		private void button4_Click(object sender, System.EventArgs e)
		{
			uint	dwRet;
			string	szStr;

			// 第１軸を＋方向に 10000 パルスドライブします。
			dwRet = Hpmcstd.McsdDriveStart( hDevice, MCUSB4sd_AXIS1, 0, 10000 );
			if ( dwRet!=0 ) 
			{
				szStr = String.Format( "{0} error\n\nError code: {1:D04}", szBoardName, dwRet);
				MessageBox.Show( szStr, szBoardName, MessageBoxButtons.OK, MessageBoxIcon.Error);
				return;
			}
			szStr = String.Format( "{0}\n" + "制御軸 {1} ＋方向に 10000 パルス出力しています", szBoardName, MCUSB4sd_AXIS1 );
			MessageBox.Show( szStr, szBoardName, MessageBoxButtons.OK, MessageBoxIcon.Information);
		}

		///////////////////////////////////////////////////////////////////////////
		// status read
		//	第１軸の各種情報を表示します。
		///////////////////////////////////////////////////////////////////////////
		private void button5_Click(object sender, System.EventArgs e)
		{
			uint	dwRet;
			string	szStr;
			uint	dwData;
			Hpmcstd.MCSDSTATUS	Status	= new Hpmcstd.MCSDSTATUS();

			// 内部カウンターを表示します。
			dwRet = Hpmcstd.McsdGetCounter( hDevice, MCUSB4sd_AXIS1, 0, out dwData );
			if ( dwRet!=0 ) 
			{
				szStr = String.Format( "{0} error\n\nError code: {1:D04}", szBoardName, dwRet);
				MessageBox.Show( szStr, szBoardName, MessageBoxButtons.OK, MessageBoxIcon.Error);
				return;
			}
			// 内部カウンタは 28 ビットなので符号拡張します。
			if ( (dwData & 0x08000000) == 0x08000000 ) dwData |= 0xF0000000;
			szStr = String.Format("{0}\n" + "制御軸 {1} 内部カウンター: {2:D}", szBoardName, MCUSB4sd_AXIS1, dwData);
			MessageBox.Show( szStr, szBoardName, MessageBoxButtons.OK, MessageBoxIcon.Information);

			// 外部カウンターを表示します。
			dwRet = Hpmcstd.McsdGetCounter( hDevice, MCUSB4sd_AXIS1, 1, out dwData );
			if ( dwRet!=0 ) 
			{
				szStr = String.Format( "{0} error\n\nError code: {1:D04}", szBoardName, dwRet);
				MessageBox.Show( szStr, szBoardName, MessageBoxButtons.OK, MessageBoxIcon.Error);
				return;
			}
			// 外部カウンタは 28 ビットなので符号拡張します。
			if ( (dwData & 0x08000000) == 0x08000000 ) dwData |= 0xF0000000;
			szStr = String.Format("{0}\n" + "制御軸 {1} 外部カウンター: {2:D}", szBoardName, MCUSB4sd_AXIS1, dwData);
			MessageBox.Show( szStr, szBoardName, MessageBoxButtons.OK, MessageBoxIcon.Information);

			// エンドステータスを取得して表示します。
			// メカニカルシグナルを取得して表示します。
			// ユニバーサルシグナルを取得して表示します。
			dwRet = Hpmcstd.McsdGetStatus( hDevice, MCUSB4sd_AXIS1, out Status);
			if ( dwRet!=0 ) 
			{
				szStr = String.Format( "{0} error\n\nError code: {1:D04}", szBoardName, dwRet);
				MessageBox.Show( szStr, szBoardName, MessageBoxButtons.OK, MessageBoxIcon.Error);
				return;
			}
			szStr = String.Format(
				"{0}\n" + "制御軸 {1}\n\n" +
				"エンドステータス : {2:X02}h\n" +
				"メカニカルシグナル : {3:X02}h\n" +
				"ユニバーサルシグナル : {4:X02}h\n"
				, szBoardName, MCUSB4sd_AXIS1, Status.bEnd, Status.bMechanical, Status.bUniversal);
			MessageBox.Show( szStr, szBoardName, MessageBoxButtons.OK, MessageBoxIcon.Information);

			// ドライブ中であれば減速停止します。
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
