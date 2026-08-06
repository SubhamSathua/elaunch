using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ELaunch.Utils
{
    /// <summary>
    /// Reduces the mouse wheel scroll sensitivity of a ScrollViewer.
    /// </summary>
    public static class ScrollWheelHelper
    {
        /// <summary>
        /// Sensitivity factor: 1.0 = default WPF behavior, 0.5 = half as sensitive.
        /// Defaults to 0.5 when attached.
        /// </summary>
        public static readonly DependencyProperty SensitivityProperty =
            DependencyProperty.RegisterAttached(
                "Sensitivity",
                typeof(double),
                typeof(ScrollWheelHelper),
                new PropertyMetadata(0.5, OnSensitivityChanged));

        public static double GetSensitivity(DependencyObject obj) => (double)obj.GetValue(SensitivityProperty);

        public static void SetSensitivity(DependencyObject obj, double value) => obj.SetValue(SensitivityProperty, value);

        private static void OnSensitivityChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ScrollViewer scrollViewer)
            {
                scrollViewer.PreviewMouseWheel -= OnPreviewMouseWheel;
                scrollViewer.PreviewMouseWheel += OnPreviewMouseWheel;
            }
        }

        private static void OnPreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (sender is not ScrollViewer scrollViewer)
            {
                return;
            }

            double sensitivity = GetSensitivity(scrollViewer);
            if (sensitivity >= 1.0)
            {
                return;
            }

            double defaultScrollPerNotch = SystemParameters.WheelScrollLines * SystemParameters.ScrollHeight;
            double adjustedScroll = e.Delta / 120.0 * defaultScrollPerNotch * sensitivity;

            if (adjustedScroll == 0)
            {
                return;
            }

            scrollViewer.ScrollToVerticalOffset(scrollViewer.VerticalOffset - adjustedScroll);
            e.Handled = true;
        }
    }
}
