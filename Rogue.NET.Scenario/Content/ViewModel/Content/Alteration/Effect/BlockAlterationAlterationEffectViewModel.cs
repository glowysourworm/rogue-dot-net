﻿using Rogue.NET.Core.Model.Scenario.Alteration.Effect;
using Rogue.NET.Core.Model.ScenarioConfiguration.Alteration.Common;
using Rogue.NET.Scenario.Content.ViewModel.Attribute;
using Rogue.NET.Scenario.Content.ViewModel.Content.Alteration.Common;
using Rogue.NET.Scenario.Content.ViewModel.Content.ScenarioMetaData;

namespace Rogue.NET.Scenario.Content.ViewModel.Content.Alteration.Effect
{
    [UIDisplay(Name = "Detect Effects",
               Description = "Effect that finds objects in the scenario with hidden effects")]
    public class BlockAlterationAlterationEffectViewModel : AlterationEffectViewModel
    {
        ScenarioImageViewModel _alterationCategory;

        public ScenarioImageViewModel AlterationCategory
        {
            get { return _alterationCategory; }
            set { this.RaiseAndSetIfChanged(ref _alterationCategory, value); }
        }

        public BlockAlterationAlterationEffectViewModel(BlockAlterationAlterationEffect effect) : base(effect)
        {
            this.AlterationCategory = new ScenarioImageViewModel(effect.AlterationCategory, effect.RogueName);
        }

        public BlockAlterationAlterationEffectViewModel(BlockAlterationAlterationEffectTemplate template) : base(template)
        {
            this.AlterationCategory = new ScenarioImageViewModel(template.Guid, template.Name, template.Name, template.AlterationCategory.SymbolDetails);
        }
    }
}
