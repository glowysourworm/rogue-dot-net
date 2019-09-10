using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.NET.Core.Processing.Event.Backend.Enum
{
    /// <summary>
    /// Type of operation that requires a target
    /// </summary>
    public enum TargetRequestType
    {
        Consume,
        Throw,
        Fire,
        InvokeSkill
    }
}
