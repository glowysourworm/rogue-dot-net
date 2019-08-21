using Rogue.NET.Core.Logic.Processing.Interface;
using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Scenario.Alteration.Common;
using Rogue.NET.Core.Model.Scenario.Alteration.Effect;
using System.Collections.Generic;

namespace Rogue.NET.Core.Logic.Processing
{
    public class PlayerEnhanceEquipmentCommandAction : IPlayerEnhanceEquipmentCommandAction
    {
        public EquipmentEnhanceAlterationEffect Effect { get; set; }
        public PlayerActionType Type { get; set; }
        public string Id { get; set; }
    }
}
