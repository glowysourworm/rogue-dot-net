using Rogue.NET.Core.Model.Enums;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Abstract;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Alteration.Interface;
using System;

namespace Rogue.ScenarioEditor.ViewModel.Model.ScenarioConfiguration.Alteration.Common
{
    [Serializable]
    public class TeleportAlterationEffectTemplateViewModel 
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

        public TeleportAlterationEffectTemplateViewModel()
        {

        }
    }
}
