using Rogue.NET.Core.Model.Scenario.Alteration.Effect;
using Rogue.NET.Core.Model.ScenarioConfiguration.Alteration.Common;
using Rogue.NET.Scenario.Content.ViewModel.Attribute;
using Rogue.NET.Scenario.Content.ViewModel.Content.Alteration.Common;
using System.Collections.ObjectModel;

namespace Rogue.NET.Scenario.Content.ViewModel.Content.Alteration.Effect
{
    [UIDisplay(Name = "Permanent",
               Description = "Creates a permanent change to a character's stats")]
    public class PermanentAlterationEffectViewModel : AlterationEffectViewModel
    {
        public ObservableCollection<AlterationAttributeViewModel> AlterationEffectAttributes { get; set; }

        public PermanentAlterationEffectViewModel(PermanentAlterationEffect effect) : base(effect)
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

            if (effect.LightRadius != 0)
                this.AlterationEffectAttributes.Add(new AlterationAttributeViewModel("Light Radius", effect.LightRadius.ToString("N0")));

            if (effect.Experience != 0)
                this.AlterationEffectAttributes.Add(new AlterationAttributeViewModel("Experience", effect.Experience.ToString("N0")));

            if (effect.Hunger != 0)
                this.AlterationEffectAttributes.Add(new AlterationAttributeViewModel("Hunger", effect.Hunger.ToString("F1")));

            if (effect.Hp != 0)
                this.AlterationEffectAttributes.Add(new AlterationAttributeViewModel("Hp", effect.Hp.ToString("F1")));

            if (effect.Mp != 0)
                this.AlterationEffectAttributes.Add(new AlterationAttributeViewModel("Mp", effect.Mp.ToString("F1")));
        }

        public PermanentAlterationEffectViewModel(PermanentAlterationEffectTemplate template) : base(template)
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

            if (template.LightRadiusRange.IsSet())
                this.AlterationEffectAttributes.Add(new AlterationAttributeViewModel("Light Radius", template.SpeedRange.ToString()));

            if (template.ExperienceRange.IsSet())
                this.AlterationEffectAttributes.Add(new AlterationAttributeViewModel("Experience", template.SpeedRange.ToString()));

            if (template.HungerRange.IsSet())
                this.AlterationEffectAttributes.Add(new AlterationAttributeViewModel("Hunger", template.HungerRange.ToString()));

            if (template.HpRange.IsSet())
                this.AlterationEffectAttributes.Add(new AlterationAttributeViewModel("Hp", template.HpRange.ToString()));

            if (template.MpRange.IsSet())
                this.AlterationEffectAttributes.Add(new AlterationAttributeViewModel("Mp", template.MpRange.ToString()));
        }
    }
}
