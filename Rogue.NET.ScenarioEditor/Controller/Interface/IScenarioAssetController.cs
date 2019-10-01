using Rogue.NET.ScenarioEditor.ViewModel.Interface;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Abstract;
using System;

namespace Rogue.NET.ScenarioEditor.Controller.Interface
{
    public interface IScenarioAssetController
    {
        void AddAsset(Type assetType, string uniqueName);
        void CopyAsset(string assetName, string assetNewName, Type assetType);
        void RemoveAsset(Type assetType, string name);
        TemplateViewModel GetAsset(string name, Type assetType);
    }
}
