using Rogue.NET.Common.ViewModel;
using Rogue.NET.Core.Model.Scenario.Content.Skill;
using Rogue.NET.Scenario.Content.ViewModel.Content.ScenarioMetaData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.NET.Scenario.Content.ViewModel.Content.Alteration
{
    public class SkillViewModel : RogueBaseViewModel
    {
        public int LevelRequirement { get; set; }
        public int PointRequirement { get; set; }
        public double RequiredAffiliationLevel { get; set; }
        public SpellViewModel Alteration { get; set; }

        public bool IsLearned { get; set; }

        public SkillViewModel(Skill skill) : base(skill)
        {
        }
    }
}
