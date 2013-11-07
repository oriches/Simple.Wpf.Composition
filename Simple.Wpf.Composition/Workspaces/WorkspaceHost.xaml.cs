namespace Simple.Wpf.Composition.Workspaces
{
    using System;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;

    public partial class WorkspaceHost : UserControl
    {
        public static readonly DependencyProperty WorkspaceProperty = DependencyProperty.Register("Workspace",
          typeof(Workspace),
          typeof(WorkspaceHost),
          new PropertyMetadata(null, WorkspaceChanged));

        public WorkspaceHost()
        {
            InitializeComponent();
            SetNoWorkspaceContent();
        }
        
        public Workspace Workspace
        {
            get
            {
                return (Workspace)GetValue(WorkspaceProperty);
            }
            set
            {
                SetValue(WorkspaceProperty, value);
            }
        }
        
        private static void WorkspaceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue == e.NewValue)
            {
                return;
            }

            var host = (WorkspaceHost)d;

            var oldWorkspace = (Workspace)e.OldValue;
            var newWorkspace = (Workspace)e.NewValue;
            
            host.UnloadWorkspace(oldWorkspace);
            host.LoadWorkspace(newWorkspace);
        }

        private void LoadWorkspace(Workspace newWorkspace)
        {
            if (newWorkspace != null)
            {
                LoadResources(newWorkspace.Resources);

                WorkspacePresenter.Content = newWorkspace.Content;
            }
            else
            {
                SetNoWorkspaceContent();
            }
        }

        private void UnloadWorkspace(Workspace workspace)
        {
            if (workspace != null)
            {
                UnloadResources(workspace.Resources);
            }
        }

        private void LoadResources(Uri uri)
        {
            if (uri == null)
            {
                return;
            }
            
            var newResourceDictionary = new ResourceDictionary
            {
                Source = uri
            };

            Resources.MergedDictionaries.Add(newResourceDictionary);
        }

        private void UnloadResources(Uri uri)
        {
            if (uri == null)
            {
                return;
            }

            var index = Resources.MergedDictionaries.Where(x => x.Source == uri)
                    .Select((x, i) => i)
                    .Single();

            if (index != -1)
            {
                Resources.MergedDictionaries.RemoveAt(index);
            }
        }

        private void SetNoWorkspaceContent()
        {
            WorkspacePresenter.ContentTemplateSelector = null;
            WorkspacePresenter.Content = new TextBlock
            {
                Text = "No workspace selected!",
                Margin = new Thickness(5),
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };
        }
    }
}
