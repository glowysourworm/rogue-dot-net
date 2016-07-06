using Microsoft.Practices.Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.NET.Common.Events.Scenario
{
    public class UserCommandEvent : CompositePresentationEvent<UserCommandEvent>
    {
        public LevelCommandArgs LevelCommand { get; set; }
    }
}
