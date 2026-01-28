using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace E_Launchpad.Utils
{
    /// <summary>
    /// Utility class to extract icons from .ico files
    /// According to README, the profile icon is in the bottom-left corner (127x127 px)
    /// </summary>
    public static class IconExtractor
    {
        /// <summary>
        /// Extracts the profile icon from an Edge Profile.ico file
        /// The icon is located in the bottom-left corner (127x127 px)
        /// </summary>
        /// <param name="icoPath">Path to the .ico file</param>
        /// <returns>BitmapSource of the profile icon, or null if extraction fails</returns>
        public static BitmapSource? ExtractProfileIcon(string icoPath)
        {
            try
            {
                if (!File.Exists(icoPath))
                    return null;

                // Load the .ico file
                using var stream = new FileStream(icoPath, FileMode.Open, FileAccess.Read, FileShare.Read);
                var decoder = new IconBitmapDecoder(stream, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.OnLoad);

                // Get the largest frame (icons contain multiple sizes)
                BitmapFrame? largestFrame = null;
                int maxSize = 0;

                foreach (var frame in decoder.Frames)
                {
                    int size = frame.PixelWidth * frame.PixelHeight;
                    if (size > maxSize)
                    {
                        maxSize = size;
                        largestFrame = frame;
                    }
                }

                if (largestFrame == null)
                    return null;

                // According to README, the profile icon is in the bottom-left 127x127 px
                // So we need to crop the image
                int iconSize = 127;
                int fullWidth = largestFrame.PixelWidth;
                int fullHeight = largestFrame.PixelHeight;

                // Check if the image is large enough to contain the expected region
                if (fullWidth < iconSize || fullHeight < iconSize)
                {
                    // If the icon is too small, just return the whole image
                    return largestFrame;
                }

                // Calculate the bottom-left corner position
                int x = 0; // Left edge
                int y = fullHeight - iconSize; // Bottom edge - 127px

                // Create a cropped bitmap
                var croppedBitmap = new CroppedBitmap(largestFrame, new System.Windows.Int32Rect(x, y, iconSize, iconSize));

                // Freeze for cross-thread access
                croppedBitmap.Freeze();

                return croppedBitmap;
            }
            catch
            {
                // If extraction fails, return null
                return null;
            }
        }

        /// <summary>
        /// Tries to load a profile icon, checking multiple possible locations
        /// </summary>
        /// <param name="profilePath">Path to the profile directory</param>
        /// <returns>BitmapSource of the profile icon, or null if not found</returns>
        public static BitmapSource? LoadProfileAvatar(string profilePath)
        {
            // Try different possible icon file names
            string[] possibleNames = 
            {
                "Edge Profile.ico",
                "Google Profile Picture.png",
                "Edge Profile Picture.png"
            };

            foreach (var name in possibleNames)
            {
                string fullPath = Path.Combine(profilePath, name);
                
                if (File.Exists(fullPath))
                {
                    if (fullPath.EndsWith(".ico", StringComparison.OrdinalIgnoreCase))
                    {
                        var icon = ExtractProfileIcon(fullPath);
                        if (icon != null)
                            return icon;
                    }
                    else if (fullPath.EndsWith(".png", StringComparison.OrdinalIgnoreCase))
                    {
                        try
                        {
                            var bitmap = new BitmapImage();
                            bitmap.BeginInit();
                            bitmap.UriSource = new Uri(fullPath);
                            bitmap.CacheOption = BitmapCacheOption.OnLoad;
                            bitmap.EndInit();
                            bitmap.Freeze();
                            return bitmap;
                        }
                        catch
                        {
                            continue;
                        }
                    }
                }
            }

            return null;
        }
    }
}
