using VBAudioRouter.Capture;
using Windows.Win32;

namespace VBAudioRouter;
internal static class AudioInterfaceActivator
{
    public static ValueTask<T> ActivateAudioInterfaceAsync<T>(string deviceId) where T : class
    {
        ActivateAudioInterfaceCompletionHandler<T> handler = new();
        PInvoke.ActivateAudioInterfaceAsync(deviceId, typeof(T).GUID, activationParams: null, handler, out var resultHandler);
        handler.WaitForCompletion();

        resultHandler.GetActivateResult(out var hres, out var result);
        hres.ThrowOnFailure();

        return ValueTask.FromResult((T)result);
    }
}
