using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Scenario.Alteration.Interface;

namespace Rogue.NET.Core.Processing.Command.Backend.CommandData
{
    public class PlayerAlterationEffectMultiItemCommandData : PlayerMultiItemCommandData
    {
        public IAlterationEffect Effect { get; set; }

        public PlayerAlterationEffectMultiItemCommandData(IAlterationEffect effect, PlayerMultiItemActionType action, string[] itemIds)
            : base(action, itemIds)
        {
            this.Effect = effect;
        }
    }
}
