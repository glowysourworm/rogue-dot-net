using Rogue.NET.Common.Extension;
using Rogue.NET.Common.Extension.Event;
using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Scenario;
using Rogue.NET.Core.Model.Scenario.Content.Item;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

namespace Rogue.NET.Scenario.Content.ViewModel.ItemGrid
{
    public class ConsumableItemGridRowViewModel : ItemGridRowViewModel<Consumable>
    {
        #region Backing Fields
        ICommand _increaseSelectedQuantityCommand;
        ICommand _decreaseSelectedQuantityCommand;

        ConsumableSubType _subType;
        int _requiredLevel;
        int _quantity;
        int _selectedQuantity;
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
        public int Quantity
        {
            get { return _quantity; }
            set { this.RaiseAndSetIfChanged(ref _quantity, value); }
        }
        public int SelectedQuantity
        {
            get { return _selectedQuantity; }
            set { this.RaiseAndSetIfChanged(ref _selectedQuantity, value); }
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

        // Commands for multi-selection mode
        public ICommand IncreaseSelectedQuantityCommand
        {
            get { return _increaseSelectedQuantityCommand; }
            set { this.RaiseAndSetIfChanged(ref _increaseSelectedQuantityCommand, value); }
        }
        public ICommand DecreaseSelectedQuantityCommand
        {
            get { return _decreaseSelectedQuantityCommand; }
            set { this.RaiseAndSetIfChanged(ref _decreaseSelectedQuantityCommand, value); }
        }

        public ConsumableItemGridRowViewModel(Consumable item, ScenarioMetaData metaData, string displayName, IEnumerable<Consumable> similarItems, bool isEnabled) : base(item, metaData, displayName, isEnabled)
        {
            this.IncreaseSelectedQuantityCommand = new SimpleCommand(() =>
            {
                this.SelectedQuantity++;
                this.SelectedQuantity.Clip(0, this.Quantity);

                // Fire event to update total selected quantity
                OnSelectionChanged();
            });

            this.DecreaseSelectedQuantityCommand = new SimpleCommand(() =>
            {
                this.SelectedQuantity--;
                this.SelectedQuantity.Clip(0, this.Quantity);

                // Fire event to update total selected quantity
                OnSelectionChanged();
            });

            Update(item, metaData, displayName, similarItems, isEnabled);
        }

        public override void Update(Consumable item, ScenarioMetaData metaData, string displayName, IEnumerable<Consumable> similarItems, bool isEnabled)
        {
            base.Update(item, metaData, displayName, similarItems, isEnabled);

            // UI Property
            this.SelectedQuantity = 0;

            this.SubType = item.SubType;
            this.RequiredLevel = item.LevelRequired;
            this.Quantity = similarItems.Count();
            this.Uses = similarItems.Sum(x => x.Uses);
            this.Weight = similarItems.Sum(x => x.Weight);
            this.HasCharacterClassRequirement = item.HasCharacterClassRequirement;
            this.CharacterClass = item.CharacterClass;
        }
    }
}
