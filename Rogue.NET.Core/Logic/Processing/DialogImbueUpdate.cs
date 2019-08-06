using Rogue.NET.Core.Logic.Processing.Enum;
using Rogue.NET.Core.Logic.Processing.Interface;
using Rogue.NET.Core.Model.Scenario.Alteration;
using Rogue.NET.Core.Model.Scenario.Alteration.Common;
using System.Collections.Generic;

namespace Rogue.NET.Core.Logic.Processing
{
    public class DialogImbueUpdate : IDialogImbueUpdate
    {
        public IEnumerable<AttackAttribute> ImbueAttackAttributes { get; set; }
        public DialogEventType Type { get; set; }
    }
}
