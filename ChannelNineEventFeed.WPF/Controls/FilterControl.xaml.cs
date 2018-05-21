using Autofac;
using ChannelNineEventFeed.Data.Events;
using ChannelNineEventFeed.Data.Interfaces;
using ChannelNineEventFeed.Library.Interfaces;
using ChannelNineEventFeed.Library.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

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

        public void InitEvenFilter()
        {
            _feedService = ((App)Application.Current).AppContainer.Resolve<IFeedService>();
            _presentationService = ((App)Application.Current).AppContainer.Resolve<IPresentationService>();

            filterControlContainer.Children.Clear();

            // Event
            var eventNameFilterControl = new FilterSelectors(FilterType.Event, _feedService.GetEvents().ToList());
            eventNameFilterControl.FilterChanged += FilterChanged;
            filterControlContainer.Children.Add(eventNameFilterControl);
        }

        private void FilterChanged(object sender, Tuple<FilterType, List<object>> e)
        {
            Filtering?.Invoke(this, null);

            Application.Current.Dispatcher.Invoke(async () =>
            {
                await Task.Run(() => FilterPresentations(e));
            });

            //// The Event filter was cleared
            //if (e.Item1 == FilterType.Event && e.Item2.Count() == 0)
            //{
            //    RemoveFilterControls(new List<FilterType>() { FilterType.Category, FilterType.SessionType, FilterType.Level, FilterType.Year });
            //}

            //// At least one Event was selected
            //if (e.Item1 == FilterType.Event && e.Item2.Count() > 0)
            //{
            //    RemoveFilterControls(new List<FilterType>() { FilterType.Category, FilterType.SessionType, FilterType.Level, FilterType.Year });

            //    // Add Year Filter
            //    var selectedEvent = _feedService.GetEvents().Where(x => x.Name == e.Item2[0].ToString()).FirstOrDefault();
            //    var yearsFilterControl = new FilterSelectors(FilterType.Year, selectedEvent.Years);
            //    yearsFilterControl.FilterChanged += FilterChanged;
            //    filterControlContainer.Children.Add(yearsFilterControl);
            //}

            //// Event and Year filters exist and both have at least one selected value
            //if (e.Item1 == FilterType.Year && e.Item2.Count() > 0 && ContainsFilterControlTypes(new List<FilterType>() { FilterType.Event, FilterType.Year }))
            //{
            //    // Add Category Filter
            //    var categoriesFilterControl = new FilterSelectors(FilterType.Category, _presentationService.GetFilteredCategories(Filters));
            //    categoriesFilterControl.FilterChanged += FilterChanged;
            //    filterControlContainer.Children.Add(categoriesFilterControl);
            //}
        }

        private bool ContainsFilterControlTypes(List<FilterType> filterTypes)
        {
            var containsFilterTypes = Dispatcher.Invoke(new Func<bool>(() =>
            {
                var result = false;

                if (filterControlContainer.Children.Count > 1)
                {
                    var countOfFiltersFound = 0;
                    foreach (FilterSelectors filterSelector in filterControlContainer.Children)
                    {
                        if (filterTypes.Contains(filterSelector.FilterBy) /*&& filterSelector.HasValue()*/)
                        {
                            countOfFiltersFound++;
                        }
                    }
                    result = countOfFiltersFound == filterTypes.Count();
                }
                return result;
            }));
            return containsFilterTypes;
        }

        private void RemoveFilterControls(List<FilterType> filterTypes)
        {
            //if (filterControlContainer.Children.Count > 1)
            //{
            //    var filterSelectorsToRemove = new List<FilterSelectors>();
            //    foreach (FilterSelectors filterSelector in filterControlContainer.Children)
            //    {
            //        if (filterSelector.FilterBy != FilterType.Event && filterTypes.Contains(filterSelector.FilterBy))
            //        {
            //            filterSelectorsToRemove.Add(filterSelector);
            //        }
            //    }
            //    foreach (var filterSelector in filterSelectorsToRemove)
            //    {
            //        filterControlContainer.Children.Remove(filterSelector);
            //    }
            //}

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

        private void FilterPresentations(Tuple<FilterType, List<object>> e)
        {
            if (Filters == null)
            {
                Filters = new EventFilters();
            }

            switch (e.Item1)
            {
                case FilterType.Event:
                    Filters.EventNames = new List<string>();
                    foreach (var filter in e.Item2)
                    {
                        Filters.EventNames.Add(filter.ToString());
                    }

                    break;
                case FilterType.Year:
                    Filters.EventYears = new List<string>();
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
                    Filters.CategorIds = new List<int>();
                    foreach (var filter in e.Item2)
                    {
                        Filters.CategorIds.Add((filter as Category).Id);
                    }
                    break;
                case FilterType.Speaker:
                    Filters.SpeakerIds = new List<int>();
                    foreach (var filter in e.Item2)
                    {
                        Filters.SpeakerIds.Add((filter as Speaker).Id);
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

            AddFilterControls(e);
            FilterComplete?.Invoke(presentations);
        }

        private void AddFilterControls(Tuple<FilterType, List<object>> e)
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
                var selectedEvent = _feedService.GetEvents().Where(x => x.Name == e.Item2[0].ToString()).FirstOrDefault();

                Application.Current.Dispatcher.Invoke(() =>
                {
                    var yearsFilterControl = new FilterSelectors(FilterType.Year, selectedEvent.Years);
                    yearsFilterControl.FilterChanged += FilterChanged;
                    filterControlContainer.Children.Add(yearsFilterControl);
                });
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
