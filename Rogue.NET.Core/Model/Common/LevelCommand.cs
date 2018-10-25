using Rogue.NET.Core.Model.Common.Interface;
using Rogue.NET.Core.Model.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.NET.Core.Model.Common
{
    [Export(typeof(ILevelCommand))]
    public class LevelCommand : ILevelCommand
    {
        public LevelAction Action { get; set; }
        public Compass Direction { get; set; }
        public string ScenarioObjectId { get; set; }
    }
}
