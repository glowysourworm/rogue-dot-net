using Rogue.NET.ScenarioEditor.ViewModel.Attribute;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Abstract;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Alteration.Interface;
using Rogue.NET.ScenarioEditor.Views.Assets.SharedControl.AlterationControl.EffectControl;
using System;

namespace Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Alteration.Common
{
    [Serializable]
    [UIType(DisplayName = "Temporary",
            Description = "Causes a temporary change to a character's stats",
            ViewType = typeof(TemporaryEffectParameters),
            BaseType = UITypeAttributeBaseType.Alteration)]
    public class TemporaryAlterationEffectTemplateViewModel 
        : TemplateViewModel, IConsumableAlterationEffectTemplateViewModel, 
                             IConsumableProjectileAlterationEffectTemplateViewModel,
                             IDoodadAlterationEffectTemplateViewModel,
                             IEnemyAlterationEffectTemplateViewModel,
                             IFriendlyAlterationEffectTemplateViewModel,
                             ITemporaryCharacterAlterationEffectTemplateViewModel,
                             ISkillAlterationEffectTemplateViewModel
    {
        private SymbolEffectTemplateViewModel _symbolAlteration;
        private bool _canSeeInvisibleCharacters;
        private RangeViewModel<int> _eventTime;
        private AlteredCharacterStateTemplateViewModel _alteredState;
        private RangeViewModel<double> _strengthRange;
        private RangeViewModel<double> _intelligenceRange;
        private RangeViewModel<double> _agilityRange;
        private RangeViewModel<double> _speedRange;
        private RangeViewModel<double> _lightRadiusRange;
        private RangeViewModel<double> _visionRange;
        private RangeViewModel<double> _foodUsagePerTurnRange;
        private RangeViewModel<double> _hpPerStepRange;
        private RangeViewModel<double> _healthPerStepRange;
        private RangeViewModel<double> _staminaPerStepRange;
        private RangeViewModel<double> _attackRange;
        private RangeViewModel<double> _defenseRange;
        private bool _isStackable;
        private bool _hasAlteredState;

        public SymbolEffectTemplateViewModel SymbolAlteration
        {
            get { return _symbolAlteration; }
            set { this.RaiseAndSetIfChanged(ref _symbolAlteration, value); }
        }
        public bool CanSeeInvisibleCharacters
        {
            get { return _canSeeInvisibleCharacters; }
            set { this.RaiseAndSetIfChanged(ref _canSeeInvisibleCharacters, value); }
        }
        public RangeViewModel<int> EventTime
        {
            get { return _eventTime; }
            set { this.RaiseAndSetIfChanged(ref _eventTime, value); }
        }
        public AlteredCharacterStateTemplateViewModel AlteredState
        {
            get { return _alteredState; }
            set { this.RaiseAndSetIfChanged(ref _alteredState, value); }
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
        public RangeViewModel<double> VisionRange
        {
            get { return _visionRange; }
            set { this.RaiseAndSetIfChanged(ref _visionRange, value); }
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
        public bool IsStackable
        {
            get { return _isStackable; }
            set { this.RaiseAndSetIfChanged(ref _isStackable, value); }
        }
        public bool HasAlteredState
        {
            get { return _hasAlteredState; }
            set { this.RaiseAndSetIfChanged(ref _hasAlteredState, value); }
        }


        public TemporaryAlterationEffectTemplateViewModel()
        {
            this.SymbolAlteration = new SymbolEffectTemplateViewModel();
            this.EventTime = new RangeViewModel<int>(20, 30);

            this.AgilityRange = new RangeViewModel<double>(0, 0);
            this.AttackRange = new RangeViewModel<double>(0, 0);
            this.LightRadiusRange = new RangeViewModel<double>(0, 0);
            this.VisionRange = new RangeViewModel<double>(0, 0);
            this.DefenseRange = new RangeViewModel<double>(0, 0);
            this.FoodUsagePerTurnRange = new RangeViewModel<double>(0, 0);
            this.HpPerStepRange = new RangeViewModel<double>(0, 0);
            this.HealthPerStepRange = new RangeViewModel<double>(0, 0);
            this.IntelligenceRange = new RangeViewModel<double>(0, 0);
            this.SpeedRange = new RangeViewModel<double>(0, 0);
            this.StaminaPerStepRange = new RangeViewModel<double>(0, 0);
            this.StrengthRange = new RangeViewModel<double>(0, 0);

            this.AlteredState = new AlteredCharacterStateTemplateViewModel();
        }
    }
}
