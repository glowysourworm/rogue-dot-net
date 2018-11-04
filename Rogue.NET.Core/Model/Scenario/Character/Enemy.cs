using Rogue.NET.Core.Model.Scenario.Alteration;
using Rogue.NET.Core.Model.Scenario.Dynamic;
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
        public bool WasAttackedByPlayer { get; set; }

        public BehaviorDetails BehaviorDetails { get; set; }

        public Dictionary<string, AttackAttribute> AttackAttributes { get; set; }

        public CharacterAlteration Alteration { get; set; }

        public Enemy() : base()
        {
            this.BehaviorDetails = new BehaviorDetails();
            this.AttackAttributes = new Dictionary<string, AttackAttribute>();
            this.Alteration = new CharacterAlteration();
        }
        public Enemy(string symbol, string name) : base(name, symbol)
        {
            this.BehaviorDetails = new BehaviorDetails();
            this.AttackAttributes = new Dictionary<string, AttackAttribute>();
            this.Alteration = new CharacterAlteration();
        }
    }
}
