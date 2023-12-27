using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Windows.Foundation;

namespace VBAudioRouter.Utils;

public class MediaTransportControlsWrapper(MediaTransportControls control)
{

    private bool _IsMuted = false;
    public bool IsMuted
    {
        get
        {
            return _IsMuted;
        }
        set
        {
        }
    }

    public event TypedEventHandler<MediaTransportControlsWrapper, bool> MutedChanged;

    private void AudioMuteButton_Click(object sender, RoutedEventArgs e)
    {
        IsMuted = !IsMuted;
    }

    public double Volume
    {
        get
        {
            return 0;
        }
        set
        { }
    }

    public event TypedEventHandler<MediaTransportControlsWrapper, double> VolumeChanged;

    private void VolumeSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
    {

    }

    private bool _IsPlaying;
    public bool IsPlaying
    {
        get
        {
            return _IsPlaying;
        }
        set
        {
        }
    }

    public event TypedEventHandler<MediaTransportControlsWrapper, bool> PlayStateChanged;

    private void PlayPauseButton_Click(object sender, RoutedEventArgs e)
    {
        IsPlaying = !IsPlaying;
    }

    private TimeSpan _Position;
    public TimeSpan Position
    {
        get
        {
            return _Position;
        }
        set
        {
        }
    }

    public event TypedEventHandler<MediaTransportControlsWrapper, TimeSpan> PositionChanged;

    private void ProgressSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
    {
    }

    private TimeSpan _Duration;
    public TimeSpan Duration
    {
        get
        {
            return _Duration;
        }
        set
        {
        }
    }
}
