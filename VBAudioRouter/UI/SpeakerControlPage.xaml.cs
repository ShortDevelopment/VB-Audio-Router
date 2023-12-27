using System.Collections.ObjectModel;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Navigation;
using VBAudioRouter.Controls;
using Windows.Win32.Foundation;
using Windows.Win32.Media.Audio;
using Windows.Win32.Media.Audio.Endpoints;

namespace VBAudioRouter.UI;

internal sealed partial class SpeakerControlPage : Page, IAudioEndpointVolumeCallback, IAudioSessionNotification
{
    public SpeakerControlPage()
    {
        InitializeComponent();
    }

    public IAudioEndpointVolume VolumeManager { get; private set; }
    public IAudioMeterInformation MeterInformation { get; private set; }
    public IAudioSessionManager2 AudioSessionManager { get; private set; }

    protected override async void OnNavigatedTo(NavigationEventArgs e)
    {
        string deviceId = (string)e.Parameter;
        VolumeManager = await AudioInterfaceActivator.ActivateAudioInterfaceAsync<IAudioEndpointVolume>(deviceId);
        MeterInformation = (IAudioMeterInformation)VolumeManager;

        VolumeManager.RegisterControlChangeNotify(this);
        unsafe
        {
            OnNotify((AUDIO_VOLUME_NOTIFICATION_DATA*)0);
        }

        System.Timers.Timer timer = new()
        {
            Interval = 30
        };
        timer.Elapsed += (s, e) =>
        {
            var unused = DispatcherQueue.TryEnqueue(() =>
            {
                MeterInformation.GetMeteringChannelCount(out var channelCount);
                float[] meters = new float[channelCount];
                MeterInformation.GetChannelsPeakValues((uint)meters.Length, meters);
                LeftMeter.ScaleY = meters[0];
                if (meters.Length > 1)
                    RightMeter.ScaleY = meters[1];
            });
        };
        timer.Enabled = true;

        AudioSessionManager = (IAudioSessionManager2)await AudioInterfaceActivator.ActivateAudioInterfaceAsync<IAudioSessionManager>(deviceId);
        AudioSessionManager.RegisterSessionNotification(this);
        var sessionEnumerator = AudioSessionManager.GetSessionEnumerator();
        sessionEnumerator.GetCount(out var sessionCount);
        for (var sessionIndex = 0; sessionIndex < sessionCount; sessionIndex++)
        {
            sessionEnumerator.GetSession(sessionIndex, out var session);
            AddAudioSession(session);
        }
    }

    private void AudioControlPage_Unloaded(object sender, RoutedEventArgs e)
    {
        VolumeManager?.UnregisterControlChangeNotify(this);
        AudioSessionManager?.UnregisterSessionNotification(this);
    }

    public readonly ObservableCollection<AudioSessionControl> AudioSessions = new();
    private void AddAudioSession(IAudioSessionControl session)
    {
        AudioSessions.Add(new AudioSessionControl(this, (NAudio.CoreAudioApi.Interfaces.IAudioSessionControl)session));
    }

    private double oldValue = -1;
    private unsafe void Slider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
    {
        if (VolumeManager == null | oldValue == GainSlider.Value)
            return;
        oldValue = GainSlider.Value;
        VolumeManager.SetMasterVolumeLevelScalar(Convert.ToSingle(GainSlider.Value / 100.0), (Guid*)0);
    }

    private bool isMuted = false;
    private unsafe void MuteToggleButton_Click(object sender, RoutedEventArgs e)
    {
        if (VolumeManager == null)
            return;

        if (sender != null)
        {
            isMuted = !isMuted;
            VolumeManager.SetMute(isMuted, (Guid*)0);
        }

        if (isMuted)
            MuteButton.Icon = new SymbolIcon(Symbol.Mute);
        else
            MuteButton.Icon = new SymbolIcon(Symbol.Volume);
    }

    public unsafe void OnNotify(AUDIO_VOLUME_NOTIFICATION_DATA* notifyData)
    {
        DispatcherQueue.TryEnqueue(() =>
        {
            VolumeManager.GetMasterVolumeLevelScalar(out var volumeScalar);
            GainSlider.Value = volumeScalar * 100.0;

            BOOL pMuted;
            VolumeManager.GetMute(&pMuted);
            isMuted = pMuted;

            MuteToggleButton_Click(null, null);
        });
    }

    public void OnSessionCreated(IAudioSessionControl newSession)
    {
        AddAudioSession(newSession);
    }
}
