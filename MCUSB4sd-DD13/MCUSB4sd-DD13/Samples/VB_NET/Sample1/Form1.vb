Option Strict Off
Option Explicit On
Friend Class Form1
	Inherits System.Windows.Forms.Form
#Region "Windows フォーム デザイナによって生成されたコード"
	Public Sub New()
		MyBase.New()
		If m_vb6FormDefInstance Is Nothing Then
			If m_InitializingDefInstance Then
				m_vb6FormDefInstance = Me
			Else
				Try 
					'スタートアップ フォームについては、最初に作成されたインスタンスが既定インスタンスになります。
					If System.Reflection.Assembly.GetExecutingAssembly.EntryPoint.DeclaringType Is Me.GetType Then
						m_vb6FormDefInstance = Me
					End If
				Catch
				End Try
			End If
		End If
		'この呼び出しは、Windows フォーム デザイナで必要です。
		InitializeComponent()
	End Sub
	'Form は、コンポーネント一覧に後処理を実行するために dispose をオーバーライドします。
	Protected Overloads Overrides Sub Dispose(ByVal Disposing As Boolean)
		If Disposing Then
			If Not components Is Nothing Then
				components.Dispose()
			End If
		End If
		MyBase.Dispose(Disposing)
	End Sub
	'Windows フォーム デザイナで必要です。
	Private components As System.ComponentModel.IContainer
	Public ToolTip1 As System.Windows.Forms.ToolTip
    Public WithEvents Command4 As System.Windows.Forms.Button
	Public WithEvents Command3 As System.Windows.Forms.Button
	Public WithEvents Command2 As System.Windows.Forms.Button
	Public WithEvents Command1 As System.Windows.Forms.Button
	'メモ : 以下のプロシージャは Windows フォーム デザイナで必要です。
	'Windows フォーム デザイナを使って変更できます。
	'コード エディタを使って修正しないでください。
	<System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container
        Me.ToolTip1 = New System.Windows.Forms.ToolTip(Me.components)
        Me.Command4 = New System.Windows.Forms.Button
        Me.Command3 = New System.Windows.Forms.Button
        Me.Command2 = New System.Windows.Forms.Button
        Me.Command1 = New System.Windows.Forms.Button
        Me.SuspendLayout()
        '
        'Command4
        '
        Me.Command4.BackColor = System.Drawing.SystemColors.Control
        Me.Command4.Cursor = System.Windows.Forms.Cursors.Default
        Me.Command4.ForeColor = System.Drawing.SystemColors.ControlText
        Me.Command4.Location = New System.Drawing.Point(16, 144)
        Me.Command4.Name = "Command4"
        Me.Command4.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.Command4.Size = New System.Drawing.Size(105, 25)
        Me.Command4.TabIndex = 3
        Me.Command4.Text = "Command4"
        '
        'Command3
        '
        Me.Command3.BackColor = System.Drawing.SystemColors.Control
        Me.Command3.Cursor = System.Windows.Forms.Cursors.Default
        Me.Command3.ForeColor = System.Drawing.SystemColors.ControlText
        Me.Command3.Location = New System.Drawing.Point(16, 104)
        Me.Command3.Name = "Command3"
        Me.Command3.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.Command3.Size = New System.Drawing.Size(105, 25)
        Me.Command3.TabIndex = 2
        Me.Command3.Text = "Command3"
        '
        'Command2
        '
        Me.Command2.BackColor = System.Drawing.SystemColors.Control
        Me.Command2.Cursor = System.Windows.Forms.Cursors.Default
        Me.Command2.ForeColor = System.Drawing.SystemColors.ControlText
        Me.Command2.Location = New System.Drawing.Point(16, 64)
        Me.Command2.Name = "Command2"
        Me.Command2.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.Command2.Size = New System.Drawing.Size(105, 25)
        Me.Command2.TabIndex = 1
        Me.Command2.Text = "Command2"
        '
        'Command1
        '
        Me.Command1.BackColor = System.Drawing.SystemColors.Control
        Me.Command1.Cursor = System.Windows.Forms.Cursors.Default
        Me.Command1.ForeColor = System.Drawing.SystemColors.ControlText
        Me.Command1.Location = New System.Drawing.Point(16, 24)
        Me.Command1.Name = "Command1"
        Me.Command1.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.Command1.Size = New System.Drawing.Size(105, 25)
        Me.Command1.TabIndex = 0
        Me.Command1.Text = "Command1"
        '
        'Form1
        '
        Me.AutoScaleBaseSize = New System.Drawing.Size(5, 12)
        Me.BackColor = System.Drawing.SystemColors.Control
        Me.ClientSize = New System.Drawing.Size(137, 190)
        Me.Controls.Add(Me.Command4)
        Me.Controls.Add(Me.Command3)
        Me.Controls.Add(Me.Command2)
        Me.Controls.Add(Me.Command1)
        Me.Cursor = System.Windows.Forms.Cursors.Default
        Me.Location = New System.Drawing.Point(4, 23)
        Me.Name = "Form1"
        Me.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "Form1"
        Me.ResumeLayout(False)

    End Sub
#End Region 
#Region "アップグレード ウィザードのサポート コード"
	Private Shared m_vb6FormDefInstance As Form1
	Private Shared m_InitializingDefInstance As Boolean
	Public Shared Property DefInstance() As Form1
		Get
			If m_vb6FormDefInstance Is Nothing OrElse m_vb6FormDefInstance.IsDisposed Then
				m_InitializingDefInstance = True
				m_vb6FormDefInstance = New Form1()
				m_InitializingDefInstance = False
			End If
			DefInstance = m_vb6FormDefInstance
		End Get
		Set
			m_vb6FormDefInstance = Value
		End Set
	End Property
#End Region 
	
	Private Sub Form1_Load(ByVal eventSender As System.Object, ByVal eventArgs As System.EventArgs) Handles MyBase.Load
		'Handle get
		ghW = Me.Handle.ToInt32
		'Axis
        gwAxis = MCUSB4sd_AXIS1
		'Caption
		Command1.Text = "Board open"
		Command2.Text = "Board close"
		Command3.Text = "Drive start"
		Command4.Text = "Status read"
    End Sub

    Private Sub Command1_Click(ByVal eventSender As System.Object, ByVal eventArgs As System.EventArgs) Handles Command1.Click
        Dim SpdDataPPS As MCSDSPDDATAPPS
        Dim dwErr As Integer
        'Board Open
        ghDevice = McsdOpen("MCUSB4sd", 0)
        If (ghDevice = G_FALSE) Then
            MsgBox("McsdOpen" & Chr(10) & "BOARD : MCUSB4sd" & Chr(10) & "ID : 0" & Chr(10) & "Open Error !!", MsgBoxStyle.OKOnly + MsgBoxStyle.Critical)
            Exit Sub
        End If
        MsgBox("Board Open (ID 0)", MsgBoxStyle.OKOnly + MsgBoxStyle.Information)
        'Parameter set
        '  Set pulse mode
        '    Pulse mode : 2 pulse
        '    nDIR       : CW  pulse, Active Hi
        '    nPULSE     : CCW pulse, Active Hi
        dwErr = McsdSetPulseMode(ghDevice, gwAxis, 4)
        If (dwErr) Then
            MsgBox("McsdSetPulseMode" & Chr(10) & "Error code : " & CStr(dwErr), MsgBoxStyle.OKOnly + MsgBoxStyle.Critical)
            Exit Sub
        End If
        'Parameter set
        '  Signal active level set
        '    n+LMT active level        Low
        '    n-LMT active level        Low
        '    nAlarm active level       Low
        '    nInposition active level  Low
        dwErr = McsdSetLimit(ghDevice, gwAxis, &H33S)
        If (dwErr) Then
            MsgBox("McsdSetLimit" & Chr(10) & "Error code : " & CStr(dwErr), MsgBoxStyle.OKOnly + MsgBoxStyle.Critical)
            Exit Sub
        End If
        'Parameter set
        '  External counter mode set
        '    Up/down signal count
        dwErr = McsdSetExternalCounterMode(ghDevice, gwAxis, 0)
        If (dwErr) Then
            MsgBox("McsdSetExternalCounterMode" & Chr(10) & "Error code : " & CStr(dwErr), MsgBoxStyle.OKOnly + MsgBoxStyle.Critical)
            Exit Sub
        End If
        'Parameter set
        '  Signal stop set
        '    Alarm stop mode set
        '    Inposition wait mode reset
        '    Limit input stop enable (Emergency stop)
        dwErr = McsdSetSignalStop(ghDevice, gwAxis, 1, 0, 2)
        If (dwErr) Then
            MsgBox("McsdSetSignalStop" & Chr(10) & "Error code : " & CStr(dwErr), MsgBoxStyle.OKOnly + MsgBoxStyle.Critical)
            Exit Sub
        End If
        'Counter clear
        dwErr = McsdSetCounter(ghDevice, gwAxis, 0, 0)
        If (dwErr) Then
            MsgBox("McsdSetCounter" & Chr(10) & "Error code : " & CStr(dwErr), MsgBoxStyle.OKOnly + MsgBoxStyle.Critical)
            Exit Sub
        End If
        dwErr = McsdSetCounter(ghDevice, gwAxis, 1, 0)
        If (dwErr) Then
            MsgBox("McsdSetCounter" & Chr(10) & "Error code : " & CStr(dwErr), MsgBoxStyle.OKOnly + MsgBoxStyle.Critical)
            Exit Sub
        End If
        'Speed set
        SpdDataPPS.dAccel = 250
        SpdDataPPS.dHighSpeed = 5000
        SpdDataPPS.dLowSpeed = 100
        SpdDataPPS.dSratio = 0
        dwErr = McsdSetSpeedPPS(ghDevice, gwAxis, SpdDataPPS)
        If (dwErr) Then
            MsgBox("McsdSetSpeedPPS" & Chr(10) & "Error code : " & CStr(dwErr), MsgBoxStyle.OKOnly + MsgBoxStyle.Critical)
            Exit Sub
        End If
    End Sub

    Private Sub Command2_Click(ByVal eventSender As System.Object, ByVal eventArgs As System.EventArgs) Handles Command2.Click
        Dim dwErr As Integer
        'Board Close
        dwErr = McsdClose(ghDevice)
        If (dwErr) Then
            MsgBox("McsdClose" & Chr(10) & "Error code : " & CStr(dwErr), MsgBoxStyle.OKOnly + MsgBoxStyle.Critical)
            Exit Sub
        End If
        MsgBox("Board Close (ID 0)", MsgBoxStyle.OKOnly + MsgBoxStyle.Information)
    End Sub

    Private Sub Command3_Click(ByVal eventSender As System.Object, ByVal eventArgs As System.EventArgs) Handles Command3.Click
        Dim dwErr As Integer
        'Drive Start
        '  + Index pulse drive
        dwErr = McsdDriveStart(ghDevice, gwAxis, 0, 10000)
        If (dwErr) Then
            MsgBox("McsdDriveStart" & Chr(10) & "Error code : " & CStr(dwErr), MsgBoxStyle.OKOnly + MsgBoxStyle.Critical)
            Exit Sub
        End If
        '
        MsgBox("制御軸 " & CStr(gwAxis) & " ＋方向に 10000 パルス出力しています", MsgBoxStyle.OKOnly + MsgBoxStyle.Information)
    End Sub

    Private Sub Command4_Click(ByVal eventSender As System.Object, ByVal eventArgs As System.EventArgs) Handles Command4.Click
        Dim Status As MCSDSTATUS
        Dim dwCount As Integer
        Dim dwErr As Integer
        'Status Read
        dwErr = McsdGetStatus(ghDevice, gwAxis, Status)
        If (dwErr) Then
            MsgBox("McsdGetStatus" & Chr(10) & "Error code : " & CStr(dwErr), MsgBoxStyle.OKOnly + MsgBoxStyle.Critical)
            Exit Sub
        End If
        'Internal counter read
        dwErr = McsdGetCounter(ghDevice, gwAxis, 0, dwCount)
        If (dwErr) Then
            MsgBox("McsdGetStatus" & Chr(10) & "Error code : " & CStr(dwErr), MsgBoxStyle.OKOnly + MsgBoxStyle.Critical)
            Exit Sub
        End If
        ' カウンタは 28 ビットなので符号拡張します。
        If ((dwCount And &H8000000) = &H8000000) Then
            dwCount += &HF0000000
        End If
        MsgBox("Board status" & Chr(10) & "Axis : " & CStr(gwAxis) & Chr(10) & "Internal counter : " & CStr(dwCount) & Chr(10) & "Drive status : " & Hex(Status.bDrive) & "h" & Chr(10) & "End status : " & Hex(Status.bEnd) & "h" & Chr(10) & "Mechanical signal : " & Hex(Status.bMechanical) & "h" & Chr(10) & "Universal signal : " & Hex(Status.bUniversal) & "h", MsgBoxStyle.OKOnly + MsgBoxStyle.Information)
    End Sub
End Class