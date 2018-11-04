using Rogue.NET.Core.Logic.Processing.Enum;
using Rogue.NET.Core.Logic.Processing.Interface;
using Rogue.NET.Core.Model.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.NET.Core.Logic.Processing
{
    public class ScenarioUpdate : IScenarioUpdate
    {
        public ScenarioUpdateType ScenarioUpdateType { get; set; }

        public string PlayerDeathMessage { get; set; }

        public int LevelNumber { get; set; }
        public PlayerStartLocation StartLocation { get; set; }
    }
}
