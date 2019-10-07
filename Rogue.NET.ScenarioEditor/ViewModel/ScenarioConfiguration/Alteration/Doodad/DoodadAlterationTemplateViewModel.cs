using Rogue.NET.Core.Model.Enums;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Abstract;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Alteration.Interface;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Animation;
using System;

namespace Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Alteration.Doodad
{
    [Serializable]
    public class DoodadAlterationTemplateViewModel : AlterationTemplateViewModel
    {
        public DoodadAlterationTemplateViewModel()
        {
            this.Animation = new AnimationSequenceTemplateViewModel();
        }
    }
}
