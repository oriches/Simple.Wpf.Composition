namespace Simple.Wpf.Composition.Workspaces
{
    public interface IWorkspaceDescriptor
    {
        int Position { get; }

        string Name { get; }

        Workspace CreateWorkspace();
    }
}