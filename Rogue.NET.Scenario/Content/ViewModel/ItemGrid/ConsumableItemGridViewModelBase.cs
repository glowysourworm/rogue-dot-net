using Rogue.NET.Common.Extension;
using Rogue.NET.Common.Extension.Event;
using Rogue.NET.Common.Extension.Prism.EventAggregator;
using Rogue.NET.Core.Service.Interface;
using Rogue.NET.Scenario.Content.ViewModel.ItemGrid.Enum;
using Rogue.NET.Scenario.Content.ViewModel.ItemGrid.ItemGridRow;
using Rogue.NET.Scenario.Events.Content.PlayerSubpanel;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;

using Consumable = Rogue.NET.Core.Model.Scenario.Content.Item.Consumable;

namespace Rogue.NET.Scenario.Content.ViewModel.ItemGrid
{
    /// <summary>
    /// View Model component for the consumables item grid. updates from the backend are
    /// subscribed to for updating individual items; and two constructors are provided for
    /// injection / manual use.
    /// </summary>
    public class ConsumableItemGridViewModelBase : ItemGridViewModel<ConsumableItemGridRowViewModel, Consumable>
    {
        public ConsumableItemGridViewModelBase(
            IRogueEventAggregator eventAggregator,
            IModelService modelService) : base(eventAggregator, modelService)
        {
        }

        public override string Header
        {
            get { return "@(',')@"; }
        }
        public override Brush HeaderBrush
        {
            get { return Brushes.White; }
        }
        protected override IEnumerable<Consumable> GetCollection(IModelService modelService)
        {
            // Source (Select first of each group by name)
            return modelService.Player
                               .Consumables
                               .Values
                               .GroupBy(x => x.RogueName)
                               .Select(x => x.First())
                               .OrderBy(x => x.SubType.ToString())
                               .ThenBy(x => x.RogueName);
        }
        protected override Func<Consumable, ConsumableItemGridRowViewModel> GetItemConstructor(IModelService modelService)
        {
            return (item) =>
            {
                var consumables = modelService.Player.Consumables.Values;

                return new ConsumableItemGridRowViewModel(
                                item,
                                modelService.ScenarioEncyclopedia[item.RogueName],
                                modelService.GetDisplayName(item),
                                consumables.Gather(item, x => x.RogueName),
                                GetIsEnabled(modelService, item));
            };
        }
        protected override Action<Consumable, ConsumableItemGridRowViewModel> GetItemUpdater(IModelService modelService)
        {
            return (item, viewModel) =>
            {
                var consumables = modelService.Player.Consumables.Values;

                viewModel.UpdateGrouped(item,
                                         modelService.ScenarioEncyclopedia[item.RogueName],
                                         modelService.GetDisplayName(item),
                                         consumables.Gather(item, x => x.RogueName),
                                         GetIsEnabled(modelService, item));
            };
        }
        protected override Task ProcessSingleItemNonDialog(IRogueEventAggregator eventAggregator, string itemId)
        {
            throw new Exception("Should be overridden in child class");
        }

        protected override bool GetIsEnabled(IModelService modelService, Consumable item)
        {
            return true;
        }
    }
}
