using ProtoBuf;
using Rogue.NET.Core.Model.ScenarioConfiguration.Abstract;
using System;

namespace Rogue.NET.Core.Model.ScenarioConfiguration.Content
{
    [Serializable]
    [ProtoContract(AsReferenceDefault = true)]
    public class CombatAttributeTemplate : DungeonObjectTemplate
    {
        private Range<double> _attack;
        private Range<double> _resistance;

        private bool _appliesToStrengthBasedCombat;
        private bool _appliesToIntelligenceBasedCombat;

        private bool _scaledByStrength;
        private bool _scaledByIntelligence;

        [ProtoMember(1)]
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
        [ProtoMember(2)]
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

        [ProtoMember(3)]
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
        [ProtoMember(4)]
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

        [ProtoMember(5)]
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
        [ProtoMember(6)]
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

        public CombatAttributeTemplate()
        {
            this.Attack = new Range<double>(0, 0, 0, 1000);
            this.Resistance = new Range<double>(0, 0, 0, 1000);
        }
    }
}
