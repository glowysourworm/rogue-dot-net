using Rogue.NET.Core.Logic.Processing.Enum;
using Rogue.NET.Core.Model.Scenario.Alteration;
using System.Collections.Generic;

namespace Rogue.NET.Core.Logic.Processing.Interface
{
    public interface IDialogUpdate
    {
        DialogEventType Type { get; set; }

        IEnumerable<AttackAttribute> ImbueAttackAttributes { get; set; }
    }
}
