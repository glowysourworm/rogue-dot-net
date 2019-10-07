using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.ScenarioConfiguration.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.NET.Core.Model.ScenarioConfiguration.Animation
{
    [Serializable]
    public class AnimationBaseTemplate : Template
    {
        private AnimationPointTargetType _pointTargetType;
        private BrushTemplate _fillTemplate;

        public AnimationPointTargetType PointTargetType
        {
            get { return _pointTargetType; }
            set
            {
                if (_pointTargetType != value)
                {
                    _pointTargetType = value;
                    OnPropertyChanged("PointTargetType");
                }
            }
        }
        public BrushTemplate FillTemplate

        {
            get { return _fillTemplate; }
            set
            {
                if (_fillTemplate != value)
                {
                    _fillTemplate = value;
                    OnPropertyChanged("FillTemplate");
                }
            }
        }

        public AnimationBaseTemplate()
        {
            this.FillTemplate = new BrushTemplate();
            this.PointTargetType = AnimationPointTargetType.Source;
        }
    }
}
