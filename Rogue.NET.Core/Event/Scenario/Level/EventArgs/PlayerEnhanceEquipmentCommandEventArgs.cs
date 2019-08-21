using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Scenario.Alteration.Effect;

namespace Rogue.NET.Core.Event.Scenario.Level.EventArgs
{
    public class PlayerEnhanceEquipmentCommandEventArgs : PlayerCommandEventArgs
    {
        public EquipmentEnhanceAlterationEffect Effect { get; set; }

        public PlayerEnhanceEquipmentCommandEventArgs(PlayerActionType action, string id) : base (action, id)
        {

        }
    }
}
