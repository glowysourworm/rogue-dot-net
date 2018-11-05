using Prism.Events;
using Prism.Mef.Modularity;
using Prism.Modularity;
using Prism.Regions;
using Rogue.NET.Common.Events.Scenario;
using Rogue.NET.ScenarioEditor.Interface;
using System.ComponentModel.Composition;

namespace Rogue.NET.ScenarioEditor
{
    [ModuleExport("ScenarioEditor", typeof(ScenarioEditorModule))]
    public class ScenarioEditorModule : IModule
    {
        readonly IEventAggregator _eventAggregator;
        readonly IRegionManager _regionManager;
        readonly IScenarioEditorController _scenarioEditorController;

        [ImportingConstructor]
        public ScenarioEditorModule(
            IRegionManager regionManager,
            IEventAggregator eventAggregator,
            IScenarioEditorController scenarioEditorController)
        {
            _regionManager = regionManager;
            _eventAggregator = eventAggregator;
            _scenarioEditorController = scenarioEditorController;
        }

        public void Initialize()
        {
            _eventAggregator.GetEvent<EditScenarioEvent>().Subscribe(() =>
            {
                _regionManager.RequestNavigate("MainRegion", "Editor");
                _regionManager.RequestNavigate("DesignRegion", "EditorInstructions");

                // Create an instance of the config so that there aren't any null refs.
                _scenarioEditorController.New();
            });
        }
    }
}
