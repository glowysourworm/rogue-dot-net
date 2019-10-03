using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration;

namespace Rogue.NET.ScenarioEditor.Controller.Interface
{
    public interface IScenarioEditorController
    {
        ScenarioConfigurationContainerViewModel CurrentConfig { get; }

        /// <summary>
        /// Runs a validation routine and publishes the result to the IScenarioValidationViewModel
        /// </summary>
        void Validate();

        void New();
        void Open(string name, bool builtIn);
        void Save();
    }
}
