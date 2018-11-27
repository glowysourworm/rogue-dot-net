using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Generator.Interface;
using Rogue.NET.Core.Model.Scenario;
using Rogue.NET.Core.Model.ScenarioConfiguration.Alteration;
using Rogue.NET.Core.Model.ScenarioConfiguration.Content;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;

namespace Rogue.NET.Core.Model.Generator
{
    [Export(typeof(IScenarioMetaDataGenerator))]
    public class ScenarioMetaDataGenerator : IScenarioMetaDataGenerator
    {
        public ScenarioMetaDataGenerator() { }

        public ScenarioMetaData CreateScenarioMetaData(ConsumableTemplate template)
        {
            return new ScenarioMetaData()
            {
                AttackAttributes = new List<AttackAttributeTemplate>(),
                CharacterColor = template.SymbolDetails.CharacterColor,
                CharacterSymbol = template.SymbolDetails.CharacterSymbol,
                Description = template.ShortDescription,
                Icon = template.SymbolDetails.Icon,
                IsCursed = template.IsCursed,
                IsCurseIdentified = false,
                IsIdentified = template.IsObjectiveItem,           // Identify all objectives immediately
                IsObjective = template.IsObjectiveItem,
                IsUnique = template.IsUnique,
                LongDescription = template.LongDescription,
                ObjectType = DungeonMetaDataObjectTypes.Item,
                RogueName = template.Name,
                SmileyAuraColor = template.SymbolDetails.SmileyAuraColor,
                SmileyBodyColor = template.SymbolDetails.SmileyBodyColor,
                SmileyLineColor = template.SymbolDetails.SmileyLineColor,
                SmileyMood = template.SymbolDetails.SmileyMood,
                SymbolType = template.SymbolDetails.Type,

                // Designation for Scenario Meta Data
                Type = template.SubType.ToString()
            };
        }

        public ScenarioMetaData CreateScenarioMetaData(EquipmentTemplate template)
        {
            return new ScenarioMetaData()
            {
                AttackAttributes = new List<AttackAttributeTemplate>(template.AttackAttributes),
                CharacterColor = template.SymbolDetails.CharacterColor,
                CharacterSymbol = template.SymbolDetails.CharacterSymbol,
                Description = template.ShortDescription,
                Icon = template.SymbolDetails.Icon,
                IsCursed = template.IsCursed,
                IsCurseIdentified = false,
                IsIdentified = template.IsObjectiveItem,
                IsObjective = template.IsObjectiveItem,
                IsUnique = template.IsUnique,
                LongDescription = template.LongDescription,
                ObjectType = DungeonMetaDataObjectTypes.Item,
                RogueName = template.Name,
                SmileyAuraColor = template.SymbolDetails.SmileyAuraColor,
                SmileyBodyColor = template.SymbolDetails.SmileyBodyColor,
                SmileyLineColor = template.SymbolDetails.SmileyLineColor,
                SmileyMood = template.SymbolDetails.SmileyMood,
                SymbolType = template.SymbolDetails.Type,

                // Designation for Scenario Meta Data
                Type = template.Type.ToString()
            };
        }

        public ScenarioMetaData CreateScenarioMetaData(EnemyTemplate template)
        {
            return new ScenarioMetaData()
            {
                AttackAttributes = new List<AttackAttributeTemplate>(template.AttackAttributes),
                CharacterColor = template.SymbolDetails.CharacterColor,
                CharacterSymbol = template.SymbolDetails.CharacterSymbol,
                Description = template.ShortDescription,
                Icon = template.SymbolDetails.Icon,
                IsCursed = template.IsCursed,
                IsCurseIdentified = false,
                IsIdentified = template.IsObjectiveItem,
                IsObjective = template.IsObjectiveItem,
                IsUnique = template.IsUnique,
                LongDescription = template.LongDescription,
                ObjectType = DungeonMetaDataObjectTypes.Enemy,
                RogueName = template.Name,
                SmileyAuraColor = template.SymbolDetails.SmileyAuraColor,
                SmileyBodyColor = template.SymbolDetails.SmileyBodyColor,
                SmileyLineColor = template.SymbolDetails.SmileyLineColor,
                SmileyMood = template.SymbolDetails.SmileyMood,
                SymbolType = template.SymbolDetails.Type,

                // Designation for Scenario Meta Data
                Type = "Enemy"
            };
        }

        public ScenarioMetaData CreateScenarioMetaData(DoodadTemplate template)
        {
            return new ScenarioMetaData()
            {
                AttackAttributes = new List<AttackAttributeTemplate>(),
                CharacterColor = template.SymbolDetails.CharacterColor,
                CharacterSymbol = template.SymbolDetails.CharacterSymbol,
                Description = template.ShortDescription,
                Icon = template.SymbolDetails.Icon,
                IsCursed = template.IsCursed,
                IsCurseIdentified = false,
                IsIdentified = template.IsObjectiveItem,
                IsObjective = template.IsObjectiveItem,
                IsUnique = template.IsUnique,
                LongDescription = template.LongDescription,
                ObjectType = DungeonMetaDataObjectTypes.Doodad,
                RogueName = template.Name,
                SmileyAuraColor = template.SymbolDetails.SmileyAuraColor,
                SmileyBodyColor = template.SymbolDetails.SmileyBodyColor,
                SmileyLineColor = template.SymbolDetails.SmileyLineColor,
                SmileyMood = template.SymbolDetails.SmileyMood,
                SymbolType = template.SymbolDetails.Type,

                // Designation for Scenario Meta Data
                Type = DoodadType.Magic.ToString()
            };
        }

        public ScenarioMetaData CreateScenarioMetaData(SkillSetTemplate template)
        {
            return new ScenarioMetaData()
            {
                AttackAttributes = new List<AttackAttributeTemplate>(),
                CharacterColor = template.SymbolDetails.CharacterColor,
                CharacterSymbol = template.SymbolDetails.CharacterSymbol,
                Description = template.ShortDescription,
                Icon = template.SymbolDetails.Icon,
                IsCursed = template.IsCursed,
                IsCurseIdentified = false,
                IsIdentified = template.IsObjectiveItem,
                IsObjective = template.IsObjectiveItem,
                IsUnique = template.IsUnique,
                LongDescription = template.LongDescription,
                ObjectType = DungeonMetaDataObjectTypes.Skill,
                RogueName = template.Name,
                SmileyAuraColor = template.SymbolDetails.SmileyAuraColor,
                SmileyBodyColor = template.SymbolDetails.SmileyBodyColor,
                SmileyLineColor = template.SymbolDetails.SmileyLineColor,
                SmileyMood = template.SymbolDetails.SmileyMood,
                SymbolType = template.SymbolDetails.Type,

                // Designation for Scenario Meta Data
                Type = "Skill"
            };
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
                        RogueName = "Stairs Up",
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
                        RogueName = "Stairs Down",
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
                        RogueName = "Save Point",
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
                        RogueName = "Teleporter A",
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
                        RogueName = "Teleporter B",
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
                        RogueName = "Random Teleporter",
                        SymbolType = SymbolTypes.Image,
                        Type = DoodadType.Normal.ToString()
                    };
                default:
                    throw new Exception("Trying to create unknown Normal Doodad type");
            }
        }
    }
}
