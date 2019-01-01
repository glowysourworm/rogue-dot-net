﻿using ProtoBuf;
using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.ScenarioConfiguration.Abstract;
using Rogue.NET.Core.Model.ScenarioConfiguration.Content;
using System;
using System.Collections.Generic;

namespace Rogue.NET.Core.Model.ScenarioConfiguration.Alteration
{
    [Serializable]
    [ProtoContract(AsReferenceDefault = true, SkipConstructor = true)]
    public class AlterationEffectTemplate : Template
    {
        private SymbolDetailsTemplate _symbolAlteration;
        private bool _isSymbolAlteration;
        private bool _canSeeInvisibleCharacters;
        private Range<int> _eventTime;
        private AlteredCharacterStateTemplate _alteredState;
        private AlteredCharacterStateTemplate _remediedState;
        private Range<double> _strengthRange;
        private Range<double> _intelligenceRange;
        private Range<double> _agilityRange;
        private Range<double> _speedRange;
        private Range<double> _auraRadiusRange;
        private Range<double> _foodUsagePerTurnRange;
        private Range<double> _hpPerStepRange;
        private Range<double> _mpPerStepRange;
        private Range<double> _attackRange;
        private Range<double> _defenseRange;
        private Range<double> _magicBlockProbabilityRange;
        private Range<double> _dodgeProbabilityRange;
        private Range<double> _experienceRange;
        private Range<double> _hungerRange;
        private Range<double> _hpRange;
        private Range<double> _mpRange;
        private Range<double> _criticalHit;

        [ProtoMember(1)]
        public SymbolDetailsTemplate SymbolAlteration
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
        [ProtoMember(2)]
        public bool IsSymbolAlteration
        {
            get { return _isSymbolAlteration; }
            set
            {
                if (_isSymbolAlteration != value)
                {
                    _isSymbolAlteration = value;
                    OnPropertyChanged("IsSymbolAlteration");
                }
            }
        }
        [ProtoMember(3)]
        public bool CanSeeInvisibleCharacters
        {
            get { return _canSeeInvisibleCharacters; }
            set
            {
                if (_canSeeInvisibleCharacters != value)
                {
                    _canSeeInvisibleCharacters = value;
                    OnPropertyChanged("CanSeeInvisibleCharacters");
                }
            }
        }
        [ProtoMember(4)]
        public Range<int> EventTime
        {
            get { return _eventTime; }
            set
            {
                if (_eventTime != value)
                {
                    _eventTime = value;
                    OnPropertyChanged("EventTime");
                }
            }
        }
        [ProtoMember(5, AsReference = true)]
        public AlteredCharacterStateTemplate AlteredState
        {
            get { return _alteredState; }
            set
            {
                if (_alteredState != value)
                {
                    _alteredState = value;
                    OnPropertyChanged("AlteredState");
                }
            }
        }
        [ProtoMember(6, AsReference = true)]
        public AlteredCharacterStateTemplate RemediedState
        {
            get { return _remediedState; }
            set
            {
                if (_remediedState != value)
                {
                    _remediedState = value;
                    OnPropertyChanged("RemediedState");
                }
            }
        }
        [ProtoMember(7)]
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
        [ProtoMember(8)]
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
        [ProtoMember(9)]
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
        [ProtoMember(10)]
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
        [ProtoMember(11)]
        public Range<double> AuraRadiusRange
        {
            get { return _auraRadiusRange; }
            set
            {
                if (_auraRadiusRange != value)
                {
                    _auraRadiusRange = value;
                    OnPropertyChanged("AuraRadiusRange");
                }
            }
        }
        [ProtoMember(12)]
        public Range<double> FoodUsagePerTurnRange
        {
            get { return _foodUsagePerTurnRange; }
            set
            {
                if (_foodUsagePerTurnRange != value)
                {
                    _foodUsagePerTurnRange = value;
                    OnPropertyChanged("FoodUsagePerTurnRange");
                }
            }
        }
        [ProtoMember(13)]
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
        [ProtoMember(14)]
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
        [ProtoMember(15)]
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
        [ProtoMember(16)]
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
        [ProtoMember(17)]
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
        [ProtoMember(18)]
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
        [ProtoMember(19)]
        public Range<double> ExperienceRange
        {
            get { return _experienceRange; }
            set
            {
                if (_experienceRange != value)
                {
                    _experienceRange = value;
                    OnPropertyChanged("ExperienceRange");
                }
            }
        }
        [ProtoMember(20)]
        public Range<double> HungerRange
        {
            get { return _hungerRange; }
            set
            {
                if (_hungerRange != value)
                {
                    _hungerRange = value;
                    OnPropertyChanged("HungerRange");
                }
            }
        }
        [ProtoMember(21)]
        public Range<double> HpRange
        {
            get { return _hpRange; }
            set
            {
                if (_hpRange != value)
                {
                    _hpRange = value;
                    OnPropertyChanged("HpRange");
                }
            }
        }
        [ProtoMember(22)]
        public Range<double> MpRange
        {
            get { return _mpRange; }
            set
            {
                if (_mpRange != value)
                {
                    _mpRange = value;
                    OnPropertyChanged("MpRange");
                }
            }
        }
        [ProtoMember(23)]
        public Range<double> CriticalHit
        {
            get { return _criticalHit; }
            set
            {
                if (_criticalHit != value)
                {
                    _criticalHit = value;
                    OnPropertyChanged("CriticalHit");
                }
            }
        }

        [ProtoMember(24)]
        public List<AttackAttributeTemplate> AttackAttributes { get; set; }
        [ProtoMember(25)]
        public List<CombatAttributeTemplate> CombatAttributes { get; set; }

        public AlterationEffectTemplate()
        {
            this.SymbolAlteration = new SymbolDetailsTemplate();
            this.AlteredState = new AlteredCharacterStateTemplate(); // Creates a state of "Normal"
            this.EventTime = new Range<int>(0, 20, 30, 1000);

            this.AgilityRange = new Range<double>(-100, 0, 0, 100);
            this.SpeedRange = new Range<double>(-1, 0, 0, 1);
            this.AttackRange = new Range<double>(-100, 0, 0, 100);
            this.AuraRadiusRange = new Range<double>(-25, 0, 0, 25);
            this.DefenseRange = new Range<double>(-100, 0, 0, 100);
            this.DodgeProbabilityRange = new Range<double>(-1, 0, 0, 1);
            this.ExperienceRange = new Range<double>(-100000, 0, 0, 100000);
            this.FoodUsagePerTurnRange = new Range<double>(-10, 0, 0, 10);
            this.HpPerStepRange = new Range<double>(-100, 0, 0, 100);
            this.HpRange = new Range<double>(-1000, 0, 0, 1000);
            this.HungerRange = new Range<double>(-100, 0, 0, 100);
            this.IntelligenceRange = new Range<double>(-100, 0, 0, 100);
            this.MagicBlockProbabilityRange = new Range<double>(-1, 0, 0, 1);
            this.MpPerStepRange = new Range<double>(-100, 0, 0, 100);
            this.MpRange = new Range<double>(-100, 0, 0, 100);
            this.StrengthRange = new Range<double>(-100, 0, 0, 100);

            this.CriticalHit = new Range<double>(-1, 0, 0, 1);

            this.AttackAttributes = new List<AttackAttributeTemplate>();
            this.CombatAttributes = new List<CombatAttributeTemplate>();
            this.RemediedState = new AlteredCharacterStateTemplate();
        }
    }
}
