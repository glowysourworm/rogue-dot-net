using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Abstract;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Alteration.Interface;
using System;

namespace Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Alteration.Common
{
    [Serializable]
    public class PassiveAlterationEffectTemplateViewModel 
        : TemplateViewModel, IEquipmentCurseAlterationEffectTemplateViewModel,
                    IEquipmentEquipAlterationEffectTemplateViewModel,
                    ISkillAlterationEffectTemplateViewModel
    {
        private SymbolDeltaTemplateViewModel _symbolAlteration;
        private bool _canSeeInvisibleCharacters;
        private RangeViewModel<double> _strengthRange;
        private RangeViewModel<double> _intelligenceRange;
        private RangeViewModel<double> _agilityRange;
        private RangeViewModel<double> _speedRange;
        private RangeViewModel<double> _lightRadiusRange;
        private RangeViewModel<double> _foodUsagePerTurnRange;
        private RangeViewModel<double> _hpPerStepRange;
        private RangeViewModel<double> _mpPerStepRange;
        private RangeViewModel<double> _attackRange;
        private RangeViewModel<double> _defenseRange;
        private RangeViewModel<double> _magicBlockProbabilityRange;
        private RangeViewModel<double> _dodgeProbabilityRange;
        private RangeViewModel<double> _experienceRange;
        private RangeViewModel<double> _hungerRange;
        private RangeViewModel<double> _hpRange;
        private RangeViewModel<double> _mpRange;
        private RangeViewModel<double> _criticalHit;

        public SymbolDeltaTemplateViewModel SymbolAlteration
        {
            get { return _symbolAlteration; }
            set { this.RaiseAndSetIfChanged(ref _symbolAlteration, value); }
        }
        public bool CanSeeInvisibleCharacters
        {
            get { return _canSeeInvisibleCharacters; }
            set { this.RaiseAndSetIfChanged(ref _canSeeInvisibleCharacters, value); }
        }
        public RangeViewModel<double> StrengthRange
        {
            get { return _strengthRange; }
            set { this.RaiseAndSetIfChanged(ref _strengthRange, value); }
        }
        public RangeViewModel<double> IntelligenceRange
        {
            get { return _intelligenceRange; }
            set { this.RaiseAndSetIfChanged(ref _intelligenceRange, value); }
        }
        public RangeViewModel<double> AgilityRange
        {
            get { return _agilityRange; }
            set { this.RaiseAndSetIfChanged(ref _agilityRange, value); }
        }
        public RangeViewModel<double> SpeedRange
        {
            get { return _speedRange; }
            set { this.RaiseAndSetIfChanged(ref _speedRange, value); }
        }
        public RangeViewModel<double> LightRadiusRange
        {
            get { return _lightRadiusRange; }
            set { this.RaiseAndSetIfChanged(ref _lightRadiusRange, value); }
        }
        public RangeViewModel<double> FoodUsagePerTurnRange
        {
            get { return _foodUsagePerTurnRange; }
            set { this.RaiseAndSetIfChanged(ref _foodUsagePerTurnRange, value); }
        }
        public RangeViewModel<double> HpPerStepRange
        {
            get { return _hpPerStepRange; }
            set { this.RaiseAndSetIfChanged(ref _hpPerStepRange, value); }
        }
        public RangeViewModel<double> MpPerStepRange
        {
            get { return _mpPerStepRange; }
            set { this.RaiseAndSetIfChanged(ref _mpPerStepRange, value); }
        }
        public RangeViewModel<double> AttackRange
        {
            get { return _attackRange; }
            set { this.RaiseAndSetIfChanged(ref _attackRange, value); }
        }
        public RangeViewModel<double> DefenseRange
        {
            get { return _defenseRange; }
            set { this.RaiseAndSetIfChanged(ref _defenseRange, value); }
        }
        public RangeViewModel<double> MagicBlockProbabilityRange
        {
            get { return _magicBlockProbabilityRange; }
            set { this.RaiseAndSetIfChanged(ref _magicBlockProbabilityRange, value); }
        }
        public RangeViewModel<double> DodgeProbabilityRange
        {
            get { return _dodgeProbabilityRange; }
            set { this.RaiseAndSetIfChanged(ref _dodgeProbabilityRange, value); }
        }
        public RangeViewModel<double> ExperienceRange
        {
            get { return _experienceRange; }
            set { this.RaiseAndSetIfChanged(ref _experienceRange, value); }
        }
        public RangeViewModel<double> HungerRange
        {
            get { return _hungerRange; }
            set { this.RaiseAndSetIfChanged(ref _hungerRange, value); }
        }
        public RangeViewModel<double> HpRange
        {
            get { return _hpRange; }
            set { this.RaiseAndSetIfChanged(ref _hpRange, value); }
        }
        public RangeViewModel<double> MpRange
        {
            get { return _mpRange; }
            set { this.RaiseAndSetIfChanged(ref _mpRange, value); }
        }
        public RangeViewModel<double> CriticalHit
        {
            get { return _criticalHit; }
            set { this.RaiseAndSetIfChanged(ref _criticalHit, value); }
        }

        public PassiveAlterationEffectTemplateViewModel()
        {
            this.SymbolAlteration = new SymbolDeltaTemplateViewModel();

            this.AgilityRange = new RangeViewModel<double>(-100, 0, 0, 100);
            this.SpeedRange = new RangeViewModel<double>(-1, 0, 0, 1);
            this.AttackRange = new RangeViewModel<double>(-100, 0, 0, 100);
            this.LightRadiusRange = new RangeViewModel<double>(-25, 0, 0, 25);
            this.DefenseRange = new RangeViewModel<double>(-100, 0, 0, 100);
            this.DodgeProbabilityRange = new RangeViewModel<double>(-1, 0, 0, 1);
            this.ExperienceRange = new RangeViewModel<double>(-100000, 0, 0, 100000);
            this.FoodUsagePerTurnRange = new RangeViewModel<double>(-10, 0, 0, 10);
            this.HpPerStepRange = new RangeViewModel<double>(-100, 0, 0, 100);
            this.HpRange = new RangeViewModel<double>(-1000, 0, 0, 1000);
            this.HungerRange = new RangeViewModel<double>(-100, 0, 0, 100);
            this.IntelligenceRange = new RangeViewModel<double>(-100, 0, 0, 100);
            this.MagicBlockProbabilityRange = new RangeViewModel<double>(-1, 0, 0, 1);
            this.MpPerStepRange = new RangeViewModel<double>(-100, 0, 0, 100);
            this.MpRange = new RangeViewModel<double>(-100, 0, 0, 100);
            this.StrengthRange = new RangeViewModel<double>(-100, 0, 0, 100);

            this.CriticalHit = new RangeViewModel<double>(-1, 0, 0, 1);
        }
    }
}
