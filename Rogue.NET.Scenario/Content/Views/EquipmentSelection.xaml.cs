using System.ComponentModel.Composition;
using System.Windows.Controls;
using Rogue.NET.Scenario.Content.ViewModel.Content;

namespace Rogue.NET.Scenario.Views
{
    [PartCreationPolicy(CreationPolicy.Shared)]
    [Export]
	public partial class EquipmentSelectionCtrl : UserControl
	{

        [ImportingConstructor]
		public EquipmentSelectionCtrl(PlayerViewModel playerViewModel)
		{
            this.DataContext = playerViewModel;

			this.InitializeComponent();
		}
    }
}