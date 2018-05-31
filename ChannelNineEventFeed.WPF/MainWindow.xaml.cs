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

        public static readonly DependencyProperty QueueProperty = DependencyProperty.Register("Queue", typeof(List<QueuedPresentation>), typeof(MainWindow), null);

        private GridLength _previousFilterPanelWidth = new GridLength(0);
        private GridLength _previousQueuePanelWidth = new GridLength(5);

        public MainWindow()
        {
            InitializeComponent();
            Application.Current.MainWindow = this;
            InitializeControlsAndData();
            Unloaded += MainWindow_Unloaded;

            Queue = new List<QueuedPresentation>();
            PresentationData = new List<Presentation>();
        }

        public List<Presentation> PresentationData
        {
            get { return (List<Presentation>)GetValue(PresentationDataProperty); }
            set { SetValue(PresentationDataProperty, value); }
        }

        public List<QueuedPresentation> Queue
        {
            get { return (List<QueuedPresentation>)GetValue(QueueProperty); }
            set { SetValue(QueueProperty, value); }
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

        private async void InitializeDatabases()
        {
            var databaseInitializer = ((App)Application.Current).AppContainer.Resolve<IDatabaseInitializer>();
            var appSettings = ((App)Application.Current).AppContainer.Resolve<IAppSettings>();
            await databaseInitializer.InitDatabase(appSettings.ConnectionString);
        }

        private async void CreateNewDatabases()
        {
            var databaseInitializer = ((App)Application.Current).AppContainer.Resolve<IDatabaseInitializer>();
            var appSettings = ((App)Application.Current).AppContainer.Resolve<IAppSettings>();
            await databaseInitializer.RecreateDatabase(appSettings.ConnectionString);
        }

        private void InitFilterControl()
        {
            filterControl.InitEventFilter();
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
                buttonGrid.IsEnabled = isEnabled;
                filterControlLabel.Opacity = isEnabled ? 1.0 : 0.6;
                presentationCountLabel.Opacity = isEnabled ? 1.0 : 0.6;
                presentationTitleLabel.Opacity = isEnabled ? 1.0 : 0.6;
                queueCountLabel.Opacity = isEnabled ? 1.0 : 0.6;
                queueTitleLabel.Opacity = isEnabled ? 1.0 : 0.6;
            });
            Dispatcher.BeginInvoke(method);
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

        private void ExpandCollapseSplitter(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var splitter = (GridSplitter)sender;
            var isLeftGridSplitter = splitter.Tag.ToString() == "Left";
            var filterPanelColumn = ((Grid)splitter.Parent).ColumnDefinitions[0];
            var queuePanelColumn = ((Grid)splitter.Parent).ColumnDefinitions[2];
            ColumnDefinition targetColumn = null;
            if (isLeftGridSplitter)
            {
                targetColumn = filterPanelColumn;
                if (targetColumn.Width.Value > 0)
                {
                    _previousFilterPanelWidth = filterPanelColumn.Width;
                }
                if (targetColumn.Width.Value > 0)
                {
                    targetColumn.Width = new GridLength(0);
                }
                else
                {
                    targetColumn.Width = _previousFilterPanelWidth;
                }
            }
            else
            {
                targetColumn = queuePanelColumn;
                if (targetColumn.Width.Value > 5)
                {
                    _previousQueuePanelWidth = queuePanelColumn.Width;
                }
                if (targetColumn.Width.Value > 5)
                {
                    targetColumn.Width = new GridLength(5);
                }
                else
                {
                    targetColumn.Width = _previousQueuePanelWidth;
                }
            }
        }

        private void ShowHideFilterPanel_Click(object sender, RoutedEventArgs e)
        {
            var buttonContent = "Filter Panel";
            var button = (Button)sender;
            ColumnDefinition targetColumn = eventsGrid.ColumnDefinitions[0];
            if (targetColumn.Width.Value > 0)
            {
                _previousFilterPanelWidth = targetColumn.Width;
            }
            if (targetColumn.Width.Value > 0)
            {
                targetColumn.Width = new GridLength(0);
                button.Content = "Show " + buttonContent;
            }
            else
            {
                targetColumn.Width = _previousFilterPanelWidth;
                button.Content = "Hide " + buttonContent;
            }
        }

        private void ShowHideQueuePanel_Click(object sender, RoutedEventArgs e)
        {
            var buttonContent = "Queue Panel";
            var button = (Button)sender;
            ColumnDefinition targetColumn = eventsGrid.ColumnDefinitions[2];
            if (targetColumn.Width.Value > 5)
            {
                _previousQueuePanelWidth = targetColumn.Width;
            }
            if (targetColumn.Width.Value > 5)
            {
                targetColumn.Width = new GridLength(5);
                button.Content = "Show " + buttonContent;
            }
            else
            {
                targetColumn.Width = _previousQueuePanelWidth;
                button.Content = "Hide " + buttonContent;
            }
        }

        private void NuclearOption_Click(object sender, RoutedEventArgs e)
        {
            var messageBoxResult = MessageBox.Show("Are you sure?", "Delete database and create a new one", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (messageBoxResult == MessageBoxResult.Yes)
            {

                App.Current.Dispatcher.Invoke(async () =>
                {
                    filterControl.Reset();
                    await Task.Run(() => CreateNewDatabases());
                    InitFilterControl();
                    PresentationData = null;
                    dataGrid.ItemsSource = null;
                });
            }
        }
    }
}
