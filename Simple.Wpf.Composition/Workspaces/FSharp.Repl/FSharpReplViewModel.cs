using Simple.Wpf.FSharp.Repl.UI.Controllers;
using Simple.Wpf.FSharp.Repl.UI.ViewModels;
using BaseViewModel = Simple.Wpf.Composition.Infrastructure.BaseViewModel;

namespace Simple.Wpf.Composition.Workspaces.FSharp.Repl
{
    public sealed class FSharpReplViewModel : BaseViewModel
    {
        private readonly IReplEngineController _replEngineController;

        public FSharpReplViewModel(IReplEngineController replEngineController)
        {
            _replEngineController = replEngineController;
        }

        public IReplEngineViewModel Content => _replEngineController.ViewModel;
    }
}