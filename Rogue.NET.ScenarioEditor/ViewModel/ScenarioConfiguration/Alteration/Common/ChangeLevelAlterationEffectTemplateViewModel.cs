using Rogue.NET.ScenarioEditor.ViewModel.Attribute;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Abstract;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Alteration.Interface;
using Rogue.NET.ScenarioEditor.Views.Assets.SharedControl.AlterationControl.EffectControl;
using System;

namespace Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Alteration.Common
{
    [Serializable]
    [UIType(DisplayName = "Change Level",
            Description = "Changes the scenario level by a specified amount",
            ViewType = typeof(ChangeLevelEffectParameters),
            BaseType = UITypeAttributeBaseType.Alteration)]
    public class ChangeLevelAlterationEffectTemplateViewModel 
        : TemplateViewModel, IConsumableAlterationEffectTemplateViewModel,
                             IDoodadAlterationEffectTemplateViewModel,
                             ISkillAlterationEffectTemplateViewModel                    
    {
        RangeViewModel<int> _levelChange;

        public RangeViewModel<int> LevelChange
        {
            get { return _levelChange; }
            set { this.RaiseAndSetIfChanged(ref _levelChange, value); }
        }

        public ChangeLevelAlterationEffectTemplateViewModel()
        {
            this.LevelChange = new RangeViewModel<int>(0, 0);
        }
    }
}
