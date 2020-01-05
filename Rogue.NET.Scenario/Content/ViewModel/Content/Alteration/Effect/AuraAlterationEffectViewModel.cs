using Rogue.NET.Core.Model.Scenario.Alteration.Effect;
using Rogue.NET.Core.Model.ScenarioConfiguration.Alteration.Common;
using Rogue.NET.Scenario.Content.ViewModel.Attribute;
using Rogue.NET.Scenario.Content.ViewModel.Content.Alteration.Common;
using System.Collections.ObjectModel;

namespace Rogue.NET.Scenario.Content.ViewModel.Content.Alteration.Effect
{
    [UIDisplay(Name = "Aura",
               Description = "Causes a stat change to characters in range of the source character")]
    public class AuraAlterationEffectViewModel : AlterationEffectViewModel
    {
        public ObservableCollection<AlterationAttributeViewModel> AlterationEffectAttributes { get; set; }

        public AuraAlterationEffectViewModel(AuraAlterationEffect effect) : base(effect)
        {
            this.AlterationEffectAttributes = new ObservableCollection<AlterationAttributeViewModel>();

            if (effect.Strength != 0)
                this.AlterationEffectAttributes.Add(new AlterationAttributeViewModel("Strength", effect.Strength.ToString("F1")));

            if (effect.Agility != 0)
                this.AlterationEffectAttributes.Add(new AlterationAttributeViewModel("Agility", effect.Agility.ToString("F1")));

            if (effect.Intelligence != 0)
                this.AlterationEffectAttributes.Add(new AlterationAttributeViewModel("Intelligence", effect.Intelligence.ToString("F1")));

            if (effect.Speed != 0)
                this.AlterationEffectAttributes.Add(new AlterationAttributeViewModel("Speed", effect.Speed.ToString("F1")));

            if (effect.HealthPerStep != 0)
                this.AlterationEffectAttributes.Add(new AlterationAttributeViewModel("Health Regen", effect.HealthPerStep.ToString("F1")));

            if (effect.StaminaPerStep != 0)
                this.AlterationEffectAttributes.Add(new AlterationAttributeViewModel("Stamina Regen", effect.StaminaPerStep.ToString("F1")));

            if (effect.Attack != 0)
                this.AlterationEffectAttributes.Add(new AlterationAttributeViewModel("Attack", effect.Attack.ToString("F1")));

            if (effect.Defense != 0)
                this.AlterationEffectAttributes.Add(new AlterationAttributeViewModel("Defense", effect.Defense.ToString("F1")));
        }

        public AuraAlterationEffectViewModel(AuraAlterationEffectTemplate template) : base(template)
        {
            this.AlterationEffectAttributes = new ObservableCollection<AlterationAttributeViewModel>();

            if (template.StrengthRange.IsSet())
                this.AlterationEffectAttributes.Add(new AlterationAttributeViewModel("Strength", template.StrengthRange.ToString()));

            if (template.AgilityRange.IsSet())
                this.AlterationEffectAttributes.Add(new AlterationAttributeViewModel("Agility", template.AgilityRange.ToString()));

            if (template.IntelligenceRange.IsSet())
                this.AlterationEffectAttributes.Add(new AlterationAttributeViewModel("Intelligence", template.IntelligenceRange.ToString()));

            if (template.SpeedRange.IsSet())
                this.AlterationEffectAttributes.Add(new AlterationAttributeViewModel("Speed", template.SpeedRange.ToString()));

            if (template.HealthPerStepRange.IsSet())
                this.AlterationEffectAttributes.Add(new AlterationAttributeViewModel("Health Regen", template.HealthPerStepRange.ToString()));

            if (template.StaminaPerStepRange.IsSet())
                this.AlterationEffectAttributes.Add(new AlterationAttributeViewModel("Stamina Regen", template.StaminaPerStepRange.ToString()));

            if (template.AttackRange.IsSet())
                this.AlterationEffectAttributes.Add(new AlterationAttributeViewModel("Attack", template.AttackRange.ToString()));

            if (template.DefenseRange.IsSet())
                this.AlterationEffectAttributes.Add(new AlterationAttributeViewModel("Defense", template.DefenseRange.ToString()));
        }
    }
}
