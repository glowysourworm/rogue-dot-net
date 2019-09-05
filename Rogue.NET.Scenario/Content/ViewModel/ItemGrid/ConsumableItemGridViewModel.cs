using Rogue.NET.Common.Extension;
using Rogue.NET.Common.Extension.Prism.EventAggregator;
using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.ScenarioConfiguration.Alteration.Extension;
using Rogue.NET.Core.Service.Interface;
using Rogue.NET.Scenario.Content.ViewModel.ItemGrid.Enum;
using System;
using System.ComponentModel.Composition;
using System.Linq;

using Consumable = Rogue.NET.Core.Model.Scenario.Content.Item.Consumable;
using Equipment = Rogue.NET.Core.Model.Scenario.Content.Item.Equipment;

namespace Rogue.NET.Scenario.Content.ViewModel.ItemGrid
{
    /// <summary>
    /// View Model component for the consumables item grid. updates from the backend are
    /// subscribed to for updating individual items; and two constructors are provided for
    /// injection / manual use.
    /// </summary>
    [PartCreationPolicy(CreationPolicy.NonShared)]
    [Export]
    public class ConsumableItemGridViewModel : ItemGridViewModel<Consumable>
    {
        [ImportingConstructor]
        public ConsumableItemGridViewModel(
            IRogueEventAggregator eventAggregator,
            IModelService modelService) : base(eventAggregator, modelService)
        {
            // Initializing Intended Action Here
            //
            // NOTE*** This is for the primary use of this view model - which is to
            //         provide data binding for the primary consumables grid.
            this.IntendedAction = ItemGridIntendedAction.Consume;
            this.SelectionMode = ItemGridSelectionMode.Single;
        }

        protected override bool IsItemEnabled(Consumable item, IModelService modelService)
        {
            var inventory = modelService.Player.Inventory.Values;

            // Make sure identifyConsumable applies to consume an identify item (there's at least one thing to identify)
            var isIdentify = item.HasAlteration && item.Alteration.Effect.IsIdentify();

            // Checks to see whether there are any items to identify in the player's inventory
            var canUseIdentify = inventory.Any(x => (!x.IsIdentified && x is Equipment) ||
                                                     !modelService.ScenarioEncyclopedia[x.RogueName].IsIdentified);

            switch (this.IntendedAction)
            {
                // Level and Character Class:  Handled by other code. 
                //                             Allow user to TRY to consume for those so that they see the message
                //
                // Notes:                      Always allow
                //
                // Identify:                   Must have unidentified item
                //
                case ItemGridIntendedAction.Consume:
                    return (item.HasAlteration && item.SubType != ConsumableSubType.Ammo && !isIdentify) ||
                           (item.HasLearnedSkillSet) ||
                           (item.SubType == ConsumableSubType.Note) || 
                           (isIdentify && canUseIdentify);

                // Always allow dropping of consumables (no curse to bother with)
                case ItemGridIntendedAction.Drop:
                    return true;
                
                // Only if item is not yet identified:  Rogue-Encyclopedia check only (no specific properties)
                case ItemGridIntendedAction.Identify:
                    return !modelService.ScenarioEncyclopedia[item.RogueName].IsIdentified;

                // FOR NOW: Allow only for throw alteration. (This will be changed) :)
                case ItemGridIntendedAction.Throw:
                    return item.HasProjectileAlteration;

                case ItemGridIntendedAction.Transmute:
                    return !modelService.ScenarioEncyclopedia[item.RogueName].IsObjective;

                default:
                    throw new Exception("Unhandled Consumable Item Grid View Model Intended Action");
            }
        }

        protected override void Update(IModelService modelService)
        {
            // Filter out items for identify function
            var consumables = this.IntendedAction == ItemGridIntendedAction.Identify ? 
                                          modelService.Player.Consumables.Values.Where(x => !modelService.ScenarioEncyclopedia[x.RogueName].IsIdentified)
                                        : modelService.Player.Consumables.Values;

            // Call Unhook to remove item events before synchronizing
            UnHookItems();

            // Consumables
            this.Items.SynchronizeFrom(

                // Source (Select first of each group by name)
                consumables.GroupBy(x => x.RogueName)
                           .Select(x => x.First())
                           .OrderBy(x => x.SubType.ToString())
                           .ThenBy(x => x.RogueName),

                // Comparer
                (model, viewModel) => model.Id == viewModel.Id,

                // Constructor
                consumable => new ConsumableItemGridRowViewModel(
                                    consumable, 
                                    modelService.ScenarioEncyclopedia[consumable.RogueName],
                                    modelService.GetDisplayName(consumable), 
                                    consumables.Gather(consumable, x => x.RogueName),
                                    IsItemEnabled(consumable, modelService)),

                // Update
                (consumable, viewModel) =>
                {
                    viewModel.Update(consumable,
                                     modelService.ScenarioEncyclopedia[consumable.RogueName],
                                     modelService.GetDisplayName(consumable), 
                                     consumables.Gather(consumable, x => x.RogueName),
                                     IsItemEnabled(consumable, modelService));
                });

            // Call Hook to add listener to events from child items
            HookItems();
        }
    }
}
