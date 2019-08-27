using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Scenario.Alteration.Effect;
using Rogue.NET.Core.Model.ScenarioConfiguration.Alteration.Common;
using Rogue.NET.Scenario.Content.ViewModel.Attribute;
using Rogue.NET.Scenario.Content.ViewModel.Content.Alteration.Common;
using System.Collections.ObjectModel;
using System.Linq;

namespace Rogue.NET.Scenario.Content.ViewModel.Content.Alteration.Effect
{
    [UIDisplay(Name = "Enhance Equipment",
               Description = "Modifies the Player's equipment with the option to use a dialog window")]
    public class EquipmentEnhanceAlterationEffectViewModel : AlterationEffectViewModel
    {
        public AlterationModifyEquipmentType Type { get; set; }

        public string ClassChange { get; set; }
        public string QualityChange { get; set; }
        public ObservableCollection<AttackAttributeViewModel> AttackAttributes { get; set; }

        public EquipmentEnhanceAlterationEffectViewModel(EquipmentEnhanceAlterationEffect effect) : base(effect)
        {
            this.Type = effect.Type;
            this.ClassChange = effect.ClassChange.ToString("N0");
            this.QualityChange = effect.QualityChange.ToString("F1");

            this.AttackAttributes = new ObservableCollection<AttackAttributeViewModel>(
                effect.AttackAttributes
                      .Where(x => x.Attack > 0 || x.Resistance > 0 || x.Weakness > 0)
                      .Select(x => new AttackAttributeViewModel(x)));
        }

        public EquipmentEnhanceAlterationEffectViewModel(EquipmentEnhanceAlterationEffectTemplate template) : base(template)
        {
            this.Type = template.Type;
            this.ClassChange = template.ClassChange.ToString("N0");
            this.QualityChange = template.QualityChange.ToString("F1");

            this.AttackAttributes = new ObservableCollection<AttackAttributeViewModel>(
                template.AttackAttributes
                        .Where(x => x.Attack.IsSet() || x.Resistance.IsSet() || x.Weakness.IsSet())
                        .Select(x => new AttackAttributeViewModel(x)));
        }
    }
}
