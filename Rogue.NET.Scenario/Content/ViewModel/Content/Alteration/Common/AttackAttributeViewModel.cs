using Rogue.NET.Core.Model.Scenario.Alteration.Common;
using Rogue.NET.Core.Model.ScenarioConfiguration.Content;
using Rogue.NET.Scenario.Content.ViewModel.Content.ScenarioMetaData;

namespace Rogue.NET.Scenario.Content.ViewModel.Content.Alteration.Common
{
    public class AttackAttributeViewModel : ScenarioImageViewModel
    {
        double _attack;
        double _resistance;
        double _weakness;
        bool _immune;

        double _attackHigh;
        double _resistanceHigh;
        double _weaknessHigh;

        public AttackAttributeViewModel(AttackAttribute attackAttribute) : base(attackAttribute, attackAttribute.RogueName)
        {
            this.Attack = attackAttribute.Attack;
            this.Resistance = attackAttribute.Resistance;
            this.Weakness = attackAttribute.Weakness;
            this.Immune = attackAttribute.Immune;
        }

        public AttackAttributeViewModel(AttackAttributeTemplate attackAttributeTemplate) 
            : base(attackAttributeTemplate.Guid, attackAttributeTemplate.Name, attackAttributeTemplate.Name, attackAttributeTemplate.SymbolDetails)
        {
            this.Attack = attackAttributeTemplate.Attack.Low;
            this.AttackHigh = attackAttributeTemplate.Attack.High;
            this.Resistance = attackAttributeTemplate.Resistance.Low;
            this.ResistanceHigh = attackAttributeTemplate.Resistance.High;
            this.Weakness = attackAttributeTemplate.Weakness.Low;
            this.WeaknessHigh = attackAttributeTemplate.Weakness.High;
            this.Immune = attackAttributeTemplate.Immune;
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
        public bool Immune
        {
            get { return _immune; }
            set { this.RaiseAndSetIfChanged(ref _immune, value); }
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

        public bool IsSet()
        {
            return this.Attack != 0D ||
                   this.Resistance != 0D ||
                   this.Weakness != 0D;
        }
    }
}
