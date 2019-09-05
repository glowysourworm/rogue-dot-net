using Rogue.NET.Common.Extension.Event;
using Rogue.NET.Core.Model.Scenario;
using Rogue.NET.Core.Model.Scenario.Content;
using Rogue.NET.Scenario.Content.ViewModel.Content.ScenarioMetaData;
using System;
using System.Collections.Generic;
using System.Windows.Input;

namespace Rogue.NET.Scenario.Content.ViewModel.ItemGrid
{
    public abstract class ItemGridRowViewModel<T> : ScenarioImageViewModel where T : ScenarioImage
    {
        ICommand _processSingleItemCommand;

        bool _isSelected;
        bool _isEnabled;
        bool _isObjective;
        bool _isUnique;

        public bool IsSelected
        {
            get { return _isSelected; }
            set { this.RaiseAndSetIfChanged(ref _isSelected, value); OnSelectionChanged(); }
        }

        /// <summary>
        /// Gets / Sets a value to say whether item grid row should be allowed to show its button
        /// for user selection.
        /// </summary>
        public bool IsEnabled
        {
            get { return _isEnabled; }
            set { this.RaiseAndSetIfChanged(ref _isEnabled, value); }
        }
        public bool IsObjective
        {
            get { return _isObjective; }
            set { this.RaiseAndSetIfChanged(ref _isObjective, value); }
        }
        public bool IsUnique
        {
            get { return _isUnique; }
            set { this.RaiseAndSetIfChanged(ref _isUnique, value); }
        }

        /// <summary>
        /// Primary command for processing intended action with single selected item
        /// </summary>
        public ICommand ProcessSingleItemCommand
        {
            get { return _processSingleItemCommand; }
            set { this.RaiseAndSetIfChanged(ref _processSingleItemCommand, value); }
        }

        public event SimpleAsyncEventHandler<ItemGridRowViewModel<T>> ProcessSingleItemEvent;
        public event SimpleEventHandler SelectionChanged;

        /// <summary>
        /// Constructor for item grid row that takes the item + similar items (having the same RogueBase.RogueName)
        /// which means that they stack on top of each other in the item grid. Certain properties may then be
        /// aggregate (such as weight). BY CONVENTION SIMILAR ITEMS INCLUDES THE ITEM
        /// </summary>
        public ItemGridRowViewModel(T item, ScenarioMetaData metaData, string displayName, bool isEnabled) : base(item, displayName)
        {
            this.IsEnabled = isEnabled;
            this.IsObjective = metaData.IsObjective;
            this.IsUnique = metaData.IsUnique;

            this.ProcessSingleItemCommand = new SimpleAsyncCommand(async () =>
            {
                if (this.ProcessSingleItemEvent != null)
                    await this.ProcessSingleItemEvent(this);

            }, () => true);
        }

        /// <summary>
        /// Specifies how to update the instance of ItemGridRowViewModel from its source object
        /// </summary>
        public virtual void Update(T scenarioImage, ScenarioMetaData metaData, string displayName, IEnumerable<T> similarItems, bool isEnabled)
        {
            this.IsEnabled = isEnabled;

            // Go ahead and reset "IsSelected" because update typically follows some kind of model update
            // action (which means the use for the grid is "over")
            this.IsSelected = false;

            this.DisplayName = displayName;
        }

        /// <summary>
        /// Fires an event to update the total selected quantity
        /// </summary>
        protected void OnSelectionChanged()
        {
            if (this.SelectionChanged != null)
                this.SelectionChanged();
        }
    }
}
