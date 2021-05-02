using System.Threading;
using System.Windows;

namespace AutomataConstructor
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            AutomataConstructor.Properties.Settings.Default.language = "en-US";
            var language = AutomataConstructor.Properties.Settings.Default.language;
            Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(language);
            base.OnStartup(e);
        }
    }
}
