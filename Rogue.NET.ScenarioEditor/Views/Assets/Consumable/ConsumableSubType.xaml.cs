using Rogue.NET.Core.Model.ScenarioConfiguration.Content;
using Rogue.NET.ScenarioEditor.ViewModel;
using Rogue.NET.ScenarioEditor.Views.Controls;
using System;
using System.Windows;
using System.Windows.Controls;

using ConsumableSubTypeEnum = Rogue.NET.Core.Model.Enums.ConsumableSubType;

namespace Rogue.NET.ScenarioEditor.Views.Assets.Consumable
{
    public partial class ConsumableSubType : UserControl, IWizardPage
    {
        IWizardViewModel _containerViewModel;

        public ConsumableSubType()
        {
            InitializeComponent();
        }

        public Type NextPage
        {
            get { return typeof(ConsumableType); }
        }

        public void Inject(IWizardViewModel containerViewModel, object model)
        {
            _containerViewModel = containerViewModel;

            this.DataContext = model;

            var consumable = model as ConsumableTemplate;

            if (consumable == null)
                return;

            switch (consumable.SubType)
            {
                case ConsumableSubTypeEnum.Ammo:
                    this.AmmoRB.IsChecked = true;
                    break;
                case ConsumableSubTypeEnum.Food:
                    this.FoodRB.IsChecked = true;
                    break;
                case ConsumableSubTypeEnum.Manual:
                    this.ManualRB.IsChecked = true;
                    break;
                case ConsumableSubTypeEnum.Misc:
                    this.MiscRB.IsChecked = true;
                    break;
                case ConsumableSubTypeEnum.Potion:
                    this.PotionRB.IsChecked = true;
                    break;
                case ConsumableSubTypeEnum.Scroll:
                    this.ScrollRB.IsChecked = true;
                    break;
                case ConsumableSubTypeEnum.Wand:
                    this.WandRB.IsChecked = true;
                    break;
            }
        }

        private void RB_Click(object sender, RoutedEventArgs e)
        {
            var model = this.DataContext as ConsumableTemplate;
            var radioButton = sender as RadioButton;
            model.SubType = (ConsumableSubTypeEnum)radioButton.Tag;
        }
    }
}
