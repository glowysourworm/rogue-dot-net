using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Scenario.Alteration.Consumable;
using Rogue.NET.Core.Model.Scenario.Content.Skill;

using System;

namespace Rogue.NET.Core.Model.Scenario.Content.Item
{
    [Serializable]
    public class Consumable : ItemBase
    {
        public ConsumableType Type { get; set; }
        public ConsumableSubType SubType { get; set; }

        public bool HasSpell { get; set; }
        public bool HasProjectileSpell { get; set; }
        public bool HasLearnedSkillSet { get; set; }

        public ConsumableAlteration Alteration { get; set; }
        public ConsumableProjectileAlteration ProjectileAlteration { get; set; }
        public bool HasAlteration { get; set; }
        public bool HasProjectileAlteration { get; set; }

        public bool IdentifyOnUse { get; set; }

        public int Uses { get; set; }

        public Spell Spell { get; set; }
        public Spell AmmoSpell { get; set; }
        public Spell ProjectileSpell { get; set; }
        public SkillSet LearnedSkill { get; set; }

        public string NoteMessage { get; set; }

        public Consumable() : base()
        {
            this.Type = ConsumableType.OneUse;
            this.SubType = ConsumableSubType.Food;
            this.HasSpell = false;
            this.IdentifyOnUse = false;
            this.Spell = new Spell();
            this.AmmoSpell = new Spell();
            this.LearnedSkill = new SkillSet();
            this.Weight = 0.5;
            this.NoteMessage = "";
        }
        public Consumable(ConsumableType type, ConsumableSubType subType, string name, ImageResources icon)
            : base(name, icon)
        {
            this.Type = type;
            this.SubType = subType;
            this.RogueName = name;
            this.HasSpell = false;
            this.IdentifyOnUse = false;
            this.LearnedSkill = new SkillSet();
            this.Spell = new Spell();
            this.AmmoSpell = new Spell();
            this.NoteMessage = "";
        }
    }
}
