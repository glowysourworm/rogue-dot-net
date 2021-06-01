using Rogue.NET.Common.Serialization.Planning;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.NET.Common.Serialization.Interface
{
    /// <summary>
    /// Provides custom serialization control for use with the RecursiveSerializer.
    /// </summary>
    public interface IRecursiveSerializable
    {
        /// <summary>
        /// Method to define property that are later to be stored
        /// </summary>
        void GetPropertyDefinitions(IPropertyPlanner planner);

        /// <summary>
        /// Method used to store properties to the underlying serialization stream
        /// </summary>
        void GetProperties(IPropertyWriter writer);

        /// <summary>
        /// Method used to set properties from the underlying serialization stream
        /// </summary>
        void SetProperties(IPropertyReader reader);
    }
}
