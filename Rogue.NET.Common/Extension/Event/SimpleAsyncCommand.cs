using System;
using System.Threading.Tasks;

namespace Rogue.NET.Common.Extension.Event
{
    public class SimpleAsyncCommand : ISimpleAsyncCommand
    {
        readonly Func<Task> _function;
        readonly Func<bool> _canExecute;

        public SimpleAsyncCommand(Func<Task> function)
        {
            _function = function;
        }
        public SimpleAsyncCommand(Func<Task> function, Func<bool> canExecute)
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
}
