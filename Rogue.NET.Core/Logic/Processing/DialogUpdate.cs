using Rogue.NET.Core.Logic.Processing.Enum;
using Rogue.NET.Core.Logic.Processing.Interface;
using Rogue.NET.Core.Model.Scenario.Alteration;
using System.Collections.Generic;

namespace Rogue.NET.Core.Logic.Processing
{
    public class DialogUpdate : IDialogUpdate
    {
        public DialogEventType Type { get; set; }
    }
}
