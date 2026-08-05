using System;
using System.IO;
using System.Text.Json;

namespace E_Launchpad.Services
{
    /// <summary>
    /// Loads user-facing branding from branding.json (next to the exe).
    /// Every value falls back to a sensible default when the file is missing
    /// or a key is absent, so the app always shows something reasonable.
    /// </summary>
    public static class Branding
    {
        private const string FileName = "branding.json";

        public static string AppName { get; private set; } = "e_launchpad";
        public static string AppBrand { get; private set; } = "E Launchpad";
        public static string TitleBar { get; private set; } = "E Launchpad Profile Launcher";
        public static string HomePageTitle { get; private set; } = "Who's using E Launchpad?";
        public static string HomePageSubtitle { get; private set; } = "Select your profile to continue";
        public static string GuestModeLabel { get; private set; } = "Browse as Guest";
        public static string DownloadMessage { get; private set; } = "Download Edge For Windows to use E-Launcher";
        public static string DownloadButton { get; private set; } = "Download Edge For Windows";
        public static string AboutDisclaimer { get; private set; } =
            "E Launchpad is an independent, third-party application. It is not affiliated with, endorsed by, or sponsored by Microsoft or Microsoft Edge.";

        public static void Initialize()
        {
            try
            {
                string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, FileName);
                if (!File.Exists(path))
                {
                    System.Diagnostics.Debug.WriteLine($"Branding file not found at {path}; using defaults.");
                    return;
                }

                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var data = JsonSerializer.Deserialize<BrandingData>(File.ReadAllText(path), options);
                if (data == null) return;

                if (!string.IsNullOrEmpty(data.AppName)) AppName = data.AppName;
                if (!string.IsNullOrEmpty(data.AppBrand)) AppBrand = data.AppBrand;
                if (!string.IsNullOrEmpty(data.TitleBar)) TitleBar = data.TitleBar;
                if (!string.IsNullOrEmpty(data.HomePageTitle)) HomePageTitle = data.HomePageTitle;
                if (!string.IsNullOrEmpty(data.HomePageSubtitle)) HomePageSubtitle = data.HomePageSubtitle;
                if (!string.IsNullOrEmpty(data.GuestModeLabel)) GuestModeLabel = data.GuestModeLabel;
                if (!string.IsNullOrEmpty(data.DownloadMessage)) DownloadMessage = data.DownloadMessage;
                if (!string.IsNullOrEmpty(data.DownloadButton)) DownloadButton = data.DownloadButton;
                if (!string.IsNullOrEmpty(data.AboutDisclaimer)) AboutDisclaimer = data.AboutDisclaimer;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to load branding: {ex.Message}");
            }
        }

        private class BrandingData
        {
            public string? AppName { get; set; }
            public string? AppBrand { get; set; }
            public string? TitleBar { get; set; }
            public string? HomePageTitle { get; set; }
            public string? HomePageSubtitle { get; set; }
            public string? GuestModeLabel { get; set; }
            public string? DownloadMessage { get; set; }
            public string? DownloadButton { get; set; }
            public string? AboutDisclaimer { get; set; }
        }
    }
}
