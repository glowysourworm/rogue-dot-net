using Rogue.NET.Core.Model.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.NET.Core.Model.Scenario.Character.Extension
{
    public static class CharacterExtension
    {
        public static double GetMagicBlockBase(this Character character)
        {
            return character.IntelligenceBase / 100;
        }
        public static double GetDodgeBase(this Character character)
        {
            return character.AgilityBase / 100;
        }
        public static double GetHaulMax(this Character character)
        {
            return character.StrengthBase * ModelConstants.HaulMaxStrengthMultiplier;
        }
        public static double GetMpRegen(this Character character)
        {
            var result = character.MpRegenBase;

            // TODO
            //foreach (AlterationEffect alt in this.Alterations.Where(alt => alt.ProjectCharacterCanSupport(this)))
            //    result += alt.MpPerStep;

            return result;
        }
        public static double GetHpRegen(this Character character, bool regenerate)
        {
            double d = regenerate ? character.HpRegenBase : 0;

            ////Normal alteration effects
            //if (regenerate)
            //{
            //    foreach (AlterationEffect alt in this.Alterations.Where(alt => alt.ProjectCharacterCanSupport(this)))
            //        d += alt.HpPerStep;
            //}

            ////Malign attack attribute contributions
            //foreach (AlterationEffect malignEffect in this.AttackAttributeTemporaryMalignEffects)
            //{
            //    foreach (AttackAttribute malignAttribute in malignEffect.AttackAttributes)
            //    {
            //        double resistance = 0;
            //        double weakness = 0;
            //        double attack = malignAttribute.Attack;

            //        //Friendly attack attribute contributions
            //        foreach (AlterationEffect friendlyEffect in this.AttackAttributeTemporaryFriendlyEffects)
            //        {
            //            resistance += friendlyEffect.AttackAttributes.First(z => z.RogueName == malignAttribute.RogueName).Resistance;
            //            weakness += friendlyEffect.AttackAttributes.First(z => z.RogueName == malignAttribute.RogueName).Weakness;

            //        }

            //        //Equipment contributions
            //        foreach (Equipment e in this.EquipmentInventory.Where(z => z.IsEquiped))
            //        {
            //            resistance += e.AttackAttributes.First(z => z.RogueName == malignAttribute.RogueName).Resistance;
            //            weakness += e.AttackAttributes.First(z => z.RogueName == malignAttribute.RogueName).Weakness;
            //        }

            //        d -= Calculator.CalculateAttackAttributeMelee(attack, resistance, weakness);
            //    }
            //}

            return d;
        }
        public static double GetStrength(this Character character)
        {
            double result = character.StrengthBase;

            //foreach (AlterationEffect alt in this.Alterations.Where(alt => alt.ProjectCharacterCanSupport(this)))
            //    d += alt.Strength;

            return Math.Max(0.1, result);
        }
        public static double GetStrengthBase(this Character character)
        {
            return character.StrengthBase;
        }
        public static double GetAgility(this Character character)
        {
            double d = character.AgilityBase;

            //foreach (AlterationEffect alt in this.Alterations.Where(alt => alt.ProjectCharacterCanSupport(this)))
            //    d += alt.Agility;

            return Math.Max(0.1, d);
        }
        public static double GetIntelligence(this Character character)
        {
            double d = character.IntelligenceBase;

            //foreach (AlterationEffect alt in this.Alterations.Where(alt => alt.ProjectCharacterCanSupport(this)))
            //    d += alt.Intelligence;

            return Math.Max(0.1, d);
        }
        public static double GetAuraRadius(this Character character)
        {
            double d = character.AuraRadiusBase;

            //foreach (AlterationEffect alt in this.Alterations.Where(alt => alt.ProjectCharacterCanSupport(this)))
            //    d += alt.AuraRadius;

            return Math.Max(0, d);
        }
        public static double GetMagicBlock(this Character character)
        {
            double d = GetIntelligence(character) / 100;

            //foreach (AlterationEffect alt in this.Alterations.Where(alt => alt.ProjectCharacterCanSupport(this)))
            //    d += alt.MagicBlockProbability;

            return Math.Max(Math.Min(1, d), 0);
        }
        public static double GetDodge(this Character character)
        {
            double d = GetAgility(character) / 100;

            //foreach (AlterationEffect alt in this.Alterations.Where(alt => alt.ProjectCharacterCanSupport(this)))
            //    d += alt.DodgeProbability;

            return Math.Max(Math.Min(1, d), 0);
        }
        public static double GetHaul(this Character character)
        {
            return character.Equipment.Values.Sum(x => x.Weight) +
                   character.Consumables.Values.Sum(x => x.Weight);
        }

        public static double GetAttack(this Character character)
        {
            double a = character.GetStrength();

            foreach (var equipment in character.Equipment.Values.Where(x => x.IsEquipped))
            {
                switch (equipment.Type)
                {
                    case EquipmentType.OneHandedMeleeWeapon:
                        a += ((equipment.Class + 1) * equipment.Quality);
                        break;
                    case EquipmentType.TwoHandedMeleeWeapon:
                        a += ((equipment.Class + 1) * equipment.Quality) * 2;
                        break;
                    case EquipmentType.RangeWeapon:
                        a += ((equipment.Class + 1) * equipment.Quality) / 2;
                        break;
                }
            }

            // TODO
            //foreach (AlterationEffect alt in this.Alterations.Where(alt => alt.ProjectCharacterCanSupport(this)))
            //    a += alt.Attack;

            return Math.Max(0, a);
        }
        public static double GetAttackBase(this Character character)
        {
            return character.StrengthBase;
        }
        public static double GetDefense(this Character character)
        {
            double defense = character.GetStrength() / 5.0D;

            foreach (var equipment in character.Equipment.Values.Where(x => x.IsEquipped))
            {
                switch (equipment.Type)
                {
                    case EquipmentType.Armor:
                        defense += ((equipment.Class + 1) * equipment.Quality);
                        break;
                    case EquipmentType.Shoulder:
                        defense += (((equipment.Class + 1) * equipment.Quality) / 5);
                        break;
                    case EquipmentType.Boots:
                        defense += (((equipment.Class + 1) * equipment.Quality) / 10);
                        break;
                    case EquipmentType.Gauntlets:
                        defense += (((equipment.Class + 1) * equipment.Quality) / 10);
                        break;
                    case EquipmentType.Belt:
                        defense += (((equipment.Class + 1) * equipment.Quality) / 8);
                        break;
                    case EquipmentType.Shield:
                        defense += (((equipment.Class + 1) * equipment.Quality) / 5);
                        break;
                    case EquipmentType.Helmet:
                        defense += (((equipment.Class + 1) * equipment.Quality) / 5);
                        break;
                }
            }

            // TODO
            //foreach (AlterationEffect alt in this.Alterations.Where(alt => alt.ProjectCharacterCanSupport(this)))
            //    defense += alt.Defense;

            return Math.Max(0, defense);
        }
        public static double GetDefenseBase(this Character character)
        {
            return character.GetStrengthBase() / 5.0D;
        }

        public static double GetCriticalHitProbability(this Character character)
        {
            double d = ModelConstants.CRITICAL_HIT_BASE;

            // TODO
            //foreach (AlterationEffect alt in this.Alterations.Where(alt => alt.ProjectCharacterCanSupport(this)))
            //    d += alt.CriticalHit;

            return Math.Max(0, d);
        }

        // TODO
        //public bool IsMuted
        //{
        //    get
        //    {
        //        return this.States.Any(z => z == CharacterStateType.Silenced);
        //    }
        //}

        // TODO
        // collection affects other properties - raise property changed for affected properties
        //protected virtual void Alterations_CollectionAltered(object sender, CollectionAlteredEventArgs e)
        //{
        //    foreach (var property in _boundProperties)
        //        OnPropertyChanged(property);

        //    var affectedSymbol = this.Alterations.Count() == 0 ? this.SavedSymbolInfo : base.SymbolInfo;

        //    //Combine symbol detail deltas
        //    foreach (var effect in this.Alterations.Where(alt => alt.ProjectCharacterCanSupport(this)).Where(z => z.IsSymbolAlteration))
        //    {
        //        //Returns a new instance with combined traits from the parents
        //        affectedSymbol = Calculator.CalculateSymbolDelta(effect.SymbolAlteration, affectedSymbol);

        //        //Combine ID's to identify the alteration
        //        affectedSymbol.Id += effect.SymbolAlteration.Guid;
        //    }
        //    this.SymbolInfo = affectedSymbol;
        //    InvalidateVisual();
        //}
    }
}
