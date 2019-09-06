using Rogue.NET.Common.Extension.Prism.EventAggregator;
using Rogue.NET.Core.Service.Interface;
using System.ComponentModel.Composition;
using System.Windows.Media;

using Consumable = Rogue.NET.Core.Model.Scenario.Content.Item.Consumable;

namespace Rogue.NET.Scenario.Content.ViewModel.ItemGrid.DialogMode
{
    /// <summary>
    /// View Model component for the consumables item grid. updates from the backend are
    /// subscribed to for updating individual items; and two constructors are provided for
    /// injection / manual use.
    /// </summary>
    [PartCreationPolicy(CreationPolicy.NonShared)]
    [Export]
    public class ConsumableIdentifyItemGridViewModel : ConsumableItemGridViewModelBase
    {
        [ImportingConstructor]
        public ConsumableIdentifyItemGridViewModel(
            IRogueEventAggregator eventAggregator,
            IModelService modelService) : base(eventAggregator, modelService)
        {
        }

        public override string Header
        {
            get { return "Identify"; }
        }
        public override Brush HeaderBrush
        {
            get { return Brushes.Yellow; }
        }

        protected override bool GetIsEnabled(IModelService modelService, Consumable item)
        {
            return !modelService.ScenarioEncyclopedia[item.RogueName].IsIdentified;
        }
    }
}
