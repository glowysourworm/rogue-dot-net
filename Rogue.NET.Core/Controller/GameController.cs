using Rogue.NET.Core.Controller.Interface;
using System.ComponentModel.Composition;

namespace Rogue.NET.Core.Controller
{
    [Export(typeof(IGameController))]
    public class GameController : IGameController
    {
    }
}
