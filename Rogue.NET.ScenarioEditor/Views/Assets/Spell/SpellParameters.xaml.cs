using Rogue.NET.ScenarioEditor.ViewModel;
using Rogue.NET.ScenarioEditor.Views.Controls;
using System;
using System.Windows.Controls;

namespace Rogue.NET.ScenarioEditor.Views.Assets.Spell
{
    public partial class SpellParameters : UserControl, IWizardPage
    {
        public SpellParameters()
        {
            InitializeComponent();
        }

        public Type NextPage
        {
            get { return typeof(SpellParameters); }
        }

        public void Inject(IWizardViewModel containerViewModel, object model)
        {
            _containerViewModel = containerViewModel;

            this.DataContext = model;
        }

        IWizardViewModel _containerViewModel;
    }
}
