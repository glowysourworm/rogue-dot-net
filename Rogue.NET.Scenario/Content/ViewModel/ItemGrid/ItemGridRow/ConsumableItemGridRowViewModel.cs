using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Scenario.Abstract;
using Rogue.NET.Core.Model.Scenario.Content.Item;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;

namespace Rogue.NET.Scenario.Content.ViewModel.ItemGrid.ItemGridRow
{
    [PartCreationPolicy(CreationPolicy.NonShared)]
    [Export]
    public class ConsumableItemGridRowViewModel : ItemGridGroupedRowViewModel<Consumable>
    {
        #region Backing Fields
        ConsumableSubType _subType;
        int _requiredLevel;
        int _uses;
        double _weight;
        bool _hasCharacterClassRequirement;
        string _characterClass;
        #endregion

        public ConsumableSubType SubType
        {
            get { return _subType; }
            set { this.RaiseAndSetIfChanged(ref _subType, value); }
        }
        public int RequiredLevel
        {
            get { return _requiredLevel; }
            set { this.RaiseAndSetIfChanged(ref _requiredLevel, value); }
        }
        public int Uses
        {
            get { return _uses; }
            set { this.RaiseAndSetIfChanged(ref _uses, value); }
        }
        public double Weight
        {
            get { return _weight; }
            set { this.RaiseAndSetIfChanged(ref _weight, value); }
        }
        public bool HasCharacterClassRequirement
        {
            get { return _hasCharacterClassRequirement; }
            set { this.RaiseAndSetIfChanged(ref _hasCharacterClassRequirement, value); }
        }
        public string CharacterClass
        {
            get { return _characterClass; }
            set { this.RaiseAndSetIfChanged(ref _characterClass, value); }
        }


        public ConsumableItemGridRowViewModel(Consumable item, ScenarioMetaData metaData, string displayName, IEnumerable<Consumable> similarItems, bool isEnabled) : base(item, metaData, displayName, isEnabled, similarItems)
        {
            UpdateGrouped(item, metaData, displayName, similarItems, isEnabled);
        }

        public override void UpdateGrouped(Consumable item, ScenarioMetaData metaData, string displayName, IEnumerable<Consumable> similarItems, bool isEnabled)
        {
            base.UpdateGrouped(item, metaData, displayName, similarItems, isEnabled);

            this.SubType = item.SubType;
            this.RequiredLevel = item.LevelRequired;
            this.Uses = similarItems.Sum(x => x.Uses);
            this.Weight = similarItems.Sum(x => x.Weight);
            this.HasCharacterClassRequirement = item.HasCharacterClassRequirement;
            this.CharacterClass = item.CharacterClass;
        }
    }
}
