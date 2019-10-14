using Rogue.NET.Common.Extension.Event;
using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Scenario.Abstract;
using Rogue.NET.Core.Model.Scenario.Content;
using Rogue.NET.Scenario.Content.ViewModel.Content.ScenarioMetaData;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Windows.Input;

namespace Rogue.NET.Scenario.Content.ViewModel.ItemGrid.ItemGridRow
{
    [PartCreationPolicy(CreationPolicy.NonShared)]
    [Export]
    public abstract class ItemGridRowViewModelBase<T> : ScenarioImageViewModel where T : ScenarioObject
    {
        /// <summary>
        /// Event that notifies container class about single item selection
        /// </summary>
        public event SimpleAsyncEventHandler<ItemGridRowViewModelBase<T>> ProcessSingleItemEvent;

        /// <summary>
        /// Occurs when number of selected items changes to notify collection holder to increment a
        /// total counter
        /// </summary>
        public abstract event SimpleEventHandler<ItemGridRowViewModelBase<T>> SelectionChangedEvent;

        ICommand _processSingleItemCommand;

        bool _isEnabled;
        bool _isObjective;
        bool _isUnique;
        bool _isDetected;

        ScenarioImageViewModel _isDetectedImage;

        public abstract bool IsSelected { get; set; }
        public abstract IEnumerable<string> GetSelectedItemIds();

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
        public bool IsDetected
        {
            get { return _isDetected; }
            set { this.RaiseAndSetIfChanged(ref _isDetected, value); }
        }
        public ScenarioImageViewModel IsDetectedImage
        {
            get { return _isDetectedImage; }
            set { this.RaiseAndSetIfChanged(ref _isDetectedImage, value); }
        }

        /// <summary>
        /// Primary command for processing intended action with single selected item
        /// </summary>
        public ICommand ProcessSingleItemCommand
        {
            get { return _processSingleItemCommand; }
            set { this.RaiseAndSetIfChanged(ref _processSingleItemCommand, value); }
        }

        /// <summary>
        /// Constructor for item grid row that takes the item + similar items (having the same RogueBase.RogueName)
        /// which means that they stack on top of each other in the item grid. Certain properties may then be
        /// aggregate (such as weight). BY CONVENTION SIMILAR ITEMS INCLUDES THE ITEM
        /// </summary>
        public ItemGridRowViewModelBase(T item, ScenarioMetaData metaData, string displayName, bool isEnabled) : base(item, displayName)
        {
            this.IsEnabled = isEnabled;
            this.IsObjective = metaData.IsObjective;
            this.IsUnique = metaData.IsUnique;

            // TODO:DEBOUNCE
            this.ProcessSingleItemCommand = new SimpleAsyncCommand(async () =>
            {
                if (this.ProcessSingleItemEvent != null)
                    await this.ProcessSingleItemEvent(this);

            }, () => true);
        }

        /// <summary>
        /// Specifies how to update the instance of ItemGridRowViewModel from its source object
        /// </summary>
        public virtual void Update(T item, ScenarioMetaData metaData, string displayName, bool isEnabled)
        {
            this.IsEnabled = isEnabled;
            this.DisplayName = displayName;

            this.IsUnique = metaData.IsUnique;
            this.IsObjective = metaData.IsObjective;
            this.IsDetected = item.IsDetectedAlignment || item.IsDetectedCategory;

            if (item.IsDetectedAlignment)
            {
                // TODO: WE HAVE TO SUPPORT MULTIPLE DETECTED TYPES AND CATEGORIES PER ITEM.
                switch (item.DetectedAlignmentType)
                {
                    case AlterationAlignmentType.Neutral:
                        this.IsDetectedImage = new ScenarioImageViewModel(ScenarioImage.CreateGameSymbol(item.RogueName, Common.Constant.GameSymbol.DetectMagicNeutral), item.RogueName);
                        break;
                    case AlterationAlignmentType.Good:
                        this.IsDetectedImage = new ScenarioImageViewModel(ScenarioImage.CreateGameSymbol(item.RogueName, Common.Constant.GameSymbol.DetectMagicGood), item.RogueName);
                        break;
                    case AlterationAlignmentType.Bad:
                        this.IsDetectedImage = new ScenarioImageViewModel(ScenarioImage.CreateGameSymbol(item.RogueName, Common.Constant.GameSymbol.DetectMagicBad), item.RogueName);
                        break;
                    default:
                        throw new System.Exception("Unhandled Alteration Alignment Type ItemGridRowViewModelBase");
                }
            }
            else if (item.IsDetectedCategory)
            {
                this.IsDetectedImage = new ScenarioImageViewModel(item.DetectedAlignmentCategory, item.DetectedAlignmentCategory.RogueName);
            }
            // Default Symbol = "?"
            else
                this.IsDetectedImage = new ScenarioImageViewModel(ScenarioImage.CreateGameSymbol(item.RogueName, Common.Constant.GameSymbol.Identify), item.RogueName);
        }
    }
}
