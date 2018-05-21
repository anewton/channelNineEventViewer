using ChannelNineEventFeed.Library.Interfaces;
using ChannelNineEventFeed.WPF.Extensions;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;

namespace ChannelNineEventFeed.WPF.Controls
{
    public partial class FilterSelectors : UserControl, IDisposable
    {
        public FilterSelectors()
        {
            InitializeComponent();
        }

        public FilterSelectors(FilterType type, IEnumerable<IEvent> items)
            : this()
        {
            FilterBy = type;
            textTitle.Text = type.ToString().ProperCaseWithSpaces();
            if (items != null)
            {
                AddItems(items);
            }
        }

        public FilterSelectors(FilterType type, IEnumerable<string> items = null)
            : this()
        {
            FilterBy = type;
            textTitle.Text = type.ToString().ProperCaseWithSpaces();
            if (items != null)
            {
                AddItems(items);
            }
        }

        public FilterSelectors(FilterType type, IEnumerable<ICategory> items)
            : this()
        {
            FilterBy = type;
            textTitle.Text = type.ToString().ProperCaseWithSpaces();
            if (items != null)
            {
                AddItems(items);
            }
        }

        public FilterSelectors(FilterType type, IEnumerable<ISpeaker> items)
            : this()
        {
            FilterBy = type;
            textTitle.Text = type.ToString().ProperCaseWithSpaces();
            if (items != null)
            {
                AddItems(items);
            }
        }

        public event EventHandler<Tuple<FilterType, List<object>>> FilterChanged;

        public FilterType FilterBy { get; private set; }

        internal bool HasValue()
        {
            foreach (CheckBox checkBox in selectors.Children)
            {
                if (checkBox.IsChecked.HasValue && checkBox.IsChecked.Value)
                {
                    return true;
                }
            }
            return false;
        }

        internal void UpdateItems(IEnumerable<ICategory> items)
        {
            AddItems(items);
        }

        internal void UpdateItems(IEnumerable<ISpeaker> items)
        {
            AddItems(items);
        }

        internal void UpdateItems(IEnumerable<string> items)
        {
            AddItems(items);
        }

        private void AddItems(IEnumerable<IEvent> items)
        {
            RemoveItems();
            foreach (var item in items)
            {
                AddCheckBoxControl(item.Name, item.Name);
            }
        }

        private void AddItems(IEnumerable<ICategory> items)
        {
            RemoveItems();
            foreach (var item in items)
            {
                if (!string.IsNullOrEmpty(item.Name))
                {
                    AddCheckBoxControl(item.Name, item);
                }
            }
        }

        private void AddItems(IEnumerable<ISpeaker> items)
        {
            RemoveItems();
            foreach (var item in items)
            {
                if (!string.IsNullOrEmpty(item.Name))
                {
                    AddCheckBoxControl(item.Name, item);
                }
            }
        }

        private void AddItems(IEnumerable<string> items)
        {
            RemoveItems();
            foreach (var item in items)
            {
                if (!string.IsNullOrEmpty(item))
                {
                    AddCheckBoxControl(item, item);
                }
            }
        }

        private void AddCheckBoxControl(string label, object tag)
        {
            var checkbox = new CheckBox()
            {
                Content = label,
                Tag = tag,
                IsThreeState = false,
                Margin = new Thickness(12, 12, 12, 12)
            };
            checkbox.Checked += CheckboxChanged;
            checkbox.Unchecked += CheckboxChanged;
            selectors.Children.Add(checkbox);
        }

        private void RemoveItems()
        {
            if (selectors.Children != null && selectors.Children.Count > 0)
            {
                foreach (CheckBox checkbox in selectors.Children)
                {
                    checkbox.Checked -= CheckboxChanged;
                    checkbox.Unchecked -= CheckboxChanged;
                }
                selectors.Children.Clear();
            }
        }

        private void CheckboxChanged(object sender, RoutedEventArgs e)
        {
            //get select values
            var selectedValues = new List<object>();
            foreach (CheckBox checkBox in selectors.Children)
            {
                if (checkBox.IsChecked.HasValue && checkBox.IsChecked.Value)
                {
                    selectedValues.Add(checkBox.Tag);
                }
            }
            FilterChanged?.Invoke(this, new Tuple<FilterType, List<object>>(FilterBy, selectedValues));
        }

        #region IDisposable

        private bool _isDisposed;

        ~FilterSelectors()
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
                if (selectors.Children != null && selectors.Children.Count > 0)
                {
                    foreach (CheckBox checkbox in selectors.Children)
                    {
                        checkbox.Checked -= CheckboxChanged;
                        checkbox.Unchecked -= CheckboxChanged;
                    }
                    selectors.Children.Clear();
                }
                _isDisposed = true;
            }
        }

        #endregion
    }
}
