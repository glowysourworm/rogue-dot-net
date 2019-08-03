using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Abstract;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Alteration.Interface;
using System;

namespace Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Alteration.Equipment
{
    [Serializable]
    public class EquipmentEquipAlterationTemplateViewModel : TemplateViewModel
    {
        private IEquipmentEquipAlterationEffectTemplateViewModel _effect;

        public IEquipmentEquipAlterationEffectTemplateViewModel Effect
        {
            get { return _effect; }
            set { this.RaiseAndSetIfChanged(ref _effect, value); }
        }

        public EquipmentEquipAlterationTemplateViewModel()
        {
        }
    }
}
