using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Collections;
using System.Reflection;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using Rogue.NET.Common;
using Rogue.NET.Common.Collections;
using Rogue.NET.Model.Scenario;
using Microsoft.Practices.Prism.Events;
using Rogue.NET.Scenario.ViewModel;
using Rogue.NET.Model;
using Rogue.NET.Common.Events.Scenario;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Prism.PubSubEvents;

namespace Rogue.NET.Scenario.Views
{
    public partial class ItemGrid : UserControl
    {
        IEventAggregator _eventAggregator;

        /// <summary>
        /// Kludge: Injection isn't working from splash windows - so exposed the e.a. here
        /// </summary>
        public IEventAggregator EventAggregator
        {
            get { return _eventAggregator; }
            set { _eventAggregator = value; }
        }

        ItemGridModes _mode = ItemGridModes.Consumable;
        public ItemGridModes Mode
        {
            get { return _mode; }
            set
            {
                _mode = value;
                switch (_mode)
                {
                    case ItemGridModes.Consumable:
                        this.Consume.Visibility = System.Windows.Visibility.Visible;
                        this.Throw.Visibility = System.Windows.Visibility.Visible;
                        this.Drop.Visibility = System.Windows.Visibility.Visible;
                        this.Enchant.Visibility = System.Windows.Visibility.Collapsed;
                        this.Uncurse.Visibility = System.Windows.Visibility.Collapsed;
                        this.Equip.Visibility = System.Windows.Visibility.Collapsed;
                        this.Identify.Visibility = System.Windows.Visibility.Collapsed;
                        this.Trade.Visibility = System.Windows.Visibility.Collapsed;

                        this.ClassColumn.Visibility = System.Windows.Visibility.Collapsed;
                        break;
                    case ItemGridModes.Equipment:
                        this.Consume.Visibility = System.Windows.Visibility.Collapsed;
                        this.Throw.Visibility = System.Windows.Visibility.Collapsed;
                        this.Drop.Visibility = System.Windows.Visibility.Visible;
                        this.Enchant.Visibility = System.Windows.Visibility.Collapsed;
                        this.Uncurse.Visibility = System.Windows.Visibility.Collapsed;
                        this.Equip.Visibility = System.Windows.Visibility.Visible;
                        this.Identify.Visibility = System.Windows.Visibility.Collapsed;
                        this.Trade.Visibility = System.Windows.Visibility.Collapsed;
                        break;
                    case ItemGridModes.Inventory:
                        this.Consume.Visibility = System.Windows.Visibility.Collapsed;
                        this.Throw.Visibility = System.Windows.Visibility.Collapsed;
                        this.Drop.Visibility = System.Windows.Visibility.Visible;
                        this.Enchant.Visibility = System.Windows.Visibility.Collapsed;
                        this.Uncurse.Visibility = System.Windows.Visibility.Collapsed;
                        this.Equip.Visibility = System.Windows.Visibility.Collapsed;
                        this.Identify.Visibility = System.Windows.Visibility.Collapsed;
                        this.Trade.Visibility = System.Windows.Visibility.Collapsed;

                        this.ClassColumn.Visibility = System.Windows.Visibility.Collapsed;
                        this.QuantityColumn.Visibility = System.Windows.Visibility.Collapsed;
                        break;
                    case ItemGridModes.Identify:
                        this.Consume.Visibility = System.Windows.Visibility.Collapsed;
                        this.Throw.Visibility = System.Windows.Visibility.Collapsed;
                        this.Drop.Visibility = System.Windows.Visibility.Collapsed;
                        this.Enchant.Visibility = System.Windows.Visibility.Collapsed;
                        this.Uncurse.Visibility = System.Windows.Visibility.Collapsed;
                        this.Equip.Visibility = System.Windows.Visibility.Collapsed;
                        this.Identify.Visibility = System.Windows.Visibility.Visible;
                        this.Trade.Visibility = System.Windows.Visibility.Collapsed;

                        this.ClassColumn.Visibility = System.Windows.Visibility.Collapsed;
                        this.QuantityColumn.Visibility = System.Windows.Visibility.Collapsed;
                        break;
                    case ItemGridModes.Uncurse:
                        this.Consume.Visibility = System.Windows.Visibility.Collapsed;
                        this.Throw.Visibility = System.Windows.Visibility.Collapsed;
                        this.Drop.Visibility = System.Windows.Visibility.Collapsed;
                        this.Enchant.Visibility = System.Windows.Visibility.Collapsed;
                        this.Uncurse.Visibility = System.Windows.Visibility.Visible;
                        this.Equip.Visibility = System.Windows.Visibility.Collapsed;
                        this.Identify.Visibility = System.Windows.Visibility.Collapsed;
                        this.Trade.Visibility = System.Windows.Visibility.Collapsed;
                        break;
                    case ItemGridModes.Enchant:
                        this.Consume.Visibility = System.Windows.Visibility.Collapsed;
                        this.Throw.Visibility = System.Windows.Visibility.Collapsed;
                        this.Drop.Visibility = System.Windows.Visibility.Collapsed;
                        this.Enchant.Visibility = System.Windows.Visibility.Visible;
                        this.Uncurse.Visibility = System.Windows.Visibility.Collapsed;
                        this.Equip.Visibility = System.Windows.Visibility.Collapsed;
                        this.Identify.Visibility = System.Windows.Visibility.Collapsed;
                        this.Trade.Visibility = System.Windows.Visibility.Collapsed;
                        break;
                    case ItemGridModes.Imbue:
                        this.Consume.Visibility = System.Windows.Visibility.Collapsed;
                        this.Throw.Visibility = System.Windows.Visibility.Collapsed;
                        this.Drop.Visibility = System.Windows.Visibility.Collapsed;
                        this.Enchant.Visibility = System.Windows.Visibility.Visible;
                        this.Uncurse.Visibility = System.Windows.Visibility.Collapsed;
                        this.Equip.Visibility = System.Windows.Visibility.Collapsed;
                        this.Identify.Visibility = System.Windows.Visibility.Collapsed;
                        this.Trade.Visibility = System.Windows.Visibility.Collapsed;
                        break;
                }
            }
        }
        public LevelAction IntendedAction { get; set; }

        public event EventHandler ActionSubmittedEvent;

        public ItemGrid()
        {
            InitializeComponent();

            this.DataContextChanged += ItemGrid_DataContextChanged;
        }
        [InjectionConstructor]
        public ItemGrid(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;

            InitializeComponent();

            var view = CollectionViewSource.GetDefaultView(this.TheGrid.ItemsSource);
            if (view != null)
            {
                view.SortDescriptions.Add(new SortDescription("Type", ListSortDirection.Ascending));
                view.SortDescriptions.Add(new SortDescription("RogueName", ListSortDirection.Ascending));
                view.Refresh();
            }

            this.DataContextChanged += ItemGrid_DataContextChanged;
        }
        private void ItemGrid_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var levelData = this.DataContext as LevelData;
            if (levelData == null)
                return;


            levelData.Player.ConsumableInventory.CollectionAltered -= UpdateItemGrid;
            levelData.Player.EquipmentInventory.CollectionAltered -= UpdateItemGrid;

            levelData.Player.ConsumableInventory.CollectionAltered += UpdateItemGrid;
            levelData.Player.EquipmentInventory.CollectionAltered += UpdateItemGrid;
            
            SetupItemGrid();
        }

        private void UpdateItemGrid(object sender, CollectionAlteredEventArgs e)
        {
            if (e.Item is Consumable && this.Mode == ItemGridModes.Equipment)
                return;

            if (e.Item is Equipment && this.Mode == ItemGridModes.Consumable)
                return;

            OnUpdateItemGrid(e);
        }

        private void OnUpdateItemGrid(CollectionAlteredEventArgs e)
        {
            var levelData = this.DataContext as LevelData;
            if (levelData == null)
                return;

            var itemSource = this.TheGrid.ItemsSource as ObservableCollection<ItemGridViewModel>;
            switch (e.Action)
            {
                case CollectionAction.Add:
                    if (this.Mode == ItemGridModes.Consumable)
                    {
                        var existingItem = itemSource.FirstOrDefault(z => z.RogueSavedName == ((Item)e.Item).RogueName);
                        if (existingItem != null)
                            existingItem.UpdateItem((Item)e.Item, levelData.Encyclopedia, levelData.Player.ConsumableInventory);
                        else
                        {
                            itemSource.Add(new ItemGridViewModel((Item)e.Item, levelData.Encyclopedia, levelData.Player.ConsumableInventory));
                            SetItemButtonVisibility();
                        }
                    }
                    else
                    {
                        itemSource.Add(new ItemGridViewModel((Item)e.Item, levelData.Encyclopedia, levelData.Player.ConsumableInventory));
                        SetItemButtonVisibility();
                    }
                    break;
                case CollectionAction.Remove:
                    if (this.Mode == ItemGridModes.Consumable)
                    {
                        var existingItem = itemSource.FirstOrDefault(z => z.RogueSavedName == ((Item)e.Item).RogueName);
                        if (existingItem.Quantity == 1)
                            itemSource.Remove(existingItem);

                        else
                        {
                            var nextItem = levelData.Player.ConsumableInventory.Except(new Consumable[] { (Consumable)e.Item }).First(z => z.RogueName == ((Item)e.Item).RogueName);
                            existingItem.UpdateItem((Item)nextItem, levelData.Encyclopedia, levelData.Player.ConsumableInventory);
                        }
                    }
                    else
                    {
                        var existingItem = itemSource.FirstOrDefault(z => z.Id == ((Item)e.Item).Id);
                        itemSource.Remove(existingItem);
                    }
                    break;
                case CollectionAction.PropertyChanged:
                    {
                        var existingItem = this.Mode == ItemGridModes.Consumable ? 
                            itemSource.FirstOrDefault(z => z.RogueSavedName == ((Item)e.Item).RogueName) : 
                            itemSource.FirstOrDefault(z => z.Id == ((Item)e.Item).Id);

                        existingItem.UpdateItem((Item)e.Item, levelData.Encyclopedia, levelData.Player.ConsumableInventory);
                    }
                    break;
            }
        }

        private void SetupItemGrid()
        {
            var levelData = this.DataContext as LevelData;
            if (levelData == null)
                return;

            var items = this.Mode == ItemGridModes.Consumable ? levelData.Player.ConsumableInventory :
                            this.Mode == ItemGridModes.Equipment ? levelData.Player.EquipmentInventory :
                                levelData.Player.ConsumableInventory.Cast<Item>().Union(levelData.Player.EquipmentInventory);

            var comparer = this.Mode == ItemGridModes.Consumable ? (IEqualityComparer<ScenarioObject>) new ScenarioObjectNameComparer() : new ScenarioObjectIdComparer();

            // item source
            var itemSource = new ObservableCollection<ItemGridViewModel>();
            this.TheGrid.ItemsSource = itemSource;

            // sort description
            var view = CollectionViewSource.GetDefaultView(this.TheGrid.ItemsSource);
            if (view != null)
            {
                view.SortDescriptions.Add(new SortDescription("Type", ListSortDirection.Ascending));
                view.SortDescriptions.Add(new SortDescription("RogueName", ListSortDirection.Ascending));
            }

            // add items
            foreach (var item in items.Distinct<Item>(comparer))
                itemSource.Add(new ItemGridViewModel(item, levelData.Encyclopedia, levelData.Player.ConsumableInventory));



            SetItemButtonVisibility();
            InvalidateVisual();
        }

        private void SetItemButtonVisibility()
        {
            var items = this.TheGrid.ItemsSource as IEnumerable<ItemGridViewModel>;
            if (items != null)
            {
                foreach (var item in items)
                {
                    item.ShowButton = false;
                    item.ShowButton |= item.ConsumeEnable && this.Consume.IsChecked.Value;
                    item.ShowButton |= item.EquipEnable && this.Equip.IsChecked.Value;
                    item.ShowButton |= item.IdentifyEnable && this.Identify.IsChecked.Value;
                    item.ShowButton |= item.UncurseEnable && this.Uncurse.IsChecked.Value;
                    item.ShowButton |= item.EnchantEnable && this.Enchant.IsChecked.Value;
                    item.ShowButton |= this.Trade.IsChecked.Value;
                    item.ShowButton |= item.ThrowEnable && this.Throw.IsChecked.Value;
                    item.ShowButton |= item.DropEnable && this.Drop.IsChecked.Value;
                }

                this.ButtonColumn.Visibility = items.All(z => !z.ShowButton) ? Visibility.Collapsed : Visibility.Visible;
            }
        }

        private void SelectCheckBox_Click(object sender, RoutedEventArgs e)
        {
            var itemViewModel = (sender as Button).Tag as ItemGridViewModel;

            _eventAggregator.GetEvent<UserCommandEvent>().Publish(new UserCommandEvent()
            {
                LevelCommand = new LevelCommandArgs(this.IntendedAction, Compass.Null, itemViewModel.Id)
            });

            if (ActionSubmittedEvent != null)
                ActionSubmittedEvent(this, new EventArgs());
        }

        private void RadioButton_Click(object sender, RoutedEventArgs e)
        {
            var radioButton = sender as RadioButton;
            if (radioButton != null)
            {
                this.ModeTB.Text = radioButton.Name;
                this.ModeTB.Foreground = radioButton.BorderBrush;
                this.IntendedAction = (LevelAction)Enum.Parse(typeof(LevelAction), radioButton.Name);
            }

            SetItemButtonVisibility();
        }

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);

            this.TheGrid.Height = sizeInfo.NewSize.Height - 60;
        }
    }
    public enum ItemGridModes
    {
        Inventory,
        Consumable,
        Equipment,
        Identify,
        Uncurse,
        Enchant,
        Imbue
    }
}