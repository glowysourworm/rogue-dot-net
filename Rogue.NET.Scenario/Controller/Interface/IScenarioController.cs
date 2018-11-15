using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.NET.Scenario.Controller.Interface
{
    /// <summary>
    /// Component that works with the IScenarioService to process primary game queues { Animation, UI, and Data }
    /// </summary>
    public interface IScenarioController
    {
        /// <summary>
        /// Subscribes to user commands
        /// </summary>
        void Initialize();

        /// <summary>
        /// Starts primary worker process to accept user commands and process backend queues
        /// </summary>
        void Start();

        /// <summary>
        /// Stops primary worker process
        /// </summary>
        void Stop();
    }
}
