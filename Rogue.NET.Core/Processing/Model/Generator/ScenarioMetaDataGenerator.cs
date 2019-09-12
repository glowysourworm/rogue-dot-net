using Rogue.NET.Core.Model;
using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Scenario;
using Rogue.NET.Core.Model.Scenario.Content;
using Rogue.NET.Core.Model.ScenarioConfiguration.Abstract;
using Rogue.NET.Core.Model.ScenarioConfiguration.Alteration;
using Rogue.NET.Core.Model.ScenarioConfiguration.Content;
using Rogue.NET.Core.Processing.Model.Generator.Interface;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;

namespace Rogue.NET.Core.Processing.Model.Generator
{
    [Export(typeof(IScenarioMetaDataGenerator))]
    public class ScenarioMetaDataGenerator : IScenarioMetaDataGenerator
    {
        public ScenarioMetaDataGenerator() { }

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
                        Icon = ImageResources.StairsUp,
                        IsCursed = false,
                        IsIdentified = false,
                        IsObjective = false,
                        IsUnique = false,
                        LongDescription = "These stairs lead to the previous upper level",
                        ObjectType = DungeonMetaDataObjectTypes.Doodad,
                        RogueName = ModelConstants.DoodadStairsUpRogueName,
                        SymbolType = SymbolTypes.Image,
                        Type = DoodadType.Normal.ToString()
                    };
                case DoodadNormalType.StairsDown:
                    return new ScenarioMetaData()
                    {
                        Description = "Some stairs leading down (Press \"D\" to Advance to Next Level)",
                        Icon = ImageResources.StairsDown,
                        IsCursed = false,
                        IsIdentified = false,
                        IsObjective = false,
                        IsUnique = false,
                        LongDescription = "These stairs lead to the next lower level",
                        ObjectType = DungeonMetaDataObjectTypes.Doodad,
                        RogueName = ModelConstants.DoodadStairsDownRogueName,
                        SymbolType = SymbolTypes.Image,
                        Type = DoodadType.Normal.ToString()
                    };
                case DoodadNormalType.SavePoint:
                    return new ScenarioMetaData()
                    {
                        Description = "A Tall Odd Shrine (Press \"D\" to Save Progress)",
                        Icon = ImageResources.SavePoint,
                        IsCursed = false,
                        IsIdentified = false,
                        IsObjective = false,
                        IsUnique = false,
                        LongDescription = "Contains strange markings related to reincarnation",
                        ObjectType = DungeonMetaDataObjectTypes.Doodad,
                        RogueName = ModelConstants.DoodadSavePointRogueName,
                        SymbolType = SymbolTypes.Image,
                        Type = DoodadType.Normal.ToString()
                    };
                case DoodadNormalType.Teleport1:
                    return new ScenarioMetaData()
                    {
                        Description = "A shrine dedicated to magical transport",
                        Icon = ImageResources.teleport1,
                        IsCursed = false,
                        IsIdentified = false,
                        IsObjective = false,
                        IsUnique = false,
                        LongDescription = "Contains strange markings related to magical transportation",
                        ObjectType = DungeonMetaDataObjectTypes.Doodad,
                        RogueName = ModelConstants.DoodadTeleporterARogueName,
                        SymbolType = SymbolTypes.Image,
                        Type = DoodadType.Normal.ToString()
                    };
                case DoodadNormalType.Teleport2:
                    return new ScenarioMetaData()
                    {
                        Description = "A shrine dedicated to magical transport",
                        Icon = ImageResources.teleport2,
                        IsCursed = false,
                        IsIdentified = false,
                        IsObjective = false,
                        IsUnique = false,
                        LongDescription = "Contains strange markings related to magical transportation",
                        ObjectType = DungeonMetaDataObjectTypes.Doodad,
                        RogueName = ModelConstants.DoodadTeleporterBRogueName,
                        SymbolType = SymbolTypes.Image,
                        Type = DoodadType.Normal.ToString()
                    };
                case DoodadNormalType.TeleportRandom:
                    return new ScenarioMetaData()
                    {
                        Description = "A shrine dedicated to magical transport",
                        Icon = ImageResources.TeleportRandom,
                        IsCursed = false,
                        IsIdentified = false,
                        IsObjective = false,
                        IsUnique = false,
                        LongDescription = "Contains strange markings related to magical transportation",
                        ObjectType = DungeonMetaDataObjectTypes.Doodad,
                        RogueName = ModelConstants.DoodadTeleporterRandomRogueName,
                        SymbolType = SymbolTypes.Image,
                        Type = DoodadType.Normal.ToString()
                    };
                default:
                    throw new Exception("Trying to create unknown Normal Doodad type");
            }

        }

        protected void SetScenarioObjectProperties(ScenarioMetaData metaData, DungeonObjectTemplate template)
        {
            metaData.CharacterColor = template.SymbolDetails.CharacterColor;
            metaData.CharacterSymbol = template.SymbolDetails.CharacterSymbol;
            metaData.Description = template.ShortDescription;
            metaData.DisplayIcon = template.SymbolDetails.DisplayIcon;
            metaData.Icon = template.SymbolDetails.Icon;
            metaData.IsCursed = template.IsCursed;
            metaData.IsCurseIdentified = false;
            metaData.IsIdentified = template.IsObjectiveItem;
            metaData.IsObjective = template.IsObjectiveItem;
            metaData.IsUnique = template.IsUnique;
            metaData.LongDescription = template.LongDescription;
            metaData.RogueName = template.Name;
            metaData.SmileyLightRadiusColor = template.SymbolDetails.SmileyAuraColor;
            metaData.SmileyBodyColor = template.SymbolDetails.SmileyBodyColor;
            metaData.SmileyLineColor = template.SymbolDetails.SmileyLineColor;
            metaData.SmileyExpression = template.SymbolDetails.SmileyExpression;
            metaData.SymbolType = template.SymbolDetails.Type;
        }
    }
}
