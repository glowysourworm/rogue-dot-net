using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Abstract;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Alteration.Interface;
using System;

namespace Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Alteration.Common
{
    [Serializable]
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
            this.LevelChange = new RangeViewModel<int>(-50, 0, 0, 50);
        }
    }
}
