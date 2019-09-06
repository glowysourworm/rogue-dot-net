using Rogue.NET.Common.Extension.Event;
using Rogue.NET.Core.Model.Scenario;
using Rogue.NET.Core.Model.Scenario.Content;
using Rogue.NET.Scenario.Content.ViewModel.Content.ScenarioMetaData;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Windows.Input;

namespace Rogue.NET.Scenario.Content.ViewModel.ItemGrid.ItemGridRow
{
    /// <summary>
    /// Item Grid Row that implements a single-item row (non-grouped)
    /// </summary>
    [PartCreationPolicy(CreationPolicy.NonShared)]
    [Export]
    public abstract class ItemGridRowViewModel<T> : ItemGridRowViewModelBase<T> where T : ScenarioImage
    {
        public override event SimpleEventHandler<ItemGridRowViewModelBase<T>> SelectionChangedEvent;

        // The item that this grid row represents
        T _item;

        bool _isSelected;

        public override bool IsSelected
        {
            get { return _isSelected; }
            set { this.RaiseAndSetIfChanged(ref _isSelected, value); OnSelectionChanged(); }
        }
        public override IEnumerable<string> GetSelectedItemIds()
        {
            return _isSelected ? new string[] { _item.Id } : new string[] { };
        }

        /// <summary>
        /// Constructor for item grid row that takes the item + similar items (having the same RogueBase.RogueName)
        /// which means that they stack on top of each other in the item grid. Certain properties may then be
        /// aggregate (such as weight). BY CONVENTION SIMILAR ITEMS INCLUDES THE ITEM
        /// </summary>
        public ItemGridRowViewModel(T item, ScenarioMetaData metaData, string displayName, bool isEnabled) 
                : base(item, metaData, displayName, isEnabled)
        {
            this.IsSelected = false;

            _item = item;
        }

        public override void Update(T item, ScenarioMetaData metaData, string displayName, bool isEnabled)
        {
            base.Update(item, metaData, displayName, isEnabled);

            this.IsSelected = false;

            _item = item;
        }

        private void OnSelectionChanged()
        {
            if (this.SelectionChangedEvent != null)
                this.SelectionChangedEvent(this);
        }
    }
}
