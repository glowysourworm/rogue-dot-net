﻿using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model;
using Rogue.NET.Common.ViewModel;
using Rogue.NET.Common.Extension.Prism.EventAggregator;
using Rogue.NET.Core.Processing.Event.Backend;
using Rogue.NET.Core.GameRouter.GameEvent.Backend.Enum;
using Rogue.NET.Core.Processing.Service.Interface;
using Rogue.NET.Core.Processing.Event.Level;

using System;
using System.Linq;
using System.ComponentModel.Composition;

using ScenarioMetaDataClass = Rogue.NET.Core.Model.Scenario.Abstract.ScenarioMetaData;


namespace Rogue.NET.Scenario.Content.ViewModel.Content.ScenarioMetaData
{
    [PartCreationPolicy(CreationPolicy.Shared)]
    [Export(typeof(RogueEncyclopediaViewModel))]
    public class RogueEncyclopediaViewModel : NotifyViewModel
    {
        public PagedObservableCollection<RogueEncyclopediaCategoryViewModel> Categories { get; set; }

        const int PAGE_SIZE = 6;
        const string OBJECTIVE_NAME = "Objective";

        [ImportingConstructor]
        public RogueEncyclopediaViewModel(IRogueEventAggregator eventAggregator, IModelService modelService, IScenarioResourceService scenarioResourceService)
        {
            this.Categories = new PagedObservableCollection<RogueEncyclopediaCategoryViewModel>(PAGE_SIZE);

            // Reset event
            eventAggregator.GetEvent<LevelLoadedEvent>().Subscribe(() =>
            {
                // First clear all categories and empty pages
                this.Categories.Clear();

                // Have to force a reset here and filter categories that are empty
                CreateCategories(modelService, scenarioResourceService);

                // Initialize the categories
                UpdateOrAdd(modelService, scenarioResourceService);
            });

            // Update event
            eventAggregator.GetEvent<LevelEvent>().Subscribe(update =>
            {
                switch (update.LevelUpdateType)
                {
                    case LevelEventType.EncyclopediaCurseIdentify:
                    case LevelEventType.EncyclopediaIdentify:
                    case LevelEventType.PlayerSkillSetAdd:
                        UpdateOrAdd(modelService, scenarioResourceService);
                        break;
                }

            });
        }

        private void CreateCategories(IModelService modelService, IScenarioResourceService scenarioResourceService)
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
                constructor("Friendly", "Friendly", "Characters that will try and be your buddy"),
                constructor("Temporary Character", "Temporary Character", "Characters that are manifest in-game"),
                constructor("Skill", "Skill", "Skill sets that you can learn"),
                constructor(OBJECTIVE_NAME, OBJECTIVE_NAME, "Enemies, Items, or Objects that are your mission objective")

              // Filter all empty categories (always leave objective category)
            }.Where(x => modelService.ScenarioEncyclopedia.Contains(z => z.Type == x.CategoryName) || x.IsObjectiveCategory));
        }

        private void UpdateOrAdd(IModelService modelService, IScenarioResourceService scenarioResourceService)
        {
            // To locate the ScenarioMetaDataViewModel -> find category by Type
            modelService.ScenarioEncyclopedia.ModifyEach(metaData =>
            {
                // Update base category
                UpdateCategory(this.Categories.PagedFirstOrDefault(x => x.CategoryName == metaData.Type), metaData, scenarioResourceService);

                // Update the objective category
                if (metaData.IsObjective)
                    UpdateCategory(this.Categories.PagedFirst(x => x.CategoryName == OBJECTIVE_NAME), metaData, scenarioResourceService);
            });

            OnPropertyChanged("Categories");
        }

        private void UpdateCategory(RogueEncyclopediaCategoryViewModel category, ScenarioMetaDataClass metaData, IScenarioResourceService scenarioResourceService)
        {
            if (category == null)
                throw new Exception("Unknown Scenario MetaData Type");

            var item = category.Items.PagedFirstOrDefault(x => x.RogueName == metaData.RogueName);

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
