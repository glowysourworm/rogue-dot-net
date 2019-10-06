using Rogue.NET.Core.Model.Scenario.Content;
using Rogue.NET.Core.Model.Scenario.Content.Layout;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.NET.Core.Processing.Event.Backend.EventData
{
    public class ProjectileAnimationEventData
    {
        public ScenarioImage ProjectileImage { get; set; }
        public GridLocation SourceLocation { get; set; }
        public GridLocation TargetLocation { get; set; }
    }
}
