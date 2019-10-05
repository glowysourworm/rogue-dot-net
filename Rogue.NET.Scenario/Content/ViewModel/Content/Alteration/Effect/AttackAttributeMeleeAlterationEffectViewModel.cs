using Rogue.NET.Core.Model.Scenario.Alteration.Effect;
using Rogue.NET.Core.Model.ScenarioConfiguration.Alteration.Common;
using Rogue.NET.Scenario.Content.ViewModel.Attribute;
using Rogue.NET.Scenario.Content.ViewModel.Content.Alteration.Common;
using System.Collections.ObjectModel;
using System.Linq;

namespace Rogue.NET.Scenario.Content.ViewModel.Content.Alteration.Effect
{
    [UIDisplay(Name = "Attack Attribute (Combat)",
               Description = "Creates a one-time hit towards the affected character(s)")]
    public class AttackAttributeMeleeAlterationEffectViewModel : AlterationEffectViewModel
    {
        public ObservableCollection<AttackAttributeViewModel> AttackAttributes { get; set; }

        public AttackAttributeMeleeAlterationEffectViewModel(AttackAttributeMeleeAlterationEffect effect)
                : base(effect)
        {
            this.AttackAttributes = new ObservableCollection<AttackAttributeViewModel>(
                effect.AttackAttributes
                      .Where(x => x.Attack > 0 || x.Resistance > 0 || x.Weakness > 0)
                      .Select(x => new AttackAttributeViewModel(x)));
        }

        public AttackAttributeMeleeAlterationEffectViewModel(AttackAttributeMeleeAlterationEffectTemplate template)
                : base(template)
        {
            this.AttackAttributes = new ObservableCollection<AttackAttributeViewModel>(
                template.AttackAttributes
                        .Where(x => x.Attack.IsSet() || x.Resistance.IsSet() || x.Weakness.IsSet() || x.Immune)
                        .Select(x => new AttackAttributeViewModel(x)));
        }
    }
}
