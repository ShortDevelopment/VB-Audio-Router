Namespace Utils

    Public Class SliderTimeSpanValueConverter
        Implements IValueConverter

        Public Function Convert(value As Object, targetType As Type, parameter As Object, language As String) As Object Implements IValueConverter.Convert
            If targetType = GetType(String) Then
                Return TimeSpan.FromMilliseconds(CType(value, Double)).ToString("mm\:ss")
            ElseIf value.GetType() = GetType(TimeSpan) Then
                Return DirectCast(value, TimeSpan).TotalMilliseconds
            Else
                Throw New NotImplementedException()
            End If
        End Function

        Public Function ConvertBack(value As Object, targetType As Type, parameter As Object, language As String) As Object Implements IValueConverter.ConvertBack
            Return Convert(value, targetType, parameter, language)
        End Function
    End Class

End Namespace