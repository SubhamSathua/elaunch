using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace ELaunch.Converters
{
    /// <summary>
    /// Converts null values to Visibility
    /// </summary>
    public class NullToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool inverse = parameter != null && bool.Parse(parameter.ToString() ?? "false");
            
            if (inverse)
            {
                // Show when value is NOT null
                return value != null ? Visibility.Visible : Visibility.Collapsed;
            }
            else
            {
                // Show when value is null
                return value == null ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Converts bool to Visibility
    /// </summary>
    public class BoolToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool boolValue)
            {
                bool inverse = parameter?.ToString() == "Inverse";
                if (inverse)
                    boolValue = !boolValue;
                    
                return boolValue ? Visibility.Visible : Visibility.Collapsed;
            }
            
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Converts multiple bools to Visibility (for default profile icon)
    /// Shows only when both IsGuest and IsAddNew are false
    /// </summary>
    public class MultiBoolToVisibilityConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length >= 2 && values[0] is bool isGuest && values[1] is bool isAddNew)
            {
                // Show default icon only when both are false
                return (!isGuest && !isAddNew) ? Visibility.Visible : Visibility.Collapsed;
            }
            
            return Visibility.Collapsed;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
