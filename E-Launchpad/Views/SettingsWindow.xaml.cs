using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Collections.Generic;
using System.Text.Json;
using E_Launchpad.Models;
using E_Launchpad.Services;

namespace E_Launchpad.Views
{
    public partial class SettingsWindow : Window
    {
        private string _currentTheme = "Dark"; // Will be synced with actual state
        private System.Windows.Controls.Border? _selectedNavButton;

        public SettingsWindow()
        {
            InitializeComponent();
            
            Loaded += SettingsWindow_Loaded;
            Closed += SettingsWindow_Closed;
            
            // Match owner window size when owner is set
            SourceInitialized += SettingsWindow_SourceInitialized;
            
            // Subscribe to theme changes
            ThemeManager.ThemeChanged += OnThemeChanged;
        }

        private void SettingsWindow_Closed(object? sender, EventArgs e)
        {
            // Unsubscribe from theme changes to prevent memory leaks
            ThemeManager.ThemeChanged -= OnThemeChanged;
        }

        private void OnThemeChanged()
        {
            // Reload icons for the new theme
            LoadIcons();
            
            // Refresh close button background
            CloseButton.Background = (SolidColorBrush)FindResource("Settings.CloseBtnBg");
            
            // Refresh selected navigation button background
            if (_selectedNavButton != null)
            {
                RefreshNavigationButtonStyles();
            }
            
            // Update theme cards with new colors
            SyncWithCurrentTheme();
            UpdateThemeCards();
        }

        private void RefreshNavigationButtonStyles()
        {
            // Reset all buttons to unselected
            ThemeButton.Background = (SolidColorBrush)FindResource("Settings.UnselectedListViewBg");
            FeedbackButton.Background = (SolidColorBrush)FindResource("Settings.UnselectedListViewBg");
            PrivacyButton.Background = (SolidColorBrush)FindResource("Settings.UnselectedListViewBg");
            LicenseButton.Background = (SolidColorBrush)FindResource("Settings.UnselectedListViewBg");
            AboutButton.Background = (SolidColorBrush)FindResource("Settings.UnselectedListViewBg");
            
            // Re-apply selection to current selected button
            if (_selectedNavButton != null)
            {
                _selectedNavButton.Background = (SolidColorBrush)FindResource("Settings.SelectedListViewBg");
            }
        }

        private void SettingsWindow_SourceInitialized(object? sender, EventArgs e)
        {
            // Match the owner's size and position
            if (Owner != null)
            {
                this.Left = Owner.Left;
                this.Top = Owner.Top;
                this.Width = Owner.ActualWidth;
                this.Height = Owner.ActualHeight;
            }
        }

        private void SettingsWindow_Loaded(object sender, RoutedEventArgs e)
        {
            LoadIcons();
            LoadPrivacyPolicy();
            LoadOpenSourceLicenses();
            LoadAboutIcon();
            
            // Set dynamic copyright year
            CopyrightText.Text = $"© {DateTime.Now.Year} {Branding.AppBrand} contributors";
            
            // Set Theme button as selected by default
            SelectNavigationButton(ThemeButton);
            
            // Sync with actual theme state
            SyncWithCurrentTheme();
            
            // Update theme cards to reflect current state
            UpdateThemeCards();
        }
        
        private void LoadAboutIcon()
        {
            try
            {
                string basePath = AppDomain.CurrentDomain.BaseDirectory;
                string iconPath = System.IO.Path.Combine(basePath, "Assets", "icon.png");
                
                if (System.IO.File.Exists(iconPath))
                {
                    var bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.UriSource = new Uri(iconPath, UriKind.Absolute);
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    bitmap.EndInit();
                    bitmap.Freeze();
                    
                    AboutAppIcon.Source = bitmap;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to load about icon: {ex.Message}");
            }
        }
        
        private void SourceCodeLink_Click(object sender, MouseButtonEventArgs e)
        {
            try
            {
                var psi = new System.Diagnostics.ProcessStartInfo
                {
                    FileName = "https://github.com/Subham-x/E-Launchpad",
                    UseShellExecute = true
                };
                System.Diagnostics.Process.Start(psi);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to open link: {ex.Message}");
            }
        }
        
        private void LoadPrivacyPolicy()
        {
            try
            {
                string basePath = AppDomain.CurrentDomain.BaseDirectory;
                string privacyPath = System.IO.Path.Combine(basePath, "Assets", "privacy-policy.txt");
                
                if (System.IO.File.Exists(privacyPath))
                {
                    PrivacyPolicyText.Text = System.IO.File.ReadAllText(privacyPath);
                }
                else
                {
                    PrivacyPolicyText.Text = "Privacy policy file not found.";
                }
            }
            catch (Exception ex)
            {
                PrivacyPolicyText.Text = $"Failed to load privacy policy: {ex.Message}";
            }
        }
        
        private void LoadOpenSourceLicenses()
        {
            try
            {
                string basePath = AppDomain.CurrentDomain.BaseDirectory;
                string licensesPath = System.IO.Path.Combine(basePath, "license", "open-source license.json");
                
                if (!System.IO.File.Exists(licensesPath))
                {
                    LicenseListPanel.Children.Add(CreateLicenseErrorItem("License manifest not found."));
                    return;
                }
                
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var licenses = JsonSerializer.Deserialize<List<LicenseInfo>>(
                    System.IO.File.ReadAllText(licensesPath), options);
                
                LicenseListPanel.Children.Clear();
                
                if (licenses == null || licenses.Count == 0)
                {
                    LicenseListPanel.Children.Add(CreateLicenseErrorItem("No open-source licenses listed."));
                    return;
                }
                
                foreach (var license in licenses)
                {
                    var item = new Border
                    {
                        Background = (SolidColorBrush)FindResource("Settings.UnselectedListViewBg"),
                        CornerRadius = new CornerRadius(10),
                        Padding = new Thickness(15, 12, 15, 12),
                        Margin = new Thickness(0, 0, 0, 8),
                        Cursor = Cursors.Hand,
                        Tag = license
                    };
                    item.MouseLeftButtonDown += LicenseItem_Click;
                    item.MouseEnter += LicenseItem_MouseEnter;
                    item.MouseLeave += LicenseItem_MouseLeave;
                    
                    var stack = new StackPanel();
                    stack.Children.Add(new TextBlock
                    {
                        Text = license.Name,
                        Foreground = (SolidColorBrush)FindResource("Text.Primary"),
                        FontSize = 14,
                        FontWeight = FontWeights.SemiBold
                    });
                    stack.Children.Add(new TextBlock
                    {
                        Text = license.Type,
                        Foreground = (SolidColorBrush)FindResource("Text.Secondary"),
                        FontSize = 12,
                        Margin = new Thickness(0, 2, 0, 0)
                    });
                    
                    item.Child = stack;
                    LicenseListPanel.Children.Add(item);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to load licenses: {ex.Message}");
                LicenseListPanel.Children.Add(CreateLicenseErrorItem($"Failed to load licenses: {ex.Message}"));
            }
        }
        
        private System.Windows.Controls.TextBlock CreateLicenseErrorItem(string message)
        {
            return new System.Windows.Controls.TextBlock
            {
                Text = message,
                Foreground = (SolidColorBrush)FindResource("Text.Secondary"),
                FontSize = 12,
                TextWrapping = TextWrapping.Wrap,
                Margin = new Thickness(5, 5, 5, 5)
            };
        }
        
        private void LicenseItem_Click(object sender, MouseButtonEventArgs e)
        {
            if (sender is not Border item || item.Tag is not LicenseInfo license) return;
            
            try
            {
                string licensePath = System.IO.Path.Combine(
                    AppDomain.CurrentDomain.BaseDirectory, "license", license.Path);
                
                LicenseDetailText.Text = System.IO.File.Exists(licensePath)
                    ? System.IO.File.ReadAllText(licensePath)
                    : "License file not found.";
            }
            catch (Exception ex)
            {
                LicenseDetailText.Text = $"Failed to load license: {ex.Message}";
            }
            
            LicenseListView.Visibility = Visibility.Collapsed;
            LicenseDetailView.Visibility = Visibility.Visible;
        }
        
        private void LicenseItem_MouseEnter(object sender, MouseEventArgs e)
        {
            var item = sender as Border;
            if (item != null)
            {
                item.Background = (SolidColorBrush)FindResource("Settings.UnselectedListViewHover");
            }
        }
        
        private void LicenseItem_MouseLeave(object sender, MouseEventArgs e)
        {
            var item = sender as Border;
            if (item != null)
            {
                item.Background = (SolidColorBrush)FindResource("Settings.UnselectedListViewBg");
            }
        }
        
        private void SyncWithCurrentTheme()
        {
            // Check if system mode is active first
            if (ThemeManager.IsSystemMode)
            {
                _currentTheme = "System";
            }
            else if (ThemeManager.IsDarkMode)
            {
                _currentTheme = "Dark";
            }
            else
            {
                _currentTheme = "Light"; 
            }
            
            System.Diagnostics.Debug.WriteLine($"SettingsWindow synced to theme: {_currentTheme}");
        }

        private void LoadIcons()
        {
            // Load close icon
            LoadIcon(CloseIcon, "close");
            
            // Load theme icons
            LoadIcon(SystemThemeIcon, "system");
            LoadIcon(DayThemeIcon, "light");
            LoadIcon(NightThemeIcon, "dark");
        }

        private void LoadIcon(System.Windows.Controls.Image imageControl, string iconName)
        {
            string? iconPath = ThemeManager.GetIconPath(iconName);
            
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
                    
                    imageControl.Source = bitmap;
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Failed to load icon {iconName}: {ex.Message}");
                }
            }
        }

        private void CloseButton_Click(object sender, MouseButtonEventArgs e)
        {
            this.Close();
        }

        private void ThemeButton_Click(object sender, MouseButtonEventArgs e)
        {
            SelectNavigationButton(ThemeButton);
            ShowContent(ThemeContent);
        }

        private void FeedbackButton_Click(object sender, MouseButtonEventArgs e)
        {
            // Close the settings popup and open the feedback page in the main window.
            var mainWindow = Owner as MainWindow;
            Close();
            mainWindow?.OpenFeedbackPage();
        }

        private void PrivacyButton_Click(object sender, MouseButtonEventArgs e)
        {
            SelectNavigationButton(PrivacyButton);
            ShowContent(PrivacyContent);
        }

        private void LicenseButton_Click(object sender, MouseButtonEventArgs e)
        {
            SelectNavigationButton(LicenseButton);
            ShowContent(LicenseContent);
            
            // Reset to list view if a detail view was showing
            LicenseDetailView.Visibility = Visibility.Collapsed;
            LicenseListView.Visibility = Visibility.Visible;
        }

        private void LicenseBackButton_Click(object sender, MouseButtonEventArgs e)
        {
            LicenseDetailView.Visibility = Visibility.Collapsed;
            LicenseListView.Visibility = Visibility.Visible;
        }

        private void AboutButton_Click(object sender, MouseButtonEventArgs e)
        {
            SelectNavigationButton(AboutButton);
            ShowContent(AboutContent);
        }

        private void SelectNavigationButton(System.Windows.Controls.Border selectedButton)
        {
            // Reset all buttons
            ThemeButton.Background = (SolidColorBrush)FindResource("Settings.UnselectedListViewBg");
            FeedbackButton.Background = (SolidColorBrush)FindResource("Settings.UnselectedListViewBg");
            PrivacyButton.Background = (SolidColorBrush)FindResource("Settings.UnselectedListViewBg");
            LicenseButton.Background = (SolidColorBrush)FindResource("Settings.UnselectedListViewBg");
            AboutButton.Background = (SolidColorBrush)FindResource("Settings.UnselectedListViewBg");
            
            // Set font weight to normal
            ((System.Windows.Controls.TextBlock)ThemeButton.Child).FontWeight = FontWeights.Normal;
            ((System.Windows.Controls.TextBlock)FeedbackButton.Child).FontWeight = FontWeights.Normal;
            ((System.Windows.Controls.TextBlock)PrivacyButton.Child).FontWeight = FontWeights.Normal;
            ((System.Windows.Controls.TextBlock)LicenseButton.Child).FontWeight = FontWeights.Normal;
            ((System.Windows.Controls.TextBlock)AboutButton.Child).FontWeight = FontWeights.Normal;
            
            // Highlight selected button
            selectedButton.Background = (SolidColorBrush)FindResource("Settings.SelectedListViewBg");
            ((System.Windows.Controls.TextBlock)selectedButton.Child).FontWeight = FontWeights.SemiBold;
            
            // Track the selected button
            _selectedNavButton = selectedButton;
        }

        private void ShowContent(System.Windows.UIElement contentToShow)
        {
            // Hide all content
            ThemeContent.Visibility = Visibility.Collapsed;
            PrivacyContent.Visibility = Visibility.Collapsed;
            LicenseContent.Visibility = Visibility.Collapsed;
            AboutContent.Visibility = Visibility.Collapsed;
            
            // Show selected content
            contentToShow.Visibility = Visibility.Visible;
        }

        private void SystemTheme_Click(object sender, MouseButtonEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("SystemTheme_Click called");
            _currentTheme = "System";
            ThemeManager.ApplySystemTheme();
            UpdateThemeCards();
        }

        private void DayTheme_Click(object sender, MouseButtonEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("=== DayTheme_Click START ===");
            System.Diagnostics.Debug.WriteLine($"Before: ThemeManager.IsDarkMode = {ThemeManager.IsDarkMode}");
            
            _currentTheme = "Light";
            
            // Test current background color before theme change
            var currentBg = Application.Current.Resources["Background"] as SolidColorBrush;
            System.Diagnostics.Debug.WriteLine($"Background before change: {currentBg?.Color}");
            
            ThemeManager.ApplyLightTheme();
            
            // Test background color after theme change
            var newBg = Application.Current.Resources["Background"] as SolidColorBrush;
            System.Diagnostics.Debug.WriteLine($"Background after change: {newBg?.Color}");
            System.Diagnostics.Debug.WriteLine($"After: ThemeManager.IsDarkMode = {ThemeManager.IsDarkMode}");
            
            UpdateThemeCards();
            System.Diagnostics.Debug.WriteLine("=== DayTheme_Click END ===");
        }

        private void NightTheme_Click(object sender, MouseButtonEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("NightTheme_Click called - switching to Dark theme");
            _currentTheme = "Dark";
            ThemeManager.ApplyDarkTheme();
            UpdateThemeCards();
        }

        private void UpdateThemeCards()
        {
            System.Diagnostics.Debug.WriteLine($"UpdateThemeCards called with _currentTheme: {_currentTheme}");
            
            // Reset all cards
            SystemThemeCard.Background = (SolidColorBrush)FindResource("Settings.ThemeCardUnchecked");
            DayThemeCard.Background = (SolidColorBrush)FindResource("Settings.ThemeCardUnchecked");
            NightThemeCard.Background = (SolidColorBrush)FindResource("Settings.ThemeCardUnchecked");
            
            // Highlight selected card based on current theme
            switch (_currentTheme)
            {
                case "System":
                    SystemThemeCard.Background = (SolidColorBrush)FindResource("Settings.ThemeCardChecked");
                    System.Diagnostics.Debug.WriteLine("System card highlighted");
                    break;
                case "Light":
                    DayThemeCard.Background = (SolidColorBrush)FindResource("Settings.ThemeCardChecked");
                    System.Diagnostics.Debug.WriteLine("Day card highlighted");
                    break;
                case "Dark":
                    NightThemeCard.Background = (SolidColorBrush)FindResource("Settings.ThemeCardChecked");
                    System.Diagnostics.Debug.WriteLine("Night card highlighted");
                    break;
            }
        }

        // ===== HOVER EVENT HANDLERS =====

        private void CloseButton_MouseEnter(object sender, MouseEventArgs e)
        {
            CloseButton.Background = (SolidColorBrush)FindResource("Settings.CloseBtnHover");
        }

        private void CloseButton_MouseLeave(object sender, MouseEventArgs e)
        {
            CloseButton.Background = (SolidColorBrush)FindResource("Settings.CloseBtnBg");
        }

        private void NavButton_MouseEnter(object sender, MouseEventArgs e)
        {
            var button = sender as System.Windows.Controls.Border;
            if (button != null && button != _selectedNavButton)
            {
                button.Background = (SolidColorBrush)FindResource("Settings.UnselectedListViewHover");
            }
        }

        private void NavButton_MouseLeave(object sender, MouseEventArgs e)
        {
            var button = sender as System.Windows.Controls.Border;
            if (button != null && button != _selectedNavButton)
            {
                button.Background = (SolidColorBrush)FindResource("Settings.UnselectedListViewBg");
            }
        }

        private void ThemeCard_MouseEnter(object sender, MouseEventArgs e)
        {
            var card = sender as System.Windows.Controls.Border;
            if (card == null) return;

            // Only apply hover if the card is not currently selected
            bool isSelected = (card == SystemThemeCard && _currentTheme == "System") ||
                             (card == DayThemeCard && _currentTheme == "Light") ||
                             (card == NightThemeCard && _currentTheme == "Dark");

            if (!isSelected)
            {
                card.Background = (SolidColorBrush)FindResource("Settings.ThemeCardUncheckedHover");
            }
        }

        private void ThemeCard_MouseLeave(object sender, MouseEventArgs e)
        {
            var card = sender as System.Windows.Controls.Border;
            if (card == null) return;

            // Only reset if the card is not currently selected
            bool isSelected = (card == SystemThemeCard && _currentTheme == "System") ||
                             (card == DayThemeCard && _currentTheme == "Light") ||
                             (card == NightThemeCard && _currentTheme == "Dark");

            if (!isSelected)
            {
                card.Background = (SolidColorBrush)FindResource("Settings.ThemeCardUnchecked");
            }
        }

        // ===== EMAIL AND TOAST HANDLERS =====

        private void EmailLink_Click(object sender, MouseButtonEventArgs e)
        {
            string email = "hyper.devstudio@protonmail.com";
            
            // Open mail app
            try
            {
                var psi = new System.Diagnostics.ProcessStartInfo
                {
                    FileName = $"mailto:{email}",
                    UseShellExecute = true
                };
                System.Diagnostics.Process.Start(psi);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to open mail app: {ex.Message}");
            }
            
            // Copy email to clipboard
            System.Windows.Clipboard.SetText(email);
            
            // Show toast notification
            ShowToast("Email copied!");
        }

        private System.Windows.Threading.DispatcherTimer? _toastTimer;

        private void ShowToast(string message)
        {
            ToastText.Text = message;
            ToastNotification.Visibility = Visibility.Visible;
            ToastNotification.Opacity = 1;
            
            // Cancel previous timer if exists
            _toastTimer?.Stop();
            
            // Create timer to hide toast after 4 seconds
            _toastTimer = new System.Windows.Threading.DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(4)
            };
            _toastTimer.Tick += (s, e) =>
            {
                _toastTimer.Stop();
                ToastNotification.Visibility = Visibility.Collapsed;
            };
            _toastTimer.Start();
        }
    }
}
