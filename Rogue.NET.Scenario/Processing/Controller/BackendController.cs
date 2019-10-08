using Rogue.NET.Common.Extension.Event;
using Rogue.NET.Common.Extension.Prism.EventAggregator;
using Rogue.NET.Core.GameRouter.GameEvent.Backend.Enum;
using Rogue.NET.Core.Processing.Command.Backend.CommandData;
using Rogue.NET.Core.Processing.Event.Backend;
using Rogue.NET.Core.Processing.Event.Backend.EventData;
using Rogue.NET.Core.Processing.Service.Interface;
using Rogue.NET.Scenario.Processing.Controller.Interface;
using System.ComponentModel.Composition;
using System.Threading.Tasks;

namespace Rogue.NET.Scenario.Processing.Controller
{
    [Export(typeof(IBackendController))]
    public class BackendController : IBackendController
    {
        readonly IRogueEventAggregator _eventAggregator;
        readonly IScenarioService _scenarioService;

        public event SimpleEventHandler<TargetRequestEventData> TargetingModeRequestEvent;

        [ImportingConstructor]
        public BackendController(
            IRogueEventAggregator eventAggregator, 
            IScenarioService scenarioService)
        {
            _eventAggregator = eventAggregator;
            _scenarioService = scenarioService;
        }

        public void Clear()
        {
            _scenarioService.ClearQueues();
        }

        public async Task PublishLevelCommand(LevelCommandData e)
        {
            // First issue command
            _scenarioService.IssueCommand(e);

            await Work();
        }

        public async Task PublishPlayerCommand(PlayerCommandData e)
        {
            // First issue command
            _scenarioService.IssuePlayerCommand(e);

            await Work();
        }

        public async Task PublishPlayerMultiItemCommand(PlayerMultiItemCommandData e)
        {
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
                // Projectile Animations (Throwing, Ammo)

                var projectileAnimationData = _scenarioService.DequeueProjectileAnimationEventData();
                while (projectileAnimationData != null)
                {
                    // Process Animation (await)
                    await ProcessProjectileAnimationEvent(projectileAnimationData);

                    // Dequeue Next Animation
                    projectileAnimationData = _scenarioService.DequeueProjectileAnimationEventData();
                }

                // Animations (Alterations)

                var animationData = _scenarioService.DequeueAnimationEventData();
                while (animationData != null)
                {
                    // Process Animation (await)
                    await ProcessAnimationEvent(animationData);

                    // Dequeue Next Animation
                    animationData = _scenarioService.DequeueAnimationEventData();
                }

                // Level Event

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

            // CHECK FOR TARGETING MODE REQUEST
            //
            // NOTE** These should be queued with no other backend processing to do. But, 
            //        the backend sequencing needs to be refactored anyway... For now, just
            //        leave it here or wherever it works.
            //
            //        Have to halt processing - which means this can only be processed after
            //        everything else has finished. So, clear the queues and publish the request.

            // Target Request Event
            var targetRequestEventData = _scenarioService.DequeueTargetRequestEventData();
            if (targetRequestEventData != null)
            {
                // Have to halt processing - which means clear the queues to be sure no other
                // target requests have been added (THIS NEEDS TO BE REFACTORED - CREATE THE BACKEND SEQUENCER!)
                _scenarioService.ClearQueues();

                // Process Level Update (await)
                if (this.TargetingModeRequestEvent != null)
                    this.TargetingModeRequestEvent(targetRequestEventData);
            }
        }

        #region (private) Processing
        private async Task ProcessAnimationEvent(AnimationEventData eventData)
        {
            await _eventAggregator.GetEvent<AnimationStartEvent>().Publish(eventData);
        }
        private async Task ProcessProjectileAnimationEvent(ProjectileAnimationEventData eventData)
        {
            await _eventAggregator.GetEvent<ProjectileAnimationStartEvent>().Publish(eventData);
        }
        private void ProcessLevelEvent(LevelEventData eventData)
        {
            _eventAggregator.GetEvent<LevelEvent>().Publish(eventData);
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
