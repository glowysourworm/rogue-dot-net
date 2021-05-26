using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.NET.Common.Serialization.Interface
{
    /// <summary>
    /// Convenience interface for BaseFormatter implementations
    /// </summary>
    internal interface IBaseFormatter
    {
        object Read(Stream stream);

        void Write(Stream stream, object theObject);
    }
}
