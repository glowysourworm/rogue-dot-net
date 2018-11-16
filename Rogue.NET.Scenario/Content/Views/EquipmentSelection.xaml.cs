using System.ComponentModel.Composition;
using System.Windows;
using System.Windows.Controls;
using Rogue.NET.Core.Graveyard;
using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Service.Interface;
using Rogue.NET.Scenario.Content.ViewModel.Content;

namespace Rogue.NET.Scenario.Views
{
    [Export]
	public partial class EquipmentSelectionCtrl : UserControl
	{
        readonly IScenarioResourceService _resourceService;

        [ImportingConstructor]
		public EquipmentSelectionCtrl(PlayerViewModel playerViewModel)
		{
            this.DataContext = playerViewModel;

			this.InitializeComponent();
		}
    }
}