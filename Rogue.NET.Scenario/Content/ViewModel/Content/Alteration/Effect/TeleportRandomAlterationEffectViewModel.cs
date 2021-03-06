﻿using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Scenario.Alteration.Effect;
using Rogue.NET.Core.Model.ScenarioConfiguration.Alteration.Common;
using Rogue.NET.Scenario.Content.ViewModel.Attribute;
using Rogue.NET.Scenario.Content.ViewModel.Content.Alteration.Common;

namespace Rogue.NET.Scenario.Content.ViewModel.Content.Alteration.Effect
{
    [UIDisplay(Name = "Teleport",
               Description = "Transports a source character to a random location")]
    public class TeleportRandomAlterationEffectViewModel : AlterationEffectViewModel
    {
        AlterationRandomPlacementType _teleportType;
        string _range;

        public AlterationRandomPlacementType TeleportType
        {
            get { return _teleportType; }
            set { this.RaiseAndSetIfChanged(ref _teleportType, value); }
        }

        public string Range
        {
            get { return _range; }
            set { this.RaiseAndSetIfChanged(ref _range, value); }
        }

        public TeleportRandomAlterationEffectViewModel(TeleportRandomAlterationEffect effect) : base(effect)
        {
            this.TeleportType = effect.TeleportType;
            this.Range = effect.Range.ToString("N0");
        }

        public TeleportRandomAlterationEffectViewModel(TeleportRandomAlterationEffectTemplate template) : base(template)
        {
            this.TeleportType = template.TeleportType;
            this.Range = template.Range.ToString("N0");
        }
    }
}
