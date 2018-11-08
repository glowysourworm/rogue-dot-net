using Rogue.NET.ScenarioEditor.ViewModel.Constant;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Abstract;
using System.Collections.Generic;
using System.Linq;

namespace Rogue.NET.ScenarioEditor.Utility
{
    public static class ConfigurationCollectionResolver
    {
        /// <summary>
        /// Returns the configuration collection for the specified asset type
        /// </summary>
        public static object GetAssetCollection(ScenarioConfigurationContainerViewModel configuration, string assetType)
        {
            // For layouts must use collection in the DungeonTemplate
            if (assetType == AssetType.Layout)
                return configuration.DungeonTemplate.LayoutTemplates;

            // Use reflection to get the collection from the configuration
            var collectionType = AssetType.AssetCollectionTypes[assetType];
            var collectionPropertyInfo = typeof(ScenarioConfigurationContainerViewModel)
                                            .GetProperties()
                                            .First(x => x.PropertyType == collectionType);

            return collectionPropertyInfo.GetValue(configuration);
        }
    }
}
