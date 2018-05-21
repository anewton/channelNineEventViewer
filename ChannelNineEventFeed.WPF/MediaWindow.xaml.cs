using Autofac;
using ChannelNineEventFeed.Data.Feeds;
using ChannelNineEventFeed.Data.Interfaces;
using ChannelNineEventFeed.Library.Models;
using System;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace ChannelNineEventFeed.WPF
{
    public partial class MediaWindow : Window
    {
        public static readonly DependencyProperty CurrentSessionProperty =
            DependencyProperty.Register("CurrentSession", typeof(Session), typeof(MediaWindow), new PropertyMetadata(null));

        public static readonly DependencyProperty MediaItemProperty =
            DependencyProperty.Register("MediaItem", typeof(Media), typeof(MediaWindow), new PropertyMetadata(null, OnMediaItemChanged));

        private WindowState _previousState = WindowState.Normal;
        private bool _isFullScreen = false;

        private IMediaService _mediaService;

        public MediaWindow()
        {
            InitializeComponent();
            Loaded += MediaWindowLoaded;

            _mediaService = ((App)Application.Current).AppContainer.Resolve<IMediaService>();
        }

        public MediaWindow(Session session, Media media)
            : this()
        {
            MediaItem = media;
            CurrentSession = session;
        }

        public MediaWindow(Session session, Media media, string downloadedPath)
            : this()
        {
            MediaItem = media;
            CurrentSession = session;
            DownloadedPath = downloadedPath;
        }

        public string DownloadedPath { get; set; }

        public Session CurrentSession
        {
            get { return (Session)GetValue(CurrentSessionProperty); }
            set { SetValue(CurrentSessionProperty, value); }
        }

        public Media MediaItem
        {
            get { return (Media)GetValue(MediaItemProperty); }
            set { SetValue(MediaItemProperty, value); }
        }

        private static void OnMediaItemChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var owner = d as MediaWindow;
            owner.DataContext = e.NewValue;
        }

        private void MediaWindowLoaded(object sender, RoutedEventArgs e)
        {
            KeyUp += MediaWindowKeyUp;
            VidPlayer.DownloadRequested += VidPlayer_DownloadRequested;
        }

        private double BytesToMB(long value)
        {
            return (double)value / (1024 * 1024);
        }

        private string GetNetworkFolder()
        {
            var videoFolder = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "My Videos");
            var networkFolder = System.IO.Path.Combine(videoFolder, CurrentSession.EventName, CurrentSession.EventYear);
            return networkFolder;
        }

        private void VidPlayer_DownloadRequested(object sender, EventArgs e)
        {
            var feedType = VideoFeed.VideoFeeds.Where(vf => vf.Type == MediaItem.MediaType).FirstOrDefault();
            var mediaFileExtension = feedType.MediaFileExtension;
            var networkFolder = GetNetworkFolder();
            var dirExists = System.IO.Directory.Exists(networkFolder);
            if (!dirExists)
            {
                System.IO.Directory.CreateDirectory(networkFolder);
            }
            WebClient fileClient = new WebClient();
            fileClient.DownloadProgressChanged += (s, args) =>
            {
                //Console.Write(e.ProgressPercentage);

                ThreadStart method = new ThreadStart(() =>
                {
                    var received = BytesToMB(args.BytesReceived);
                    var total = BytesToMB(args.TotalBytesToReceive);
                    var percentDownloaded = received / total;
                    spinner.UpdateMessage(string.Format("Downloading... {0} of {1} MB. {2}", Math.Round(received, 2), Math.Round(total, 2), percentDownloaded.ToString("P")));
                });
                Dispatcher.BeginInvoke(DispatcherPriority.Input, method);
            };
            fileClient.DownloadFileCompleted += (s, args) =>
            {
                // Save media download progress as completed
                var mediaToken = args.UserState as Media;
                mediaToken.IsDownloaded = true;
                mediaToken.IsDownloadInProgress = false;
                _mediaService.UpdateMedia(mediaToken);

                ThreadStart method = new ThreadStart(() =>
                {
                    spinner.Stop();
                });
                Dispatcher.BeginInvoke(DispatcherPriority.Input, method);
            };
            var fileName = string.Format("{0}_{1}{2}", MediaItem.MediaType, CleanVideoTitle(CurrentSession.Title), mediaFileExtension);
            var fullPath = System.IO.Path.Combine(networkFolder, fileName);
            var fileExists = System.IO.File.Exists(fullPath);
            var canDownload = true;
            if (fileExists)
            {
                MessageBox.Show("The file already exists or is currently downloading.", "Download Cancelled", MessageBoxButton.OK);
                canDownload = false;
            }

            var mediaFromDatabase = _mediaService.GetMediaById(MediaItem.Id);
            if (mediaFromDatabase.IsDownloadInProgress.HasValue && mediaFromDatabase.IsDownloadInProgress.Value)
            {
                MessageBox.Show("The file is currently downloading.", "Download Cancelled", MessageBoxButton.OK);
                canDownload = false;
            }

            if (canDownload)
            {
                // Save media download progress as started
                MediaItem.IsDownloaded = false;
                MediaItem.IsDownloadInProgress = true;
                _mediaService.UpdateMedia(MediaItem);
                spinner.Start("Downloading...");
                fileClient.DownloadFileAsync(new Uri(MediaItem.DownloadLink), fullPath, MediaItem);
            }
        }

        private string CleanVideoTitle(string videoTitle)
        {
            if (string.IsNullOrEmpty(videoTitle))
            {
                return string.Empty;
            }
            return videoTitle.Replace(':', ' ').Replace('"', ' ').Replace('/', '-').Replace('?', ' ');
        }

        private void MediaWindowKeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Escape && _isFullScreen)
            {
                SwitchFullScreenMode();
            }
        }

        private void VidPlayer_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            SwitchFullScreenMode();
        }

        private void SwitchFullScreenMode()
        {
            if (!_isFullScreen)
            {
                WindowStyle = WindowStyle.None;
                _previousState = WindowState;
                WindowState = WindowState.Maximized;
            }
            else
            {
                WindowStyle = WindowStyle.SingleBorderWindow;
                WindowState = _previousState;
            }
            _isFullScreen = !_isFullScreen;
            VidPlayer.FullScreen = _isFullScreen;
        }

        private void PageLinkClicked(object sender, RoutedEventArgs e)
        {
            var url = (sender as Button).Content.ToString();
            Process.Start(new ProcessStartInfo(url));
        }
    }
}
