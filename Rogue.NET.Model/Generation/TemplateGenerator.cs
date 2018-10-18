using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using Rogue.NET.Scenario.Model;
using Rogue.NET.Common;
using Rogue.NET.Scenario;
using Rogue.NET.Model.Scenario;

namespace Rogue.NET.Model.Generation
{
    public static class TemplateGenerator //TODO: inherit from GEneratorBase
    {
        public static SymbolDetails GenerateSymbol(SymbolDetailsTemplate t)
        {
            switch (t.Type)
            {
                case SymbolTypes.Character:
                    return new SymbolDetails(1, 10, t.CharacterSymbol, t.CharacterColor);
                case SymbolTypes.Image:
                    return new SymbolDetails(1, 10, t.Icon);
                case SymbolTypes.Smiley:
                default:
                    return new SymbolDetails(1, 10, t.SmileyMood, t.SmileyBodyColor, t.SmileyLineColor, t.SmileyAuraColor);
            }
        }
        public static SymbolDetails GenerateDefaultDoodadSymbol(DoodadNormalType type)
        {
            switch (type)
            {
                case DoodadNormalType.SavePoint:
                    return new SymbolDetails(1, 10, ImageResources.SavePoint);

                case DoodadNormalType.StairsDown:
                    return new SymbolDetails(1, 10, ImageResources.StairsDown);

                case DoodadNormalType.StairsUp:
                    return new SymbolDetails(1, 10, ImageResources.StairsUp);

                case DoodadNormalType.Teleport1:
                    return new SymbolDetails(1, 10, ImageResources.teleport1);

                case DoodadNormalType.Teleport2:
                    return new SymbolDetails(1, 10, ImageResources.teleport2);

                case DoodadNormalType.TeleportRandom:
                    return new SymbolDetails(1, 10, ImageResources.TeleportRandom);

                default:
                    throw new Exception("Unhandled default doodad symbol details creation");
            }
        }
        public static Behavior GenerateBehavior(BehaviorTemplate t, ref Random r)
        {
            Behavior b = new Behavior();
            b.AttackType = t.AttackType;
            b.CanOpenDoors = t.CanOpenDoors;
            b.CounterAttackProbability = t.CounterAttackProbability;
            b.CriticalRatio = t.CriticalRatio;
            b.DisengageRadius = t.DisengageRadius;
            b.EnemySkill = GenerateSkill(t.EnemySpell, ref r);
            b.EngageRadius = t.EngageRadius;
            b.MovementType = t.MovementType;
            return b;
        }
        public static Equipment GenerateEquipment(EquipmentTemplate t, ref Random r)
        {
            if (t.IsUnique && t.HasBeenGenerated)
                return null;

            Equipment e = new Equipment();
            if (t.HasAttackSpell)
                e.AttackSpell = GenerateSkill(t.AttackSpell, ref r);

            if (t.HasEquipSpell)
                e.EquipSpell = GenerateSkill(t.EquipSpell, ref r);

            if (t.HasCurseSpell)
                e.CurseSpell = GenerateSkill(t.CurseSpell, ref r);

            e.SymbolInfo = GenerateSymbol(t.SymbolDetails);
            e.HasAttackSpell = t.HasAttackSpell;
            e.HasEquipSpell = t.HasEquipSpell;
            e.HasCurseSpell = t.HasCurseSpell;
            e.Type = t.Type;
            e.Class = t.Class.GetRandomValue(ref r);
            e.IsEquiped = false;
            e.IsCursed = t.IsCursed;
            e.RogueName = t.Name;
            e.AmmoName = t.AmmoTemplate == null ? "" : t.AmmoTemplate.Name;
            e.Weight = t.Weight;
            e.Quality = t.Quality.GetRandomValue(ref r);

            foreach (AttackAttributeTemplate attrib in t.AttackAttributes)
                e.AttackAttributes.Add(new AttackAttribute()
                {
                    RogueName = attrib.Name,
                    SymbolInfo = GenerateSymbol(attrib.SymbolDetails),
                    Attack = attrib.Attack.GetRandomValue(ref r),
                    Resistance = attrib.Resistance.GetRandomValue(ref r),
                    Weakness = attrib.Weakness.GetRandomValue(ref r)
                });

            t.HasBeenGenerated = true;
            return e;
        }
        public static Consumable GenerateConsumable(ConsumableTemplate t, ref Random r)
        {
            if (t.IsUnique && t.HasBeenGenerated)
                return null;

            Consumable c = new Consumable();
            c.RogueName = t.Name;
            c.Spell = GenerateSkill(t.SpellTemplate, ref r);
            c.ProjectileSpell = GenerateSkill(t.ProjectileSpellTemplate, ref r);
            c.LearnedSkill = GenerateSkillSet(t.LearnedSkill, ref r);
            c.AmmoSpell = GenerateSkill(t.AmmoSpellTemplate, ref r);
            c.HasLearnedSkillSet = t.HasLearnedSkill;
            c.HasProjectileSpell = t.IsProjectile;
            c.HasSpell = t.HasSpell;
            c.SymbolInfo = GenerateSymbol(t.SymbolDetails);
            c.Type = t.Type;
            c.SubType = t.SubType;
            c.Weight = t.Weight;
            c.Uses = t.UseCount.GetRandomValue(ref r);
            t.HasBeenGenerated = true;
            return c;
        }
        public static DoodadMagic GenerateDoodad(DoodadTemplate t, ref Random r)
        {
            DoodadMagic d = new DoodadMagic();
            if (t.IsAutomatic)
                d.AutomaticSpell = GenerateSkill(t.AutomaticMagicSpellTemplate, ref r);

            if (t.IsInvoked)
                d.InvokedSpell = GenerateSkill(t.InvokedMagicSpellTemplate, ref r);

            d.IsAutomatic = t.IsAutomatic;
            d.IsHidden = !t.IsVisible;
            d.IsInvoked = t.IsInvoked;
            d.IsOneUse = t.IsOneUse;
            d.RogueName = t.Name;
            d.SymbolInfo = GenerateSymbol(t.SymbolDetails);
            d.HasBeenUsed = false;
            t.HasBeenGenerated = true;
            return d;
        }
        public static Enemy GenerateEnemy(EnemyTemplate t, Random r)
        {
            if (t.IsUnique && t.HasBeenGenerated)
                return null;

            if (t.IsObjectiveItem && t.HasBeenGenerated)
                return null;

            Enemy e = new Enemy();
            e.CreatureClass = t.CreatureClass.Name;
            e.AgilityBase = t.Agility.GetRandomValue(ref r);
            e.ExperienceGiven = t.ExperienceGiven.GetRandomValue(ref r);
            e.Hp = t.Hp.GetRandomValue(ref r);
            e.Mp = t.Mp.GetRandomValue(ref r);
            e.MpMax = e.Mp;
            e.HpMax = e.Hp;
            e.RogueName = t.Name;
            e.StrengthBase = t.Strength.GetRandomValue(ref r);
            e.IntelligenceBase = t.Intelligence.GetRandomValue(ref r);
            e.SymbolInfo = GenerateSymbol(t.SymbolDetails);
            e.BehaviorDetails = new BehaviorDetails();
            e.BehaviorDetails.PrimaryBehavior = GenerateBehavior(t.BehaviorDetails.PrimaryBehavior, ref r);
            e.BehaviorDetails.SecondaryBehavior = GenerateBehavior(t.BehaviorDetails.SecondaryBehavior, ref r);
            e.BehaviorDetails.SecondaryProbability = t.BehaviorDetails.SecondaryProbability;
            e.BehaviorDetails.SecondaryReason = t.BehaviorDetails.SecondaryReason;

            //Attack Attributes
            foreach (AttackAttributeTemplate at in t.AttackAttributes)
                e.AttackAttributes.Add(new AttackAttribute() 
                { 
                    RogueName = at.Name,
                    SymbolInfo = GenerateSymbol(at.SymbolDetails),
                    Attack = at.Attack.GetRandomValue(ref r), 
                    Resistance = at.Resistance.GetRandomValue(ref r),
                    Weakness = at.Weakness.GetRandomValue(ref r)
                });

            //Starting Consumables
            foreach (ProbabilityConsumableTemplate ct in t.StartingConsumables)
            {
                if (r.NextDouble() > ct.GenerationProbability)
                    continue;

                Consumable i = GenerateConsumable((ConsumableTemplate)ct.TheTemplate, ref r);
                if (i != null)
                    e.ConsumableInventory.Add(i);
            }

            //Starting Equipment
            foreach (ProbabilityEquipmentTemplate et in t.StartingEquipment)
            {
                if (r.NextDouble() > et.GenerationProbability)
                    continue;

                Equipment ei = GenerateEquipment((EquipmentTemplate)et.TheTemplate, ref r);
                if (ei != null)
                    e.EquipmentInventory.Add(ei);
            }

            t.HasBeenGenerated = true;
            return e;
        }
        public static Alteration GenerateAlteration(
            Spell spell,
            Random r)
        {
            AlterationCost thecost = new AlterationCost();
            AlterationEffect altEffect = GenerateAlterationEffect(spell.Id, spell.RogueName, spell.DisplayName, spell.EffectRange, spell.Effect, ref r);
            AlterationEffect altAuraEffect = GenerateAlterationEffect(spell.Id, spell.RogueName, spell.DisplayName, spell.EffectRange, spell.AuraEffect, ref r);

            thecost.SpellId = spell.Id;
            thecost.Type = spell.Cost.Type;
            thecost.Agility = spell.Cost.Agility;
            thecost.AuraRadius = spell.Cost.AuraRadius;
            thecost.Experience = spell.Cost.Experience;
            thecost.FoodUsagePerTurn = spell.Cost.FoodUsagePerTurn;
            thecost.Hp = spell.Cost.Hp;
            thecost.Hunger = spell.Cost.Hunger;
            thecost.Intelligence = spell.Cost.Intelligence;
            thecost.Mp = spell.Cost.Mp;
            thecost.Strength = spell.Cost.Strength;
            thecost.SpellName = spell.RogueName;

            Alteration a = new Alteration();
            a.Cost = thecost;
            a.Effect = altEffect;
            a.AuraEffect = altAuraEffect;
            a.RogueName = spell.RogueName;
            a.EffectRange = spell.EffectRange;
            a.OtherEffectType = spell.OtherEffectType;
            a.AttackAttributeType = spell.AttackAttributeType;
            a.BlockType = spell.BlockType;
            a.Type = spell.Type;
            a.Stackable = spell.Stackable;
            a.CreateMonsterEnemy = spell.CreateMonsterEnemyName;
            return a;
        }
        public static AlterationEffect GenerateAlterationEffect(string spellId, string spellName, string spellDisplayName, double effectRange, AlterationEffectTemplate effect, ref Random r)
        {
            AlterationEffect theeffect = new AlterationEffect();

            theeffect.Agility = effect.AgilityRange.GetRandomValue(ref r);
            theeffect.Attack = effect.AttackRange.GetRandomValue(ref r);
            theeffect.AuraRadius = effect.AuraRadiusRange.GetRandomValue(ref r);
            theeffect.Defense = effect.DefenseRange.GetRandomValue(ref r);
            theeffect.DodgeProbability = effect.DodgeProbabilityRange.GetRandomValue(ref r);
            theeffect.Experience = effect.ExperienceRange.GetRandomValue(ref r);
            theeffect.EventTime = effect.EventTime.GetRandomValue(ref r);
            theeffect.FoodUsagePerTurn = effect.FoodUsagePerTurnRange.GetRandomValue(ref r);
            theeffect.Hp = effect.HpRange.GetRandomValue(ref r);
            theeffect.HpPerStep = effect.HpPerStepRange.GetRandomValue(ref r);
            theeffect.Hunger = effect.HungerRange.GetRandomValue(ref r);
            theeffect.Intelligence = effect.IntelligenceRange.GetRandomValue(ref r);
            theeffect.IsSymbolAlteration = effect.IsSymbolAlteration;
            theeffect.MagicBlockProbability = effect.MagicBlockProbabilityRange.GetRandomValue(ref r);
            theeffect.Mp = effect.MpRange.GetRandomValue(ref r);
            theeffect.MpPerStep = effect.MpPerStepRange.GetRandomValue(ref r);
            theeffect.PostEffectString = effect.PostEffectText;
            theeffect.RogueName = effect.Name;
            theeffect.State = effect.StateType;
            theeffect.Strength = effect.StrengthRange.GetRandomValue(ref r);
            theeffect.SymbolAlteration = effect.SymbolAlteration;
            theeffect.RogueName = spellName;
            theeffect.DisplayName = spellDisplayName;
            theeffect.CriticalHit = effect.CriticalHit.GetRandomValue(ref r);
            theeffect.IsSilence = effect.IsSilence;

            //Store list of remedied spells
            theeffect.RemediedSpellNames = effect.RemediedSpells.Select(z => z.Name).ToList();

            //Attack Attributes
            foreach (AttackAttributeTemplate at in effect.AttackAttributes)
                theeffect.AttackAttributes.Add(new AttackAttribute()
                {
                    RogueName = at.Name,
                    SymbolInfo = GenerateSymbol(at.SymbolDetails),
                    Attack = at.Attack.GetRandomValue(ref r),
                    Resistance = at.Resistance.GetRandomValue(ref r),
                    Weakness = at.Weakness.GetRandomValue(ref r)
                });

            //This is used as a reference to find the spell that was used to create the
            //effect.
            theeffect.SpellId = spellId;

            //Copied to aura effect so that it can detach from the spell
            theeffect.EffectRange = effectRange;

            return theeffect;
        }
        public static Spell GenerateSkill(SpellTemplate t, ref Random r)
        {
            Spell s = new Spell();
            s.RogueName = t.Name;
            s.Cost = t.Cost;
            s.Effect = t.Effect;
            s.AuraEffect = t.AuraEffect;
            s.Type = t.Type;
            s.BlockType = t.BlockType;
            s.OtherEffectType = t.OtherEffectType;
            s.AttackAttributeType = t.AttackAttributeType;
            s.EffectRange = t.EffectRange;
            s.Animations = t.Animations;
            s.CreateMonsterEnemyName = t.CreateMonsterEnemy;
            s.DisplayName = t.DisplayName;
            return s;
        }
        public static SkillSet GenerateSkillSet(SkillSetTemplate t, ref Random r)
        {
            SkillSet set = new SkillSet(t.Name, ImageResources.Skill1);
            set.LevelLearned = t.LevelLearned;
            foreach (SpellTemplate st in t.Spells)
                set.Skills.Add(GenerateSkill(st, ref r));
            set.SymbolInfo = GenerateSymbol(t.SymbolDetails);
            return set;
        }
        public static Player GeneratePlayer(PlayerTemplate t, ref Random r)
        {
            Player p = new Player();
            p.FoodUsagePerTurnBase = t.FoodUsage.GetRandomValue(ref r);
            p.IntelligenceBase = t.Intelligence.GetRandomValue(ref r);
            p.StrengthBase = t.Strength.GetRandomValue(ref r);
            p.AgilityBase = t.Agility.GetRandomValue(ref r);
            p.HpMax = t.Hp.GetRandomValue(ref r);
            p.MpMax = t.Mp.GetRandomValue(ref r);
            p.Hp = p.HpMax;
            p.Mp = p.MpMax;
            p.AuraRadiusBase = t.AuraRadius;

            p.SymbolInfo = new SymbolDetails(1, 10, t.SymbolDetails.SmileyMood, t.SymbolDetails.SmileyBodyColor, t.SymbolDetails.SmileyLineColor, t.SymbolDetails.SmileyAuraColor);
            p.Class = t.Class;
            p.Experience = 0;
            p.Hunger = 0;
            p.Level = 0;

            //Starting Consumables
            foreach (ProbabilityConsumableTemplate pt in t.StartingConsumables)
            {
                int num = CalculateGenerationNumber(pt.GenerationProbability, ref r);
                for (int i = 0; i < num; i++)
                {
                    Consumable c = GenerateConsumable((ConsumableTemplate)pt.TheTemplate, ref r);
                    if (c != null)
                        p.ConsumableInventory.Add(c);
                }
            }

            //Starting Equipment
            foreach (ProbabilityEquipmentTemplate et in t.StartingEquipment)
            {
                int num = CalculateGenerationNumber(et.GenerationProbability, ref r);
                for (int i = 0; i < num; i++)
                {
                    Equipment e = GenerateEquipment((EquipmentTemplate)et.TheTemplate, ref r);
                    if (e != null)
                    {
                        e.IsEquiped = et.EquipOnStartup;
                        p.EquipmentInventory.Add(e);
                    }
                }
            }

            //Starting Skills
            foreach (SkillSetTemplate st in t.Skills)
                p.Skills.Add(GenerateSkillSet(st, ref r));

            foreach (SkillSet s in p.Skills)
                s.IsLearned = (s.LevelLearned <= p.Level);

            return p;
        }

        /// <summary>
        /// Generates a single consumable for a given probability 0 < x < 1
        /// </summary>
        public static Consumable GenerateProbabilityConsumable(ProbabilityConsumableTemplate pt, ref Random r)
        {
            int num = CalculateGenerationNumber(pt.GenerationProbability, ref r);
            return (num > 0) ? GenerateConsumable((ConsumableTemplate)pt.TheTemplate, ref r) : null;
        }
        /// <summary>
        /// Generates a single equipment for a given probability 0 < x < 1
        /// </summary>
        public static Equipment GenerateProbabilityEquipment(ProbabilityEquipmentTemplate pt, ref Random r, bool equipOnStartup = false)
        {
            int num = CalculateGenerationNumber(pt.GenerationProbability, ref r);
            var equipment = (num > 0) ? GenerateEquipment((EquipmentTemplate)pt.TheTemplate, ref r) : null;
            if (equipment != null)
                equipment.IsEquiped = pt.EquipOnStartup && equipOnStartup;
            return equipment;
        }

        private static int CalculateGenerationNumber(double rate, ref Random r)
        {
            int i = (int)rate;
            double d = rate - i;
            int ct = i;
            if (d > 0 && d > r.NextDouble())
                ct++;

            return ct;
        }
    }
}
