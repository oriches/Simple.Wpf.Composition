namespace Simple.Wpf.Composition.Workspaces.FSharp.Repl
{
    using Infrastructure;

    public sealed class FSharpReplController : BaseController<FSharpReplViewModel>
    {
        public FSharpReplController(FSharpReplViewModel viewModel)
            : base(viewModel)
        {
        }
    }
}