using Prism.Events;
using Rogue.NET.Common.EventArgs;
using Rogue.NET.Common.Events.Scenario;
using Rogue.NET.Core.Event.Scenario.Level.Event;
using Rogue.NET.Core.Logic.Processing;
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
            return;

            _scenarioService.IssueCommand(new LevelCommand()
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
                while (_scenarioService.AnyAnimationEvents())
                    ProcessAnimationEvent(_scenarioService.DequeueAnimation());

                // Second: Process all UI events
                while (_scenarioService.AnyLevelEvents())
                    ProcessUIEvent(_scenarioService.DequeueLevelEvent());

                // Third: Process entire backend queue
                while (_scenarioService.ProcessBackend()) { }

                // Last: Check to see whether there are any new UI or animation
                //       events to process
                processing = _scenarioService.AnyAnimationEvents() ||
                             _scenarioService.AnyLevelEvents();
            }
        }
        private void ProcessAnimationEvent(IAnimationEvent animationEvent)
        {

        }
        private void ProcessUIEvent(ILevelUpdate uiEvent)
        {
            _eventAggregator.GetEvent<LevelUpdateEvent>().Publish(uiEvent);
        }
    }
}
