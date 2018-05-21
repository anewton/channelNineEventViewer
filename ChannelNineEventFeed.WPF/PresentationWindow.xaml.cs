using Autofac;
using ChannelNineEventFeed.Data.Feeds;
using ChannelNineEventFeed.Data.Interfaces;
using ChannelNineEventFeed.Library.Intefaces;
using ChannelNineEventFeed.Library.Interfaces;
using ChannelNineEventFeed.Library.Models;
using System;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Markup;
using System.Windows.Media;

namespace ChannelNineEventFeed.WPF
{
    public partial class PresentationWindow : Window
    {
        public static readonly DependencyProperty PresentationProperty =
            DependencyProperty.Register("Presentation", typeof(IPresentation), typeof(PresentationWindow), new PropertyMetadata(null, OnPresentationChanged));

        public PresentationWindow()
        {
            InitializeComponent();
        }

        public PresentationWindow(IPresentation presentation) : this()
        {
            Presentation = presentation;
        }

        public IPresentation Presentation
        {
            get { return (IPresentation)GetValue(PresentationProperty); }
            set { SetValue(PresentationProperty, value); }
        }

        private static void OnPresentationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var owner = d as PresentationWindow;
            owner.DataContext = e.NewValue;
            owner.ConvertDescriptionToXaml();
        }

        private void ConvertDescriptionToXaml()
        {
            var xamlText = XAMLConverter.HtmlToXamlConverter.ConvertHtmlToXaml(Presentation.Description, false);
            var xamlDescription = XamlReader.Parse(xamlText);
            var flowDoc = new FlowDocument();
            var section = xamlDescription as Section;
            if (section != null)
            {
                section.FontFamily = new FontFamily("Segoe UI");
                section.FontSize = 16;
                section.Margin = new Thickness(0, 0, 0, 0);
                flowDoc.Blocks.Add(section);
            }
            flowDocReader.Document = flowDoc;
        }

        private void PageLinkClicked(object sender, RoutedEventArgs e)
        {
            var url = (sender as Button).Content.ToString();
            Process.Start(new ProcessStartInfo(url));
        }

        private void SpeakerNameClicked(object sender, System.Windows.RoutedEventArgs e)
        {
            var url = (sender as Button).Tag.ToString();
            Process.Start(new ProcessStartInfo(url));
        }

        private void MediaClicked(object sender, System.Windows.RoutedEventArgs e)
        {
            var url = (sender as Button).Tag.ToString();
            var mediaItem = (sender as Button).DataContext as Media;
            var presentationService = ((App)Application.Current).AppContainer.Resolve<IPresentationService>();
            var session = presentationService.GetSessionBySessionId(mediaItem.SessionId);
            var mediaWindow = new MediaWindow((Session)session, mediaItem);
            mediaWindow.Show();
        }

        private void SlidesClicked(object sender, RoutedEventArgs e)
        {
            var url = Presentation.SlidesLink;
            Process.Start(new ProcessStartInfo(url));
        }

        private string GetNetworkFolder(IPresentation presentation)
        {
            var videoFolder = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "My Videos");
            var networkFolder = System.IO.Path.Combine(videoFolder, presentation.EventName, presentation.EventYear);
            return networkFolder;
        }

        private void PlayDownloadedMediaClicked(object sender, RoutedEventArgs e)
        {
            var mediaItem = (sender as Button).DataContext as Media;
            var pres = Presentation;
            var presentationService = ((App)Application.Current).AppContainer.Resolve<IPresentationService>();
            var appSettings = ((App)Application.Current).AppContainer.Resolve<IAppSettings>();
            var session = presentationService.GetSessionBySessionId(mediaItem.SessionId);
            var feedType = VideoFeed.VideoFeeds.Where(vf => vf.Type == mediaItem.MediaType).FirstOrDefault();
            var mediaFileExtension = feedType.MediaFileExtension;
            var networkFolder = GetNetworkFolder(pres);

            var fileName = string.Format("{0}_{1}{2}", mediaItem.MediaType, CleanVideoTitle(pres.Title), mediaFileExtension);
            var downlodedPath = System.IO.Path.Combine(networkFolder, fileName);
            mediaItem.DownloadLink = downlodedPath;

            var mediaWindow = new MediaWindow((Session)session, mediaItem);
            mediaWindow.Show();
        }

        private string CleanVideoTitle(string videoTitle)
        {
            if (string.IsNullOrEmpty(videoTitle))
            {
                return string.Empty;
            }
            return videoTitle.Replace(':', ' ').Replace('"', ' ').Replace('/', '-').Replace('?', ' ');
        }
    }
}
