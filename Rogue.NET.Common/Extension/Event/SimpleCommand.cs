using System;
using System.Windows.Input;

namespace Rogue.NET.Common.Extension.Event
{
    public class SimpleCommand : ICommand
    {
        Action _action;
        Func<bool> _canExecute;

        public SimpleCommand(Action action)
        {
            _action = action;
        }

        public SimpleCommand(Action action, Func<bool> canExecute)
        {
            _action = action;
            _canExecute = canExecute;
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return _canExecute();
        }

        public void Execute(object parameter)
        {
            if (_action != null)
                _action.Invoke();
        }
    }

    public class SimpleCommand<T> : ICommand
    {
        Action<T> _action;
        Func<T, bool> _canExecute;

        public SimpleCommand(Action<T> action)
        {
            _action = action;
        }

        public SimpleCommand(Action<T> action, Func<T, bool> canExecute)
        {
            _action = action;
            _canExecute = canExecute;
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return _canExecute((T)parameter);
        }

        public void Execute(object parameter)
        {
            if (_action != null)
                _action.Invoke((T)parameter);
        }
    }
}
