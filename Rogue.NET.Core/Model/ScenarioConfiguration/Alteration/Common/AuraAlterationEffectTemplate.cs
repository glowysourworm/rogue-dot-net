using Rogue.NET.Core.Model.ScenarioConfiguration.Abstract;
using Rogue.NET.Core.Model.ScenarioConfiguration.Alteration.Interface;
using System;

namespace Rogue.NET.Core.Model.ScenarioConfiguration.Alteration.Common
{
    [Serializable]
    public class AuraAlterationEffectTemplate 
        : Template, IEquipmentCurseAlterationEffectTemplate,
                    IEquipmentEquipAlterationEffectTemplate,
                    ISkillAlterationEffectTemplate
    {
        private SymbolEffectTemplate _symbolAlteration;
        private Range<double> _strengthRange;
        private Range<double> _intelligenceRange;
        private Range<double> _agilityRange;
        private Range<double> _speedRange;
        private Range<double> _healthPerStepRange;
        private Range<double> _staminaPerStepRange;
        private Range<double> _attackRange;
        private Range<double> _defenseRange;

        public SymbolEffectTemplate SymbolAlteration
        {
            get { return _symbolAlteration; }
            set
            {
                if (_symbolAlteration != value)
                {
                    _symbolAlteration = value;
                    OnPropertyChanged("SymbolAlteration");
                }
            }
        }
        public Range<double> StrengthRange
        {
            get { return _strengthRange; }
            set
            {
                if (_strengthRange != value)
                {
                    _strengthRange = value;
                    OnPropertyChanged("StrengthRange");
                }
            }
        }
        public Range<double> IntelligenceRange
        {
            get { return _intelligenceRange; }
            set
            {
                if (_intelligenceRange != value)
                {
                    _intelligenceRange = value;
                    OnPropertyChanged("IntelligenceRange");
                }
            }
        }
        public Range<double> AgilityRange
        {
            get { return _agilityRange; }
            set
            {
                if (_agilityRange != value)
                {
                    _agilityRange = value;
                    OnPropertyChanged("AgilityRange");
                }
            }
        }
        public Range<double> SpeedRange
        {
            get { return _speedRange; }
            set
            {
                if (_speedRange != value)
                {
                    _speedRange = value;
                    OnPropertyChanged("SpeedRange");
                }
            }
        }
        public Range<double> HealthPerStepRange
        {
            get { return _healthPerStepRange; }
            set
            {
                if (_healthPerStepRange != value)
                {
                    _healthPerStepRange = value;
                    OnPropertyChanged("HealthPerStepRange");
                }
            }
        }
        public Range<double> StaminaPerStepRange
        {
            get { return _staminaPerStepRange; }
            set
            {
                if (_staminaPerStepRange != value)
                {
                    _staminaPerStepRange = value;
                    OnPropertyChanged("StaminaPerStepRange");
                }
            }
        }
        public Range<double> AttackRange
        {
            get { return _attackRange; }
            set
            {
                if (_attackRange != value)
                {
                    _attackRange = value;
                    OnPropertyChanged("AttackRange");
                }
            }
        }
        public Range<double> DefenseRange
        {
            get { return _defenseRange; }
            set
            {
                if (_defenseRange != value)
                {
                    _defenseRange = value;
                    OnPropertyChanged("DefenseRange");
                }
            }
        }


        public AuraAlterationEffectTemplate()
        {
            this.SymbolAlteration = new SymbolEffectTemplate();

            this.AgilityRange = new Range<double>(0, 0);
            this.SpeedRange = new Range<double>(0, 0);
            this.AttackRange = new Range<double>(0, 0);
            this.DefenseRange = new Range<double>(0, 0);
            this.HealthPerStepRange = new Range<double>(0, 0);
            this.IntelligenceRange = new Range<double>(0, 0);
            this.StaminaPerStepRange = new Range<double>(0, 0);
            this.StrengthRange = new Range<double>(0, 0);
        }
    }
}
