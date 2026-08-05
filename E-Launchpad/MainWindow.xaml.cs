using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using E_Launchpad.Models;
using E_Launchpad.Services;
using E_Launchpad.Utils;
using System.IO;

namespace E_Launchpad
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly ProfileService _profileService;

        public MainWindow()
        {
            InitializeComponent();
            _profileService = new ProfileService();
            
            // Initialize theme system
            ThemeManager.Initialize();
            ThemeManager.ThemeChanged += OnThemeChanged;
            
            Loaded += MainWindow_Loaded;
        }

        private void OnThemeChanged()
        {
            // Refresh icons when theme changes
            LoadThemeIcons();
            
            // Refresh the profiles ItemsControl to update add-profile icon
            RefreshProfilesDisplay();
        }

        private void RefreshProfilesDisplay()
        {
            // Force the ItemsControl to re-evaluate its bindings by refreshing the ItemsSource
            var currentItems = ProfilesItemsControl.ItemsSource;
            ProfilesItemsControl.ItemsSource = null;
            ProfilesItemsControl.ItemsSource = currentItems;
        }

        private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            LoadEdgeIcon();
            LoadSettingsIcon();
            LoadGuestIcon();
            await LoadProfilesAsync();
        }

        private void LoadSettingsIcon()
        {
            string? iconPath = ThemeManager.GetIconPath("settings");
            
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
                    
                    SettingsIcon.Source = bitmap;
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Failed to load settings icon: {ex.Message}");
                }
            }
        }

        private void LoadThemeIcons()
        {
            LoadSettingsIcon();
            LoadGuestIcon();
        }

        private void LoadGuestIcon()
        {
            string? iconPath = ThemeManager.GetIconPath("guest");
            
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
                    
                    GuestIcon.Source = bitmap;
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Failed to load guest icon: {ex.Message}");
                }
            }
        }

        private void LoadEdgeIcon()
        {
            // Use the app's own icon on the home screen
            string basePath = AppDomain.CurrentDomain.BaseDirectory;
            string iconPath = System.IO.Path.Combine(basePath, "Assets", "icon.png");

            if (File.Exists(iconPath))
            {
                try
                {
                    var bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.UriSource = new Uri(iconPath, UriKind.Absolute);
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    bitmap.EndInit();
                    bitmap.Freeze();

                    EdgeIconImage.Source = bitmap;
                    System.Diagnostics.Debug.WriteLine("Loaded app icon for home screen");
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Failed to load app icon: {ex.Message}");
                }
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("App icon not found");
            }
        }

        private async Task LoadProfilesAsync()
        {
            try
            {
                // Show loading overlay
                LoadingOverlay.Visibility = Visibility.Visible;

                // Load profiles
                var profiles = await _profileService.GetProfilesAsync();

                // Update UI
                ProfilesItemsControl.ItemsSource = profiles;

                // Hide loading overlay
                LoadingOverlay.Visibility = Visibility.Collapsed;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading profiles: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                LoadingOverlay.Visibility = Visibility.Collapsed;
            }
        }

        private async void ProfileCard_Click(object sender, MouseButtonEventArgs e)
        {
            if (sender is Border border && border.Tag is EdgeProfile profile)
            {
                try
                {
                    // Launch Edge with selected profile
                    bool success = _profileService.LaunchProfile(profile);

                    if (success)
                    {
                        // Wait a moment for Edge to start
                        await Task.Delay(500);

                        // Close the launcher app
                        Application.Current.Shutdown();
                    }
                    else
                    {
                        MessageBox.Show("Failed to launch Edge. Please check that Edge is installed.", 
                                      "Launch Error", 
                                      MessageBoxButton.OK, 
                                      MessageBoxImage.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error launching profile: {ex.Message}", 
                                  "Error", 
                                  MessageBoxButton.OK, 
                                  MessageBoxImage.Error);
                }
            }
        }

        private async void GuestButton_Click(object sender, MouseButtonEventArgs e)
        {
            try
            {
                // Create a guest profile object
                var guestProfile = new EdgeProfile
                {
                    Name = "Guest",
                    Folder = "Guest",
                    IsGuest = true
                };

                // Launch Edge in guest mode
                bool success = _profileService.LaunchProfile(guestProfile);

                if (success)
                {
                    await Task.Delay(500);
                    Application.Current.Shutdown();
                }
                else
                {
                    MessageBox.Show("Failed to launch Edge in guest mode.", 
                                  "Launch Error", 
                                  MessageBoxButton.OK, 
                                  MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error launching guest mode: {ex.Message}", 
                              "Error", 
                              MessageBoxButton.OK, 
                              MessageBoxImage.Error);
            }
        }

        private void SettingsButton_Click(object sender, MouseButtonEventArgs e)
        {
            var settingsWindow = new Views.SettingsWindow()
            {
                Owner = this
            };
            settingsWindow.ShowDialog();
        }

        public void RefreshIcons()
        {
            LoadThemeIcons();
        }

        // Title bar button handlers
        private void MinimizeButton_Click(object sender, MouseButtonEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void CloseButton_Click(object sender, MouseButtonEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void TitleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        // Opens the feedback page (Page 2) and loads the feedback form in WebView2.
        // Falls back to the default browser when the WebView2 runtime is unavailable.
        public async void OpenFeedbackPage()
        {
            if (IsWebView2RuntimeAvailable())
            {
                HomePage.Visibility = Visibility.Collapsed;
                FeedbackPageView.Visibility = Visibility.Visible;

                try
                {
                    await FeedbackWebView.EnsureCoreWebView2Async();
                    FeedbackWebView.CoreWebView2.Navigate("https://tally.so/r/Np7aNW");
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Failed to load feedback in WebView2: {ex.Message}");
                    OpenFeedbackInDefaultBrowser();
                }
            }
            else
            {
                OpenFeedbackInDefaultBrowser();
            }
        }

        private void FeedbackBackButton_Click(object sender, MouseButtonEventArgs e)
        {
            FeedbackPageView.Visibility = Visibility.Collapsed;
            HomePage.Visibility = Visibility.Visible;
        }

        private void OpenFeedbackInDefaultBrowser()
        {
            try
            {
                var psi = new System.Diagnostics.ProcessStartInfo
                {
                    FileName = "https://tally.so/r/Np7aNW",
                    UseShellExecute = true
                };
                System.Diagnostics.Process.Start(psi);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to open feedback link: {ex.Message}");
            }
        }

        private bool IsWebView2RuntimeAvailable()
        {
            try
            {
                var version = Microsoft.Web.WebView2.Core.CoreWebView2Environment.GetAvailableBrowserVersionString();
                return !string.IsNullOrEmpty(version);
            }
            catch
            {
                return false;
            }
        }
    }
}