using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace ChannelNineEventFeed.WPF.Controls
{
    public partial class SpinnerSwirl : UserControl
    {
        public SpinnerSwirl()
        {
            InitializeComponent();
        }

        public void Start(string message)
        {
            textMessage.Text = message;
            var spinnerStoryboard = FindResource("spinner") as Storyboard;
            spinnerStoryboard.Begin();
            infoGrid.Visibility = Visibility.Visible;
        }

        public void Stop()
        {
            Dispatcher.BeginInvoke(new ThreadStart(() => StopSpinner()));
        }

        public void UpdateMessage(string message)
        {
            textMessage.Text = message;
        }

        private void StopSpinner()
        {
            textMessage.Text = string.Empty;
            var spinnerStoryboard = FindResource("spinner") as Storyboard;
            spinnerStoryboard.Stop();
            infoGrid.Visibility = Visibility.Hidden;
        }
    }
}
