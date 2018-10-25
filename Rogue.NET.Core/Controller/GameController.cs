using Rogue.NET.Engine.Controller.Interface;
using System.ComponentModel.Composition;

namespace Rogue.NET.Engine.Controller
{
    [Export(typeof(IGameController))]
    public class GameController : IGameController
    {
    }
}
