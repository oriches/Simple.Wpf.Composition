namespace Simple.Wpf.Composition.Workspaces.Example1
{
    public sealed class ExampleController : BaseController<ExampleViewModel>
    {
        public ExampleController(ExampleViewModel viewModel) : base(viewModel)
        {
        }
    }
}