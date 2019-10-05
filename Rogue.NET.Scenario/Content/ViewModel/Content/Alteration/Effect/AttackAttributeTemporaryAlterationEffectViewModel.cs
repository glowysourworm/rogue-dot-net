using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Scenario.Alteration.Effect;
using Rogue.NET.Core.Model.ScenarioConfiguration.Alteration.Common;
using Rogue.NET.Scenario.Content.ViewModel.Attribute;
using Rogue.NET.Scenario.Content.ViewModel.Content.Alteration.Common;
using Rogue.NET.Scenario.Content.ViewModel.Content.ScenarioMetaData;
using System.Collections.ObjectModel;
using System.Linq;

namespace Rogue.NET.Scenario.Content.ViewModel.Content.Alteration.Effect
{
    [UIDisplay(Name = "Attack Attribute (Temporary)",
               Description = "Creates a timed (Friendly or Malign) contribution to the affected character(s) Attack Attributes")]
    public class AttackAttributeTemporaryAlterationEffectViewModel : AlterationEffectViewModel
    {
        AlterationAttackAttributeCombatType _combatType;
        ScenarioImageViewModel _alteredState;
        bool _isStackable;
        bool _hasAlteredState;
        string _eventTime;

        public AlterationAttackAttributeCombatType CombatType
        {
            get { return _combatType; }
            set { this.RaiseAndSetIfChanged(ref _combatType, value); }
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

        public ObservableCollection<AttackAttributeViewModel> AttackAttributes { get; set; }

        public AttackAttributeTemporaryAlterationEffectViewModel(AttackAttributeTemporaryAlterationEffect effect)
                : base(effect)
        {
            this.AlteredState = new ScenarioImageViewModel(effect.AlteredState, effect.RogueName);
            this.CombatType = effect.CombatType;
            this.EventTime = effect.EventTime.ToString();
            this.HasAlteredState = effect.HasAlteredState;
            this.IsStackable = effect.IsStackable;

            this.AttackAttributes = new ObservableCollection<AttackAttributeViewModel>(
                effect.AttackAttributes
                      .Where(x => x.Attack > 0 || x.Resistance > 0 || x.Weakness > 0)
                      .Select(x => new AttackAttributeViewModel(x)));
        }

        public AttackAttributeTemporaryAlterationEffectViewModel(AttackAttributeTemporaryAlterationEffectTemplate template)
                : base(template)
        {
            this.AlteredState = new ScenarioImageViewModel(template.Guid, template.Name, template.Name, template.AlteredState.SymbolDetails);
            this.CombatType = template.CombatType;
            this.EventTime = template.EventTime.ToString();
            this.HasAlteredState = template.HasAlteredState;
            this.IsStackable = template.IsStackable;

            this.AttackAttributes = new ObservableCollection<AttackAttributeViewModel>(
                template.AttackAttributes
                        .Where(x => x.Attack.IsSet() || x.Resistance.IsSet() || x.Weakness.IsSet() || x.Immune)
                        .Select(x => new AttackAttributeViewModel(x)));
        }
    }
}
