using Rogue.NET.Model;
using Rogue.NET.ScenarioEditor.ViewModel;
using Rogue.NET.ScenarioEditor.Views.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

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
                case Common.ConsumableType.MultipleUses:
                    this.MultipleUsesRB.IsChecked = true;
                    break;
                case Common.ConsumableType.OneUse:
                    this.OneUseRB.IsChecked = true;
                    break;
                case Common.ConsumableType.UnlimitedUses:
                    this.UnlimitedUsesRB.IsChecked = true;
                    break;
            }
        }

        private void RB_Click(object sender, RoutedEventArgs e)
        {
            var model = this.DataContext as ConsumableTemplate;
            var radioButton = sender as RadioButton;
            model.SubType = (Common.ConsumableSubType)radioButton.Tag;
        }
    }
}
