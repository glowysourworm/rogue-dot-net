using Rogue.NET.Core.Model.ScenarioConfiguration.Abstract;
using System;

namespace Rogue.NET.Core.Model.ScenarioConfiguration.Content
{
    [Serializable]
    public class AttackAttributeTemplate : DungeonObjectTemplate
    {
        private Range<double> _attack;
        private Range<double> _resistance;
        private bool _appliesToStrengthBasedCombat;
        private bool _appliesToIntelligenceBasedCombat;
        private bool _scaledByStrength;
        private bool _scaledByIntelligence;

        public Range<double> Attack
        {
            get { return _attack; }
            set
            {
                if (_attack != value)
                {
                    _attack = value;
                    OnPropertyChanged("Attack");
                }
            }
        }
        public Range<double> Resistance
        {
            get { return _resistance; }
            set
            {
                if (_resistance != value)
                {
                    _resistance = value;
                    OnPropertyChanged("Resistance");
                }
            }
        }
        public bool AppliesToStrengthBasedCombat
        {
            get { return _appliesToStrengthBasedCombat; }
            set
            {
                if (_appliesToStrengthBasedCombat != value)
                {
                    _appliesToStrengthBasedCombat = value;
                    OnPropertyChanged("AppliesToStrengthBasedCombat");
                }
            }
        }
        public bool AppliesToIntelligenceBasedCombat
        {
            get { return _appliesToIntelligenceBasedCombat; }
            set
            {
                if (_appliesToIntelligenceBasedCombat != value)
                {
                    _appliesToIntelligenceBasedCombat = value;
                    OnPropertyChanged("AppliesToIntelligenceBasedCombat");
                }
            }
        }

        public bool ScaledByStrength
        {
            get { return _scaledByStrength; }
            set
            {
                if (_scaledByStrength != value)
                {
                    _scaledByStrength = value;
                    OnPropertyChanged("ScaledByStrength");
                }
            }
        }
        public bool ScaledByIntelligence
        {
            get { return _scaledByIntelligence; }
            set
            {
                if (_scaledByIntelligence != value)
                {
                    _scaledByIntelligence = value;
                    OnPropertyChanged("ScaledByIntelligence");
                }
            }
        }

        public AttackAttributeTemplate()
        {
            this.Attack = new Range<double>(0, 0, 0, 5000);
            this.Resistance = new Range<double>(0, 0, 0, 5000);
        }
    }
}
