﻿using System.Collections.ObjectModel;

namespace Rogue.NET.Scenario.Content.ViewModel.Message
{
    public class ScenarioMeleeMessageViewModel : ScenarioMessageViewModel
    {
        string _actorDisplayName;
        string _acteeDisplayName;
        double _baseHit;
        bool _isCriticalHit;
        bool _anyAttackAttributes;

        public string ActorDisplayName
        {
            get { return _actorDisplayName; }
            set { this.RaiseAndSetIfChanged(ref _actorDisplayName, value); }
        }
        public string ActeeDisplayName
        {
            get { return _acteeDisplayName; }
            set { this.RaiseAndSetIfChanged(ref _acteeDisplayName, value); }
        }
        public double BaseHit
        {
            get { return _baseHit; }
            set { this.RaiseAndSetIfChanged(ref _baseHit, value); }
        }
        public bool IsCriticalHit
        {
            get { return _isCriticalHit; }
            set { this.RaiseAndSetIfChanged(ref _isCriticalHit, value); }
        }
        public bool AnyAttackAttributes
        {
            get { return _anyAttackAttributes; }
            set { this.RaiseAndSetIfChanged(ref _anyAttackAttributes, value); }
        }

        public ObservableCollection<AttackAttributeHitViewModel> AttackAttributeHits { get; set; }
    }
}
