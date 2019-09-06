using Rogue.NET.Common.Extension.Event;
using Rogue.NET.Common.Extension.Prism.EventAggregator;
using Rogue.NET.Core.Model.Scenario.Content.Item;
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

namespace Rogue.NET.Scenario.Content.ViewModel.ItemGrid
{
    /// <summary>
    /// View Model component for the equipment item grid. updates from the backend are
    /// subscribed to for updating individual items; and two constructors are provided for
    /// injection / manual use.
    /// </summary>
    public class EquipmentItemGridViewModelBase : ItemGridViewModel<EquipmentItemGridRowViewModel, Equipment>
    {
        public EquipmentItemGridViewModelBase(
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
        protected override IEnumerable<Equipment> GetCollection(IModelService modelService)
        {
            return modelService.Player
                               .Equipment
                               .Values
                               .OrderBy(x => x.Type.ToString())
                               .ThenBy(x => x.RogueName);
        }
        protected override Func<Equipment, EquipmentItemGridRowViewModel> GetItemConstructor(IModelService modelService)
        {
            return (item) =>
            {
                return new EquipmentItemGridRowViewModel(
                             item,
                             modelService.ScenarioEncyclopedia[item.RogueName],
                             modelService.GetDisplayName(item),
                             GetIsEnabled(modelService, item));
            };
        }
        protected override Action<Equipment, EquipmentItemGridRowViewModel> GetItemUpdater(IModelService modelService)
        {
            return (item, viewModel) =>
            {
                viewModel.Update(item,
                                modelService.ScenarioEncyclopedia[item.RogueName],
                                modelService.GetDisplayName(item),
                                GetIsEnabled(modelService, item));
            };
        }
        protected override Task ProcessSingleItemNonDialog(IRogueEventAggregator eventAggregator, string itemId)
        {
            throw new Exception("Should be overridden in a child class");
        }

        protected override bool GetIsEnabled(IModelService modelService, Equipment item)
        {
            return true;
        }
    }
}
