using Rogue.NET.Core.Model.Scenario.Alteration;
using Rogue.NET.Core.Model.ScenarioConfiguration.Animation;
using System;
using System.Collections.Generic;

namespace Rogue.NET.Core.Model.Scenario.Content
{
    [Serializable]
    public class Religion : ScenarioImage
    {
        public bool HasAttributeBonus { get; set; }
        public bool HasBonusAttackAttributes { get; set; }
        public bool AllowsRenunciation { get; set; }
        public bool AllowsReAffiliation { get; set; }

        public AlterationEffect AttributeAlteration { get; set; }
        public AlterationEffect AttackAttributeAlteration { get; set; }

        public List<AnimationTemplate> RenunciationAnimations { get; set; }

        public Religion()
        {
            this.AttackAttributeAlteration = new AlterationEffect();
            this.AttributeAlteration = new AlterationEffect();
            this.RenunciationAnimations = new List<AnimationTemplate>();
        }
    }
}
