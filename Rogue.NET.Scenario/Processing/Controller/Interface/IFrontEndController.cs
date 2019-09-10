using Rogue.NET.Common.Extension.Event;
using Rogue.NET.Core.Processing.Command.Frontend.Data;
using Rogue.NET.Core.Processing.Event.Backend.EventData;
using System.Threading.Tasks;

namespace Rogue.NET.Scenario.Processing.Controller.Interface
{
    /// <summary>
    /// Component responsible for handling front-end calculations using view-model components
    /// </summary>
    public interface IFrontendController
    {
        Task PublishCommand(FrontendCommandData commandData);
    }
}
