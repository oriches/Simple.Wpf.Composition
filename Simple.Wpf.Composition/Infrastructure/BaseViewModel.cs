using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Reactive.Disposables;
using System.Runtime.CompilerServices;

namespace Simple.Wpf.Composition.Infrastructure
{
    public abstract class BaseViewModel : INotifyPropertyChanged
    {
        private SuspendedNotifications _suspendedNotifications;
        public event PropertyChangedEventHandler PropertyChanged;

        public IDisposable SuspendNotifications()
        {
            if (_suspendedNotifications == null) _suspendedNotifications = new SuspendedNotifications(this);

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
                handler?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        protected virtual bool SetPropertyAndNotify<T>(ref T existingValue, T newValue, Expression<Func<T>> expression)
        {
            return SetPropertyAndNotify(ref existingValue, newValue, ExpressionHelper.Name(expression));
        }

        protected virtual bool SetPropertyAndNotify<T>(ref T existingValue, T newValue,
            [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(existingValue, newValue)) return false;

            existingValue = newValue;
            if (_suspendedNotifications != null)
            {
                _suspendedNotifications.Add(propertyName);
            }
            else
            {
                var handler = PropertyChanged;
                handler?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }

            return true;
        }

        private sealed class SuspendedNotifications : IDisposable
        {
            private readonly HashSet<string> _properties = new HashSet<string>();
            private readonly BaseViewModel _target;
            private int _refCount;

            public SuspendedNotifications(BaseViewModel target)
            {
                _target = target;
            }

            public void Dispose()
            {
                _target._suspendedNotifications = null;
                foreach (var property in _properties) _target.OnPropertyChanged(property);
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
                    if (--_refCount == 0) Dispose();
                });
            }
        }
    }
}