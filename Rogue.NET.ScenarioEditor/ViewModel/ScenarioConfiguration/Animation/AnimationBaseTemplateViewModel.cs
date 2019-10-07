using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.ScenarioConfiguration.Abstract;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Animation
{
    public class AnimationBaseTemplateViewModel : TemplateViewModel
    {
        private AnimationPointTargetType _pointTargetType;
        private BrushTemplateViewModel _fillTemplate;

        /// <summary>
        /// Used for animations that require specifying which characters to target (source or affected characters)
        /// </summary>
        public AnimationPointTargetType PointTargetType
        {
            get { return _pointTargetType; }
            set { this.RaiseAndSetIfChanged(ref _pointTargetType, value); }
        }
        public BrushTemplateViewModel FillTemplate
        {
            get { return _fillTemplate; }
            set { this.RaiseAndSetIfChanged(ref _fillTemplate, value); }
        }

        public AnimationBaseTemplateViewModel()
        {
            this.FillTemplate = new BrushTemplateViewModel();
            this.PointTargetType = AnimationPointTargetType.Source;
        }
    }
}
