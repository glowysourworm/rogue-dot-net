using Rogue.NET.Core.Model.ScenarioConfiguration.Abstract;
using Rogue.NET.Core.Model.ScenarioConfiguration.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.NET.Core.Model.ScenarioConfiguration.Design
{
    [Serializable]
    public class DoodadGenerationTemplate : Template
    {
        DoodadTemplate _asset;
        double _generationWeight;
        public DoodadTemplate Asset
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
        public DoodadGenerationTemplate()
        {
            this.GenerationWeight = 1.0;
        }
    }
}
