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
        public DoodadNormal(DoodadNormalType type, string name, string pairId)
            : base(name, type == DoodadNormalType.SavePoint ? ImageResources.SavePoint :
                         type == DoodadNormalType.StairsDown ? ImageResources.StairsDown :
                         type == DoodadNormalType.StairsUp ? ImageResources.StairsUp :
                         type == DoodadNormalType.Teleport1 ? ImageResources.teleport1 :
                         type == DoodadNormalType.Teleport2 ? ImageResources.teleport2 :
                         type == DoodadNormalType.TeleportRandom ? ImageResources.TeleportRandom :
                         ImageResources.AmuletBlack)
        {
            this.Type = DoodadType.Normal;
            this.NormalType = type;
            this.PairId = pairId;
        }
    }
}
