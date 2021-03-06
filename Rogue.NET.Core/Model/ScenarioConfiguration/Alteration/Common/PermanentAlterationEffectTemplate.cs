﻿using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.ScenarioConfiguration.Abstract;
using Rogue.NET.Core.Model.ScenarioConfiguration.Alteration.Interface;
using System;

namespace Rogue.NET.Core.Model.ScenarioConfiguration.Alteration.Common
{
    [Serializable]
    public class PermanentAlterationEffectTemplate : Template, IConsumableAlterationEffectTemplate,
                                                               IConsumableProjectileAlterationEffectTemplate,
                                                               IDoodadAlterationEffectTemplate,
                                                               IEnemyAlterationEffectTemplate,
                                                               IFriendlyAlterationEffectTemplate,
                                                               ITemporaryCharacterAlterationEffectTemplate,
                                                               IEquipmentAttackAlterationEffectTemplate,
                                                               ISkillAlterationEffectTemplate
    {
        private Range<double> _strengthRange;
        private Range<double> _intelligenceRange;
        private Range<double> _agilityRange;
        private Range<double> _speedRange;
        private Range<double> _visionRange;
        private Range<double> _experienceRange;
        private Range<double> _hungerRange;
        private Range<double> _healthRange;
        private Range<double> _staminaRange;

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
        public Range<double> VisionRange
        {
            get { return _visionRange; }
            set
            {
                if (_visionRange != value)
                {
                    _visionRange = value;
                    OnPropertyChanged("VisionRange");
                }
            }
        }
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
        public Range<double> HealthRange
        {
            get { return _healthRange; }
            set
            {
                if (_healthRange != value)
                {
                    _healthRange = value;
                    OnPropertyChanged("HealthRange");
                }
            }
        }
        public Range<double> StaminaRange
        {
            get { return _staminaRange; }
            set
            {
                if (_staminaRange != value)
                {
                    _staminaRange = value;
                    OnPropertyChanged("StaminaRange");
                }
            }
        }

        public PermanentAlterationEffectTemplate()
        {
            this.AgilityRange = new Range<double>(0, 0);
            this.SpeedRange = new Range<double>(0, 0);
            this.ExperienceRange = new Range<double>(0, 0);
            this.HealthRange = new Range<double>(0, 0);
            this.HungerRange = new Range<double>(0, 0);
            this.IntelligenceRange = new Range<double>(0, 0);
            this.StaminaRange = new Range<double>(0, 0);
            this.StrengthRange = new Range<double>(0, 0);
            this.VisionRange = new Range<double>(0, 0);
        }
    }
}
