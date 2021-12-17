Imports System.Runtime.InteropServices

Namespace Interop
    ''' <summary>
    ''' <see href="https://gist.github.com/wbokkers/74e05ccc1ee2371ec55c4a7daf551a26"/>
    ''' </summary>
    Public Class AudioInterfaceActivator
        Public Shared Async Function ActivateAudioInterfaceAsync(Of T)(deviceInterfacePath As String) As Task(Of T)
            Dim activationHandler As New ActivateAudioInterfaceCompletionHandler(Of T)()
            Dim ignore As IActivateAudioInterfaceAsyncOperation
            ActivateAudioInterfaceAsync(deviceInterfacePath, GetType(T).GUID, IntPtr.Zero, activationHandler, ignore)
            Return Await activationHandler
        End Function


        <DllImport("Mmdevapi.dll", ExactSpelling:=True, PreserveSig:=False)>
        Private Shared Function ActivateAudioInterfaceAsync(
            <[In], MarshalAs(UnmanagedType.LPWStr)> ByVal deviceInterfacePath As String,
            <[In], MarshalAs(UnmanagedType.LPStruct)> ByVal riid As Guid,
            <[In]> ByVal activationParams As IntPtr,
            <[In]> ByVal completionHandler As IActivateAudioInterfaceCompletionHandler, <Out> ByRef activationOperation As IActivateAudioInterfaceAsyncOperation) As <MarshalAs(UnmanagedType.[Error])> UInteger : End Function

        <ComImport>
        <Guid("72A22D78-CDE4-431D-B8CC-843A71199B6D")>
        <InterfaceType(ComInterfaceType.InterfaceIsIUnknown)>
        Private Interface IActivateAudioInterfaceAsyncOperation
            Sub GetActivateResult(<Out, MarshalAs(UnmanagedType.[Error])> ByRef activateResult As Integer, <Out, MarshalAs(UnmanagedType.IUnknown)> ByRef activatedInterface As Object)
        End Interface

        Private Class ActivateAudioInterfaceCompletionHandler(Of T)
            Implements IActivateAudioInterfaceCompletionHandler

            Dim promise As New TaskCompletionSource(Of T)

            Public Sub ActivateCompleted(activateOperation As IActivateAudioInterfaceAsyncOperation) Implements IActivateAudioInterfaceCompletionHandler.ActivateCompleted
                Dim activateResult As Integer
                Dim activatedInterface As Object
                activateOperation.GetActivateResult(activateResult, activatedInterface)
                If Not activateResult = 0 Then promise.SetException(New Win32Exception(activateResult))
                promise.SetResult(DirectCast(activatedInterface, T))
            End Sub

            Public Function GetAwaiter() As TaskAwaiter(Of T)
                Return promise.Task.GetAwaiter()
            End Function
        End Class

        <ComImport>
        <Guid("41D949AB-9862-444A-80F6-C261334DA5EB")>
        <InterfaceType(ComInterfaceType.InterfaceIsIUnknown)>
        Private Interface IActivateAudioInterfaceCompletionHandler
            Sub ActivateCompleted(ByVal activateOperation As IActivateAudioInterfaceAsyncOperation)
        End Interface

    End Class

End Namespace