using Rogue.NET.Common.Collection;
using Rogue.NET.Core.Model.Enums;
using System;

namespace Rogue.NET.Core.Model.Scenario.Content.Doodad
{
    [Serializable]
    public class DoodadNormal : DoodadBase
    {
        public DoodadNormalType NormalType { get; set; }
        
        /// <summary>
        /// Set of transport group id's for transporters. This is kept ordered by the
        /// simple ordered list API. It contains connecting transporters AND this Id.
        /// </summary>
        public SimpleOrderedList<string> TransportGroupIds { get; set; }

        public DoodadNormal()
        {
            this.TransportGroupIds = new SimpleOrderedList<string>();
        }
    }
}
