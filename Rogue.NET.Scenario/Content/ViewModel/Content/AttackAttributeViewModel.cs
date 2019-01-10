using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Scenario.Alteration;

namespace Rogue.NET.Scenario.Content.ViewModel.Content
{
    public class AttackAttributeViewModel : ScenarioImageViewModel
    {
        double _attack;
        double _resistance;
        double _weakness;

        public AttackAttributeViewModel(AttackAttribute attackAttribute) : base(attackAttribute)
        {
            this.Attack = attackAttribute.Attack;
            this.Resistance = attackAttribute.Resistance;
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
    }
}
