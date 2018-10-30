using Rogue.NET.Core.Model.ScenarioConfiguration.Content;
using Rogue.NET.ScenarioEditor.ViewModel;
using Rogue.NET.ScenarioEditor.Views.Controls;
using System;
using System.Windows;
using System.Windows.Controls;

using ConsumableTypeEnum = Rogue.NET.Core.Model.Enums.ConsumableType;
using ConsumableSubTypeEnum = Rogue.NET.Core.Model.Enums.ConsumableSubType;

namespace Rogue.NET.ScenarioEditor.Views.Assets.Consumable
{
    public partial class ConsumableType : UserControl, IWizardPage
    {
        IWizardViewModel _containerViewModel;

        public ConsumableType()
        {
            InitializeComponent();
        }

        public Type NextPage
        {
            get { return typeof(ConsumableSpellSelection); }
        }

        public void Inject(IWizardViewModel containerViewModel, object model)
        {
            _containerViewModel = containerViewModel;

            this.DataContext = model;

            var consumable = model as ConsumableTemplate;

            if (consumable == null)
                return;

            switch (consumable.Type)
            {
                case ConsumableTypeEnum.MultipleUses:
                    this.MultipleUsesRB.IsChecked = true;
                    break;
                case ConsumableTypeEnum.OneUse:
                    this.OneUseRB.IsChecked = true;
                    break;
                case ConsumableTypeEnum.UnlimitedUses:
                    this.UnlimitedUsesRB.IsChecked = true;
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
