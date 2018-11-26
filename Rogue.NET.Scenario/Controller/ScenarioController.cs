using Prism.Events;
using Rogue.NET.Common.Events.Scenario;
using Rogue.NET.Core.Event.Scenario.Level.Event;
using Rogue.NET.Core.Event.Splash;
using Rogue.NET.Core.Logic.Processing;
using Rogue.NET.Core.Logic.Processing.Interface;
using Rogue.NET.Core.Service.Interface;
using Rogue.NET.Model.Events;
using Rogue.NET.Scenario.Controller.Interface;
using System.ComponentModel.Composition;
using System.Threading.Tasks;

namespace Rogue.NET.Scenario.Controller
{
    /// <summary>
    /// Implementation of the IScenarioController that uses separate Dispatcher / Worker threads to manage communication
    /// between:  UIThread (Front End) | IScenarioController (dispatcher) thread (Not UIThread) | Backend thread.
    /// </summary>
    [Export(typeof(IScenarioController))]
    public class ScenarioController : IScenarioController
    {
        readonly IEventAggregator _eventAggregator;
        readonly IScenarioService _scenarioService;

        string _subscriptionToken;

        // Block inputs until Start() is called
        bool _blockUserInput = true;

        [ImportingConstructor]
        public ScenarioController(
            IEventAggregator eventAggregator, 
            IScenarioService scenarioService)
        {
            _eventAggregator = eventAggregator;
            _scenarioService = scenarioService;

            // Subscribe to user input
            _eventAggregator.GetEvent<UserCommandEvent>().Subscribe(OnUserCommand);
        }
        public void Stop()
        {
            _blockUserInput = true;

            // Second, clear and unload all backend queues. (This is safe because backend thread is halted)
            _scenarioService.ClearQueues();
        }
        public void Start()
        {
            _blockUserInput = false;
        }

        private async Task OnUserCommand(UserCommandEventArgs e)
        {
            if (_blockUserInput)
                return;

            // First issue command
            _scenarioService.IssueCommand(new LevelCommandAction()
            {
                Action = e.Action,
                Direction = e.Direction,
                ScenarioObjectId = e.ItemId
            });

            // Then, process all message queues
            await Work();
        }

        /// <summary>
        /// Primary message queue processing. Will process messages synchronously until
        /// all queues are completely empty
        /// </summary>
        private async Task Work()
        {
            var processing = true;

            // 0) Frontend Processing (Wait)
            // 1) Backend Process 1 Message
            // 2) If (no further processing of either) Then completed (can accept new user command)
            while (processing)
            {
                // First: Process all animations
                while (_scenarioService.AnyAnimationEvents())
                    await ProcessAnimationUpdate(_scenarioService.DequeueAnimationUpdate());

                // Second: Process all Splash events
                while (_scenarioService.AnySplashEvents())
                    ProcessSplashUpdate(_scenarioService.DequeueSplashUpdate());

                // Third: Process all Dialog events
                while (_scenarioService.AnyDialogEvents())
                    ProcessDialogUpdate(_scenarioService.DequeueDialogUpdate());

                // Fourth: Process all UI events
                while (_scenarioService.AnyLevelEvents())
                    ProcessUIUpdate(_scenarioService.DequeueLevelUpdate());

                // Fifth: Process all Scenario Events
                while (_scenarioService.AnyScenarioEvents())
                    ProcessScenarioUpdate(_scenarioService.DequeueScenarioUpdate());

                // Finally: Process the next backend work item. (If processing finished - then allow user command)
                if (!_scenarioService.ProcessBackend())
                    processing = false;
            }
        }

        #region (private) Processing
        private async Task ProcessAnimationUpdate(IAnimationUpdate update)
        {
            await _eventAggregator.GetEvent<AnimationStartEvent>().Publish(update);
        }
        private void ProcessUIUpdate(ILevelUpdate update)
        {
            _eventAggregator.GetEvent<LevelUpdateEvent>().Publish(update);
        }
        private void ProcessScenarioUpdate(IScenarioUpdate update)
        {
            _eventAggregator.GetEvent<ScenarioUpdateEvent>().Publish(update);
        }
        private void ProcessSplashUpdate(ISplashUpdate update)
        {
            // Synchronous Show / Hide
            _eventAggregator.GetEvent<SplashEvent>().Publish(update);
        }
        private void ProcessDialogUpdate(IDialogUpdate update)
        {
            // Synchronous window.ShowDialog() events
            _eventAggregator.GetEvent<DialogEvent>().Publish(update);
        }
        #endregion
    }
}
