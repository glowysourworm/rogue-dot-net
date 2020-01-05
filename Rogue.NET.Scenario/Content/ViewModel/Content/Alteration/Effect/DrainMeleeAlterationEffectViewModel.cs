using Rogue.NET.Core.Model.Scenario.Alteration.Effect;
using Rogue.NET.Core.Model.ScenarioConfiguration.Alteration.Common;
using Rogue.NET.Scenario.Content.ViewModel.Attribute;
using Rogue.NET.Scenario.Content.ViewModel.Content.Alteration.Common;
using System.Collections.ObjectModel;

namespace Rogue.NET.Scenario.Content.ViewModel.Content.Alteration.Effect
{
    [UIDisplay(Name = "Drain (Combat)",
               Description = "Transfers a character's Hp or Stamina to another character (Combat Only)")]
    public class DrainMeleeAlterationEffectViewModel : AlterationEffectViewModel
    {
        public ObservableCollection<AlterationAttributeViewModel> AlterationEffectAttributes { get; set; }

        public DrainMeleeAlterationEffectViewModel(DrainMeleeAlterationEffect effect) : base(effect)
        {
            this.AlterationEffectAttributes = new ObservableCollection<AlterationAttributeViewModel>();

            if (effect.Health != 0)
                this.AlterationEffectAttributes.Add(new AlterationAttributeViewModel("Health", effect.Health.ToString("F1")));

            if (effect.Stamina != 0)
                this.AlterationEffectAttributes.Add(new AlterationAttributeViewModel("Stamina", effect.Stamina.ToString("F1")));
        }
        public DrainMeleeAlterationEffectViewModel(DrainMeleeAlterationEffectTemplate template) : base(template)
        {
            this.AlterationEffectAttributes = new ObservableCollection<AlterationAttributeViewModel>();

            if (template.Health.IsSet())
                this.AlterationEffectAttributes.Add(new AlterationAttributeViewModel("Health", template.Health.ToString()));

            if (template.Stamina.IsSet())
                this.AlterationEffectAttributes.Add(new AlterationAttributeViewModel("Stamina", template.Stamina.ToString()));
        }
    }
}
