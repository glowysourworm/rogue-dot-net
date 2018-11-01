using Rogue.NET.Core.Logic.Content.Interface;
using Rogue.NET.Core.Model;
using Rogue.NET.Core.Model.Scenario.Character;
using System;
using System.ComponentModel.Composition;
using System.Linq;

namespace Rogue.NET.Core.Logic.Content
{
    [Export(typeof(ICharacterProcessor))]
    public class CharacterProcessor : ICharacterProcessor
    {
        public CharacterProcessor() { }

        public double GetMagicBlockBase(Character character)
        {
            return character.IntelligenceBase / 100;
        }
        public double GetDodgeBase(Character character)
        {
            return character.AgilityBase / 100;
        }
        public double GetHaulMax(Character character)
        {
            return character.StrengthBase * ModelConstants.HAUL_STRENGTH_MULTIPLIER;
        }
        public double GetMpRegen(Character character)
        {
            var result = character.MpRegenBase;

            // TODO
            //foreach (AlterationEffect alt in this.Alterations.Where(alt => alt.ProjectCharacterCanSupport(this)))
            //    result += alt.MpPerStep;

            return result;
        }
        public double GetHpRegen(Character character, bool regenerate)
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
        public double GetStrength(Character character)
        {
            double result = character.StrengthBase;

            //foreach (AlterationEffect alt in this.Alterations.Where(alt => alt.ProjectCharacterCanSupport(this)))
            //    d += alt.Strength;

            return Math.Max(0.1, result);
        }
        public double GetAgility(Character character)
        {
            double d = character.AgilityBase;

            //foreach (AlterationEffect alt in this.Alterations.Where(alt => alt.ProjectCharacterCanSupport(this)))
            //    d += alt.Agility;

            return Math.Max(0.1, d);
        }
        public double GetIntelligence(Character character)
        {
            double d = character.IntelligenceBase;

            //foreach (AlterationEffect alt in this.Alterations.Where(alt => alt.ProjectCharacterCanSupport(this)))
            //    d += alt.Intelligence;

            return Math.Max(0.1, d);
        }
        public double GetAuraRadius(Character character)
        {
            double d = character.AuraRadiusBase;

            //foreach (AlterationEffect alt in this.Alterations.Where(alt => alt.ProjectCharacterCanSupport(this)))
            //    d += alt.AuraRadius;

            return Math.Max(0, d);
        }
        public double GetMagicBlock(Character character)
        {
            double d = this.GetIntelligence(character) / 100;

            //foreach (AlterationEffect alt in this.Alterations.Where(alt => alt.ProjectCharacterCanSupport(this)))
            //    d += alt.MagicBlockProbability;

            return Math.Max(Math.Min(1, d), 0);
        }
        public double GetDodge(Character character)
        {
            double d = this.GetAgility(character) / 100;

            //foreach (AlterationEffect alt in this.Alterations.Where(alt => alt.ProjectCharacterCanSupport(this)))
            //    d += alt.DodgeProbability;

            return Math.Max(Math.Min(1, d), 0);
        }
        public double GetHaul(Character character)
        {
            return character.Equipment.Values.Sum(x => x.Weight) +
                   character.Consumables.Values.Sum(x => x.Weight);
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
