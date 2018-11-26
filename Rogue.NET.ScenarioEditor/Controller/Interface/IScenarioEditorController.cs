using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration;
using System.Threading.Tasks;

namespace Rogue.NET.ScenarioEditor.Controller.Interface
{
    public interface IScenarioEditorController
    {
        ScenarioConfigurationContainerViewModel CurrentConfig { get; }

        void New();
        Task Open(string name, bool builtIn);
        Task Save();
        void Validate();
    }
}
