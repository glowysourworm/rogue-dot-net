using Rogue.NET.ScenarioEditor.ViewModel.Attribute;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Abstract;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Alteration.Interface;
using Rogue.NET.ScenarioEditor.Views.Assets.SharedControl.AlterationControl.EffectControl;
using System;

namespace Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Alteration.Common
{
    [Serializable]
    [UIType(DisplayName = "Drain (Combat)",
            Description = "Transfers a character's Hp or Mp to another character (Combat Only)",
            ViewType = typeof(DrainMeleeEffectParameters))]
    public class DrainMeleeAlterationEffectTemplateViewModel
                    : TemplateViewModel, IEquipmentAttackAlterationEffectTemplateViewModel
    {
        RangeViewModel<double> _hp;
        RangeViewModel<double> _mp;

        public RangeViewModel<double> Hp
        {
            get { return _hp; }
            set { this.RaiseAndSetIfChanged(ref _hp, value); }
        }
        public RangeViewModel<double> Mp
        {
            get { return _mp; }
            set { this.RaiseAndSetIfChanged(ref _mp, value); }
        }

        public DrainMeleeAlterationEffectTemplateViewModel()
        {
            this.Hp = new RangeViewModel<double>(0, 0, 0, 100);
            this.Mp = new RangeViewModel<double>(0, 0, 0, 100);
        }
    }
}
