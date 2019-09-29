using Rogue.NET.Core.Model.ScenarioConfiguration.Abstract;
using Rogue.NET.Core.Model.ScenarioConfiguration.Content;
using System;

namespace Rogue.NET.Core.Model.ScenarioConfiguration.Design
{
    [Serializable]
    public class FriendlyGenerationTemplate : Template
    {
        FriendlyTemplate _asset;
        double _generationWeight;
        public FriendlyTemplate Asset
        {
            get { return _asset; }
            set
            {
                if (_asset != value)
                {
                    _asset = value;
                    OnPropertyChanged("Asset");
                }
            }
        }
        public double GenerationWeight
        {
            get { return _generationWeight; }
            set
            {
                if (_generationWeight != value)
                {
                    _generationWeight = value;
                    OnPropertyChanged("Asset");
                }
            }
        }
        public FriendlyGenerationTemplate()
        {
            this.GenerationWeight = 1.0;
        }
    }
}
