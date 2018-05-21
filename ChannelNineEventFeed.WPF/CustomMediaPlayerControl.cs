using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Threading;

namespace ChannelNineEventFeed.WPF
{
    [TemplatePart(Name = "VideoPlayer", Type = typeof(MediaElement))]
    [TemplatePart(Name = "playButton", Type = typeof(Image))]
    [TemplatePart(Name = "pauseButton", Type = typeof(Image))]
    [TemplatePart(Name = "stopButton", Type = typeof(Image))]
    [TemplatePart(Name = "resetSpeedButton", Type = typeof(Image))]
    [TemplatePart(Name = "volumeSlider", Type = typeof(Slider))]
    [TemplatePart(Name = "speedRatioSlider", Type = typeof(Slider))]
    [TemplatePart(Name = "timelineSlider", Type = typeof(CustomSlider))]
    [TemplatePart(Name = "bufferingTextBlock", Type = typeof(TextBlock))]
    [TemplatePart(Name = "openSummaryButton", Type = typeof(Image))]
    [TemplatePart(Name = "textBlockSortByDate", Type = typeof(TextBlock))]
    [TemplatePart(Name = "textBlockSortByTitle", Type = typeof(TextBlock))]
    [TemplatePart(Name = "textBlockSortByCode", Type = typeof(TextBlock))]

    [TemplatePart(Name = "gridVideoListControls", Type = typeof(Grid))]

    [TemplateVisualState(Name = "FullScreen", GroupName = "FullScreenStates")]
    [TemplateVisualState(Name = "NormalScreen", GroupName = "FullScreenStates")]
    [TemplateVisualState(Name = "SummaryOpen", GroupName = "ShowingSummaryStates")]
    [TemplateVisualState(Name = "SummaryClosed", GroupName = "ShowingSummaryStates")]

    public class CustomMediaPlayerControl : Control
    {
        public static readonly DependencyProperty FullScreenProperty =
            DependencyProperty.Register("FullScreen", typeof(bool), typeof(CustomMediaPlayerControl), new PropertyMetadata((bool)false, new PropertyChangedCallback(OnFullScreenChanged)));

        public static readonly DependencyProperty VideoSourceProperty =
            DependencyProperty.Register("VideoSource", typeof(Uri), typeof(CustomMediaPlayerControl), new PropertyMetadata((Uri)null, new PropertyChangedCallback(OnVideoSourceChanged)));

        private static void OnVideoSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CustomMediaPlayerControl owner = (CustomMediaPlayerControl)d;
            if (owner._videoPlayer != null)
            {
                var videoUrl = e.NewValue.ToString();
                videoUrl = videoUrl.Replace("https://", string.Empty);
                var videoUri = new Uri(videoUrl, UriKind.RelativeOrAbsolute);
                owner._videoPlayer.Source = videoUri;
            }
        }

        private static void OnFullScreenChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CustomMediaPlayerControl owner = (CustomMediaPlayerControl)d;
            if ((bool)e.NewValue == true)
            {
                VisualStateManager.GoToState(owner, "FullScreen", true);
            }
            else
            {
                VisualStateManager.GoToState(owner, "NormalScreen", true);
            }
        }

        private MediaElement _videoPlayer = null;
        private Image _playButton = null;
        private Image _pauseButton = null;
        private Image _stopButton = null;
        private Image _resetSpeedButton = null;
        private Slider _volumeSlider = null;
        private Slider _speedRatioSlider = null;
        private CustomSlider _timelineSlider = null;
        private TextBlock _bufferingTextBlock = null;
        private Thumb _timelineSliderThumb = null;
        private bool _isSeekSliderDragging = false;
        private DispatcherTimer _seekSliderSyncTimer = null;
        private Grid _gridVideoListControls = null;
        private Image _openSummaryButton = null;

        public CustomMediaPlayerControl()
        {
            Loaded += new RoutedEventHandler(CustomMediaPlayerControl_Loaded);
        }

        public event EventHandler DownloadRequested;

        public bool FullScreen
        {
            get { return (bool)GetValue(FullScreenProperty); }
            set { SetValue(FullScreenProperty, value); }
        }

        public Uri VideoSource
        {
            get { return (Uri)GetValue(VideoSourceProperty); }
            set { SetValue(VideoSourceProperty, value); }
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _videoPlayer = GetTemplateChild("VideoPlayer") as MediaElement;
            _playButton = GetTemplateChild("playButton") as Image;
            _pauseButton = GetTemplateChild("pauseButton") as Image;
            _stopButton = GetTemplateChild("stopButton") as Image;
            _volumeSlider = GetTemplateChild("volumeSlider") as Slider;
            _speedRatioSlider = GetTemplateChild("speedRatioSlider") as Slider;
            _timelineSlider = GetTemplateChild("timelineSlider") as CustomSlider;
            _bufferingTextBlock = GetTemplateChild("bufferingTextBlock") as TextBlock;
            _resetSpeedButton = GetTemplateChild("resetSpeedButton") as Image;
            _gridVideoListControls = GetTemplateChild("gridVideoListControls") as Grid;
            _openSummaryButton = GetTemplateChild("openSummaryButton") as Image;

            //Init events
            _videoPlayer.BufferingEnded += new RoutedEventHandler(VideoPlayer_BufferingEnded);
            _videoPlayer.BufferingStarted += new RoutedEventHandler(VideoPlayer_BufferingStarted);

            _videoPlayer.Loaded += new RoutedEventHandler(VideoPlayer_Loaded);
            _videoPlayer.MediaOpened += new RoutedEventHandler(VideoPlayer_MediaOpened);
            _videoPlayer.MediaEnded += new RoutedEventHandler(VideoPlayer_MediaEnded);
            _videoPlayer.MediaFailed += new EventHandler<ExceptionRoutedEventArgs>(VideoPlayer_MediaFailed);

            _playButton.MouseLeftButtonDown += new System.Windows.Input.MouseButtonEventHandler(PlayButton_MouseLeftButtonDown);
            _pauseButton.MouseLeftButtonDown += new System.Windows.Input.MouseButtonEventHandler(PauseButton_MouseLeftButtonDown);
            _stopButton.MouseLeftButtonDown += new System.Windows.Input.MouseButtonEventHandler(StopButton_MouseLeftButtonDown);
            _volumeSlider.ValueChanged += new RoutedPropertyChangedEventHandler<double>(VolumeSlider_ValueChanged);
            _speedRatioSlider.ValueChanged += new RoutedPropertyChangedEventHandler<double>(SpeedRatioSlider_ValueChanged);
            _timelineSlider.Loaded += new RoutedEventHandler(TimelineSlider_Loaded);
            _timelineSlider.ValueChanged += new RoutedPropertyChangedEventHandler<double>(TimelineSlider_ValueChanged);
            _resetSpeedButton.MouseLeftButtonDown += new System.Windows.Input.MouseButtonEventHandler(ResetSpeedButton_MouseLeftButtonDown1);
            _openSummaryButton.MouseLeftButtonDown += new System.Windows.Input.MouseButtonEventHandler(OpenSummaryButton_MouseLeftButtonDown);
        }

        private void CustomMediaPlayerControl_Loaded(object sender, RoutedEventArgs e)
        {
        }

        private void OpenSummaryButton_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            //TODO: Download video to network storage folder
            DownloadRequested?.Invoke(this, null);
        }

        private void Vid_VideoClicked(object sender, RoutedEventArgs e)
        {
            //TODO: Set the VideoSource and play the video
            var videoUri = string.Empty;
            VideoSource = new Uri(videoUri);
            _videoPlayer.Play();
            _gridVideoListControls.Visibility = Visibility.Collapsed;
        }

        private void ResetSpeedButton_MouseLeftButtonDown1(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            _speedRatioSlider.Value = 1.0;
        }

        private void TimelineSlider_Loaded(object sender, RoutedEventArgs e)
        {
            var track = _timelineSlider.Template.FindName("PART_Track", _timelineSlider) as Track;
            _timelineSliderThumb = track.Thumb;
            _timelineSliderThumb.DragStarted += new DragStartedEventHandler(TimelineSliderThumb_DragStarted);
            _timelineSliderThumb.DragCompleted += new DragCompletedEventHandler(TimelineSliderThumb_DragCompleted);
        }

        private void TimelineSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (!_isSeekSliderDragging)
            {
                _timelineSlider.ThumbToolTipText = new DateTime(TimeSpan.FromSeconds(_timelineSlider.Value).Ticks).ToString("HH:mm:ss");
            }
        }

        private void TimelineSliderThumb_DragCompleted(object sender, DragCompletedEventArgs e)
        {
            _isSeekSliderDragging = false;
            _videoPlayer.Position = TimeSpan.FromSeconds(_timelineSlider.Value);
        }

        private void TimelineSliderThumb_DragStarted(object sender, DragStartedEventArgs e)
        {
            _isSeekSliderDragging = true;
        }

        private void VideoPlayer_BufferingEnded(object sender, RoutedEventArgs e)
        {
            _bufferingTextBlock.Visibility = System.Windows.Visibility.Collapsed;
        }

        private void VideoPlayer_BufferingStarted(object sender, RoutedEventArgs e)
        {
            _bufferingTextBlock.Visibility = System.Windows.Visibility.Visible;
        }

        private void VideoPlayer_Loaded(object sender, RoutedEventArgs e)
        {
            //Load an initial VideoSource if it is already set
            if (VideoSource != null)
            {
                _videoPlayer.Source = VideoSource;
            }
        }

        private void InitializePropertyValues()
        {
            // Set the media's starting Volume and SpeedRatio to the current value of the
            // their respective slider controls.
            _videoPlayer.Volume = (double)_volumeSlider.Value;
            _videoPlayer.SpeedRatio = (double)_speedRatioSlider.Value;
        }

        private void VideoPlayer_MediaFailed(object sender, ExceptionRoutedEventArgs e)
        {
            //TODO: set IsPlayableInMediaElement = false;

            MessageBox.Show(e.ErrorException.ToString());
        }

        private void VideoPlayer_MediaEnded(object sender, RoutedEventArgs e)
        {
            // When the media playback is finished. Stop() the media to seek to media start.
            _videoPlayer.Stop();
        }

        private void VideoPlayer_MediaOpened(object sender, RoutedEventArgs e)
        {
            // When the media opens, initialize the "Seek To" slider maximum value
            // to the total number of seconds in the length of the media clip.
            if (_videoPlayer.NaturalDuration.HasTimeSpan)
            {
                _timelineSlider.Maximum = _videoPlayer.NaturalDuration.TimeSpan.TotalSeconds;
                _timelineSlider.SmallChange = 1;
                _timelineSlider.LargeChange = Math.Min(10, _timelineSlider.Maximum / 10);
            }

            // Initialize the time to sync the slider to video position
            _seekSliderSyncTimer = new DispatcherTimer();
            _seekSliderSyncTimer.Interval = TimeSpan.FromMilliseconds(200);
            _seekSliderSyncTimer.Tick += new EventHandler(SeekSliderSyncTimer_Tick);
            _seekSliderSyncTimer.Start();

            //TODO: Mark the video as playable IsPlayableInMediaElement = true;
        }

        private void SeekSliderSyncTimer_Tick(object sender, EventArgs e)
        {
            if (!_isSeekSliderDragging)
            {
                _timelineSlider.Value = _videoPlayer.Position.TotalSeconds;
                _timelineSlider.ThumbToolTipText =
                    new DateTime(TimeSpan.FromSeconds(_timelineSlider.Value).Ticks).ToString("HH:mm:ss");
            }
        }

        private void SpeedRatioSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            // Change the speed of the media.
            _videoPlayer.SpeedRatio = e.NewValue;
        }

        private void VolumeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            // Change the volume of the media.

            _videoPlayer.Volume = e.NewValue;
        }

        private void StopButton_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            // The Stop method stops and resets the media to be played from
            // the beginning.
            _videoPlayer.Stop();
        }

        private void PauseButton_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            // The Pause method pauses the media if it is currently running.
            // The Play method can be used to resume.
            _videoPlayer.Pause();
        }

        private void PlayButton_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            try
            {
                // The Play method will begin the media if it is not currently active or
                // resume media if it is paused. This has no effect if the media is
                // already running.
                _videoPlayer.Play();

                // Initialize the MediaElement property values.
                InitializePropertyValues();
            }
            catch (Exception)
            {
                MessageBox.Show("Unable to play media", "Playback Error", MessageBoxButton.OK);
            }
        }
    }
}
