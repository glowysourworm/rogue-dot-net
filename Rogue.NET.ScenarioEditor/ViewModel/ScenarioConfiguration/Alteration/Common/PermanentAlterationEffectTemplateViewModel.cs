using Rogue.NET.ScenarioEditor.ViewModel.Attribute;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Abstract;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Alteration.Interface;
using Rogue.NET.ScenarioEditor.Views.Assets.SharedControl.AlterationControl.EffectControl;
using System;

namespace Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Alteration.Common
{
    [Serializable]
    [UIType(DisplayName = "Permanent",
            Description = "Creates a permanent change to a character's stats",
            ViewType = typeof(PermanentEffectParameters))]
    public class PermanentAlterationEffectTemplateViewModel 
        : TemplateViewModel, IConsumableAlterationEffectTemplateViewModel,
                    IConsumableProjectileAlterationEffectTemplateViewModel,
                    IDoodadAlterationEffectTemplateViewModel,
                    IEnemyAlterationEffectTemplateViewModel,
                    IFriendlyAlterationEffectTemplateViewModel,
                    ITemporaryCharacterAlterationEffectTemplateViewModel,
                    IEquipmentAttackAlterationEffectTemplateViewModel,
                    ISkillAlterationEffectTemplateViewModel
    {
        private RangeViewModel<double> _strengthRange;
        private RangeViewModel<double> _intelligenceRange;
        private RangeViewModel<double> _agilityRange;
        private RangeViewModel<double> _speedRange;
        private RangeViewModel<double> _lightRadiusRange;
        private RangeViewModel<double> _experienceRange;
        private RangeViewModel<double> _hungerRange;
        private RangeViewModel<double> _hpRange;
        private RangeViewModel<double> _mpRange;

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

        public PermanentAlterationEffectTemplateViewModel()
        {
            this.AgilityRange = new RangeViewModel<double>(0, 0);
            this.SpeedRange = new RangeViewModel<double>(0, 0);
            this.LightRadiusRange = new RangeViewModel<double>(0, 0);
            this.ExperienceRange = new RangeViewModel<double>(0, 0);
            this.HpRange = new RangeViewModel<double>(0, 0);
            this.HungerRange = new RangeViewModel<double>(0, 0);
            this.IntelligenceRange = new RangeViewModel<double>(0, 0);
            this.MpRange = new RangeViewModel<double>(0, 0);
            this.StrengthRange = new RangeViewModel<double>(0, 0);
        }
    }
}
