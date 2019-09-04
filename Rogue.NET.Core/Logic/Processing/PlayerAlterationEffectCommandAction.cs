using Rogue.NET.Core.Logic.Processing.Interface;
using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Scenario.Alteration.Interface;

namespace Rogue.NET.Core.Logic.Processing
{
    public class PlayerAlterationEffectCommandAction : IPlayerAlterationEffectCommandAction
    {
        public IAlterationEffect Effect { get; set; }
        public PlayerActionType Type { get; set; }
        public string Id { get; set; }
    }
}
