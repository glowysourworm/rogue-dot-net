using Microsoft.Practices.Prism.Events;
using Rogue.NET.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.NET.Model.Events
{
    public class PlayerDiedEvent : CompositePresentationEvent<PlayerDiedEvent>
    {
        public string PlayerName { get; set; }
        public string DiedOf { get; set; }
    }
}
