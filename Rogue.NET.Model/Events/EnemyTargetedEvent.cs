using Microsoft.Practices.Prism.Events;
using Rogue.NET.Model.Scenario;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.NET.Model.Events
{
    public class EnemyTargetedEvent : CompositePresentationEvent<EnemyTargetedEvent>
    {
        public Enemy TargetedEnemy { get; set; }
        public bool TargetingEnded { get; set; }
    }
}
