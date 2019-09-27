using Rogue.NET.Core.Model.Enums;
using System;

namespace Rogue.NET.Core.Model.Scenario.Content.Doodad
{
    [Serializable]
    public abstract class DoodadBase : ScenarioObject
    {
        public bool IsOneUse { get; set; }
        public bool HasBeenUsed { get; set; }
        public DoodadType Type { get; set; }

        public DoodadBase()
        {
        }
    }
}
