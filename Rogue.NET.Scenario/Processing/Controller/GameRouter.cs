using Rogue.NET.Common.Extension.Event;
using Rogue.NET.Common.Extension.Prism.EventAggregator;
using Rogue.NET.Core.Processing.Command.Backend;
using Rogue.NET.Core.Processing.Command.Frontend;
using Rogue.NET.Core.Processing.Command.View;
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
    [Export(typeof(IGameRouter))]
    public class GameRouter : IGameRouter
    {
        readonly IRogueEventAggregator _eventAggregator;
        readonly IKeyResolver _keyResolver;

        public event SimpleEventHandler RequestMaximizedWindowEvent;

        public GameCommandMode CommandMode { get; private set; }

        [ImportingConstructor]
        public GameRouter(IRogueEventAggregator eventAggregator,
                          IKeyResolver keyResolver)
        {
            _eventAggregator = eventAggregator;
            _keyResolver = keyResolver;

            this.CommandMode = GameCommandMode.BackendCommand;
        }

        public Task IssueCommand(Key key, bool shift, bool ctrl, bool alt)
        {
            switch (this.CommandMode)
            {
                case GameCommandMode.FrontendCommand:
                    {
                        // Escape from Front-end mode [ End-Targeting, ... ]
                        if (key == Key.Escape)
                        {
                            return Task.Delay(1);
                        }

                        // Frontend Command
                        var frontendCommand = _keyResolver.ResolveFrontendCommand(key, shift, ctrl, alt);

                        // Only publish this command during Frontend Mode
                        if (frontendCommand != null)
                            return _eventAggregator.GetEvent<FrontendCommand>().Publish(frontendCommand);

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

                        // Only publish this command during Backend Mode
                        if (levelCommand != null)
                            return _eventAggregator.GetEvent<LevelCommand>().Publish(levelCommand);

                        // Resolve Backend Command (Player Command)
                        var playerCommand = _keyResolver.ResolvePlayerCommand(key, shift, ctrl, alt);
                        if (playerCommand != null)
                            return _eventAggregator.GetEvent<PlayerCommand>().Publish(playerCommand);

                        // Resolve View Command (Second)
                        var viewCommand = _keyResolver.ResolveViewCommand(key, shift, ctrl, alt);

                        // Only publish this command during Backend Mode
                        if (viewCommand != null)
                            return _eventAggregator.GetEvent<ViewCommand>().Publish(viewCommand);

                        return Task.Delay(1);
                    }
                default:
                    throw new Exception("Unknown Game Mode Command Type");
            }
        }

        void OnRequestMaximizedWindow()
        {
            if (this.RequestMaximizedWindowEvent != null)
                this.RequestMaximizedWindowEvent();
        }
    }
}
