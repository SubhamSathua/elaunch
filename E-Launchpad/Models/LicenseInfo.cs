namespace E_Launchpad.Models
{
    /// <summary>
    /// Represents an open-source license entry from the license manifest.
    /// </summary>
    public class LicenseInfo
    {
        public string Name { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string Path { get; set; } = string.Empty;
    }
}
