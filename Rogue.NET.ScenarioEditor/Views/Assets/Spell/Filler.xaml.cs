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
