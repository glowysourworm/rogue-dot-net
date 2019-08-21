using Rogue.NET.Core.Logic.Processing.Enum;
using Rogue.NET.Core.Logic.Processing.Interface;
using Rogue.NET.Core.Model.Scenario.Alteration.Effect;

namespace Rogue.NET.Core.Logic.Processing
{
    public class DialogEquipmentEnhanceUpdate : IDialogEquipmentEnhanceUpdate
    {
        public EquipmentEnhanceAlterationEffect Effect { get; set; }
        public DialogEventType Type { get; set; }
    }
}
