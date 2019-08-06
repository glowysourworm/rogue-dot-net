using Rogue.NET.Core.Model.Scenario.Alteration;
using Rogue.NET.Core.Model.Scenario.Alteration.Common;
using System.Collections.Generic;

namespace Rogue.NET.Core.Logic.Processing.Interface
{
    public interface IDialogImbueUpdate : IDialogUpdate
    {
        IEnumerable<AttackAttribute> ImbueAttackAttributes { get; set; }
    }
}
