using Rogue.NET.Core.Logic.Processing.Enum;
using Rogue.NET.Core.Model.Scenario.Alteration;
using System.Collections.Generic;

namespace Rogue.NET.Core.Logic.Processing.Interface
{
    public interface IDialogUpdate : IRogueUpdate
    {
        DialogEventType Type { get; set; }
    }
}
