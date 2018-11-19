using Rogue.NET.Core.Service.Interface;
using Rogue.NET.ScenarioEditor.ViewModel;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Rogue.NET.ScenarioEditor.Views.Construction
{
    [Export]
    public partial class ScenarioObjectPlacement : UserControl
    {
        [ImportingConstructor]
        public ScenarioObjectPlacement()
        {
            InitializeComponent();
        }
    }
}
