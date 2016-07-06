using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Unity;
using Rogue.NET.Model.Events;
using Rogue.NET.Scenario.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rogue.NET.Model.Scenario;
using System.Drawing;
using Rogue.NET.Scenario.Outro.Views;
using Rogue.NET.Common.Events.Scenario;
using Rogue.NET.Intro.Views;
using Rogue.NET.Common.Events.ScenarioEditor;
using Rogue.NET.Scenario.Events;
using Rogue.NET.Scenario.Intro.Views.GameSetup;
using Microsoft.Practices.Prism.PubSubEvents;

namespace Rogue.NET.Scenario
{
    public class ScenarioController
    {
        readonly IUnityContainer _unityContainer;
        readonly IEventAggregator _eventAggregator;
        readonly IRegionManager _regionManager;

        public ScenarioController(
            IUnityContainer unityContainer,
            IEventAggregator eventAggregator,
            IRegionManager regionManager)
        {
            _unityContainer = unityContainer;
            _eventAggregator = eventAggregator;
            _regionManager = regionManager;

            Initialize();
        }
        private void Initialize()
        {
            _eventAggregator.GetEvent<LevelInitializedEvent>().Subscribe((e) =>
            {
                var view = _unityContainer.Resolve<GameView>();
                view.DataContext = e.Data;

                _regionManager.RequestNavigate("MainRegion", "GameView");
                _regionManager.RequestNavigate("GameRegion", "LevelView");
                _regionManager.RequestNavigate("GameInfoRegion", "GameInfoView");

            }, true);

            _eventAggregator.GetEvent<PlayerDiedEvent>().Subscribe((e) =>
            {
                var view = _unityContainer.Resolve<DeathDisplay>();
                view.DataContext = e;

                _regionManager.RequestNavigate("MainRegion", "DeathDisplay");
            }, true);

            _eventAggregator.GetEvent<ExitScenarioEvent>().Subscribe((e) =>
            {
                _regionManager.RequestNavigate("MainRegion", "GameSetupView");
                _regionManager.RequestNavigate("GameSetupRegion", "NewOpenEdit");
            });

            _eventAggregator.GetEvent<ExitScenarioEditorEvent>().Subscribe((e) =>
            {
                _regionManager.RequestNavigate("MainRegion", "GameSetupView");
                _regionManager.RequestNavigate("GameSetupRegion", "NewOpenEdit");
            }, true);

            _eventAggregator.GetEvent<IntroFinishedEvent>().Subscribe((e) =>
            {
                _regionManager.RequestNavigate("MainRegion", "GameSetupView");
                _regionManager.RequestNavigate("GameSetupRegion", "NewOpenEdit");
            });

            _eventAggregator.GetEvent<GameSetupDisplayFinished>().Subscribe((e) =>
            {
                _regionManager.RequestNavigate("GameSetupRegion", e.NextDisplayType.Name);
            });
        }
    }
}
