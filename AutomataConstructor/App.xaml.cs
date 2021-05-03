using System.Globalization;
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
            //AutomataConstructor.Properties.Settings.Default.language = "ru-RU";
            var language = Thread.CurrentThread.CurrentUICulture;
            Thread.CurrentThread.CurrentUICulture = new CultureInfo(CultureInfo.CurrentUICulture.TwoLetterISOLanguageName);
            //Thread.CurrentThread.CurrentUICulture = new CultureInfo("ru-RU");
            base.OnStartup(e);
        }
    }
}
