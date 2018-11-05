using Rogue.NET.Core.Model.ScenarioConfiguration;

namespace Rogue.NET.ScenarioEditor.Interface
{
    public interface IScenarioEditorController
    {
        void New();
        void Open(string name, bool builtIn);
        void Save();
        void Validate();
        void ShowDifficulty();

        string AddAsset(string assetType);
        void RemoveAsset(string assetType, string assetName);
        void LoadAsset(string assetType, string assetName);
        bool UpdateAssetName(string oldName, string newName, string type);

        void LoadConstruction(string constructionName);
    }
}
