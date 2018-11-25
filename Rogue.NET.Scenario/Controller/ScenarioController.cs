using Prism.Events;
using Rogue.NET.Common.EventArgs;
using Rogue.NET.Common.Events.Scenario;
using Rogue.NET.Common.Events.Splash;
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
using System.Threading;
using System.Windows;

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
        readonly IFrontEndController _frontEndController;

        // NOTE*** This is required because the Publish / Subscribe mechanism for this IEventAggregator isn't
        //         working!!!  I need to be able to process animation "synchronously". I tried the following:
        //
        //         1) async / await Subscribe to Animation Event - works! (i was amazed..)
        //         2) Set PublisherThread option on Subscribe to force wait on the calling thread (didn't work)
        //         3) Tried Un-subscribe in the IScenarioController to force blocking of events from user input (DIDN'T WORK!!)
        //         4) Also tried keepSubscriberReferenceAlive = false for UserCommand events (DIDN'T WORK!)
        //
        //         So... Am resorting to using a separate backend thread.
        Thread _backendThread;
        bool _backendThreadWork = false;

        const int FRONTEND_WAIT = 50;

        /// <summary>
        /// Allow a single user command to be stored. When background thread is idle - process the command
        /// and set to null.
        /// </summary>
        LevelCommandAction _userCommand;

        object _backendThreadInterruptLock = new object();
        object _userCommandLock = new object();

        [ImportingConstructor]
        public ScenarioController(
            IEventAggregator eventAggregator, 
            IFrontEndController frontEndController,
            IScenarioService scenarioService)
        {
            _eventAggregator = eventAggregator;
            _scenarioService = scenarioService;
            _frontEndController = frontEndController;
        }

        public void Initialize()
        {
            // Subscribe to user input
            _eventAggregator.GetEvent<UserCommandEvent>().Subscribe(OnUserCommand, true);
        }
        public void Start()
        {
            // Close the backend if running
            Shutdown();

            _backendThreadWork = true;         // This operation is thread-safe because backend thread not running
            _backendThread = new Thread(Work);
            _backendThread.Start();
        }
        public void Stop()
        {
            Shutdown();
        }

        private void OnUserCommand(LevelCommandEventArgs e)
        {
            if (!_backendThreadWork)
                return;

            lock (_userCommandLock)
            {
                _userCommand = new LevelCommandAction()
                {
                    Action = e.Action,
                    Direction = e.Direction,
                    ScenarioObjectId = e.ItemId
                };
            }
        }

        /// <summary>
        /// Primary dispatch method for the backend thread
        /// </summary>
        private void Work()
        {
            var processing = true;

            // 0) Frontend Processing (Wait)
            // 1) Backend Process 1 Message
            // 2) If (no further processing of either) Then (issue user command)
            while (processing)
            {
                // First: Process all animations
                while (_scenarioService.AnyAnimationEvents())
                    ProcessAnimationUpdate(_scenarioService.DequeueAnimationUpdate());

                // Second: Process all Splash events
                while (_scenarioService.AnySplashEvents())
                    ProcessSplashUpdate(_scenarioService.DequeueSplashUpdate());

                // Third: Process all UI events
                while (_scenarioService.AnyLevelEvents())
                    ProcessUIUpdate(_scenarioService.DequeueLevelUpdate());

                // Fourth: Process all Scenario Events
                //
                // NOTE*** Level change events require unloading the backend. This will cause an
                //         interrupt in the thread. To avoid a deadlock - must do this after processing
                //         anything on the UI Thread - which waits during _backendThread.Join() until 
                //         processing is finished (deadlock)
                //
                // UPDATE  Changed Invoke of the Dispatcher to BeginInvoke to avoid this issue. Appears to
                //         be working fine. So, may not require such careful attention here.
                while (_scenarioService.AnyScenarioEvents())
                    ProcessScenarioUpdate(_scenarioService.DequeueScenarioUpdate());

                // Finally: Process the next backend work item. (If processing finished - then allow user command)
                if (!_scenarioService.ProcessBackend() /*&& processing*/)
                {
                    // Lock user command resource and wait to issue
                    lock (_userCommandLock)
                    {
                        if (_userCommand != null)
                        {
                            // IssueCommand -> Fills up IScenarioService queues. Example: 0) Player moves 1) Enemy reactions queued 
                            // Processing involves checking animation and UI update queues - then continuing
                            // with backend logic
                            _scenarioService.IssueCommand(_userCommand);

                            // NOTE** This signals the main thread that the command has been processed
                            _userCommand = null;
                        }
                    }
                }

                // Check the interrupt flag
                lock (_backendThreadInterruptLock)
                {
                    processing = _backendThreadWork;
                }

                Thread.Sleep(1);
            }
        }

        /// <summary>
        /// Closes the backend and Joins the backend thread
        /// </summary>
        private void Shutdown()
        {
            // First, stop execution of backend thread
            if (_backendThread != null)
            {
                // Send message to the thread to finish up final execution loop
                InterruptBackendThread();

                // Safely join the thread
                _backendThread.Join();
                _backendThread = null;

                // Also Safely set user command null
                _userCommand = null;
            }

            // Second, clear and unload all backend queues. (This is safe because backend thread is halted)
            _scenarioService.ClearQueues();
        }

        /// <summary>
        /// Sends message to backend thread to halt processing on next iteration
        /// </summary>
        private void InterruptBackendThread()
        {
            lock (_backendThreadInterruptLock)
            {
                _backendThreadWork = false;
            }
        }

        #region (private) Processing
        private void ProcessAnimationUpdate(IAnimationUpdate update)
        {
            // Post message to front end and tell it to process
            //
            // NOTE*** Posting message on the dispatch thread but reading output from this thread. For full
            //         Dispatcher / worker scenario - would normally have separate Frontend thread to manage
            //         its queue. 
            InvokeDispatcher(() =>
            {
                _frontEndController.PostInputMessage(update);
            });

            // Wait for message to return
            while (_frontEndController.ReadOutputMessage<IAnimationUpdate>() != update)
                Thread.Sleep(FRONTEND_WAIT);
        }
        private void ProcessUIUpdate(ILevelUpdate update)
        {
            InvokeDispatcher(() =>
            {
                // NOTE*** Fire and forget
                _eventAggregator.GetEvent<LevelUpdateEvent>().Publish(update);
            });
        }
        private void ProcessScenarioUpdate(IScenarioUpdate update)
        {
            InvokeDispatcher(() =>
            {
                // NOTE*** Fire and forget
                _eventAggregator.GetEvent<ScenarioUpdateEvent>().Publish(update);
            });
        }
        private void ProcessSplashUpdate(ISplashUpdate update)
        {
            // Post message to front end and tell it to process
            //
            // NOTE*** Posting message on the dispatch thread but reading output from this thread. For full
            //         Dispatcher / worker scenario - would normally have separate Frontend thread to manage
            //         its queue. 
            InvokeDispatcher(() =>
            {
                _frontEndController.PostInputMessage(update);
            });

            // Wait for message to return
            while (_frontEndController.ReadOutputMessage<ISplashUpdate>() != update)
                Thread.Sleep(FRONTEND_WAIT);
        }

        private void InvokeDispatcher(Action action)
        {
            Application.Current.Dispatcher.BeginInvoke(action);
        }
        #endregion
    }
}
