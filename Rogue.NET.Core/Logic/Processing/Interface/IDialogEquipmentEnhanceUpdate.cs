using Rogue.NET.Core.Model.Scenario.Alteration.Effect;

namespace Rogue.NET.Core.Logic.Processing.Interface
{
    public interface IDialogEquipmentEnhanceUpdate : IDialogUpdate
    {
        EquipmentEnhanceAlterationEffect Effect { get; set; }
    }
}
