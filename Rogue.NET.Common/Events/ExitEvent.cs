using Microsoft.Practices.Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.NET.Common.Events
{
    /// <summary>
    /// Application exit event
    /// </summary>
    public class ExitEvent : CompositePresentationEvent<ExitEvent>
    {
    }
}
