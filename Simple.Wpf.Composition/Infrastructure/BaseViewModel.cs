namespace Simple.Wpf.Composition.Infrastructure
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq.Expressions;
    using System.Reactive.Disposables;
    using System.Runtime.CompilerServices;

    public abstract class BaseViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private sealed class SuspendedNotifications : IDisposable
        {
            private readonly BaseViewModel _target;
            private readonly HashSet<string> _properties = new HashSet<string>();
            private int _refCount;

            public SuspendedNotifications(BaseViewModel target)
            {
                _target = target;
            }

            public void Add(string propertyName)
            {
                _properties.Add(propertyName);
            }

            public IDisposable AddRef()
            {
                ++_refCount;
                return Disposable.Create(() =>
                {
                    if (--_refCount == 0)
                    {
                        Dispose();
                    }
                });
            }

            public void Dispose()
            {
                _target._suspendedNotifications = null;
                foreach (var property in _properties)
                {
                    _target.OnPropertyChanged(property);
                }
            }
        }

        private SuspendedNotifications _suspendedNotifications;

        public IDisposable SuspendNotifications()
        {
            if (_suspendedNotifications == null)
            {
                _suspendedNotifications = new SuspendedNotifications(this);
            }

            return _suspendedNotifications.AddRef();
        }

        protected virtual void OnPropertyChanged<T>(Expression<Func<T>> expression)
        {
            OnPropertyChanged(ExpressionHelper.Name(expression));
        }
        
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            if (_suspendedNotifications != null)
            {
                _suspendedNotifications.Add(propertyName);
            }
            else
            {
                var handler = PropertyChanged;
                if (handler != null)
                {
                    handler(this, new PropertyChangedEventArgs(propertyName));
                }
            }
        }

        protected virtual bool SetPropertyAndNotify<T>(ref T existingValue, T newValue, Expression<Func<T>> expression)
        {
            return SetPropertyAndNotify(ref existingValue, newValue, ExpressionHelper.Name(expression));
        }

        protected virtual bool SetPropertyAndNotify<T>(ref T existingValue, T newValue, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(existingValue, newValue))
            {
                return false;
            }

            existingValue = newValue;
            if (_suspendedNotifications != null)
            {
                _suspendedNotifications.Add(propertyName);
            }
            else
            {
                var handler = PropertyChanged;
                if (handler != null)
                {
                    handler(this, new PropertyChangedEventArgs(propertyName));
                }
            }

            return true;
        }
    }
}