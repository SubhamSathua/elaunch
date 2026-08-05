using System.Configuration;
using System.Data;
using System.Windows;
using E_Launchpad.Services;

namespace E_Launchpad
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            // Load branding before any window XAML resolves {x:Static Branding.*}
            Branding.Initialize();
            base.OnStartup(e);
        }
    }

}
