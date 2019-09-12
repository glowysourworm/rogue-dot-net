using Rogue.NET.Core.Model.Enums;
using Rogue.NET.ScenarioEditor.ViewModel.Attribute;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Abstract;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Alteration.Interface;
using Rogue.NET.ScenarioEditor.Views.Assets.SharedControl.AlterationControl.EffectControl;
using System;

namespace Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Alteration.Common
{
    [Serializable]
    [UIType(DisplayName = "Teleport",
            Description = "Transports a source / target (affected) character to a random location",
            ViewType = typeof(TeleportRandomEffectParameters))]
    public class TeleportRandomAlterationEffectTemplateViewModel 
        : TemplateViewModel, IConsumableAlterationEffectTemplateViewModel,
                             IDoodadAlterationEffectTemplateViewModel,
                             IEnemyAlterationEffectTemplateViewModel,
                             ISkillAlterationEffectTemplateViewModel
    {
        AlterationRandomPlacementType _teleportType;
        int _range;

        public AlterationRandomPlacementType TeleportType
        {
            get { return _teleportType; }
            set { this.RaiseAndSetIfChanged(ref _teleportType, value); }
        }
        public int Range
        {
            get { return _range; }
            set { this.RaiseAndSetIfChanged(ref _range, value); }
        }

        public TeleportRandomAlterationEffectTemplateViewModel()
        {

        }
    }
}
