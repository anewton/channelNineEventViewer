using Autofac;
using ChannelNineEventFeed.Data.Interfaces;
using ChannelNineEventFeed.Library.Intefaces;
using ChannelNineEventFeed.Library.Interfaces;
using ChannelNineEventFeed.Library.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace ChannelNineEventFeed.WPF
{
    public partial class MainWindow : Window
    {
        public static readonly DependencyProperty PresentationDataProperty = DependencyProperty.Register("PresentationData", typeof(List<Presentation>), typeof(MainWindow), new PropertyMetadata(null));

        public MainWindow()
        {
            InitializeComponent();
            Application.Current.MainWindow = this;
            InitializeControlsAndData();
            Unloaded += MainWindow_Unloaded;
        }

        public List<Presentation> PresentationData
        {
            get { return (List<Presentation>)GetValue(PresentationDataProperty); }
            set { SetValue(PresentationDataProperty, value); }
        }

        private async void InitializeControlsAndData()
        {
            // Init database and gather event data
            EnableDisableUI(false);
            spinner.Start("Initializing database...");
            LoadAndBuildContainer();
            await Task.Run(() => InitializeDatabases());
            InitFilterControl();
            spinner.Stop();
            EnableDisableUI(true);
        }

        private void MainWindow_Unloaded(object sender, RoutedEventArgs e)
        {
            filterControl.Filtering -= FilterControl_Filtering;
            filterControl.FilterComplete -= FilterControl_FilterComplete;
            filterControl.Dispose();
        }

        private void LoadAndBuildContainer()
        {
            var builder = new ContainerBuilder();
            var dependencyModule = new DependencyModule();
            dependencyModule.Load(builder);
            var container = builder.Build();
            ((App)Application.Current).AppContainer = container;
        }

        private void InitializeDatabases()
        {
            var databaseInitializer = ((App)Application.Current).AppContainer.Resolve<IDatabaseInitializer>();
            var appSettings = ((App)Application.Current).AppContainer.Resolve<IAppSettings>();
            databaseInitializer.InitDatabase(appSettings.ConnectionString);
        }

        private void InitFilterControl()
        {
            filterControl.InitEvenFilter();
            filterControl.Filtering += FilterControl_Filtering;
            filterControl.FilterComplete += FilterControl_FilterComplete;
        }

        private void FilterControl_FilterComplete(List<IPresentation> presentations)
        {
            spinner.Stop();
            EnableDisableUI(true);
            if (presentations != null)
            {
                ThreadStart method = new ThreadStart(() =>
                {
                    PresentationData = presentations.Cast<Presentation>().ToList();
                    dataGrid.ItemsSource = PresentationData;
                    var viewButtonColumn = FindResource("ViewButtonColumn") as DataGridTemplateColumn;
                    if (dataGrid.Columns.Count == 5)
                    {
                        dataGrid.Columns.Insert(0, viewButtonColumn);
                    }
                });
                Dispatcher.BeginInvoke(DispatcherPriority.Input, method);
            }
            else
            {
                ThreadStart method = new ThreadStart(() =>
                {
                    PresentationData = null;
                    dataGrid.ItemsSource = null;
                });
                Dispatcher.BeginInvoke(DispatcherPriority.Input, method);
            }
        }

        private void FilterControl_Filtering(object sender, RoutedEventArgs e)
        {
            EnableDisableUI(false);
            spinner.Start("Filtering data...");
        }

        private void EnableDisableUI(bool isEnabled)
        {
            ThreadStart method = new ThreadStart(() =>
            {
                contentGrid.IsEnabled = isEnabled;
            });
            Dispatcher.BeginInvoke(method);
            // buttonGrid.IsEnabled = isEnabled;
        }

        private void ViewDetailsClick(object sender, RoutedEventArgs e)
        {
            try
            {
                var presentationService = ((App)Application.Current).AppContainer.Resolve<IPresentationService>();
                var presentationData = ((System.Windows.FrameworkElement)sender).DataContext as Presentation;
                var presentationId = presentationData.Id;
                var presentation = presentationService.GetPresentationBySessionId(presentationId);
                PresentationWindow popup = new PresentationWindow(presentation);
                popup.Show();
            }
            catch (Exception)
            {
                MessageBox.Show("Invalid request", "Error", MessageBoxButton.OK);
            }
        }
    }
}
