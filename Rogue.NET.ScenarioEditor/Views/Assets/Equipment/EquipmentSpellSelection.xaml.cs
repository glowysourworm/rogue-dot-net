using Rogue.NET.ScenarioEditor.ViewModel;
using Rogue.NET.ScenarioEditor.Views.Controls;
using System;
using System.Windows.Controls;

namespace Rogue.NET.ScenarioEditor.Views.Assets.Equipment
{
    public partial class EquipmentSpellSelection : UserControl, IWizardPage
    {
        IWizardViewModel _containerViewModel;

        public EquipmentSpellSelection()
        {
            InitializeComponent();
        }

        public Type NextPage
        {
            get { return typeof(EquipmentParameters); }
        }

        public void Inject(IWizardViewModel containerViewModel, object model)
        {
            _containerViewModel = containerViewModel;

            this.DataContext = model;
        }
    }
}
