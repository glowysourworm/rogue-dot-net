using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Rogue.NET.Common.Extension.Prism.EventAggregator
{
    public interface IAsyncCommand : ICommand
    {
        void RaiseCanExecuteChanged();
    }
}
