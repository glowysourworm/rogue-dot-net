using Rogue.NET.Core.Model.Scenario.Alteration;
using Rogue.NET.Core.Model.Scenario.Dynamic;
using Rogue.NET.Core.Model.ScenarioConfiguration.Animation;
using System;
using System.Collections.Generic;

namespace Rogue.NET.Core.Model.Scenario.Character
{
    [Serializable]
    public class Enemy : Character
    {
        public double ExperienceGiven { get; set; }
        public double TurnCounter { get; set; }
        public bool IsEngaged { get; set; }
        public bool IsInvisible { get; set; }

        public BehaviorDetails BehaviorDetails { get; set; }

        public Dictionary<string, AttackAttribute> AttackAttributes { get; set; }

        public override CharacterAlteration Alteration { get; set; }

        public List<AnimationTemplate> DeathAnimations { get; set; }

        public Enemy() : base()
        {
            this.BehaviorDetails = new BehaviorDetails();
            this.AttackAttributes = new Dictionary<string, AttackAttribute>();
            this.Alteration = new CharacterAlteration();
            this.DeathAnimations = new List<AnimationTemplate>();
        }
        public Enemy(string symbol, string name) : base(name, symbol)
        {
            this.BehaviorDetails = new BehaviorDetails();
            this.AttackAttributes = new Dictionary<string, AttackAttribute>();
            this.Alteration = new CharacterAlteration();
            this.DeathAnimations = new List<AnimationTemplate>();
        }
    }
}
