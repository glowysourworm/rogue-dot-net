using Rogue.NET.Core.Model.Scenario.Content.Layout;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.NET.Core.Processing.Model.Generator.Component
{
    public class RoomEqualityComparer : IEqualityComparer<Room>
    {
        public bool Equals(Room room1, Room room2)
        {
            if (room1 == null)
                return room2 == null;

            else if (room2 == null)
                return false;

            var center1 = room1.Bounds.Center;
            var center2 = room2.Bounds.Center;

            if (center1 == GridLocation.Empty)
                return center2 == GridLocation.Empty;

            else if (center2 == GridLocation.Empty)
                return false;

            return center1.Equals(center2);
        }

        public int GetHashCode(Room obj)
        {
            return obj.GetHashCode();
        }
    }
}
