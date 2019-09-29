using Rogue.NET.Core.Model.ScenarioConfiguration.Abstract;
using System;

namespace Rogue.NET.Core.Model.ScenarioConfiguration.Design
{
    [Serializable]
    public class LevelBranchGenerationTemplate : Template
    {
        LevelBranchTemplate _levelBranch;
        double _generationWeight;
        public LevelBranchTemplate LevelBranch
        {
            get { return _levelBranch; }
            set
            {
                if (_levelBranch != value)
                {
                    _levelBranch = value;
                    OnPropertyChanged("LevelBranch");
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
        public LevelBranchGenerationTemplate()
        {
            this.LevelBranch = new LevelBranchTemplate();
            this.GenerationWeight = 1.0;
        }
    }
}
