using Rogue.NET.Core.Model.ScenarioConfiguration;

namespace Rogue.NET.ScenarioEditor.Interface
{
    public interface IScenarioEditorController
    {
        ScenarioConfigurationContainer New();
        ScenarioConfigurationContainer Open(string name, bool builtIn);
        void Save();
        void Validate();
        void Score();
        void Upload();
        void Download();

        string AddAsset(string assetType);
        void RemoveAsset(string assetType, string assetName);
        void LoadAsset(string assetType, string assetName);
        bool UpdateAssetName(string oldName, string newName, string type);

        void LoadConstruction(string constructionName);
    }
}
