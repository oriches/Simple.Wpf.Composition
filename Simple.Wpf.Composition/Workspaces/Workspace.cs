using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Simple.Wpf.Composition.Infrastructure;

namespace Simple.Wpf.Composition.Workspaces
{
    public sealed class Workspace : INotifyPropertyChanged
    {
        private readonly BaseController _controller;
        private readonly Action _dispose;
        private string _title;

        public Workspace(BaseController controller, Uri resources, Action dispose)
        {
            Resources = resources;

            _controller = controller;
            _dispose = dispose;
        }

        public BaseViewModel Content => _controller.ViewModel;

        public Type Type => _controller.Type;

        public Uri Resources { get; }

        public string Title
        {
            get => _title;

            set
            {
                _title = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

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
            handler?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}