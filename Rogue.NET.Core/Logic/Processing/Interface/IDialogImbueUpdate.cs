using Rogue.NET.Core.Model.Scenario.Alteration.Effect;

namespace Rogue.NET.Core.Logic.Processing.Interface
{
    public interface IDialogModifyEquipmentUpdate : IDialogUpdate
    {
        EquipmentModifyAlterationEffect Effect { get; set; }
    }
}
