using Rogue.NET.ScenarioEditor.ViewModel;
using Rogue.NET.ScenarioEditor.Views.Controls;
using System;
using System.Windows.Controls;

namespace Rogue.NET.ScenarioEditor.Views.Assets.Consumable
{
    public partial class ConsumableSymbol : UserControl, IWizardPage
    {
        IWizardViewModel _containerViewModel;

        public ConsumableSymbol()
        {
            InitializeComponent();
        }

        public Type NextPage
        {
            get { return typeof(ConsumableSymbol); }
        }

        public void Inject(IWizardViewModel containerViewModel, object model)
        {
            _containerViewModel = containerViewModel;

            this.DataContext = model;
        }
    }
}
