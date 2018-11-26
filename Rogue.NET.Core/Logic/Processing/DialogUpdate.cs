using Rogue.NET.Core.Logic.Processing.Enum;
using Rogue.NET.Core.Logic.Processing.Interface;

namespace Rogue.NET.Core.Logic.Processing
{
    public class DialogUpdate : IDialogUpdate
    {
        public DialogEventType Type { get; set; }
    }
}
