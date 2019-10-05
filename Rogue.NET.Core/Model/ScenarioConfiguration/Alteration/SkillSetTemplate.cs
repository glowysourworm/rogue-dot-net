using Rogue.NET.Core.Model.ScenarioConfiguration.Abstract;
using System;
using System.Collections.Generic;

namespace Rogue.NET.Core.Model.ScenarioConfiguration.Alteration
{
    [Serializable]
    public class SkillSetTemplate : DungeonObjectTemplate
    {
        public List<SkillTemplate> Skills { get; set; }

        public SkillSetTemplate()
        {
            this.Skills = new List<SkillTemplate>();
        }
    }
}
