using Rogue.NET.Core.Model.Enums;
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
        private SymbolDeltaTemplate _symbolAlteration;
        private Range<double> _strengthRange;
        private Range<double> _intelligenceRange;
        private Range<double> _agilityRange;
        private Range<double> _speedRange;
        private Range<double> _hpPerStepRange;
        private Range<double> _mpPerStepRange;
        private Range<double> _attackRange;
        private Range<double> _defenseRange;
        private Range<double> _magicBlockProbabilityRange;
        private Range<double> _dodgeProbabilityRange;

        public SymbolDeltaTemplate SymbolAlteration
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
        public Range<double> HpPerStepRange
        {
            get { return _hpPerStepRange; }
            set
            {
                if (_hpPerStepRange != value)
                {
                    _hpPerStepRange = value;
                    OnPropertyChanged("HpPerStepRange");
                }
            }
        }
        public Range<double> MpPerStepRange
        {
            get { return _mpPerStepRange; }
            set
            {
                if (_mpPerStepRange != value)
                {
                    _mpPerStepRange = value;
                    OnPropertyChanged("MpPerStepRange");
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
        public Range<double> MagicBlockProbabilityRange
        {
            get { return _magicBlockProbabilityRange; }
            set
            {
                if (_magicBlockProbabilityRange != value)
                {
                    _magicBlockProbabilityRange = value;
                    OnPropertyChanged("MagicBlockProbabilityRange");
                }
            }
        }
        public Range<double> DodgeProbabilityRange
        {
            get { return _dodgeProbabilityRange; }
            set
            {
                if (_dodgeProbabilityRange != value)
                {
                    _dodgeProbabilityRange = value;
                    OnPropertyChanged("DodgeProbabilityRange");
                }
            }
        }

        public AuraAlterationEffectTemplate()
        {
            this.SymbolAlteration = new SymbolDeltaTemplate();

            this.AgilityRange = new Range<double>(0, 0);
            this.SpeedRange = new Range<double>(0, 0);
            this.AttackRange = new Range<double>(0, 0);
            this.DefenseRange = new Range<double>(0, 0);
            this.DodgeProbabilityRange = new Range<double>(0, 0);
            this.HpPerStepRange = new Range<double>(0, 0);
            this.IntelligenceRange = new Range<double>(0, 0);
            this.MagicBlockProbabilityRange = new Range<double>(0, 0);
            this.MpPerStepRange = new Range<double>(0, 0);
            this.StrengthRange = new Range<double>(0, 0);
        }
    }
}
