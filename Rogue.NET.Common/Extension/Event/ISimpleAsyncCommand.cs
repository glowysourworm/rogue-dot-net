using System.Windows.Input;

namespace Rogue.NET.Common.Extension.Event
{
    public interface ISimpleAsyncCommand : ICommand
    {
        void RaiseCanExecuteChanged();
    }
}
