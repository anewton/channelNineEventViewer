using Autofac;
using System.Windows;

namespace ChannelNineEventFeed.WPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public IContainer AppContainer { get; set; }
    }
}
