using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using ELaunch.Services;

namespace ELaunch.Converters
{
    /// <summary>
    /// Converts IsAddNew boolean to the appropriate add profile icon path
    /// </summary>
    public class AddProfileIconConverter : IValueConverter
    {
        public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool isAddNew && isAddNew)
            {
                string? iconPath = ThemeManager.GetIconPath("add-profile");

                if (iconPath != null)
                {
                    try
                    {
                        var bitmap = new BitmapImage();
                        bitmap.BeginInit();
                        bitmap.UriSource = new Uri(iconPath, UriKind.Absolute);
                        bitmap.CacheOption = BitmapCacheOption.OnLoad;
                        bitmap.EndInit();
                        bitmap.Freeze();
                        return bitmap;
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"Failed to load add-profile icon: {ex.Message}");
                    }
                }
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
