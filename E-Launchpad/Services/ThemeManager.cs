using System;
using System.Windows;
using System.Windows.Media;
using Microsoft.Win32;

namespace E_Launchpad.Services
{
    public static class ThemeManager
    {
        private static bool _isDarkMode = true;
        private static bool _isSystemMode = false;
        
        public static event Action? ThemeChanged;

        public static bool IsDarkMode => _isDarkMode;
        public static bool IsSystemMode => _isSystemMode;

        public static void Initialize()
        {
            // Load default dark theme
            ApplyDarkTheme();
        }

        public static void ApplyDarkTheme()
        {
            System.Diagnostics.Debug.WriteLine("ThemeManager.ApplyDarkTheme() called");
            _isDarkMode = true;
            _isSystemMode = false;
            
            var resources = Application.Current.Resources;
            
            // Dark theme colors - applied directly to resources
            resources["Background"] = new SolidColorBrush(Color.FromRgb(0x0e, 0x11, 0x1b));
            resources["CardContainer.Background"] = new SolidColorBrush(Color.FromRgb(0x15, 0x1a, 0x28));
            resources["CardContainer.Border"] = new SolidColorBrush(Color.FromRgb(0x29, 0x2e, 0x3b));
            resources["Card.Background"] = new SolidColorBrush(Color.FromRgb(0x22, 0x28, 0x3c));
            resources["Card.Border"] = new SolidColorBrush(Color.FromRgb(0x38, 0x3e, 0x50));
            resources["GuestButton.Background"] = new SolidColorBrush(Color.FromRgb(0x2d, 0x33, 0x48));
            resources["GuestButton.Border"] = new SolidColorBrush(Color.FromRgb(0x40, 0x45, 0x59));
            resources["Text.Primary"] = new SolidColorBrush(Colors.White);
            resources["Text.Secondary"] = new SolidColorBrush(Color.FromRgb(0xaa, 0xaa, 0xaa));
            
            // Settings colors
            resources["Settings.MainBg"] = new SolidColorBrush(Color.FromRgb(0x1c, 0x23, 0x38));
            resources["Settings.MainBgOutline"] = new SolidColorBrush(Color.FromRgb(0x38, 0x41, 0x5b));
            resources["Settings.SelectedListViewBg"] = new SolidColorBrush(Color.FromRgb(0x15, 0x1a, 0x28));
            resources["Settings.UnselectedListViewBg"] = new SolidColorBrush(Colors.Transparent);
            resources["Settings.PreviewBg"] = new SolidColorBrush(Color.FromRgb(0x15, 0x1a, 0x28));
            resources["Settings.PreviewOutline"] = new SolidColorBrush(Color.FromRgb(0x15, 0x1a, 0x28));
            resources["Settings.ThemeCardChecked"] = new SolidColorBrush(Color.FromRgb(0x4a, 0x54, 0x71));
            resources["Settings.ThemeCardUnchecked"] = new SolidColorBrush(Color.FromRgb(0x1b, 0x21, 0x32));
            resources["Settings.CloseBtnBg"] = new SolidColorBrush(Color.FromRgb(0x15, 0x1a, 0x28));
            
            // Hover colors
            resources["Settings.ThemeCardUncheckedHover"] = new SolidColorBrush(Color.FromRgb(0x25, 0x2c, 0x42));
            resources["Settings.UnselectedListViewHover"] = new SolidColorBrush(Color.FromRgb(0x1a, 0x20, 0x30));
            resources["Settings.CloseBtnHover"] = new SolidColorBrush(Color.FromRgb(0x25, 0x2c, 0x42));
            resources["GuestButton.Hover"] = new SolidColorBrush(Color.FromRgb(0x38, 0x40, 0x58));

            System.Diagnostics.Debug.WriteLine("Dark theme applied directly to resources");
            ThemeChanged?.Invoke();
        }

        public static void ApplyLightTheme()
        {
            System.Diagnostics.Debug.WriteLine("ThemeManager.ApplyLightTheme() called");
            _isDarkMode = false;
            _isSystemMode = false;
            
            var resources = Application.Current.Resources;
            
            // Light theme colors - applied directly to resources  
            resources["Background"] = new SolidColorBrush(Color.FromRgb(0xef, 0xea, 0xe7));
            resources["CardContainer.Background"] = new SolidColorBrush(Color.FromRgb(0xf8, 0xf4, 0xf1));
            resources["CardContainer.Border"] = new SolidColorBrush(Color.FromRgb(0xcb, 0xcb, 0xcc));
            resources["Card.Background"] = new SolidColorBrush(Color.FromRgb(0xfb, 0xf9, 0xf7));
            resources["Card.Border"] = new SolidColorBrush(Color.FromRgb(0xe3, 0xe3, 0xe3));
            resources["GuestButton.Background"] = new SolidColorBrush(Color.FromRgb(0xfd, 0xfc, 0xfb));
            resources["GuestButton.Border"] = new SolidColorBrush(Color.FromRgb(0xdb, 0xda, 0xda));
            resources["Text.Primary"] = new SolidColorBrush(Colors.Black);
            resources["Text.Secondary"] = new SolidColorBrush(Color.FromRgb(0x66, 0x66, 0x66));
            
            // Settings colors
            resources["Settings.MainBg"] = new SolidColorBrush(Color.FromRgb(0xf8, 0xf4, 0xf1));
            resources["Settings.MainBgOutline"] = new SolidColorBrush(Color.FromRgb(0x9d, 0x9d, 0x9d));
            resources["Settings.SelectedListViewBg"] = new SolidColorBrush(Color.FromRgb(0xd1, 0xd1, 0xd1));
            resources["Settings.UnselectedListViewBg"] = new SolidColorBrush(Colors.Transparent);
            resources["Settings.PreviewBg"] = new SolidColorBrush(Colors.White);
            resources["Settings.PreviewOutline"] = new SolidColorBrush(Color.FromRgb(0xdd, 0xdd, 0xdd));
            resources["Settings.ThemeCardChecked"] = new SolidColorBrush(Color.FromRgb(0xff, 0xd9, 0xbd));
            resources["Settings.ThemeCardUnchecked"] = new SolidColorBrush(Color.FromRgb(0xe5, 0xe5, 0xe5));
            resources["Settings.CloseBtnBg"] = new SolidColorBrush(Color.FromRgb(0xd1, 0xd1, 0xd1));
            
            // Hover colors
            resources["Settings.ThemeCardUncheckedHover"] = new SolidColorBrush(Color.FromRgb(0xd8, 0xd8, 0xd8));
            resources["Settings.UnselectedListViewHover"] = new SolidColorBrush(Color.FromRgb(0xe0, 0xe0, 0xe0));
            resources["Settings.CloseBtnHover"] = new SolidColorBrush(Color.FromRgb(0xc0, 0xc0, 0xc0));
            resources["GuestButton.Hover"] = new SolidColorBrush(Color.FromRgb(0xf0, 0xef, 0xee));

            System.Diagnostics.Debug.WriteLine("Light theme applied directly to resources");
            ThemeChanged?.Invoke();
        }

        public static void ApplySystemTheme()
        {
            _isSystemMode = true;
            
            // Detect Windows theme from registry
            bool windowsUsesDarkMode = false;
            try
            {
                using (var key = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize"))
                {
                    if (key != null)
                    {
                        var value = key.GetValue("AppsUseLightTheme");
                        if (value != null)
                        {
                            // 0 = dark mode, 1 = light mode
                            windowsUsesDarkMode = (int)value == 0;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to detect Windows theme: {ex.Message}");
                windowsUsesDarkMode = true; // Default to dark
            }
            
            System.Diagnostics.Debug.WriteLine($"Windows uses dark mode: {windowsUsesDarkMode}");
            
            // Apply the appropriate theme colors but keep _isSystemMode = true
            if (windowsUsesDarkMode)
            {
                ApplyDarkThemeColors();
            }
            else
            {
                ApplyLightThemeColors();
            }
            
            // Reset to system mode since ApplyDark/LightThemeColors sets it to false
            _isSystemMode = true;
            ThemeChanged?.Invoke();
        }
        
        private static void ApplyDarkThemeColors()
        {
            _isDarkMode = true;
            var resources = Application.Current.Resources;
            
            // Dark theme colors
            resources["Background"] = new SolidColorBrush(Color.FromRgb(0x0e, 0x11, 0x1b));
            resources["CardContainer.Background"] = new SolidColorBrush(Color.FromRgb(0x15, 0x1a, 0x28));
            resources["CardContainer.Border"] = new SolidColorBrush(Color.FromRgb(0x29, 0x2e, 0x3b));
            resources["Card.Background"] = new SolidColorBrush(Color.FromRgb(0x22, 0x28, 0x3c));
            resources["Card.Border"] = new SolidColorBrush(Color.FromRgb(0x38, 0x3e, 0x50));
            resources["GuestButton.Background"] = new SolidColorBrush(Color.FromRgb(0x2d, 0x33, 0x48));
            resources["GuestButton.Border"] = new SolidColorBrush(Color.FromRgb(0x40, 0x45, 0x59));
            resources["Text.Primary"] = new SolidColorBrush(Colors.White);
            resources["Text.Secondary"] = new SolidColorBrush(Color.FromRgb(0xaa, 0xaa, 0xaa));
            
            // Settings colors
            resources["Settings.MainBg"] = new SolidColorBrush(Color.FromRgb(0x1c, 0x23, 0x38));
            resources["Settings.MainBgOutline"] = new SolidColorBrush(Color.FromRgb(0x38, 0x41, 0x5b));
            resources["Settings.SelectedListViewBg"] = new SolidColorBrush(Color.FromRgb(0x15, 0x1a, 0x28));
            resources["Settings.UnselectedListViewBg"] = new SolidColorBrush(Colors.Transparent);
            resources["Settings.PreviewBg"] = new SolidColorBrush(Color.FromRgb(0x15, 0x1a, 0x28));
            resources["Settings.PreviewOutline"] = new SolidColorBrush(Color.FromRgb(0x15, 0x1a, 0x28));
            resources["Settings.ThemeCardChecked"] = new SolidColorBrush(Color.FromRgb(0x4a, 0x54, 0x71));
            resources["Settings.ThemeCardUnchecked"] = new SolidColorBrush(Color.FromRgb(0x1b, 0x21, 0x32));
            resources["Settings.CloseBtnBg"] = new SolidColorBrush(Color.FromRgb(0x15, 0x1a, 0x28));
            
            // Hover colors
            resources["Settings.ThemeCardUncheckedHover"] = new SolidColorBrush(Color.FromRgb(0x25, 0x2c, 0x42));
            resources["Settings.UnselectedListViewHover"] = new SolidColorBrush(Color.FromRgb(0x1a, 0x20, 0x30));
            resources["Settings.CloseBtnHover"] = new SolidColorBrush(Color.FromRgb(0x25, 0x2c, 0x42));
            resources["GuestButton.Hover"] = new SolidColorBrush(Color.FromRgb(0x38, 0x40, 0x58));
        }
        
        private static void ApplyLightThemeColors()
        {
            _isDarkMode = false;
            var resources = Application.Current.Resources;
            
            // Light theme colors
            resources["Background"] = new SolidColorBrush(Color.FromRgb(0xef, 0xea, 0xe7));
            resources["CardContainer.Background"] = new SolidColorBrush(Color.FromRgb(0xf8, 0xf4, 0xf1));
            resources["CardContainer.Border"] = new SolidColorBrush(Color.FromRgb(0xcb, 0xcb, 0xcc));
            resources["Card.Background"] = new SolidColorBrush(Color.FromRgb(0xfb, 0xf9, 0xf7));
            resources["Card.Border"] = new SolidColorBrush(Color.FromRgb(0xe3, 0xe3, 0xe3));
            resources["GuestButton.Background"] = new SolidColorBrush(Color.FromRgb(0xfd, 0xfc, 0xfb));
            resources["GuestButton.Border"] = new SolidColorBrush(Color.FromRgb(0xdb, 0xda, 0xda));
            resources["Text.Primary"] = new SolidColorBrush(Colors.Black);
            resources["Text.Secondary"] = new SolidColorBrush(Color.FromRgb(0x66, 0x66, 0x66));
            
            // Settings colors
            resources["Settings.MainBg"] = new SolidColorBrush(Color.FromRgb(0xf8, 0xf4, 0xf1));
            resources["Settings.MainBgOutline"] = new SolidColorBrush(Color.FromRgb(0x9d, 0x9d, 0x9d));
            resources["Settings.SelectedListViewBg"] = new SolidColorBrush(Color.FromRgb(0xd1, 0xd1, 0xd1));
            resources["Settings.UnselectedListViewBg"] = new SolidColorBrush(Colors.Transparent);
            resources["Settings.PreviewBg"] = new SolidColorBrush(Colors.White);
            resources["Settings.PreviewOutline"] = new SolidColorBrush(Color.FromRgb(0xdd, 0xdd, 0xdd));
            resources["Settings.ThemeCardChecked"] = new SolidColorBrush(Color.FromRgb(0xff, 0xd9, 0xbd));
            resources["Settings.ThemeCardUnchecked"] = new SolidColorBrush(Color.FromRgb(0xe5, 0xe5, 0xe5));
            resources["Settings.CloseBtnBg"] = new SolidColorBrush(Color.FromRgb(0xd1, 0xd1, 0xd1));
            
            // Hover colors
            resources["Settings.ThemeCardUncheckedHover"] = new SolidColorBrush(Color.FromRgb(0xd8, 0xd8, 0xd8));
            resources["Settings.UnselectedListViewHover"] = new SolidColorBrush(Color.FromRgb(0xe0, 0xe0, 0xe0));
            resources["Settings.CloseBtnHover"] = new SolidColorBrush(Color.FromRgb(0xc0, 0xc0, 0xc0));
            resources["GuestButton.Hover"] = new SolidColorBrush(Color.FromRgb(0xf0, 0xef, 0xee));
        }

        public static string? GetIconPath(string iconName)
        {
            string themePath = _isDarkMode ? "Dark" : "Light";
            string basePath = AppDomain.CurrentDomain.BaseDirectory;
            
            // Try SVG first, then PNG
            string[] extensions = { ".svg", ".png" };
            
            foreach (var ext in extensions)
            {
                string iconPath = System.IO.Path.Combine(basePath, "Assets", "Icons", themePath, $"{iconName}{ext}");
                if (System.IO.File.Exists(iconPath))
                {
                    return iconPath;
                }
            }
            
            return null;
        }
    }
}