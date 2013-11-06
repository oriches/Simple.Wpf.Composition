namespace Simple.Wpf.Composition.Workspaces
{
    using System.Windows;

    public sealed class WorkspaceDataTemplateSelector : System.Windows.Controls.DataTemplateSelector
    {
        private readonly WorkspaceHost _workspaceHost;

        public WorkspaceDataTemplateSelector(WorkspaceHost workspaceHost)
        {
            _workspaceHost = workspaceHost;
        }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item == null)
            {
                return null;
            }

            var viewModelName = item.GetType().Name;
            var templateName = viewModelName.Replace("ViewModel", "ViewTemplate");
            
            return (DataTemplate)_workspaceHost.FindResource(templateName);
        }
    }
}