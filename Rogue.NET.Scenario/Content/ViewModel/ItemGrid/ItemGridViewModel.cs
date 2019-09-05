using Rogue.NET.Common.Extension.Prism.EventAggregator;
using Rogue.NET.Common.ViewModel;
using Rogue.NET.Core.Event.Scenario.Level.Command;
using Rogue.NET.Core.Event.Scenario.Level.Event;
using Rogue.NET.Core.Event.Scenario.Level.EventArgs;
using Rogue.NET.Core.Logic.Processing.Enum;
using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Scenario.Content.Item;
using Rogue.NET.Core.Service.Interface;
using Rogue.NET.Model.Events;
using Rogue.NET.Scenario.Content.ViewModel.ItemGrid.Enum;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;
using Rogue.NET.Common.Extension.Event;

namespace Rogue.NET.Scenario.Content.ViewModel.ItemGrid
{
    public abstract class ItemGridViewModel<T> : NotifyViewModel, IDisposable where T : ItemBase
    {
        /// <summary>
        /// Event to notify UI listeners on selection changed (single selection DIALOG mode only). During
        /// multiple selection - it's assumed the UI is bound to the total selecte count to provide
        /// an OK button.
        /// </summary>
        public event SimpleEventHandler<ItemGridRowViewModel<T>> SingleDialogSelectionEvent;

        readonly IRogueEventAggregator _eventAggregator;
        readonly IModelService _modelService;
        readonly string _levelLoadedToken;
        readonly string _levelUpdateToken;

        bool _isDisposed;
        bool _isDialog;

        ItemGridIntendedAction _intendedAction;
        ItemGridSelectionMode _selectionMode;
        string _header;
        Brush _headerBrush;
        int _totalSelected;

        /// <summary>
        /// Set to true to prevent automatic single-item events from firing
        /// </summary>
        public bool IsDialog
        {
            get { return _isDialog; }
            set { this.RaiseAndSetIfChanged(ref _isDialog, value); }
        }

        public ItemGridIntendedAction IntendedAction
        {
            get { return _intendedAction; }
            set
            {
                this.RaiseAndSetIfChanged(ref _intendedAction, value);

                // This is to protect the code path during initialization.
                if (_modelService.IsLoaded)
                    Update(_modelService);

                UpdateHeader();
            }
        }
        public ItemGridSelectionMode SelectionMode
        {
            get { return _selectionMode; }
            set { this.RaiseAndSetIfChanged(ref _selectionMode, value); }
        }
        public string Header
        {
            get { return _header; }
            set { this.RaiseAndSetIfChanged(ref _header, value); }
        }
        public Brush HeaderBrush
        {
            get { return _headerBrush; }
            set { this.RaiseAndSetIfChanged(ref _headerBrush, value); }
        }
        public int TotalSelected
        {
            get { return _totalSelected; }
            set { this.RaiseAndSetIfChanged(ref _totalSelected, value); }
        }

        public ObservableCollection<ItemGridRowViewModel<T>> Items { get; set; }

        public ItemGridViewModel(IRogueEventAggregator eventAggregator, IModelService modelService)
        {
            _eventAggregator = eventAggregator;
            _modelService = modelService;

            this.Items = new ObservableCollection<ItemGridRowViewModel<T>>();

            // Player Events
            _levelUpdateToken = eventAggregator.GetEvent<LevelUpdateEvent>().Subscribe(update =>
            {
                // TODO: This could be refined by the inherited classes
                switch (update.LevelUpdateType)
                {
                    case LevelUpdateType.PlayerConsumableRemove:
                    case LevelUpdateType.PlayerConsumableAddOrUpdate:
                    case LevelUpdateType.PlayerEquipmentRemove:
                    case LevelUpdateType.PlayerEquipmentAddOrUpdate:
                    case LevelUpdateType.PlayerAll:
                    case LevelUpdateType.EncyclopediaCurseIdentify:
                    case LevelUpdateType.EncyclopediaIdentify:
                        Update(modelService);
                        break;
                    default:
                        break;
                }
            });

            // Level Loaded
            _levelLoadedToken = eventAggregator.GetEvent<LevelLoadedEvent>().Subscribe(() =>
            {
                Update(modelService);
            });
        }

        protected virtual Task ProcessSingleItem(ItemGridRowViewModel<T> item)
        {
            // First, handle dialog mode events - these will process event aggregator publish
            // tasks in the IDialogContainer
            //
            if (this.IsDialog)
            {
                // MUST SET TO SELECTED BECAUSE OF THE WAY THE EVENT IS PROPAGATED BACK
                item.IsSelected = true;

                if (this.SingleDialogSelectionEvent != null)
                {
                    this.SingleDialogSelectionEvent(item);
                    return Task.Delay(1);
                }
                else
                    throw new Exception("Improper use of dialog mode / single-selection ItemGridViewModel");
            }

            // Level Action
            switch (this.IntendedAction)
            {
                // These actions are "Level Commands" -> no other data needs to be sent back
                case ItemGridIntendedAction.Consume:
                case ItemGridIntendedAction.Drop:
                case ItemGridIntendedAction.Throw:
                case ItemGridIntendedAction.Equip:
                    {
                        LevelActionType levelAction;

                        if (System.Enum.TryParse(this.IntendedAction.ToString(), out levelAction))
                        {
                            // Primary use is non-dialog (player sub-panel equipment / consumables)
                            return _eventAggregator.GetEvent<UserCommandEvent>()
                                                   .Publish(new LevelCommandEventArgs(levelAction,
                                                                                      Compass.Null,
                                                                                      item.Id));
                        }
                        else
                            throw new Exception("Unknown Level Action Type");
                    }
                default:
                    throw new Exception("Unhandled Item Grid Intended Action");
            }
        }
        protected void UpdateHeader()
        {
            switch (_intendedAction)
            {
                case ItemGridIntendedAction.Consume:
                    {
                        this.Header = "Consume";
                        this.HeaderBrush = Brushes.Fuchsia;
                    }
                    break;
                case ItemGridIntendedAction.Drop:
                    {
                        this.Header = "Drop";
                        this.HeaderBrush = Brushes.Red;
                    }
                    break;
                case ItemGridIntendedAction.EnchantWeapon:
                    {
                        this.Header = "Enchant Weapon";
                        this.HeaderBrush = Brushes.Tan;
                    }
                    break;
                case ItemGridIntendedAction.EnchantArmor:
                    {
                        this.Header = "Enchant Armor";
                        this.HeaderBrush = Brushes.Tan;
                    }
                    break;
                case ItemGridIntendedAction.ImbueArmor:
                    {
                        this.Header = "Imbue Armor";
                        this.HeaderBrush = Brushes.OrangeRed;
                    }
                    break;
                case ItemGridIntendedAction.ImbueWeapon:
                    {
                        this.Header = "Imbue Weapon";
                        this.HeaderBrush = Brushes.OrangeRed;
                    }
                    break;
                case ItemGridIntendedAction.EnhanceWeapon:
                    {
                        this.Header = "Enhance Weapon";
                        this.HeaderBrush = Brushes.Beige;
                    }
                    break;
                case ItemGridIntendedAction.EnhanceArmor:
                    {
                        this.Header = "Enhance Armor";
                        this.HeaderBrush = Brushes.Beige;
                    }
                    break;
                case ItemGridIntendedAction.Equip:
                    {
                        this.Header = "Equip";
                        this.HeaderBrush = Brushes.White;
                    }
                    break;
                case ItemGridIntendedAction.Identify:
                    {
                        this.Header = "Identify";
                        this.HeaderBrush = Brushes.Yellow;
                    }
                    break;
                case ItemGridIntendedAction.Throw:
                    {
                        this.Header = "Throw";
                        this.HeaderBrush = Brushes.Orange;
                    }
                    break;
                case ItemGridIntendedAction.Uncurse:
                    {
                        this.Header = "Uncurse";
                        this.HeaderBrush = Brushes.White;
                    }
                    break;
                default:
                    throw new Exception("Unhandled Item Grid Intended Action");
            }
        }

        // TODO: Could re-work this to use abstract methods to hook the items sync procedure and
        //       then hook / unhook items in the base class; but seemed like this is ok for now.

        /// <summary>
        /// Adds listeners for events on the Items collection to process primary single item command
        /// </summary>
        protected virtual void HookItems()
        {
            foreach (var item in this.Items)
            {
                item.ProcessSingleItemEvent += ProcessSingleItem;
                item.SelectionChanged += OnSelectionChanged;
            }
        }

        /// <summary>
        /// Removes listeners for events on the Items collection to process primary single item command
        /// </summary>
        protected virtual void UnHookItems()
        {
            foreach (var item in this.Items)
            {
                item.ProcessSingleItemEvent -= ProcessSingleItem;
                item.SelectionChanged -= OnSelectionChanged;
            }
        }

        protected abstract void Update(IModelService modelService);
        protected abstract bool IsItemEnabled(T item, IModelService modelService);

        private void OnSelectionChanged()
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

        public void Dispose()
        {
            if (!_isDisposed)
            {
                _isDisposed = true;

                _eventAggregator.GetEvent<LevelLoadedEvent>()
                                .UnSubscribe(_levelLoadedToken);

                _eventAggregator.GetEvent<LevelUpdateEvent>()
                                .UnSubscribe(_levelUpdateToken);
            }
        }
    }
}
