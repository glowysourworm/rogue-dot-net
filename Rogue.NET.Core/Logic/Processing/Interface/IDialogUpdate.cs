using Rogue.NET.Core.Logic.Processing.Enum;

namespace Rogue.NET.Core.Logic.Processing.Interface
{
    public interface IDialogUpdate
    {
        DialogEventType Type { get; set; }
    }
}
