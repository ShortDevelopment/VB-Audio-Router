Imports Windows.Devices.Enumeration
Imports Windows.Media.Devices

Namespace Dialogs

    Public NotInheritable Class OutputDeviceSelectDialog
        Inherits ContentDialog

        Private Property AudioRenderDevices As DeviceInformationCollection
        Private Async Sub OutputDeviceSelectDialog_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
            If DesignMode.DesignModeEnabled Or DesignMode.DesignMode2Enabled Then Exit Sub
            AudioRenderDevices = Await DeviceInformation.FindAllAsync(MediaDevice.GetAudioRenderSelector())
            OutputDevicesComboBox.ItemsSource = AudioRenderDevices.Select(Function(device) device.Name).ToList()

            For i As Integer = 0 To AudioRenderDevices.Count - 1
                If AudioRenderDevices.Item(i).IsDefault Then
                    OutputDevicesComboBox.SelectedIndex = i
                    Exit For
                End If
            Next
        End Sub

        Public Property SelectedRenderDevice As DeviceInformation

        Private Sub ContentDialog_PrimaryButtonClick(sender As ContentDialog, args As ContentDialogButtonClickEventArgs)
            If OutputDevicesComboBox.SelectedIndex < 0 Then Exit Sub
            SelectedRenderDevice = AudioRenderDevices(OutputDevicesComboBox.SelectedIndex)
        End Sub


    End Class


End Namespace