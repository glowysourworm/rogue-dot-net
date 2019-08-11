using Rogue.NET.Core.Utility;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Abstract;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Alteration;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Animation;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Content;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Layout;
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
        public const string Equipment = "Equipment";
        public const string Layout = "Layout";
        public const string SkillSet = "SkillSet";

        public const string ConsumableDisplay = "Consumable";
        public const string DoodadDisplay = "Scenario Object";
        public const string EnemyDisplay = "Enemy";
        public const string EquipmentDisplay = "Equipment";
        public const string LayoutDisplay = "Layout";
        public const string SkillSetDisplay = "Skill Set";

        public const string ConsumableViewName = "Consumable";
        public const string DoodadViewName = "Doodad";
        public const string EnemyViewName = "Enemy";
        public const string EquipmentViewName = "Equipment";
        public const string LayoutViewName = "Layout";
        public const string SkillSetViewName = "SkillSet";

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
        /// Collection of asset display names by asset type
        /// </summary>
        public static readonly Dictionary<string, string> AssetTypes = new Dictionary<string, string>()
        {
            { Consumable, ConsumableDisplay },
            { Doodad, DoodadDisplay },
            { Enemy, EnemyDisplay },
            { Equipment, EquipmentDisplay },
            { Layout, LayoutDisplay },
            { SkillSet, SkillSetDisplay }
        };

        /// <summary>
        /// Collection of asset view names by asset type
        /// </summary>
        public static readonly Dictionary<string, string> AssetViews = new Dictionary<string, string>()
        {
            { Consumable, ConsumableViewName },
            { Doodad, DoodadViewName },
            { Enemy, EnemyViewName },
            { Equipment, EquipmentViewName },
            { Layout, LayoutViewName },
            { SkillSet, SkillSetViewName }
        };

        /// <summary>
        /// Collection of asset collection types by asset type
        /// </summary>
        public static readonly Dictionary<string, Type> AssetCollectionTypes = new Dictionary<string, Type>()
        {
            { Consumable, typeof(ObservableCollection<ConsumableTemplateViewModel>) },
            { Doodad, typeof(ObservableCollection<DoodadTemplateViewModel>) },
            { Enemy, typeof(ObservableCollection<EnemyTemplateViewModel>) },
            { Equipment, typeof(ObservableCollection<EquipmentTemplateViewModel>) },
            { Layout, typeof(ObservableCollection<LayoutTemplateViewModel>) },
            { SkillSet, typeof(ObservableCollection<SkillSetTemplateViewModel>) }
        };
    }
}
