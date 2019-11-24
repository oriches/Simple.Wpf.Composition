using System;

namespace Simple.Wpf.Composition.Infrastructure
{
    public abstract class BaseController : IDisposable
    {
        protected BaseController(BaseViewModel viewModel = null)
        {
            ViewModel = viewModel;
        }

        public BaseViewModel ViewModel { get; protected set; }

        public abstract Type Type { get; }

        public virtual void Dispose()
        {
        }
    }

    public abstract class BaseController<TViewModel> : BaseController where TViewModel : BaseViewModel
    {
        protected BaseController(TViewModel viewModel = null)
            : base(viewModel)
        {
        }

        public new virtual TViewModel ViewModel
        {
            get => (TViewModel) base.ViewModel;

            protected set => base.ViewModel = value;
        }

        public override Type Type => typeof(TViewModel);
    }
}