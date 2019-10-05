using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Scenario.Alteration.Effect;
using Rogue.NET.Core.Model.ScenarioConfiguration.Alteration.Common;
using Rogue.NET.Scenario.Content.ViewModel.Attribute;
using Rogue.NET.Scenario.Content.ViewModel.Content.Alteration.Common;
using System.Collections.ObjectModel;
using System.Linq;

namespace Rogue.NET.Scenario.Content.ViewModel.Content.Alteration.Effect
{
    [UIDisplay(Name = "Attack Attribute (Aura)",
               Description = "Creates an Aura surrounding the source character (Friendly / Malign) that affects all characters in a specified range")]
    public class AttackAttributeAuraAlterationEffectViewModel : AlterationEffectViewModel
    {
        public AlterationAttackAttributeCombatType CombatType { get; set; }

        public ObservableCollection<AttackAttributeViewModel> AttackAttributes { get; set; }

        public AttackAttributeAuraAlterationEffectViewModel(AttackAttributeAuraAlterationEffect effect)
                : base(effect)
        {
            this.CombatType = effect.CombatType;

            this.AttackAttributes = new ObservableCollection<AttackAttributeViewModel>(
                effect.AttackAttributes
                      .Where(x => x.Attack > 0 || x.Resistance > 0 || x.Weakness > 0)
                      .Select(x => new AttackAttributeViewModel(x)));
        }
        public AttackAttributeAuraAlterationEffectViewModel(AttackAttributeAuraAlterationEffectTemplate template)
                : base(template)
        {
            this.CombatType = template.CombatType;

            this.AttackAttributes = new ObservableCollection<AttackAttributeViewModel>(
                template.AttackAttributes
                        .Where(x => x.Attack.IsSet() || x.Resistance.IsSet() || x.Weakness.IsSet() || x.Immune)
                        .Select(x => new AttackAttributeViewModel(x)));
        }
    }
}
