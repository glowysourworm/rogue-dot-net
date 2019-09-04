using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Scenario.Alteration.Effect;
using Rogue.NET.Core.Model.Scenario.Alteration.Interface;

namespace Rogue.NET.Core.Event.Scenario.Level.EventArgs
{
    public class PlayerAlterationEffectCommandEventArgs : PlayerCommandEventArgs
    {
        public IAlterationEffect Effect { get; set; }

        public PlayerAlterationEffectCommandEventArgs(IAlterationEffect effect, PlayerActionType action, string id) 
            : base (action, id)
        {
            this.Effect = effect;
        }
    }
}
