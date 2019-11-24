using Simple.Wpf.Composition.Infrastructure;

namespace Simple.Wpf.Composition.Workspaces.FSharp.Repl
{
    public sealed class FSharpReplController : BaseController<FSharpReplViewModel>
    {
        public FSharpReplController(FSharpReplViewModel viewModel)
            : base(viewModel)
        {
        }
    }
}