using Rogue.NET.ScenarioEditor.ViewModel.Attribute;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Abstract;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Alteration.Interface;
using Rogue.NET.ScenarioEditor.Views.Assets.SharedControl.AlterationControl.EffectControl;
using System;

namespace Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Alteration.Common
{
    [Serializable]
    [UIType(DisplayName = "Drain (Combat)",
            Description = "Transfers a character's Hp or Stamina to another character (Combat Only)",
            ViewType = typeof(DrainMeleeEffectParameters),
            BaseType = UITypeAttributeBaseType.Alteration)]
    public class DrainMeleeAlterationEffectTemplateViewModel
                    : TemplateViewModel, IEquipmentAttackAlterationEffectTemplateViewModel
    {
        RangeViewModel<double> _health;
        RangeViewModel<double> _stamina;

        public RangeViewModel<double> Health
        {
            get { return _health; }
            set { this.RaiseAndSetIfChanged(ref _health, value); }
        }
        public RangeViewModel<double> Stamina
        {
            get { return _stamina; }
            set { this.RaiseAndSetIfChanged(ref _stamina, value); }
        }

        public DrainMeleeAlterationEffectTemplateViewModel()
        {
            this.Health = new RangeViewModel<double>(0, 0);
            this.Stamina = new RangeViewModel<double>(0, 0);
        }
    }
}
