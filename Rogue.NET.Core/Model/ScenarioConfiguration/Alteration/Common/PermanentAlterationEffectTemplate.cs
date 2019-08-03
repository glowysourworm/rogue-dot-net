using Rogue.NET.Core.Model.ScenarioConfiguration.Abstract;
using Rogue.NET.Core.Model.ScenarioConfiguration.Alteration.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.NET.Core.Model.ScenarioConfiguration.Alteration.Common
{
    [Serializable]
    public class PermanentAlterationEffectTemplate 
        : Template, IConsumableAlterationEffectTemplate,
                    IConsumableProjectileAlterationEffectTemplate,
                    IDoodadAlterationEffectTemplate,
                    IEnemyAlterationEffectTemplate,
                    IEquipmentAttackAlterationEffectTemplate,
                    ISkillAlterationEffectTemplate
    {
        private Range<double> _strengthRange;
        private Range<double> _intelligenceRange;
        private Range<double> _agilityRange;
        private Range<double> _speedRange;
        private Range<double> _lightRadiusRange;
        private Range<double> _experienceRange;
        private Range<double> _hungerRange;
        private Range<double> _hpRange;
        private Range<double> _mpRange;

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
        public Range<double> LightRadiusRange
        {
            get { return _lightRadiusRange; }
            set
            {
                if (_lightRadiusRange != value)
                {
                    _lightRadiusRange = value;
                    OnPropertyChanged("AuraRadiusRange");
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

        public PermanentAlterationEffectTemplate()
        {
            this.AgilityRange = new Range<double>(-100, 0, 0, 100);
            this.SpeedRange = new Range<double>(-1, 0, 0, 1);
            this.LightRadiusRange = new Range<double>(-25, 0, 0, 25);
            this.ExperienceRange = new Range<double>(-100000, 0, 0, 100000);
            this.HpRange = new Range<double>(-1000, 0, 0, 1000);
            this.HungerRange = new Range<double>(-100, 0, 0, 100);
            this.IntelligenceRange = new Range<double>(-100, 0, 0, 100);
            this.MpRange = new Range<double>(-100, 0, 0, 100);
            this.StrengthRange = new Range<double>(-100, 0, 0, 100);
        }
    }
}
