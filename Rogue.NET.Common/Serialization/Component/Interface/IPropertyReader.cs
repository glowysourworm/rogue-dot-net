using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.NET.Common.Serialization.Component.Interface
{
    public interface IPropertyReader
    {
        T Read<T>(string propertyName);
    }
}
