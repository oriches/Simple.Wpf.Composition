namespace Simple.Wpf.Composition.Workspaces.FSharp.Repl
{
    using Wpf.FSharp.Repl.UI.Controllers;
    using Wpf.FSharp.Repl.UI.ViewModels;
    using BaseViewModel = Infrastructure.BaseViewModel;

    public sealed class FSharpReplViewModel : BaseViewModel
    {
        private readonly IReplEngineController _replEngineController;

        public FSharpReplViewModel(IReplEngineController replEngineController)
        {
            _replEngineController = replEngineController;
        }

        public IReplEngineViewModel Content { get { return _replEngineController.ViewModel; } }
    }
}