using ChannelNineEventFeed.WPF.Extensions;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;

namespace ChannelNineEventFeed.WPF.Controls
{
    public partial class GroupedSelectorControl : UserControl
    {
        public static readonly DependencyProperty FilterItemsProperty =
            DependencyProperty.Register("FilterItems", typeof(IDictionary<string, object>), typeof(GroupedSelectorControl), null);

        public GroupedSelectorControl()
        {
            InitializeComponent();
        }

        public GroupedSelectorControl(FilterType type, IEnumerable<object> items, Func<object, string> selector)
            : this()
        {
            FilterBy = type;
            textTitle.Text = type.ToString().ProperCaseWithSpaces();
            var displayItems = new Dictionary<string, object>();
            foreach (var item in items)
            {
                if (item != null)
                {
                    displayItems.Add(selector.Invoke(item), item);
                }
            }
        }

        //public enum ControlModes
        //{
        //    Grouped,
        //    GroupSelected,
        //    All
        //}

        public event EventHandler<Tuple<FilterType, List<object>>> FilterChanged;

        public FilterType FilterBy { get; private set; }

        public IDictionary<string, object> FilterItems
        {
            get { return (IDictionary<string, object>)GetValue(FilterItemsProperty); }
            set { SetValue(FilterItemsProperty, value); }
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
            //checkbox.Checked += CheckboxChanged;
            //checkbox.Unchecked += CheckboxChanged;
            //selectors.Children.Add(checkbox);
        }

        //private void CheckboxChanged(object sender, RoutedEventArgs e)
        //{
        //    //get select values
        //    var selectedValues = new List<object>();
        //    foreach (CheckBox checkBox in selectors.Children)
        //    {
        //        if (checkBox.IsChecked.HasValue && checkBox.IsChecked.Value)
        //        {
        //            selectedValues.Add(checkBox.Tag);
        //        }
        //    }
        //    FilterChanged?.Invoke(this, new Tuple<FilterType, List<object>>(FilterBy, selectedValues));
        //}

        #region IDisposable

        private bool _isDisposed;

        ~GroupedSelectorControl()
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
                //if (selectors.Children != null && selectors.Children.Count > 0)
                //{
                //    foreach (CheckBox checkbox in selectors.Children)
                //    {
                //        checkbox.Checked -= CheckboxChanged;
                //        checkbox.Unchecked -= CheckboxChanged;
                //    }
                //    selectors.Children.Clear();
                //}
                _isDisposed = true;
            }
        }

        #endregion
    }
}