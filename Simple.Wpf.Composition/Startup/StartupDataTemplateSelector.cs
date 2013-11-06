namespace Simple.Wpf.Composition.Startup
{
    using System.Windows;

    public sealed class StartupDataTemplateSelector : System.Windows.Controls.DataTemplateSelector
    {
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item == null)
            {
                return null;
            }

            var viewModelName = item.GetType().Name;
            var templateName = viewModelName.Replace("ViewModel", "ViewTemplate");
            
            return (DataTemplate)App.Current.MainWindow.FindResource(templateName);
        }
    }
}