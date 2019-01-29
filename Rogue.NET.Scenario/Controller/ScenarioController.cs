﻿using Prism.Events;
using Rogue.NET.Common.Events.Scenario;
using Rogue.NET.Core.Event.Scenario.Level.Event;
using Rogue.NET.Core.Event.Splash;
using Rogue.NET.Core.Logic.Processing;
using Rogue.NET.Core.Logic.Processing.Enum;
using Rogue.NET.Core.Logic.Processing.Interface;
using Rogue.NET.Core.Service.Interface;
using Rogue.NET.Model.Events;
using Rogue.NET.Scenario.Controller.Interface;
using System;
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
            _eventAggregator.GetEvent<UserCommandEvent>().Subscribe(async (e) => await OnUserCommand(e));
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
                Action = e.LevelAction,
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
                // High Priority
                while (_scenarioService.AnyUpdates(RogueUpdatePriority.High))
                    await ProcessUpdate(_scenarioService.DequeueUpdate(RogueUpdatePriority.High));

                // Low Priority
                while (_scenarioService.AnyUpdates(RogueUpdatePriority.Low))
                    await ProcessUpdate(_scenarioService.DequeueUpdate(RogueUpdatePriority.Low));
                
                // Fifth: Process all Scenario Events
                while (_scenarioService.AnyUpdates(RogueUpdatePriority.Critical))
                {
                    // Have to cancel processing on certain scenario events
                    processing = await ProcessUpdate(_scenarioService.DequeueUpdate(RogueUpdatePriority.Critical));
                }

                // Finally: Process the next backend work item. (If processing finished - then allow user command)
                if (!_scenarioService.ProcessBackend() && processing)
                    processing = false;
            }
        }

        private async Task<bool> ProcessUpdate(IRogueUpdate update)
        {
            if (update is IAnimationUpdate)
            {
                await ProcessAnimationUpdate(update as IAnimationUpdate);
                return true;
            }
            else if (update is ILevelUpdate)
            {
                ProcessUIUpdate(update as ILevelUpdate);
                return true;
            }
            else if (update is IScenarioUpdate)
            {
                return ProcessScenarioUpdate(update as IScenarioUpdate);
            }
            else if (update is ISplashUpdate)
            {
                ProcessSplashUpdate(update as ISplashUpdate);
                return true;
            }
            else if (update is IDialogUpdate)
            {
                ProcessDialogUpdate(update as IDialogUpdate);
                return true;
            }
            else
                throw new Exception("Unknown IRogueUpdate Type");
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
        private bool ProcessScenarioUpdate(IScenarioUpdate update)
        {
            _eventAggregator.GetEvent<ScenarioUpdateEvent>().Publish(update);

            switch (update.ScenarioUpdateType)
            {
                case ScenarioUpdateType.LevelChange:
                case ScenarioUpdateType.PlayerDeath:
                    return false;
            }

            return true;
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
