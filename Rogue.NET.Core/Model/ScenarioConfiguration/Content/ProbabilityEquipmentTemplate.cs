using ProtoBuf;
using Rogue.NET.Core.Model.ScenarioConfiguration.Abstract;
using System;

namespace Rogue.NET.Core.Model.ScenarioConfiguration.Content
{
    [Serializable]
    [ProtoContract(AsReferenceDefault = true)]
    public class ProbabilityEquipmentTemplate : Template
    {
        private EquipmentTemplate _theTemplate;
        private double _generationProbability;
        private bool _equipOnStartup;

        [ProtoMember(1, AsReference = true)]
        public EquipmentTemplate TheTemplate
        {
            get { return _theTemplate; }
            set
            {
                if (_theTemplate != value)
                {
                    _theTemplate = value;
                    OnPropertyChanged("TheTemplate");
                }
            }
        }
        [ProtoMember(2)]
        public double GenerationProbability
        {
            get { return _generationProbability; }
            set
            {
                if (_generationProbability != value)
                {
                    _generationProbability = value;
                    OnPropertyChanged("GenerationProbability");
                }
            }
        }
        [ProtoMember(3)]
        public bool EquipOnStartup
        {
            get { return _equipOnStartup; }
            set
            {
                _equipOnStartup = value;
                OnPropertyChanged("EquipOnStartup");
            }
        }

        public ProbabilityEquipmentTemplate()
        {
            this.TheTemplate = new EquipmentTemplate();
        }
    }
}
