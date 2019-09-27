using Rogue.NET.Core.Model.Enums;
using System;

namespace Rogue.NET.Core.Model.Scenario.Content.Doodad
{
    [Serializable]
    public class DoodadNormal : DoodadBase
    {
        public string PairId { get; set; }
        public DoodadNormalType NormalType { get; set; }

        public DoodadNormal()
        {
        }
    }
}
