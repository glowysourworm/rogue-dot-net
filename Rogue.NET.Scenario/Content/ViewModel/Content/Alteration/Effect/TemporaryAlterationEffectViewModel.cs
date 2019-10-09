using Rogue.NET.Core.Model.Scenario.Alteration.Effect;
using Rogue.NET.Core.Model.ScenarioConfiguration.Alteration.Common;
using Rogue.NET.Scenario.Content.ViewModel.Attribute;
using Rogue.NET.Scenario.Content.ViewModel.Content.Alteration.Common;
using Rogue.NET.Scenario.Content.ViewModel.Content.ScenarioMetaData;
using System.Collections.ObjectModel;

namespace Rogue.NET.Scenario.Content.ViewModel.Content.Alteration.Effect
{
    [UIDisplay(Name = "Temporary",
               Description = "Causes a temporary change to a character's stats")]
    public class TemporaryAlterationEffectViewModel : AlterationEffectViewModel
    {
        bool _canSeeInvisibleCharacters;
        bool _isStackable;
        bool _hasAlteredState;
        string _eventTime;
        ScenarioImageViewModel _alteredState;

        public ObservableCollection<AlterationAttributeViewModel> AlterationEffectAttributes { get; set; }

        public bool CanSeeInvisibleCharacters
        {
            get { return _canSeeInvisibleCharacters; }
            set { this.RaiseAndSetIfChanged(ref _canSeeInvisibleCharacters, value); }
        }
        public bool IsStackable
        {
            get { return _isStackable; }
            set { this.RaiseAndSetIfChanged(ref _isStackable, value); }
        }
        public bool HasAlteredState
        {
            get { return _hasAlteredState; }
            set { this.RaiseAndSetIfChanged(ref _hasAlteredState, value); }
        }
        public string EventTime
        {
            get { return _eventTime; }
            set { this.RaiseAndSetIfChanged(ref _eventTime, value); }
        }
        public ScenarioImageViewModel AlteredState
        {
            get { return _alteredState; }
            set { this.RaiseAndSetIfChanged(ref _alteredState, value); }
        }

        public TemporaryAlterationEffectViewModel(TemporaryAlterationEffect effect) : base(effect)
        {
            this.AlterationEffectAttributes = new ObservableCollection<AlterationAttributeViewModel>();
            this.AlteredState = new ScenarioImageViewModel(effect.AlteredState, effect.RogueName);
            this.CanSeeInvisibleCharacters = effect.CanSeeInvisibleCharacters;
            this.EventTime = effect.EventTime.ToString("F1");
            this.HasAlteredState = effect.HasAlteredState;
            this.IsStackable = effect.IsStackable;

            if (effect.Agility != 0)
                this.AlterationEffectAttributes.Add(new AlterationAttributeViewModel("Agility", effect.Agility.ToString("F1")));

            if (effect.Attack != 0)
                this.AlterationEffectAttributes.Add(new AlterationAttributeViewModel("Attack", effect.Attack.ToString("F1")));

            if (effect.Defense != 0)
                this.AlterationEffectAttributes.Add(new AlterationAttributeViewModel("Defense", effect.Defense.ToString("F1")));

            if (effect.FoodUsagePerTurn != 0)
                this.AlterationEffectAttributes.Add(new AlterationAttributeViewModel("Food Usage (per turn)", effect.FoodUsagePerTurn.ToString("F1")));

            if (effect.HpPerStep != 0)
                this.AlterationEffectAttributes.Add(new AlterationAttributeViewModel("Hp Regen", effect.HpPerStep.ToString("F1")));

            if (effect.StaminaPerStep != 0)
                this.AlterationEffectAttributes.Add(new AlterationAttributeViewModel("Stamina Regen", effect.StaminaPerStep.ToString("F1")));

            if (effect.Intelligence != 0)
                this.AlterationEffectAttributes.Add(new AlterationAttributeViewModel("Intelligence", effect.Intelligence.ToString("F1")));

            if (effect.Defense != 0)
                this.AlterationEffectAttributes.Add(new AlterationAttributeViewModel("Defense", effect.Defense.ToString("F1")));

            if (effect.LightRadius != 0)
                this.AlterationEffectAttributes.Add(new AlterationAttributeViewModel("Light Radius", effect.LightRadius.ToString("F1")));

            if (effect.Speed != 0)
                this.AlterationEffectAttributes.Add(new AlterationAttributeViewModel("Speed", effect.Speed.ToString("F1")));

            if (effect.Strength != 0)
                this.AlterationEffectAttributes.Add(new AlterationAttributeViewModel("Strength", effect.Strength.ToString("F1")));
        }

        public TemporaryAlterationEffectViewModel(TemporaryAlterationEffectTemplate template) : base(template)
        {
            this.AlterationEffectAttributes = new ObservableCollection<AlterationAttributeViewModel>();
            this.AlteredState = new ScenarioImageViewModel(template.Guid, template.Name, template.Name, template.AlteredState.SymbolDetails);
            this.CanSeeInvisibleCharacters = template.CanSeeInvisibleCharacters;
            this.EventTime = template.EventTime.ToString();
            this.HasAlteredState = template.HasAlteredState;
            this.IsStackable = template.IsStackable;

            if (template.AgilityRange.IsSet())
                this.AlterationEffectAttributes.Add(new AlterationAttributeViewModel("Agility", template.AgilityRange.ToString()));

            if (template.AttackRange.IsSet())
                this.AlterationEffectAttributes.Add(new AlterationAttributeViewModel("Attack", template.AttackRange.ToString()));

            if (template.DefenseRange.IsSet())
                this.AlterationEffectAttributes.Add(new AlterationAttributeViewModel("Defense", template.DefenseRange.ToString()));

            if (template.FoodUsagePerTurnRange.IsSet())
                this.AlterationEffectAttributes.Add(new AlterationAttributeViewModel("Food Usage (per turn)", template.FoodUsagePerTurnRange.ToString()));

            if (template.HpPerStepRange.IsSet())
                this.AlterationEffectAttributes.Add(new AlterationAttributeViewModel("Hp Regen", template.HpPerStepRange.ToString()));

            if (template.StaminaPerStepRange.IsSet())
                this.AlterationEffectAttributes.Add(new AlterationAttributeViewModel("Stamina Regen", template.StaminaPerStepRange.ToString()));

            if (template.IntelligenceRange.IsSet())
                this.AlterationEffectAttributes.Add(new AlterationAttributeViewModel("Intelligence", template.IntelligenceRange.ToString()));

            if (template.DefenseRange.IsSet())
                this.AlterationEffectAttributes.Add(new AlterationAttributeViewModel("Defense", template.DefenseRange.ToString()));

            if (template.LightRadiusRange.IsSet())
                this.AlterationEffectAttributes.Add(new AlterationAttributeViewModel("Light Radius", template.LightRadiusRange.ToString()));

            if (template.SpeedRange.IsSet())
                this.AlterationEffectAttributes.Add(new AlterationAttributeViewModel("Speed", template.SpeedRange.ToString()));

            if (template.StrengthRange.IsSet())
                this.AlterationEffectAttributes.Add(new AlterationAttributeViewModel("Strength", template.StrengthRange.ToString()));
        }
    }
}
