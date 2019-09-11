using Rogue.NET.Core.Utility;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Abstract;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Alteration;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Content;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Layout;
using Rogue.NET.ScenarioEditor.Views.Assets;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Rogue.NET.ScenarioEditor.ViewModel.Constant
{
    public static class AssetType
    {
        public const string Consumable = "Consumable";
        public const string Doodad = "Doodad";
        public const string Enemy = "Enemy";
        public const string Friendly = "Friendly";
        public const string TemporaryCharacter = "TemporaryCharacter";
        public const string Equipment = "Equipment";
        public const string Layout = "Layout";
        public const string SkillSet = "SkillSet";

        public static bool HasSymbol(string assetType)
        {
            return !(assetType == Layout);
        }

        public static bool HasLevelPlacement(string assetType)
        {
            return assetType == Layout ||
                   assetType == Consumable ||
                   assetType == Doodad ||
                   assetType == Enemy ||
                   assetType == Friendly ||
                   assetType == Equipment;
        }

        public static string GetSubType(TemplateViewModel viewModel)
        {
            if (viewModel is EquipmentTemplateViewModel)
                return GetSubType(viewModel as EquipmentTemplateViewModel);

            else if (viewModel is ConsumableTemplateViewModel)
                return GetSubType(viewModel as ConsumableTemplateViewModel);

            else
                return "";
        }
        public static string GetSubType(EquipmentTemplateViewModel viewModel)
        {
            return TextUtility.CamelCaseToTitleCase(viewModel.Type.ToString());
        }
        public static string GetSubType(ConsumableTemplateViewModel viewModel)
        {
            return TextUtility.CamelCaseToTitleCase(viewModel.Type.ToString());
        }

        public static int GetRequiredLevel(DungeonObjectTemplateViewModel viewModel)
        {
            if (viewModel is ConsumableTemplateViewModel)
                return (viewModel as ConsumableTemplateViewModel).LevelRequired;

            else if (viewModel is EquipmentTemplateViewModel)
                return (viewModel as EquipmentTemplateViewModel).LevelRequired;

            return 0;
        }

        /// <summary>
        /// Collection of asset view types by asset name
        /// </summary>
        public static readonly Dictionary<string, Type> AssetViewTypes = new Dictionary<string, Type>()
        {
            { Consumable, typeof(Consumable) },
            { Doodad, typeof(Doodad) },
            { Enemy, typeof(Enemy) },
            { Friendly, typeof(Friendly) },
            { TemporaryCharacter , typeof(TemporaryCharacter) },
            { Equipment, typeof(Equipment) },
            { Layout, typeof(Layout) },
            { SkillSet, typeof(SkillSet) }
        };

        /// <summary>
        /// Collection of asset collection types by asset type
        /// </summary>
        public static readonly Dictionary<string, Type> AssetCollectionTypes = new Dictionary<string, Type>()
        {
            { Consumable, typeof(ObservableCollection<ConsumableTemplateViewModel>) },
            { Doodad, typeof(ObservableCollection<DoodadTemplateViewModel>) },
            { Enemy, typeof(ObservableCollection<EnemyTemplateViewModel>) },
            { Friendly, typeof(ObservableCollection<FriendlyTemplateViewModel>) },
            { TemporaryCharacter, typeof(ObservableCollection<TemporaryCharacterTemplateViewModel>) },
            { Equipment, typeof(ObservableCollection<EquipmentTemplateViewModel>) },
            { Layout, typeof(ObservableCollection<LayoutTemplateViewModel>) },
            { SkillSet, typeof(ObservableCollection<SkillSetTemplateViewModel>) }
        };
    }
}
