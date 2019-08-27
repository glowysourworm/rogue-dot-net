using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Scenario.Alteration.Effect;
using Rogue.NET.Core.Model.ScenarioConfiguration.Alteration.Common;
using Rogue.NET.Scenario.Content.ViewModel.Attribute;
using Rogue.NET.Scenario.Content.ViewModel.Content.Alteration.Common;
using System.Collections.ObjectModel;
using System.Linq;

namespace Rogue.NET.Scenario.Content.ViewModel.Content.Alteration.Effect
{
    [UIDisplay(Name = "Attack Attribute (Passive)",
               Description = "Creates a (Friendly or Malign) Attack Attribute affect on the source character")]
    public class AttackAttributePassiveAlterationEffectViewModel : AlterationEffectViewModel
    {
        public AlterationAttackAttributeCombatType CombatType { get; set; }

        public ObservableCollection<AttackAttributeViewModel> AttackAttributes { get; set; }

        public AttackAttributePassiveAlterationEffectViewModel(AttackAttributePassiveAlterationEffect effect)
                : base(effect)
        {
            this.CombatType = effect.CombatType;

            this.AttackAttributes = new ObservableCollection<AttackAttributeViewModel>(
                effect.AttackAttributes
                      .Where(x => x.Attack > 0 || x.Resistance > 0 || x.Weakness > 0)
                      .Select(x => new AttackAttributeViewModel(x)));
        }
        public AttackAttributePassiveAlterationEffectViewModel(AttackAttributePassiveAlterationEffectTemplate template)
                : base(template)
        {
            this.CombatType = template.CombatType;

            this.AttackAttributes = new ObservableCollection<AttackAttributeViewModel>(
                template.AttackAttributes
                        .Where(x => x.Attack.IsSet() || x.Resistance.IsSet() || x.Weakness.IsSet())
                        .Select(x => new AttackAttributeViewModel(x)));
        }
    }
}
