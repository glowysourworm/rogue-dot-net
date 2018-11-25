using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Rogue.NET.Common.Extension.Prism.EventAggregator
{
    public class AsyncCommand : IAsyncCommand
    {
        readonly Func<Task> _function;
        readonly Func<bool> _canExecute;

        public AsyncCommand(Func<Task> function)
        {
            _function = function;
        }
        public AsyncCommand(Func<Task> function, Func<bool> canExecute)
        {
            _function = function;
            _canExecute = canExecute;
        }
        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            if (_canExecute != null)
                return _canExecute();

            return true;
        }

        public async void Execute(object parameter)
        {
            await _function();
        }

        public void RaiseCanExecuteChanged()
        {
            if (this.CanExecuteChanged != null)
                this.CanExecuteChanged(this, new EventArgs());
        }
    }
    public class AsyncCommand<T> : ICommand
    {
        readonly Func<T, Task> _function;
        readonly Func<T, bool> _canExecute;

        public AsyncCommand(Func<T, Task> function)
        {
            _function = function;
        }
        public AsyncCommand(Func<T, Task> function, Func<T, bool> canExecute)
        {
            _function = function;
            _canExecute = canExecute;
        }
        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            if (_canExecute != null)
                return _canExecute((T)parameter);

            return true;
        }

        public async void Execute(object parameter)
        {
            await _function((T)parameter);
        }

        public void RaiseCanExecuteChanged()
        {
            if (this.CanExecuteChanged != null)
                this.CanExecuteChanged(this, new EventArgs());
        }
    }
}
