namespace Simple.Wpf.Composition.Workspaces
{
    using System;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using Infrastructure;

    public sealed class Workspace : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private readonly BaseController _controller;
        private readonly Action _dispose;
        private string _title;

        public Workspace(BaseController controller, Uri resources, Action dispose)
        {
            Resources = resources;

            _controller = controller;
            _dispose = dispose;
        }

        public BaseViewModel Content { get { return _controller.ViewModel; } }

        public Type Type { get { return _controller.Type; } }

        public Uri Resources { get; private set; }

        public string Title
        {
            get
            {
                return _title;
            }

            set
            {
                _title = value;
                OnPropertyChanged();
            }
        }

        public void Dispose()
        {
            if (_dispose != null)
            {
                _controller.Dispose();
                _dispose();
            }
        }

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}