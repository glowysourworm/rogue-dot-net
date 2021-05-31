using KellermanSoftware.CompareNetObjects;

using Rogue.NET.Common.Extension;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.NET.UnitTest.Extension
{
    /// <summary>
    /// Extension of KellermanSoftware "Difference" for serialization
    /// </summary>
    [Serializable]
    public class CompareDifference
    {
        public string ChildPropertyName { get; set; }
        public string Object2TypeName { get; set; }
        public string Object1TypeName { get; set; }
        public string Object2Value { get; set; }
        public string Object1Value { get; set; }
        public string MessagePrefix { get; set; }
        public string PropertyName { get; set; }
        public string ParentPropertyName { get; set; }
        public string ActualName { get; set; }
        public string ExpectedName { get; set; }

        public CompareDifference() { }

        public CompareDifference(Difference difference)
        {
            difference.MapOnto(this);

            this.ParentPropertyName = difference.ParentPropertyName;
        }
    }
}
