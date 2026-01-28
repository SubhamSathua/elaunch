using System.Windows.Media.Imaging;

namespace E_Launchpad.Models
{
    /// <summary>
    /// Represents an Edge browser profile
    /// </summary>
    public class EdgeProfile
    {
        /// <summary>
        /// Display name of the profile (read from Preferences)
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Folder name (e.g., "Default", "Profile 1", "Guest")
        /// </summary>
        public string Folder { get; set; } = string.Empty;

        /// <summary>
        /// Full path to the profile directory
        /// </summary>
        public string FullPath { get; set; } = string.Empty;

        /// <summary>
        /// Profile avatar/icon image
        /// </summary>
        public BitmapSource? Avatar { get; set; }

        /// <summary>
        /// Whether this is the Guest profile
        /// </summary>
        public bool IsGuest { get; set; }

        /// <summary>
        /// Whether this is the "Add New Profile" card
        /// </summary>
        public bool IsAddNew { get; set; }
    }
}
