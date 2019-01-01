using ProtoBuf;
using Rogue.NET.Core.Model.ScenarioConfiguration.Abstract;
using System;

namespace Rogue.NET.Core.Model.ScenarioConfiguration.Content
{
    [Serializable]
    [ProtoContract(AsReferenceDefault = true)]
    public class AttackAttributeTemplate : DungeonObjectTemplate
    {
        private Range<double> _attack;
        private Range<double> _resistance;

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

        public AttackAttributeTemplate()
        {
            this.Attack = new Range<double>(0, 0, 0, 5000);
            this.Resistance = new Range<double>(0, 0, 0, 5000);
        }
    }
}
