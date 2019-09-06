using Rogue.NET.Common.Extension;
using Rogue.NET.Common.Extension.Event;
using Rogue.NET.Core.Model.Scenario;
using Rogue.NET.Core.Model.Scenario.Content;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows.Input;

namespace Rogue.NET.Scenario.Content.ViewModel.ItemGrid.ItemGridRow
{
    /// <summary>
    /// Item Grid Row implementation that groups items to provide group quantities
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [PartCreationPolicy(CreationPolicy.NonShared)]
    [Export]
    public abstract class ItemGridGroupedRowViewModel<T> : ItemGridRowViewModelBase<T> where T : ScenarioImage
    {
        public override event SimpleEventHandler<ItemGridRowViewModelBase<T>> SelectionChangedEvent;

        ICommand _increaseSelectedQuantityCommand;
        ICommand _decreaseSelectedQuantityCommand;

        int _quantity;
        int _selectedQuantity;
        bool _isSelected;

        // This is a list of all items that belong to the group
        List<T> _groupedItems { get; set; }

        /// <summary>
        /// Returns true if the total selected quantity is greater than zero
        /// </summary>
        public override bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                if (value)
                {
                    _isSelected = true;
                    _selectedQuantity = _selectedQuantity == 0 ? 1 : _selectedQuantity;
                }
                else
                {
                    _isSelected = false;
                    _selectedQuantity = 0;
                }

                OnPropertyChanged(() => this.IsSelected);
                OnPropertyChanged(() => this.SelectedQuantity);
            }
        }
        public override IEnumerable<string> GetSelectedItemIds()
        {
            return _groupedItems.Select(x => x.Id)
                                .Take(_selectedQuantity)
                                .Actualize();
        }

        // Properties that apply to a group of items
        public int Quantity
        {
            get { return _quantity; }
            set { this.RaiseAndSetIfChanged(ref _quantity, value); }
        }
        public int SelectedQuantity
        {
            get { return _selectedQuantity; }
            set
            {
                if (value > 0)
                    _isSelected = true;

                else
                    _isSelected = false;

                _selectedQuantity = value;

                OnPropertyChanged(() => this.IsSelected);
                OnPropertyChanged(() => this.SelectedQuantity);
            }
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

        /// <summary>
        /// Constructor for item grid row that takes the item + similar items (having the same RogueBase.RogueName)
        /// which means that they stack on top of each other in the item grid. Certain properties may then be
        /// aggregate (such as weight). BY CONVENTION SIMILAR ITEMS INCLUDES THE ITEM
        /// </summary>
        public ItemGridGroupedRowViewModel(T item, ScenarioMetaData metaData, string displayName, bool isEnabled, IEnumerable<T> groupedItems) 
                : base(item, metaData, displayName, isEnabled)
        {
            _groupedItems = new List<T>(groupedItems);

            this.IncreaseSelectedQuantityCommand = new SimpleCommand(() =>
            {
                this.SelectedQuantity++;
                this.SelectedQuantity = this.SelectedQuantity.Clip(0, this.Quantity);

                OnSelectionChanged();
            });

            this.DecreaseSelectedQuantityCommand = new SimpleCommand(() =>
            {
                this.SelectedQuantity--;
                this.SelectedQuantity = this.SelectedQuantity.Clip(0, this.Quantity);

                OnSelectionChanged();
            });
        }

        public virtual void UpdateGrouped(T scenarioImage, ScenarioMetaData metaData, string displayName, IEnumerable<T> similarItems, bool isEnabled)
        {
            base.Update(scenarioImage, metaData, displayName, isEnabled);

            this.SelectedQuantity = 0;
            this.Quantity = similarItems.Count();

            _groupedItems.Clear();
            _groupedItems.AddRange(similarItems);
        }

        // Notify listeners that the selected quantity / is selected has changed
        private void OnSelectionChanged()
        {
            if (this.SelectionChangedEvent != null)
                this.SelectionChangedEvent(this);
        }
    }
}
