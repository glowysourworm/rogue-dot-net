using Rogue.NET.ScenarioEditor.Views.Controls;
using System;
using System.Windows.Controls;

namespace Rogue.NET.ScenarioEditor.Views.Assets.Spell
{
    public partial class Filler : UserControl, IWizardPage
    {
        public Filler()
        {
            InitializeComponent();
        }

        public Type NextPage
        {
            get { return typeof(SpellParameters); }
        }

        public void Inject(ViewModel.IWizardViewModel containerViewModel, object model)
        {
            
        }
    }
}
