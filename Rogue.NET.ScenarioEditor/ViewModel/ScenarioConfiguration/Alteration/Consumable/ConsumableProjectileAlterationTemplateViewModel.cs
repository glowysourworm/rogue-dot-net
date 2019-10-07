using Rogue.NET.Core.Model.Enums;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Abstract;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Alteration.Interface;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Animation;
using System;

namespace Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Alteration.Consumable
{
    [Serializable]
    public class ConsumableProjectileAlterationTemplateViewModel : AlterationTemplateViewModel
    {
        public ConsumableProjectileAlterationTemplateViewModel()
        {
            this.Animation = new AnimationSequenceTemplateViewModel()
            {
                TargetType = AlterationTargetType.Target
            };
        }
    }
}
