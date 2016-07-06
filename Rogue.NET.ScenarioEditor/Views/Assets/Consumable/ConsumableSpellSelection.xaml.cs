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
    /// <summary>
    /// Interaction logic for ConsumableSpellSelection.xaml
    /// </summary>
    public partial class ConsumableSpellSelection : UserControl, IWizardPage
    {
        IWizardViewModel _containerViewModel;

        public ConsumableSpellSelection()
        {
            InitializeComponent();
        }

        public Type NextPage
        {
            get { return typeof(ConsumableParameters); }
        }

        public void Inject(IWizardViewModel containerViewModel, object model)
        {
            _containerViewModel = containerViewModel;

            this.DataContext = model;
        }
    }
}
