using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace ChannelNineEventFeed.WPF.Converters
{
    public class BoolToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return Visibility.Collapsed;
            }

            VisibilityConverterMode mode = GetMode(parameter);
            switch (mode)
            {
                case VisibilityConverterMode.VisibleIfTrue:
                    return (bool)value ? Visibility.Visible : Visibility.Collapsed;
                case VisibilityConverterMode.VisibleIfNotTrue:
                    return !(bool)value ? Visibility.Visible : Visibility.Collapsed;
                default:
                    throw new InvalidOperationException("Invalid mode.");
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            VisibilityConverterMode mode = GetMode(parameter);
            switch (mode)
            {
                case VisibilityConverterMode.VisibleIfTrue:
                    return (Visibility)value == Visibility.Visible;
                case VisibilityConverterMode.VisibleIfNotTrue:
                    return (Visibility)value != Visibility.Visible;
                default:
                    throw new InvalidOperationException("Invalid mode.");
            }
        }

        private static VisibilityConverterMode GetMode(object parameter)
        {
            if (parameter == null)
            {
                return VisibilityConverterMode.VisibleIfTrue;
            }

            if (parameter is VisibilityConverterMode)
            {
                return (VisibilityConverterMode)parameter;
            }

            return (VisibilityConverterMode)Enum.Parse(typeof(VisibilityConverterMode), (string)parameter, false);
        }
    }
}