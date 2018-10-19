using Rogue.NET.Common;
using Rogue.NET.Model.Scenario;

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Practices.Prism.PubSubEvents;

namespace Rogue.NET.Model.Generation
{
    public interface IDungeonGenerator
    {
        ScenarioContainer CreateScenario(ScenarioConfiguration config, int seed, bool survivorMode);
        ScenarioContainer CreateDebugScenario(ScenarioConfiguration config);
    }
    public class ScenarioGenerator : GeneratorBase, IDungeonGenerator
    {
        readonly ScenarioLayoutGenerator _layoutGenerator;
        readonly ScenarioContentGenerator _contentGenerator;

        public ScenarioGenerator(
            IEventAggregator eventAggregator,
            ScenarioLayoutGenerator layoutGenerator,
            ScenarioContentGenerator contentGenerator)
            : base(eventAggregator)
        {
            _layoutGenerator = layoutGenerator;
            _contentGenerator = contentGenerator;
        }

        public ScenarioContainer CreateScenario(ScenarioConfiguration c, int seed, bool survivorMode)
        {
            ScenarioContainer d = new ScenarioContainer();
            Random r = new Random(seed);

            //Generate Dungeon
            d.Player1 = TemplateGenerator.GeneratePlayer(c.PlayerTemplate, ref r);
            d.Player1.AttributeEmphasis = AttributeEmphasis.Agility;
            List<Level> layouts = _layoutGenerator.CreateDungeonLayouts(c, r);

            Level[] contents = _contentGenerator.CreateContents(c, layouts.ToArray(), survivorMode, r);
            for (int i = 0; i < layouts.Count; i++)
            {
                d.LoadedLevels.Add(layouts[i]);
            }

            PublishLoadingMessage("Initializing Scenario Meta Data", 99);

            //Load Encyclopedia Rogue-Tanica
            foreach (ConsumableTemplate t in c.ConsumableTemplates)
            {
                if (!d.ItemEncyclopedia.ContainsKey(t.Name))
                {
                    d.ItemEncyclopedia.Add(t.Name, new ScenarioMetaData(TemplateGenerator.GenerateSymbol(t.SymbolDetails), new AttackAttributeTemplate[] { }, DungeonMetaDataObjectTypes.Item, false, t.IsUnique, t.IsObjectiveItem, t.Name, t.SubType.ToString(), t.ShortDescription, t.LongDescription));
                }
            }
            foreach (EquipmentTemplate t in c.EquipmentTemplates)
            {
                if (!d.ItemEncyclopedia.ContainsKey(t.Name))
                    d.ItemEncyclopedia.Add(t.Name, new ScenarioMetaData(TemplateGenerator.GenerateSymbol(t.SymbolDetails), t.AttackAttributes,  DungeonMetaDataObjectTypes.Item, t.IsCursed, t.IsUnique, t.IsObjectiveItem, t.Name, t.Type.ToString(), t.ShortDescription, t.LongDescription));
            }
            foreach (DoodadTemplate dd in c.DoodadTemplates)
            {
                if (!d.ItemEncyclopedia.ContainsKey(dd.Name))
                    d.ItemEncyclopedia.Add(dd.Name, new ScenarioMetaData(TemplateGenerator.GenerateSymbol(dd.SymbolDetails), new AttackAttributeTemplate[]{}, DungeonMetaDataObjectTypes.Doodad, dd.IsCursed, dd.IsUnique, dd.IsObjectiveItem, dd.Name, DoodadType.Magic.ToString(), dd.ShortDescription, dd.LongDescription));
            }
            foreach (EnemyTemplate t in c.EnemyTemplates)
            {
                if (!d.ItemEncyclopedia.ContainsKey(t.Name))
                    d.ItemEncyclopedia.Add(t.Name, new ScenarioMetaData(TemplateGenerator.GenerateSymbol(t.SymbolDetails), t.AttackAttributes, DungeonMetaDataObjectTypes.Enemy, t.IsCursed, t.IsUnique, t.IsObjectiveItem, t.Name, "Enemy", t.ShortDescription, t.LongDescription));
            }
            foreach (SkillSetTemplate t in c.SkillTemplates)
            {
                if (!d.ItemEncyclopedia.ContainsKey(t.Name))
                    d.ItemEncyclopedia.Add(t.Name, new ScenarioMetaData(TemplateGenerator.GenerateSymbol(t.SymbolDetails), new AttackAttributeTemplate[]{},  DungeonMetaDataObjectTypes.Skill, t.IsCursed, t.IsUnique, t.IsObjectiveItem, t.Name, "Skill", t.ShortDescription, t.LongDescription));
            }

            //progressUpdate("Adding 'other' items...", 99);

            //Add entries for normal doodads
            d.ItemEncyclopedia.Add("Save Point", new ScenarioMetaData(TemplateGenerator.GenerateDefaultDoodadSymbol(DoodadNormalType.SavePoint), new AttackAttributeTemplate[] { }, DungeonMetaDataObjectTypes.Doodad, false, false, false, "Save Point", DoodadType.Normal.ToString(), "A tall odd shrine", "Contains strange markings related to reincarnation"));
            d.ItemEncyclopedia.Add("Stairs Down", new ScenarioMetaData(TemplateGenerator.GenerateDefaultDoodadSymbol(DoodadNormalType.StairsDown), new AttackAttributeTemplate[] { }, DungeonMetaDataObjectTypes.Doodad, false, false, false, "Stairs Down", DoodadType.Normal.ToString(), "Some stiars leading down", "These stairs lead to the next lower level of the dungeon"));
            d.ItemEncyclopedia.Add("Stairs Up", new ScenarioMetaData(TemplateGenerator.GenerateDefaultDoodadSymbol(DoodadNormalType.StairsUp), new AttackAttributeTemplate[] { }, DungeonMetaDataObjectTypes.Doodad, false, false, false, "Stairs Up", DoodadType.Normal.ToString(), "Some stairs leading up", "These stairs lead to the next upper level of the dungeon"));
            d.ItemEncyclopedia.Add("Teleport 1", new ScenarioMetaData(TemplateGenerator.GenerateDefaultDoodadSymbol(DoodadNormalType.Teleport1), new AttackAttributeTemplate[] { }, DungeonMetaDataObjectTypes.Doodad, false, false, false, "Teleporter A", DoodadType.Normal.ToString(), "A shrine dedicated to magical transport", "Contains strange markings related to magical transportation"));
            d.ItemEncyclopedia.Add("Teleport 2", new ScenarioMetaData(TemplateGenerator.GenerateDefaultDoodadSymbol(DoodadNormalType.Teleport2), new AttackAttributeTemplate[] { }, DungeonMetaDataObjectTypes.Doodad, false, false, false, "Teleporter B", DoodadType.Normal.ToString(), "A shrine dedicated to magical transport", "Contains strange markings related to magical transportation"));
            d.ItemEncyclopedia.Add("Random Teleporter", new ScenarioMetaData(TemplateGenerator.GenerateDefaultDoodadSymbol(DoodadNormalType.TeleportRandom), new AttackAttributeTemplate[] { }, DungeonMetaDataObjectTypes.Doodad, false, false, false, "Save Point", DoodadType.Normal.ToString(), "A tall odd shrine", "Contains strange markings related to magical transportation"));

            //Identify player skills / equipment / consumables
            foreach (SkillSet s in d.Player1.Skills)
            {
                if (s.LevelLearned <= d.Player1.Level)
                {
                    d.ItemEncyclopedia[s.RogueName].IsIdentified = true;
                }
            }

            foreach (Equipment e in d.Player1.EquipmentInventory)
            {
                d.ItemEncyclopedia[e.RogueName].IsIdentified = true;
                e.IsIdentified = true;
            }

            foreach (Consumable cc in d.Player1.ConsumableInventory)
            {
                d.ItemEncyclopedia[cc.RogueName].IsIdentified = true;
                cc.IsIdentified = true;
            }
            return d;
        }
        public ScenarioContainer CreateDebugScenario(ScenarioConfiguration c)
        {
            LayoutTemplate t = new LayoutTemplate();
            t.GenerationRate = 1;
            t.HiddenDoorProbability = 0;
            t.Level = new Range<int>(0, 0, 100, 100);
            t.Name = "Debug Level";
            t.NumberRoomCols = 3;
            t.NumberRoomRows = 3;
            t.RoomDivCellHeight = 20;
            t.RoomDivCellWidth = 20;
            t.Type = LayoutType.Normal;
            c.DungeonTemplate.LayoutTemplates.Clear();
            c.DungeonTemplate.LayoutTemplates.Add(t);

            foreach (ConsumableTemplate ct in c.ConsumableTemplates)
            {
                ct.Level = new Range<int>(0, 0, 100, 100);
                ct.GenerationRate = 3;
            }
            foreach (EquipmentTemplate et in c.EquipmentTemplates)
            {
                et.Level = new Range<int>(0, 0, 100, 100);
                et.GenerationRate = 3;
            }
            foreach (EnemyTemplate en in c.EnemyTemplates)
            {
                en.Level = new Range<int>(0, 0, 100, 100);
                en.GenerationRate = 3;
            }
            foreach (DoodadTemplate d in c.DoodadTemplates)
            {
                d.Level = new Range<int>(0, 0, 100, 100);
                d.GenerationRate = 3;
            }

            ConsumableTemplate identifyScroll = c.ConsumableTemplates.First(z => z.Name.Contains("Identify"));
            for (int i = 0;i<20;i++)
                c.PlayerTemplate.StartingConsumables.Add(
                    new ProbabilityConsumableTemplate()
                    {
                        TheTemplate = identifyScroll,
                        GenerationProbability = 1
                    });

            return CreateScenario(c, 1234, false);
        }
    }
}
