﻿using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Scenario.Alteration;
using Rogue.NET.Core.Model.ScenarioConfiguration.Content;
using Rogue.NET.Scenario.Content.ViewModel.Content.ScenarioMetaData;

namespace Rogue.NET.Scenario.Content.ViewModel.Content.Alteration
{
    public class AttackAttributeViewModel : ScenarioImageViewModel
    {
        double _attack;
        double _resistance;
        double _weakness;

        double _attackHigh;
        double _resistanceHigh;
        double _weaknessHigh;

        public AttackAttributeViewModel(AttackAttribute attackAttribute) : base(attackAttribute)
        {
            this.Attack = attackAttribute.Attack;
            this.Resistance = attackAttribute.Resistance;
            this.Weakness = attackAttribute.Weakness;
        }

        public AttackAttributeViewModel(AttackAttributeTemplate attackAttributeTemplate) 
            : base(attackAttributeTemplate.Guid, attackAttributeTemplate.Name, attackAttributeTemplate.SymbolDetails)
        {
            this.Attack = attackAttributeTemplate.Attack.Low;
            this.AttackHigh = attackAttributeTemplate.Attack.High;
            this.Resistance = attackAttributeTemplate.Resistance.Low;
            this.ResistanceHigh = attackAttributeTemplate.Resistance.High;
            this.Weakness = attackAttributeTemplate.Weakness.Low;
            this.WeaknessHigh = attackAttributeTemplate.Weakness.High;
        }

        public double Attack
        {
            get { return _attack; }
            set { this.RaiseAndSetIfChanged(ref _attack, value); }
        }
        public double Resistance
        {
            get { return _resistance; }
            set { this.RaiseAndSetIfChanged(ref _resistance, value); }
        }
        public double Weakness
        {
            get { return _weakness; }
            set { this.RaiseAndSetIfChanged(ref _weakness, value); }
        }

        // Use for the case of a range (to show)
        public double AttackHigh
        {
            get { return _attackHigh; }
            set { this.RaiseAndSetIfChanged(ref _attackHigh, value); }
        }
        public double ResistanceHigh
        {
            get { return _resistanceHigh; }
            set { this.RaiseAndSetIfChanged(ref _resistanceHigh, value); }
        }
        public double WeaknessHigh
        {
            get { return _weaknessHigh; }
            set { this.RaiseAndSetIfChanged(ref _weaknessHigh, value); }
        }
    }
}
