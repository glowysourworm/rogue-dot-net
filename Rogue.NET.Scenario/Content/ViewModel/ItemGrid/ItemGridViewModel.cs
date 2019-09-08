using Rogue.NET.Common.Extension.Prism.EventAggregator;
using Rogue.NET.Common.ViewModel;
using Rogue.NET.Core.Model.Scenario.Content.Item;
using Rogue.NET.Common.Extension.Event;
using Rogue.NET.Scenario.Content.ViewModel.ItemGrid.ItemGridRow;
using Rogue.NET.Common.Extension;
using Rogue.NET.Core.Processing.Event.Backend;
using Rogue.NET.Core.GameRouter.GameEvent.Backend.Enum;

using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Media;
using Rogue.NET.Core.Processing.Service.Interface;
using Rogue.NET.Core.Processing.Event.Level;

namespace Rogue.NET.Scenario.Content.ViewModel.ItemGrid
{
    public abstract class ItemGridViewModel<TViewModel, TModel> 
            : NotifyViewModel where TViewModel : ItemGridRowViewModelBase<TModel>
                              where TModel : ItemBase
    {
        /// <summary>
        /// Event to notify UI listeners on selection changed (single selection DIALOG mode only). During
        /// multiple selection - it's assumed the UI is bound to the total selecte count to provide
        /// an OK button.
        /// </summary>
        public event SimpleEventHandler<TViewModel> SingleDialogSelectionEvent;

        readonly IRogueEventAggregator _eventAggregator;
        readonly IModelService _modelService;

        bool _isDialog;
        int _totalSelected;

        /// <summary>
        /// Set to true to prevent automatic single-item events from firing
        /// </summary>
        public bool IsDialog
        {
            get { return _isDialog; }
            set { this.RaiseAndSetIfChanged(ref _isDialog, value); }
        }
        public int TotalSelected
        {
            get { return _totalSelected; }
            set { this.RaiseAndSetIfChanged(ref _totalSelected, value); }
        }
        public abstract string Header { get; }
        public abstract Brush HeaderBrush { get; }
        public ObservableCollection<TViewModel> Items { get; set; }

        protected abstract IEnumerable<TModel> GetCollection(IModelService modelService);
        protected abstract Func<TModel, TViewModel> GetItemConstructor(IModelService modelService);
        protected abstract Action<TModel, TViewModel> GetItemUpdater(IModelService modelService);
        protected abstract bool GetIsEnabled(IModelService modelService, TModel item);

        public ItemGridViewModel(IRogueEventAggregator eventAggregator, IModelService modelService)
        {
            _eventAggregator = eventAggregator;
            _modelService = modelService;

            this.Items = new ObservableCollection<TViewModel>();

            // Player Events
            eventAggregator.GetEvent<LevelEvent>().Subscribe(update =>
            {
                // TODO: This could be refined by the inherited classes
                switch (update.LevelUpdateType)
                {
                    case LevelEventType.PlayerConsumableRemove:
                    case LevelEventType.PlayerConsumableAddOrUpdate:
                    case LevelEventType.PlayerEquipmentRemove:
                    case LevelEventType.PlayerEquipmentAddOrUpdate:
                    case LevelEventType.PlayerAll:
                    case LevelEventType.EncyclopediaCurseIdentify:
                    case LevelEventType.EncyclopediaIdentify:
                        Update();
                        break;
                    default:
                        break;
                }
            });

            // Level Loaded
            eventAggregator.GetEvent<LevelLoadedEvent>().Subscribe(() =>
            {
                Update();
            });

            // Initialize in base didn't work because the model service is locally handled
            //
            // This will initialize if the model service is loaded. otherwise, event aggregator
            // will handle data loading as soon as the level is loaded.
            if (modelService.IsLoaded)
                Update();
        }

        /// <summary>
        /// Method to process a non-dialog mode single item event (using the event aggregator)
        /// </summary>
        protected abstract Task ProcessSingleItemNonDialog(IRogueEventAggregator eventAggregator, string itemId);
        protected virtual Task ProcessSingleItem(ItemGridRowViewModelBase<TModel> item)
        {
            // First, handle dialog mode events - these will process event aggregator publish
            // tasks in the IDialogContainer
            //
            if (this.IsDialog)
            {
                if (this.SingleDialogSelectionEvent != null)
                {
                    this.SingleDialogSelectionEvent((TViewModel)item);
                    return Task.Delay(1);
                }
                else
                    throw new Exception("Improper use of dialog mode / single-selection ItemGridViewModel");
            }
            else
                return ProcessSingleItemNonDialog(_eventAggregator, item.Id);
        }
        protected virtual void Update()
        {
            // Unhook current item collection
            UnHookItems();

            // Synchronize item collection using implementations of the abstract delegates
            this.Items.SynchronizeFrom(

                // Source
                GetCollection(_modelService),

                // Comparer (Always Id's for Items)
                (model, viewModel) => model.Id == viewModel.Id,

                // Constructor
                GetItemConstructor(_modelService),

                // Update
                GetItemUpdater(_modelService));

            // Hook new item collection events
            HookItems();
        }

        // Adds listeners for events on the Items collection to process primary single item command
        private void HookItems()
        {
            foreach (var item in this.Items)
            {
                item.ProcessSingleItemEvent += ProcessSingleItem;
                item.SelectionChangedEvent += OnSelectionChanged;
            }
        }

        // Removes listeners for events on the Items collection to process primary single item command
        private void UnHookItems()
        {
            foreach (var item in this.Items)
            {
                item.ProcessSingleItemEvent -= ProcessSingleItem;
                item.SelectionChangedEvent -= OnSelectionChanged;
            }
        }

        /// <summary>
        /// Gets an aggregate of selected item id's for the entire collection
        /// </summary>
        public IEnumerable<string> GetSelectedItemIds()
        {
            return this.Items.Aggregate(new List<string>(), (aggreagate, item) =>
            {
                aggreagate.AddRange(item.GetSelectedItemIds());

                return aggreagate;
            });
        }

        private void OnSelectionChanged(ItemGridRowViewModelBase<TModel> sender)
        {
            // Update total items selected
            this.TotalSelected = this.Items.Sum(item =>
            {
                if (item is ConsumableItemGridRowViewModel)
                    return (item as ConsumableItemGridRowViewModel).SelectedQuantity;

                else
                    return item.IsSelected ? 1 : 0;
            });
        }
    }
}
