using System.Windows;
using Simple.Wpf.Composition.Startup;

namespace Simple.Wpf.Composition
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            BootStrapper.Start();

            Current.Exit += (s, a) => BootStrapper.Stop();

            new MainWindow {DataContext = BootStrapper.RootVisual}.Show();
        }
    }
}