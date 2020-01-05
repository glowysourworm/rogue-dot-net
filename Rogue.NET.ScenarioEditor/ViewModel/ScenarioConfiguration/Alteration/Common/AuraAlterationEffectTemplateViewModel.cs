using Rogue.NET.ScenarioEditor.ViewModel.Attribute;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Abstract;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Alteration.Interface;
using Rogue.NET.ScenarioEditor.Views.Assets.SharedControl.AlterationControl.EffectControl;
using System;

namespace Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Alteration.Common
{
    [Serializable]
    [UIType(DisplayName = "Aura",
            Description = "Causes a stat change to characters in range of the source character",
            ViewType = typeof(AuraEffectParameters),
            BaseType = UITypeAttributeBaseType.Alteration)]
    public class AuraAlterationEffectTemplateViewModel 
        : TemplateViewModel, IEquipmentCurseAlterationEffectTemplateViewModel,
                             IEquipmentEquipAlterationEffectTemplateViewModel,
                             ISkillAlterationEffectTemplateViewModel
    {
        private SymbolEffectTemplateViewModel _symbolAlteration;
        private RangeViewModel<double> _strengthRange;
        private RangeViewModel<double> _intelligenceRange;
        private RangeViewModel<double> _agilityRange;
        private RangeViewModel<double> _speedRange;
        private RangeViewModel<double> _hpPerStepRange;
        private RangeViewModel<double> _healthPerStepRange;
        private RangeViewModel<double> _staminaPerStepRange;
        private RangeViewModel<double> _attackRange;
        private RangeViewModel<double> _defenseRange;

        public SymbolEffectTemplateViewModel SymbolAlteration
        {
            get { return _symbolAlteration; }
            set { this.RaiseAndSetIfChanged(ref _symbolAlteration, value); }
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
        public RangeViewModel<double> HpPerStepRange
        {
            get { return _hpPerStepRange; }
            set { this.RaiseAndSetIfChanged(ref _hpPerStepRange, value); }
        }
        public RangeViewModel<double> HealthPerStepRange
        {
            get { return _healthPerStepRange; }
            set { this.RaiseAndSetIfChanged(ref _healthPerStepRange, value); }
        }
        public RangeViewModel<double> StaminaPerStepRange
        {
            get { return _staminaPerStepRange; }
            set { this.RaiseAndSetIfChanged(ref _staminaPerStepRange, value); }
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

        public AuraAlterationEffectTemplateViewModel()
        {
            this.SymbolAlteration = new SymbolEffectTemplateViewModel();

            this.AgilityRange = new RangeViewModel<double>(0, 0);
            this.SpeedRange = new RangeViewModel<double>(0, 0);
            this.AttackRange = new RangeViewModel<double>(0, 0);
            this.DefenseRange = new RangeViewModel<double>(0, 0);
            this.HpPerStepRange = new RangeViewModel<double>(0, 0);
            this.HealthPerStepRange = new RangeViewModel<double>(0, 0);
            this.IntelligenceRange = new RangeViewModel<double>(0, 0);
            this.StaminaPerStepRange = new RangeViewModel<double>(0, 0);
            this.StrengthRange = new RangeViewModel<double>(0, 0);
        }
    }
}
