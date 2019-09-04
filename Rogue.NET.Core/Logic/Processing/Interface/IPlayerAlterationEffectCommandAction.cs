using Rogue.NET.Core.Model.Scenario.Alteration.Interface;

namespace Rogue.NET.Core.Logic.Processing.Interface
{
    public interface IPlayerAlterationEffectCommandAction : IPlayerCommandAction
    {
        IAlterationEffect Effect { get; set; }
    }
}
