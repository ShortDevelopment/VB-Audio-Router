
Imports Windows.Devices.Enumeration
Imports Windows.Media.Devices

Public NotInheritable Class AudioControlPage
    Inherits Page

    Private Property AudioRenderDevices As DeviceInformationCollection
    Private Async Sub OutputDeviceSelectDialog_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
        If DesignMode.DesignModeEnabled Or DesignMode.DesignMode2Enabled Then Exit Sub
        AudioRenderDevices = Await DeviceInformation.FindAllAsync(MediaDevice.GetAudioRenderSelector())
        OutputDevicesComboBox.ItemsSource = AudioRenderDevices.Select(Function(device) device.Name).ToList()

        For i As Integer = 0 To AudioRenderDevices.Count - 1
            If AudioRenderDevices(i).IsDefault Then
                OutputDevicesComboBox.SelectedIndex = i
                Exit For
            End If
        Next
    End Sub

    Private Sub OutputDevicesComboBox_SelectionChanged(sender As Object, e As SelectionChangedEventArgs)
        ContentFrame.Navigate(GetType(SpeakerControlPage), AudioRenderDevices(OutputDevicesComboBox.SelectedIndex).Id)
    End Sub
End Class
