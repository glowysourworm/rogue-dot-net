using Rogue.NET.Core.Model.ScenarioConfiguration.Abstract;
using Rogue.NET.Core.Model.ScenarioConfiguration.Layout;
using System;

namespace Rogue.NET.Core.Model.ScenarioConfiguration.Design
{
    [Serializable]
    public class LayoutGenerationTemplate : Template
    {
        LayoutTemplate _asset;
        double _partyRoomGenerationRate;
        double _generationWeight;
        public LayoutTemplate Asset
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
        public double PartyRoomGenerationRate
        {
            get { return _partyRoomGenerationRate; }
            set
            {
                if (_partyRoomGenerationRate != value)
                {
                    _partyRoomGenerationRate = value;
                    OnPropertyChanged("PartyRoomGenerationRate");
                }
            }
        }
        public LayoutGenerationTemplate()
        {
            this.PartyRoomGenerationRate = ModelConstants.Scenario.PartyRoomGenerationRateDefault;
            this.GenerationWeight = 1;
        }
    }
}
