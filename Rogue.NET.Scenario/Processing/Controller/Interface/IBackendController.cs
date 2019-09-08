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
        /// Reset backend queues for loading new data model; and blocks user input events
        /// </summary>
        void Stop();

        /// <summary>
        /// Enables user input events
        /// </summary>
        void Start();
    }
}
