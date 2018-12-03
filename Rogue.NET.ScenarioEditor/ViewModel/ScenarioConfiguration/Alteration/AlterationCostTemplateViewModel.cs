using Rogue.NET.Core.Model.Enums;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Abstract;

namespace Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Alteration
{
    public class AlterationCostTemplateViewModel : TemplateViewModel
    {
        private AlterationCostType _type;
        private double _strength;
        private double _intelligence;
        private double _agility;
        private double _speed;
        private double _foodUsagePerTurn;
        private double _auraRadius;
        private double _experience;
        private double _hunger;
        private double _hp;
        private double _mp;
        private double _hpPerStep;
        private double _mpPerStep;
        private double _hungerPerStep;

        public AlterationCostType Type
        {
            get { return _type; }
            set { this.RaiseAndSetIfChanged(ref _type, value); }
        }
        public double Strength
        {
            get { return _strength; }
            set { this.RaiseAndSetIfChanged(ref _strength, value); }
        }
        public double Intelligence
        {
            get { return _intelligence; }
            set { this.RaiseAndSetIfChanged(ref _intelligence, value); }
        }
        public double Agility
        {
            get { return _agility; }
            set { this.RaiseAndSetIfChanged(ref _agility, value); }
        }
        public double Speed
        {
            get { return _speed; }
            set { this.RaiseAndSetIfChanged(ref _speed, value); }
        }
        public double FoodUsagePerTurn
        {
            get { return _foodUsagePerTurn; }
            set { this.RaiseAndSetIfChanged(ref _foodUsagePerTurn, value); }
        }
        public double AuraRadius
        {
            get { return _auraRadius; }
            set { this.RaiseAndSetIfChanged(ref _auraRadius, value); }
        }
        public double Experience
        {
            get { return _experience; }
            set { this.RaiseAndSetIfChanged(ref _experience, value); }
        }
        public double Hunger
        {
            get { return _hunger; }
            set { this.RaiseAndSetIfChanged(ref _hunger, value); }
        }
        public double Hp
        {
            get { return _hp; }
            set { this.RaiseAndSetIfChanged(ref _hp, value); }
        }
        public double Mp
        {
            get { return _mp; }
            set { this.RaiseAndSetIfChanged(ref _mp, value); }
        }
        public double HpPerStep
        {
            get { return _hpPerStep; }
            set { this.RaiseAndSetIfChanged(ref _hpPerStep, value); }
        }
        public double MpPerStep
        {
            get { return _mpPerStep; }
            set { this.RaiseAndSetIfChanged(ref _mpPerStep, value); }
        }
        public double HungerPerStep
        {
            get { return _hungerPerStep; }
            set { this.RaiseAndSetIfChanged(ref _hungerPerStep, value); }
        }
    }
}
