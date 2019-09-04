using Rogue.NET.Core.Logic.Processing.Enum;
using Rogue.NET.Core.Logic.Processing.Interface;
using Rogue.NET.Core.Model.Scenario.Alteration.Interface;

namespace Rogue.NET.Core.Logic.Processing
{
    public class DialogAlterationEffectUpdate : IDialogAlterationEffectUpdate
    {
        public DialogEventType Type { get; set; }
        public IAlterationEffect Effect { get; set; }
    }
}
