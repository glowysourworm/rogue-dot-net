using Prism.Events;
using Rogue.NET.Common.EventArgs;
using Rogue.NET.Common.Events.Scenario;
using Rogue.NET.Core.Event.Scenario.Level.Event;
using Rogue.NET.Core.Logic.Processing;
using Rogue.NET.Core.Logic.Processing.Enum;
using Rogue.NET.Core.Logic.Processing.Interface;
using Rogue.NET.Core.Service.Interface;
using Rogue.NET.Scenario.Controller.Interface;
using System.ComponentModel.Composition;

namespace Rogue.NET.Scenario.Controller
{
    [Export(typeof(IScenarioController))]
    public class ScenarioController : IScenarioController
    {
        readonly IEventAggregator _eventAggregator;
        readonly IScenarioService _scenarioService;

        SubscriptionToken _userCommandToken;

        [ImportingConstructor]
        public ScenarioController(
            IEventAggregator eventAggregator, 
            IScenarioService scenarioService)
        {
            _eventAggregator = eventAggregator;
            _scenarioService = scenarioService;
        }

        public void Initialize()
        {
            
        }
        public void EnterGameMode()
        {
            // Subscribe to user input
            _userCommandToken = _eventAggregator.GetEvent<UserCommandEvent>().Subscribe(OnUserCommand, true);
            
        }
        public void ExitGameMode()
        {
            _eventAggregator.GetEvent<UserCommandEvent>().Unsubscribe(_userCommandToken);
        }

        private void OnUserCommand(LevelCommandEventArgs e)
        {
            _scenarioService.IssueCommand(new LevelCommandAction()
            {
                Action = e.Action,
                Direction = e.Direction,
                ScenarioObjectId = e.ItemId
            });

            // IssueCommand -> Fills up IScenarioService queues. Example: 0) Player moves 1) Enemy reactions queued 
            // Processing involves checking animation and UI update queues - then continuing
            // with backend logic (IScenarioService.Process)

            // Zero:  Enter processing loop
            var processing = true;
            while (processing)
            {
                // First: Process all animations
                while (_scenarioService.AnyAnimationEvents() && processing)
                    processing = ProcessAnimationUpdate(_scenarioService.DequeueAnimationUpdate());

                // Second: Process all Scenario Events
                while (_scenarioService.AnyScenarioEvents() && processing)
                    processing = ProcessScenarioUpdate(_scenarioService.DequeueScenarioUpdate());

                // Third: Process all Splash events
                while (_scenarioService.AnySplashEvents() && processing)
                    processing = ProcessSplashUpdate(_scenarioService.DequeueSplashUpdate());

                // Fourth: PRocess all UI events
                while (_scenarioService.AnyLevelEvents())
                    processing = ProcessUIUpdate(_scenarioService.DequeueLevelUpdate());

                // Finally: Process the rest of the backend (Data) queue.
                while (_scenarioService.ProcessBackend()) { }

                processing = processing &&
                    (_scenarioService.AnyAnimationEvents() ||
                     _scenarioService.AnyLevelEvents() ||
                     _scenarioService.AnyScenarioEvents() ||
                     _scenarioService.AnySplashEvents());
            }
        }
        private bool ProcessAnimationUpdate(IAnimationUpdate update)
        {
            return true;
        }
        private bool ProcessUIUpdate(ILevelUpdate update)
        {
            _eventAggregator.GetEvent<LevelUpdateEvent>().Publish(update);

            return true;
        }
        private bool ProcessScenarioUpdate(IScenarioUpdate update)
        {
            return update.ScenarioUpdateType != ScenarioUpdateType.PlayerDeath;
        }
        private bool ProcessSplashUpdate(ISplashUpdate update)
        {
            return true;
        }
    }
}
