using System.Diagnostics;
using System.IO;
using E_Launchpad.Models;
using E_Launchpad.Utils;
using Newtonsoft.Json.Linq;

namespace E_Launchpad.Services
{
    /// <summary>
    /// Service for managing Edge browser profiles
    /// </summary>
    public class ProfileService
    {
        private readonly string _edgeUserDataPath;
        private readonly string _edgeExecutablePath;

        public ProfileService()
        {
            // Get Edge User Data folder
            string localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            _edgeUserDataPath = Path.Combine(localAppData, "Microsoft", "Edge", "User Data");

            // Try to find Edge executable
            _edgeExecutablePath = FindEdgeExecutable();
        }

        /// <summary>
        /// Finds the Edge executable path
        /// </summary>
        private string FindEdgeExecutable()
        {
            // Try common locations
            string[] possiblePaths =
            {
                @"C:\Program Files (x86)\Microsoft\Edge\Application\msedge.exe",
                @"C:\Program Files\Microsoft\Edge\Application\msedge.exe"
            };

            foreach (var path in possiblePaths)
            {
                if (File.Exists(path))
                    return path;
            }

            return possiblePaths[0]; // Default to first path
        }

        /// <summary>
        /// Scans the Edge User Data folder and returns all valid profiles
        /// </summary>
        public async Task<List<EdgeProfile>> GetProfilesAsync()
        {
            var profiles = new List<EdgeProfile>();

            if (!Directory.Exists(_edgeUserDataPath))
                return profiles;

            // Scan profile directories in parallel
            var directories = Directory.GetDirectories(_edgeUserDataPath);
            var tasks = new List<Task<EdgeProfile?>>();

            foreach (var dir in directories)
            {
                string folderName = Path.GetFileName(dir);

                // Only process Default and Profile N folders
                if (folderName == "Default" || folderName.StartsWith("Profile "))
                {
                    tasks.Add(LoadProfileAsync(dir, folderName));
                }
            }

            var results = await Task.WhenAll(tasks);

            // Add valid profiles
            foreach (var profile in results)
            {
                if (profile != null)
                    profiles.Add(profile);
            }

            // Add "Add New Profile" card
            profiles.Add(new EdgeProfile
            {
                Name = "Add new Profile",
                Folder = "",
                FullPath = "",
                IsAddNew = true,
                Avatar = null
            });

            return profiles;
        }

        /// <summary>
        /// Loads a single profile from a directory
        /// </summary>
        private async Task<EdgeProfile?> LoadProfileAsync(string profilePath, string folderName)
        {
            try
            {
                // Check if Preferences file exists to validate the profile
                string preferencesPath = Path.Combine(profilePath, "Preferences");
                if (!File.Exists(preferencesPath))
                    return null;

                // Read profile name from Preferences JSON
                string profileName = folderName;
                try
                {
                    string json = await File.ReadAllTextAsync(preferencesPath);
                    var preferences = JObject.Parse(json);
                    
                    // Try to get the profile name
                    var name = preferences["profile"]?["name"]?.ToString();
                    if (!string.IsNullOrWhiteSpace(name))
                        profileName = name;
                }
                catch
                {
                    // If JSON parsing fails, use folder name
                }

                // Load avatar in background
                var avatar = await Task.Run(() => IconExtractor.LoadProfileAvatar(profilePath));

                return new EdgeProfile
                {
                    Name = profileName,
                    Folder = folderName,
                    FullPath = profilePath,
                    Avatar = avatar,
                    IsGuest = false,
                    IsAddNew = false
                };
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Launches Edge with a specific profile
        /// </summary>
        public bool LaunchProfile(EdgeProfile profile)
        {
            try
            {
                if (!File.Exists(_edgeExecutablePath))
                    return false;

                var startInfo = new ProcessStartInfo
                {
                    FileName = _edgeExecutablePath,
                    UseShellExecute = true
                };

                if (profile.IsGuest)
                {
                    // Launch in Guest mode
                    startInfo.Arguments = "--guest";
                }
                else if (profile.IsAddNew)
                {
                    // Create new profile by finding next available number
                    int nextProfileNumber = FindNextProfileNumber();
                    string newProfileFolder = $"Profile {nextProfileNumber}";
                    startInfo.Arguments = $"--profile-directory=\"{newProfileFolder}\"";
                }
                else
                {
                    // Launch specific profile
                    startInfo.Arguments = $"--profile-directory=\"{profile.Folder}\"";
                }

                Process.Start(startInfo);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Finds the next available profile number
        /// </summary>
        private int FindNextProfileNumber()
        {
            int num = 1;
            while (true)
            {
                string folderName = $"Profile {num}";
                string profilePath = Path.Combine(_edgeUserDataPath, folderName);
                
                if (!Directory.Exists(profilePath))
                    return num;
                
                num++;
            }
        }

        /// <summary>
        /// Refreshes the profiles list (clears cache if any)
        /// </summary>
        public Task<List<EdgeProfile>> RefreshProfilesAsync()
        {
            return GetProfilesAsync();
        }
    }
}
