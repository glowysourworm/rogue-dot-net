using Rogue.NET.Core.Model.ScenarioConfiguration.Abstract;
using System;

namespace Rogue.NET.Core.Model.ScenarioConfiguration.Content
{
    [Serializable]
    public class ProbabilityConsumableTemplate : Template
    {
        private Template _theTemplate;
        private double _generationProbability;

        public Template TheTemplate
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
