namespace Simple.Wpf.Composition.Workspaces.Example2
{
    public sealed class ExampleController : BaseController<ExampleViewModel>
    {
        public ExampleController(ExampleViewModel viewModel) : base(viewModel)
        {
        }
    }
}