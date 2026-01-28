using System;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
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
            LoadAboutIcon();
            
            // Set dynamic copyright year
            CopyrightText.Text = $"© {DateTime.Now.Year} E-Launchpad contributors";
            
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
                    FileName = "https://github.com/iSubhamMani/e-launchpad",
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
            // Open feedback form in default browser
            try
            {
                var psi = new System.Diagnostics.ProcessStartInfo
                {
                    FileName = "https://tally.so/",
                    UseShellExecute = true
                };
                System.Diagnostics.Process.Start(psi);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to open feedback link: {ex.Message}");
            }
        }

        private void PrivacyButton_Click(object sender, MouseButtonEventArgs e)
        {
            SelectNavigationButton(PrivacyButton);
            ShowContent(PrivacyContent);
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
            AboutButton.Background = (SolidColorBrush)FindResource("Settings.UnselectedListViewBg");
            
            // Set font weight to normal
            ((System.Windows.Controls.TextBlock)ThemeButton.Child).FontWeight = FontWeights.Normal;
            ((System.Windows.Controls.TextBlock)FeedbackButton.Child).FontWeight = FontWeights.Normal;
            ((System.Windows.Controls.TextBlock)PrivacyButton.Child).FontWeight = FontWeights.Normal;
            ((System.Windows.Controls.TextBlock)AboutButton.Child).FontWeight = FontWeights.Normal;
            
            // Highlight selected button
            selectedButton.Background = (SolidColorBrush)FindResource("Settings.SelectedListViewBg");
            ((System.Windows.Controls.TextBlock)selectedButton.Child).FontWeight = FontWeights.SemiBold;
            
            // Track the selected button
            _selectedNavButton = selectedButton;
        }

        private void ShowContent(System.Windows.Controls.StackPanel contentToShow)
        {
            // Hide all content
            ThemeContent.Visibility = Visibility.Collapsed;
            PrivacyContent.Visibility = Visibility.Collapsed;
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
            string email = "subham@example.com";
            
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
