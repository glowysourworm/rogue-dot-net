using Rogue.NET.Common.Extension.Event;
using Rogue.NET.Core.Processing.Command.Backend.CommandData;
using Rogue.NET.Core.Processing.Event.Backend.EventData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.NET.Scenario.Processing.Controller.Interface
{
    /// <summary>
    /// Component that works with the IScenarioService to process primary game queues { Animation, UI, Dialog, and Data }
    /// </summary>
    public interface IBackendController
    {
        /// <summary>
        /// Event signaling targeting mode request
        /// </summary>
        event SimpleEventHandler<TargetRequestEventData> TargetingModeRequestEvent;

        /// <summary>
        /// Clears backend queues to prepare for loading new model
        /// </summary>
        void Clear();

        /// <summary>
        /// Publishes async level command data to the backend components
        /// </summary>
        Task PublishLevelCommand(LevelCommandData commandData);

        /// <summary>
        /// Publishes async player command data to the backend components
        /// </summary>
        Task PublishPlayerCommand(PlayerCommandData commandData);

        /// <summary>
        /// Publishes async player multi item command data to the backend components
        /// </summary>
        Task PublishPlayerMultiItemCommand(PlayerMultiItemCommandData commandData);
    }
}
