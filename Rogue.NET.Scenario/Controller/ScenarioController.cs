using Rogue.NET.Common.Extension.Prism.EventAggregator;
using Rogue.NET.Core.GameRouter.GameEvent.Backend.Enum;
using Rogue.NET.Core.Processing.Command.Backend;
using Rogue.NET.Core.Processing.Command.Backend.CommandData;
using Rogue.NET.Core.Processing.Event.Backend;
using Rogue.NET.Core.Processing.Event.Backend.EventData;
using Rogue.NET.Core.Service.Interface;
using Rogue.NET.Scenario.Controller.Interface;
using System;
using System.ComponentModel.Composition;
using System.Linq;
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
        readonly IRogueEventAggregator _eventAggregator;
        readonly IScenarioService _scenarioService;

        string _subscriptionToken;

        // Block inputs until Start() is called
        bool _blockUserInput = true;

        [ImportingConstructor]
        public ScenarioController(
            IRogueEventAggregator eventAggregator, 
            IScenarioService scenarioService)
        {
            _eventAggregator = eventAggregator;
            _scenarioService = scenarioService;

            // Subscribe to user input
            _eventAggregator.GetEvent<LevelCommand>().Subscribe(async (e) => await OnLevelCommand(e));
            _eventAggregator.GetEvent<PlayerCommand>().Subscribe(async (e) => await OnPlayerCommand(e));
            _eventAggregator.GetEvent<PlayerMultiItemCommand>().Subscribe(async (e) => await OnPlayerMultiItemCommand(e));
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

        private async Task OnLevelCommand(LevelCommandData e)
        {
            if (_blockUserInput)
                return;

            // First issue command
            _scenarioService.IssueCommand(e);

            await Work();
        }

        private async Task OnPlayerCommand(PlayerCommandData e)
        {
            if (_blockUserInput)
                return;

            // First issue command
            _scenarioService.IssuePlayerCommand(e);

            await Work();
        }

        private async Task OnPlayerMultiItemCommand(PlayerMultiItemCommandData e)
        {
            if (_blockUserInput)
                return;

            // First issue command
            _scenarioService.IssuePlayerMultiItemCommand(e);

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
                // High Priority (Animations)

                var animationData = _scenarioService.DequeueAnimationEventData();
                while (animationData != null)
                {
                    // Process Animation (await)
                    await ProcessAnimationEvent(animationData);

                    // Dequeue Next Animation
                    animationData = _scenarioService.DequeueAnimationEventData();
                }

                // Level Update

                var levelEventData = _scenarioService.DequeueLevelEventData();
                while (levelEventData != null)
                {
                    // Process Level Update (await)
                    ProcessLevelEvent(levelEventData);

                    // Dequeue Next
                    levelEventData = _scenarioService.DequeueLevelEventData();
                }

                // Dialog Event

                var dialogEventData = _scenarioService.DequeueDialogEventData();
                while (dialogEventData != null)
                {
                    // Process Dialog (await)
                    ProcessDialogEvent(dialogEventData);

                    // Dequeue Next
                    dialogEventData = _scenarioService.DequeueDialogEventData();
                }

                // Process all Scenario Events
                //
                // THESE AFFECT THE LEVEL LOADING. SO, ALLOW UI PROCESSING FIRST (Low / High)

                var scenarioEventData = _scenarioService.DequeueScenarioEventData();
                while (scenarioEventData != null && processing)
                {
                    // Have to cancel processing on certain scenario events
                    processing = ProcessScenarioEvent(scenarioEventData);

                    // Dequeue Scenario Event
                    scenarioEventData = _scenarioService.DequeueScenarioEventData();
                }

                // Finally: Process the next backend work item. (If processing finished - then allow user command)
                //
                //          Processing backend messages will queue new work items for all priorities.
                if (!_scenarioService.ProcessBackend() && processing)
                    processing = false;
            }
        }

        #region (private) Processing
        private async Task ProcessAnimationEvent(AnimationEventData update)
        {
            await _eventAggregator.GetEvent<AnimationStartEvent>().Publish(update);
        }
        private void ProcessLevelEvent(LevelEventData update)
        {
            _eventAggregator.GetEvent<LevelEvent>().Publish(update);
        }
        private bool ProcessScenarioEvent(ScenarioEventData update)
        {
            _eventAggregator.GetEvent<ScenarioEvent>().Publish(update);

            switch (update.ScenarioUpdateType)
            {
                case ScenarioUpdateType.LevelChange:
                case ScenarioUpdateType.PlayerDeath:
                    return false;
            }

            return true;
        }
        private void ProcessSplashUpdate(SplashEventData update)
        {
            // Synchronous Show / Hide
            _eventAggregator.GetEvent<SplashEvent>().Publish(update);
        }
        private void ProcessDialogEvent(DialogEventData update)
        {
            // Synchronous events for showing a "Dialog"
            _eventAggregator.GetEvent<DialogEvent>().Publish(update);
        }
        #endregion
    }
}
