using Rogue.NET.Core.Logic.Processing.Interface;
using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Scenario.Alteration.Interface;
using System.Collections.Generic;

namespace Rogue.NET.Core.Logic.Processing
{
    public class PlayerAlterationEffectMultiItemCommandAction : IPlayerAlterationEffectMultiItemCommandAction
    {
        public IAlterationEffect Effect { get; set; }
        public PlayerMultiItemActionType Type { get; set; }
        public IEnumerable<string> ItemIds { get; set; }
    }
}
