namespace Simple.Wpf.Composition.Workspaces.Memory
{
    public sealed class MemoryController : BaseController<MemoryViewModel>
    {
        public MemoryController(MemoryViewModel viewModel)
            : base(viewModel)
        {
        }
    }
}