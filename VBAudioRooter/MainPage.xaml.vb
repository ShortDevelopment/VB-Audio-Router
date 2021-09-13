﻿
Imports VBAudioRouter.AudioGraphControl
Imports VBAudioRouter.Controls
Imports Windows.ApplicationModel.ExtendedExecution.Foreground
Imports Windows.Media.Audio
Imports Windows.System
Imports Windows.UI

Public NotInheritable Class MainPage
    Inherits Page

    Private Async Sub MainPage_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
        Await InitAudioGraphAsync(CreateGraphSettings())
        Await EnableBackgroundAudioAsync()

        Await DefaultOutputNode.Initialize(CurrentAudioGraph)
    End Sub

#Region "Audio Graph"
    Public ReadOnly Property CurrentAudioGraph As AudioGraph = Nothing

    Private Function CreateGraphSettings() As AudioGraphSettings
        Dim settings As New AudioGraphSettings(Windows.Media.Render.AudioRenderCategory.Media)
        settings.QuantumSizeSelectionMode = QuantumSizeSelectionMode.LowestLatency
        'settings.EncodingProperties = New AudioEncodingProperties()
        'settings.EncodingProperties.Subtype = "Float"
        'settings.EncodingProperties.SampleRate = 48000
        'settings.EncodingProperties.ChannelCount = 2
        'settings.EncodingProperties.BitsPerSample = 32
        'settings.EncodingProperties.Bitrate = 3072000
        Return settings
    End Function

    Private Async Function InitAudioGraphAsync(settings As AudioGraphSettings) As Task
        Dim result = Await AudioGraph.CreateAsync(settings)
        If Not result.Status = AudioGraphCreationStatus.Success Then Throw result.ExtendedError
        _CurrentAudioGraph = result.Graph
    End Function
#End Region

#Region "Background Audio"
    Public ReadOnly Property BackgroundAudioSession As ExtendedExecutionForegroundSession

    Private Async Function EnableBackgroundAudioAsync() As Task
        _BackgroundAudioSession = New ExtendedExecutionForegroundSession()
        BackgroundAudioSession.Reason = ExtendedExecutionForegroundReason.BackgroundAudio
        BackgroundAudioSession.Description = "Play Background audio"
        Dim result = Await BackgroundAudioSession.RequestExtensionAsync()
        If result = ExtendedExecutionForegroundResult.Denied Then
            ShowWarning()
        End If
    End Function

#Region "Warning"
    Private Sub BackgroundAudioPermissionsWarning_Closed(sender As Microsoft.UI.Xaml.Controls.InfoBar, args As Microsoft.UI.Xaml.Controls.InfoBarClosedEventArgs)
        BackgroundAudioPermissionsWarning.Visibility = Visibility.Collapsed
    End Sub
    Private Sub ShowWarning()
        BackgroundAudioPermissionsWarning.Visibility = Visibility.Visible
        BackgroundAudioPermissionsWarning.IsOpen = True
    End Sub
#End Region
#End Region

#Region "GraphState"
    Public ReadOnly Property GraphState As GraphState = GraphState.Stopped
    Private Sub NotifyAllState()
        ' Notify Node (UI)
        For Each child In NodeContainer.Children
            If GetType(NodeControl) = child.GetType() Then
                Dim control As NodeControl = DirectCast(child, NodeControl)
                If control.NodeContent IsNot Nothing AndAlso GetType(IAudioNodeControl).IsAssignableFrom(control.NodeContent.GetType()) Then
                    Dim node As IAudioNodeControl = DirectCast(control.NodeContent, IAudioNodeControl)
                    node.OnStateChanged(GraphState.Started)
                End If
            End If
        Next
    End Sub
#End Region

    Private Sub PlayButton_Click(sender As Object, e As RoutedEventArgs) Handles PlayButton.Click
        CurrentAudioGraph.Start()

        Me._GraphState = GraphState.Started
        NotifyAllState()

        ' UI
        PlayButton.IsEnabled = False
        StopButton.IsEnabled = True
    End Sub

    Private Sub StopButton_Click(sender As Object, e As RoutedEventArgs) Handles StopButton.Click
        CurrentAudioGraph.Stop()

        Me._GraphState = GraphState.Stopped
        NotifyAllState()

        ' UI
        PlayButton.IsEnabled = True
        StopButton.IsEnabled = False
    End Sub

#Region "Context Menu"
    Private Async Sub MenuFlyoutItem_Click(sender As Object, e As RoutedEventArgs)
        Try
            Dim tag As String = DirectCast(DirectCast(sender, MenuFlyoutItem).Tag, String)
            If Not String.IsNullOrEmpty(tag) Then
                Dim contentEle = DirectCast(Activator.CreateInstance(Me.GetType().Assembly.GetType($"VBAudioRouter.Controls.{tag}")), IAudioNodeControl)

#Region "UI"
                Dim nodeContainer As New NodeControl()
                nodeContainer.HorizontalAlignment = HorizontalAlignment.Left
                nodeContainer.VerticalAlignment = VerticalAlignment.Top
                nodeContainer.Title = String.Join("", contentEle.GetType().Name.Replace("NodeControl", "").ToCharArray().Select(Function(x) If(Char.IsUpper(x), " " + x, x.ToString())))
                If contentEle.NodeType = NodeTypeEnum.Effect Then
                    nodeContainer.TitleBrush = New SolidColorBrush(DirectCast(Application.Current.Resources("NodeTitleBarColor2"), Color))
                End If
#End Region

                contentEle.Canvas = ConnectionCanvas

                ' Assign content to container ("window")
                nodeContainer.NodeContent = contentEle
                Me.NodeContainer.Children.Add(nodeContainer)

                ' Wait for node to be initialized by uwp
                Await Dispatcher.RunIdleAsync(Async Sub()
                                                  ' Initialize
                                                  Await DirectCast(contentEle, IAudioNodeControl).Initialize(CurrentAudioGraph)
                                                  ' Update State
                                                  DirectCast(contentEle, IAudioNodeControl).OnStateChanged(GraphState)
                                              End Sub)
            End If
        Catch ex As Exception
            Dim dialog As New Dialogs.ErrorDialog(ex)
            dialog.Title = "Failed to add node"
            Call dialog.ShowAsync()
        End Try
    End Sub
#End Region

    Private Async Sub HelpButton_Click(sender As Object, e As RoutedEventArgs)
        Await Launcher.LaunchUriAsync(New Uri("https://github.com/ShortDevelopment/VB-Audio-Router/wiki"))
    End Sub

End Class
