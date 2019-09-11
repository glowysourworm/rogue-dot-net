using Rogue.NET.Core.Model.Scenario.Alteration.Common;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Abstract;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Alteration.Common;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Alteration.Interface;
using System;

namespace Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Alteration.Equipment
{
    [Serializable]
    public class EquipmentCurseAlterationTemplateViewModel : AlterationTemplateViewModel
    {
        private AuraSourceParametersTemplateViewModel _auraParameters;

        public AuraSourceParametersTemplateViewModel AuraParameters
        {
            get { return _auraParameters; }
            set { this.RaiseAndSetIfChanged(ref _auraParameters, value); }
        }

        public EquipmentCurseAlterationTemplateViewModel()
        {
            this.AuraParameters = new AuraSourceParametersTemplateViewModel();
        }
    }
}
