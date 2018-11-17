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

namespace Rogue.NET.Scenario.Content.ViewModel.Content
{
    [Export(typeof(RogueEncyclopediaViewModel))]
    public class RogueEncyclopediaViewModel
    {
        public ObservableCollection<RogueEncyclopediaCategoryViewModel> Categories { get; set; }

        [ImportingConstructor]
        public RogueEncyclopediaViewModel(IEventAggregator eventAggregator, IModelService modelService, IScenarioResourceService scenarioResourceService)
        {
            this.Categories = new ObservableCollection<RogueEncyclopediaCategoryViewModel>();

            CreateCategories(scenarioResourceService);

            eventAggregator.GetEvent<LevelLoadedEvent>().Subscribe(() =>
            {
                UpdateOrAdd(modelService);

            }, ThreadOption.UIThread, true);
            eventAggregator.GetEvent<LevelUpdateEvent>().Subscribe(update =>
            {
                switch (update.LevelUpdateType)
                {
                    case LevelUpdateType.EncyclopediaCurseIdentify:
                    case LevelUpdateType.EncyclopediaIdentify:
                        UpdateOrAdd(modelService);
                        break;
                }

            }, ThreadOption.UIThread, true);
        }

        private void CreateCategories(IScenarioResourceService scenarioResourceService)
        {
            var constructor = new Func<ImageResources, string, string, RogueEncyclopediaCategoryViewModel>((imageResource, categoryName, displayName) =>
            {
                return new RogueEncyclopediaCategoryViewModel()
                {
                    Source = scenarioResourceService.GetImage(imageResource),
                    Height = ModelConstants.CELLHEIGHT * 2,
                    Width = ModelConstants.CELLWIDTH * 2,
                    DisplayName = displayName,
                    CategoryName = categoryName
                };
            });

            this.Categories.AddRange(new RogueEncyclopediaCategoryViewModel[]
            {
                constructor(ImageResources.PotionGreen, ConsumableSubType.Potion.ToString(), "Potion"),
                constructor(ImageResources.ScrollBlue, ConsumableSubType.Scroll.ToString(), "Scroll"),
                constructor(ImageResources.FoodRed, ConsumableSubType.Food.ToString(), "Food"),
                constructor(ImageResources.ManualFuschia, ConsumableSubType.Manual.ToString(),"Manual"),
                constructor(ImageResources.WandWhite, ConsumableSubType.Wand.ToString(),"Wand"),
                constructor(ImageResources.ArrowBlack, ConsumableSubType.Ammo.ToString(),"Ammunition"),
                constructor(ImageResources.MirrorRainbow, ConsumableSubType.Misc.ToString(),"Miscellaneous"),
                constructor(ImageResources.ShortSwordBlack, EquipmentType.OneHandedMeleeWeapon.ToString(), "One Handed Weapon"),
                constructor(ImageResources.SwordWhite, EquipmentType.TwoHandedMeleeWeapon.ToString(),  "Two Handed Weapon"),
                constructor(ImageResources.SwordWhite, EquipmentType.RangeWeapon.ToString(),  "Range Weapon"),
                constructor(ImageResources.ArmorBlue, EquipmentType.Armor.ToString(), "Armor"),
                constructor(ImageResources.ShieldBlack, EquipmentType.Shield.ToString(), "Shield"),
                constructor(ImageResources.HelmetBlack, EquipmentType.Helmet.ToString(), "Helmet"),
                constructor(ImageResources.GauntletsBlack, EquipmentType.Gauntlets.ToString(), "Gauntlets"),
                constructor(ImageResources.BeltBlack, EquipmentType.Belt.ToString(), "Belt"),
                constructor(ImageResources.ShoulderBlack, EquipmentType.Shoulder.ToString(), "Shoulder"),
                constructor(ImageResources.BootsBlack, EquipmentType.Boots.ToString(), "Boots"),
                constructor(ImageResources.AmuletBlue, EquipmentType.Amulet.ToString(), "Amulet"),
                constructor(ImageResources.OrbRainbow, EquipmentType.Orb.ToString(), "Orb"),
                constructor(ImageResources.RingGreen, EquipmentType.Ring.ToString(), "Ring"),
                constructor(ImageResources.SavePoint, DoodadType.Normal.ToString(), "Normal Scenario Objects"),
                constructor(ImageResources.WellRed, DoodadType.Magic.ToString(), "Magic Scenario Objects"),
                new RogueEncyclopediaCategoryViewModel()
                {
                    Source = scenarioResourceService.GetImage("T", "#FFFFFFFF"),
                    Height = ModelConstants.CELLHEIGHT * 2,
                    Width = ModelConstants.CELLWIDTH * 2,
                    DisplayName = "Enemy",
                    CategoryName = "Enemy"
                },
                constructor(ImageResources.Skill1, "Skill", "Skill")
            });
        }

        private void UpdateOrAdd(IModelService modelService)
        {
            foreach (var metaData in modelService.ScenarioEncyclopedia.Values)
            {
                // To locate the ScenarioMetaDataViewModel -> find category by Type
                var category = this.Categories.FirstOrDefault(x => x.CategoryName == metaData.Type);

                if (category == null)
                    throw new Exception("Unknown Scenario MetaData Type");

                var item = category.Items.FirstOrDefault(x => x.RogueName == metaData.RogueName);

                // Add
                if (item == null)
                    category.Items.Add(new ScenarioMetaDataViewModel(metaData));

                // Update
                else
                    item.Update(metaData);
            }
        }
    }
}
