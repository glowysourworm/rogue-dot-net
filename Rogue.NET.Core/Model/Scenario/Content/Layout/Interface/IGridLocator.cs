using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.NET.Core.Model.Scenario.Content.Layout.Interface
{
    /// <summary>
    /// Interface for related 2D array objects that have a location on a 2D array or a member that has
    /// some location specified by 2 indices.
    /// </summary>
    public interface IGridLocator : ISerializable
    {
        int Column { get; }
        int Row { get; }

        /// <summary>
        /// Override method for object.Equals(...)
        /// </summary>
        bool Equals(object obj);
    }
}
