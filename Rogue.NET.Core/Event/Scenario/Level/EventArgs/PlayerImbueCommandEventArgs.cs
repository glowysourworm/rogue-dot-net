using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Scenario.Alteration.Common;
using System.Collections.Generic;

namespace Rogue.NET.Core.Event.Scenario.Level.EventArgs
{
    public class PlayerImbueCommandEventArgs : PlayerCommandEventArgs
    {
        public IEnumerable<AttackAttribute> ImbueAttackAttributes { get; set; }

        public PlayerImbueCommandEventArgs(PlayerActionType action, string id) : base (action, id)
        {

        }
    }
}
