using Autofac;
using ChannelNineEventFeed.Data.Events;
using ChannelNineEventFeed.Data.Interfaces;
using ChannelNineEventFeed.Library.Interfaces;
using ChannelNineEventFeed.Library.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Xml;

namespace ChannelNineEventFeed.WPF.Controls
{
    public partial class FilterControl : UserControl, IDisposable
    {
        private IFeedService _feedService;
        private IPresentationService _presentationService;

        public FilterControl()
        {
            InitializeComponent();
        }

        public delegate void FilterCompleteHandler(List<IPresentation> presentations);
        public event FilterCompleteHandler FilterComplete;
        public event RoutedEventHandler Filtering;

        public EventFilters Filters { get; set; }

        public void InitEventFilter()
        {
            _feedService = ((App)Application.Current).AppContainer.Resolve<IFeedService>();
            _presentationService = ((App)Application.Current).AppContainer.Resolve<IPresentationService>();

            filterControlContainer.Children.Clear();

            // Event
            var eventNameFilterControl = new FilterSelectors(FilterType.Event, _feedService.GetEvents().ToList());
            eventNameFilterControl.FilterChanged += FilterChanged;
            filterControlContainer.Children.Add(eventNameFilterControl);
        }

        public void Reset()
        {
            Filters = new EventFilters();
            RemoveAllFilterControls();
            UpdateSelectedFiltersIndicator();
        }

        private void FilterChanged(object sender, Tuple<FilterType, List<object>> e)
        {
            Filtering?.Invoke(this, null);
            Application.Current.Dispatcher.Invoke(async () =>
            {
                await Task.Run(() => FilterPresentations(e));
            });
        }

        private bool ContainsFilterControlTypes(List<FilterType> filterTypes)
        {
            var containsFilterTypes = Dispatcher.Invoke(new Func<bool>(() =>
            {
                var result = false;
                var countOfFiltersFound = 0;
                foreach (FilterSelectors filterSelector in filterControlContainer.Children)
                {
                    if (filterTypes.Contains(filterSelector.FilterBy) && filterSelector.HasValue())
                    {
                        result = true;
                        countOfFiltersFound++;
                    }
                    else
                    {
                        result = false;
                        break;
                    }
                }
                result = countOfFiltersFound == filterTypes.Count();
                return result;
            }));
            return containsFilterTypes;
        }

        private void FilterPresentations(Tuple<FilterType, List<object>> e)
        {
            if (Filters == null)
            {
                Filters = new EventFilters();
            }

            switch (e.Item1)
            {
                case FilterType.Event:
                    if (e.Item2 == null || e.Item2.Count == 0)
                    {
                        ClearAllOtherFilters();
                    }
                    Filters.EventNames = new List<string>();
                    foreach (var filter in e.Item2)
                    {
                        Filters.EventNames.Add(filter.ToString());
                    }
                    break;
                case FilterType.Year:
                    ClearAllOtherFilters();
                    foreach (var filter in e.Item2)
                    {
                        Filters.EventYears.Add(filter.ToString());
                    }
                    break;
                case FilterType.SessionType:
                    Filters.SessionTypes = new List<string>();
                    foreach (var filter in e.Item2)
                    {
                        Filters.SessionTypes.Add(filter.ToString());
                    }
                    break;
                case FilterType.Category:
                    Filters.CategoryIds = new List<int>();
                    Filters.CategoryNames = new List<string>();
                    foreach (var filter in e.Item2)
                    {
                        Filters.CategoryIds.Add((filter as Category).Id);
                        Filters.CategoryNames.Add((filter as Category).Name);
                    }
                    break;
                case FilterType.Speaker:
                    Filters.SpeakerIds = new List<int>();
                    Filters.SpeakerNames = new List<string>();
                    foreach (var filter in e.Item2)
                    {
                        Filters.SpeakerIds.Add((filter as Speaker).Id);
                        Filters.SpeakerNames.Add((filter as Speaker).Name);
                    }
                    break;
            }

            // Must be filtering at least by Event and Year to display presentations
            List<IPresentation> presentations = null;
            var requiredFilters = new List<FilterType>() { FilterType.Event, FilterType.Year };
            var canFilter = ContainsFilterControlTypes(requiredFilters);
            if (canFilter)
            {
                presentations = _presentationService.GetFilteredPresentations(Filters).ToList();
            }
            UpdateFilterControls(e);
            UpdateSelectedFiltersIndicator();
            FilterComplete?.Invoke(presentations);
        }

        private void ClearAllOtherFilters()
        {
            Filters.EventYears = new List<string>();
            Filters.CategoryIds = new List<int>();
            Filters.CategoryNames = new List<string>();
            Filters.SpeakerIds = new List<int>();
            Filters.SpeakerNames = new List<string>();
        }

        private void UpdateFilterControls(Tuple<FilterType, List<object>> e)
        {
            // The Event filter was cleared
            if (e.Item1 == FilterType.Event && e.Item2.Count() == 0)
            {
                RemoveFilterControls(new List<FilterType>() { FilterType.Year, FilterType.Category, FilterType.SessionType, FilterType.Speaker });
            }

            // The Year filter was cleared
            if (e.Item1 == FilterType.Year && e.Item2.Count() == 0)
            {
                RemoveFilterControls(new List<FilterType>() { FilterType.Category, FilterType.SessionType, FilterType.Speaker });
            }

            // At least one Event was selected
            if (e.Item1 == FilterType.Event && e.Item2.Count() > 0)
            {
                RemoveFilterControls(new List<FilterType>() { FilterType.Category, FilterType.SessionType, FilterType.Speaker, FilterType.Year });
                // Add Year filter selector
                var yearValues = new List<string>();
                foreach (var item in e.Item2)
                {
                    var selectedEvent = _feedService.GetEvents().Where(x => x.Name == item.ToString()).FirstOrDefault();
                    yearValues.AddRange(selectedEvent.Years);
                }
                if (yearValues.Count > 0)
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        var yearsFilterControl = new FilterSelectors(FilterType.Year, yearValues);
                        yearsFilterControl.FilterChanged += FilterChanged;
                        filterControlContainer.Children.Add(yearsFilterControl);
                    });
                }
            }

            // Add or update other filters
            if (e.Item1 == FilterType.Year && e.Item2.Count() > 0 && ContainsFilterControlTypes(new List<FilterType>() { FilterType.Event, FilterType.Year }))
            {
                // Add or update Category filter selector
                if (!ContainsFilterControlTypes(new List<FilterType>() { FilterType.Category }))
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        var categoriesFilterControl = new FilterSelectors(FilterType.Category, _presentationService.GetFilteredCategories(Filters));
                        categoriesFilterControl.FilterChanged += FilterChanged;
                        filterControlContainer.Children.Add(categoriesFilterControl);
                    });
                }
                else
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        var filterControl = FindFilterSelectorByFilterType(FilterType.Category);
                        if (filterControl != null)
                        {
                            filterControl.UpdateItems(_presentationService.GetFilteredCategories(Filters));
                        }
                    });
                }

                // Add or update Session Type filter selector
                if (!ContainsFilterControlTypes(new List<FilterType>() { FilterType.SessionType }))
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        var sessionTypeFilterControl = new FilterSelectors(FilterType.SessionType, _presentationService.GetFilteredSessionTypes(Filters));
                        sessionTypeFilterControl.FilterChanged += FilterChanged;
                        filterControlContainer.Children.Add(sessionTypeFilterControl);
                    });
                }
                else
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        var filterControl = FindFilterSelectorByFilterType(FilterType.SessionType);
                        if (filterControl != null)
                        {
                            filterControl.UpdateItems(_presentationService.GetFilteredSessionTypes(Filters));
                        }
                    });
                }

                // Add or update Speaker filter selector
                if (!ContainsFilterControlTypes(new List<FilterType>() { FilterType.Speaker }))
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        var speakerFilterControl = new FilterSelectors(FilterType.Speaker, _presentationService.GetFilteredSpeakers(Filters));
                        speakerFilterControl.FilterChanged += FilterChanged;
                        filterControlContainer.Children.Add(speakerFilterControl);
                    });
                }
                else
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        var filterControl = FindFilterSelectorByFilterType(FilterType.Speaker);
                        if (filterControl != null)
                        {
                            filterControl.UpdateItems(_presentationService.GetFilteredSpeakers(Filters));
                        }
                    });
                }
            }
        }

        private void RemoveAllFilterControls()
        {
            Dispatcher.Invoke(() =>
            {
                if (filterControlContainer.Children.Count > 1)
                {
                    var filterSelectorsToRemove = new List<FilterSelectors>();
                    foreach (FilterSelectors filterSelector in filterControlContainer.Children)
                    {
                        filterSelectorsToRemove.Add(filterSelector);
                    }
                    foreach (var filterSelector in filterSelectorsToRemove)
                    {
                        filterControlContainer.Children.Remove(filterSelector);
                    }
                }
            });
        }

        private void RemoveFilterControls(List<FilterType> filterTypes)
        {
            Dispatcher.Invoke(() =>
            {
                if (filterControlContainer.Children.Count > 1)
                {
                    var filterSelectorsToRemove = new List<FilterSelectors>();
                    foreach (FilterSelectors filterSelector in filterControlContainer.Children)
                    {
                        if (filterSelector.FilterBy != FilterType.Event && filterTypes.Contains(filterSelector.FilterBy))
                        {
                            filterSelectorsToRemove.Add(filterSelector);
                        }
                    }
                    foreach (var filterSelector in filterSelectorsToRemove)
                    {
                        filterControlContainer.Children.Remove(filterSelector);
                    }
                }
            });
        }

        private void UpdateSelectedFiltersIndicator()
        {
            Dispatcher.Invoke(() =>
            {
                TextBlock textBlockToReplace = null;
                foreach (var item in controlsGrid.Children)
                {
                    if (item is TextBlock && ((TextBlock)item).Name == "selectedFiltersIndicator")
                    {
                        textBlockToReplace = (TextBlock)item;
                    }
                }
                controlsGrid.Children.Remove(textBlockToReplace);
                if (Filters.HasAtLeastOneFilter())
                {
                    string xaml = "<TextBlock xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml' xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation' x:Name='selectedFiltersIndicator' HorizontalAlignment='Left' TextWrapping='Wrap' Margin='12,0,0,6' >{0}</TextBlock>";
                    var logicSummaryTextBlock = (TextBlock)XamlReader.Load(new XmlTextReader(new StringReader(string.Format(xaml, "Logic Summary:    " + Filters.GetXamlToString()))));
                    controlsGrid.Children.Add(logicSummaryTextBlock);
                }
            });
        }

        private FilterSelectors FindFilterSelectorByFilterType(FilterType filterType)
        {
            FilterSelectors result = null;
            if (filterControlContainer.Children.Count > 1)
            {
                foreach (FilterSelectors filterSelector in filterControlContainer.Children)
                {
                    if (filterSelector.FilterBy == filterType)
                    {
                        result = filterSelector;
                        break;
                    }
                }
            }
            return result;
        }

        #region IDisposable

        private bool _isDisposed;

        ~FilterControl()
        {
            Dispose(false);
        }

        [MethodImpl(MethodImplOptions.NoOptimization)]
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool isDisposing)
        {
            if (_isDisposed)
            {
                return;
            }

            if (isDisposing)
            {
                // Invoke Dispose for any disposable objects here
                foreach (FilterSelectors filterSelector in filterControlContainer.Children)
                {
                    filterSelector.FilterChanged -= FilterChanged;
                    filterSelector.Dispose();
                }
                filterControlContainer.Children.Clear();
                _isDisposed = true;
            }
        }

        #endregion
    }
}
