using Rogue.NET.Common.Extension.Prism.EventAggregator;
using Rogue.NET.Core.Model.Scenario.Content.Item;
using Rogue.NET.Core.Service.Interface;
using System.ComponentModel.Composition;
using System.Windows.Media;

namespace Rogue.NET.Scenario.Content.ViewModel.ItemGrid.DialogMode
{
    /// <summary>
    /// View Model component for the equipment item grid. updates from the backend are
    /// subscribed to for updating individual items; and two constructors are provided for
    /// injection / manual use.
    /// </summary>
    [PartCreationPolicy(CreationPolicy.NonShared)]
    [Export]
    public class EquipmentTransmuteItemGridViewModel : EquipmentItemGridViewModelBase
    {
        [ImportingConstructor]
        public EquipmentTransmuteItemGridViewModel(
            IRogueEventAggregator eventAggregator,
            IModelService modelService) : base(eventAggregator, modelService)
        {
        }

        public override string Header
        {
            get { return "Transmute"; }
        }
        public override Brush HeaderBrush
        {
            get { return Brushes.Tan; }
        }
        protected override bool GetIsEnabled(IModelService modelService, Equipment item)
        {
            return !modelService.ScenarioEncyclopedia[item.RogueName].IsObjective;
        }
    }
}
