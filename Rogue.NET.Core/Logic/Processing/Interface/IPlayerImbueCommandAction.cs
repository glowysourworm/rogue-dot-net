using Rogue.NET.Core.Model.Scenario.Alteration.Common;
using System.Collections.Generic;

namespace Rogue.NET.Core.Logic.Processing.Interface
{
    public interface IPlayerImbueCommandAction : IPlayerCommandAction
    {
        IEnumerable<AttackAttribute> ImbueAttackAttributes { get; set; }
    }
}
