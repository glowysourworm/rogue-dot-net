using Rogue.NET.Core.Model.Scenario.Alteration.Effect;
using Rogue.NET.Core.Model.ScenarioConfiguration.Alteration.Common;
using Rogue.NET.Scenario.Content.ViewModel.Attribute;
using Rogue.NET.Scenario.Content.ViewModel.Content.Alteration.Common;
using System.Collections.ObjectModel;

namespace Rogue.NET.Scenario.Content.ViewModel.Content.Alteration.Effect
{
    [UIDisplay(Name = "Drain (Combat)",
               Description = "Transfers a character's Hp or Mp to another character (Combat Only)")]
    public class DrainMeleeAlterationEffectViewModel : AlterationEffectViewModel
    {
        public ObservableCollection<AlterationAttributeViewModel> AlterationEffectAttributes { get; set; }

        public DrainMeleeAlterationEffectViewModel(DrainMeleeAlterationEffect effect) : base(effect)
        {
            this.AlterationEffectAttributes = new ObservableCollection<AlterationAttributeViewModel>();

            if (effect.Hp != 0)
                this.AlterationEffectAttributes.Add(new AlterationAttributeViewModel("Hp", effect.Hp.ToString("F1")));

            if (effect.Mp != 0)
                this.AlterationEffectAttributes.Add(new AlterationAttributeViewModel("Mp", effect.Mp.ToString("F1")));
        }
        public DrainMeleeAlterationEffectViewModel(DrainMeleeAlterationEffectTemplate template) : base(template)
        {
            this.AlterationEffectAttributes = new ObservableCollection<AlterationAttributeViewModel>();

            if (template.Hp.IsSet())
                this.AlterationEffectAttributes.Add(new AlterationAttributeViewModel("Hp", template.Hp.ToString()));

            if (template.Mp.IsSet())
                this.AlterationEffectAttributes.Add(new AlterationAttributeViewModel("Mp", template.Mp.ToString()));
        }
    }
}
