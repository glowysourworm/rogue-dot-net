using Rogue.NET.Core.Model.ScenarioConfiguration.Abstract;
using System;

namespace Rogue.NET.Core.Model.ScenarioConfiguration.Content
{
    [Serializable]
    public class ProbabilityEquipmentTemplate : Template
    {
        private EquipmentTemplate _theTemplate;
        private double _generationProbability;
        private bool _equipOnStartup;

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
