using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.NET.Common.Serialization.Target
{
    internal class SerializationNullObject : SerializationObjectBase
    {
        public SerializationNullObject(HashedObjectInfo nullInfo) : base(nullInfo)
        {
        }

        internal override IEnumerable<PropertyStorageInfo> GetProperties(PropertyReader reader)
        {
            throw new Exception("Invalid use of GetProperties for null-referenced object");
        }
    }
}
