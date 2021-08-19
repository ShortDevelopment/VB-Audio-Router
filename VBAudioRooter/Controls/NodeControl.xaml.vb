Imports VBAudioRooter.Utils
Imports Windows.UI
Imports Windows.UI.Xaml.Markup

Namespace Controls

    <ContentProperty(Name:="NodeContent")>
    Public Class NodeControl
        Inherits UserControl
        Implements INotifyPropertyChanged

        Public Shared Property NodeContentProperty As DependencyProperty = DependencyProperty.Register("NodeContent", GetType(Object), GetType(NodeControl), Nothing)
        Public Property NodeContent As Object
            Get
                Return GetValue(NodeContentProperty)
            End Get
            Set(value As Object)
                SetValue(NodeContentProperty, value)
                OnPropertyChanged()
            End Set
        End Property

        Public Shared Property TitleProperty As DependencyProperty = DependencyProperty.Register("Title", GetType(String), GetType(NodeControl), Nothing)
        Public Property Title As String
            Get
                Return GetValue(TitleProperty)
            End Get
            Set(value As String)
                SetValue(TitleProperty, value)
                OnPropertyChanged()
            End Set
        End Property

        Public Shared Property TitleBrushProperty As DependencyProperty = DependencyProperty.Register("TitleBrushProperty", GetType(Brush), GetType(NodeControl), Nothing)
        Public Property TitleBrush As Brush
            Get
                Return GetValue(TitleBrushProperty)
            End Get
            Set(value As Brush)
                SetValue(TitleBrushProperty, value)
                OnPropertyChanged()
            End Set
        End Property

#Region "INotifyPropertyChanged"
        Private Sub OnPropertyChanged(<CallerMemberName> Optional propertyName As String = Nothing)
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(propertyName))
        End Sub

        Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged
#End Region

        Public Sub New()
            InitializeComponent()
            Me.DataContext = Me
            Me.TitleBrush = New SolidColorBrush(ColorTranslator.FromHex("#9E343E"))
        End Sub

        Private Sub UserControl_ManipulationDelta(sender As Object, e As ManipulationDeltaRoutedEventArgs)
            positionTransform.TranslateX += e.Delta.Translation.X
            positionTransform.TranslateY += e.Delta.Translation.Y
        End Sub
    End Class

End Namespace
