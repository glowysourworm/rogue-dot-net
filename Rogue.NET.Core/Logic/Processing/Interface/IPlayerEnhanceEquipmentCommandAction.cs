using Rogue.NET.Core.Model.Scenario.Alteration.Common;
using Rogue.NET.Core.Model.Scenario.Alteration.Effect;
using System.Collections.Generic;

namespace Rogue.NET.Core.Logic.Processing.Interface
{
    public interface IPlayerEnhanceEquipmentCommandAction : IPlayerCommandAction
    {
        EquipmentEnhanceAlterationEffect Effect { get; set; }
    }
}
