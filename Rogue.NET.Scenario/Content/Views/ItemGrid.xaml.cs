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
using Rogue.NET.Core.Model.Scenario.Alteration;
using System.Collections.Generic;
using Rogue.NET.Core.Utility;

namespace Rogue.NET.Scenario.Views
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

        /// <summary>
        /// REQUIRED*** This is a set of Attack Attributes for Imbue processing. This was NOT 
        ///             how I wanted this to work; but it had to be done either here - or a one-off
        ///             queue on the backend which I didn't want. This was safer... and it's basically
        ///             the last thing I needed to complete the full functionality so I said.. meh :)
        /// </summary>
        public IEnumerable<AttackAttribute> ImbueAttackAttributes { get; set; }

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
            var itemViewModel = (sender as Button).Tag as ItemGridRowViewModel;

            await _eventAggregator.GetEvent<UserCommandEvent>()
                            .Publish(new UserCommandEventArgs()
                            {
                                Action = (LevelAction)Enum.Parse(typeof(LevelAction), this.IntendedAction.ToString()),
                                Direction = Compass.Null,
                                ItemId = itemViewModel.Id
                            });
        }
        
        private void SetMode(ItemGridModes mode)
        {
            _mode = mode;

            // Title for the Item Grid
            this.ModeTB.Text = TextUtility.CamelCaseToTitleCase(mode.ToString());

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