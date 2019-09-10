using Rogue.NET.Common.Extension.Event;
using Rogue.NET.Common.Extension.Prism.EventAggregator;
using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Processing.Command.Backend;
using Rogue.NET.Core.Processing.Command.Backend.CommandData;
using Rogue.NET.Core.Processing.Command.Frontend;
using Rogue.NET.Core.Processing.Command.Frontend.Data;
using Rogue.NET.Core.Processing.Command.Frontend.Enum;
using Rogue.NET.Core.Processing.Command.View;
using Rogue.NET.Core.Processing.Event.Backend.Enum;
using Rogue.NET.Core.Processing.Event.Backend.EventData;
using Rogue.NET.Scenario.Processing.Controller.Enum;
using Rogue.NET.Scenario.Processing.Controller.Interface;
using Rogue.NET.Scenario.Processing.Service.Interface;
using System;
using System.ComponentModel.Composition;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Rogue.NET.Scenario.Processing.Controller
{
    [PartCreationPolicy(CreationPolicy.Shared)]
    [Export(typeof(ICommandRouter))]
    public class CommandRouter : ICommandRouter
    {
        readonly IRogueEventAggregator _eventAggregator;
        readonly IBackendController _backendController;
        readonly IFrontendController _frontendController;
        readonly IKeyResolver _keyResolver;

        public event SimpleEventHandler RequestMaximizedWindowEvent;

        public GameCommandMode CommandMode { get; set; }

        // TARGET MODE STORED STATE - MUST BE CAREFULLY MAINTAINED
        TargetRequestEventData _targetRequestEventData;

        [ImportingConstructor]
        public CommandRouter(IRogueEventAggregator eventAggregator,
                             IBackendController backendController,
                             IFrontendController frontendController,
                             IKeyResolver keyResolver)
        {
            _eventAggregator = eventAggregator;
            _backendController = backendController;
            _frontendController = frontendController;
            _keyResolver = keyResolver;

            // Block user commands until owning component calls "Start()"
            this.CommandMode = GameCommandMode.Blocked;

            InitializeEvents();
        }

        protected void InitializeEvents()
        {
            // *** Listen for backend COMMANDS (NOT EVENTS) here - and route them appropriately
            //
            _eventAggregator.GetEvent<LevelCommand>()
                            .Subscribe(async (e) =>
                            {
                                // For Frontend Mode - Must end targeting for commands published from
                                // the UI (Item Grid)
                                if (this.CommandMode == GameCommandMode.FrontendCommand)
                                {
                                    // Set Backend mode
                                    this.CommandMode = GameCommandMode.BackendCommand;

                                    // Select Target (OR) End Targeting - Depends on if data was stored
                                    if (_targetRequestEventData == null)
                                        await _frontendController.PublishCommand(new FrontendCommandData(FrontendCommandType.EndTargeting, Compass.Null));

                                    else
                                        await _frontendController.PublishCommand(new FrontendCommandData(FrontendCommandType.SelectTarget, Compass.Null));

                                    // Clear targeting data (Backend data required will be forwarded from the LevelCommand)
                                    _targetRequestEventData = null;
                                }

                                await _backendController.PublishLevelCommand(e);
                            });

            _eventAggregator.GetEvent<PlayerCommand>()
                            .Subscribe(async (e) => await _backendController.PublishPlayerCommand(e));

            _eventAggregator.GetEvent<PlayerMultiItemCommand>()
                            .Subscribe(async (e) => await _backendController.PublishPlayerMultiItemCommand(e));

            _backendController.TargetingModeRequestEvent += (eventData) =>
            {
                // Enter frontend mode
                this.CommandMode = GameCommandMode.FrontendCommand;

                // Save the request data
                _targetRequestEventData = eventData;

                // Start targeting
                _frontendController.PublishCommand(new FrontendCommandData(FrontendCommandType.StartTargeting, Compass.Null));
            };
        }

        public void Stop()
        {
            // First, block user inputs
            this.CommandMode = GameCommandMode.Blocked;

            // Second, clear and unload all backend queues
            _backendController.Clear();
        }
        public void Start()
        {
            // Processing begins with issuing backend commands
            this.CommandMode = GameCommandMode.BackendCommand;
        }

        public Task IssueCommand(Key key, bool shift, bool ctrl, bool alt)
        {
            switch (this.CommandMode)
            {
                case GameCommandMode.FrontendCommand:
                    {
                        // Frontend Command
                        var frontendCommand = _keyResolver.ResolveFrontendCommand(key, shift, ctrl, alt);

                        // Backend Command
                        var backendCommand = _keyResolver.ResolveLevelCommand(key, shift, ctrl, alt);

                        // DEPENDING ON FRONTEND COMMAND / BACKEND COMMAND
                        //
                        if (frontendCommand != null)
                        {
                            // Select Target -> Set Backend Mode -> Publish Stored Request
                            if (frontendCommand.Type == FrontendCommandType.SelectTarget)
                            {
                                // Set Backend Mode
                                this.CommandMode = GameCommandMode.BackendCommand;

                                // If there was a pending request for a target - then publish the appropriate
                                // backend command and also: Frontend -> Select Target
                                if (_targetRequestEventData != null)
                                {
                                    // Select Target
                                    _frontendController.PublishCommand(frontendCommand);

                                    Task task = null;

                                    switch (_targetRequestEventData.Type)
                                    {
                                        case TargetRequestType.Consume:
                                            task = _backendController.PublishLevelCommand(new LevelCommandData(LevelCommandType.Consume, Compass.Null, _targetRequestEventData.AssociatedId));
                                            break;
                                        case TargetRequestType.Throw:
                                            task = _backendController.PublishLevelCommand(new LevelCommandData(LevelCommandType.Throw, Compass.Null, _targetRequestEventData.AssociatedId));
                                            break;
                                        case TargetRequestType.Fire:
                                            task = _backendController.PublishLevelCommand(new LevelCommandData(LevelCommandType.Fire, Compass.Null, _targetRequestEventData.AssociatedId));
                                            break;
                                        case TargetRequestType.InvokeSkill:
                                            task = _backendController.PublishLevelCommand(new LevelCommandData(LevelCommandType.InvokeSkill, Compass.Null, _targetRequestEventData.AssociatedId));
                                            break;
                                        default:
                                            throw new Exception("Unhandled Target Request Type");
                                    }

                                    _targetRequestEventData = null;
                                    return task;
                                }

                                // No Stored Request -> End Targeting
                                else
                                    return _frontendController.PublishCommand(new FrontendCommandData(FrontendCommandType.EndTargeting, Compass.Null));
                            }

                            // Clear out targeting data before sending command
                            else if (frontendCommand.Type == FrontendCommandType.EndTargeting)
                            {
                                _targetRequestEventData = null;

                                this.CommandMode = GameCommandMode.BackendCommand;

                                return _frontendController.PublishCommand(frontendCommand);
                            }

                            // Else, send frontend
                            else
                                return _frontendController.PublishCommand(frontendCommand);
                        }

                        // Send backend command only if level action matches one of the targeting actions.
                        //
                        // Consume / Throw invoked from the item grid -> Level Command (see above)
                        else if (backendCommand != null &&
                                (backendCommand.LevelAction == LevelCommandType.Fire ||
                                 backendCommand.LevelAction == LevelCommandType.InvokeSkill))
                        {
                            // Reset target request data
                            _targetRequestEventData = null;

                            // Set backend mode
                            this.CommandMode = GameCommandMode.BackendCommand;

                            // Publsih frontend command to stop processing
                            _frontendController.PublishCommand(new FrontendCommandData(FrontendCommandType.SelectTarget, Compass.Null));

                            // Publish backend command
                            return _backendController.PublishLevelCommand(backendCommand);
                        }

                        return Task.Delay(1);
                    }
                case GameCommandMode.BackendCommand:
                    {
                        // Escape from Full-Screen Mode
                        if (key == Key.Escape)
                        {
                            OnRequestMaximizedWindow();
                            return Task.Delay(1);
                        }

                        // Commands: { Backend, Frontend, View }
                        //
                        // Try to resolve each type to see what it was - and
                        // publish command for the current game mode

                        // Resolve Backend Command (First)
                        var levelCommand = _keyResolver.ResolveLevelCommand(key, shift, ctrl, alt);
                        var frontendCommand = _keyResolver.ResolveFrontendCommand(key, shift, ctrl, alt);

                        if (frontendCommand != null &&
                            frontendCommand.Type == FrontendCommandType.StartTargeting)
                        {
                            // Enter Frontend Command Mode
                            this.CommandMode = GameCommandMode.FrontendCommand;

                            // Publish Command
                            return _frontendController.PublishCommand(frontendCommand);
                        }

                        // Level Command
                        if (levelCommand != null)
                            return _backendController.PublishLevelCommand(levelCommand);

                        // Player Command (Originates from Dialog / UI)
                        var playerCommand = _keyResolver.ResolvePlayerCommand(key, shift, ctrl, alt);
                        if (playerCommand != null)
                            return _backendController.PublishPlayerCommand(playerCommand);

                        // Resolve View Command (Second)
                        var viewCommand = _keyResolver.ResolveViewCommand(key, shift, ctrl, alt);

                        // Only publish this command during Backend Mode
                        if (viewCommand != null)
                            return _eventAggregator.GetEvent<ViewCommand>().Publish(viewCommand);

                        return Task.Delay(1);
                    }
                case GameCommandMode.DialogCommand:
                    return Task.Delay(1);

                case GameCommandMode.Blocked:
                    // Escape from Full-Screen Mode
                    if (key == Key.Escape)
                        OnRequestMaximizedWindow();

                    return Task.Delay(1);
                default:
                    throw new Exception("Unknown Game Mode Command Type");
            }
        }

        private void OnRequestMaximizedWindow()
        {
            if (this.RequestMaximizedWindowEvent != null)
                this.RequestMaximizedWindowEvent();
        }
    }
}
