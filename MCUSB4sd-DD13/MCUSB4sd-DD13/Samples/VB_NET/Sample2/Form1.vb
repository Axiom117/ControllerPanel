Option Strict Off
Option Explicit On
Friend Class Form1
	Inherits System.Windows.Forms.Form
#Region "Windows �t�H�[�� �f�U�C�i�ɂ���Đ������ꂽ�R�[�h"
	Public Sub New()
		MyBase.New()
		If m_vb6FormDefInstance Is Nothing Then
			If m_InitializingDefInstance Then
				m_vb6FormDefInstance = Me
			Else
				Try 
					'�X�^�[�g�A�b�v �t�H�[���ɂ��ẮA�ŏ��ɍ쐬���ꂽ�C���X�^���X������C���X�^���X�ɂȂ�܂��B
					If System.Reflection.Assembly.GetExecutingAssembly.EntryPoint.DeclaringType Is Me.GetType Then
						m_vb6FormDefInstance = Me
					End If
				Catch
				End Try
			End If
		End If
		'���̌Ăяo���́AWindows �t�H�[�� �f�U�C�i�ŕK�v�ł��B
		InitializeComponent()
	End Sub
	'Form �́A�R���|�[�l���g�ꗗ�Ɍ㏈�������s���邽�߂� dispose ���I�[�o�[���C�h���܂��B
	Protected Overloads Overrides Sub Dispose(ByVal Disposing As Boolean)
		If Disposing Then
			If Not components Is Nothing Then
				components.Dispose()
			End If
		End If
		MyBase.Dispose(Disposing)
	End Sub
	'Windows �t�H�[�� �f�U�C�i�ŕK�v�ł��B
	Private components As System.ComponentModel.IContainer
	Public ToolTip1 As System.Windows.Forms.ToolTip
    Public WithEvents Command5 As System.Windows.Forms.Button
	Public WithEvents Command4 As System.Windows.Forms.Button
	Public WithEvents Command3 As System.Windows.Forms.Button
	Public WithEvents Command2 As System.Windows.Forms.Button
	Public WithEvents Command1 As System.Windows.Forms.Button
	'���� : �ȉ��̃v���V�[�W���� Windows �t�H�[�� �f�U�C�i�ŕK�v�ł��B
	'Windows �t�H�[�� �f�U�C�i���g���ĕύX�ł��܂��B
	'�R�[�h �G�f�B�^���g���ďC�����Ȃ��ł��������B
	<System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container
        Me.ToolTip1 = New System.Windows.Forms.ToolTip(Me.components)
        Me.Command5 = New System.Windows.Forms.Button
        Me.Command4 = New System.Windows.Forms.Button
        Me.Command3 = New System.Windows.Forms.Button
        Me.Command2 = New System.Windows.Forms.Button
        Me.Command1 = New System.Windows.Forms.Button
        Me.SuspendLayout()
        '
        'Command5
        '
        Me.Command5.BackColor = System.Drawing.SystemColors.Control
        Me.Command5.Cursor = System.Windows.Forms.Cursors.Default
        Me.Command5.ForeColor = System.Drawing.SystemColors.ControlText
        Me.Command5.Location = New System.Drawing.Point(16, 184)
        Me.Command5.Name = "Command5"
        Me.Command5.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.Command5.Size = New System.Drawing.Size(105, 25)
        Me.Command5.TabIndex = 4
        Me.Command5.Text = "Command5"
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
        Me.ClientSize = New System.Drawing.Size(137, 238)
        Me.Controls.Add(Me.Command5)
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
#Region "�A�b�v�O���[�h �E�B�U�[�h�̃T�|�[�g �R�[�h"
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
		Command4.Text = "Org return"
        Command5.Text = "Status read"
    End Sub

    Private Sub Command1_Click(ByVal eventSender As System.Object, ByVal eventArgs As System.EventArgs) Handles Command1.Click
        Dim dwErr As Integer
        Dim Orgret(4) As MCSDORGRET
        Dim SpdData(0) As MCSDSPDDATA
        SpdData(0).Initialize()

        'Boad Open
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
        '���N�����x	200PPS
        '�ō����x 	5000PPS
        '���������� 	300mSec
        '�r��������	100%�iS���������j
        SpdData(0).dwMode = 2
        SpdData(0).dwRange = 500
        SpdData(0).dwHighSpeed = 2500
        SpdData(0).dwLowSpeed = 100
        SpdData(0).dwRate(0) = 229
        SpdData(0).dwRate(1) = 8191
        SpdData(0).dwRate(2) = 8191
        SpdData(0).dwRateChgPnt(0) = 8191
        SpdData(0).dwRateChgPnt(1) = 8191
        SpdData(0).dwScw(0) = 1500
        SpdData(0).dwScw(1) = 4095
        SpdData(0).dwRearPulse = 0
        dwErr = McsdSetSpeed(ghDevice, gwAxis, SpdData(0))
        If (dwErr) Then
            MsgBox("McsdSetSpeed" & Chr(10) & "Error code : " & CStr(dwErr), MsgBoxStyle.OKOnly + MsgBoxStyle.Critical)
            Exit Sub
        End If
        '���[�U�[��`�^���_���o�h���C�u�̐ݒ���s���܂��B
        '���_�M��(ORG)�́|��������G�b�W�����o���܂��B
        '
        Orgret(0).dwTable = 0
        Orgret(0).dwAxis = MCUSB4sd_AXIS1
        Orgret(0).dwAction = 1      ' �����x���Ȃ玟�̃e�[�u�����s
        Orgret(0).dwDir = 0         ' -�����Ƀh���C�u
        Orgret(0).dwSPD = 0         ' HI Speed
        Orgret(0).dwEdge = 4        ' ORG�����o
        Orgret(0).dwWait = 0
        '
        Orgret(1).dwTable = 1
        Orgret(1).dwAxis = MCUSB4sd_AXIS1
        Orgret(1).dwAction = 1      ' �����x����ORG���Ȃ玟�̃e�[�u�����s
        Orgret(1).dwDir = 1         ' +�����Ƀh���C�u
        Orgret(1).dwSPD = 0         ' HI Speed
        Orgret(1).dwEdge = 12       ' ORG�����o
        Orgret(1).dwWait = 0
        '
        Orgret(2).dwTable = 2
        Orgret(2).dwAxis = MCUSB4sd_AXIS1
        Orgret(2).dwAction = 1      ' �����x����ORG���Ȃ玟�̃e�[�u�����s
        Orgret(2).dwDir = 0         ' -�����Ƀh���C�u
        Orgret(2).dwSPD = 1         ' LO Speed
        Orgret(2).dwEdge = 4        ' ORG�����o
        Orgret(2).dwWait = 0
        '
        Orgret(3).dwTable = 3
        Orgret(3).dwAxis = MCUSB4sd_AXIS1
        Orgret(3).dwAction = 1      ' �����x����ORG���Ȃ玟�̃e�[�u�����s
        Orgret(3).dwDir = 1         ' +�����Ƀh���C�u
        Orgret(3).dwSPD = 1         ' LO Speed
        Orgret(3).dwEdge = 12       ' ORG�����o
        Orgret(3).dwWait = 0
        '
        Orgret(4).dwTable = 4
        Orgret(4).dwAction = 3      ' �e�[�u���ݒ�I��
        '
        For I As Integer = 0 To 4
            dwErr = McsdSetOrgReturn(ghDevice, Orgret(I))
            If (dwErr) Then
                MsgBox("McsdSetOrgReturn" & Chr(10) & "Error code : " & CStr(dwErr), MsgBoxStyle.OKOnly + MsgBoxStyle.Critical)
                Exit Sub
            End If
        Next I

        '
        MsgBox("��P���̐ݒ肪�������܂���", MsgBoxStyle.OKOnly + MsgBoxStyle.Information)

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
        dwErr = McsdDriveStart(ghDevice, gwAxis, 0, 15000)
        If (dwErr) Then
            MsgBox("McsdDriveStart" & Chr(10) & "Error code : " & CStr(dwErr), MsgBoxStyle.OKOnly + MsgBoxStyle.Critical)
            Exit Sub
        End If
        '
        MsgBox("���䎲 " & CStr(gwAxis) & " �{������ 15000 �p���X�o�͂��Ă��܂�", MsgBoxStyle.OKOnly + MsgBoxStyle.Information)
    End Sub

    Private Sub Command4_Click(ByVal eventSender As System.Object, ByVal eventArgs As System.EventArgs) Handles Command4.Click
        Dim dwErr As Integer
        Dim result As Integer
        'OrgReturn Drive Start
        dwErr = McsdOrgReturn(ghDevice, gwAxis, 0)
        If (dwErr) Then
            MsgBox("McsdOrgReturn" & Chr(10) & "Error code : " & CStr(dwErr), MsgBoxStyle.OKOnly + MsgBoxStyle.Critical)
            Exit Sub
        End If
        '
        result = MsgBox("���䎲 " & CStr(gwAxis) & " ���_���A�h���C�u�����s���ł�" & Chr(10) & "[�L�����Z��]�ŋ}��~���܂�", MsgBoxStyle.OKCancel + MsgBoxStyle.Information)
        If (result = MsgBoxResult.Cancel) Then
            dwErr = McsdDriveStop(ghDevice, gwAxis, 1)
            If (dwErr <> 0) Then
                MsgBox("McsdDriveStop" & Chr(10) & "Error code : " & CStr(dwErr), MsgBoxStyle.OKOnly + MsgBoxStyle.Critical)
                Exit Sub
            End If
        End If

    End Sub

    Private Sub Command5_Click(ByVal eventSender As System.Object, ByVal eventArgs As System.EventArgs) Handles Command5.Click
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
        ' �J�E���^�� 28 �r�b�g�Ȃ̂ŕ����g�����܂��B
        If ((dwCount And &H8000000) = &H8000000) Then
            dwCount += &HF0000000
        End If
        MsgBox("Board status" & Chr(10) & "Axis : " & CStr(gwAxis) & Chr(10) & "Internal counter : " & CStr(dwCount) & Chr(10) & "Drive status : " & Hex(Status.bDrive) & "h" & Chr(10) & "End status : " & Hex(Status.bEnd) & "h" & Chr(10) & "Mechanical signal : " & Hex(Status.bMechanical) & "h" & Chr(10) & "Universal signal : " & Hex(Status.bUniversal) & "h", MsgBoxStyle.OKOnly + MsgBoxStyle.Information)
    End Sub

End Class