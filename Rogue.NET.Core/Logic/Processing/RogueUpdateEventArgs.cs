using Rogue.NET.Core.Logic.Processing.Enum;
using Rogue.NET.Core.Logic.Processing.Interface;
using System;

namespace Rogue.NET.Core.Logic.Processing
{
    public class RogueUpdateEventArgs : EventArgs
    {
        public IRogueUpdate Update { get; set; }
        public RogueUpdatePriority Priority { get; set; }
    }
}
