using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Scenario.Alteration.Common;

using System;

namespace Rogue.NET.Core.Model.Scenario.Content
{
    [Serializable]
    public abstract class ScenarioObject : ScenarioImage
    {
        public bool IsExplored { get; set; }
        public bool IsHidden { get; set; }
        public bool IsRevealed { get; set; }
        public bool IsDetectedAlignment { get; set; }
        public bool IsDetectedCategory { get; set; }
        public AlterationAlignmentType DetectedAlignmentType { get; set; }
        public AlterationCategory DetectedAlignmentCategory { get; set; }

        public ScenarioObject()
        {
            this.RogueName = "Unnamed";
        }

        public override string ToString()
        {
            return this.RogueName;
        }
    }
}
