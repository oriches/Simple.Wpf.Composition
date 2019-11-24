using Simple.Wpf.Composition.Infrastructure;

namespace Simple.Wpf.Composition.Workspaces.Example
{
    public sealed class ExampleController : BaseController<ExampleViewModel>
    {
        public ExampleController(ExampleViewModel viewModel) : base(viewModel)
        {
        }
    }
}