using Rogue.NET.Scenario.Content.ViewModel.Content;

using System.ComponentModel.Composition;
using System.Windows.Controls;

namespace Rogue.NET.Scenario.Content.Views
{
    [PartCreationPolicy(CreationPolicy.Shared)]
    [Export]
    public partial class AlterationCtrl : UserControl
    {
        [ImportingConstructor]
        public AlterationCtrl(PlayerViewModel viewModel)
        {
            this.DataContext = viewModel;

            InitializeComponent();
        }
    }
}
