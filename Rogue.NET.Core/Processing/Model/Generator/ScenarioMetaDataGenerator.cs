using Rogue.NET.Common.Constant;
using Rogue.NET.Core.Model;
using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Scenario.Abstract;
using Rogue.NET.Core.Model.ScenarioConfiguration.Abstract;
using Rogue.NET.Core.Model.ScenarioConfiguration.Alteration;
using Rogue.NET.Core.Model.ScenarioConfiguration.Content;
using Rogue.NET.Core.Processing.Model.Generator.Interface;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;

namespace Rogue.NET.Core.Processing.Model.Generator
{
    [PartCreationPolicy(CreationPolicy.Shared)]
    [Export(typeof(IScenarioMetaDataGenerator))]
    public class ScenarioMetaDataGenerator : IScenarioMetaDataGenerator
    {
        readonly ISymbolDetailsGenerator _symbolDetailsGenerator;

        [ImportingConstructor]
        public ScenarioMetaDataGenerator(ISymbolDetailsGenerator symbolDetailsGenerator)
        {
            _symbolDetailsGenerator = symbolDetailsGenerator;
        }

        public ScenarioMetaData CreateScenarioMetaData(ConsumableTemplate template)
        {
            var metaData = new ScenarioMetaData();

            SetScenarioObjectProperties(metaData, template);

            metaData.AttackAttributes = new List<AttackAttributeTemplate>();
            metaData.ObjectType = DungeonMetaDataObjectTypes.Item;

            // Designation for Scenario Meta Data
            metaData.Type = template.SubType.ToString();

            return metaData;
        }

        public ScenarioMetaData CreateScenarioMetaData(EquipmentTemplate template)
        {
            var metaData = new ScenarioMetaData();

            SetScenarioObjectProperties(metaData, template);

            metaData.AttackAttributes = new List<AttackAttributeTemplate>(template.AttackAttributes);
            metaData.ObjectType = DungeonMetaDataObjectTypes.Item;

            // Designation for Scenario Meta Data (Equipment)
            metaData.Type = template.Type.ToString();

            return metaData;
        }

        public ScenarioMetaData CreateScenarioMetaData(EnemyTemplate template)
        {
            var metaData = new ScenarioMetaData();

            SetScenarioObjectProperties(metaData, template);

            metaData.AttackAttributes = new List<AttackAttributeTemplate>(template.AttackAttributes);
            metaData.ObjectType = DungeonMetaDataObjectTypes.Character;

            // Designation for Scenario Meta Data (Enemy)
            metaData.Type = "Enemy";

            return metaData;
        }

        public ScenarioMetaData CreateScenarioMetaData(FriendlyTemplate template)
        {
            var metaData = new ScenarioMetaData();

            SetScenarioObjectProperties(metaData, template);

            metaData.AttackAttributes = new List<AttackAttributeTemplate>(template.AttackAttributes);
            metaData.ObjectType = DungeonMetaDataObjectTypes.Character;

            // Designation for Scenario Meta Data (Friendly)
            metaData.Type = "Friendly";

            return metaData;
        }

        public ScenarioMetaData CreateScenarioMetaData(TemporaryCharacterTemplate template)
        {
            var metaData = new ScenarioMetaData();

            SetScenarioObjectProperties(metaData, template);

            metaData.AttackAttributes = new List<AttackAttributeTemplate>(template.AttackAttributes);
            metaData.ObjectType = DungeonMetaDataObjectTypes.Character;

            // Designation for Scenario Meta Data (Temporary Character)
            metaData.Type = "Temporary Character";

            return metaData;
        }

        public ScenarioMetaData CreateScenarioMetaData(DoodadTemplate template)
        {
            var metaData = new ScenarioMetaData();

            SetScenarioObjectProperties(metaData, template);

            metaData.ObjectType = DungeonMetaDataObjectTypes.Doodad;

            // Designation for Scenario Meta Data (Magic Doodads)
            metaData.Type = DoodadType.Magic.ToString();

            return metaData;
        }

        public ScenarioMetaData CreateScenarioMetaData(SkillSetTemplate template)
        {
            var metaData = new ScenarioMetaData();

            SetScenarioObjectProperties(metaData, template);

            metaData.ObjectType = DungeonMetaDataObjectTypes.Skill;

            // Designation for Scenario Meta Data (Magic Doodads)
            metaData.Type = "Skill";

            return metaData;
        }

        public ScenarioMetaData CreateScenarioMetaData(DoodadNormalType doodadNormalType)
        {
            switch (doodadNormalType)
            {
                case DoodadNormalType.StairsUp:
                    return new ScenarioMetaData()
                    {
                        Description = "Some stairs leading up (Press \"D\" to Advance to Previous Level)",
                        IsCursed = false,
                        IsIdentified = false,
                        IsObjective = false,
                        IsUnique = false,
                        LongDescription = "These stairs lead to the previous upper level",
                        ObjectType = DungeonMetaDataObjectTypes.Doodad,
                        RogueName = ModelConstants.DoodadStairsUpRogueName,
                        SymbolType = SymbolType.Game,
                        GameSymbol = GameSymbol.StairsUp,
                        Type = DoodadType.Normal.ToString()
                    };
                case DoodadNormalType.StairsDown:
                    return new ScenarioMetaData()
                    {
                        Description = "Some stairs leading down (Press \"D\" to Advance to Next Level)",
                        IsCursed = false,
                        IsIdentified = false,
                        IsObjective = false,
                        IsUnique = false,
                        LongDescription = "These stairs lead to the next lower level",
                        ObjectType = DungeonMetaDataObjectTypes.Doodad,
                        RogueName = ModelConstants.DoodadStairsDownRogueName,
                        SymbolType = SymbolType.Game,
                        GameSymbol = GameSymbol.StairsDown,
                        Type = DoodadType.Normal.ToString()
                    };
                case DoodadNormalType.SavePoint:
                    return new ScenarioMetaData()
                    {
                        Description = "A Tall Odd Shrine (Press \"D\" to Save Progress)",
                        IsCursed = false,
                        IsIdentified = false,
                        IsObjective = false,
                        IsUnique = false,
                        LongDescription = "Contains strange markings related to reincarnation",
                        ObjectType = DungeonMetaDataObjectTypes.Doodad,
                        RogueName = ModelConstants.DoodadSavePointRogueName,
                        SymbolType = SymbolType.Game,
                        GameSymbol = GameSymbol.SavePoint,
                        Type = DoodadType.Normal.ToString()
                    };
                case DoodadNormalType.Teleport1:
                    return new ScenarioMetaData()
                    {
                        Description = "A shrine dedicated to magical transport",
                        IsCursed = false,
                        IsIdentified = false,
                        IsObjective = false,
                        IsUnique = false,
                        LongDescription = "Contains strange markings related to magical transportation",
                        ObjectType = DungeonMetaDataObjectTypes.Doodad,
                        RogueName = ModelConstants.DoodadTeleporterARogueName,
                        SymbolType = SymbolType.Game,
                        GameSymbol = GameSymbol.Teleport1,
                        Type = DoodadType.Normal.ToString()
                    };
                case DoodadNormalType.Teleport2:
                    return new ScenarioMetaData()
                    {
                        Description = "A shrine dedicated to magical transport",
                        IsCursed = false,
                        IsIdentified = false,
                        IsObjective = false,
                        IsUnique = false,
                        LongDescription = "Contains strange markings related to magical transportation",
                        ObjectType = DungeonMetaDataObjectTypes.Doodad,
                        RogueName = ModelConstants.DoodadTeleporterBRogueName,
                        SymbolType = SymbolType.Game,
                        GameSymbol = GameSymbol.Teleport2,
                        Type = DoodadType.Normal.ToString()
                    };
                case DoodadNormalType.TeleportRandom:
                    return new ScenarioMetaData()
                    {
                        Description = "A shrine dedicated to magical transport",
                        IsCursed = false,
                        IsIdentified = false,
                        IsObjective = false,
                        IsUnique = false,
                        LongDescription = "Contains strange markings related to magical transportation",
                        ObjectType = DungeonMetaDataObjectTypes.Doodad,
                        RogueName = ModelConstants.DoodadTeleporterRandomRogueName,
                        SymbolType = SymbolType.Game,
                        GameSymbol = GameSymbol.TeleportRandom,
                        Type = DoodadType.Normal.ToString()
                    };
                default:
                    throw new Exception("Trying to create unknown Normal Doodad type");
            }

        }

        protected void SetScenarioObjectProperties(ScenarioMetaData metaData, DungeonObjectTemplate template)
        {
            // Map the base symbol details
            _symbolDetailsGenerator.MapSymbolDetails(template.SymbolDetails, metaData);

            // Map the rest of the public properties
            metaData.Description = template.ShortDescription;
            metaData.IsCursed = template.IsCursed;
            metaData.IsCurseIdentified = false;
            metaData.IsIdentified = template.IsObjectiveItem;
            metaData.IsObjective = template.IsObjectiveItem;
            metaData.IsUnique = template.IsUnique;
            metaData.LongDescription = template.LongDescription;
            metaData.RogueName = template.Name;
        }
    }
}
