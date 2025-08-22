VERSION 5.00
Begin VB.Form Form1 
   Caption         =   "Form1"
   ClientHeight    =   2745
   ClientLeft      =   60
   ClientTop       =   345
   ClientWidth     =   2055
   LinkTopic       =   "Form1"
   ScaleHeight     =   2745
   ScaleWidth      =   2055
   StartUpPosition =   2  '‰æ–Ê‚Ì’†‰›
   Begin VB.CommandButton Command4 
      Caption         =   "Command4"
      Height          =   375
      Left            =   240
      TabIndex        =   3
      Top             =   2160
      Width           =   1575
   End
   Begin VB.CommandButton Command3 
      Caption         =   "Command3"
      Height          =   375
      Left            =   240
      TabIndex        =   2
      Top             =   1560
      Width           =   1575
   End
   Begin VB.CommandButton Command2 
      Caption         =   "Command2"
      Height          =   375
      Left            =   240
      TabIndex        =   1
      Top             =   960
      Width           =   1575
   End
   Begin VB.CommandButton Command1 
      Caption         =   "Command1"
      Height          =   375
      Left            =   240
      TabIndex        =   0
      Top             =   360
      Width           =   1575
   End
End
Attribute VB_Name = "Form1"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = False
Attribute VB_PredeclaredId = True
Attribute VB_Exposed = False
'
Dim ghW As Long
Dim ghDevice As Long
Dim gwAxis As Integer
Dim dwErr As Long

Private Sub Form_Load()
    'Axis
    gwAxis = MCUSB4sd_AXIS1
    'Caption
    Command1.Caption = "Board open"
    Command2.Caption = "Board close"
    Command3.Caption = "Drive start"
    Command4.Caption = "Status read"
End Sub

Private Sub Command1_Click()
    Dim SpdDataPPS As MCSDSPDDATAPPS
    'Board Open
    ghDevice = McsdOpen("MCUSB4sd", 0)
    If (ghDevice = G_FALSE) Then
        MsgBox "McsdOpen" + Chr(10) + "BOARD : MCUSB4sd" + Chr(10) + _
            "ID : 0" + Chr(10) + "Open Error !!", (vbOKOnly + vbCritical)
        Exit Sub
    End If
    MsgBox "Board Open (ID 0)", (vbOKOnly + vbInformation)
    'Parameter set
    '  Set pulse mode
    '    Pulse mode : 2 pulse
    '    nDIR       : CW  pulse, Active Hi
    '    nPULSE     : CCW pulse, Active Hi
    dwErr = McsdSetPulseMode(ghDevice, gwAxis, 4)
    If (dwErr) Then
        MsgBox "McsdSetPulseMode" + Chr(10) + _
            "Error code : " + CStr(dwErr), (vbOKOnly + vbCritical)
        Exit Sub
    End If
    'Parameter set
    '  Signal active level set
    '    n+LMT active level        Low
    '    n-LMT active level        Low
    '    nAlarm active level       Low
    '    nInposition active level  Low
    dwErr = McsdSetLimit(ghDevice, gwAxis, &H33)
    If (dwErr) Then
        MsgBox "McsdSetLimit" + Chr(10) + _
            "Error code : " + CStr(dwErr), (vbOKOnly + vbCritical)
        Exit Sub
    End If
    'Parameter set
    '  External counter mode set
    '    Up/down signal count
    dwErr = McsdSetExternalCounterMode(ghDevice, gwAxis, 0)
    If (dwErr) Then
        MsgBox "McsdSetExternalCounterMode" + Chr(10) + _
            "Error code : " + CStr(dwErr), (vbOKOnly + vbCritical)
        Exit Sub
    End If
    'Parameter set
    '  Signal stop set
    '    Alarm stop mode set
    '    Inposition wait mode reset
    '    Limit input stop enable (Emergency stop)
    dwErr = McsdSetSignalStop(ghDevice, gwAxis, 1, 0, 2)
    If (dwErr) Then
        MsgBox "McsdSetSignalStop" + Chr(10) + _
            "Error code : " + CStr(dwErr), (vbOKOnly + vbCritical)
        Exit Sub
    End If
    'Counter clear
    dwErr = McsdSetCounter(ghDevice, gwAxis, 0, 0)
    If (dwErr) Then
        MsgBox "McsdSetCounter" + Chr(10) + _
            "Error code : " + CStr(dwErr), (vbOKOnly + vbCritical)
        Exit Sub
    End If
    dwErr = McsdSetCounter(ghDevice, gwAxis, 1, 0)
    If (dwErr) Then
        MsgBox "McsdSetCounter" + Chr(10) + _
            "Error code : " + CStr(dwErr), (vbOKOnly + vbCritical)
        Exit Sub
    End If
    'Speed set
    SpdDataPPS.dAccel = 250
    SpdDataPPS.dHighSpeed = 5000
    SpdDataPPS.dLowSpeed = 100
    SpdDataPPS.dSratio = 0
    dwErr = McsdSetSpeedPPS(ghDevice, gwAxis, SpdDataPPS)
    If (dwErr) Then
        MsgBox "McsdSetSpeedPPS" + Chr(10) + _
            "Error code : " + CStr(dwErr), (vbOKOnly + vbCritical)
        Exit Sub
    End If
End Sub

Private Sub Command2_Click()
    'Board Close
    dwErr = McsdClose(ghDevice)
    If (dwErr) Then
        MsgBox "McsdClose" + Chr(10) + _
            "Error code : " + CStr(dwErr), (vbOKOnly + vbCritical)
        Exit Sub
    End If
    MsgBox "Board Close (ID 0)", (vbOKOnly + vbInformation)
End Sub

Private Sub Command3_Click()
    'Drive Start
    '  + Index pulse drive
    dwErr = McsdDriveStart(ghDevice, gwAxis, 0, 10000)
    If (dwErr) Then
        MsgBox "McsdDriveStart" + Chr(10) + _
            "Error code : " + CStr(dwErr), (vbOKOnly + vbCritical)
        Exit Sub
    End If
End Sub

Private Sub Command4_Click()
    Dim Status As MCSDSTATUS
    Dim dwCount As Long
    'Status Read
    dwErr = McsdGetStatus(ghDevice, gwAxis, Status)
    If (dwErr) Then
        MsgBox "McsdGetStatus" + Chr(10) + _
        "Error code : " + CStr(dwErr), (vbOKOnly + vbCritical)
        Exit Sub
    End If
    'Internal counter read
    dwErr = McsdGetCounter(ghDevice, gwAxis, 0, dwCount)
    If (dwErr) Then
        MsgBox "McsdGetStatus" + Chr(10) + _
        "Error code : " + CStr(dwErr), (vbOKOnly + vbCritical)
        Exit Sub
    End If
    MsgBox "Board status" + Chr(10) + _
        "Axis : " + CStr(gwAxis) + Chr(10) + _
        "Internal counter : " + CStr(dwCount) + Chr(10) + _
        "Drive status : " + Hex(Status.bDrive) + "h" + Chr(10) + _
        "End status : " + Hex(Status.bEnd) + "h" + Chr(10) + _
        "Mechanical signal : " + Hex(Status.bMechanical) + "h" + Chr(10) + _
        "Universal signal : " + Hex(Status.bUniversal) + "h", (vbOKOnly + vbInformation)
End Sub


