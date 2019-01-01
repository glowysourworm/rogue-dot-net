using ProtoBuf;
using Rogue.NET.Core.Model.ScenarioConfiguration.Abstract;
using System;

namespace Rogue.NET.Core.Model.ScenarioConfiguration.Content
{
    [Serializable]
    [ProtoContract(AsReferenceDefault = true)]
    public class ProbabilityConsumableTemplate : Template
    {
        private ConsumableTemplate _theTemplate;
        private double _generationProbability;

        [ProtoMember(1, AsReference = true)]
        public ConsumableTemplate TheTemplate
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

        public ProbabilityConsumableTemplate()
        {
            this.TheTemplate = new ConsumableTemplate();
        }
    }
}
