using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.NET.Common.Serialization.Target
{
    /// <summary>
    /// Specifies how elements can be ADDED to the collection
    /// </summary>
    public enum CollectionInterfaceType : byte
    {
        IList = 0,
        IDictionary = 1
    }
}
