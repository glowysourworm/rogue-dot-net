using System;
using System.Windows;
using System.Windows.Controls;
using System.ComponentModel.Composition;

using Rogue.NET.Common.Events.Scenario;
using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Scenario.ViewModel.ItemGrid;
using Rogue.NET.Scenario.Content.ViewModel.ItemGrid;

using Prism.Events;
using System.Windows.Threading;

namespace Rogue.NET.Scenario.Content.Views
{
    [Export]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public partial class ItemGrid : UserControl
    {
        readonly IEventAggregator _eventAggregator;

        public static readonly DependencyProperty IntendedActionProperty = 
            DependencyProperty.Register("IntendedAction", typeof(ItemGridActions), typeof(ItemGrid));

        public static readonly DependencyProperty IsDialogModeProperty =
            DependencyProperty.Register("IsDialogMode", typeof(bool), typeof(ItemGrid));

        public static readonly DependencyProperty ShowDetailsProperty =
            DependencyProperty.Register("ShowDetails", typeof(bool), typeof(ItemGrid));

        // NOTE*** Using block to prevent multiple events for void -> async -> await. Should change
        //         to ICommand invoke instead.
        bool _executingTask = false;

        ItemGridModes _mode;

        public ItemGridModes Mode
        {
            get { return _mode; }
            set { SetMode(value); }
        }
        public ItemGridActions IntendedAction
        {
            get { return (ItemGridActions)GetValue(IntendedActionProperty); }
            set { SetValue(IntendedActionProperty, value); }
        }
        public bool IsDialogMode
        {
            get { return (bool)GetValue(IsDialogModeProperty); }
            set { SetValue(IsDialogModeProperty, value); }
        }
        public bool ShowDetails
        {
            get { return (bool)GetValue(ShowDetailsProperty); }
            set { SetValue(ShowDetailsProperty, value); }
        }

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

        private async void SelectCheckBox_Click(object sender, RoutedEventArgs e)
        {
            // Have to block here because this method doesn't return a Task to the calling
            // thread. So, can't wait for task to finish. This prevents two executions for the same item
            if (_executingTask)
                return;

            _executingTask = true;

            var itemViewModel = (sender as Button).Tag as ItemGridRowViewModel;

            await _eventAggregator.GetEvent<UserCommandEvent>()
                            .Publish(new UserCommandEventArgs()
                            {
                                Action = (LevelAction)Enum.Parse(typeof(LevelAction), this.IntendedAction.ToString()),
                                Direction = Compass.Null,
                                ItemId = itemViewModel.Id
                            });

            // ISSUE WITH VIEW UPDATING - NOT SURE WHY BUT VIEW MODEL FOR ITEM GRID NOT UP TO DATE WHEN 
            // FIRING THE NEXT EVENT! GOING TO TRY TO FORCE WAIT FOR APPLICATION IDLE TO ALLOW BINDING TO CATCH UP.

            this.Dispatcher.Invoke(() =>
            {
                _executingTask = false;

            }, DispatcherPriority.ApplicationIdle);
        }
        
        private void SetMode(ItemGridModes mode)
        {
            _mode = mode;

            this.Consume.Visibility = mode == ItemGridModes.Consumable ? Visibility.Visible : Visibility.Collapsed;
            this.Equip.Visibility = mode == ItemGridModes.Equipment ? Visibility.Visible : Visibility.Collapsed;
            this.Identify.Visibility = mode == ItemGridModes.Identify ? Visibility.Visible : Visibility.Collapsed;
            this.Uncurse.Visibility = mode == ItemGridModes.Uncurse ? Visibility.Visible : Visibility.Collapsed;
            this.EnchantArmor.Visibility = mode == ItemGridModes.EnchantArmor ? Visibility.Visible : Visibility.Collapsed;
            this.EnchantWeapon.Visibility = mode == ItemGridModes.EnchantWeapon ? Visibility.Visible : Visibility.Collapsed;
            this.ImbueArmor.Visibility = mode == ItemGridModes.ImbueArmor ? Visibility.Visible : Visibility.Collapsed;
            this.ImbueWeapon.Visibility = mode == ItemGridModes.ImbueWeapon ? Visibility.Visible : Visibility.Collapsed;
            this.Throw.Visibility = mode == ItemGridModes.Consumable ? Visibility.Visible : Visibility.Collapsed;
            this.Drop.Visibility = mode == ItemGridModes.Consumable ? Visibility.Visible :
                                   mode == ItemGridModes.Equipment ? Visibility.Visible :
                                   mode == ItemGridModes.Inventory ? Visibility.Visible : Visibility.Collapsed;

            this.ClassColumn.Visibility = mode == ItemGridModes.EnchantArmor ? Visibility.Visible :
                                          mode == ItemGridModes.EnchantWeapon ? Visibility.Visible :
                                          mode == ItemGridModes.Equipment ? Visibility.Visible :
                                          mode == ItemGridModes.ImbueArmor ? Visibility.Visible :
                                          mode == ItemGridModes.ImbueWeapon ? Visibility.Visible :
                                          mode == ItemGridModes.Uncurse ? Visibility.Visible : Visibility.Collapsed;

            this.QuantityColumn.Visibility = mode == ItemGridModes.Consumable ? Visibility.Visible : Visibility.Collapsed;
        }
    }
}