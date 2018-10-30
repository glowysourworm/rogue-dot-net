using Rogue.NET.ScenarioEditor.ViewModel;
using Rogue.NET.ScenarioEditor.Views.Controls;
using System;
using System.Windows.Controls;

namespace Rogue.NET.ScenarioEditor.Views.Assets.Equipment
{
    public partial class EquipmentSymbol : UserControl, IWizardPage
    {
        IWizardViewModel _containerViewModel;

        public EquipmentSymbol()
        {
            InitializeComponent();
        }

        public Type NextPage
        {
            get { return typeof(EquipmentSymbol); }
        }

        public void Inject(IWizardViewModel containerViewModel, object model)
        {
            _containerViewModel = containerViewModel;

            this.DataContext = model;
        }
    }
}
