using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Media.Imaging;

namespace ELaunch.Utils
{
    /// <summary>
    /// Extracts icons from executable files
    /// </summary>
    public static class ExeIconExtractor
    {
        [DllImport("shell32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr ExtractIcon(IntPtr hInst, string lpszExeFileName, int nIconIndex);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool DestroyIcon(IntPtr hIcon);

        /// <summary>
        /// Extracts the icon from an executable file
        /// </summary>
        /// <param name="exePath">Path to the .exe file</param>
        /// <returns>BitmapSource of the icon, or null if extraction fails</returns>
        public static BitmapSource? ExtractIconFromExe(string exePath)
        {
            try
            {
                if (!File.Exists(exePath))
                    return null;

                IntPtr hIcon = ExtractIcon(IntPtr.Zero, exePath, 0);
                
                if (hIcon == IntPtr.Zero)
                    return null;

                try
                {
                    var icon = System.Drawing.Icon.FromHandle(hIcon);
                    var bitmap = icon.ToBitmap();
                    
                    // Convert to BitmapSource
                    IntPtr hBitmap = bitmap.GetHbitmap();
                    try
                    {
                        var bitmapSource = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                            hBitmap,
                            IntPtr.Zero,
                            System.Windows.Int32Rect.Empty,
                            BitmapSizeOptions.FromEmptyOptions());
                        
                        bitmapSource.Freeze();
                        return bitmapSource;
                    }
                    finally
                    {
                        DeleteObject(hBitmap);
                    }
                }
                finally
                {
                    DestroyIcon(hIcon);
                }
            }
            catch
            {
                return null;
            }
        }

        [DllImport("gdi32.dll")]
        private static extern bool DeleteObject(IntPtr hObject);
    }
}
