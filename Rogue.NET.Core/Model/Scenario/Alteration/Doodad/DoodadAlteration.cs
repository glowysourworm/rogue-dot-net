using Rogue.NET.Core.Model.Scenario.Alteration.Interface;
using Rogue.NET.Core.Model.Scenario.Animation;
using System;

namespace Rogue.NET.Core.Model.Scenario.Alteration.Doodad
{
    [Serializable]
    public class DoodadAlteration : RogueBase
    {
        public AnimationContainer Animation { get; set; }
        public IDoodadAlterationEffect Effect { get; set; }

        public DoodadAlteration()
        {
            this.Animation = new AnimationContainer();
        }
    }
}
