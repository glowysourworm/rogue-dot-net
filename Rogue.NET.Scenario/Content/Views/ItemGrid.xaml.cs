using System;
using System.Windows;
using System.Windows.Controls;
using System.ComponentModel.Composition;

using Rogue.NET.Common.Events.Scenario;
using Rogue.NET.Common.EventArgs;
using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Scenario.ViewModel.ItemGrid;
using Rogue.NET.Scenario.Content.ViewModel.ItemGrid;

using Prism.Events;


namespace Rogue.NET.Scenario.Views
{
    [Export]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public partial class ItemGrid : UserControl
    {
        readonly IEventAggregator _eventAggregator;

        public static readonly DependencyProperty IntendedActionProperty = DependencyProperty.Register("IntendedAction", typeof(ItemGridActions), typeof(ItemGrid));

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
                        // Radio Buttons
                        this.Consume.Visibility = System.Windows.Visibility.Visible;
                        this.Throw.Visibility = System.Windows.Visibility.Visible;
                        this.Drop.Visibility = System.Windows.Visibility.Visible;
                        this.Enchant.Visibility = System.Windows.Visibility.Collapsed;
                        this.Uncurse.Visibility = System.Windows.Visibility.Collapsed;
                        this.Equip.Visibility = System.Windows.Visibility.Collapsed;
                        this.Identify.Visibility = System.Windows.Visibility.Collapsed;

                        // Columns
                        this.ClassColumn.Visibility = System.Windows.Visibility.Collapsed;
                        this.WeightColumn.Visibility = System.Windows.Visibility.Collapsed;
                        break;
                    case ItemGridModes.Equipment:
                        this.Consume.Visibility = System.Windows.Visibility.Collapsed;
                        this.Throw.Visibility = System.Windows.Visibility.Collapsed;
                        this.Drop.Visibility = System.Windows.Visibility.Visible;
                        this.Enchant.Visibility = System.Windows.Visibility.Collapsed;
                        this.Uncurse.Visibility = System.Windows.Visibility.Collapsed;
                        this.Equip.Visibility = System.Windows.Visibility.Visible;
                        this.Identify.Visibility = System.Windows.Visibility.Collapsed;

                        // Columns
                        this.ClassColumn.Visibility = System.Windows.Visibility.Visible;
                        this.QuantityColumn.Visibility = System.Windows.Visibility.Collapsed;
                        this.WeightColumn.Visibility = System.Windows.Visibility.Collapsed;
                        break;
                    case ItemGridModes.Inventory:
                        this.Consume.Visibility = System.Windows.Visibility.Collapsed;
                        this.Throw.Visibility = System.Windows.Visibility.Collapsed;
                        this.Drop.Visibility = System.Windows.Visibility.Visible;
                        this.Enchant.Visibility = System.Windows.Visibility.Collapsed;
                        this.Uncurse.Visibility = System.Windows.Visibility.Collapsed;
                        this.Equip.Visibility = System.Windows.Visibility.Collapsed;
                        this.Identify.Visibility = System.Windows.Visibility.Collapsed;

                        // Columns
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

                        // Columns
                        this.ClassColumn.Visibility = System.Windows.Visibility.Collapsed;
                        this.QuantityColumn.Visibility = System.Windows.Visibility.Collapsed;
                        this.WeightColumn.Visibility = System.Windows.Visibility.Collapsed;
                        break;
                    case ItemGridModes.Uncurse:
                        this.Consume.Visibility = System.Windows.Visibility.Collapsed;
                        this.Throw.Visibility = System.Windows.Visibility.Collapsed;
                        this.Drop.Visibility = System.Windows.Visibility.Collapsed;
                        this.Enchant.Visibility = System.Windows.Visibility.Collapsed;
                        this.Uncurse.Visibility = System.Windows.Visibility.Visible;
                        this.Equip.Visibility = System.Windows.Visibility.Collapsed;
                        this.Identify.Visibility = System.Windows.Visibility.Collapsed;

                        // Columns
                        this.ClassColumn.Visibility = System.Windows.Visibility.Visible;
                        this.QuantityColumn.Visibility = System.Windows.Visibility.Collapsed;
                        this.WeightColumn.Visibility = System.Windows.Visibility.Collapsed;
                        break;
                    case ItemGridModes.Enchant:
                        this.Consume.Visibility = System.Windows.Visibility.Collapsed;
                        this.Throw.Visibility = System.Windows.Visibility.Collapsed;
                        this.Drop.Visibility = System.Windows.Visibility.Collapsed;
                        this.Enchant.Visibility = System.Windows.Visibility.Visible;
                        this.Uncurse.Visibility = System.Windows.Visibility.Collapsed;
                        this.Equip.Visibility = System.Windows.Visibility.Collapsed;
                        this.Identify.Visibility = System.Windows.Visibility.Collapsed;

                        // Columns
                        this.ClassColumn.Visibility = System.Windows.Visibility.Visible;
                        this.QuantityColumn.Visibility = System.Windows.Visibility.Collapsed;
                        this.WeightColumn.Visibility = System.Windows.Visibility.Collapsed;
                        break;
                    case ItemGridModes.Imbue:
                        this.Consume.Visibility = System.Windows.Visibility.Collapsed;
                        this.Throw.Visibility = System.Windows.Visibility.Collapsed;
                        this.Drop.Visibility = System.Windows.Visibility.Collapsed;
                        this.Enchant.Visibility = System.Windows.Visibility.Visible;
                        this.Uncurse.Visibility = System.Windows.Visibility.Collapsed;
                        this.Equip.Visibility = System.Windows.Visibility.Collapsed;
                        this.Identify.Visibility = System.Windows.Visibility.Collapsed;

                        // Columns
                        this.ClassColumn.Visibility = System.Windows.Visibility.Visible;
                        this.QuantityColumn.Visibility = System.Windows.Visibility.Collapsed;
                        this.WeightColumn.Visibility = System.Windows.Visibility.Collapsed;
                        break;
                }
            }
        }
        public ItemGridActions IntendedAction
        {
            get { return (ItemGridActions)GetValue(IntendedActionProperty); }
            set { SetValue(IntendedActionProperty, value); }
        }
        public event EventHandler ActionSubmittedEvent;

        [ImportingConstructor]
        public ItemGrid(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;

            InitializeComponent();

            this.DataContextChanged += ItemGrid_DataContextChanged;
        }

        private void ItemGrid_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            InvalidateVisual();
        }

        private void SelectCheckBox_Click(object sender, RoutedEventArgs e)
        {
            var itemViewModel = (sender as Button).Tag as ItemGridRowViewModel;

            _eventAggregator.GetEvent<UserCommandEvent>()
                            .Publish(new LevelCommandEventArgs(
                                (LevelAction)Enum.Parse(typeof(LevelAction), 
                                this.IntendedAction.ToString()), Compass.Null, itemViewModel.Id));

            if (ActionSubmittedEvent != null)
                ActionSubmittedEvent(this, new EventArgs());
        }

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);

            this.TheGrid.Height = sizeInfo.NewSize.Height - 60;
        }
    }
}