using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using NAudio.CoreAudioApi.Interfaces;
using VBAudioRouter.UI;

namespace VBAudioRouter.Controls;

public sealed partial class AudioSessionControl : UserControl, IAudioSessionEvents
{
    public IAudioSessionControl AudioSession
    {
        get;
    }
    public NAudio.CoreAudioApi.AudioSessionControl NAudioSessionControl
    {
        get;
    }
    public NAudio.CoreAudioApi.AudioMeterInformation AudioMeterInformation
    {
        get;
    }
    public NAudio.CoreAudioApi.SimpleAudioVolume SimpleAudioVolume
    {
        get;
    }

    readonly SpeakerControlPage SpeakerControlPageInstance;

    internal AudioSessionControl(SpeakerControlPage parent, IAudioSessionControl audioSession)
    {
        InitializeComponent();

        SpeakerControlPageInstance = parent;
        NAudioSessionControl = new NAudio.CoreAudioApi.AudioSessionControl(audioSession);
        AudioMeterInformation = NAudioSessionControl.AudioMeterInformation;
        SimpleAudioVolume = NAudioSessionControl.SimpleAudioVolume;

        VolumeSlider.Value = SimpleAudioVolume.Volume * 100;
        DisplayNameTextBlock.Text = NAudioSessionControl.DisplayName;

        System.Timers.Timer timer = new()
        {
            Interval = 30
        };
        timer.Elapsed += (s, e) =>
        {
            var unused = DispatcherQueue.TryEnqueue(() =>
            {
                var peakValues = AudioMeterInformation.PeakValues;
                if (peakValues.Count > 0)
                    LeftMeter.ScaleX = peakValues[0];
                if (peakValues.Count > 1)
                    RightMeter.ScaleX = peakValues[1];
            });
        };
        timer.Enabled = true;

        audioSession.RegisterAudioSessionNotification(this);
    }

    private void AudioSessionControl_Unloaded(object sender, RoutedEventArgs e)
    {
        AudioSession?.UnregisterAudioSessionNotification(this);
    }

    private double oldValue = -1;
    private void VolumeSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
    {
        if (SimpleAudioVolume == null | oldValue == VolumeSlider.Value)
            return;
        oldValue = VolumeSlider.Value;
        SimpleAudioVolume.Volume = System.Convert.ToSingle(VolumeSlider.Value / (double)100.0F);
    }

    private bool isMuted = false;
    private void MuteButton_Click(object sender, RoutedEventArgs e)
    {
        if (SimpleAudioVolume == null)
            return;

        if (sender != null)
        {
            isMuted = !isMuted;
            SimpleAudioVolume.Mute = isMuted;
        }

        if (isMuted)
            MuteButton.Icon = new SymbolIcon(Symbol.Mute);
        else
            MuteButton.Icon = new SymbolIcon(Symbol.Volume);
    }

    public int OnDisplayNameChanged(string displayName, ref Guid eventContext)
    {
        DisplayNameTextBlock.Text = NAudioSessionControl.DisplayName;

        return 0;
    }

    public int OnIconPathChanged(string iconPath, ref Guid eventContext)
    {
        return 0;
    }

    public int OnSimpleVolumeChanged(float volume, bool isMuted, ref Guid eventContext)
    {
        DispatcherQueue.TryEnqueue(() =>
        {
            if (oldValue != volume * 100)
            {
                oldValue = volume * 100;
                VolumeSlider.Value = volume * 100;
            }
            this.isMuted = isMuted;
            MuteButton_Click(null, null);
        });

        return 0;
    }

    public int OnChannelVolumeChanged(uint channelCount, IntPtr newVolumes, uint channelIndex, ref Guid eventContext)
    {
        return 0;
    }

    public int OnGroupingParamChanged(ref Guid groupingId, ref Guid eventContext)
    {
        return 0;
    }

    public int OnStateChanged(AudioSessionState state)
    {
        return 0;
    }

    public int OnSessionDisconnected(AudioSessionDisconnectReason disconnectReason)
    {
        DispatcherQueue.TryEnqueue(() => SpeakerControlPageInstance.AudioSessions.Remove(this));

        return 0;
    }
}
