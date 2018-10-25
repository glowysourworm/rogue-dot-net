using Rogue.NET.Core.Event;
using Rogue.NET.Core.Model.Common.Interface;
using Rogue.NET.Core.Model.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.NET.Engine.Service.Interface
{
    /// <summary>
    /// Component has the responsibility of providing methods to interact with scenario model via 
    /// the Engine components
    /// </summary>
    public interface IScenarioService
    {
        /// <summary>
        /// Issues primary player command 
        /// </summary>
        /// <param name="action">Intended action</param>
        /// <param name="direction">desired direction for action</param>
        /// <param name="id">involved RogueBase.Id for action</param>
        void IssueCommand(ILevelCommand levelCommand);

        void QueueLevelLoadRequest(int levelNumber, PlayerStartLocation startLocation);

        void QueueSplashScreenEvent(SplashEventType type);
    }
}
