Imports Windows.UI

Namespace Utils
    Module Extensions
        ''' <summary>
        ''' https://stackoverflow.com/a/14353572/15213858
        ''' </summary>
        ''' <returns></returns>
        <Extension()>
        Public Function Map(ByVal value As Double, ByVal fromSource As Double, ByVal toSource As Double, ByVal fromTarget As Double, ByVal toTarget As Double) As Double
            Return (value - fromSource) / (toSource - fromSource) * (toTarget - fromTarget) + fromTarget
        End Function
    End Module

    Public Class ColorTranslator
        ''' <summary>
        ''' http://joeljoseph.net/converting-hex-to-color-in-universal-windows-platform-uwp/
        ''' </summary>
        Public Shared Function FromHex(hex As String) As Color
            hex = hex.Replace("#", String.Empty)
            Dim a As Byte = 255
            Dim r As Byte = CType(Convert.ToUInt32(hex.Substring(0, 2), 16), Byte)
            Dim g As Byte = CType(Convert.ToUInt32(hex.Substring(2, 2), 16), Byte)
            Dim b As Byte = CType(Convert.ToUInt32(hex.Substring(4, 2), 16), Byte)
            Return Color.FromArgb(a, r, g, b)
        End Function
    End Class

End Namespace