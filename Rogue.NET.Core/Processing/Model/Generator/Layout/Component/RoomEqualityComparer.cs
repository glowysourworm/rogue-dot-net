using Rogue.NET.Core.Model.Scenario.Content.Layout;
using System.Collections.Generic;

namespace Rogue.NET.Core.Processing.Model.Generator.Layout.Component
{
    public class RoomEqualityComparer : IEqualityComparer<Core.Model.Scenario.Content.Layout.Region>
    {
        public bool Equals(Core.Model.Scenario.Content.Layout.Region room1, Core.Model.Scenario.Content.Layout.Region room2)
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

        public int GetHashCode(Core.Model.Scenario.Content.Layout.Region obj)
        {
            return obj.GetHashCode();
        }
    }
}
