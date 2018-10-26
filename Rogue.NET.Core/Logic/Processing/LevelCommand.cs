using Rogue.NET.Core.Logic.Processing.Interface;
using Rogue.NET.Core.Model.Enums;
using System.ComponentModel.Composition;

namespace Rogue.NET.Core.Logic.Processing
{
    [Export(typeof(ILevelCommand))]
    public class LevelCommand : ILevelCommand
    {
        public LevelAction Action { get; set; }
        public Compass Direction { get; set; }
        public string ScenarioObjectId { get; set; }
    }
}
