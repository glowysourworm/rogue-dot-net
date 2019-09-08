using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Scenario.Alteration.Interface;

namespace Rogue.NET.Core.Processing.Command.Backend.CommandData
{
    public class PlayerAlterationEffectCommandData : PlayerCommandData
    {
        public IAlterationEffect Effect { get; set; }

        public PlayerAlterationEffectCommandData(IAlterationEffect effect, PlayerCommandType action, string id) 
            : base (action, id)
        {
            this.Effect = effect;
        }
    }
}
