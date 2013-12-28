namespace Simple.Wpf.Composition.Workspaces.Example
{
    using Infrastructure;

    public sealed class ExampleController : BaseController<ExampleViewModel>
    {
        public ExampleController(ExampleViewModel viewModel) : base(viewModel)
        {
        }
    }
}