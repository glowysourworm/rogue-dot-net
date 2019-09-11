using Rogue.NET.Core.Model.Enums;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Abstract;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Alteration.Interface;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Animation;
using System;

namespace Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Alteration.Enemy
{
    [Serializable]
    public class EnemyAlterationTemplateViewModel : AlterationTemplateViewModel
    {
        public EnemyAlterationTemplateViewModel()
        {
            this.AnimationGroup = new AnimationGroupTemplateViewModel();
            this.Cost = new AlterationCostTemplateViewModel();
        }
    }
}
