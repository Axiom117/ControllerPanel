Module Module1
    '
    Public Const WM_USER As Integer = &H400
    Public Const G_FALSE As Integer = &HFFFFFFFF
    Public Const MCUSB4sd_AXIS1 As Short = 0
    Public Const MCUSB4sd_AXIS2 As Short = 1
    Public Const MCUSB4sd_AXIS3 As Short = 2
    Public Const MCUSB4sd_AXIS4 As Short = 3
    '
    Public ghW As Integer
    Public ghDevice As Integer
    Public gwAxis As Short
End Module