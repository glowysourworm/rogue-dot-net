using Rogue.NET.ScenarioEditor.ViewModel.Interface;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Abstract;

namespace Rogue.NET.ScenarioEditor.Controller.Interface
{
    public interface IScenarioAssetController
    {
        void AddAsset(string assetType, string uniqueName);
        void RemoveAsset(string assetType, string name);
        TemplateViewModel GetAsset(string name, string assetType);
    }
}
