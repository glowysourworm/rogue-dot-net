using Rogue.NET.Core.Logic.Processing.Interface;
using Rogue.NET.Core.Model.Enums;
using System.ComponentModel.Composition;

namespace Rogue.NET.Core.Logic.Processing
{
    [Export(typeof(ILevelCommandAction))]
    public class LevelCommandAction : ILevelCommandAction
    {
        public LevelActionType Action { get; set; }
        public Compass Direction { get; set; }
        public string Id { get; set; }
    }
}
