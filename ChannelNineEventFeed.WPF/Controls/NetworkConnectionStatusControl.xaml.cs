using System.Net.NetworkInformation;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace ChannelNineEventFeed.WPF.Controls
{
    public partial class NetworkConnectionStatusControl : UserControl
    {
        public static readonly DependencyProperty IsNetworkConnectedProperty =
                    DependencyProperty.Register("IsNetworkConnected", typeof(bool), typeof(NetworkConnectionStatusControl), new PropertyMetadata(true));

        public NetworkConnectionStatusControl()
        {
            InitializeComponent();
            Loaded += NetworkConnectionStatusControl_Loaded;
            Unloaded += NetworkConnectionStatusControl_Unloaded;
        }

        public bool IsNetworkConnected
        {
            get { return (bool)GetValue(IsNetworkConnectedProperty); }
            set { SetValue(IsNetworkConnectedProperty, value); }
        }

        private void NetworkConnectionStatusControl_Unloaded(object sender, RoutedEventArgs e)
        {
            NetworkChange.NetworkAvailabilityChanged -= new NetworkAvailabilityChangedEventHandler(OnNetworkAvailabilityChanged);
        }

        private void NetworkConnectionStatusControl_Loaded(object sender, RoutedEventArgs e)
        {
            NetworkChange.NetworkAvailabilityChanged += new NetworkAvailabilityChangedEventHandler(OnNetworkAvailabilityChanged);
            ThreadStart method = new ThreadStart(() =>
            {
                IsNetworkConnected = NetworkInterface.GetIsNetworkAvailable();
            });
            Dispatcher.BeginInvoke(DispatcherPriority.Input, method);
        }

        private void OnNetworkAvailabilityChanged(object sender, NetworkAvailabilityEventArgs e)
        {
            ThreadStart method = new ThreadStart(() =>
            {
                IsNetworkConnected = e.IsAvailable;
            });
            Dispatcher.BeginInvoke(DispatcherPriority.Input, method);
        }
    }
}
