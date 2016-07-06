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
using Rogue.NET.Common;
using Rogue.NET.Scenario.Model;
using Rogue.NET.Model.Scenario;
using Dotway.WPF.Effects;
using Rogue.NET.Model;

namespace Rogue.NET.Scenario.Views
{
    public partial class ShopCtrl : UserControl
    {
        private const int AGILITY_BASE = 300;
        private const int STRENGTH_BASE = 200;
        private const int INTELLIGENCE_BASE = 150;
        private const int ELEMENTAL_BASE = 50;
        private const int HP_BASE = 20;
        private const int MP_BASE = 5;
        private const int EXP_BASE = 1;
        private const int HUNGER_BASE = 1;
        private const int HP_REGEN_BASE = 200;
        private const int MP_REGEN_BASE = 50;
        private const int HUNGER_PER_STEP = 150;
        private const int LIGHT_RADIUS_BASE = 100;
        private const int CLASS_BASE = 1000;
        private const int QUALITY_BASE = 500;

        public ShopCtrl()
        {
            InitializeComponent();
            this.DataContextChanged += new DependencyPropertyChangedEventHandler(ShopCtrl_DataContextChanged);
        }

        private void ShopCtrl_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            SetItems();
        }
        private void SetItems()
        {
            //ScenarioViewModel view = this.DataContext as ScenarioViewModel;
            //if (view == null)
            //    return;

            //this.PlayerItemGrid.SetItemsSource(view.Level.Player.Inventory);
            //this.ShopItemGrid.SetItemsSource(view.ShopItems);

            //this.YourTradeListBox.ItemsSource = view.Level.Player.Inventory.Where(z => z.IsMarkedForTrade);
            //this.ShopTradeListBox.ItemsSource = view.ShopItems.Where(z => z.IsMarkedForTrade);

            //this.PlayerItemGrid.RefreshMode();
            //this.ShopItemGrid.RefreshMode();
            CalculateBalance();
        }
        private void Reset()
        {
            //OnLevelActionRequest(new LevelCommandArgs(LevelAction.ClearTradeMarkedItems, Compass.Null, ""));

            SetItems();
            CalculateBalance();
        }
        private void CalculateBalance()
        {
            var view = this.DataContext as LevelData;
            if (view == null)
                return;

            int shopValue = view.ShopConsumables.Where(z => z.IsMarkedForTrade).Sum(z => z.ShopValue);
            int playerValue = view.Player.ConsumableInventory.Where(z => z.IsMarkedForTrade).Sum(z => z.ShopValue);

            int balance = playerValue - shopValue;

            if (balance >= 0)
            {
                this.TradeButton.Visibility = System.Windows.Visibility.Visible;
                this.YourGroupBox.BorderBrush = Brushes.Green;
                this.ShopGroupBox.BorderBrush = Brushes.Green;
            }
            else
            {
                this.TradeButton.Visibility = System.Windows.Visibility.Hidden;
                this.YourGroupBox.BorderBrush = Brushes.Red;
                this.ShopGroupBox.BorderBrush = Brushes.Red;
            }
        }
        private int CalculateItemValue(Item i)
        {
            return 0;
        }
        private void TradeButton_Click(object sender, RoutedEventArgs e)
        {
            //OnLevelActionRequest(new LevelCommandArgs(LevelAction.Trade, Compass.Null, ""));

            Reset();
        }
        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            Reset();
        }
        private void ShopItemGrid_Rogue2ItemEvent(object sender, LevelCommandArgs e)
        {
            //OnLevelActionRequest(e);
            if (e.Action == LevelAction.ToggleMarkForTrade)
                SetItems();
        }
        private void PlayerItemGrid_Rogue2ItemEvent(object sender, LevelCommandArgs e)
        {
            //OnLevelActionRequest(e);
            if (e.Action == LevelAction.ToggleMarkForTrade)
                SetItems();
        }
        /*
        #region IRogue2GameDisplay
        public event EventHandler<ModeChangeEventArgs> ModeChangeRequest;
        public event EventHandler<LevelCommandArgs> LevelActionRequest;
        public event EventHandler<LevelMessageEventArgs> LevelMessageRequest;
        protected void OnModeChangeRequest(GameCtrl.DisplayMode mode)
        {
            if (ModeChangeRequest != null)
                ModeChangeRequest(this, new ModeChangeEventArgs(mode));
        }
        protected void OnLevelActionRequest(LevelCommandArgs e)
        {
            if (LevelActionRequest != null)
                LevelActionRequest(this, e);
        }
        protected void OnLevelMessageRequest(string msg)
        {
            if (LevelMessageRequest != null)
                LevelMessageRequest(this, new LevelMessageEventArgs(msg));
        }
        public string DisplayName
        {
            get { return "Mysterious Shop..."; }
        }
        public ImageSource DisplayImageSource
        {
            get { return Rogue2ResourceManager.GetImage(ImageResources.Shop); }
        }
        public GameCtrl.DisplayMode Mode
        {
            get { return GameCtrl.DisplayMode.Shop; }
        }
        #endregion
        */
    }
}
