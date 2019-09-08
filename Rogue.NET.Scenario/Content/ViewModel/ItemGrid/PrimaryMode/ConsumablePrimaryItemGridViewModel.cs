using Rogue.NET.Common.Extension.Prism.EventAggregator;
using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.ScenarioConfiguration.Alteration.Extension;
using Rogue.NET.Core.Processing.Command.Backend;
using Rogue.NET.Core.Processing.Command.Backend.CommandData;
using Rogue.NET.Core.Service.Interface;
using Rogue.NET.Scenario.Content.ViewModel.ItemGrid.Enum;
using System;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Media;

using Consumable = Rogue.NET.Core.Model.Scenario.Content.Item.Consumable;
using Equipment = Rogue.NET.Core.Model.Scenario.Content.Item.Equipment;

namespace Rogue.NET.Scenario.Content.ViewModel.ItemGrid.PrimaryMode
{
    /// <summary>
    /// View Model component for the consumables item grid. updates from the backend are
    /// subscribed to for updating individual items; and two constructors are provided for
    /// injection / manual use.
    /// </summary>
    [PartCreationPolicy(CreationPolicy.NonShared)]
    [Export]
    public class ConsumablePrimaryItemGridViewModel : ConsumableItemGridViewModelBase
    {
        ConsumablePrimaryMode _primaryMode;

        public ConsumablePrimaryMode PrimaryMode
        {
            get { return _primaryMode; }
            set { this.RaiseAndSetIfChanged(ref _primaryMode, value); Update(); }
        }

        [ImportingConstructor]
        public ConsumablePrimaryItemGridViewModel(
            IRogueEventAggregator eventAggregator,
            IModelService modelService) : base(eventAggregator, modelService)
        {
        }

        public override string Header
        {
            get
            {
                switch (this.PrimaryMode)
                {
                    case ConsumablePrimaryMode.Consume:
                        return "Consume";
                    case ConsumablePrimaryMode.Throw:
                        return "Throw";
                    case ConsumablePrimaryMode.Drop:
                        return "Drop";
                    default:
                        throw new Exception("Unhandled ConsumablePrimaryMode");
                }
            }
        }
        public override Brush HeaderBrush
        {
            get
            {
                switch (this.PrimaryMode)
                {
                    case ConsumablePrimaryMode.Consume:
                        return Brushes.Fuchsia;
                    case ConsumablePrimaryMode.Throw:
                        return Brushes.Orange;
                    case ConsumablePrimaryMode.Drop:
                        return Brushes.Red;
                    default:
                        throw new Exception("Unhandled ConsumablePrimaryMode");
                }
            }
        }
        protected override Task ProcessSingleItemNonDialog(IRogueEventAggregator eventAggregator, string itemId)
        {
            switch (this.PrimaryMode)
            {
                case ConsumablePrimaryMode.Consume:
                    return eventAggregator.GetEvent<LevelCommand>()
                                          .Publish(new LevelCommandData(LevelCommandType.Consume, Compass.Null, itemId));
                case ConsumablePrimaryMode.Throw:
                    return eventAggregator.GetEvent<LevelCommand>()
                                          .Publish(new LevelCommandData(LevelCommandType.Throw, Compass.Null, itemId));
                case ConsumablePrimaryMode.Drop:
                    return eventAggregator.GetEvent<LevelCommand>()
                                          .Publish(new LevelCommandData(LevelCommandType.Drop, Compass.Null, itemId));
                default:
                    throw new Exception("Unhandled ConsumablePrimaryMode");
            }
        }

        protected override bool GetIsEnabled(IModelService modelService, Consumable item)
        {
            var inventory = modelService.Player.Inventory.Values;

            // Make sure identifyConsumable applies to consume an identify item (there's at least one thing to identify)
            var isIdentify = item.HasAlteration && item.Alteration.Effect.IsIdentify();

            // Checks to see whether there are any items to identify in the player's inventory
            var canUseIdentify = inventory.Any(x => (!x.IsIdentified && x is Equipment) ||
                                                     !modelService.ScenarioEncyclopedia[x.RogueName].IsIdentified);

            switch (this.PrimaryMode)
            {
                case ConsumablePrimaryMode.Consume:
                    return (item.HasAlteration && item.SubType != ConsumableSubType.Ammo && !isIdentify) ||
                           (item.HasLearnedSkillSet) ||
                           (item.SubType == ConsumableSubType.Note) ||
                           (isIdentify && canUseIdentify);
                case ConsumablePrimaryMode.Throw:
                    return item.HasProjectileAlteration;
                case ConsumablePrimaryMode.Drop:
                    return true;
                default:
                    throw new Exception("Unhandled ConsumablePrimaryMode");
            }
        }

        // Create an update override with other variable notifications for 
        // primary mode changes
        protected override void Update()
        {
            base.Update();

            OnPropertyChanged(() => this.Header);
            OnPropertyChanged(() => this.HeaderBrush);
        }
    }
}
