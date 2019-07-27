using Rogue.NET.Core.Logic.Processing.Interface;
using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Scenario.Alteration;
using System.Collections.Generic;

namespace Rogue.NET.Core.Logic.Processing
{
    public class PlayerImbueCommandAction : IPlayerImbueCommandAction
    {
        public IEnumerable<AttackAttribute> ImbueAttackAttributes { get; set; }
        public PlayerActionType Type { get; set; }
        public string Id { get; set; }
    }
}
