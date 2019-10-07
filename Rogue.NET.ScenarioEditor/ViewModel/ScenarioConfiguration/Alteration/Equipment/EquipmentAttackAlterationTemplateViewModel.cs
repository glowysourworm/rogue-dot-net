using Rogue.NET.Core.Model.Enums;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Abstract;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Alteration.Interface;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Animation;
using System;

namespace Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Alteration.Equipment
{
    [Serializable]
    public class EquipmentAttackAlterationTemplateViewModel : AlterationTemplateViewModel
    {
        public EquipmentAttackAlterationTemplateViewModel()
        {
            this.Animation = new AnimationSequenceTemplateViewModel()
            {
                TargetType = AlterationTargetType.Target
            };
            this.Cost = new AlterationCostTemplateViewModel();
        }
    }
}
