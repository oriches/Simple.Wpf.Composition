namespace Simple.Wpf.Composition
{
    using System.Windows;
    using Startup;

    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            BootStrapper.Start();

            Current.Exit += (s, a) => BootStrapper.Stop();

            System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("en-GB");

            base.OnStartup(e);
            
            var mainWindow = new MainWindow
            {
                DataContext = BootStrapper.RootVisual
            };

            mainWindow.Show();
        }
    }
}
