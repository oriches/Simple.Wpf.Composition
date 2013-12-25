namespace Simple.Wpf.Composition
{
    using System;

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

        public virtual new TViewModel ViewModel
        {
            get
            {
                return (TViewModel)base.ViewModel;
            }

            protected set
            {
                base.ViewModel = value;
            }
        }

        public override Type Type { get { return typeof (TViewModel); } }
    }
}