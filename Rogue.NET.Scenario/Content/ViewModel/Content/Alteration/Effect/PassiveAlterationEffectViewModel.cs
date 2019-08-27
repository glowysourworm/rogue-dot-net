using Rogue.NET.Core.Model.Scenario.Alteration.Effect;
using Rogue.NET.Core.Model.ScenarioConfiguration.Alteration.Common;
using Rogue.NET.Scenario.Content.ViewModel.Attribute;
using Rogue.NET.Scenario.Content.ViewModel.Content.Alteration.Common;
using System.Collections.ObjectModel;

namespace Rogue.NET.Scenario.Content.ViewModel.Content.Alteration.Effect
{
    [UIDisplay(Name = "Passive",
               Description = "Creates a change to a character's stats that is activated / deactivated")]
    public class PassiveAlterationEffectViewModel : AlterationEffectViewModel
    {
        public ObservableCollection<AlterationAttributeViewModel> AlterationEffectAttributes { get; set; }

        bool _canSeeInvisibleCharacters;

        public bool CanSeeInvisibleCharacters
        {
            get { return _canSeeInvisibleCharacters; }
            set { this.RaiseAndSetIfChanged(ref _canSeeInvisibleCharacters, value); }
        }

        public PassiveAlterationEffectViewModel(PassiveAlterationEffect effect) : base(effect)
        {
            this.CanSeeInvisibleCharacters = effect.CanSeeInvisibleCharacters;

            if (effect.Strength != 0)
                this.AlterationEffectAttributes.Add(new AlterationAttributeViewModel("Strength", effect.Strength.ToString("F1")));

            if (effect.Agility != 0)
                this.AlterationEffectAttributes.Add(new AlterationAttributeViewModel("Agility", effect.Agility.ToString("F1")));

            if (effect.Intelligence != 0)
                this.AlterationEffectAttributes.Add(new AlterationAttributeViewModel("Intelligence", effect.Intelligence.ToString("F1")));

            if (effect.Speed != 0)
                this.AlterationEffectAttributes.Add(new AlterationAttributeViewModel("Speed", effect.Speed.ToString("F1")));

            if (effect.LightRadius != 0)
                this.AlterationEffectAttributes.Add(new AlterationAttributeViewModel("Light Radius", effect.LightRadius.ToString("N0")));

            if (effect.FoodUsagePerTurn != 0)
                this.AlterationEffectAttributes.Add(new AlterationAttributeViewModel("Food Usage (per turn)", effect.FoodUsagePerTurn.ToString("F1")));

            if (effect.HpPerStep != 0)
                this.AlterationEffectAttributes.Add(new AlterationAttributeViewModel("Hp Regen", effect.HpPerStep.ToString("F1")));

            if (effect.MpPerStep != 0)
                this.AlterationEffectAttributes.Add(new AlterationAttributeViewModel("Mp Regen", effect.MpPerStep.ToString("F1")));

            if (effect.Attack != 0)
                this.AlterationEffectAttributes.Add(new AlterationAttributeViewModel("Attack", effect.Attack.ToString("F1")));

            if (effect.Defense != 0)
                this.AlterationEffectAttributes.Add(new AlterationAttributeViewModel("Defense", effect.Defense.ToString("F1")));

            if (effect.MagicBlockProbability != 0)
                this.AlterationEffectAttributes.Add(new AlterationAttributeViewModel("Mental Block", effect.MagicBlockProbability.ToString("F1")));

            if (effect.DodgeProbability != 0)
                this.AlterationEffectAttributes.Add(new AlterationAttributeViewModel("Dodge", effect.DodgeProbability.ToString("F1")));

            if (effect.CriticalHit != 0)
                this.AlterationEffectAttributes.Add(new AlterationAttributeViewModel("Critical Hit", effect.CriticalHit.ToString("F1")));
        }

        public PassiveAlterationEffectViewModel(PassiveAlterationEffectTemplate template) : base(template)
        {
            this.CanSeeInvisibleCharacters = template.CanSeeInvisibleCharacters;

            if (template.StrengthRange.IsSet())
                this.AlterationEffectAttributes.Add(new AlterationAttributeViewModel("Strength", template.StrengthRange.ToString()));

            if (template.AgilityRange.IsSet())
                this.AlterationEffectAttributes.Add(new AlterationAttributeViewModel("Agility", template.AgilityRange.ToString()));

            if (template.IntelligenceRange.IsSet())
                this.AlterationEffectAttributes.Add(new AlterationAttributeViewModel("Intelligence", template.IntelligenceRange.ToString()));

            if (template.SpeedRange.IsSet())
                this.AlterationEffectAttributes.Add(new AlterationAttributeViewModel("Speed", template.SpeedRange.ToString()));

            if (template.LightRadiusRange.IsSet())
                this.AlterationEffectAttributes.Add(new AlterationAttributeViewModel("Light Radius", template.SpeedRange.ToString()));

            if (template.FoodUsagePerTurnRange.IsSet())
                this.AlterationEffectAttributes.Add(new AlterationAttributeViewModel("Food Usage (per turn)", template.SpeedRange.ToString()));

            if (template.HpPerStepRange.IsSet())
                this.AlterationEffectAttributes.Add(new AlterationAttributeViewModel("Hp Regen", template.HpPerStepRange.ToString()));

            if (template.MpPerStepRange.IsSet())
                this.AlterationEffectAttributes.Add(new AlterationAttributeViewModel("Mp Regen", template.MpPerStepRange.ToString()));

            if (template.AttackRange.IsSet())
                this.AlterationEffectAttributes.Add(new AlterationAttributeViewModel("Attack", template.AttackRange.ToString()));
            
            if (template.DefenseRange.IsSet())
                this.AlterationEffectAttributes.Add(new AlterationAttributeViewModel("Defense", template.DefenseRange.ToString()));

            if (template.MagicBlockProbabilityRange.IsSet())
                this.AlterationEffectAttributes.Add(new AlterationAttributeViewModel("Mental Block", template.MagicBlockProbabilityRange.ToString()));

            if (template.DodgeProbabilityRange.IsSet())
                this.AlterationEffectAttributes.Add(new AlterationAttributeViewModel("Dodge", template.DodgeProbabilityRange.ToString()));

            if (template.CriticalHit.IsSet())
                this.AlterationEffectAttributes.Add(new AlterationAttributeViewModel("Critical Hit", template.CriticalHit.ToString()));
        }
    }
}
