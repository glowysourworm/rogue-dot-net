using Rogue.NET.Core.Model.Enums;
using Rogue.NET.ScenarioEditor.ViewModel.Attribute;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Abstract;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Alteration.Interface;
using Rogue.NET.ScenarioEditor.Views.Assets.SharedControl.AlterationControl.EffectControl;
using System;

namespace Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Alteration.Common
{
    [Serializable]
    [UIType(DisplayName = "Reveal",
            Description = "Reveals items, characters, or scenario objects in the level",
            ViewType = typeof(RevealEffectParameters))]
    public class RevealAlterationEffectTemplateViewModel 
        : TemplateViewModel, IConsumableAlterationEffectTemplateViewModel,
                             IDoodadAlterationEffectTemplateViewModel,
                             ISkillAlterationEffectTemplateViewModel
    {
        AlterationRevealType _type;

        public AlterationRevealType Type
        {
            get { return _type; }
            set { this.RaiseAndSetIfChanged(ref _type, value); }
        }

        public RevealAlterationEffectTemplateViewModel() { }
    }
}
