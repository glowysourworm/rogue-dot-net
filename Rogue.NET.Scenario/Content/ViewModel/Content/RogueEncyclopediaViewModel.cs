using Prism.Events;
using Rogue.NET.Core.Service.Interface;
using Rogue.NET.Model.Events;
using System.Linq;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model;
using System;
using Rogue.NET.Core.Event.Scenario.Level.Event;
using Rogue.NET.Core.Logic.Processing.Enum;
using Rogue.NET.Common.ViewModel;
using System.Collections.Generic;
using Rogue.NET.Core.Model.Scenario;

namespace Rogue.NET.Scenario.Content.ViewModel.Content
{
    [PartCreationPolicy(CreationPolicy.Shared)]
    [Export(typeof(RogueEncyclopediaViewModel))]
    public class RogueEncyclopediaViewModel : NotifyViewModel
    {
        public ObservableCollection<RogueEncyclopediaCategoryViewModel> Categories { get; set; }

        const string OBJECTIVE_NAME = "Objective";

        [ImportingConstructor]
        public RogueEncyclopediaViewModel(IEventAggregator eventAggregator, IModelService modelService, IScenarioResourceService scenarioResourceService)
        {
            this.Categories = new ObservableCollection<RogueEncyclopediaCategoryViewModel>();

            CreateCategories(scenarioResourceService);

            eventAggregator.GetEvent<LevelLoadedEvent>().Subscribe(() =>
            {
                UpdateOrAdd(modelService, scenarioResourceService);

            });
            eventAggregator.GetEvent<LevelUpdateEvent>().Subscribe(update =>
            {
                switch (update.LevelUpdateType)
                {
                    case LevelUpdateType.EncyclopediaCurseIdentify:
                    case LevelUpdateType.EncyclopediaIdentify:
                    case LevelUpdateType.PlayerSkillSetAdd:
                        UpdateOrAdd(modelService, scenarioResourceService);
                        break;
                }

            });
        }

        private void CreateCategories(IScenarioResourceService scenarioResourceService)
        {
            var constructor = new Func<string, string, string, RogueEncyclopediaCategoryViewModel>((categoryName, categoryDisplayName, categoryDescription) =>
            {
                return new RogueEncyclopediaCategoryViewModel(scenarioResourceService)
                {
                    Height = ModelConstants.CellHeight * 2,
                    Width = ModelConstants.CellWidth * 2,
                    CategoryDescription = categoryDescription,
                    CategoryName = categoryName,
                    CategoryDisplayName = categoryDisplayName
                };
            });

            this.Categories.AddRange(new RogueEncyclopediaCategoryViewModel[]
            {
                // List of ALL CATEGORIES
                constructor(ConsumableSubType.Potion.ToString(), "Potion", "Items that you can drink"),
                constructor(ConsumableSubType.Scroll.ToString(), "Scroll", "Items that you can read"),
                constructor(ConsumableSubType.Food.ToString(), "Food", "Consumable items that asuage hunger"),
                constructor(ConsumableSubType.Manual.ToString(),"Manual", "Items that can teach new skills"),
                constructor(ConsumableSubType.Wand.ToString(),"Wand", "Items that channel magic"),
                constructor(ConsumableSubType.Ammo.ToString(),"Ammunition", "Ammunition for range weapons"),
                constructor(ConsumableSubType.Misc.ToString(),"Miscellaneous", "Miscellaneous items"),
                constructor(ConsumableSubType.Note.ToString(),"Notes", "Items that you can read"),
                constructor(EquipmentType.OneHandedMeleeWeapon.ToString(), "One Handed Weapon", "Weapons for one hand (may equip two at once)"),
                constructor(EquipmentType.TwoHandedMeleeWeapon.ToString(),  "Two Handed Weapon", "Weapons for two hands"),
                constructor(EquipmentType.RangeWeapon.ToString(),  "Range Weapon", "Range weapons (require ammunition)"),
                constructor(EquipmentType.Armor.ToString(), "Armor", "Equipment to protect your body"),
                constructor(EquipmentType.Shield.ToString(), "Shield", "Defensive equipment to be used with one hand"),
                constructor(EquipmentType.Helmet.ToString(), "Helmet", "Defensive equipment for your head"),
                constructor(EquipmentType.Gauntlets.ToString(), "Gauntlets", "Defensive equipment for your hands"),
                constructor(EquipmentType.Belt.ToString(), "Belt", "Equipment to be worn on your waist"),
                constructor(EquipmentType.Shoulder.ToString(), "Shoulder", "Equipment to protect your shoulders"),
                constructor(EquipmentType.Boots.ToString(), "Boots", "Equipment to protect your feet"),
                constructor(EquipmentType.Amulet.ToString(), "Amulet", "Equipment that channels magic"),
                constructor(EquipmentType.Orb.ToString(), "Orb", "Equipment that channels magic"),
                constructor(EquipmentType.Ring.ToString(), "Ring", "Equipment that channels magic"),
                constructor(DoodadType.Normal.ToString(), "Normal Scenario Objects", "Objects that aid you on your way"),
                constructor(DoodadType.Magic.ToString(), "Magic Scenario Objects", "Objects that have special effects"),
                constructor("Enemy", "Enemy", "Characters that will likely try and hurt you"),
                constructor("Skill", "Skill", "Skill sets that you can learn"),
                constructor(OBJECTIVE_NAME, OBJECTIVE_NAME, "Enemies, Items, or Objects that are your mission objective")
            });
        }

        private void UpdateOrAdd(IModelService modelService, IScenarioResourceService scenarioResourceService)
        {
            // To locate the ScenarioMetaDataViewModel -> find category by Type
            foreach (var metaData in modelService.ScenarioEncyclopedia.Values)
            {
                // Update base category
                UpdateCategory(this.Categories.FirstOrDefault(x => x.CategoryName == metaData.Type), metaData, scenarioResourceService);

                // Update the objective category
                if (metaData.IsObjective)
                    UpdateCategory(this.Categories.First(x => x.CategoryName == OBJECTIVE_NAME), metaData, scenarioResourceService);
            }

            OnPropertyChanged("Categories");
        }

        private void UpdateCategory(RogueEncyclopediaCategoryViewModel category, ScenarioMetaData metaData, IScenarioResourceService scenarioResourceService)
        {
            if (category == null)
                throw new Exception("Unknown Scenario MetaData Type");

            var item = category.Items.FirstOrDefault(x => x.RogueName == metaData.RogueName);

            // Add
            if (item == null)
                category.Items.Add(new ScenarioMetaDataViewModel(metaData, scenarioResourceService));

            // Update
            else
                item.Update(metaData, scenarioResourceService);

            // Invalidate calculated category properties (percent complete / image source)
            category.Invalidate();
        }
    }
}
